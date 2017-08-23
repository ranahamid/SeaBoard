using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NSBD.SharepointAutoMapper;
using Microsoft.SharePoint;
using AutoMapper;
using System.IO;
using SB.AR.AppWeb.Models;

using System.Net.Mail;

namespace SB.AR.AppWeb.ViewModels
{
    //[SharePointContextFilter]
    public class ARDiscussionViewModel : ViewModelBase
    {
        public List<Models.ARDiscussions> discussions { get; set; }
        //private SharePointContext _spContext = null;
        //private int arID = 0;
        private Models.AR _ar;
        public ARDiscussionViewModel() { }
        public ARDiscussionViewModel(SharePointContext _sharePointContext, Models.AR ar)
            : base(_sharePointContext)
        {
            if (ar == null)
                ar = new Models.AR();
            _ar = ar;
        }
        public Models.AR AR
        {
            get
            {
                return _ar;
            }
        }

        private string GetDiscussionTABUrlForAR()
        {
            string appARRedirectURL = string.Empty;
            try
            {
                string spHostUrl = Convert.ToString(HttpContext.Current.Session["SPHostUrl"]);
                //spHostUrl = HttpUtility.UrlEncode(spHostUrl);
                //string appURLBasic = HttpContext.Current.Request.Url.AbsoluteUri.Replace("/Discussion/SaveDiscussion", "");
                //appARRedirectURL = string.Format("{0}/Seaboard/MainView?SPHostUrl={1}{2}&id={3}&tab=discussion", appURLBasic, spHostUrl, HttpContext.Current.Request.Url.Query, _ar.ID);

                appARRedirectURL = string.Format("{0}/SitePages/arapphome.aspx?aid={1}&tab=maintab", spHostUrl, _ar.AR_ID);
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex);

            }
            return appARRedirectURL;
        }

        public void SendMail(ARDiscussions discussion)
        {


            const string emailSubjectTemplate = "A new comment has been made to AR {AR#} ({ARTitle})";
            const string emailBodyTemplate = @"
An AR you are linked to has received a new comment.<br/><br/>
<table>
<tr>
    <td>AR#  </td><td>{AR#}</td>
</tr>
<tr>
    <td>AR Title:</td><td>{ARTitle}</td>
</tr>
<tr>
    <td>Comment:</td><td>{Comment}</td>
</tr>
</table>
 
 <br/><br/>

To view the AR, please <u><a href=""{RedirectURL}"">click here</a></u>";

            try
            {

                string CommentFromToken = discussion.From.Name;
                string CommentToken = discussion.Messsage;
                string ARNumberToken = _ar.AR_ID.ToString();
                string ARTitleToken = _ar.Title.Trim();

                string appARRedirectURL = GetDiscussionTABUrlForAR();

                string emailSubjectData = emailSubjectTemplate.Replace("{AR#}", ARNumberToken).Replace("{ARTitle}", ARTitleToken);
                string emailBodyData = emailBodyTemplate.Replace("{AR#}", ARNumberToken).Replace("{ARTitle}", ARTitleToken).Replace("{RedirectURL}", appARRedirectURL).Replace("{CommentFrom}", CommentFromToken).Replace("{Comment}", CommentToken);

                
                var emailAddress = discussion.ToAddress.Where(a => !string.IsNullOrEmpty(a.Email)).Select(a => a.Email).ToArray<string>();
                
                if(emailAddress.Length > 0)
                {
                    emailAddress = (from email in emailAddress select email).Distinct().ToArray();
                }

                if (emailAddress.Length > 0)
                {
                    string MailAddressFrom = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["MailAddressFrom"]);
                    string MailSmtpHost = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["MailSmtpHost"]);
                    int MailSmtpPort = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["MailSmtpPort"]);

                    MailMessage msg = new MailMessage(MailAddressFrom, String.Join(",", emailAddress));
                    msg.IsBodyHtml = true;
                    msg.Subject = emailSubjectData;
                    msg.Body = emailBodyData;
                    System.Net.Mail.SmtpClient client = new SmtpClient(MailSmtpHost, MailSmtpPort);
                    
