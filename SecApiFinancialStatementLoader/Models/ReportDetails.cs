namespace SecApiFinancialStatementLoader.Models
{
    public class ReportDetails
    {
        public string AccessionNumber { get; set; }
        public string ReportDate { get; set; }

        public override string ToString()
        {
            return $"AccessionNumber: {AccessionNumber}; ReportDate: {ReportDate}";
        }
    }
}
