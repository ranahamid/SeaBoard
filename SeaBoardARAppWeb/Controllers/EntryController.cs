using SB.AR.AppWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using model = SB.AR.AppWeb.Models;

namespace SB.AR.AppWeb.Controllers
{
    public class EntryController : SBControllerBase
    {
        //
        // GET: /Entry/
        public ActionResult Index()
        {
            return PartialView();
        }       
        public string GetDescription(string arTypeId)
        {
            var typeDescrips = new ARTypeViewModel(SPContext).GetAllArTypes();
            if (typeDescrips != null)
            {
                var des = typeDescrips.FirstOrDefault(d => d.AR_Type.ToLower().Trim().Equals(arTypeId.ToLower().Trim()));
                if (des != null)
                    return des.TypeDescription;
            }
            return string.Empty;
        }

	}
}