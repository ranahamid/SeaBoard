using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;
using arapp = SB.AR.AppWeb.Models;
using SB.AR.AppWeb.Models;
using Newtonsoft.Json;

namespace SB.AR.AppWeb.Controllers
{
    public class AttachmentsController : SBControllerBase
    {
        //
        // GET: /Attachments/
        public ActionResult Index()
        {
            //SetarDetailViewBag();
            bool isAdmin = this.IsSiteCollectionAdmin();
            ViewBag.IsSiteAdmin = isAdmin;
            if (AR != null && string.IsNullOrEmpty(AR.Attachment_Folder_Id))
            {
                AR.Attachment_Folder_Id = AttachmentFolderID();
                SaveARToList(AR);
            }
            ViewModels.ARAttachmentsViewModel arAttachments = new ViewModels.ARAttachmentsViewModel(SPContext, AR);
            return PartialView("_Attachments", arAttachments);
        }
        //added code for Reject status attachment remove
        private string AttachmentFolderID()
        {
            var date = DateTime.Now;
            Random rnd = new Random();

            var Attachment_Folder_Id = string.Format("{0}-{1}-{2}-{3}-{4}", date.Year, date.Month, date.Day, date.Millisecond, rnd.Next(99999));
            return Attachment_Folder_Id;
        }

        [HttpPost]
        public void DownloadFiles(string AttachmentFolderId)
        {
            ViewModels.ARAttachmentsViewModel arAttachments = new ViewModels.ARAttachmentsViewModel(AttachmentFolderId);
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                zip.AddDirectoryByName(AttachmentFolderId);
                foreach (var file in arAttachments.attachments)
                {
                    zip.AddFile(file.FileUrl, AttachmentFolderId);

                }
                Response.Clear();
                Response.BufferOutput = false;
                string zipName = String.Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                Response.ContentType = "application/zip";
                Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(Response.OutputStream);

                Response.End();
            }
        }

        [HttpPost]
        public ActionResult UploadFile()
        {
           
            string shareFileFolder = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["ShareFileFolder"]);
           // string ARNumber = HttpContext.Request.Params["ARNumber"];
            var _ar = this.AR;
            string Attachment_Folder_Id = _ar.Attachment_Folder_Id;

