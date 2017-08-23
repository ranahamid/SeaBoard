using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SB.AR.AppWeb.Models
{
    public class Category
    {
       
        [SharepointFieldName("ID")]
        public Int32? Id { get; set; }

        [SharepointFieldName("Title")]
        public String Name { get; set; }
    }
    public class Division
    {

        [SharepointFieldName("ID")]
        public Int32? Id { get; set; }

        [SharepointFieldName("Title")]
        public String Name { get; set; }
    }
    public class Company
    {

        [SharepointFieldName("ID")]
        public Int32? Id { get; set; }

        [SharepointFieldName("Title")]
        public String Name { get; set; }

        [SharepointFieldName("Division")]
        public LookupFieldMapper Division { get; set; }

    }
}