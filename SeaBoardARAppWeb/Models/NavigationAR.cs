using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SB.AR.AppWeb.Models
{
    public class NavigationAR
    {
        [SharepointFieldName("Title")]
        public string Title { get; set; }
        [SharepointFieldName("URL")]
        public string URL { get; set; }
        [SharepointFieldName("Status")]
        public string Status { get; set; }

        
    }
}