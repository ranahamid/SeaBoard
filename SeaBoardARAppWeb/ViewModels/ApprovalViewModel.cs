using Microsoft.SharePoint.Client;
using NSBD.SharepointAutoMapper;
using SB.AR.AppWeb.Helper;
using SB.AR.AppWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using arapp = SB.AR.AppWeb.Models;
namespace SB.AR.AppWeb.ViewModels
{

    public class ApprovalViewModel : ViewModelBase
    {

        private arapp.AR _ar;
        public ApprovalViewModel(SharePointContext spContext, arapp.AR ar)
            : base(spContext)
        {
            _ar = ar;
        }
        public SharePointContext SharePointContext
        {
            get
            {
                return base._spContext;
            }
        }

        public arapp.AR AR
        {
            get
            {
                return this._ar;
            }

        }
        public string DivisionName
        {
            get
            {
                if (_ar.Division == null)
                    return string.Empty;
                return _ar.Division.Value;
            }
        }
        public string CompanyName
        {
            get
            {
                if (_ar.Company_Name == null)
                    return string.Empty;
                return _ar.Company_Name.Value;
            }
        }
        public string ARID
        {
            get
            {
                if (_ar.AR_ID > 0)
                {
                    return ((int)_ar.AR_ID).ToString("N").Replace(".00", "");
                }
                return string.Empty;
            }
        }

