# SEC API Financial Statement Loader
This service is responsible for fetching data from 

## Table of Contents
- Glossary Terms
  - SEC
  - SEC API
  - Financial Statement
  - XBRL
- Workflow Overview
- Infrastructure Overview
- How to (Build / Deploy / Destroy)
- Useful Links

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