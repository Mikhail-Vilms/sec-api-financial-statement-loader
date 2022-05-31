data "archive_file" "lambda_zip_payload" {
    type = "zip"
    source_dir = "../${local.lambdaProjectName}/bin/Debug/netcoreapp3.1/"
    output_path = "lambda_payload.zip"
}

resource "aws_lambda_function" "loader-lambda-function" {
    filename = data.archive_file.lambda_zip_payload.output_path
    function_name = local.lambdaName
    handler = "${local.lambdaProjectName}::${local.lambdaProjectName}.Function::FunctionHandler"
    runtime = "dotnetcore3.1"
    role = aws_iam_role.loader-lambda-function-exec-role.arn
    source_code_hash = filebase64sha256("lambda_payload.zip")
    publish = "true"
    timeout = 30
}

resource "aws_lambda_event_source_mapping" "sqs-to-lambda-mapping" {
    event_source_arn = aws_sqs_queue.loader-lambda-function-q.arn
    function_name = aws_lambda_function.loader-lambda-function.arn
}

resource "aws_cloudwatch_log_group" "loader-lambda-function-log-group" {
    name = "/aws/lambda/${aws_lambda_function.loader-lambda-function.function_name}"
    retention_in_days = 0
    lifecycle {
      prevent_destroy = false
    }
}