        public string AR_ID
        {
            get
            {
                if (_ar.AR_ID > 0)
                {
                    return ((int)_ar.AR_ID).ToString().Replace(".0", "");
                }
                return string.Empty;
            }
        }
        public User CurrentUser
        {
            get
            {
                User spUser = null;
                using (var clientContext = _spContext.CreateUserClientContextForSPHost())
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
        public AR.AppWeb.Models.WorkFlow ApprovalWorkflow
        {
            get;
            set;
        }

        List<string> CompanyApproverFirst;
        Dictionary<string, string> ApproverDic = new Dictionary<string, string>();


        public List<AR.AppWeb.Models.ApprovalHistory> GetApprovalsHistory
        {
            get
            {
                // CompanyApproverFirst = new List<string>();
                List<Models.ApprovalHistory> approvalHistory = new List<Models.ApprovalHistory>();
                try
                {
                    using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                    {
                        if (clientContext != null)
                        {


                            //#region ApprovalHistory
                            List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.ApprovalHistory);

                            CamlQuery query = new CamlQuery();

                            query.ViewXml = string.Format(@"<View>
                                    <Query>
                                          <ViewFields>
                                              <FieldRef Name='AR_ID' />
                                              <FieldRef Name='Title' />
                                              <FieldRef Name='Approver' />
                                              <FieldRef Name='Date_Assigned' />
                                              <FieldRef Name='Date_Completed' />
                                              <FieldRef Name='Task_Status' />
                                              <FieldRef Name='Sequence' />
                                             
                                           </ViewFields>
                                           <Where>
                                              <Eq>
                                                 <FieldRef Name='AR_ID' />
                                                 <Value Type='Number'>{0}</Value>
                                              </Eq>
                                           </Where>                             
                                    </Query>
                                </View>", AR_ID);//, AR_ID
                            var arItems = arList.GetItems(query);
                            clientContext.Load(arItems);
                            clientContext.ExecuteQuery();

                            foreach (var itm in arItems)
                            {
                                var apprHistory = itm.ProjectToEntity<SB.AR.AppWeb.Models.ApprovalHistory>();
                                approvalHistory.Add(apprHistory);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Utility.Logging.LogErrorException(ex, "Get All My Approvals or SP Context is null");
                }

                return approvalHistory;
            }
        }

        public List<AR.AppWeb.Models.WorkFlow> AllApprovals
        {
            get
            {
                // CompanyApproverFirst = new List<string>();
                List<Models.WorkFlow> WorkFlows = new List<Models.WorkFlow>();
                try
                {
                    using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                    {
                        if (clientContext != null)
                        {


                            #region companyApprover
                            List arList1 = clientContext.Web.Lists.GetByTitle(SPListMeta.ARAPPROVERS);
                            CamlQuery query1 = new CamlQuery();

                            query1.ViewXml = string.Format(@"
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

                            </View>", DivisionName, CompanyName);

                            var arItems1 = arList1.GetItems(query1);
                            clientContext.Load(arItems1);
                            clientContext.ExecuteQuery();
                            foreach (var itm in arItems1)
                            {

                                string DeptReviewer_1 = string.Empty;
                                if (itm["Dept_Reviewer_1"] != null)
                                {
                                    FieldUserValue Dept_Reviewer_1 = itm["Dept_Reviewer_1"] as FieldUserValue;
                                    DeptReviewer_1 = Dept_Reviewer_1.LookupValue;
                                }
                                string DeptReviewer_2 = string.Empty;
                                if (itm["Dept_Reviewer_2"] != null)
                                {
                                    FieldUserValue DeptReviewer_2_1 = itm["Dept_Reviewer_2"] as FieldUserValue;
                                    DeptReviewer_2 = DeptReviewer_2_1.LookupValue;
                                }
                                string DeptReviewer_3 = string.Empty;
                                if (itm["Dept_Reviewer_3"] != null)
                                {
                                    FieldUserValue DeptReviewer_3_1 = itm["Dept_Reviewer_3"] as FieldUserValue;
                                    DeptReviewer_3 = DeptReviewer_3_1.LookupValue;
                                }
                                string DeptReviewer_4 = string.Empty;
                                if (itm["Dept_Reviewer_4"] != null)
                                {
                                    FieldUserValue DeptReviewer_4_1 = itm["Dept_Reviewer_4"] as FieldUserValue;
                                    DeptReviewer_4 = DeptReviewer_4_1.LookupValue;
                                }
                                string DeptReviewer_5 = string.Empty;
                                if (itm["Dept_Reviewer_5"] != null)
                                {
                                    FieldUserValue Dept_Reviewer_5_1 = itm["Dept_Reviewer_5"] as FieldUserValue;
                                    DeptReviewer_5 = Dept_Reviewer_5_1.LookupValue;
                                }
                                string IT_Reviewer1 = string.Empty;
                                if (_ar.IT_Review)
                                {
                                    if (itm["IT_Reviewer"] != null)
                                    {
                                        FieldUserValue IT_Reviewer_1 = itm["IT_Reviewer"] as FieldUserValue;
                                        IT_Reviewer1 = IT_Reviewer_1.LookupValue;
                                    }
                                }
                                string HR_Reviewer1 = string.Empty;
                                if (_ar.HR_Review)
                                {
                                    if (itm["HR_Reviewer"] != null)
                                    {
                                        FieldUserValue HR_Reviewer_1 = itm["HR_Reviewer"] as FieldUserValue;
                                        HR_Reviewer1 = HR_Reviewer_1.LookupValue;
                                    }
                                }
                                string Legal_Reviewer1 = string.Empty;
                                if (_ar.Legal_Review)
                                {
                                    if (itm["Legal_Reviewer"] != null)
                                    {
                                        FieldUserValue Legal_Reviewer_1 = itm["Legal_Reviewer"] as FieldUserValue;
                                        Legal_Reviewer1 = Legal_Reviewer_1.LookupValue;
                                    }
                                }

                                string Ops_Leader1 = string.Empty;
                                if (itm["Ops_Leader"] != null)
                                {
                                    FieldUserValue Ops_Leader_1 = itm["Ops_Leader"] as FieldUserValue;
                                    Ops_Leader1 = Ops_Leader_1.LookupValue;
                                }
                                string Ops_Leader_2 = string.Empty;
                                if (itm["Ops_Leader_2"] != null)
                                {
                                    FieldUserValue Ops_Leader_2_1 = itm["Ops_Leader_2"] as FieldUserValue;
                                    Ops_Leader_2 = Ops_Leader_2_1.LookupValue;
                                }
                                string Ops_Leader_3 = string.Empty;
                                if (itm["Ops_Leader_3"] != null)
                                {
                                    FieldUserValue Ops_Leader_3_1 = itm["Ops_Leader_3"] as FieldUserValue;
                                    Ops_Leader_3 = Ops_Leader_3_1.LookupValue;
                                }
                                string Finance_Leader = string.Empty;
                                if (itm["Finance_Leader"] != null)
                                {
                                    FieldUserValue Finance_Leader1 = itm["Finance_Leader"] as FieldUserValue;
                                    Finance_Leader = Finance_Leader1.LookupValue;
                                }
                                string President = string.Empty;
                                if (itm["President"] != null)
                                {
                                    FieldUserValue PresidentField = itm["President"] as FieldUserValue;
                                    President = PresidentField.LookupValue;
                                }


                                ApproverDic.Add("Department 1", DeptReviewer_1);
                                ApproverDic.Add("Department 2", DeptReviewer_2);
                                ApproverDic.Add("Department 3", DeptReviewer_3);
                                ApproverDic.Add("Department 4", DeptReviewer_4);
                                ApproverDic.Add("Department 5", DeptReviewer_5);
                                ApproverDic.Add("IT Reviewer", IT_Reviewer1);
                                ApproverDic.Add("HR Reviewer", HR_Reviewer1);
                                ApproverDic.Add("Legal Reviewer", Legal_Reviewer1);
                                ApproverDic.Add("Operations", Ops_Leader1);
                                ApproverDic.Add("Operations 2", Ops_Leader_2);
                                ApproverDic.Add("Operations 3", Ops_Leader_3);
                                ApproverDic.Add("Finance", Finance_Leader);
                                ApproverDic.Add("President", President);

                            }

                            #endregion
                            /*<Query>
        <Where>
            <Contains>
                <FieldRef Name="Title" />
                <Value Type="Text">4,067</Value>
            </Contains>
        </Where>
    </Query>*/
                            List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.Workflow_Tasks);
                            CamlQuery query = new CamlQuery();

                            query.ViewXml = string.Format(@"<View>
                                    <Query>
                                       <Where>
                                            <Or>
                                                <Or>
                                                    <Or>
                                                        <Or>
                                                            <Contains>
                                                              <FieldRef Name='Title' />
                                                                <Value Type='Text'>{0}&nbsp;</Value>
                                                            </Contains>
                                                            <Contains>
                                                              <FieldRef Name='Title' />
                                                                <Value Type='Text'>{0} </Value>
                                                            </Contains>
                                                        </Or>
                                                            <Contains>
                                                              <FieldRef Name='Title' />
                                                                <Value Type='Text'>AR#&nbsp;{0}</Value>
                                                            </Contains>
                                                    </Or>
                                                     <Contains>
                                                        <FieldRef Name='Title' />
                                                        <Value Type='Text'>AR#{0}</Value>
                                                    </Contains>
                                                </Or>
                                                    <Contains>
                                                        <FieldRef Name='Title' />
                                                        <Value Type='Text'>AR# {0} </Value>
                                                    </Contains>
                                            </Or>
                                        </Where>                              
                                    </Query>
                                </View>", ARID);
                            var arItems = arList.GetItems(query);
                            clientContext.Load(arItems);
                            clientContext.ExecuteQuery();
                            foreach (var itm in arItems)
                            {
                                FieldUserValue user = null;
                                string Assign = string.Empty;
                                if (itm["AssignedTo"] != null)
                                {
                                    FieldUserValue divi = itm["AssignedTo"] as FieldUserValue;
                                    Assign = divi.LookupValue;

                                    user = new FieldUserValue
                                    {
                                        LookupId = divi.LookupId,
                                    };
                                }

                                var arId = itm["Title"] != null ? Convert.ToString(itm["Title"]) : "";
                                double AR_ID = 0;
                                if (arId.Trim().StartsWith("AR#"))
                                {
                                    arId = arId.Split(' ')[0];
                                    if (!string.IsNullOrEmpty(arId))
                                        arId = arId.Trim();

                                    if (arId.Contains("["))
                                        arId = arId.Split('[')[0];

                                    if (!string.IsNullOrEmpty(arId))
                                        arId = arId.Trim();

                                    arId = arId.Replace(",", "").Replace("AR#", "");
                                    var result = double.TryParse(arId, out AR_ID);
                                }

                                DateTime dateApproved = Convert.ToDateTime(itm["Modified"]);

                                string myKey = string.Empty;
                                if (ApproverDic.Any(x => x.Value == Assign))
                                {
                                    myKey = ApproverDic.First(x => x.Value == Assign).Key;
                                    ApproverDic.Remove(myKey);
                                }


                                var wf = new Models.WorkFlow
                                {
                                    RoleName = myKey != null ? myKey : string.Empty,
                                    PercentComplete = itm["PercentComplete"] != null ? Convert.ToDouble(itm["PercentComplete"]) : 0,
                                    Title = itm["Title"] != null ? Convert.ToString(itm["Title"]) : "",
                                    Status = itm["Status"] != null ? Convert.ToString(itm["Status"]) : "",
                                    ID = itm["ID"] != null ? Convert.ToInt32(itm["ID"]) : 0,
                                    Priority = itm["Priority"] != null ? Convert.ToString(itm["Priority"]) : "",
                                    DueDate = itm["DueDate"] != null ? Convert.ToDateTime(itm["DueDate"]) : default(DateTime),
                                    WorkflowLink = itm["WorkflowLink"] != null ? Convert.ToString(itm["WorkflowLink"]) : "",
                                    WorkflowOutcome = itm["WorkflowOutcome"] != null ? Convert.ToString(itm["WorkflowOutcome"]) : "",
                                    AssignedTo = Assign,
                                    AR_ID = AR_ID,
                                    StartDate = itm["StartDate"] != null ? Convert.ToDateTime(itm["StartDate"]) : default(DateTime),
                                    DateApproved = itm["Modified"] != null ? (DateTime?)dateApproved : null,

                                };

                                WorkFlows.Add(wf);
                                //  CompanyApproverFirst.Add(Assign);

                            }


                        }
                    }

                }
                catch (Exception ex)
                {
                    Utility.Logging.LogErrorException(ex, "Get All My Approvals or SP Context is null");
                }

                return WorkFlows;
            }
        }

        public bool ListNotContains(string approver)
        {
            if (CompanyApproverFirst.Contains(approver))
            {
                CompanyApproverFirst.Remove(approver);
                return false;
            }
            else
            {
                // CompanyApproverFirst.Add(approver); 
                return true;
            }
        }


        public List<AR.AppWeb.Models.CompanyApprover> GetAllCompanyApprovers
        {
            get
            {
                var clientContext = _spContext.CreateUserClientContextForSPHost();
                List<SB.AR.AppWeb.Models.CompanyApprover> CompanyApprover = new List<SB.AR.AppWeb.Models.CompanyApprover>();
                try
                {
                    if (clientContext != null)
                    {

                        #region  approver
                        CompanyApproverFirst = new List<string>();
                        List arListapprover = clientContext.Web.Lists.GetByTitle(SPListMeta.Workflow_Tasks);
                        CamlQuery queryapprover = new CamlQuery();

                        queryapprover.ViewXml = string.Format(@"<View>
                                    <Query>
                                       <Where>
                                            <BeginsWith>
                                              <FieldRef Name='Title' />
                                                <Value Type='Text'>AR#{0}</Value>
                                            </BeginsWith>
                                        </Where>                              
                                    </Query>
                                </View>", ARID);

                        var arItemsapprover = arListapprover.GetItems(queryapprover);
                        clientContext.Load(arItemsapprover);
                        clientContext.ExecuteQuery();
                        foreach (var itm in arItemsapprover)
                        {

                            string Assign = string.Empty;
                            if (itm["AssignedTo"] != null)
                            {
                                FieldUserValue divi = itm["AssignedTo"] as FieldUserValue;
                                Assign = divi.LookupValue;
                            }
                            CompanyApproverFirst.Add(Assign);
                        }


                        #endregion


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

                            </View>", DivisionName, CompanyName);

                        var arItems = arList.GetItems(query);
                        clientContext.Load(arItems);
                        clientContext.ExecuteQuery();
                        foreach (var itm in arItems)
                        {

                            string DeptReviewer_1 = string.Empty;
                            if (itm["Dept_Reviewer_1"] != null)
                            {
                                FieldUserValue Dept_Reviewer_1 = itm["Dept_Reviewer_1"] as FieldUserValue;
                                DeptReviewer_1 = Dept_Reviewer_1.LookupValue;
                            }
                            string DeptReviewer_2 = string.Empty;
                            if (itm["Dept_Reviewer_2"] != null)
                            {
                                FieldUserValue DeptReviewer_2_1 = itm["Dept_Reviewer_2"] as FieldUserValue;
                                DeptReviewer_2 = DeptReviewer_2_1.LookupValue;
                            }
                            string DeptReviewer_3 = string.Empty;
                            if (itm["Dept_Reviewer_3"] != null)
                            {
                                FieldUserValue DeptReviewer_3_1 = itm["Dept_Reviewer_3"] as FieldUserValue;
                                DeptReviewer_3 = DeptReviewer_3_1.LookupValue;
                            }
                            string DeptReviewer_4 = string.Empty;
                            if (itm["Dept_Reviewer_4"] != null)
                            {
                                FieldUserValue DeptReviewer_4_1 = itm["Dept_Reviewer_4"] as FieldUserValue;
                                DeptReviewer_4 = DeptReviewer_4_1.LookupValue;
                            }
                            string DeptReviewer_5 = string.Empty;
                            if (itm["Dept_Reviewer_5"] != null)
                            {
                                FieldUserValue Dept_Reviewer_5_1 = itm["Dept_Reviewer_5"] as FieldUserValue;
                                DeptReviewer_5 = Dept_Reviewer_5_1.LookupValue;
                            }
                            string IT_Reviewer1 = string.Empty;
                            if (_ar.IT_Review)
                            {
                                if (itm["IT_Reviewer"] != null)
                                {
                                    FieldUserValue IT_Reviewer_1 = itm["IT_Reviewer"] as FieldUserValue;
                                    IT_Reviewer1 = IT_Reviewer_1.LookupValue;
                                }
                            }
                            string HR_Reviewer1 = string.Empty;
                            if (_ar.HR_Review)
                            {
                                if (itm["HR_Reviewer"] != null)
                                {
                                    FieldUserValue HR_Reviewer_1 = itm["HR_Reviewer"] as FieldUserValue;
                                    HR_Reviewer1 = HR_Reviewer_1.LookupValue;
                                }
                            }
                            string Legal_Reviewer1 = string.Empty;
                            if (_ar.Legal_Review)
                            {
                                if (itm["Legal_Reviewer"] != null)
                                {
                                    FieldUserValue Legal_Reviewer_1 = itm["Legal_Reviewer"] as FieldUserValue;
                                    Legal_Reviewer1 = Legal_Reviewer_1.LookupValue;
                                }
                            }

                            string Ops_Leader1 = string.Empty;
                            if (itm["Ops_Leader"] != null)
                            {
                                FieldUserValue Ops_Leader_1 = itm["Ops_Leader"] as FieldUserValue;
                                Ops_Leader1 = Ops_Leader_1.LookupValue;
                            }
                            string Ops_Leader_2 = string.Empty;
                            if (itm["Ops_Leader_2"] != null)
                            {
                                FieldUserValue Ops_Leader_2_1 = itm["Ops_Leader_2"] as FieldUserValue;
                                Ops_Leader_2 = Ops_Leader_2_1.LookupValue;
                            }
                            string Ops_Leader_3 = string.Empty;
                            if (itm["Ops_Leader_3"] != null)
                            {
                                FieldUserValue Ops_Leader_3_1 = itm["Ops_Leader_3"] as FieldUserValue;
                                Ops_Leader_3 = Ops_Leader_3_1.LookupValue;
                            }
                            string Finance_Leader = string.Empty;
                            if (itm["Finance_Leader"] != null)
                            {
                                FieldUserValue Finance_Leader1 = itm["Finance_Leader"] as FieldUserValue;
                                Finance_Leader = Finance_Leader1.LookupValue;
                            }
                            string President = string.Empty;
                            if (itm["President"] != null)
                            {
                                FieldUserValue PresidentField = itm["President"] as FieldUserValue;
                                President = PresidentField.LookupValue;
                            }

                            CompanyApprover.Add(new Models.CompanyApprover
                            {

                                Dept_Reviewer_1 = (ListNotContains(DeptReviewer_1)) ? DeptReviewer_1 : "",
                                Dept_Reviewer_2 = (ListNotContains(DeptReviewer_2)) ? DeptReviewer_2 : "",
                                Dept_Reviewer_3 = (ListNotContains(DeptReviewer_3)) ? DeptReviewer_3 : "",
                                Dept_Reviewer_4 = (ListNotContains(DeptReviewer_4)) ? DeptReviewer_4 : "",
                                Dept_Reviewer_5 = (ListNotContains(DeptReviewer_5)) ? DeptReviewer_5 : "",
                                IT_Reviewer = (ListNotContains(IT_Reviewer1)) ? IT_Reviewer1 : "",
                                HR_Reviewer = (ListNotContains(HR_Reviewer1)) ? HR_Reviewer1 : "",
                                Legal_Reviewer = (ListNotContains(Legal_Reviewer1)) ? Legal_Reviewer1 : "",
                                Ops_Leader = (ListNotContains(Ops_Leader1)) ? Ops_Leader1 : "",
                                Ops_Leader_2 = (ListNotContains(Ops_Leader_2)) ? Ops_Leader_2 : "",
                                Ops_Leader_3 = (ListNotContains(Ops_Leader_3)) ? Ops_Leader_3 : "",
                                Finance_Leader = (ListNotContains(Finance_Leader)) ? Finance_Leader : "",
                                President = (ListNotContains(President)) ? President : "",
                            });


                        }

                    }

                }
                catch (Exception ex)
                {
                    Utility.Logging.LogErrorException(ex, "Get All Company Approvers or SP Context is null");
                }

                return CompanyApprover;
            }
        }

        public List<AR.AppWeb.Models.CorporateApprovers> GetAllCorporateApprovers
        {
            get
            {
                List<Models.WorkFlow> WorkFlows = new List<Models.WorkFlow>();
                var clientContext = _spContext.CreateUserClientContextForSPHost();
                List<SB.AR.AppWeb.Models.CorporateApprovers> CorporateApprover = new List<SB.AR.AppWeb.Models.CorporateApprovers>();
                try
                {
                    if (clientContext != null)
                    {
                        #region  approver

                        List arListapprover = clientContext.Web.Lists.GetByTitle(SPListMeta.Workflow_Tasks);
                        CamlQuery queryapprover = new CamlQuery();

                        queryapprover.ViewXml = string.Format(@"<View>
                                    <Query>
                                       <Where>
                                            <And>
                                                <BeginsWith>
                                                  <FieldRef Name='Title' />
                                                    <Value Type='Text'>AR#{0}</Value>
                                                </BeginsWith>
                                                <Contains>
                                                  <FieldRef Name='Title' />
                                                    <Value Type='Text'>[Corporate]</Value>
                                                </Contains>
                                            </And>
                                        </Where>                              
                                    </Query>
                                </View>", ARID);

                        var arItemsapprover = arListapprover.GetItems(queryapprover);
                        clientContext.Load(arItemsapprover);
                        clientContext.ExecuteQuery();
                        foreach (var itm in arItemsapprover)
                        {
                            FieldUserValue user = null;
                            string Assign = string.Empty;
                            if (itm["AssignedTo"] != null)
                            {
                                FieldUserValue divi = itm["AssignedTo"] as FieldUserValue;
                                Assign = divi.LookupValue;

                                user = new FieldUserValue
                                {
                                    LookupId = divi.LookupId,
                                };
                            }

                            var arId = itm["Title"] != null ? Convert.ToString(itm["Title"]) : "";
                            double AR_ID = 0;
                            if (arId.Trim().StartsWith("AR#"))
                            {
                                arId = arId.Split(' ')[0];
                                arId = arId.Replace(",", "").Replace("AR#", "");
                                var result = double.TryParse(arId, out AR_ID);
                            }

                            DateTime dateApproved = Convert.ToDateTime(itm["Modified"]);
                            var wf = new Models.WorkFlow
                            {
                                PercentComplete = itm["PercentComplete"] != null ? Convert.ToDouble(itm["PercentComplete"]) : 0,
                                Title = itm["Title"] != null ? Convert.ToString(itm["Title"]) : "",
                                Status = itm["Status"] != null ? Convert.ToString(itm["Status"]) : "",
                                ID = itm["ID"] != null ? Convert.ToInt32(itm["ID"]) : 0,
                                Priority = itm["Priority"] != null ? Convert.ToString(itm["Priority"]) : "",
                                DueDate = itm["DueDate"] != null ? Convert.ToDateTime(itm["DueDate"]) : default(DateTime),
                                WorkflowLink = itm["WorkflowLink"] != null ? Convert.ToString(itm["WorkflowLink"]) : "",
                                WorkflowOutcome = itm["WorkflowOutcome"] != null ? Convert.ToString(itm["WorkflowOutcome"]) : "",
                                AssignedTo = Assign,
                                AR_ID = AR_ID,
                                StartDate = itm["StartDate"] != null ? Convert.ToDateTime(itm["StartDate"]) : default(DateTime),
                                DateApproved = itm["Modified"] != null ? (DateTime?)dateApproved : null,

                            };
                            WorkFlows.Add(wf);
                        }

                        #endregion
                        List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.CorporateARAPPROVERS);
                        CamlQuery query = new CamlQuery();

                        query.ViewXml = string.Format(@"
                            <View>
                               <Query>
                                   <ViewFields>
                                      <FieldRef Name='Corp_CEO_Reviewer' />
                                      <FieldRef Name='Corp_CFO_Reviewer' />
                                      <FieldRef Name='Corp_HR_Reviewer' />
                                      <FieldRef Name='Corp_IT_Reviewer' />
                                      <FieldRef Name='Corp_Legal_Reviewer' />
                                      <FieldRef Name='Corp_Ops_Leader_2_Reviewer' />
                                      <FieldRef Name='Corp_Ops_Leader_Reviewer' />                                      
                                   </ViewFields>
                                   <Where>
                                         <Eq>
                                            <FieldRef Name='Division' />
                                            <Value Type='Lookup'>{0}</Value>
                                         </Eq>
                                   </Where>
                                </Query>
                            </View>", DivisionName);

                        var arItems = arList.GetItems(query);
                        clientContext.Load(arItems);
                        clientContext.ExecuteQuery();
                        foreach (var itm in arItems)
                        {
                            #region Checkbox Reviewer

                            string Corp_IT_Reviewer = string.Empty;
                            if (_ar.IT_Review)
                            {
                                if (itm["Corp_IT_Reviewer"] != null)
                                {
                                    FieldUserValue Dept_Reviewer_5_1 = itm["Corp_IT_Reviewer"] as FieldUserValue;
                                    Corp_IT_Reviewer = Dept_Reviewer_5_1.LookupValue;
                                    var wf = WorkFlows.FirstOrDefault(w => w.AssignedTo == Corp_IT_Reviewer);

                                    var corp = new Models.CorporateApprovers
                                    {
                                        Corp_IT_Reviewer = Corp_IT_Reviewer
                                    };
                                    if (wf != null)
                                    {
                                        corp.WorkFlowId = wf.ID;
                                        corp.DateApproved = wf.DateApproved;
                                        corp.DateAssigned = wf.StartDate;
                                        corp.Status = wf.Status;
                                        corp.WorkflowOutcome = wf.WorkflowOutcome;
                                        corp.AssignedTo = wf.AssignedTo;
                                    }

                                    CorporateApprover.Add(corp);
                                }


                            }

                            string Corp_HR_Reviewer = string.Empty;

                            if (_ar.HR_Review)
                            {
                                if (itm["Corp_HR_Reviewer"] != null)
                                {
                                    FieldUserValue DeptReviewer_4_1 = itm["Corp_HR_Reviewer"] as FieldUserValue;
                                    Corp_HR_Reviewer = DeptReviewer_4_1.LookupValue;

                                    var wf = WorkFlows.FirstOrDefault(w => w.AssignedTo == Corp_HR_Reviewer);
                                    var corp = new Models.CorporateApprovers
                                    {
                                        Corp_HR_Reviewer = Corp_HR_Reviewer
                                    };
                                    if (wf != null)
                                    {
                                        corp.WorkFlowId = wf.ID;
                                        corp.DateApproved = wf.DateApproved;
                                        corp.DateAssigned = wf.StartDate;
                                        corp.Status = wf.Status;
                                        corp.WorkflowOutcome = wf.WorkflowOutcome;
                                        corp.AssignedTo = wf.AssignedTo;
                                    }

                                    CorporateApprover.Add(corp);
                                }

                            }

                            string Corp_Legal_Reviewer = string.Empty;

                            if (_ar.Legal_Review)
                            {
                                if (itm["Corp_Legal_Reviewer"] != null)
                                {
                                    FieldUserValue Ops_Leader_1 = itm["Corp_Legal_Reviewer"] as FieldUserValue;
                                    Corp_Legal_Reviewer = Ops_Leader_1.LookupValue;

                                    var wf = WorkFlows.FirstOrDefault(w => w.AssignedTo == Corp_Legal_Reviewer);
                                    var corp = new Models.CorporateApprovers
                                    {
                                        Corp_Legal_Reviewer = Corp_Legal_Reviewer
                                    };
                                    if (wf != null)
                                    {
                                        corp.WorkFlowId = wf.ID;
                                        corp.DateApproved = wf.DateApproved;
                                        corp.DateAssigned = wf.StartDate;
                                        corp.Status = wf.Status;
                                        corp.WorkflowOutcome = wf.WorkflowOutcome;
                                        corp.AssignedTo = wf.AssignedTo;
                                    }

                                    CorporateApprover.Add(corp);
                                }
                            }
                            #endregion

                            string Corp_CEO_Reviewer = string.Empty;
                            if (itm["Corp_CEO_Reviewer"] != null)
                            {
                                FieldUserValue DeptReviewer_2_1 = itm["Corp_CEO_Reviewer"] as FieldUserValue;
                                Corp_CEO_Reviewer = DeptReviewer_2_1.LookupValue;

                                var wf = WorkFlows.FirstOrDefault(w => w.AssignedTo == Corp_CEO_Reviewer);
                                var corp = new Models.CorporateApprovers
                                {
                                    Corp_CEO_Reviewer = Corp_CEO_Reviewer
                                };
                                if (wf != null)
                                {
                                    corp.WorkFlowId = wf.ID;
                                    corp.DateApproved = wf.DateApproved;
                                    corp.DateAssigned = wf.StartDate;
                                    corp.Status = wf.Status;
                                    corp.WorkflowOutcome = wf.WorkflowOutcome;
                                    corp.AssignedTo = wf.AssignedTo;
                                }

                                CorporateApprover.Add(corp);
                            }
                            string Corp_CFO_Reviewer = string.Empty;
                            if (itm["Corp_CFO_Reviewer"] != null)
                            {
                                FieldUserValue DeptReviewer_3_1 = itm["Corp_CFO_Reviewer"] as FieldUserValue;
                                Corp_CFO_Reviewer = DeptReviewer_3_1.LookupValue;

                                var wf = WorkFlows.FirstOrDefault(w => w.AssignedTo == Corp_CFO_Reviewer);
                                var corp = new Models.CorporateApprovers
                                {
                                    Corp_CFO_Reviewer = Corp_CFO_Reviewer
                                };
                                if (wf != null)
                                {
                                    corp.WorkFlowId = wf.ID;
                                    corp.DateApproved = wf.DateApproved;
                                    corp.DateAssigned = wf.StartDate;
                                    corp.Status = wf.Status;
                                    corp.WorkflowOutcome = wf.WorkflowOutcome;
                                    corp.AssignedTo = wf.AssignedTo;
                                }

                                CorporateApprover.Add(corp);
                            }


                            string Corp_Ops_Leader_2_Reviewer = string.Empty;
                            if (itm["Corp_Ops_Leader_2_Reviewer"] != null)
                            {
                                FieldUserValue Ops_Leader_2_1 = itm["Corp_Ops_Leader_2_Reviewer"] as FieldUserValue;
                                Corp_Ops_Leader_2_Reviewer = Ops_Leader_2_1.LookupValue;

                                var wf = WorkFlows.FirstOrDefault(w => w.AssignedTo == Corp_Ops_Leader_2_Reviewer);
                                var corp = new Models.CorporateApprovers
                                {
                                    Corp_Ops_Leader_2_Reviewer = Corp_Ops_Leader_2_Reviewer
                                };
                                if (wf != null)
                                {
                                    corp.WorkFlowId = wf.ID;
                                    corp.DateApproved = wf.DateApproved;
                                    corp.DateAssigned = wf.StartDate;
                                    corp.Status = wf.Status;
                                    corp.WorkflowOutcome = wf.WorkflowOutcome;
                                    corp.AssignedTo = wf.AssignedTo;
                                }

                                CorporateApprover.Add(corp);
                            }
                            string Corp_Ops_Leader_Reviewer = string.Empty;
                            if (itm["Corp_Ops_Leader_Reviewer"] != null)
                            {
                                FieldUserValue Ops_Leader_3_1 = itm["Corp_Ops_Leader_Reviewer"] as FieldUserValue;
                                Corp_Ops_Leader_Reviewer = Ops_Leader_3_1.LookupValue;

                                var wf = WorkFlows.FirstOrDefault(w => w.AssignedTo == Corp_Ops_Leader_Reviewer);
                                var corp = new Models.CorporateApprovers
                                {
                                    Corp_Ops_Leader_Reviewer = Corp_Ops_Leader_Reviewer
                                };
                                if (wf != null)
                                {
                                    corp.WorkFlowId = wf.ID;
                                    corp.DateApproved = wf.DateApproved;
                                    corp.DateAssigned = wf.StartDate;
                                    corp.Status = wf.Status;
                                    corp.WorkflowOutcome = wf.WorkflowOutcome;
                                    corp.AssignedTo = wf.AssignedTo;
                                }

                                CorporateApprover.Add(corp);
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    Utility.Logging.LogErrorException(ex, "Get All Corporate Approvers or SP Context is null");
                }

                return CorporateApprover;
            }
        }
    }
}