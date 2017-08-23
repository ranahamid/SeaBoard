using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SB.AR.AppWeb.Models;
namespace SB.AR.AppWeb.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            try
            {
                string hostUrl = string.Empty;
                if (Request["SPHostUrl"] != null)
                    hostUrl = Request["SPHostUrl"].ToString();
               
              // return View();
               return View("index", new { SPHostUrl = hostUrl });
                
              }
            catch(Exception e)
            {
                return View();
            }
        }
	}
}