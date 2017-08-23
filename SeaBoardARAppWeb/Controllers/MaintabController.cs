using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SB.AR.AppWeb.ViewModels;
using System.Net.Http;
using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Helper;
using Newtonsoft.Json;
using SB.AR.AppWeb.Models;
using NSBD.SharepointAutoMapper;
using System.IO;

namespace SB.AR.AppWeb.Controllers
{
    public class MaintabController : SBControllerBase
    {
        //
        // GET: /Maintab/
        public ActionResult Index(string arTypeId, string arTitle, bool isTabClick)
        {
            ViewBag.ARTypeID = arTypeId;
            this.ARType = arTypeId;
            ViewBag.ArTypeName = this.ARTypeName;
            this.ARTitle = string.Empty;
            
            if (!string.IsNullOrEmpty(arTypeId))
            {
                Session["AR"] = null;
                var ar = new AR.AppWeb.Models.AR();
                ar.AR_Type = this.ARTypeName;
                ar.Title =string.Empty;
                ar.Current_Status = "Not Submitted";
             
                ar.Attachment_Folder_Id = AttachmentFolderID();
                if (AR != null){
                    ar.ID = AR.ID;
                }
                Session["AR"] = ar;
            }
            var nrw = new MaintabViewModel(SPContext, this.AR);
            return PartialView("_Maintab", nrw);
        }

        private string AttachmentFolderID()
        {
            var date = DateTime.Now;
            Random rnd = new Random();

            var Attachment_Folder_Id = string.Format("{0}-{1}-{2}-{3}-{4}", date.Year, date.Month, date.Day, date.Millisecond, rnd.Next(99999));
            return Attachment_Folder_Id;
        }

        //private string AttachmentFolderID()
        //{
        //    string shareFileFolder = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["ShareFileFolder"]);
        //    var date = DateTime.Now;
        //    string ARNumber = string.Empty;

        //    var isCreated = false;
        //    while (isCreated == false)
        //    {
        //        ARNumber = string.Format("{0}-{1}-{2}-{3}", date.Year, date.Month, date.Day, date.Millisecond);
        //        if (!string.IsNullOrEmpty(ARNumber))
        //        {
        //            if (!string.IsNullOrEmpty(shareFileFolder))
        //            {
        //                if (!Directory.Exists(shareFileFolder))
        //                {
        //                    Directory.CreateDirectory(shareFileFolder);
        //                }
        //                string ShareItemFolder = Path.Combine(shareFileFolder, ARNumber);
        //                if (!Directory.Exists(ShareItemFolder))
        //                {
        //                    Directory.CreateDirectory(ShareItemFolder);
        //                }
        //            }
        //        }
        //    }

        //    return ARNumber;
        //}

        [HttpPost]
        public ActionResult CancelAR(AR.AppWeb.Models.AR maintab)
        {
            var hostUrl = this.SPHostUrl;
            return RedirectToAction("Index", "Home", new { SPHostUrl = hostUrl });
        }


