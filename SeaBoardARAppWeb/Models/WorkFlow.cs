using Microsoft.SharePoint.Client;
using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SB.AR.AppWeb.Models
{
    public class WorkFlow
    {


        [SharepointFieldName("ID")]
        public int ID { get; set; }


        [SharepointFieldName("Title")]
        public string Title { get; set; }


        [SharepointFieldName("AssignedTo")]
        public string AssignedTo { get; set; }

        [SharepointFieldName("Status")]
        public string Status { get; set; }

        [SharepointFieldName("Priority")]
        public string Priority { get; set; }

        [SharepointFieldName("DueDate")]
        public DateTime DueDate { get; set; }

        [SharepointFieldName("StartDate")]
        public DateTime StartDate { get; set; }

        [SharepointFieldName("Date_x0020_Approved")]
        public DateTime? DateApproved { get; set; }
        

        [SharepointFieldName("PercentComplete")]
        public double PercentComplete { get; set; }

        [SharepointFieldName("Predecessors")]
        public LookupFieldMapper Predecessors { get; set; }


        [SharepointFieldName("WorkflowLink")]
        public string WorkflowLink { get; set; }


        [SharepointFieldName("WorkflowOutcome")]
        public string WorkflowOutcome { get; set; }

        private string _lookupValue = string.Empty;

        public double? AR_ID { get; set; }

        public string RoleName { get; set; }

    }
}