            HttpPostedFileBase arFile = HttpContext.Request.Files["UploadedFile"];
            if (!string.IsNullOrEmpty(Attachment_Folder_Id))
            {
                if (!string.IsNullOrEmpty(shareFileFolder))
                {
                    if (!Directory.Exists(shareFileFolder))
                    {
                        Directory.CreateDirectory(shareFileFolder);
                    }
                    string ShareItemFolder = Path.Combine(shareFileFolder, Attachment_Folder_Id);
                    if (!Directory.Exists(ShareItemFolder))
                    {
                        Directory.CreateDirectory(ShareItemFolder);
                    }

                    string Filename = Path.GetFileName(arFile.FileName);
                    string tempPath = Path.Combine(Server.MapPath("~/Uploads"), string.Concat(Filename, "_", Guid.NewGuid().ToString()));

                    if (!System.IO.File.Exists(Path.Combine(ShareItemFolder, Path.GetFileName(arFile.FileName))))
                    {
                        var stream = this.Request.InputStream;
                        stream = arFile.InputStream;
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(stream))
                        {
                            fileData = binaryReader.ReadBytes((int)stream.Length);
                        }
                        System.IO.File.WriteAllBytes(tempPath, fileData);
                        System.IO.File.Copy(tempPath, Path.Combine(ShareItemFolder, Path.GetFileName(arFile.FileName)), true);
                        System.IO.File.Delete(tempPath);
                    }
                }
            }
            return Json(new { status = "Success", message = "Success" });
        }


        [HttpPost]
        public ActionResult DeleteFile()
        {
            string shareFileFolder = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["ShareFileFolder"]);
           // string ARNumber = HttpContext.Request.Params["ARNumber"];
            var _ar = this.AR;
            string Attachment_Folder_Id = _ar.Attachment_Folder_Id;

            string arFile = HttpContext.Request.Params["FileName"];
            string ShareItemFolder = Path.Combine(shareFileFolder, Attachment_Folder_Id, arFile);



            if (System.IO.File.Exists(ShareItemFolder))
            {
                System.IO.File.Delete(ShareItemFolder);
            }


            return Json(new { status = "Success", message = "Success" });
        }

        [HttpGet]
        public virtual ActionResult Download(string file)
        {
            string arFile = Request.QueryString["file"];
            string shareFileFolder = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["ShareFileFolder"]);
            // string ARNumber = HttpContext.Request.Params["ARNumber"];
            var _ar = this.AR;
            string Attachment_Folder_Id = _ar.Attachment_Folder_Id;
            string fullFilePath = Path.Combine(shareFileFolder, Attachment_Folder_Id, arFile);
            //string fullPath = Path.Combine(Server.MapPath("~/MyFiles"), file);
            //return File(fullPath, "application/octet-stream", file);
            byte[] data = System.IO.File.ReadAllBytes(fullFilePath);
            return File(data, "application/octet-stream", file);
        }

        [HttpPost]
        public ActionResult SubmitAR()
        {
            List<SubmitResult> returnResults = new List<SubmitResult>();   
            var _ar = this.AR;
            foreach (var prop in _ar.GetType().GetProperties())
            {
                var required = prop.GetCustomAttributesData().Where(A => A.AttributeType.Name.Equals("RequiredAttribute")).FirstOrDefault();
                var controller = prop.GetCustomAttributesData().Where(A => A.AttributeType.Name.Equals("UsedInAttribute")).FirstOrDefault();
                if (required != null && controller != null)
                {
                    var ControllerName = controller.NamedArguments.Where(a => a.MemberName.Equals("ControllerName")).FirstOrDefault().TypedValue.Value.ToString();
                    var associateArType = controller.NamedArguments.Where(a => a.MemberName.Equals("AssociateTab")).FirstOrDefault().TypedValue.Value;
                    var FieldName = controller.NamedArguments.Where(a => a.MemberName.Equals("FieldName")).FirstOrDefault().TypedValue.Value;

                    if (ControllerName.ToUpper() == "FINANCIALS")
                    {
                        if (associateArType == null)
                            continue;

                        string arType = _ar.AR_Type;
                        var allAssociate = associateArType.ToString().Split(',');

                        var isMatch = allAssociate.FirstOrDefault(a => a.ToLower().Trim() == arType.ToLower().Trim());
                        if (isMatch == null)
                            continue;
                    }
                    if(prop.PropertyType.FullName.IndexOf("System.String",StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        if (prop.Name.Trim().ToUpper() == "PMOWNER")
                        {
                            if (_ar.PMUser != null && _ar.PMUser.LookupValue != null)
                                _ar.PMOwner = _ar.PMUser.LookupValue;
                            else if (_ar.PMOwnerLogin != null)
                                _ar.PMOwner = _ar.PMOwnerLogin;

                        }                        
                        if(prop.GetValue(_ar, null) == null || string.IsNullOrEmpty(prop.GetValue(_ar, null).ToString()))
                        {
                            returnResults.Add(new SubmitResult() { 
                                Tab = ControllerName,
                                Message = string.Concat(FieldName, " is Required")
                            });

                        }
                    }
                    else if (prop.PropertyType.FullName.IndexOf("System.Int32", StringComparison.OrdinalIgnoreCase) > -1 || prop.PropertyType.FullName.IndexOf("System.Double", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        if (prop.Name.Trim().ToUpper() == "DIVISIONID")
                        {
                            if (_ar.DivisionId == null && _ar.Division != null)
                                _ar.DivisionId = _ar.Division.ID;
                        }
                        else if (prop.Name.Trim().ToUpper() == "COMPANYID")
                        {
                            if (_ar.CompanyId == null && _ar.Company_Name != null)
                                _ar.CompanyId = _ar.Company_Name.ID;
                        }
                        var GetValue=Convert.ToInt32(prop.GetValue(_ar, null));
                        if (GetValue == 0)
                        {
                            returnResults.Add(new SubmitResult()
                            {
                                Tab = ControllerName,
                                Message = string.Concat(FieldName, " is Required")
                            });

                        }
                    }
                    else if (prop.PropertyType.FullName.IndexOf("System.DateTime", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        if (prop.GetValue(_ar, null) == null)
                        {
                            returnResults.Add(new SubmitResult()
                            {
                                Tab = ControllerName,
                                Message = string.Concat(FieldName, " is Required")
                            });

                        }
                    }
                    else if (prop.PropertyType.FullName.IndexOf("FieldUserValue", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        var value = prop.GetValue(_ar, null);
                        if (value == null)
                        {
                            returnResults.Add(new SubmitResult()
                            {
                                Tab = ControllerName,
                                Message = string.Concat(FieldName, " is Required")
                            });

                        }
                    }
                    else if (prop.PropertyType.FullName.IndexOf("LookupFieldMapper", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        if (prop.GetValue(_ar, null) == null)
                        {
                            returnResults.Add(new SubmitResult()
                            {
                                Tab = ControllerName,
                                Message = string.Concat(FieldName, " is Required")
                            });

                        }
                    }
                    
                    //Console.WriteLine("{0}={1} {2} {3}", prop.Name, prop.GetValue(obj, null), fieldData, prop.GetType().ToString());
                }
            }

            if(_ar.Project_End != null && _ar.Project_Start != null && _ar.Project_Start > _ar.Project_End)
                returnResults.Add(new SubmitResult()
                {
                    Tab = SB.AR.AppWeb.Helper.Tabs.MAIN,
                    Message = "Project start date should be before project end date"
                });

            if (returnResults.Count == 0)
            {
                var updateAR = new arapp.AR();

                if(this.AR != null)
                    updateAR = this.AR;

                updateAR.ID = _ar.ID;
                updateAR.Current_Status = "Pending Approvals";
                updateAR.Submit_Action = "Submit for Approvals";
                SaveARToList(updateAR);

            }

            returnResults = returnResults.OrderBy(a => a.Tab).ToList();
            return Json(returnResults, JsonRequestBehavior.AllowGet); 
        }

        [HttpPost]
        public ActionResult SaveARAsDraft()
        {
            var hostUrl = this.SPHostUrl;
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
        public ActionResult SaveARAsSubmitRMReview()
        {
            var hostUrl = this.SPHostUrl;

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
        private void SetarDetailViewBag()
        {
            var arDetail = TempData["ARDetail"] as Models.AR;
            ViewBag.Title = arDetail.Title;
            ViewBag.Company_Name = arDetail.Company_Name.Value;
            ViewBag.Division = arDetail.Division.Value;
            ViewBag.Total_Cost = arDetail.Total_Cost;
            ViewBag.Current_Status = arDetail.Current_Status;
            ViewBag.AR_Type = arDetail.AR_Type;
            ViewBag.ARNumber = arDetail.ARNumber;
            ViewBag.AR_ID = arDetail.AR_ID;
            TempData["ARDetail"] = arDetail;
        }
    }

    public class SubmitResult
    {
        public string Tab { get; set; }
        public string Message { get; set; }
    }
}