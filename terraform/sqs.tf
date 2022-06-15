data "aws_sns_topic" "target-sns" {
  name = local.sourceSns
}

resource "aws_sqs_queue" "loader-lambda-function-q" {
  name = "${local.lambdaName}-Q"
}

resource "aws_sns_topic_subscription" "target-sns-sub" {
  topic_arn = data.aws_sns_topic.target-sns.arn
  protocol = "sqs"
  endpoint = aws_sqs_queue.loader-lambda-function-q.arn
}

resource "aws_sqs_queue_policy" "loader-lambda-function-q-policy" {
  queue_url = aws_sqs_queue.loader-lambda-function-q.id
  policy = data.aws_iam_policy_document.sqs_to_sns_subscription_policy.json
}

data "aws_iam_policy_document" "sqs_to_sns_subscription_policy"{
  statement {
    actions = [
      "sqs:SendMessage"
    ]

    resources = [
      aws_sqs_queue.loader-lambda-function-q.arn
    ]

    principals {
      type = "Service"
      identifiers = ["sns.amazonaws.com"]
    }

    condition {
      test = "ArnEquals"
      values = [
        data.aws_sns_topic.target-sns.arn
      ]
      variable = "aws:SourceArn"
    }
  }
}
