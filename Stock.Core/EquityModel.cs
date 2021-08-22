using System;
using System.Collections.Generic;
using System.Text;

namespace Search.Core
{
    public class EquityModel
    {
        public EquityModel()
        {

        }
        public EquityModel(IDictionary<string, string> dictionary)
        {
            EquityCode = dictionary["Security Code"];
            IssuerName = dictionary["Issuer Name"];
            SecurityId = dictionary["Security Id"];
            SecurityName = dictionary["Security Name"];
            IsActive = dictionary["Status"]=="Active";
            Group = dictionary["Group"];
            if (decimal.TryParse(dictionary["Face Value"], out decimal t))
                FaceValue = t;
            ISINNo = dictionary["ISIN No"];
            Industry = dictionary["Industry"];
            Instrument = dictionary["Instrument"];
        }
        public string EquityCode { get; set; }
        public string IssuerName { get; set; }
        public string SecurityId { get; set; }
        public string SecurityName { get; set; }
        public bool IsActive { get; set; }
        public string Group { get; set; }
        public decimal FaceValue { get; set; }
        public string ISINNo { get; set; }
        public string Industry { get; set; }
        public string Instrument { get; set; }

    }
}