        [HttpPost]
        public ActionResult SaveARAsDraft(AR.AppWeb.Models.AR maintab)
        {
            var hostUrl = this.SPHostUrl;            
            var result = SaveAR(maintab);

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
        public ActionResult SaveARAsSubmitRMReview(AR.AppWeb.Models.AR maintab)
        {
            var hostUrl = this.SPHostUrl;
                        
            var result = SaveAR(maintab);
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
        static readonly object _object = new object();

        public int count = 0;
        [HttpPost]
        public ActionResult SaveAR(AR.AppWeb.Models.AR maintab)
        {
            if (count == 0)
            {
                count++;
                lock (_object)
                {
                    ViewBag.ArTypeName = this.ARTypeName;
                    SB.AR.AppWeb.Models.AR Ar = null;
                    
                    if(this.AR == null)
                        Ar = new SB.AR.AppWeb.Models.AR();
                    else
                        Ar = this.AR;

                    if (this.AR != null)
                    {
                        Ar.ID = this.AR.ID;
                       ///if (!string.IsNullOrEmpty(this.AR.Current_Status))
                           // Ar.Current_Status = this.AR.Current_Status;
                        //else
                        //    Ar.Current_Status = "Not Submitted";

                        if (!string.IsNullOrEmpty(this.AR.Submit_Action))
                            Ar.Submit_Action = this.AR.Submit_Action;
                    }
                    else
                    {

                        Ar.ID = maintab.ID;
                    }

                    Ar.Title = maintab.Title;
                    if (this.AR != null)
                        Ar.AR_Type = this.AR.AR_Type;
                    Ar.LocalAR = maintab.LocalAR;
                    Ar.ARNumber = maintab.ARNumber;
                    Ar.PMOwner = maintab.PMOwner;
                    Ar.Project_Start = maintab.Project_Start;
                    Ar.Project_End = maintab.Project_End;
                    Ar.CategoryId = maintab.CategoryId;
                    Ar.DivisionId = maintab.DivisionId;
                    Ar.CompanyId = maintab.CompanyId;
                    Ar.Engineering_Review = maintab.Engineering_Review;
                    Ar.IT_Review = maintab.IT_Review;
                    Ar.Legal_Review = maintab.Legal_Review;
                    Ar.HR_Review = maintab.HR_Review;
                    Ar.Location = maintab.Location;
                    Ar.IsMaintab = true;

                    if(this.AR.Current_Status != null && this.AR.Current_Status.Equals("Pending Approvals"))
                    {
                        Ar.Audit = maintab.Audit;
                    }

                    if (null != Session["_companies"])
                    {
                        var _companies = (List<SelectListItem>)Session["_companies"];
                        var sItemType = _companies.FirstOrDefault(c => c.Value == Ar.CompanyId.ToString());
                        if (sItemType != null)
                        {
                            Ar.Company_Name = new LookupFieldMapper
                            {

                                ID = Convert.ToInt32(sItemType.Value),
                                Value = sItemType.Text
                            };
                        }

                    }
                    if (null != Session["divisions"])
                    {
                        var divisions = (List<SelectListItem>)Session["divisions"];
                        var sItemType = divisions.FirstOrDefault(c => c.Value == Ar.DivisionId.ToString());
                        if (sItemType != null)
                        {
                            Ar.Division = new LookupFieldMapper
                            {

                                ID = Convert.ToInt32(sItemType.Value),
                                Value = sItemType.Text
                            };
                        }

                    }
                    if (null != Session["Categories"])
                    {
                        var Categories = (List<SelectListItem>)Session["Categories"];
                        var sItemType = Categories.FirstOrDefault(c => c.Value == Ar.CategoryId.ToString());
                        if (sItemType != null)
                        {
                            Ar.Category = new LookupFieldMapper
                            {

                                ID = Convert.ToInt32(sItemType.Value),
                                Value = sItemType.Text
                            };
                        }

                    }
                    if (!string.IsNullOrEmpty(Ar.PMOwner))
                    {
                        Ar.PMOwnerLogin = Ar.PMOwner;
                        using (var clientContext = SPContext.CreateUserClientContextForSPHost())
                        {
                            FieldUserValue pmOwner = !string.IsNullOrEmpty(Ar.PMOwner) ? PeoplePickerHelper.SPEnsureSBUser(clientContext, Ar.PMOwner) : null;
                            if (pmOwner != null)
                            {
                                Ar.PMUser = pmOwner;
                                if (Request.Params["LookupValue"] != null)
                                    Ar.UserNameLookupValue = Request.Params["LookupValue"].ToString();

                            }
                        }
                    }
                    if (!Ar.IsApproved)
                    {
                        Session["AR"] = Ar;
                    }
                    //var data = SaveARToList(Ar);

                    var retObj = new Result
                    {
                        Data = AR,
                        IsRedirect = false

                    };
                    var resultData = JsonConvert.SerializeObject(retObj);
                    return Json(resultData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                count = 0;
                var retObj3 = new Result
                {
                    Data = AR,
                    IsRedirect = false

                };
                var resultData3 = JsonConvert.SerializeObject(retObj3);
                return Json(resultData3, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult FillCompany(int divisionId)
        {
            var vm = new CompanyMasterViewModel(SPContext)
            {
                DivisionId = divisionId,

            };           
            return PartialView("_CompanyList", vm);
        }

        
        //[HttpPost]
        //public JsonResult FlagAR(AR.AppWeb.Models.AR maintab)
        //{

        //    //TODO: create and call AR save just for this field as otherwise AR cannot be edited in Pending Approvals Mode.


        //    var result = new Result
        //    {
        //        Data = "success",
        //        IsRedirect = true
        //    };
        //    return Json(result);
        //}

        public string GetFlagUserIDs()
        {
            List<string> users = new List<string>();
            string showCheckBox = "0";
            
            var clientContext = this.SPContext.CreateUserClientContextForSPHost();
            List arListapprover = clientContext.Web.Lists.GetByTitle(SPListMeta.AUDITUSERS);
            CamlQuery cq = CamlQuery.CreateAllItemsQuery();
            var allitems = arListapprover.GetItems(cq);

            clientContext.Load(clientContext.Web.CurrentUser, user => user.LoginName, user => user.Id);
            clientContext.Load(allitems);
            clientContext.ExecuteQuery();
            foreach(var item in allitems)
            {
                Microsoft.SharePoint.Client.FieldUserValue Audit_User = item["Audit_User"] as FieldUserValue;
                users.Add(Audit_User.LookupId.ToString());                
            }

            // check if current user is part of Audit_Users list, if yes then allowed to check the Flag AR checkbox
            if (users.Contains(clientContext.Web.CurrentUser.Id.ToString()))
            {
                showCheckBox = "1";
            }

            System.Web.Script.Serialization.JavaScriptSerializer oSerializer =
             new System.Web.Script.Serialization.JavaScriptSerializer();
            //return oSerializer.Serialize(users.ToArray());
            return oSerializer.Serialize(showCheckBox);
            
        }

        
        public JsonResult SaveFlagARToList(string arid)
        {
            int ar_id = Convert.ToInt32(arid);
            

            //if (string.IsNullOrEmpty(ar.Title))
            //{
            //    var emptyresponse = new HttpResponseMessage(HttpStatusCode.NotImplemented)
            //    {
            //        Content = new StringContent(string.Empty)
            //    };
            //    return emptyresponse;
            //}

            var returnObj = new { result = "", user = "" };

            using (var clientContext = SPContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    List oList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);
                    ListItem oListItem = null;

                    if (ar_id > 0)
                    {
                        oListItem = oList.GetItemById(arid);
                        clientContext.Load(oListItem);
                        clientContext.Load(clientContext.Web.CurrentUser, user => user.LoginName, user => user.Id, user => user.Title);
                        
                        clientContext.ExecuteQuery();

                        //FieldUserValue currentUser = PeoplePickerHelper.SPEnsureSBUser(clientContext, clientContext.Web.CurrentUser.LoginName);
                        oListItem["Audit_Updated_By"] = clientContext.Web.CurrentUser.Id;
                        oListItem["Audit"] = true;

                        oListItem.Update();
                        clientContext.Load(oListItem);
                        clientContext.ExecuteQuery();

                        var item = oListItem.ProjectToEntity<SB.AR.AppWeb.Models.AR>();

                        Session["AR"] = item;
                                                
                        returnObj = new { result = "success", user = clientContext.Web.CurrentUser.Title };
                    }
                }
            }

            var result = new Result
            {
                Data = returnObj,
                IsRedirect = true
            };
            return Json(result);

        }
	}
}