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
using SB.AR.AppWeb;
using SB.AR.AppWeb.Utility;

namespace SB.AR.AppWeb.Helper
{
    public static class Tabs
    {
        public const string MAIN = "MAIN";
        public const string NARRATIVE = "NARRATIVE";
        public const string FINANCIALS = "FINANCIALS";
    }

    public static class SPListMeta
    {
        public const string ARTYPEDESCRIPTION = "AR Type Descriptions";
        public const string ARType = "AR_Type";
        public const string Category = "Category_Master";
        public const string Division = "Division_Master";
        public const string Company = "Company_Master";
        public const string AR = "AR";
        public const string ARDISCUSSION = "AR_Discussion";
        public const string ARAPPROVERS = "Company_Approvers";

        public const string ApprovalHistory = "Approval_History";

        public const string CorporateARAPPROVERS = "Corporate_Approvers";
        
        public const string Workflow_Tasks = "Workflow Tasks";
        public const string Company_Master = "Company_Master";
        public const string NavigationAR = "AR Navigation";
        public const string AUDITUSERS = "Audit_Users";

    }

    public class SBHepler : Controller
    {
        public static string accessToken { get; set; }
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
                SB.AR.AppWeb.Utility.Logging.LogErrorException(ex, "AddAppContextToViewBag()");
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
                Logging.LogErrorException(ex, "GetSelectListItems()");
            }


            return selectList;
        }

        public static IEnumerable<SelectListItem> GetLookUpList(HttpContextBase httpContext, string ListInternalName, string valueField, string textField)
        {
            List<SelectListItem> clients = new List<SelectListItem>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(httpContext);
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
                Logging.LogErrorException(ex, "GetLookUpList()");
            }


            return new SelectList(clients, "Value", "Text");
        }
        public static IEnumerable<SelectListItem> GetUsersFromGroup(HttpContextBase httpContext, string GroupName)
        {
            List<SelectListItem> clients = new List<SelectListItem>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(httpContext);
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
                Logging.LogErrorException(ex, "GetUsersFromGroup()");
            }


            return new SelectList(clients, "Value", "Text");
        }
        public static IEnumerable<SelectListItem> GetFieldChoices(HttpContext httpContext, string ListInternalName, string internalFieldName)
        {
            List<SelectListItem> choices = new List<SelectListItem>();
            try
            {
                var spContext = SharePointContextProvider.Current.GetSharePointContext(httpContext);

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
                Logging.LogErrorException(ex, "GetFieldChoices()");
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

        public static bool checkIsMemberExists(HttpContext httpContext, string grpName)
        {
            bool isMember = false;
            try
            {
                RetrieveAccessToken(httpContext);

                HttpWebRequest request = HttpWebRequest.CreateHttp(String.Format("{0}/_api/web/sitegroups/getbyname('" + grpName + "')/CanCurrentUserViewMembership", httpContext.Request.QueryString["SPHostUrl"]));
                request.Accept = "application/json;odata=verbose";
                request.Headers.Add("Authorization", accessToken);
                Stream postStream = request.GetResponse().GetResponseStream();


                StreamReader postReader = new StreamReader(postStream);


                var result = new JavaScriptSerializer().Deserialize<dynamic>(postReader.ReadToEnd());
                if (result != null)
                {
                    isMember = Convert.ToBoolean(result["d"]["CanCurrentUserViewMembership"]);
                    Logging.LogErrorException(null, "checkIsMemberExists () : Member GroupName: " + grpName + " isMember= " + isMember.ToString());
                }

            }
            catch (Exception ex)
            {
                isMember = false;
                Logging.LogErrorException(ex, "checkIsMemberExists () ");
            }
            return isMember;
        }
        public static void RetrieveAccessToken(HttpContext httpContext)
        {
            ClientContext ctx = SharePointContextProvider.Current.GetSharePointContext(httpContext).CreateUserClientContextForSPHost();
            ctx.ExecutingWebRequest += ctx_ExecutingWebRequest;
            ctx.ExecuteQuery();
        }

        public static void ctx_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            accessToken = e.WebRequestExecutor.RequestHeaders.Get("Authorization");
        }

        [SharePointContextFilter]
        public static List<string> GetSPlistItems(HttpContext httpContext, string Name, string whereCondition)
        {
            List<string> AllRecords = null;
            try
            {

                var spContext = SharePointContextProvider.Current.GetSharePointContext(httpContext);

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
                                Logging.LogErrorException(ex, "GetSPlistItems for admin");
                            }

                        }
                    }
                }
            }


            catch (Exception ex)
            {
                Logging.LogErrorException(ex, "GetSPlistItems");
            }
            return AllRecords;
        }


    }
}
