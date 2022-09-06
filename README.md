# SEC API Financial Statement Loader
This service is responsible for fetching data from 

## Table of Contents
- **[Project Overview](#ii-project-overview)**
- Glossary Terms
  - SEC
  - SEC API
  - Financial Statement
  - XBRL
- Workflow Overview
- Infrastructure Overview
- How to (Build / Deploy / Destroy)
- Useful Links

## Project Overview
- "SEC API Financial Statement Loader" service is part of the project that retrieves financial data from SEC API, transformes it, stores it into DynamoDB using format that is convinient for searching and queuerying and provides it through public API 
- Problem that this services solves: SEC API is a public API and has rate limiting . Besides that, 

#### List of services that are part of this project:
1. **[Financial Data Loader Bootsrapping](https://github.com/Mikhail-Vilms/sec-api-financial-data-loader-bootsrapping): Service contains terraform scripts necessary for boostrapping the rest of services**
2. **[Company Details Loader](https://github.com/Mikhail-Vilms/sec-api-company-details-loader)**
3. **[Financial Data Loader](https://github.com/Mikhail-Vilms/sec-api-financial-data-loader)**
4. **[Financial Statement Loader](https://github.com/Mikhail-Vilms/sec-api-financial-statement-loader): Responsible for:**
   - **Retrieving structure for the given financial statement and company**
   - **Saving it to DynamoDB table**
   - **Notifying "Financial Position Loader" service what financial positions are ready to be processed**
5. **[Financial Position Loader](https://github.com/Mikhail-Vilms/sec-api-financial-position-loader): Responsible for fethching all financial facts for the given financial position (that belongs to the specific )**
6. **[Financial Data Service](https://github.com/Mikhail-Vilms/sec-api-financial-data-service)**
7. **[Financial Data Service UI](https://github.com/Mikhail-Vilms/sec-api-financial-data-service-ui)**

#### AWS Infrastructure Diagram:
![Project_Overview_2](https://user-images.githubusercontent.com/57194114/188555596-ea1ff87c-8909-4ee8-864f-d14a41127af0.jpg)

## Glossary Terms

#### SEC
U.S. Securities and Exchange Comission

#### SEC API
"data.sec.gov" was created to host RESTful data Application Programming Interfaces (APIs) delivering JSON-formatted data to external customers and to web pages on SEC.gov. These APIs do not require any authentication or API keys to access. Currently included in the APIs are the submissions history by filer and the XBRL data from financial statements (forms 10-Q, 10-K,8-K, 20-F, 40-F, 6-K, and their variants). See here: https://www.sec.gov/edgar/sec-api-documentation

#### Financial Statement
Financial statements are written records that convey the business activities and the financial performance of a company. Financial statements are often audited by government agencies, accountants, firms, etc. to ensure accuracy and for tax, financing, or investing purposes. For-profit primary financial statements include the balance sheet, income statement, statement of cash flow, and statement of changes in equity. https://www.investopedia.com/terms/f/financial-statements.asp
  - Balance Sheet:
  - Income Statement:
  - Cash Flow Statement:
  - https://www.sec.gov/reportspubs/investor-publications/investorpubsbegfinstmtguidehtm.html

#### XBRL
XBRL is the international standard for the electronic representation of business reports. At the heart of the XBRL standard is the XBRL 2.1 Specification, originally released in 2003. This spe cification defines the basic building blocks of facts, instance documents, concepts and taxonomies which are common to all implementations of XBRL.


## Workflow Overview
- Lambda is triggered by SQS queue. SQS queue is subscribed to SNS topic. Lambda expects messages that have three fields:
  - 
  - 
  - 
- 
## Infrastructure Overview

## How to (Build / Deploy / Destroy)

## Useful Links
