using System;
using System.Xml;
using System.Xml.Schema;

namespace SecApiReportStructureLoader.Helpers
{
    public class TaxanomyXsdDocHelper
    {
        public static string GetCashFlowsUriFromXsdSchema(XmlSchema taxanomyXsdSchema)
        {
            // 1) Looking for <xs:annotation>:
            XmlSchemaAnnotation xsa = null;
            foreach (XmlSchemaObject xsdItem in taxanomyXsdSchema.Items)
            {
                if (xsdItem is XmlSchemaAnnotation)
                {
                    // found <xs:annotation>
                    xsa = xsdItem as XmlSchemaAnnotation;
                }
            }

            if (xsa == null)
            {
                return null;
            }

            // 2) Looking for <xs:appinfo>:
            XmlSchemaAppInfo xsai = null;
            foreach (XmlSchemaObject xsaItem in xsa.Items)
            {
                if (xsaItem is XmlSchemaAppInfo)
                {
                    // found <xs:appinfo>
                    xsai = xsaItem as XmlSchemaAppInfo;
                }
            }

            if (xsai == null)
            {
                return null;
            }

            // 3) Looking for "ConsolidatedStatementsofCashFlows" attribute:
            foreach (XmlNode xsaiNode in xsai.Markup)
            {
                if (xsaiNode.Attributes.GetNamedItem("id") != null && xsaiNode.Attributes.GetNamedItem("id").Value.ToUpper() == "CONSOLIDATEDSTATEMENTSOFCASHFLOWS")
                {
                    return xsaiNode.Attributes.GetNamedItem("roleURI")?.Value;
                }
            }

            return null;
        }
    }
}
