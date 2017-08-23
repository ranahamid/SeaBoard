using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SB.AR.AppWeb.Models
{    
    public class KeyCodes
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Contact Name required!", AllowEmptyStrings = false)]
        public string ContactName { get; set; }
        [Required(ErrorMessage = "Contact No required!", AllowEmptyStrings = false)]
        public string ContactNo { get; set; }
        public int KeyCodeID { get; set; }
        public int ServiceRequestID { get; set; }
        public string OldKeyCode { get; set; }
        public string ReferenceKeyCode { get; set; }
        public string Description { get; set; }
        public string NewKeyCode { get; set; }

        public bool IsDeleted { get; set; }

    }
}