using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NSBD.SharepointAutoMapper;
using SB.AR.AppWeb.Helper;
using SB.AR.AppWeb.ViewModels;
using System.Net.Http;
using System.Net;

namespace SB.AR.AppWeb.Controllers
{
    public class HomeController : SBControllerBase
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            Uri spUrl = null;
            foreach (string key in Request.QueryString.Keys)
            {
                if (key == "HostUrl" || key == "SPHostUrl")
                {
                    spUrl = new Uri(Request.QueryString[key]);
                    break;
                }
            }
            
            ClientContext clientContext = null;
            clientContext = TokenHelper.GetS2SClientContextWithWindowsIdentity(spUrl, Request.LogonUserIdentity);
            
            Session["AR"] = null;
            var hostUrl = this.SPHostUrl;
            var appUrl = this.SPAppWebUrl;
            var lanUrl = this.SPLanguage;
            if (this.AR != null)
            {
                Session["AR"] = null;
                this.AR = null;
            }


            if (!string.IsNullOrEmpty(hostUrl))
            {
                hostUrl = hostUrl.Replace("http://", "https://");
            }
            
            var allPeopleData = PeoplePickerHelper.GetPeoplePickerSearchData(SPContext);
            if(!string.IsNullOrEmpty(allPeopleData))
            {
                Session["allPeopleData"] = allPeopleData;
            }
            return RedirectToAction("Index", "Seaboard", new { SPHostUrl = hostUrl });
        }

        [HttpPost]
        public JsonResult CancelAR()
        {
            if (this.AR != null)
            {
                //if (this.AR.ID > 0 && Request["aid"] != null)
                //    Delete(this.AR.ID);
                this.AR = null;
                Session["AR"] = null;
            }
            var result = new Result
            {
                Data = "success",
                IsRedirect = true
            };
            return Json(result);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Buttons(string currenttab, string idPrev, string idNext, string controller = "/maintab/savear")
        {
            ViewBag.NextTab = idNext;
            ViewBag.PrevTab = idPrev;
            ViewBag.Controller = controller;
            ViewBag.CurrentTab = currenttab;

            var model = new SB.AR.AppWeb.ViewModels.ARButtonViewModel(SPContext, this.AR);
            if (model.AR == null)
                model.AR = this.AR;

            return PartialView("~/Views/Shared/_Buttons.cshtml", model);
        }
        /// <summary>
        /// Display dynamic navigation menu
        /// </summary>
        /// <returns></returns>
        //[SharePointContextFilter]
        public ActionResult Menu()
        {
            NavigationViewModel viewModel = new NavigationViewModel();
            try
            {
                viewModel = new NavigationViewModel(SPContext, this.AR);
                if (SPContext != null)
                {
                    ClientContext clientContext = SPContext.CreateUserClientContextForSPHost();
                    List<AR.AppWeb.Models.NavigationAR> Nav = GetNavigationAR(clientContext);

                    if (Request["SPHostUrl"] != null)
                        viewModel.HostUrl = Request["SPHostUrl"].ToString();
                    else
                        viewModel.HostUrl = this.SPHostUrl;
                    viewModel.Navigation = Nav;
                    return PartialView("_Navigation", viewModel);
                }
            }
            catch (Exception ex)
            {
                viewModel = new NavigationViewModel();
                Utility.Logging.LogErrorException(ex, "Client Context is null");
                return PartialView("_Navigation", viewModel);
            }
            return PartialView("_Navigation", viewModel);
        }
        /// <summary>
        /// Pull dynamic navigation property from content list
        /// </summary>
        /// <param name="clientContext">ClientContext</param>
        /// <returns></returns>
        public List<AR.AppWeb.Models.NavigationAR> GetNavigationAR(ClientContext clientContext)
        {
            List<Models.NavigationAR> myARs = new List<Models.NavigationAR>();


            if (clientContext != null)
            {
                List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.NavigationAR);
                CamlQuery query = new CamlQuery();
                // int loggedInUser = CurrentUser.Id;
                query.ViewXml = string.Format(@"<View><Query>   
                                                        <ViewFields>
                                                              <FieldRef Name='Title' />
                                                              <FieldRef Name='URL' />
                                                              <FieldRef Name='Status' />
                                                       </ViewFields>
                                                    </Query> </View>");
                var arItems = arList.GetItems(query);
                clientContext.Load(arItems);
                clientContext.ExecuteQuery();
                foreach (var itm in arItems)
                {

                    myARs.Add(new Models.NavigationAR
                    {

                        Title = itm["Title"] != null ? Convert.ToString(itm["Title"]) : "",
                        URL = itm["URL"] != null ? Convert.ToString(itm["URL"]) : "",
                        Status = itm["Status"] != null ? Convert.ToString(itm["Status"]) : "",

                    });
                }


            }

            return myARs;
        }

      
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult LoadScripts()
        {
            return View("_Scripts");
        }

    }
}
