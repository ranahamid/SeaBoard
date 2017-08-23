using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SB.AR.AppWeb.Models
{
    public class CompanyApprover
    {
      

        [SharepointFieldName("Dept_Reviewer_1")]
        public string Dept_Reviewer_1 { get; set; }

        [SharepointFieldName("Dept_Reviewer_2")]
        public string Dept_Reviewer_2 { get; set; }

        [SharepointFieldName("Dept_Reviewer_3")]
        public string Dept_Reviewer_3 { get; set; }

        [SharepointFieldName("Dept_Reviewer_4")]
        public string Dept_Reviewer_4 { get; set; }

        [SharepointFieldName("Dept_Reviewer_5")]

        public string Dept_Reviewer_5 { get; set; }
        [SharepointFieldName("IT_Reviewer")]
        public string IT_Reviewer { get; set; }

        [SharepointFieldName("HR_Reviewer")]
        public string HR_Reviewer { get; set; }

        [SharepointFieldName("Legal_Reviewer")]
        public string Legal_Reviewer { get; set; }

        [SharepointFieldName("Ops_Leader")]
        public string Ops_Leader { get; set; }

        [SharepointFieldName("Ops_Leader_2")]
        public string Ops_Leader_2 { get; set; }

        [SharepointFieldName("Ops_Leader_3")]
        public string Ops_Leader_3 { get; set; }

        [SharepointFieldName("Finance_Leader")]
        public string Finance_Leader { get; set; }

        [SharepointFieldName("President")]
        public string President { get; set; }
        
        [SharepointFieldName("Division")]
        public LookupFieldMapper Division { get; set; }
        
        [SharepointFieldName("Company_Name")]
        public LookupFieldMapper Company_Name { get; set; }
        
    

    }
}