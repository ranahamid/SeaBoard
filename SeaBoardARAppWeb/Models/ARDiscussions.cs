using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SB.AR.AppWeb.Models
{

    public class ARDiscussions 
    {
        [SharepointFieldName("Messsage")]
        public string Messsage { set; get; }
        [SharepointFieldName("AllApprovers")]
        public bool AllApprovers { set; get; }
        [SharepointFieldName("ProjectManagers")]
        public bool ProjectManagers { set; get; }
        [SharepointFieldName("Orignator")]
        public bool Orignator { set; get; }
        [SharepointFieldName("Public")]
        public bool Public { set; get; }
        [SharepointFieldName("ToAddress")]
        public List<UserDetails> ToAddress { set; get; }
        [SharepointFieldName("Author")]
        public UserDetails From { get; set; }
        [SharepointFieldName("Created")]
        public DateTime Created { get; set; }
    }
}