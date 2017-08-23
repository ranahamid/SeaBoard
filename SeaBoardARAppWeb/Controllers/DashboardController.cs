using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NSBD.SharepointAutoMapper;

using SB.AR.AppWeb.ViewModels;


namespace SB.AR.AppWeb.Controllers
{
    public class DashboardController : SBControllerBase
    {
        //
        // GET: /Dashboard/
        [SharePointContextFilter]
        public ActionResult Index()
        {
            return View();
        }

	}
}