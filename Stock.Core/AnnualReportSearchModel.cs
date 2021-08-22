using System;
using System.Collections.Generic;
using System.Text;

namespace Search.Core
{
    public class AnnualReportSearchModel
    {
        public AnnualReportSearchModel()
        {

        }
        public EquityModel EquityDetails { get; set; }
        public string ObjectId { get; set; }
        public string PdfText { get; set; }
        public int PageNumber { get; set; }
        public string PdfFileName { get; set; }
    }
}
