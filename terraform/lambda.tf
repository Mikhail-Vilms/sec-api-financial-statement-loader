data "archive_file" "lambda_zip_payload" {
    type = "zip"
    source_dir = "../SecApiReportStructureLoader/bin/Debug/netcoreapp3.1/"
    output_path = "lambda_payload.zip"
}

resource "aws_lambda_function" "sec-api-report-structure-loader-lambda" {
    filename = data.archive_file.lambda_zip_payload.output_path
    function_name = "Sec-Api-Report-Structure-Loader"
    handler = "SecApiReportStructureLoader::SecApiReportStructureLoader.Function::FunctionHandler"
    runtime = "dotnetcore3.1"
    role = aws_iam_role.sec-api-report-structure-loader-lambda-exec-role.arn
    source_code_hash = filebase64sha256("lambda_payload.zip")
    publish = "true"
    timeout = 30
}

resource "aws_lambda_event_source_mapping" "sqs-to-lambda-mapping" {
    event_source_arn = aws_sqs_queue.sec-api-report-structure-loader-trigger-sqs.arn
    function_name = aws_lambda_function.sec-api-report-structure-loader-lambda.arn
}

resource "aws_cloudwatch_log_group" "sec-api-report-structure-loader-log-group" {
    name = "/aws/lambda/${aws_lambda_function.sec-api-report-structure-loader-lambda.function_name}"
    retention_in_days = 0
    lifecycle {
      prevent_destroy = false
    }
}