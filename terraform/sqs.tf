data "aws_sns_topic" "companies-to-process-sns" {
  name = "Sec-Api-Data-Service-Companies-To-Process"
}

resource "aws_sqs_queue" "sec-api-report-structure-loader-trigger-sqs" {
    name = "Sec-Api-Report-Structure-Loader-Trigger-Sqs"
}

resource "aws_sns_topic_subscription" "companies-to-process-sns-sub" {
    topic_arn = data.aws_sns_topic.companies-to-process-sns.arn
    protocol = "sqs"
    endpoint = aws_sqs_queue.sec-api-report-structure-loader-trigger-sqs.arn
}