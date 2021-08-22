using System;
using System.Collections.Generic;
using System.Text;

namespace Search.Core
{
    public class PdfReportDetails
    {
        public PdfReportDetails()
        {
             
        }
        public string ObjectID { get; set; }
        public string PdfText { get; set; }
        public string PdfFileName { get; set; }
        public string EquityCode { get; set; }
        public int PageNumber { get; set; }
        public string Group { get; set; }
        public string Industry { get; set; }
        public string Instrument { get; set; }
        public string IssuerName { get; set; }
        public string SecurityId { get; set; }
        public string SecurityName { get; set; }
        public decimal FaceValue { get; set; }
    }
}
