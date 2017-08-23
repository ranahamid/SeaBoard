using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SB.AR.AppWeb.Models
{
    public class Search
    {
        public string KeywordString { get; set; }
        public string DivisionString { get; set; }
        public string CompanyString { get; set; }
        public string CategoryString { get; set; }
        public string PMOwner { get; set; }
        public string CreatedBy { get; set; }
        public double? AmountFrom { get; set; }
        public double? AmountTo { get; set; }
        public DateTime? SubmittedFrom { get; set; }
        public DateTime? SubmittedTo { get; set; }
        public string Status { get; set; }
        public string LookupValue { get; set; }
        public int LookupId { get; set; }
        public int CompanyId { get; set; }
        public int DivisionId { get; set; }
    }
}
