using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using model = SB.AR.AppWeb.Models;
using NSBD.SharepointAutoMapper;
using SB.AR.AppWeb.ViewModels;
using Newtonsoft.Json;
using SB.AR.AppWeb.Models;

namespace SB.AR.AppWeb.Controllers
{
    public class SBControllerBase : Controller
    {
        public SharePointContext SPContext
        {
            get
            {
               
                if(Session["spContext"] !=null)
                    return (SharePointContext)Session["spContext"];
                else
                {
                    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
                    SetSPContext(spContext);
                    return spContext;
                }
            }
        }
        public string SPHostUrl
        {
            get
            {

                if (Session["SPHostUrl"] != null)
                    return (string)Session["SPHostUrl"];
                else if (Request["SPHostUrl"] != null)
                {
                    Session["SPHostUrl"] = Request["SPHostUrl"];
                    return (string)Request["SPHostUrl"];
                }

                return string.Empty;
            }
        }
        public string SPAppWebUrl
        {
            get
            {

                if (Session["SPAppWebUrl"] != null)
                    return (string)Session["SPAppWebUrl"];
                else if (Request["SPAppWebUrl"] != null)
                {
                    Session["SPAppWebUrl"] = Request["SPAppWebUrl"];
                    return (string)Request["SPAppWebUrl"];
                }

                return string.Empty;
            }
        }
        public string SPLanguage
        {
            get
            {

                if (Session["SPLanguage"] != null)
                    return (string)Session["SPLanguage"];
                else if (Request["SPLanguage"] != null)
                {
                    Session["SPLanguage"] = Request["SPLanguage"];
                    return (string)Request["SPLanguage"];
                }

                return string.Empty;
            }
        }

        public User CurrentUser
        { 
                get 
                  {
                        User spUser = null;                        
                        using (var clientContext = SPContext.CreateUserClientContextForSPHost())
                        {
                            if (clientContext != null)
                            {
                                spUser = clientContext.Web.CurrentUser;
                                clientContext.Load(spUser);
                                clientContext.ExecuteQuery();
                            }
                        }
                        return spUser;
                  } 
        }

        public ARViewModel ARViewModel
        {
            get
            {
                var ar = new SB.AR.AppWeb.Models.AR();

                if (this.AR != null)
                    ar = this.AR;
                else
                {
                    ar.Title = this.ARTitle;
                    ar.AR_Type = this.ARType;
                }
                var arViewModel = new ARViewModel
                {
                    AR = ar,
                    ARTypeViewModel = null,
                    AR_Type = this.ARTypeName
                };
                return arViewModel;
            }
        }

        public string ARTitle
        {
            get
            {
                if (Session["arTitle"] != null)
                    return (string)Session["arTitle"];
                return string.Empty;
            }
            set
            {
                Session["arTitle"] = value;
            }
        }
        public string ARType
        {
            get
            {
                if (Session["ARType"] != null)
                    return (string)Session["ARType"];
                return string.Empty;
            }
            set
            {
                Session["ARType"] = value; 
            }
        }


        public string ARTypeName
        {
            get
            {
                if(Session["ARTypeCollection"] != null)
                {
                    List<SelectListItem> choice = (List<SelectListItem>) Session["ARTypeCollection"];
                    var type = choice.Where(c => c.Value == this.ARType).FirstOrDefault();
                    if (type != null)
                       return  type.Text;
                }
                return string.Empty;
            }
        }

