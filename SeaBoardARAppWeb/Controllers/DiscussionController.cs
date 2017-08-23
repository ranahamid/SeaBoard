using Newtonsoft.Json;
using SB.AR.AppWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SB.AR.AppWeb.Controllers
{
    public class DiscussionController : SBControllerBase
    {

        //
        // GET: /Discussion/
        public ActionResult Index()
        {
            //SetarDetailViewBag();
            ViewModels.ARDiscussionViewModel arDiscussions = new ViewModels.ARDiscussionViewModel(this.SPContext, AR);
            arDiscussions.discussions = arDiscussions.GetARDiscussion();
            // Get the discussion 
            return PartialView("_Discussion", arDiscussions);
        }
        [HttpPost]
        public ActionResult SaveARAsDraft(AR.AppWeb.Models.AR maintab)
        {
            var hostUrl = this.SPHostUrl;
            maintab.Current_Status = "";
            maintab.Submit_Action = "Hold as Draft";

            SaveARToList(this.AR);
            var retObj = new Result
            {
                Data = AR,
                IsRedirect = true

            };
            var resultData = JsonConvert.SerializeObject(retObj);
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveARAsSubmitRMReview(AR.AppWeb.Models.AR maintab)
        {
            var hostUrl = this.SPHostUrl;

            maintab.Current_Status = "Pending Edits";
            maintab.Submit_Action = "PM Review";
            SaveARToList(this.AR);
            var retObj = new Result
            {
                Data = AR,
                IsRedirect = true

            };
            var resultData = JsonConvert.SerializeObject(retObj);
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveDiscussion()
        {
            Models.ARDiscussions arDiscussion = new Models.ARDiscussions();
            arDiscussion.Messsage = HttpContext.Request.Params["message"];
            if (!string.IsNullOrEmpty(arDiscussion.Messsage))
            {
                int arItemId = Convert.ToInt16(HttpContext.Request.Params["ID"]);
                arDiscussion.AllApprovers = Convert.ToBoolean(HttpContext.Request.Params["approvers"]);
                arDiscussion.ProjectManagers = Convert.ToBoolean(HttpContext.Request.Params["projectmanagers"]);
                arDiscussion.Orignator = Convert.ToBoolean(HttpContext.Request.Params["originator"]);
                arDiscussion.Public = Convert.ToBoolean(HttpContext.Request.Params["arpublic"]);
                string division = AR.Division == null ? string.Empty : AR.Division.Value;// HttpContext.Request.Params["division"];
                string company = AR.Company_Name == null ? string.Empty : AR.Company_Name.Value;// HttpContext.Request.Params["company"];
                ViewModels.ARDiscussionViewModel arDiscussionsViewModel = new ViewModels.ARDiscussionViewModel(this.SPContext, AR);
                arDiscussion.ToAddress = arDiscussionsViewModel.GetToAddress(arDiscussion,division, company);
                //arDiscussion.From = arDiscussionsViewModel.GetFromAddress();
                arDiscussionsViewModel.SaveDiscussions(arDiscussion);
                arDiscussionsViewModel.SendMail(arDiscussion);
            }
           

            return Json(new {ToAddress = arDiscussion.ToAddress, From = arDiscussion.From, Created = arDiscussion.Created.ToString()});
        }
	}
}