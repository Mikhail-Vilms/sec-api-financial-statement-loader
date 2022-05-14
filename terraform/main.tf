terraform {
    backend "s3" {
        bucket = "672009997609-terraform-state"
        key = "sec-api-report-structure-loader/terraform.tfstate"
        region = "us-east-1"
    }
}

provider "aws"{
    region = "us-east-1"
}