using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SB.AR.Helper
{
    public static class SBHepler
    {
        public static string accessToken { get; set; }

        
        /// <summary>
        /// Gets the base URL.
        /// </summary>
        /// <value>
        /// The base URL.
        /// </value>
        public static string BaseUrl
        {
            get
            {
                return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
            }
        }



        public static JsonResult UploadFile(string qqfile)
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
        public static T GetTempData<T>(string key) where T : new()
        {
            if (this.TempData[key.ToString()] == null)
            {
                this.TempData[key.ToString()] = new T();
            }
            this.TempData.Keep(key.ToString());
            return (T)this.TempData[key.ToString()];
        }

        public static void SetTempData<T>(string key, T obj)
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
        public static JsonResult VerifyDownloadFile(string url, string applicationName)
        {
            string this_id = "101";
            string this_name = "20.1";
            // do here some operation  
            return Json(new { url = this_id, applicationName = this_name, Status = 1 }, JsonRequestBehavior.AllowGet);
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

        public static IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<string> elements)
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

        public static IEnumerable<SelectListItem> GetLookUpList(string ListInternalName, string valueField, string textField)
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
        public static IEnumerable<SelectListItem> GetUsersFromGroup(string GroupName)
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
        public static IEnumerable<SelectListItem> GetFieldChoices(string ListInternalName, string internalFieldName)
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

       

        public static string GetPlainTextFromHtml(string htmlString)
        {
            string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace("&nbsp;", string.Empty);

            return htmlString;
        }
        
        public static bool checkIsMemberExists(string grpName)
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
        public static void RetrieveAccessToken()
        {
            ClientContext ctx = SharePointContextProvider.Current.GetSharePointContext(HttpContext).CreateUserClientContextForSPHost();
            ctx.ExecutingWebRequest += ctx_ExecutingWebRequest;
            ctx.ExecuteQuery();
        }

                
        
        [SharePointContextFilter]
        public static List<string> GetSPlistItems(string Name, string whereCondition)
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
       
       
    }
}
