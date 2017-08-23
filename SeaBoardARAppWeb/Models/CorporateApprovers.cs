using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SB.AR.AppWeb.Models
{
    public class CorporateApprovers
    {

        [SharepointFieldName("Corp_CEO_Reviewer")]
        public string Corp_CEO_Reviewer { get; set; }

        [SharepointFieldName("Corp_CFO_Reviewer")]
        public string Corp_CFO_Reviewer { get; set; }

        [SharepointFieldName("Corp_HR_Reviewer")]
        public string Corp_HR_Reviewer { get; set; }

        [SharepointFieldName("Corp_IT_Reviewer")]
        public string Corp_IT_Reviewer { get; set; }

        [SharepointFieldName("Corp_Legal_Reviewer")]
        public string Corp_Legal_Reviewer { get; set; }

        [SharepointFieldName("Corp_Ops_Leader_2_Reviewer")]
        public string Corp_Ops_Leader_2_Reviewer { get; set; }

        [SharepointFieldName("Corp_Ops_Leader_Reviewer")]
        public string Corp_Ops_Leader_Reviewer { get; set; }

        public double? WorkFlowId { get; set; }
        public DateTime? DateAssigned { get; set; }
        public DateTime? DateApproved { get; set; }
        public string Status { get; set; }
        public string WorkflowOutcome { get; set; }
        public string AssignedTo { get; set; }
        

    }
}