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

resource "aws_sqs_queue_policy" "sec-api-report-structure-loader-trigger-sqs-policy" {
  queue_url = aws_sqs_queue.sec-api-report-structure-loader-trigger-sqs.id
  policy = data.aws_iam_policy_document.sqs_to_sns_subscription_policy.json
}

data "aws_iam_policy_document" "sqs_to_sns_subscription_policy"{
  statement {
    actions = [
      "sqs:SendMessage"
    ]

    resources = [
      aws_sqs_queue.sec-api-report-structure-loader-trigger-sqs.arn
    ]

    principals {
      type = "Service"
      identifiers = ["sns.amazonaws.com"]
    }

    condition {
      test = "ArnEquals"
      values = [
        data.aws_sns_topic.companies-to-process-sns.arn
      ]
      variable = "aws:SourceArn"
    }
  }
}
