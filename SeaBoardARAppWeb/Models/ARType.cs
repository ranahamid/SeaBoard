using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SB.AR.AppWeb.Models
{
    [SharepointListName("AR Type Descriptions")]
    public class ARType : IEntitySharepointMapper
    {
        [SharepointFieldName("ID")]
        public Int32? Id { get; set; }

        [SharepointFieldName("Title")]
        public string Title { get; set; }

        [SharepointFieldName("Type_x0020_Description")]
        public string TypeDescription { get; set; }    
    
        //ChoiceFieldMapper
        [SharepointFieldName("AR_Type")]
        public string AR_Type { get; set; }            
        
    }
}