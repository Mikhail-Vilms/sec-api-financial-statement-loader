locals {
    lambdaProjectName = "SecApiFinancialStatementLoader"
    lambdaName = "Sec-Api-Financial-Statement-Loader"
    dynamoDbTableName = "Sec-Api-Financial-Data"
    targetSns = "Sec-Api-Financial-Statements-To-Load" 
}
