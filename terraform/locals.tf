locals {
    lambdaProjectName = "SecApiFinancialStatementLoader"
    lambdaName = "Sec-Api-Financial-Statement-Loader"
    dynamoDbTableName = "Sec-Api-Financial-Data"
    sourceSns = "Sec-Api-Financial-Statements-To-Load"
    targetSns = "Sec-Api-Financial-Positions-To-Load"
}
