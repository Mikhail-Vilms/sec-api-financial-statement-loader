# SEC API Financial Statement Loader

## 

## Terminology
- SEC API (U.S. Securities and Exchange Comission): "data.sec.gov" was created to host RESTful data Application Programming Interfaces (APIs) delivering JSON-formatted data to external customers and to web pages on SEC.gov. These APIs do not require any authentication or API keys to access. Currently included in the APIs are the submissions history by filer and the XBRL data from financial statements (forms 10-Q, 10-K,8-K, 20-F, 40-F, 6-K, and their variants). See here: https://www.sec.gov/edgar/sec-api-documentation
- Financial Statement: Financial statements are written records that convey the business activities and the financial performance of a company. Financial statements are often audited by government agencies, accountants, firms, etc. to ensure accuracy and for tax, financing, or investing purposes. For-profit primary financial statements include the balance sheet, income statement, statement of cash flow, and statement of changes in equity. https://www.investopedia.com/terms/f/financial-statements.asp
  - Balance Sheet:
  - Income Statement:
  - Cash Flow Statement:
  - https://www.sec.gov/reportspubs/investor-publications/investorpubsbegfinstmtguidehtm.html
- XBRL: XBRL is the international standard for the electronic representation of business reports. At the heart of the XBRL standard is the XBRL 2.1 Specification, originally released in 2003. This specification defines the basic building blocks of facts, instance documents, concepts and taxonomies which are common to all implementations of XBRL.

This starter project consists of:
* Function.cs - class file containing a class with a single function handler method
* aws-lambda-tools-defaults.json - default argument settings for use with Visual Studio and command line deployment tools for AWS

You may also have a test project depending on the options selected.

The generated function handler is a simple method accepting a string argument that returns the uppercase equivalent of the input string. Replace the body of this method, and parameters, to suit your needs. 

## Here are some steps to follow from Visual Studio:

To deploy your function to AWS Lambda, right click the project in Solution Explorer and select *Publish to AWS Lambda*.

To view your deployed function open its Function View window by double-clicking the function name shown beneath the AWS Lambda node in the AWS Explorer tree.

To perform testing against your deployed function use the Test Invoke tab in the opened Function View window.

To configure event sources for your deployed function, for example to have your function invoked when an object is created in an Amazon S3 bucket, use the Event Sources tab in the opened Function View window.

To update the runtime configuration of your deployed function use the Configuration tab in the opened Function View window.

To view execution logs of invocations of your function use the Logs tab in the opened Function View window.

## Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can deploy your application using the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

Execute unit tests
```
    cd "SecApiFinancialStatementLoader/test/SecApiFinancialStatementLoader.Tests"
    dotnet test
```

Deploy function to AWS Lambda
```
    cd "SecApiFinancialStatementLoader/src/SecApiFinancialStatementLoader"
    dotnet lambda deploy-function
```