                    client.Send(msg);

                }
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex);

            }
        }

        public void SaveDiscussions(Models.ARDiscussions discussion)
        {
            using (var clientContext = this._spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.ARDISCUSSION);
                    ListItemCreationInformation itemInfo = new ListItemCreationInformation();
                    ListItem listItem = arList.AddItem(itemInfo);
                    listItem["Message"] = discussion.Messsage;
                    listItem["AllApprovers"] = discussion.AllApprovers;
                    listItem["ProjectManagers"] = discussion.ProjectManagers;
                    listItem["Orignator"] = discussion.Orignator;
                    listItem["Public"] = discussion.Public;
                    listItem["ToAddress"] = this.GetUsers(discussion.ToAddress);
                    listItem["ARItemId"] = this._ar.AR_ID;
                    listItem.Update();

                    clientContext.ExecuteQuery();
                    clientContext.Load(listItem);
                    clientContext.ExecuteQuery();
                    discussion.Created = Convert.ToDateTime(listItem["Created"], System.Globalization.CultureInfo.InvariantCulture);
                }
            }
        }

        public UserDetails GetFromAddress()
        {

            return null;
        }

        public List<UserDetails> GetToAddress(ARDiscussions discussion, string division, string company)
        {
            List<UserDetails> users = new List<UserDetails>();

            if (discussion.AllApprovers)
                users = this.GetApprovers(users, division, company);

            using (var clientContext = this._spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    clientContext.Load(clientContext.Web.CurrentUser);

                    clientContext.ExecuteQuery();
                    if (!string.IsNullOrEmpty(clientContext.Web.CurrentUser.Email))
                    {
                        discussion.From = new UserDetails()
                        {
                            ID = clientContext.Web.CurrentUser.Id.ToString(),
                            Email = clientContext.Web.CurrentUser.Email,
                            Name = clientContext.Web.CurrentUser.Title,
                            UserName = clientContext.Web.CurrentUser.LoginName
                        };
                    }

                    if (discussion.ProjectManagers)
                    {
                        if (this.AR.PMUser != null && this.AR.PMUser.LookupId > 0)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, this.AR.PMUser));
                        }
                    }
                }
            }
            if (discussion.Orignator)
            {
                users.Add(discussion.From);
            }
            //if(discussion.Public)
            //if(discussion.ProjectManagers) /// depnedency on the Main Tab

            return users;
        }

        public List<Models.ARDiscussions> GetARDiscussion()
        {
            List<Models.ARDiscussions> discussions = new List<Models.ARDiscussions>();
            using (var clientContext = _spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.ARDISCUSSION);
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = string.Format(@"<View>  
                                <Query>
                                    <Where><Eq><FieldRef Name='ARItemId' /><Value Type='Number'>{0}</Value></Eq></Where>
                                </Query> 
                                <ViewFields>
                                        <FieldRef Name='Message' />\
                                        <FieldRef Name='ToAddress' />\
                                        <FieldRef Name='Author' />\
                                        <FieldRef Name='Created' />
                                        
                                </ViewFields>
                                
                                </View>", this._ar.AR_ID);
                    var arItem = arList.GetItems(query);
                    clientContext.Load(arItem);
                    clientContext.ExecuteQuery();
                    // discussions = arItem.ProjectToListEntity<Models.ARDiscussions>();
                    //// Templ need to check why properties is not working with it, user field is not haadeedl in the frame work
                    foreach (var item in arItem)
                    {
                        discussions.Add(new ARDiscussions()
                        {
                            Messsage = item["Message"].ToString(),
                            ToAddress = this.GetUserFromItem((FieldUserValue[])item["ToAddress"], clientContext),
                            From = this.GetUserFromLookupId(clientContext, (FieldUserValue)item["Author"]),
                            Created = Convert.ToDateTime(item["Created"])
                        });
                    }
                }
            }

            return discussions.OrderByDescending(a => a.Created).ToList();
        }

        private List<UserDetails> GetApprovers(List<UserDetails> users, string division, string company)
        {
            var clientContext = _spContext.CreateUserClientContextForSPHost();
            List<SB.AR.AppWeb.Models.CompanyApprover> CompanyApprover = new List<SB.AR.AppWeb.Models.CompanyApprover>();
            try
            {
                if (clientContext != null)
                {
                    List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.ARAPPROVERS);
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = string.Format(@"
                            <View>
                               <Query>
                                   <ViewFields>
                                      <FieldRef Name='Dept_Reviewer_1' />
                                      <FieldRef Name='Dept_Reviewer_2' />
                                      <FieldRef Name='Dept_Reviewer_3' />
                                      <FieldRef Name='Dept_Reviewer_4' />
                                      <FieldRef Name='Dept_Reviewer_5' />
                                      <FieldRef Name='IT_Reviewer' />
                                      <FieldRef Name='HR_Reviewer' />
                                      <FieldRef Name='Legal_Reviewer' />
                                      <FieldRef Name='Ops_Leader' />
                                      <FieldRef Name='Ops_Leader_2' />
                                      <FieldRef Name='Ops_Leader_3' />
                                      <FieldRef Name='Finance_Leader' />
                                      <FieldRef Name='President' />
                                   </ViewFields>
                                   <Where>
                                      <And>
                                         <Eq>
                                            <FieldRef Name='Division' />
                                            <Value Type='Lookup'>{0}</Value>
                                         </Eq>
                                         <Eq>
                                            <FieldRef Name='Company_Name' />
                                            <Value Type='Lookup'>{1}</Value>
                                         </Eq>
                                      </And>
                                   </Where>
  
                                </Query>

                            </View>", division, company);

                    var arItems = arList.GetItems(query);
                    clientContext.Load(arItems);
                    clientContext.ExecuteQuery();
                    foreach (var itm in arItems)
                    {

                        //string DeptReviewer_1 = string.Empty;
                        if (itm["Dept_Reviewer_1"] != null)
                        {
                            //FieldUserValue Dept_Reviewer_1 = itm["Dept_Reviewer_1"] as FieldUserValue;
                            //DeptReviewer_1 = Dept_Reviewer_1.LookupValue;
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["Dept_Reviewer_1"]));
                        }
                        string DeptReviewer_2 = string.Empty;
                        if (itm["Dept_Reviewer_2"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["Dept_Reviewer_2"]));
                        }
                        string DeptReviewer_3 = string.Empty;
                        if (itm["Dept_Reviewer_3"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["Dept_Reviewer_3"]));
                        }
                        string DeptReviewer_4 = string.Empty;
                        if (itm["Dept_Reviewer_4"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["Dept_Reviewer_4"]));
                        }
                        string DeptReviewer_5 = string.Empty;
                        if (itm["Dept_Reviewer_5"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["Dept_Reviewer_5"]));
                        }
                        string IT_Reviewer1 = string.Empty;
                        if (itm["IT_Reviewer"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["IT_Reviewer"]));
                        }
                        string HR_Reviewer1 = string.Empty;
                        if (itm["HR_Reviewer"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["HR_Reviewer"]));
                        }
                        string Legal_Reviewer1 = string.Empty;
                        if (itm["Legal_Reviewer"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["Legal_Reviewer"]));
                        }
                        string Ops_Leader1 = string.Empty;
                        if (itm["Ops_Leader"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["Ops_Leader"]));
                        }
                        string Ops_Leader_2 = string.Empty;
                        if (itm["Ops_Leader_2"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["Ops_Leader_2"]));
                        }
                        string Ops_Leader_3 = string.Empty;
                        if (itm["Ops_Leader_3"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["Ops_Leader_3"]));
                        }
                        string Finance_Leader = string.Empty;
                        if (itm["Finance_Leader"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["Finance_Leader"]));
                        }
                        string President = string.Empty;
                        if (itm["President"] != null)
                        {
                            users.Add(this.GetUserFromLookupId(clientContext, (FieldUserValue)itm["President"]));
                        }

                       
                    }

                }

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "ARDiscussionViewModel exception during fetching all approvers ");
            }
            




            return users;
        }

        private Array GetUsers(List<UserDetails> users)
        {
            Array userArray = null;
            if (users != null)
            {
                var usersFilter = users.Where(a => !string.IsNullOrEmpty(a.Email)).ToList<UserDetails>();
                userArray = Array.CreateInstance(typeof(FieldUserValue), usersFilter.Count());
                for (int i = 0; i < userArray.Length; i++)
                {
                    // if(!string.IsNullOrEmpty(users[i].Email))
                    userArray.SetValue(FieldUserValue.FromUser(usersFilter[i].UserName), i);
                }


            }

            return userArray;
        }

        private List<UserDetails> GetUserFromItem(FieldLookupValue[] users, Microsoft.SharePoint.Client.ClientContext clientContext)
        {
            List<UserDetails> userDetails = new List<UserDetails>();
            if (users != null)
            {
                foreach (FieldLookupValue value in users)
                {
                    var user = GetUserFromLookupId(clientContext, value);
                    userDetails.Add(user);
                }
            }

            return userDetails;
        }

        private UserDetails GetUserFromLookupId(ClientContext clientContext, Microsoft.SharePoint.Client.FieldLookupValue userValue)
        {
            User user = clientContext.Web.GetUserById(userValue.LookupId);
            clientContext.Load(user);
            clientContext.ExecuteQuery();
            UserDetails userDetails = new UserDetails();
            userDetails.Email = user.Email;
            userDetails.Name = user.Title;
            userDetails.ID = user.Id.ToString();
            userDetails.UserName = user.LoginName;
            return userDetails;
        }
    }
}