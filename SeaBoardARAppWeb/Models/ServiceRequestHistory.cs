using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SB.AR.AppWeb.Models
{
    public class ServiceRequestHistory
    {
        public string ID { get; set; }
        public int ServiceRequestID { get; set; }
        public string Description { get; set; }
        public string UserModifiedBy { get; set; }
        public string UserModifiedDate { get; set; }
    }
}