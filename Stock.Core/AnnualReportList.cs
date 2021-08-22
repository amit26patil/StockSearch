using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Search.Core
{
    public class ReportDetails
    {
        public string Year { get; set; }
        [JsonProperty(PropertyName = "file_name")]
        public string FileName { get; set; }
        public string Code { get; set; }
        public string Group { get; set; }
        public string Industry { get; set; }
        public string Instrument { get; set; }
        public string IssuerName { get; set; }
        public string SecurityId { get; set; }
        public string SecurityName { get; set; }
        public decimal FaceValue { get; set; }
    }
    public class AnnualReportList
    {
        public List<ReportDetails> Table { get; set; }
    }
}
