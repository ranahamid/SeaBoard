using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.SharePoint.Client;
using SP = Microsoft.SharePoint.Client;
using System.Globalization;
using SB.AR.AppWeb.Models;
using System.IO; 
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using log4net;
using Microsoft.SharePoint.Client.Utilities;
using System.Web.Script.Serialization;
using System.Net.Mail;
namespace SB.AR.AppWeb.Controllers
{
    public class ServiceRequestController : Controller
    {
        public static string accessToken { get; set; }

        // GET: ServiceRequest
        [SharePointContextFilter]
        public ActionResult Index()
        {
            Models.ServiceRequest serviceRequest = null;
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);
                serviceRequest = new Models.ServiceRequest();
                serviceRequest.Audience = GetLookUpList("Audiences", "ID", "Title");
                //serviceRequest.ProductionSpecialist = GetUsersFromGroup("Production Managers");

                Session["ServiceRequest"] = null;
                serviceRequest.Mode = OperationMode.Insert;

                if (HttpContext.Request.QueryString["ServiceRequestID"] != null && !String.IsNullOrEmpty(Convert.ToString(HttpContext.Request.QueryString["ServiceRequestID"])) && Convert.ToString(HttpContext.Request.QueryString["ServiceRequestID"]) != "propVal")
                {
                    serviceRequest.Mode = OperationMode.Edit;
                    serviceRequest = SetServiceRequest(serviceRequest, "GeneralInfo");
                    serviceRequest = BindServiceRequest(serviceRequest);

                }
                serviceRequest.flag = "1";
                serviceRequest = SetServiceRequest(serviceRequest, "GeneralInfo");
                SetUserGroupDetails();


            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Get Index");
            }
            return View("GeneralInfo", serviceRequest);
        }
        [HttpGet]
        [SharePointContextFilter]
        public ActionResult GeneralInfo(ServiceRequest serviceRequest)
        {

            try
            {
                SetUserGroupDetails();
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);
                serviceRequest = GetServiceRequest();
                serviceRequest.Audience = GetLookUpList("Audiences", "ID", "Title");
                //serviceRequest.ProductionSpecialist = GetUsersFromGroup("Production Managers");
                if (HttpContext.Request.QueryString["ServiceRequestID"] != null && !String.IsNullOrEmpty(Convert.ToString(HttpContext.Request.QueryString["ServiceRequestID"])) && Convert.ToString(HttpContext.Request.QueryString["ServiceRequestID"]) != "propVal")
                {
                    serviceRequest.Mode = OperationMode.Edit;
                    serviceRequest = SetServiceRequest(serviceRequest, "GeneralInfo");
                    serviceRequest = BindServiceRequest(serviceRequest);

                }
                serviceRequest = SetServiceRequest(serviceRequest, "GeneralInfo");
                SetUserGroupDetails();
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Get GeneralInfo");
            }

            return View(serviceRequest);
        }
        [HttpPost]
        [SharePointContextFilter]
        public ActionResult GeneralInfo(ServiceRequest serviceRequest, string btnNext, string Draft, string CREATE, string KEYCODES, string SPECS, string STATUS)
        {
            try
            {
                serviceRequest.Mode = GetServiceRequest().Mode;
                serviceRequest.ProductionSpecialist = GetServiceRequest().ProductionSpecialist;
                serviceRequest.Audience = GetServiceRequest().Audience;
                GetServiceRequest().ScrapOldKeyCode = GetFieldChoices("ServiceRequest", "ScrapOldKeyCode");
                if (!string.IsNullOrEmpty(serviceRequest.FileNameList))
                {
                    List<FileDetails> uploadsFileDetails = (List<FileDetails>)Newtonsoft.Json.JsonConvert.DeserializeObject(serviceRequest.FileNameList, typeof(List<FileDetails>));
                    if (serviceRequest.Files == null)
                    {
                        serviceRequest.Files = new List<FileDetails>();
                    }
                    foreach (FileDetails f in uploadsFileDetails)
                    {
                        serviceRequest.Files.Add(f);
                    }
                    serviceRequest.FileNameList = JsonConvert.SerializeObject(serviceRequest.Files);
                }
                serviceRequest = SetServiceRequest(serviceRequest, "GeneralInfo");
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);
                if (btnNext != null)
                {
                    if ((serviceRequest.Mode == OperationMode.Insert && serviceRequest.RequestAction != ServiceRequestAction.Clone) || (serviceRequest.Mode == OperationMode.Edit && (serviceRequest.KeyCodeDetails == null || serviceRequest.KeyCodeDetails.Count == 0)))
                    {
                        List<KeyCodes> ci = new List<KeyCodes> { new KeyCodes { KeyCodeID = 0, OldKeyCode = "", ReferenceKeyCode = "", Description = "", NewKeyCode = "" } };
                        serviceRequest.KeyCodeDetails = ci;
                    }
                    return View("KeyCodesInfo", serviceRequest);
                }
                if (Draft != null)
                {
                    serviceRequest = SaveServiceRequest(serviceRequest, ServiceRequestAction.Draft);
                    AddHistoryDetails("Save as draft => General Information");
                    return View("ReditrectToSharePoint");
                }
                if (CREATE != null || KEYCODES != null || SPECS != null || STATUS != null)
                {
                    return RedirectTabView(serviceRequest, CREATE, KEYCODES, SPECS, STATUS);
                }
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Post GeneralInfo");
            }

            return View(serviceRequest);
        }

        [HttpGet]
        [SharePointContextFilter]
        public ActionResult KeyCodesInfo(ServiceRequest serviceRequest)
        {

            try
            {
                GetServiceRequest().ScrapOldKeyCode = GetFieldChoices("ServiceRequest", "ScrapOldKeyCode");
                serviceRequest = GetServiceRequest();
                serviceRequest.Mode = GetServiceRequest().Mode;
                if ((serviceRequest.Mode == OperationMode.Insert && serviceRequest.RequestAction != ServiceRequestAction.Clone) || (serviceRequest.Mode == OperationMode.Edit && serviceRequest.KeyCodeDetails == null))
                {
                    List<KeyCodes> ci = new List<KeyCodes> { new KeyCodes { KeyCodeID = 0, OldKeyCode = "", ReferenceKeyCode = "", Description = "", NewKeyCode = "" } };
                    serviceRequest.KeyCodeDetails = ci;
                }
                //serviceRequest.ScrapOldKeyCode = GetFieldChoices("ServiceRequest", "ScrapOldKeyCode");
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Get KeyCodesInfo");
            }


            //return View(ci);

            return View(serviceRequest);
        }
        [HttpPost]
        [SharePointContextFilter]
        public ActionResult KeyCodesInfo(ServiceRequest serviceRequest, string btnNext, string Draft, string CREATE, string KEYCODES, string SPECS, string STATUS)
        {
            try
            {
                serviceRequest.flag = "2";
                serviceRequest.Mode = GetServiceRequest().Mode;
                serviceRequest = SetServiceRequest(serviceRequest, "KeyCodesInfo");
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);
                if (btnNext != null)
                {
                    serviceRequest.ClientName = GetLookUpList("ClientMaster", "ID", "Title");
                    serviceRequest.FinalOutput = GetLookUpList("FinalOutput", "ID", "Title");
                    serviceRequest.AdditionalOutput = GetLookUpList("FinalOutput", "ID", "Title");
                    //serviceRequest.ApproxSize = GetFieldChoices("ServiceRequest", "ApproxSize");
                    //serviceRequest.Colors = GetFieldChoices("ServiceRequest", "Colors");
                    return View("SpecInfo", serviceRequest);
                }
                if (Draft != null)
                {
                    serviceRequest = SaveServiceRequest(serviceRequest, ServiceRequestAction.Draft);
                    AddHistoryDetails("Save as draft => Key Codes");
                    return View("ReditrectToSharePoint");
                }
                if (CREATE != null || KEYCODES != null || SPECS != null || STATUS != null)
                {
                    return RedirectTabView(serviceRequest, CREATE, KEYCODES, SPECS, STATUS);
                }

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Post KeyCodesInfo");
            }

            return View(serviceRequest);
        }
        [HttpGet]
        [SharePointContextFilter]
        public ActionResult SpecInfo()
        {
            ServiceRequest serviceRequest = null;
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);
                serviceRequest = GetServiceRequest();
                serviceRequest.Mode = GetServiceRequest().Mode;
                serviceRequest.ClientName = GetLookUpList("ClientMaster", "ID", "Title");
                serviceRequest.FinalOutput = GetLookUpList("FinalOutput", "ID", "Title");
                serviceRequest.AdditionalOutput = GetLookUpList("FinalOutput", "ID", "Title");
                //serviceRequest.ApproxSize = GetFieldChoices("ServiceRequest", "ApproxSize");
                //serviceRequest.Colors = GetFieldChoices("ServiceRequest", "Colors");

                AddAppContextToViewBag(this, HttpContext, spContext);
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Get SpecInfo");
            }


            //serviceRequest.Designer = GetSelectListItems(GetAllStates());


            return View("SpecInfo", serviceRequest);
            //return View(GetServiceRequest());
        }
        [HttpPost]
        [SharePointContextFilter]
        public ActionResult SpecInfo(ServiceRequest serviceRequest, string btnNext, string Submit, string Clone, string Draft, string CREATE, string KEYCODES, string SPECS, string STATUS)
        {
            try
            {
                serviceRequest.flag = "2";
                GetServiceRequest().StatusList = GetFieldChoices("ServiceRequest", "RequestStatus");
                serviceRequest.Mode = GetServiceRequest().Mode;
                serviceRequest = SetServiceRequest(serviceRequest, "SpecInfo");
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);


                if (Clone != null)
                {
                    serviceRequest.RequestAction = ServiceRequestAction.Clone;
                    GetServiceRequest().Mode = OperationMode.Insert;
                    serviceRequest.Mode = OperationMode.Insert;
                    serviceRequest = SetServiceRequest(serviceRequest, "SpecInfo");
                    //serviceRequest.ProductionSpecialist = GetUsersFromGroup("Production Managers");
                    GetServiceRequest().RequestStatus = null;
                    serviceRequest.RequestStatus = null;
                    AddHistoryDetails("Clone => " + serviceRequest.Title);
                    return View("GeneralInfo", serviceRequest);
                }
                else if (Submit != null)
                {
                    serviceRequest.RequestStatus = RequestStatus.Submitted.ToString();
                    serviceRequest = SetServiceRequest(serviceRequest, "SpecInfo");
                    serviceRequest = SaveServiceRequest(serviceRequest, ServiceRequestAction.Submit);
                    //AddHistoryDetails(serviceRequest.RequestStatus);
                    AddHistoryDetails(serviceRequest.RequestStatus);
                    AddHistoryDetails("Request Created");
                    return View("CompleteServiceReqest", serviceRequest);
                }
                else if (btnNext != null)
                {
                    serviceRequest = SetServiceRequest(serviceRequest, "SpecInfo");
                    serviceRequest.RequestHistory = GetServiceRequest().RequestHistory;
                    //serviceRequest.ProductionSpecialist = GetUsersFromGroup("Production Managers");
                    return View("StatusInfo", serviceRequest);
                }
                else if (Draft != null)
                {
                    serviceRequest = SetServiceRequest(serviceRequest, "SpecInfo");
                    serviceRequest = SaveServiceRequest(serviceRequest, ServiceRequestAction.Draft);
                    AddHistoryDetails("Save as draft => More Information");
                    return View("ReditrectToSharePoint");
                }
                if (CREATE != null || KEYCODES != null || SPECS != null || STATUS != null)
                {
                    return RedirectTabView(serviceRequest, CREATE, KEYCODES, SPECS, STATUS);
                }

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Post SpecInfo");
            }



            return View(serviceRequest);
        }
        [HttpGet]
        [SharePointContextFilter]
        public ActionResult StatusInfo()
        {
            ServiceRequest serviceRequest = null;
            try
            {
                GetServiceRequest().StatusList = GetFieldChoices("ServiceRequest", "RequestStatus");
                serviceRequest = GetServiceRequest();
                //serviceRequest.ProductionSpecialist = GetUsersFromGroup("Production Managers");
                serviceRequest.Mode = GetServiceRequest().Mode;
                //if ((serviceRequest.Mode == OperationMode.Insert && serviceRequest.RequestAction != ServiceRequestAction.Clone) || (serviceRequest.Mode == OperationMode.Edit && serviceRequest.KeyCodeDetails == null))
                //{
                //    List<KeyCodes> ci = new List<KeyCodes> { new KeyCodes { KeyCodeID = 0, OldKeyCode = "", ReferenceKeyCode = "", Description = "", NewKeyCode = "" } };
                //    serviceRequest.KeyCodeDetails = ci;
                //}
                //serviceRequest.ScrapOldKeyCode = GetFieldChoices("ServiceRequest", "ScrapOldKeyCode");
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);
                //return View(ci);

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Get StatusInfo");
            }


            return View(serviceRequest);
        }
        [HttpPost]
        [SharePointContextFilter]
        public ActionResult StatusInfo(ServiceRequest serviceRequest, string Submit, string Cancel, string CREATE, string KEYCODES, string SPECS, string STATUS)
        {
            try
            {
                serviceRequest.flag = "2";
                GetServiceRequest().StatusList = GetFieldChoices("ServiceRequest", "RequestStatus");
                serviceRequest.Mode = GetServiceRequest().Mode;
                serviceRequest = SetServiceRequest(serviceRequest, "StatusInfo");
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);
                if (Submit != null)
                {
                    serviceRequest = SaveServiceRequest(serviceRequest, ServiceRequestAction.Submit);
                    //AddHistoryDetails(serviceRequest.RequestStatus);
                    AddHistoryDetails(serviceRequest.RequestStatus);
                    AddHistoryDetails("Request Updated");
                    return View("ReditrectToSharePoint");
                }
                else if (Cancel != null)
                {
                    return View("ReditrectToSharePoint");
                }
                if (CREATE != null || KEYCODES != null || SPECS != null || STATUS != null)
                {
                    return RedirectTabView(serviceRequest, CREATE, KEYCODES, SPECS, STATUS);
                }
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Post StatusInfo");
            }



            return View(serviceRequest);
        }
        [HttpGet]
        [SharePointContextFilter]
        public ActionResult CompleteServiceReqest()
        {
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Get CompleteServiceReqest");
            }


            return View(GetServiceRequest());
        }
        [HttpPost]
        [SharePointContextFilter]
        public ActionResult CompleteServiceReqest(ServiceRequest serviceRequest, string btnNext, string Draft, string CREATE, string KEYCODES, string SPECS, string STATUS)
        {
            try
            {


                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);

                if (CREATE != null || KEYCODES != null || SPECS != null || STATUS != null)
                {
                    return RedirectTabView(serviceRequest, CREATE, KEYCODES, SPECS, STATUS);
                }

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Post CompleteServiceReqest");
            }

            return View(serviceRequest);
        }
        [SharePointContextFilter]
        public ServiceRequest SaveServiceRequest(ServiceRequest serviceRequest, ServiceRequestAction action)
        {
            try
            {

                ListItem item = null;
                bool isInsert = false;
                NotificationType emailNotificationType = new NotificationType();

                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);



                serviceRequest.ServiceRequestID = GetServiceRequest().ServiceRequestID;
                serviceRequest.Mode = GetServiceRequest().Mode;
                serviceRequest.RequestAction = GetServiceRequest().RequestAction;
                if (action == ServiceRequestAction.Draft && serviceRequest.Mode == OperationMode.Insert)
                {
                    serviceRequest.RequestAction = ServiceRequestAction.Draft;
                    serviceRequest.Mode = OperationMode.Insert;
                }
                else if (action == ServiceRequestAction.Draft)
                {
                    serviceRequest.RequestAction = ServiceRequestAction.Draft;
                    serviceRequest.Mode = OperationMode.Edit;
                }
                else if (action == ServiceRequestAction.Clone)
                {
                    serviceRequest.RequestAction = ServiceRequestAction.Clone;
                    serviceRequest.Mode = OperationMode.Insert;
                    emailNotificationType = NotificationType.NewRequest;
                }
                else if (action == ServiceRequestAction.Submit && serviceRequest.Mode != OperationMode.Edit)
                {
                    serviceRequest.RequestAction = ServiceRequestAction.Submit;
                    serviceRequest.Mode = OperationMode.Insert;
                    emailNotificationType = NotificationType.NewRequest;
                }
                else if (action == ServiceRequestAction.Submit && serviceRequest.Mode == OperationMode.Edit)
                {
                    emailNotificationType = NotificationType.ChangeRequest;
                }

                var clientContext = spContext.CreateUserClientContextForSPHost();
                clientContext.Load(clientContext.Web.CurrentUser);
                clientContext.ExecuteQuery();

                FieldUserValue[] onBehalfUser = null;
                //FieldUserValue AudienceUser = null;
                FieldUserValue[] forwardRequestToUser = null;
                FieldUserValue[] DesignerUser = null;
                FieldUserValue[] BackupDesignerUser = null;
                FieldUserValue[] ProductionSpecialistUser = null;

                try
                {
                    onBehalfUser = !string.IsNullOrEmpty(serviceRequest.OnBehalf) ? PeoplePickerHelper.SPEnsureMultiUser(clientContext, serviceRequest.OnBehalf) : null;
                    //AudienceUser = !string.IsNullOrEmpty(serviceRequest.Audience) ? PeoplePickerHelper.SPEnsureUser(clientContext, serviceRequest.Audience) : null;
                    forwardRequestToUser = !string.IsNullOrEmpty(serviceRequest.ForwardRequesTo) ? PeoplePickerHelper.SPEnsureMultiUser(clientContext, serviceRequest.ForwardRequesTo) : null;
                    DesignerUser = !string.IsNullOrEmpty(serviceRequest.Designer) ? PeoplePickerHelper.SPEnsureMultiUser(clientContext, serviceRequest.Designer) : null;
                    BackupDesignerUser = !string.IsNullOrEmpty(serviceRequest.BackupDesigner) ? PeoplePickerHelper.SPEnsureMultiUser(clientContext, serviceRequest.BackupDesigner) : null;
                    ProductionSpecialistUser = !string.IsNullOrEmpty(serviceRequest.ProductionSpecialist) ? PeoplePickerHelper.SPEnsureMultiUser(clientContext, serviceRequest.ProductionSpecialist) : null;

                }
                catch (Exception ex)
                {

                    Utility.Logging.LogErrorException(ex, "Save Service request()=> Error in binding users");

                }

                List serviceRequestList = clientContext.Web.Lists.GetByTitle("ServiceRequest");
                List keyCodeDetailList = clientContext.Web.Lists.GetByTitle("KeyCodeDetail");
                clientContext.Load(serviceRequestList);
                clientContext.Load(keyCodeDetailList);
                clientContext.ExecuteQuery();
                //Generalinfo


                if (serviceRequest.Mode == OperationMode.Edit)
                {
                    item = serviceRequestList.GetItemById(GetServiceRequest().ServiceRequestID);
                    clientContext.Load(serviceRequestList);
                    clientContext.ExecuteQuery();
                }
                else
                {
                    item = serviceRequestList.AddItem(new ListItemCreationInformation());
                    GetServiceRequest().ServiceRequestID = item.Id;
                    isInsert = true;
                }


                item["Title"] = serviceRequest.Title;


                //item["OnBehalf"] = PeoplePickerHelper.SPEnsureUser(clientContext, serviceRequest.OnBehalf);
                item["Audience"] = serviceRequest.AudienceLookUpID;
                //item["Audience"] = PeoplePickerHelper.SPEnsureUser(clientContext, serviceRequest.Audience);
                //item.Update();
                //For Multi user  (in Sharepoint if Multi user allow is yes)
                //item["OnBehalf"] = PeoplePickerHelper.SPEnsureMultiUser(clientContext, serviceRequest.OnBehalf);
                // item.Update();  //must item.update for update user
                //if (AudienceUser != null)
                //    item["Audience"] = AudienceUser;
                //Updatd by V
                // if (forwardRequestToUser != null)
                item["ForwardRequesTo"] = forwardRequestToUser;
                //if (onBehalfUser != null)
                item["OnBehalf"] = onBehalfUser;

                //if (DesignerUser != null)
                item["Designer"] = DesignerUser;
                //if (BackupDesignerUser != null)
                item["BackupDesigner"] = BackupDesignerUser;

                //if (ProductionSpecialistUser != null)
                item["ProductionSpecialist"] = ProductionSpecialistUser;

                item["ExpectedCompletionDate"] = serviceRequest.ExpectedCompletionDate;
                item["GraphicsCopy"] = serviceRequest.GraphicsCopy;
                item["Graphics1stProof"] = serviceRequest.Graphics1stProof;
                item["FinalApproval"] = serviceRequest.FinalApproval;
                item["ToProductionFilm"] = serviceRequest.ToProductionFilm;
                item["PrintedDelivery"] = serviceRequest.PrintedDelivery;


                //Additional Information
                item["Descriptions"] = serviceRequest.Descriptions;




                //Specs information
                item["BudgetCode"] = serviceRequest.BudgetCode;
                item["ClientName"] = serviceRequest.ClientNameLookUpID;
                item["FinalOutput"] = serviceRequest.FinalOutputLookUpID;
                item["AdditionalOutput"] = serviceRequest.AdditionalOutputLookUpID;
                item["PrintQuantity"] = serviceRequest.PrintQuantity;
                item["AdditionalInfo"] = serviceRequest.AdditionalInfo;
                item["InstructionForDelivery"] = serviceRequest.InstructionForDelivery;
                item["Paper"] = serviceRequest.Paper;
                item["Bindery"] = serviceRequest.Bindery;
                item["Pages"] = serviceRequest.Pages;
                item["ApproxSize"] = serviceRequest.ApproxSize;
                item["Colors"] = serviceRequest.Colors;
                item["RequestStatus"] = serviceRequest.RequestStatus;

                item["ProductionSpecialistNeeded"] = false;
                if (serviceRequest.ProductionSpecialistNeeded != null)
                    item["ProductionSpecialistNeeded"] = serviceRequest.ProductionSpecialistNeeded == "Yes" ? true : false;
                //item["ProductionSpecialist"] = serviceRequest.SelectedProductionSpecialist;

                item["DateAssigned"] = serviceRequest.DateAssigned;
                item["DateCompleted1"] = serviceRequest.DateCompleted;


                if (serviceRequest.RequestAction == ServiceRequestAction.Draft)
                {
                    item["RequestStatus"] = serviceRequest.RequestAction.ToString();
                }
                //if (serviceRequest.DateCompleted != null && !string.IsNullOrEmpty(Convert.ToString(serviceRequest.DateCompleted)))
                //{
                //    item["RequestStatus"] = RequestStatus.Completed.ToString();
                //}
                item["ScrapOldKeyCode"] = serviceRequest.SelectedScrapOldKeyCode;
                try
                {
                    if (serviceRequest.KeyCodeDetails != null && serviceRequest.KeyCodeDetails.Count >= 1)
                    {
                        foreach (KeyCodes keycode in serviceRequest.KeyCodeDetails)
                        {
                            item["Old_x0020_Keycode"] = keycode.OldKeyCode;
                            item["New_x0020_Keycode"] = keycode.NewKeyCode;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utility.Logging.LogErrorException(ex, "Set Oldkey and NewKey");
                }
                item.Update();
                clientContext.ExecuteQuery();

                if (GetServiceRequest().ServiceRequestID == -1 || GetServiceRequest().ServiceRequestID == 0)
                {
                    Utility.Logging.LogErrorException(null,"GetServiceRequest().ServiceRequestID=" + GetServiceRequest().ServiceRequestID.ToString());
                    try
                    {
                        GetServiceRequest().ServiceRequestID = item.Id;
                    }
                    catch(Exception ex)
                    {
                        Utility.Logging.LogErrorException(ex, "Set Oldkey and NewKey");
                    }
                    
                }

                if (serviceRequest.Files != null && serviceRequest.Files.Count > 0)
                {
                    try
                    {
                        string shareFileFolder = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["ShareFileFolder"]);

                        if (!string.IsNullOrEmpty(shareFileFolder))
                        {
                            if (!Directory.Exists(shareFileFolder))
                            {
                                Directory.CreateDirectory(shareFileFolder);
                            }
                            string ShareItemFolder = Path.Combine(shareFileFolder, GetServiceRequest().ServiceRequestID.ToString());
                            if (!Directory.Exists(ShareItemFolder))
                            {
                                Directory.CreateDirectory(ShareItemFolder);
                            }
                            foreach (FileDetails file in serviceRequest.Files)
                            {
                                try
                                {
                                    string tempPath = Path.Combine(Server.MapPath("~/Uploads"), file.FileId);
                                    if (file.Status == FileStatus.Delete)
                                    {
                                        if (!System.IO.File.Exists(Path.Combine(ShareItemFolder, file.BaseName)))
                                        {
                                            System.IO.File.Delete(Path.Combine(ShareItemFolder, file.BaseName));
                                        }
                                    }
                                    else
                                    {
                                        if (!System.IO.File.Exists(Path.Combine(ShareItemFolder, file.BaseName)))
                                        {
                                            System.IO.File.Copy(tempPath, Path.Combine(ShareItemFolder, file.BaseName), true);
                                            try
                                            {
                                                System.IO.File.Delete(tempPath);
                                            }
                                            catch(Exception ex)
                                            {
                                                Utility.Logging.LogErrorException(ex, "SaveServiceRequest() =>Deleting file...");

                                            }

                                        }
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Utility.Logging.LogErrorException(ex, "SaveServiceRequest() => Error Adding attachments...");
                                }
                                

                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Utility.Logging.LogErrorException(ex, "SaveServiceRequest() => Error Adding attachments counts...");
                    }
                    
                }
                GetServiceRequest().Mode = OperationMode.Edit;
                GetServiceRequest().RequestStatus = serviceRequest.RequestStatus;
                if (serviceRequest.RequestStatus == RequestStatus.Approved.ToString())
                {
                    emailNotificationType = NotificationType.FinalApproval;
                }
                //Key Code Details
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='ServiceRequestID'/>" +
                    "<Value Type='Text'>" + serviceRequest.ServiceRequestID + "</Value></Eq></Where></Query></View>";


                ListItemCollection collListItem = keyCodeDetailList.GetItems(camlQuery);
                clientContext.Load(collListItem);
                clientContext.ExecuteQuery();
                if (collListItem != null && collListItem.Count > 0)
                {
                    try
                    {
                        for (int i = collListItem.Count - 1; i >= 0; i--)
                        {
                            string id = collListItem[i].Id.ToString();
                            collListItem[i].DeleteObject();

                            clientContext.ExecuteQuery();
                        }
                    }

                    catch (Exception ex)
                    {
                        Utility.Logging.LogErrorException(ex, "SaveServiceRequest() => Error in Key Code Deleting...");
                    }
                }

                if (serviceRequest.KeyCodeDetails != null && serviceRequest.KeyCodeDetails.Count > 0)
                {
                    foreach (KeyCodes keycode in serviceRequest.KeyCodeDetails)
                    {
                        if (!keycode.IsDeleted)
                        {
                            ListItem keyCodeItem = keyCodeDetailList.AddItem(new ListItemCreationInformation());
                            try
                            {

                                if (serviceRequest.Mode == OperationMode.Edit)
                                {
                                    keyCodeItem["ServiceRequestID"] = serviceRequest.ServiceRequestID;
                                    if (serviceRequest.ServiceRequestID == 0 || serviceRequest.ServiceRequestID == -1)
                                    {
                                        keyCodeItem["ServiceRequestID"] = item.Id.ToString();
                                    }
                                }
                                else
                                {
                                    keyCodeItem["ServiceRequestID"] = item.Id.ToString();
                                    serviceRequest.ServiceRequestID = item.Id;
                                }

                                keyCodeItem["OldKeyCode"] = keycode.OldKeyCode;
                                keyCodeItem["ReferenceKeyCode"] = keycode.ReferenceKeyCode;
                                keyCodeItem["Descriptions"] = keycode.Description;
                                keyCodeItem["NewKeyCode"] = keycode.NewKeyCode;
                                keyCodeItem.Update();
                                clientContext.ExecuteQuery();
                            }
                            catch (Exception ex)
                            {
                                Utility.Logging.LogErrorException(ex, "SaveServiceRequest() => Error in inserting Key Code...");
                            }

                        }
                        else
                        {
                            Utility.Logging.LogErrorException(null, "SaveServiceRequest() => Key Code is deletd = true...");
                        }
                    }
                }
                else
                {
                    Utility.Logging.LogErrorException(null, "serviceRequest.KeyCodeDetails detail getting null.");
                }
                if (isInsert)
                {
                    Utility.Logging.LogErrorException(null, "Current Logedin User:" + clientContext.Web.CurrentUser.LoginName);
                    item = serviceRequestList.GetItemById(GetServiceRequest().ServiceRequestID);
                    clientContext.Load(item);
                    clientContext.ExecuteQuery();
                    item["Author"] = clientContext.Web.CurrentUser.Id;
                    item.Update();
                    clientContext.ExecuteQuery();


                }
                if (action == ServiceRequestAction.Submit)
                {
                    SendServiceRequestEmail(emailNotificationType);
                }



            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "SaveServiceRequest()");
            }

            return serviceRequest;
        }
        //[HttpPost]
        //[SharePointContextFilter]
        //public ActionResult CompleteServiceReqest(ServiceRequest serviceRequest)
        //{
        //    SetServiceRequest(serviceRequest, "CompleteServiceReqest");
        //    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
        //    AddAppContextToViewBag(this, HttpContext, spContext);

        //    return View("CompleteServiceReqest");
        //}


        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <value>
        /// The base URL.
        /// </value>
        public string BaseUrl
        {
            get
            {
                return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            }
        }



        public JsonResult UploadFile(string qqfile)
        {
            FileDetails fileobj = null;
            try
            {
                var stream = this.Request.InputStream;
                string id = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(qqfile);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(qqfile) + "_" + id.Replace("_", string.Empty).Substring(0, 8) + System.IO.Path.GetExtension(qqfile);
                if (string.IsNullOrEmpty(this.Request["qqfile"]))
                {
                    // IE Fix
                    HttpPostedFileBase postedFile = this.Request.Files[0];
                    stream = postedFile.InputStream;

                }
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(stream))
                {
                    fileData = binaryReader.ReadBytes((int)stream.Length);
                }
                System.IO.File.WriteAllBytes(Server.MapPath("~/Uploads/" + id), fileData);
                fileobj = new FileDetails() { FileId = id, FileName = fileName, FileURL = this.BaseUrl.Trim('/') + Url.Content("~/Uploads/" + id), Status = FileStatus.New, BaseName = qqfile };
                List<FileDetails> files = this.GetTempData<List<FileDetails>>("TempFiles");
                if (files == null)
                {
                    files = new List<FileDetails>();
                }
                files.Add(fileobj);
                this.SetTempData<List<FileDetails>>("TempFiles", files);
                //ServiceRequest GlobalServiceRequest = GetServiceRequest();
                //if (GlobalServiceRequest.Files == null)
                //    GlobalServiceRequest.Files = new List<FileDetails>();

                //GlobalServiceRequest.Files.Add(fileobj);
                // var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                //AddAppContextToViewBag(this, HttpContext, spContext);

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Get Index");
            }

            return this.Json(fileobj);
        }
        public T GetTempData<T>(string key) where T : new()
        {
            if (this.TempData[key.ToString()] == null)
            {
                this.TempData[key.ToString()] = new T();
            }
            this.TempData.Keep(key.ToString());
            return (T)this.TempData[key.ToString()];
        }

        public void SetTempData<T>(string key, T obj)
        {
            this.TempData[key.ToString()] = obj;
            this.TempData.Keep(key.ToString());
        }

        public JsonResult RemoveUploadFile(string id)
        {
            if (System.IO.File.Exists(Server.MapPath("~/Uploads/" + id)))
            {
                try
                {
                    System.IO.File.Delete(Server.MapPath("~/Uploads/" + id));
                }
                catch (Exception ex)
                {
                    Utility.Logging.LogErrorException(ex, "RemoveUploadFile()");
                }
            }
            return this.Json(new FileDetails() { FileId = id });
        }
        public JsonResult VerifyDownloadFile(string url, string applicationName)
        {
            string this_id = "101";
            string this_name = "20.1";
            // do here some operation  
            return Json(new { url = this_id, applicationName = this_name, Status = 1 }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //[SharePointContextFilter]
        public ActionResult DownloadFile(string url, string currentLocation, string applicationName)
        {
            try
            {
                byte[] fileData = null;
                if (url != null)
                {
                    if (url.Contains("/Uploads/"))
                    {
                        using (WebClient myWebClient = new WebClient())
                        {
                            fileData = myWebClient.DownloadData(url);
                        }
                    }
                    else
                    {
                        var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

                        //using (var fileInformation =  Microsoft.SharePoint.Client.File.OpenBinaryDirect(spContext, url))
                        //{
                        //    IList<byte> content = new List<byte>();
                        //    int b;
                        //    while ((b = fileInformation.Stream.ReadByte()) != -1)
                        //    {
                        //        content.Add((byte)b);
                        //    }
                        //    fileData= content.ToArray();
                        //}


                    }
                }
                return this.File(fileData, "application/octet-stream", Path.GetFileName(url));
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "DownloadFile()");
                return this.Redirect(currentLocation);
            }
        }

        public static void AddAppContextToViewBag(Controller controller, HttpContextBase httpContext, SharePointContext spContext)
        {

            try
            {
                if (httpContext == null) throw new ArgumentNullException("httpContext");
                if (spContext == null) throw new ArgumentNullException("spContext");

                var vb = controller.ViewBag;
                vb.SPHostUrl = spContext.SPHostUrl.ToString().TrimEnd(new[] { '/' });
                vb.SPAppWebUrl = spContext.SPAppWebUrl.ToString().TrimEnd(new[] { '/' });
                vb.SPClientTag = spContext.SPClientTag;
                vb.SPLanguage = spContext.SPLanguage;
                vb.SPSourceUrl = httpContext.Request.QueryString["SPSourceUrl"] ?? string.Empty;

                vb.IsDialog = (httpContext.Request.QueryString["IsDlg"] != null) &&
                              (httpContext.Request.QueryString["IsDlg"].Substring(0, 1) == "1");
                vb.IsDialogParam = vb.IsDialog ? "1" : "0";
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "AddAppContextToViewBag()");
            }
        }
        private ServiceRequest GetServiceRequest()
        {
            if (Session["ServiceRequest"] == null)
            {
                Session["ServiceRequest"] = new ServiceRequest();
            }
            return (ServiceRequest)Session["ServiceRequest"];
        }

        private IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<string> elements)
        {
            // Create an empty list to hold result of the operation
            var selectList = new List<SelectListItem>();
            try
            {
                // For each string in the 'elements' variable, create a new SelectListItem object
                // that has both its Value and Text properties set to a particular value.
                // This will result in MVC rendering each item as:
                //     <option value="State Name">State Name</option>
                foreach (var element in elements)
                {
                    selectList.Add(new SelectListItem
                    {
                        Value = element,
                        Text = element
                    });
                }

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "GetSelectListItems()");
            }


            return selectList;
        }

        private IEnumerable<SelectListItem> GetLookUpList(string ListInternalName, string valueField, string textField)
        {
            List<SelectListItem> clients = new List<SelectListItem>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                ListItemCollection items = null;
                var clientContext = spContext.CreateUserClientContextForSPHost();


                // Assume the web has a list named "Announcements". 
                List lstClientMaster = clientContext.Web.Lists.GetByTitle(ListInternalName);

                // This creates a CamlQuery that has a RowLimit of 100, and also specifies Scope="RecursiveAll" 
                // so that it grabs all list items, regardless of the folder they are in. 
                CamlQuery query = CamlQuery.CreateAllItemsQuery();

                items = lstClientMaster.GetItems(query);

                // Retrieve all items in the ListItemCollection from List.GetItems(Query). 
                clientContext.Load(items);
                clientContext.ExecuteQuery();
                foreach (ListItem item in items)
                {
                    clients.Add(new SelectListItem { Value = item[valueField].ToString(), Text = item[textField].ToString() });
                }

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "GetLookUpList()");
            }


            return new SelectList(clients, "Value", "Text");
        }
        private IEnumerable<SelectListItem> GetUsersFromGroup(string GroupName)
        {
            List<SelectListItem> clients = new List<SelectListItem>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                var clientContext = spContext.CreateUserClientContextForSPHost();


                // Assume the web has a list named "Announcements". 
                Microsoft.SharePoint.Client.Group group = clientContext.Web.SiteGroups.GetByName(GroupName);

                clientContext.Load(group.Users);
                clientContext.ExecuteQuery();
                if (group.Users != null && group.Users.Count > 0)
                {
                    foreach (Microsoft.SharePoint.Client.User user in group.Users)
                    {
                        clients.Add(new SelectListItem { Value = user.Id.ToString(), Text = user.Title });
                    }
                }


            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "GetUsersFromGroup()");
            }


            return new SelectList(clients, "Value", "Text");
        }
        private IEnumerable<SelectListItem> GetFieldChoices(string ListInternalName, string internalFieldName)
        {
            List<SelectListItem> choices = new List<SelectListItem>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

                var clientContext = spContext.CreateUserClientContextForSPHost();


                // Assume the web has a list named "Announcements". 
                List lstClientMaster = clientContext.Web.Lists.GetByTitle(ListInternalName);
                Field choiceField = lstClientMaster.Fields.GetByInternalNameOrTitle(internalFieldName);
                clientContext.Load(lstClientMaster);
                clientContext.Load(choiceField);
                clientContext.ExecuteQuery();
                if (choiceField.FieldTypeKind == FieldType.Choice)
                {
                    FieldChoice myChoices = clientContext.CastTo<FieldChoice>(choiceField);
                    foreach (string choice in myChoices.Choices)
                    {
                        if (choice != "Draft")
                        {
                            choices.Add(new SelectListItem { Value = choice, Text = choice });
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "GetFieldChoices()");
            }
            return new SelectList(choices, "Value", "Text");
        }

        private ServiceRequest SetServiceRequest(ServiceRequest serviceRequest, string tabInfo)
        {
            ServiceRequest GlobalServiceRequest = GetServiceRequest();
            try
            {
                switch (tabInfo)
                {
                    case "GeneralInfo":
                        GlobalServiceRequest.Mode = serviceRequest.Mode;
                        GlobalServiceRequest.Title = serviceRequest.Title;
                        GlobalServiceRequest.ForwardRequesTo = serviceRequest.ForwardRequesTo;
                        GlobalServiceRequest.AudienceLookUpID = serviceRequest.AudienceLookUpID;
                        GlobalServiceRequest.OnBehalf = serviceRequest.OnBehalf;
                        GlobalServiceRequest.ProductionSpecialistNeeded = serviceRequest.ProductionSpecialistNeeded;
                        GlobalServiceRequest.ExpectedCompletionDate = serviceRequest.ExpectedCompletionDate;
                        GlobalServiceRequest.GraphicsCopy = serviceRequest.GraphicsCopy;
                        GlobalServiceRequest.Graphics1stProof = serviceRequest.Graphics1stProof;
                        GlobalServiceRequest.FinalApproval = serviceRequest.FinalApproval;
                        GlobalServiceRequest.ToProductionFilm = serviceRequest.ToProductionFilm;
                        GlobalServiceRequest.PrintedDelivery = serviceRequest.PrintedDelivery;
                        GlobalServiceRequest.Descriptions = serviceRequest.Descriptions;
                        GlobalServiceRequest.Files = serviceRequest.Files;
                        GlobalServiceRequest.FileNameList = serviceRequest.FileNameList;
                        GlobalServiceRequest.Audience = serviceRequest.Audience;
                        break;
                    case "AdditionalInfo":
                        GlobalServiceRequest.Descriptions = serviceRequest.Descriptions;
                        GlobalServiceRequest.Files = serviceRequest.Files;
                        GlobalServiceRequest.FileNameList = serviceRequest.FileNameList;
                        break;
                    case "KeyCodesInfo":
                        GlobalServiceRequest.KeyCodeDetails = serviceRequest.KeyCodeDetails;
                        GlobalServiceRequest.SelectedScrapOldKeyCode = serviceRequest.SelectedScrapOldKeyCode;
                        break;
                    case "SpecInfo":
                        GlobalServiceRequest.BudgetCode = serviceRequest.BudgetCode;
                        GlobalServiceRequest.ClientNameLookUpID = serviceRequest.ClientNameLookUpID;
                        GlobalServiceRequest.FinalOutputLookUpID = serviceRequest.FinalOutputLookUpID;
                        GlobalServiceRequest.AdditionalOutputLookUpID = serviceRequest.AdditionalOutputLookUpID;
                        GlobalServiceRequest.PrintQuantity = serviceRequest.PrintQuantity;
                        GlobalServiceRequest.AdditionalInfo = serviceRequest.AdditionalInfo;
                        GlobalServiceRequest.InstructionForDelivery = serviceRequest.InstructionForDelivery;
                        GlobalServiceRequest.Paper = serviceRequest.Paper;
                        GlobalServiceRequest.Bindery = serviceRequest.Bindery;
                        GlobalServiceRequest.Pages = serviceRequest.Pages;
                        GlobalServiceRequest.ApproxSize = serviceRequest.ApproxSize;
                        GlobalServiceRequest.Colors = serviceRequest.Colors;
                        GlobalServiceRequest.RequestAction = serviceRequest.RequestAction;
                        GlobalServiceRequest.Mode = serviceRequest.Mode;
                        GlobalServiceRequest.RequestStatus = serviceRequest.RequestStatus;
                        break;
                    case "StatusInfo":
                        GlobalServiceRequest.ProductionSpecialist = serviceRequest.ProductionSpecialist;
                        //GlobalServiceRequest.SelectedProductionSpecialist = serviceRequest.SelectedProductionSpecialist;
                        GlobalServiceRequest.DateAssigned = serviceRequest.DateAssigned;
                        GlobalServiceRequest.DateCompleted = serviceRequest.DateCompleted;
                        GlobalServiceRequest.Designer = serviceRequest.Designer;
                        GlobalServiceRequest.BackupDesigner = serviceRequest.BackupDesigner;
                        GlobalServiceRequest.RequestStatus = serviceRequest.RequestStatus;
                        break;
                    case "Completed":
                        GlobalServiceRequest.ServiceRequestID = serviceRequest.ServiceRequestID;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "SetServiceRequest()");
            }


            // Session["ServiceRequest"] = GlobalServiceRequest;
            return GlobalServiceRequest;
        }
        private ServiceRequest BindServiceRequest(ServiceRequest serviceRequest)
        {
            ServiceRequest GlobalServiceRequest = null;
            try
            {
                GlobalServiceRequest = GetServiceRequest();
                GlobalServiceRequest.ServiceRequestID = Convert.ToInt16(HttpContext.Request.QueryString["ServiceRequestID"].ToString());
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

                var clientContext = spContext.CreateUserClientContextForSPHost();



                // Assume the web has a list named "Announcements". 
                ListItem item = clientContext.Web.Lists.GetByTitle("ServiceRequest").GetItemById(GlobalServiceRequest.ServiceRequestID);
                List keyCodeDetailList = clientContext.Web.Lists.GetByTitle("keyCodeDetail");
                clientContext.Load(keyCodeDetailList);
                clientContext.Load(item);
                clientContext.Load(item.FieldValuesAsText);
                clientContext.ExecuteQuery();
                string value = string.Empty;
                GlobalServiceRequest.Title = Convert.ToString(item["Title"]);

                //GlobalServiceRequest.ForwardRequesTo = Convert.ToString(item["ForwardRequesTo"]);
                //var uservalue = JsonHelper.Deserialize<List<PeoplePickerUser>>(serviceRequest.Audience);
                //if (uservalue.Count > 0)
                //{

                //    object value1 = uservalue.Select(x => new FieldUserValue() { LookupId = x.LookupId }).ToArray();

                //    value1 = uservalue.Select(x => new FieldUserValue() { LookupId = x.LookupId }).First();

                //}
                //for single user
                value = Convert.ToString(item["Audience"]);
                GlobalServiceRequest.AudienceLookUpID = String.IsNullOrEmpty(value) ? null : Convert.ToString(((Microsoft.SharePoint.Client.FieldLookupValue)(item["Audience"])).LookupId);

                //GlobalServiceRequest.Audience = PeoplePickerHelper.GetSPUserJson(clientContext, item["Audience"] as FieldUserValue);
                GlobalServiceRequest.OnBehalf = PeoplePickerHelper.GetSPMultiUserJson(clientContext, item["OnBehalf"] as FieldUserValue[]);
                GlobalServiceRequest.ForwardRequesTo = PeoplePickerHelper.GetSPMultiUserJson(clientContext, item["ForwardRequesTo"] as FieldUserValue[]);
                GlobalServiceRequest.Designer = PeoplePickerHelper.GetSPMultiUserJson(clientContext, item["Designer"] as FieldUserValue[]);
                GlobalServiceRequest.BackupDesigner = PeoplePickerHelper.GetSPMultiUserJson(clientContext, item["BackupDesigner"] as FieldUserValue[]);

                GlobalServiceRequest.ProductionSpecialist = PeoplePickerHelper.GetSPMultiUserJson(clientContext, item["ProductionSpecialist"] as FieldUserValue[]);

                // for multi user 
                //GlobalServiceRequest.OnBehalf = PeoplePickerHelper.GetSPMultiUserJson(clientContext, item["OnBehalf"] as FieldUserValue[]);

                value = Convert.ToString(item["ProductionSpecialistNeeded"]);
                if (value != null && value != "")
                    GlobalServiceRequest.ProductionSpecialistNeeded = Convert.ToBoolean(value) ? "Yes" : "No";


                //if(item["ProductionSpecialist"] != null && (item["ProductionSpecialist"] as FieldUserValue).LookupId !=null)
                //    GlobalServiceRequest.SelectedProductionSpecialist = String.IsNullOrEmpty(Convert.ToString((item["ProductionSpecialist"] as FieldUserValue).LookupId)) ? null : Convert.ToString((item["ProductionSpecialist"] as FieldUserValue).LookupId);


                value = Convert.ToString(item["DateAssigned"]);
                GlobalServiceRequest.DateAssigned = String.IsNullOrEmpty(value) ? null : (DateTime?)Convert.ToDateTime(value);
                value = Convert.ToString(item["DateCompleted1"]);
                GlobalServiceRequest.DateCompleted = String.IsNullOrEmpty(value) ? null : (DateTime?)Convert.ToDateTime(value);
                value = Convert.ToString(item["ExpectedCompletionDate"]);
                GlobalServiceRequest.ExpectedCompletionDate = String.IsNullOrEmpty(value) ? null : (DateTime?)Convert.ToDateTime(value);
                value = Convert.ToString(item["GraphicsCopy"]);
                GlobalServiceRequest.GraphicsCopy = String.IsNullOrEmpty(value) ? null : (DateTime?)Convert.ToDateTime(value);
                value = Convert.ToString(item["Graphics1stProof"]);
                GlobalServiceRequest.Graphics1stProof = String.IsNullOrEmpty(value) ? null : (DateTime?)Convert.ToDateTime(value);
                value = Convert.ToString(item["FinalApproval"]);
                GlobalServiceRequest.FinalApproval = String.IsNullOrEmpty(value) ? null : (DateTime?)Convert.ToDateTime(value);
                value = Convert.ToString(item["ToProductionFilm"]);
                GlobalServiceRequest.ToProductionFilm = String.IsNullOrEmpty(value) ? null : (DateTime?)Convert.ToDateTime(value);
                value = Convert.ToString(item["PrintedDelivery"]);
                GlobalServiceRequest.PrintedDelivery = String.IsNullOrEmpty(value) ? null : (DateTime?)Convert.ToDateTime(value);

                GlobalServiceRequest.SubmitedBy = Convert.ToString(((Microsoft.SharePoint.Client.FieldLookupValue)(item["Author"])).LookupValue);
                GlobalServiceRequest.SubmitedDate = Convert.ToDateTime(Convert.ToString(item["Created"]));

                //AdditionalInfo":
                GlobalServiceRequest.Descriptions = Convert.ToString(item["Descriptions"]);

                string shareFileFolder = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["ShareFileFolder"]);
                string IISWebURL = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["IISSiteURL"]);

                string ShareItemFolder = Path.Combine(shareFileFolder, GetServiceRequest().ServiceRequestID.ToString());
                if (Directory.Exists(ShareItemFolder))
                {

                    GlobalServiceRequest.Files = new List<FileDetails>();
                    string[] ArrayFiles = Directory.GetFiles(ShareItemFolder);
                    foreach (string filenamePath in ArrayFiles)
                    {
                        string fName = Path.GetFileName(filenamePath);
                        GlobalServiceRequest.Files.Add(new FileDetails() { FileId = Guid.NewGuid().ToString(), FileName = fName, FileURL = IISWebURL + "/Uploads/ServiceRequestDocuments/" + GlobalServiceRequest.ServiceRequestID + "/" + fName, Status = FileStatus.NoAction, BaseName = fName });
                    }
                    GlobalServiceRequest.FileNameList = JsonConvert.SerializeObject(GlobalServiceRequest.Files);
                }





                //"KeyCodesInfo":

                //"SpecInfo":
                GlobalServiceRequest.BudgetCode = Convert.ToString(item["BudgetCode"]);
                value = Convert.ToString(item["ClientName"]);
                GlobalServiceRequest.ClientNameLookUpID = String.IsNullOrEmpty(value) ? null : Convert.ToString(((Microsoft.SharePoint.Client.FieldLookupValue)(item["ClientName"])).LookupId);
                value = Convert.ToString(item["FinalOutput"]);
                GlobalServiceRequest.FinalOutputLookUpID = String.IsNullOrEmpty(value) ? null : Convert.ToString(((Microsoft.SharePoint.Client.FieldLookupValue)(item["FinalOutput"])).LookupId);
                value = Convert.ToString(item["AdditionalOutput"]);
                GlobalServiceRequest.AdditionalOutputLookUpID = String.IsNullOrEmpty(value) ? null : Convert.ToString(((Microsoft.SharePoint.Client.FieldLookupValue)(item["AdditionalOutput"])).LookupId);

                GlobalServiceRequest.PrintQuantity = Convert.ToString(item["PrintQuantity"]);

                GlobalServiceRequest.AdditionalInfo = Convert.ToString(item.FieldValuesAsText["AdditionalInfo"]);
                GlobalServiceRequest.InstructionForDelivery = Convert.ToString(item.FieldValuesAsText["InstructionForDelivery"]);
                GlobalServiceRequest.Paper = Convert.ToString(item["Paper"]);
                GlobalServiceRequest.Bindery = Convert.ToString(item["Bindery"]);
                GlobalServiceRequest.Pages = Convert.ToString(item["Pages"]);

                GlobalServiceRequest.ApproxSize = Convert.ToString(item["ApproxSize"]);

                GlobalServiceRequest.Colors = Convert.ToString(item["Colors"]);

                //keyCode
                value = Convert.ToString(item["ScrapOldKeyCode"]);
                GlobalServiceRequest.SelectedScrapOldKeyCode = String.IsNullOrEmpty(value) ? null : Convert.ToString(item["ScrapOldKeyCode"]);

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='ServiceRequestID'/>" +
                    "<Value Type='Text'>" + item.Id + "</Value></Eq></Where></Query></View>";

                try
                {
                    ListItemCollection collListItem = keyCodeDetailList.GetItems(camlQuery);
                    clientContext.Load(collListItem);

                    clientContext.ExecuteQuery();
                    if (collListItem != null && collListItem.Count > 0)
                    {
                        GlobalServiceRequest.KeyCodeDetails = new List<KeyCodes>();
                        foreach (ListItem keyCodeItem in collListItem)
                        {
                            try
                            {
                                KeyCodes keycode = new KeyCodes();
                                keycode.OldKeyCode = Convert.ToString(keyCodeItem["OldKeyCode"]);
                                keycode.ReferenceKeyCode = Convert.ToString(keyCodeItem["ReferenceKeyCode"]); ;
                                keycode.Description = GetPlainTextFromHtml(Convert.ToString(keyCodeItem["Descriptions"]));
                                keycode.NewKeyCode = Convert.ToString(keyCodeItem["NewKeyCode"]); ;
                                GlobalServiceRequest.KeyCodeDetails.Add(keycode);
                            }
                            catch (Exception ex)
                            {
                                Utility.Logging.LogErrorException(ex, "BindServiceRequest()=> Error in key Code For Looping");
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Utility.Logging.LogErrorException(ex, "BindServiceRequest()=> Error in Check Caml Query");
                }

                value = Convert.ToString(item["RequestStatus"]);
                GlobalServiceRequest.RequestStatus = String.IsNullOrEmpty(value) ? null : Convert.ToString(item["RequestStatus"]);
                SetServiceRequestHistory();



            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "BindServiceRequest()");
            }



            return GlobalServiceRequest;
        }


        private string GetPlainTextFromHtml(string htmlString)
        {
            string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace("&nbsp;", string.Empty);

            return htmlString;
        }
        [SharePointContextFilter]
        public void SetUserGroupDetails()
        {
            try
            {
                if (GetServiceRequest().UserGroups == null)
                {
                    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

                    var clientContext = spContext.CreateUserClientContextForSPHost();

                    if (clientContext != null)
                    {




                        //return results;
                        //                      HttpWebRequest endpointRequest =
                        //(HttpWebRequest)HttpWebRequest.Create("http://mysites.dc14.loc/sites/PS102/_api/web/sitegroups/getbyname("+"'Business%20Partners'"+")/CanCurrentUserViewMembership");
                        //                      endpointRequest.Method = "POST";
                        //                      endpointRequest.Accept = "application/json;odata=verbose";
                        //                      endpointRequest.ContentLength = 0;
                        //                      HttpWebResponse endpointResponse =
                        //                        (HttpWebResponse)endpointRequest.GetResponse();
                        bool IsAdUser = true;
                        Microsoft.SharePoint.Client.GroupCollection groups = clientContext.Web.CurrentUser.Groups;
                        clientContext.Load(clientContext.Web.CurrentUser);
                        clientContext.Load(groups);
                        clientContext.ExecuteQuery();
                        GetServiceRequest().UserGroups = groups;

                        GetServiceRequest().EditformAuthorisedAccess = false;
                        GetServiceRequest().EnableStatusField = false;

                        string[] groupName = new string[] { "Administrators", "Production Managers", "Graphic Artists", "Business Partners", "Brand Review" };
                        if (groups.Any(gp => gp.Title == "Administrators" || gp.Title == "Production Managers" || gp.Title == "Graphic Artists"))
                        {
                            GetServiceRequest().ShowNewKeyCode = true;
                            GetServiceRequest().ShowProductionSpeciaList = true;
                            GetServiceRequest().EditformAuthorisedAccess = true;
                            GetServiceRequest().EnableStatusField = true;
                            IsAdUser = false;
                        }
                        if (groups.Any(gp => gp.Title == "Business Partners" || gp.Title == "Brand Review"))
                        {
                            GetServiceRequest().EnableStatusField = true;
                            GetServiceRequest().EditformAuthorisedAccess = true;
                            IsAdUser = false;
                        }
                        if (IsAdUser)
                        {
                            string currentUserName = clientContext.Web.CurrentUser.LoginName;
                            Utility.Logging.LogErrorException(null, "current logedin user=" + currentUserName);
                            foreach (string str in groupName)
                            {
                                if (checkIsMemberExists(str))
                                {
                                    GetServiceRequest().EnableStatusField = true;
                                    GetServiceRequest().EditformAuthorisedAccess = true;
                                    if (str == "Administrators" || str == "Production Managers" || str == "Graphic Artists")
                                    {
                                        GetServiceRequest().ShowNewKeyCode = true;
                                        GetServiceRequest().ShowProductionSpeciaList = true;
                                        break;
                                    }

                                }
                            }
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "SetUserGroupDetails()");
            }

        }
        [SharePointContextFilter]
        public bool checkIsMemberExists(string grpName)
        {
            bool isMember = false;
            try
            {
                RetrieveAccessToken();

                HttpWebRequest request = HttpWebRequest.CreateHttp(String.Format("{0}/_api/web/sitegroups/getbyname('" + grpName + "')/CanCurrentUserViewMembership", HttpContext.Request.QueryString["SPHostUrl"]));
                request.Accept = "application/json;odata=verbose";
                request.Headers.Add("Authorization", accessToken);
                Stream postStream = request.GetResponse().GetResponseStream();


                StreamReader postReader = new StreamReader(postStream);


                var result = new JavaScriptSerializer().Deserialize<dynamic>(postReader.ReadToEnd());
                if (result != null)
                {
                    isMember = Convert.ToBoolean(result["d"]["CanCurrentUserViewMembership"]);
                    Utility.Logging.LogErrorException(null, "checkIsMemberExists () : Member GroupName: " + grpName + " isMember= " + isMember.ToString());
                }

            }
            catch (Exception ex)
            {
                isMember = false;
                Utility.Logging.LogErrorException(ex, "checkIsMemberExists () ");
            }
            return isMember;
        }
        public void RetrieveAccessToken()
        {
            ClientContext ctx = SharePointContextProvider.Current.GetSharePointContext(HttpContext).CreateUserClientContextForSPHost();
            ctx.ExecutingWebRequest += ctx_ExecutingWebRequest;
            ctx.ExecuteQuery();
        }

        private void ctx_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            accessToken = e.WebRequestExecutor.RequestHeaders.Get("Authorization");
        }
        public void SetServiceRequestHistory()
        {
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

                var clientContext = spContext.CreateUserClientContextForSPHost();

                if (clientContext != null)
                {
                    List RequestHistory = clientContext.Web.Lists.GetByTitle("RequestHistory");
                    clientContext.Load(RequestHistory);
                    clientContext.ExecuteQuery();

                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = "<View><Query><Where><Eq><FieldRef Name='ServiceRequestID'/>" +
                        "<Value Type='Text'>" + GetServiceRequest().ServiceRequestID + "</Value></Eq></Where><OrderBy><FieldRef Name='ID'  /></OrderBy></Query></View>";


                    ListItemCollection collListItem = RequestHistory.GetItems(camlQuery);
                    clientContext.Load(collListItem);

                    clientContext.ExecuteQuery();
                    List<ServiceRequestHistory> historyList = new List<ServiceRequestHistory>();
                    if (collListItem != null && collListItem.Count >= 1)
                    {
                        for (int y = collListItem.Count - 1; y >= 0; y--)
                        {
                            ServiceRequestHistory historyObj = new ServiceRequestHistory();
                            historyObj.Description = Convert.ToString(collListItem[y]["Descriptions"]);
                            historyObj.UserModifiedBy = Convert.ToString(((Microsoft.SharePoint.Client.FieldUserValue)collListItem[y]["UserModifiedBy"]).LookupValue); ;
                            historyObj.UserModifiedDate = Convert.ToString(collListItem[y]["UserModifiedDate"]);

                            historyList.Add(historyObj);
                        }
                    }

                    GetServiceRequest().RequestHistory = historyList;
                }

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "SetServiceRequestHistory()");
            }



        }
        [SharePointContextFilter]
        public void AddHistoryDetails(string action)
        {
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

                var clientContext = spContext.CreateUserClientContextForSPHost();

                if (clientContext != null)
                {
                    List RequestHistory = clientContext.Web.Lists.GetByTitle("RequestHistory");

                    clientContext.Load(RequestHistory);
                    clientContext.Load(clientContext.Web.CurrentUser);
                    clientContext.ExecuteQuery();

                    ListItem itemHistory = RequestHistory.AddItem(new ListItemCreationInformation());
                    itemHistory["ServiceRequestID"] = GetServiceRequest().ServiceRequestID;
                    itemHistory["UserModifiedBy"] = clientContext.Web.CurrentUser.Id;
                    itemHistory["Descriptions"] = action;
                    itemHistory["UserModifiedDate"] = DateTime.Now.ToString();
                    itemHistory.Update();
                    clientContext.ExecuteQuery();
                    SetServiceRequestHistory();

                }


            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "AddHistoryDetails()");
            }


        }

        [SharePointContextFilter]
        public ActionResult GetPeoplePickerData()
        {
            //peoplepickerhelper will get the needed values from the querrystring, get data from sharepoint, and return a result in Json format
            // return PeoplePickerHelper.GetPeoplePickerSearchData();
            return this.Json(PeoplePickerHelper.GetPeoplePickerSearchData());
        }
        [SharePointContextFilter]
        public void SendServiceRequestEmail(NotificationType emailType)
        {
            try
            {

                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

                var clientContext = spContext.CreateUserClientContextForSPHost();

                if (clientContext != null)
                {

                    List EmailConfigurationList = clientContext.Web.Lists.GetByTitle("EmailConfiguration");
                    clientContext.Load(EmailConfigurationList);
                    clientContext.ExecuteQuery();
                    Utility.Logging.LogErrorException(null, "Get item From ");
                    CamlQuery query = new Microsoft.SharePoint.Client.CamlQuery();
                    query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + emailType.ToString() + "</Value></Eq></Where></Query></View>";
                    // execute the query
                    ListItemCollection listItems = EmailConfigurationList.GetItems(query);
                    clientContext.Load(listItems);
                    clientContext.ExecuteQuery();

                    if (listItems != null && listItems.Count > 0)
                    {
                        Utility.Logging.LogErrorException(null, "Item Count from email : " + listItems.Count.ToString());
                        List<string> lstTo = new List<string>();

                        List lstServiceRequest = clientContext.Web.Lists.GetByTitle("ServiceRequest");
                        ListItem item = lstServiceRequest.GetItemById(GetServiceRequest().ServiceRequestID);
                        clientContext.Load(lstServiceRequest);
                        clientContext.Load(item);
                        clientContext.ExecuteQuery();
                        Utility.Logging.LogErrorException(null, "Item Count from email : " + listItems.Count.ToString());
                        var emailp = new EmailProperties();

                        Microsoft.SharePoint.Client.User user = null;
                        if (item["Author"] != null && !string.IsNullOrEmpty(Convert.ToString(item["Author"])))
                        {
                            try
                            {

                                user = PeoplePickerHelper.SPEnsueruser(clientContext, ((Microsoft.SharePoint.Client.FieldUserValue)item.FieldValues["Author"]).LookupId);
                                if (user != null && !string.IsNullOrEmpty(user.Email))
                                {
                                    lstTo.Add(user.Email);
                                }
                            }
                            catch (Exception ex)
                            {
                                Utility.Logging.LogErrorException(ex, "Error in Sending mail Author");
                            }
                        }
                        List<string> users = null;
                        if (item["ForwardRequesTo"] != null && !string.IsNullOrEmpty(Convert.ToString(item["ForwardRequesTo"])))
                        {
                            users = PeoplePickerHelper.SPEnsureMultiUserEmailAddress(clientContext, ((Microsoft.SharePoint.Client.FieldUserValue[])item.FieldValues["ForwardRequesTo"]));
                            if (users != null && users.Count > 0)
                            {
                                foreach (string email in users)
                                {
                                    lstTo.Add(email);
                                }
                            }
                        }

                        if (item["OnBehalf"] != null && !string.IsNullOrEmpty(Convert.ToString(item["OnBehalf"])))
                        {
                            try
                            {

                                users = PeoplePickerHelper.SPEnsureMultiUserEmailAddress(clientContext, ((Microsoft.SharePoint.Client.FieldUserValue[])item.FieldValues["OnBehalf"]));
                                if (users != null && users.Count > 0)
                                {
                                    foreach (string email in users)
                                    {
                                        lstTo.Add(email);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Utility.Logging.LogErrorException(ex, "Error in Sending mail OnBehalf");
                            }

                        }



                        List<string> memberEmailAdreess = GetSPlistItems("ADConfiguration", "Administrators");
                        if (memberEmailAdreess != null && memberEmailAdreess.Count > 0)
                        {

                            foreach (string email in memberEmailAdreess)
                            {
                                if (!lstTo.Contains(email))
                                {
                                    lstTo.Add(email);

                                }
                            }
                        }
                        //Microsoft.SharePoint.Client.Group group = PeoplePickerHelper.SPEnsuerGroup(clientContext, "Administrators");
                        //if (group != null)
                        //{
                        //    foreach (User adminUser in group.Users)
                        //    {
                        //        if (!string.IsNullOrEmpty(adminUser.Email))
                        //        {
                        //            lstTo.Add(adminUser.Email);
                        //        }
                        //    }
                        //}
                        string value = string.Empty;
                        string subject = Convert.ToString(listItems[0]["Subject"]);
                        string body = string.Empty;
                        emailp.From = Convert.ToString(listItems[0]["From"]);
                        value = Convert.ToString(listItems[0]["Subject"]);
                        emailp.Subject = value;
                        value = Convert.ToString(listItems[0]["EmailBody"]);
                        value = value.Replace("<request name>", Convert.ToString(item["Title"]));
                        value = value.Replace("&lt;request name&gt;", Convert.ToString(item["Title"]));
                        value = value.Replace("<Url>", "<a href=" + Request.QueryString["SPHostUrl"] + "/SitePages/ServiceRequest.aspx?ServiceRequestID=" + GetServiceRequest().ServiceRequestID + ">Click here to view service request.</a>");
                        value = value.Replace("&lt;Url&gt;", "<a href=" + Request.QueryString["SPHostUrl"] + "/SitePages/ServiceRequest.aspx?ServiceRequestID=" + GetServiceRequest().ServiceRequestID + ">Click here to view service request.</a>");
                        emailp.Body = value;
                        body = value;

                        if (item["Designer"] != null && !string.IsNullOrEmpty(Convert.ToString(item["Designer"])))
                        {
                            try
                            {

                                users = PeoplePickerHelper.SPEnsureMultiUserEmailAddress(clientContext, ((Microsoft.SharePoint.Client.FieldUserValue[])item.FieldValues["Designer"]));

                                if (users != null && users.Count > 0)
                                {
                                    foreach (string email in users)
                                    {
                                        lstTo.Add(email);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Utility.Logging.LogErrorException(ex, "Error in Sending mail Designer");
                            }
                        }
                        if (item["BackupDesigner"] != null && !string.IsNullOrEmpty(Convert.ToString(item["BackupDesigner"])))
                        {
                            try
                            {
                                users = PeoplePickerHelper.SPEnsureMultiUserEmailAddress(clientContext, ((Microsoft.SharePoint.Client.FieldUserValue[])item.FieldValues["BackupDesigner"]));
                                if (users != null && users.Count > 0)
                                {
                                    foreach (string email in users)
                                    {
                                        lstTo.Add(email);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Utility.Logging.LogErrorException(ex, "Error in Sending mail BackupDesigner");
                            }
                        }

                        if (item["ProductionSpecialist"] != null && !string.IsNullOrEmpty(Convert.ToString(item["ProductionSpecialist"])))
                        {
                            try
                            {
                                users = PeoplePickerHelper.SPEnsureMultiUserEmailAddress(clientContext, ((Microsoft.SharePoint.Client.FieldUserValue[])item.FieldValues["ProductionSpecialist"]));

                                if (users != null && users.Count > 0)
                                {
                                    foreach (string email in users)
                                    {
                                        lstTo.Add(email);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Utility.Logging.LogErrorException(ex, "Error in Sending mail ProductionSpecialist");
                            }

                        }


                        if (emailType == NotificationType.ChangeRequest || emailType == NotificationType.FinalApproval)
                        {
                            //group = PeoplePickerHelper.SPEnsuerGroup(clientContext, "Graphic Artists");
                            //if (group != null)
                            //{
                            //    foreach (User adminUser in group.Users)
                            //    {
                            //        if (!string.IsNullOrEmpty(adminUser.Email))
                            //        {
                            //            lstTo.Add(adminUser.Email);
                            //        }
                            //    }
                            //}

                        }
                        if (Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["SMTPClassic"]) == "true")
                        {
                            var mailMessage = new MailMessage();
                            string strToEmails = string.Empty;
                            foreach (string str in lstTo)
                            {
                                strToEmails = strToEmails + str + ",";

                            }
                            strToEmails = strToEmails.Remove(strToEmails.Length - 1);
                            mailMessage.To.Add(strToEmails);
                            mailMessage.Subject = subject;
                            mailMessage.Body = body;
                            mailMessage.IsBodyHtml = true;

                            var smtpClient = new SmtpClient { EnableSsl = false };
                            smtpClient.Send(mailMessage);
                        }
                        else
                        {
                            emailp.To = (IEnumerable<string>)lstTo;
                            Microsoft.SharePoint.Client.Utilities.Utility.SendEmail(clientContext, emailp);
                            clientContext.ExecuteQuery();
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Error in Sending mail");
            }

        }
        [SharePointContextFilter]
        public List<string> GetSPlistItems(string Name, string whereCondition)
        {
            List<string> AllRecords = null;
            try
            {

                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

                var clientContext = spContext.CreateUserClientContextForSPHost();
                
                    if (clientContext != null)
                    {

                        List spList = clientContext.Web.Lists.GetByTitle(Name);
                        clientContext.Load(spList);
                        clientContext.ExecuteQuery();
                        CamlQuery query = new Microsoft.SharePoint.Client.CamlQuery();
                        if (!string.IsNullOrEmpty(whereCondition))
                        {
                            query.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + whereCondition + "</Value></Eq></Where></Query></View>";

                        }
                        // execute the query
                        ListItemCollection listItems = spList.GetItems(query);
                        clientContext.Load(listItems, items => items.Include(item => item["Title"], item => item["Value"]));
                        clientContext.ExecuteQuery();
                        if (listItems != null && listItems.Count > 0)
                        {
                            AllRecords = new List<string>();
                            foreach (ListItem item in listItems)
                            {
                                try
                                {
                                    var elements = Convert.ToString(item["Value"]).Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                                    // To Loop through
                                    foreach (string ele in elements)
                                    {
                                        if (ele != null && !string.IsNullOrEmpty(ele))
                                        {
                                            AllRecords.Add(ele);
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Utility.Logging.LogErrorException(ex, "GetSPlistItems for admin");
                                }

                            }
                        }
                    }
                }

            
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "GetSPlistItems");
            }
            return AllRecords;
        }
        public ActionResult AdditionalInfo()
        {
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Get AdditionalInfo");
            }


            return View(GetServiceRequest());
        }
        [HttpPost]
        [SharePointContextFilter]
        public ActionResult AdditionalInfo(ServiceRequest serviceRequest, HttpPostedFileBase File, string btnNext, string Draft, string CREATE, string KEYCODES, string SPECS, string STATUS)
        {
            try
            {
                serviceRequest.Mode = GetServiceRequest().Mode;
                serviceRequest.Files = GetServiceRequest().Files;
                GetServiceRequest().ScrapOldKeyCode = GetFieldChoices("ServiceRequest", "ScrapOldKeyCode");

                if (!string.IsNullOrEmpty(serviceRequest.FileNameList))
                {
                    List<FileDetails> uploadsFileDetails = (List<FileDetails>)Newtonsoft.Json.JsonConvert.DeserializeObject(serviceRequest.FileNameList, typeof(List<FileDetails>));
                    if (serviceRequest.Files == null)
                    {
                        serviceRequest.Files = new List<FileDetails>();
                    }
                    foreach (FileDetails f in uploadsFileDetails)
                    {
                        serviceRequest.Files.Add(f);
                    }
                    serviceRequest.FileNameList = JsonConvert.SerializeObject(serviceRequest.Files);
                }
                serviceRequest = SetServiceRequest(serviceRequest, "AdditionalInfo");

                var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                AddAppContextToViewBag(this, HttpContext, spContext);
                if (btnNext != null)
                {

                    if ((serviceRequest.Mode == OperationMode.Insert && serviceRequest.RequestAction != ServiceRequestAction.Clone) || (serviceRequest.Mode == OperationMode.Edit && (serviceRequest.KeyCodeDetails == null || serviceRequest.KeyCodeDetails.Count == 0)))
                    {
                        List<KeyCodes> ci = new List<KeyCodes> { new KeyCodes { KeyCodeID = 0, OldKeyCode = "", ReferenceKeyCode = "", Description = "", NewKeyCode = "" } };
                        serviceRequest.KeyCodeDetails = ci;
                    }
                    return View("KeyCodesInfo", serviceRequest);
                }
                if (Draft != null)
                {
                    serviceRequest = SaveServiceRequest(serviceRequest, ServiceRequestAction.Draft);
                    AddHistoryDetails("Save as draft => Additional Information");
                    return View("ReditrectToSharePoint");
                }
                if (CREATE != null || KEYCODES != null || SPECS != null || STATUS != null)
                {
                    return RedirectTabView(serviceRequest, CREATE, KEYCODES, SPECS, STATUS);
                }

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Post AdditionalInfo");
            }

            return View(serviceRequest);
        }
        [SharePointContextFilter]
        public ActionResult RedirectTabView(ServiceRequest serviceRequest, string CREATE, string KEYCODES, string SPECS, string STATUS)
        {
            serviceRequest.StatusList = GetFieldChoices("ServiceRequest", "RequestStatus");
            serviceRequest.ScrapOldKeyCode = GetFieldChoices("ServiceRequest", "ScrapOldKeyCode");
            if (CREATE != null)
            {
                serviceRequest.Audience = GetLookUpList("Audiences", "ID", "Title");
                //serviceRequest.ProductionSpecialist = GetUsersFromGroup("Production Managers");                

                //serviceRequest = SetServiceRequest(serviceRequest, "GeneralInfo");
                SetUserGroupDetails();
                return View("GeneralInfo", serviceRequest);

            }
            else if (KEYCODES != null)
            {
                serviceRequest.KeyCodeDetails = GetServiceRequest().KeyCodeDetails;
                if (serviceRequest.KeyCodeDetails == null)
                {
                    if ((serviceRequest.Mode == OperationMode.Insert && serviceRequest.RequestAction != ServiceRequestAction.Clone) || (serviceRequest.Mode == OperationMode.Edit && (serviceRequest.KeyCodeDetails == null || serviceRequest.KeyCodeDetails.Count == 0)))
                    {
                        List<KeyCodes> ci = new List<KeyCodes> { new KeyCodes { KeyCodeID = 0, OldKeyCode = "", ReferenceKeyCode = "", Description = "", NewKeyCode = "" } };
                        serviceRequest.KeyCodeDetails = ci;
                    }
                }

                return View("KeyCodesInfo", serviceRequest);
            }
            else if (SPECS != null)
            {
                serviceRequest.ClientName = GetLookUpList("ClientMaster", "ID", "Title");
                serviceRequest.FinalOutput = GetLookUpList("FinalOutput", "ID", "Title");
                serviceRequest.AdditionalOutput = GetLookUpList("FinalOutput", "ID", "Title");
                //serviceRequest.ApproxSize = GetFieldChoices("ServiceRequest", "ApproxSize");
                //serviceRequest.Colors = GetFieldChoices("ServiceRequest", "Colors");
                return View("SpecInfo", serviceRequest);
            }
            else if (STATUS != null)
            {
                serviceRequest = SetServiceRequest(serviceRequest, "SpecInfo");
                serviceRequest.RequestHistory = GetServiceRequest().RequestHistory;
                //serviceRequest.ProductionSpecialist = GetUsersFromGroup("Production Managers");
                return View("StatusInfo", serviceRequest);
            }
            return View(serviceRequest);
        }
    }
}
