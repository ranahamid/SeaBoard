using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SB.AR.AppWeb.Models
{
    public class ApprovalHistory
    {
        [SharepointFieldName("ID")]
        public int ID { get; set; }

        [SharepointFieldName("Title")]
        public string Title { get; set; }

        [SharepointFieldName("Approver")]
        public string Approver { get; set; }

        [SharepointFieldName("Task_Status")]
        public string Task_Status { get; set; }

        [SharepointFieldName("Date_Assigned")]
        public DateTime? Date_Assigned { get; set; }

        [SharepointFieldName("Date_Complete")]
        public DateTime? Date_Completed { get; set; }

        [SharepointFieldName("Sequence")]
        public int Sequence { get; set; }

        public string ARId { get; set; }

    }
}