        public AR.AppWeb.Models.AR AR
        {
            get
            {
                if (Session["AR"] != null)
                {
                    var ar= (AR.AppWeb.Models.AR)Session["AR"];

                    if (Request.QueryString["id"] != null)
                    {
                        int id = Convert.ToInt32(Request["id"]);
                        ar = GetARById(id);
                    }
                    ar.SPHostUrl = this.SPHostUrl;
                    ar.SPAppWebUrl = this.SPAppWebUrl;
                    ar.SPLanguage = this.SPLanguage;
                    SetARIfRejected(ar);
                    return ar;
                }
                else if (Request.QueryString["id"] != null)
                {
                    int id = Convert.ToInt32(Request["id"]);
                    var ar = GetARById(id);

                    if(ar != null)
                    {
                        SetARIfRejected(ar);
                    }
                    return ar;
                }
                else if (Request.QueryString["aid"] != null)
                {
                    int id = Convert.ToInt32(Request["aid"]);
                    var ar = GetARById(id);

                    if (ar != null)
                    {
                        SetARIfRejected(ar);
                    }
                    return ar;
                }
                return null;
            }
            set
            {
                Session["AR"] = value;
            }
        }
        private void SetARIfRejected(AR.AppWeb.Models.AR ar)
        {
            List<Models.WorkFlow> WorkFlows = new List<Models.WorkFlow>();
            if (SPContext != null && ar.AR_ID != null && ar.AR_ID > 0)
            {
                using (var clientContext = SPContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {

                        if (!string.IsNullOrEmpty(ar.Submit_Action) && ar.Submit_Action.Equals("Complete-Rejected"))
                        {
                            ar.IsRejected = true;
                            return;
                        }
                        string strArId = ((double)ar.AR_ID).ToString("N");
                        strArId = strArId.Replace(".00", "").Replace(".0", "");
                        List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.Workflow_Tasks);
                        CamlQuery query = new CamlQuery();
                        query.ViewXml = string.Format(@"<View><Query><Where>
                                        <BeginsWith>
                                            <FieldRef Name='Title' />
                                            <Value Type='Text'>AR#{0}</Value>
                                        </BeginsWith>
                                        </Where><OrderBy><FieldRef Name='Created' Ascending='False' />
                                        </OrderBy></Query></View>", strArId);
                        var arItems = arList.GetItems(query);
                        clientContext.Load(arItems);
                        clientContext.ExecuteQuery();

                        foreach (var itm in arItems)
                        {
                            if (itm["Decision"] != null)
                            {
                                object decision = itm["Decision"];
                                if (decision != null && decision.ToString().ToLower().Trim() == "2".ToLower().Trim())
                                {
                                    ar.IsRejected = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        public void SetSPContext(SharePointContext spContext)
        {
            Session["spContext"] = spContext;
        }
        //[SharePointContextFilter]
        public ActionResult GetPeoplePickerData()
        {
            var result = this.Json(PeoplePickerHelper.GetPeoplePickerSearchData(SPContext));
            return result;
        }
        public ActionResult PeoplePickerJson(string query)
        {
           
            string principalType = "1";
            var result = this.Json(PeoplePickerHelper.GetPeoplePickerData(SPContext, query, principalType));   
     
            var peopleData = JsonConvert.DeserializeObject<List<RootObject>>(result.Data.ToString());

            var people = (from p in peopleData
                          select new People
                          {
                             LookupId = p.Key,
                             LookupValue = !string.IsNullOrEmpty(p.EntityData.Email) ? string.Format("{0} ({1})", p.DisplayText, p.EntityData.Email) : p.DisplayText
                          }).ToList();

            return Json(people, JsonRequestBehavior.AllowGet);
        }


        public AR.AppWeb.Models.AR GetARById(int arId)
        {
            if (SPContext != null)
            {
                using (var clientContext = SPContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        //For all
                        List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);
                        var query = new CamlQuery();
                        query.ViewXml = string.Format(@"<View>  
                                <Query>
                                     <Where><Eq><FieldRef Name='AR_ID' /><Value Type='Number'>{0}</Value></Eq></Where>
                                </Query> 
                                <RowLimit>1</RowLimit> 
                                </View>", arId);


                        var arItem = arList.GetItems(query);
                        clientContext.Load(arItem);
                        clientContext.ExecuteQuery();

                        //for single -Author only

                     //   List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);
                        var queryAuthor = new CamlQuery();
                        queryAuthor.ViewXml = string.Format(@"<View>
                                                                <ViewFields>
                                                                    <FieldRef Name='AR_ID' />
                                                                    <FieldRef Name='Author' />
                                                                </ViewFields>
                                                                <Query>
                                                                    <Where>
                                                                        <Eq>
                                                                            <FieldRef Name='AR_ID' />
                                                                            <Value Type='Number'>{0}</Value>
                                                                        </Eq>
                                                                    </Where>
                                                                </Query>
                                                            </View>", arId);


                        var arItemAuthor = arList.GetItems(queryAuthor);
                        clientContext.Load(arItemAuthor);
                        clientContext.ExecuteQuery();




                        if (arItem != null && arItem.Count > 0)
                        {

                            var restAR =   arItem.ProjectToListEntity<Models.AR>().FirstOrDefault();
                            //author only
                            var itemAuthor = arItemAuthor.FirstOrDefault();

                            try
                            {
                                FieldLookupValue lookup = (FieldLookupValue)itemAuthor["Author"];
                                if (lookup != null)
                                {
                                    LookupFieldMapper authorMap = new LookupFieldMapper()
                                    {
                                        ID = lookup.LookupId,
                                        Value = lookup.LookupValue
                                    };
                                    restAR.Author = authorMap;
                                }
                            }
                            catch(Exception e)
                            {

                            }
                            
                    
                            
                            return restAR;
                        }
                    }

                    
                }
            }
            return null;
        }
        public ListItem GetItemById(ClientContext clientContext, int arId)
        {
            
            if (clientContext != null)
            {
                List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);
                var query = new CamlQuery();
                query.ViewXml = string.Format(@"<View>  
                            <Query>
                                <Where><Eq><FieldRef Name='ID' /><Value Type='Number'>{0}</Value></Eq></Where>
                            </Query> 
                            <RowLimit>1</RowLimit> 
                            </View>", arId);
                var arItem = arList.GetItems(query);
                clientContext.Load(arItem);
                clientContext.ExecuteQuery();
                if (arItem != null && arItem.Count > 0)
                    return arItem[0];
            }

            return null;
        }
        public void Delete(int arId)
        {            
            using (var clientContext = SPContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {                    
                    List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);                       
                    var arItem = arList.GetItemById(arId);
                    arItem.DeleteObject();
                    clientContext.ExecuteQuery();
                }
            }
        }

        public HttpResponseMessage PrepareARListToSave(AR.AppWeb.Models.AR ar)
        {
            return SaveARToList(ar);
        }
        public HttpResponseMessage SaveARToList(AR.AppWeb.Models.AR ar)
        {
           
            ViewBag.ArTypeName = this.ARTypeName;


            if (!string.IsNullOrEmpty(ar.Title))
                this.ARTitle = ar.Title;
            else
                ar.Title = this.ARTitle;

            if (string.IsNullOrEmpty(ar.Title))
            {
                var emptyresponse = new HttpResponseMessage(HttpStatusCode.NotImplemented)
                {
                    Content = new StringContent(string.Empty)
                };
                return emptyresponse;
            }

            using (var clientContext = SPContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                        List oList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);
                        ListItem oListItem = null;

                        if (ar.ID == 0)
                        {
                            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                                oListItem = oList.AddItem(itemCreateInfo);
                        }

                        else if (ar.ID > 0)
                        {
                            oListItem = oList.GetItemById(ar.ID);
                            clientContext.Load(oListItem);
                            clientContext.ExecuteQuery();
                        }

                        if (ar.CompanyId > 0)
                            oListItem["Company_Name"] = ar.CompanyId;

                        if (ar.DivisionId > 0)
                            oListItem["Division"] = ar.DivisionId;

                        if (ar.CategoryId > 0)
                            oListItem["Category"] = ar.CategoryId;


                        #region int, double

                        if (!(ar.Original_AR_Amount == 0))
                            oListItem["Original_AR_Amount"] = ar.Original_AR_Amount;

                        if (!(ar.AdditionaAmountFundsRequired == 0))
                            oListItem["Amount_of_Additional_Funds_Requi"] = ar.AdditionaAmountFundsRequired;

                        if (!(ar.AmountOfFundingRequiredFrom == 0))
                            oListItem["Amount_of_Funding_Required_from_"] = ar.AmountOfFundingRequiredFrom;
                        if (!(ar.Budget_Amount == 0))
                        oListItem["Budget_Amount"] = ar.Budget_Amount;

                        if (!(ar.Cost_Per_Period == 0))
                        oListItem["Cost_Per_Period"] = ar.Cost_Per_Period;
                        if (!(ar.Current_Net_Book_Value == 0))
                        oListItem["Current_Net_Book_Value"] = ar.Current_Net_Book_Value;

                        if (!(ar.Current_Year_Cost_Commitment == 0))
                        oListItem["Current_Year_Cost_Commitment"] = ar.Current_Year_Cost_Commitment;

                        if (!(ar.Expected_Proceeds == 0))
                        oListItem["Expected_Proceeds"] = ar.Expected_Proceeds;

                        if (!(ar.LengthOfLease == 0))
                            oListItem["Length_of_Lease__x0028__x0023__o"] = ar.LengthOfLease;

                        if (ar.Market_Value > 0)
                        oListItem["Market_Value"] = ar.Market_Value;

                        if (!(ar.Number_of_Share_to_Aquire == 0))
                        oListItem["Number_of_Share_to_Aquire"] = ar.Number_of_Share_to_Aquire;

                        if (ar.Number_of_Shares_to_Sell > 0)
                        oListItem["Number_of_Shares_to_Sell"] = ar.Number_of_Shares_to_Sell;

                        if (ar.Original_Cost > 0)
                        oListItem["Original_Cost"] = ar.Original_Cost;

                        if (ar.Original_Purchase_Price > 0)
                        oListItem["Original_Purchase_Price"] = ar.Original_Purchase_Price;

                        if (!(ar.Other_Cost_or_Savings == 0))
                        oListItem["Other_Cost_or_Savings"] = ar.Other_Cost_or_Savings;

                        if (!(ar.PercentageOfOwnership == 0))
                            oListItem["Percentage_of_Ownership_x002F_Sh"] = ar.PercentageOfOwnership;

                        if (!(ar.Percentage_of_Ownership_at_Close == 0))
                        oListItem["Percentage_of_Ownership_at_Close"] = ar.Percentage_of_Ownership_at_Close;

                        if (!(ar.Price_Per_Share_Being_Purchased == 0))
                        oListItem["Price_Per_Share_Being_Purchased"] = ar.Price_Per_Share_Being_Purchased;

                        if (!(ar.Price_Per_Share_Being_Sold == 0))
                        oListItem["Price_Per_Share_Being_Sold"] = ar.Price_Per_Share_Being_Sold;

                        if (!(ar.Principal_Amount == 0))
                        oListItem["Principal_Amount"] = ar.Principal_Amount;

                        if (!(ar.Purchase_Price == 0))
                        oListItem["Purchase_Price"] = ar.Purchase_Price;

                        if (!(ar.ServicesInstallation == 0))
                            oListItem["Services_x002F_Installation"] = ar.ServicesInstallation;

                        if (!(ar.TaxVAT == 0))
                            oListItem["Tax_x002F_VAT"] = ar.TaxVAT;

                        if (!(ar.Total_Cost == 0))
                        oListItem["Total_Cost"] = ar.Total_Cost;

                        if (!(ar.Total_Other_Costs == 0))
                            oListItem["Total_Other_Costs"] = ar.Total_Other_Costs;
                        #endregion
                        
                            
                        
                        #region string

                        if (!string.IsNullOrEmpty(ar.Attachment_Folder_Id))
                            oListItem["Attachment_Folder_Id"] = ar.Attachment_Folder_Id;

                       
                         oListItem["Current_Status"] = ar.Current_Status;

                        if (!string.IsNullOrEmpty(ar.Submit_Action))
                            oListItem["Submit_Action"] = ar.Submit_Action;


                        if (!string.IsNullOrEmpty(ar.Equity_Description))
                            oListItem["Equity_Description"] = ar.Equity_Description;

                        if (!string.IsNullOrEmpty(ar.Title))
                        oListItem["Title"] = ar.Title;


                        if (!string.IsNullOrEmpty(ar.AR_Type))
                        oListItem["AR_Type"] = ar.AR_Type;

                        if (!string.IsNullOrEmpty(ar.ARNumber))
                            oListItem["AR__x0023_"] = ar.ARNumber;


                        if (!string.IsNullOrEmpty(ar.Asset_Location))
                        oListItem["Asset_Location"] = ar.Asset_Location;

                        if (!string.IsNullOrEmpty(ar.Borrower))
                        oListItem["Borrower"] = ar.Borrower;

                        if (!string.IsNullOrEmpty(ar.BudgetLineItem))
                            oListItem["Budget_Line_Item__x0023_"] = ar.BudgetLineItem;

                        if (!string.IsNullOrEmpty(ar.Condition_of_Assets))
                        oListItem["Condition_of_Assets"] = ar.Condition_of_Assets;

                        if (!string.IsNullOrEmpty(ar.ConsolidatedNonCon))
                            oListItem["Consolidated_x002F_Non_x002d_Con"] = ar.ConsolidatedNonCon;


                        if (!string.IsNullOrEmpty(ar.Currency_Name))
                        oListItem["Currency_Name"] = ar.Currency_Name;
                        if (!string.IsNullOrEmpty(ar.Current_Approver))
                        oListItem["Current_Approver"] = ar.Current_Approver;

                        if (!string.IsNullOrEmpty(ar.Department_to_Charge))
                        oListItem["Department_to_Charge"] = ar.Department_to_Charge;
                        if (!string.IsNullOrEmpty(ar.Economical_Life))
                        oListItem["Economical_Life"] = ar.Economical_Life;

                        if (!string.IsNullOrEmpty(ar.InvestmentType))
                            oListItem["Investment_Type"] = ar.InvestmentType;

                        if (!string.IsNullOrEmpty(ar.LeaseType))
                            oListItem["Lease_Type"] = ar.LeaseType;

                        if (!string.IsNullOrEmpty(ar.Lender))
                        oListItem["Lender"] = ar.Lender;

                        if (!string.IsNullOrEmpty(ar.LocalAR))
                            oListItem["Local_AR__x0023_"] = ar.LocalAR;

                        if (!string.IsNullOrEmpty(ar.Location))
                        oListItem["Location"] = ar.Location;

                        if (!string.IsNullOrEmpty(ar.Maturity_Date))
                        oListItem["Maturity_Date"] = ar.Maturity_Date;

                        if (!string.IsNullOrEmpty(ar.OriginalAR))
                            oListItem["Original_AR__x0023_"] = ar.OriginalAR;

                        if (!string.IsNullOrEmpty(ar.Stated_Interest_Rate))
                        oListItem["Stated_Interest_Rate"] = ar.Stated_Interest_Rate;
            
                    
                        if (!string.IsNullOrEmpty(ar.PresentSituationIssue))
                            oListItem["Present_Situation_x002F_Issue"] = ar.PresentSituationIssue;

                        if (!string.IsNullOrEmpty(ar.Proposed_Solution))
                        oListItem["Proposed_Solution"] = ar.Proposed_Solution;

                        if (!string.IsNullOrEmpty(ar.Other_Potential_Solutions))
                        oListItem["Other_Potential_Solutions"] = ar.Other_Potential_Solutions;

                        if (!string.IsNullOrEmpty(ar.Explanation_of_Costs))
                        oListItem["Explanation_of_Costs"] = ar.Explanation_of_Costs;


                        if (!string.IsNullOrEmpty(ar.Financial_Measures))
                        oListItem["Financial_Measures"] = ar.Financial_Measures;

                        #endregion
                        #region DateTime


                        if (!(ar.Date_Aquired == null))
                        oListItem["Date_Aquired"] = ar.Date_Aquired;

                        if (!(ar.Date_Assigned == null))
                        oListItem["Date_Assigned"] = ar.Date_Assigned;

                        if (!(ar.Lend_Date == null))
                        oListItem["Lend_Date"] = ar.Lend_Date;

                        if (!(ar.Project_End == null))
                        oListItem["Project_End"] = ar.Project_End;

                        if (!(ar.Project_Start == null))
                        oListItem["Project_Start"] = ar.Project_Start;

                        if (!(ar.Response_Due_Date == null))
                        oListItem["Response_Due_Date"] = ar.Response_Due_Date;

                        if (!(ar.Transaction_Close_Date == null))
                        oListItem["Transaction_Close_Date"] = ar.Transaction_Close_Date;
                        #endregion


                        #region Boolean
                        if (ar.IsMaintab)
                        {
                        oListItem["Engineering_Review"] = ar.Engineering_Review;
                        oListItem["HR_Review"] = ar.HR_Review;
                        oListItem["IT_Review"] = ar.IT_Review;
                        oListItem["Legal_Review"] = ar.Legal_Review;
                        }

                        if(!ar.IsDisposalFinance && ar.IsFinanceTab)
                        {
                            oListItem["Funds_Committed"] = ar.Funds_Committed;
                            oListItem["In_Budget"] = ar.In_Budget;

                        }
                        #endregion

                 
                    oListItem.Update();
                    clientContext.Load(oListItem);
                    clientContext.ExecuteQuery();

                    #region Owner
                    if (!string.IsNullOrEmpty(ar.PMOwner))
                    {
                        int arListId = Convert.ToInt32(oListItem["ID"]);
                        oListItem = oList.GetItemById(arListId);

                        FieldUserValue pmOwner = !string.IsNullOrEmpty(ar.PMOwner) ? PeoplePickerHelper.SPEnsureSBUser(clientContext, ar.PMOwner) : null;
                        
                        oListItem["PM_x002F_Owner"] = pmOwner.LookupId;
                        oListItem.Update();
                        clientContext.Load(oListItem);
                        clientContext.ExecuteQuery();
                        
                    }
                    #endregion

                    var item = oListItem.ProjectToEntity<SB.AR.AppWeb.Models.AR>();

                    Session["AR"] = item;
                   
                }
            }
            var response = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(ar.ID.ToString())
            };
            return response;
        
        }


        public bool UpdateWorkflowTask(int wId, string status, string approverComments)
        {
          

                using (var clientContext = SPContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        List oList = clientContext.Web.Lists.GetByTitle(SPListMeta.Workflow_Tasks);
                        ListItem oListItem = null;

                        if (wId == 0)
                        {
                            return false;
                        }
                        else if (wId > 0)
                        {
                            oListItem = oList.GetItemById(wId);
                            clientContext.Load(oListItem);
                            clientContext.ExecuteQuery();
                        }

                        if (!string.IsNullOrEmpty(status))
                        { 
                            
                            if (status.ToLower().Equals("completed"))
                            {
                                

                                oListItem["Status"] = "Completed";
                                //oListItem["WorkflowOutcome"] = "Approved";
                                oListItem["Decision"] = 1; // it appears that '1' approves the task
                                oListItem["Date_x0020_Approved"] = DateTime.Now;
                            }
                            else
                            {
                                oListItem["Status"] = "Completed";
                                oListItem["Decision"] = 2; // it appears that '2' rejects the task
                                //oListItem["WorkflowOutcome"] = "Reject";
                            }
                        }

                       if (!string.IsNullOrEmpty(approverComments))
                            oListItem["ApproverComments"] = approverComments;

                        oListItem.Update();
                        clientContext.Load(oListItem);
                        clientContext.ExecuteQuery();

                       
                    }
                }
                return true;
            
        }

        public bool IsSiteCollectionAdmin()
        {
            bool isCurrentUserSiteCollectionAdmin = false;
            using (var clientContext = SPContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    clientContext.Load(clientContext.Web, web => web.CurrentUser);
                    clientContext.ExecuteQuery();

                    isCurrentUserSiteCollectionAdmin = clientContext.Web.CurrentUser.IsSiteAdmin;
                }


            }

            return isCurrentUserSiteCollectionAdmin;
        }
    }
}