using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SB.AR.AppWeb.ViewModels;
using System.Net.Http;
using Newtonsoft.Json;
using SB.AR.AppWeb.Models;

namespace SB.AR.AppWeb.Controllers
{
    public class NarrativeController : SBControllerBase
    {
        //
        // GET: /Narrative/
        public ActionResult Index()
        {
            if(this.AR != null && this.AR.AR_ID == null)
            {
                var ar = GetARById(this.AR.ID);
                if (ar != null && ar.AR_ID != null)
                {
                    this.AR = ar;
                }
            }
            var NarrativeView = new NarrativeViewModel(SPContext, this.AR);
            return PartialView("_Narrative", NarrativeView);
        }

        [HttpPost]
        public ActionResult SaveARAsDraft(AR.AppWeb.Models.AR narrative)
        {
            //var hostUrl = this.SPHostUrl;
            //var result = SaveAR(narrative);
            //narrative.Current_Status = "";
            //narrative.Submit_Action = "Hold as Draft";

            var hostUrl = this.SPHostUrl;
            var result = SaveAR(narrative);

            this.AR.Current_Status = "Not Submitted";
            this.AR.Submit_Action = "Hold as Draft";



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
        public ActionResult SaveARAsSubmitRMReview(AR.AppWeb.Models.AR narrative)
        {
            var hostUrl = this.SPHostUrl;
            var result = SaveAR(narrative);

            this.AR.Current_Status = "Pending Edits";
            this.AR.Submit_Action = "PM Review";
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
        public ActionResult SaveAR(AR.AppWeb.Models.AR narrative)
        {
            ViewBag.ArTypeName = this.ARTypeName;
            SB.AR.AppWeb.Models.AR Ar = null;

            if (this.AR == null)
            {
                Ar = new SB.AR.AppWeb.Models.AR();
                Ar.ID = narrative.ID;
              //  Ar.Current_Status = narrative.Current_Status;
                Ar.Submit_Action = narrative.Submit_Action;
            }
            else
            {
                Ar = this.AR;
                //if (!string.IsNullOrEmpty(this.AR.Current_Status))
                //    Ar.Current_Status = this.AR.Current_Status;

                if (!string.IsNullOrEmpty(this.AR.Submit_Action))
                    Ar.Submit_Action = this.AR.Submit_Action;
            }
           
            Ar.PresentSituationIssue = narrative.PresentSituationIssue;
            Ar.Proposed_Solution = narrative.Proposed_Solution;
            Ar.Other_Potential_Solutions = narrative.Other_Potential_Solutions;
            Ar.Explanation_of_Costs = narrative.Explanation_of_Costs;
            Ar.Financial_Measures = narrative.Financial_Measures;
            //var data = SaveARToList(Ar);

            if (!Ar.IsApproved)
            {
                Session["AR"] = Ar;
            }
            var retObj = new Result
            {
                Data = AR,
                IsRedirect = false

            };
            var resultData = JsonConvert.SerializeObject(retObj);
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }
    }
}