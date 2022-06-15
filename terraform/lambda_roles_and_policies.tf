resource "aws_iam_role" "loader-lambda-function-exec-role" {
    name = "${local.lambdaName}-Exec-Role"
    assume_role_policy = data.aws_iam_policy_document.lambda-assume-role-policy-doc.json
}

data "aws_iam_policy_document" "lambda-assume-role-policy-doc" {
    statement {
        actions    = ["sts:AssumeRole"]
        effect     = "Allow"
        sid        = ""
        principals {
            type        = "Service"
            identifiers = ["lambda.amazonaws.com"]
        }
    }
}

resource "aws_iam_role_policy_attachment" "lambda-full-access-policy-attach" {
    role = aws_iam_role.loader-lambda-function-exec-role.name
    policy_arn = "arn:aws:iam::aws:policy/AWSLambda_FullAccess"
}

resource "aws_iam_role_policy_attachment" "lambda-sqs-access-policy-attach" {
    role = aws_iam_role.loader-lambda-function-exec-role.name
    policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaSQSQueueExecutionRole"
}

data "aws_iam_policy_document" "lambda-dynamo-access-policy-doc" {
    statement {
        actions = [
            "dynamodb:GetItem",
            "dynamodb:DescribeTable",
            "dynamodb:PutItem",
            "dynamodb:UpdateItem",
            "dynamodb:BatchGetItem",
            "dynamodb:BatchWriteItem",
            "dynamodb:DeleteItem"
        ]
        resources = [
            "arn:aws:dynamodb:us-west-2:672009997609:table/${local.dynamoDbTableName}"
        ]
    }

    statement {
        actions = [
            "SNS:Publish"
        ]
        resources = [
            "arn:aws:sns:us-west-2:672009997609:${local.targetSns}"
        ]
    }
}

resource "aws_iam_policy" "lambda-dynamo-access-policy" {
    name   = "${local.lambdaName}-Dynamo-Access-Policy"
    path   = "/"
    policy = data.aws_iam_policy_document.lambda-dynamo-access-policy-doc.json
}

resource "aws_iam_role_policy_attachment" "lambda-sqs-dynamo-policy-attach" {
    role = aws_iam_role.loader-lambda-function-exec-role.name
    policy_arn = aws_iam_policy.lambda-dynamo-access-policy.arn
}
