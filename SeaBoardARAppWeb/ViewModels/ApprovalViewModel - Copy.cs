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
                   return ((int)_ar.AR_ID).ToString("N").Replace(".00","");
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
        List<string> CorpApproverFirst;
        Dictionary<string, string> ApproverDic = new Dictionary<string, string>();


        
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

                               
                                ApproverDic.Add("Dept_Reviewer_1", DeptReviewer_1);
                                ApproverDic.Add("Dept_Reviewer_2", DeptReviewer_2);
                                ApproverDic.Add("Dept_Reviewer_3", DeptReviewer_3);
                                ApproverDic.Add("Dept_Reviewer_4", DeptReviewer_4);
                                ApproverDic.Add("Dept_Reviewer_5", DeptReviewer_5);
                                ApproverDic.Add("IT_Reviewer", IT_Reviewer1);
                                ApproverDic.Add("HR_Reviewer", HR_Reviewer1);
                                ApproverDic.Add("Legal_Reviewer", Legal_Reviewer1);
                                ApproverDic.Add("Ops_Leader", Ops_Leader1);
                                ApproverDic.Add("Ops_Leader_2", Ops_Leader_2);
                                ApproverDic.Add("Ops_Leader_3", Ops_Leader_3);
                                ApproverDic.Add("Finance_Leader", Finance_Leader);
                                ApproverDic.Add("President", President);
                            }

                            #endregion
                            #region corporateApprover

//                            List arList2query232 = clientContext.Web.Lists.GetByTitle(SPListMeta.CorporateARAPPROVERS);
//                            CamlQuery query232 = new CamlQuery();

//                            query232.ViewXml = string.Format(@"
//                            <View>
//                               <Query>
//                                   <ViewFields>
//                                      <FieldRef Name='Corp_CEO_Reviewer' />
//                                      <FieldRef Name='Corp_CFO_Reviewer' />
//                                      <FieldRef Name='Corp_HR_Reviewer' />
//                                      <FieldRef Name='Corp_IT_Reviewer' />
//                                      <FieldRef Name='Corp_Legal_Reviewer' />
//                                      <FieldRef Name='Corp_Ops_Leader_2_Reviewer' />
//                                      <FieldRef Name='Corp_Ops_Leader_Reviewer' />
//                                      
//                                   </ViewFields>
//                                   <Where>
//                                      
//                                         <Eq>
//                                            <FieldRef Name='Division' />
//                                            <Value Type='Lookup'>{0}</Value>
//                                         </Eq>
//                                        
//                                   </Where>
//  
//                                </Query>
//
//                            </View>", DivisionName);

//                            var arItems2 = arList2query232.GetItems(query232);
//                            clientContext.Load(arItems2);
//                            clientContext.ExecuteQuery();
//                            foreach (var itm34 in arItems2)
//                            {
//                                string Corp_CEO_Reviewer = string.Empty;
//                                if (itm34["Corp_CEO_Reviewer"] != null)
//                                {
//                                    FieldUserValue DeptReviewer_2_1_Corp = itm34["Corp_CEO_Reviewer"] as FieldUserValue;
//                                    Corp_CEO_Reviewer = DeptReviewer_2_1_Corp.LookupValue;
//                                }
//                                string Corp_CFO_Reviewer = string.Empty;
//                                if (itm34["Corp_CFO_Reviewer"] != null)
//                                {
//                                    FieldUserValue DeptReviewer_3_1_Corp = itm34["Corp_CFO_Reviewer"] as FieldUserValue;
//                                    Corp_CFO_Reviewer = DeptReviewer_3_1_Corp.LookupValue;
//                                }
//                                string Corp_HR_Reviewer = string.Empty;
//                                if (itm34["Corp_HR_Reviewer"] != null)
//                                {
//                                    FieldUserValue DeptReviewer_4_1_Corp = itm34["Corp_HR_Reviewer"] as FieldUserValue;
//                                    Corp_HR_Reviewer = DeptReviewer_4_1_Corp.LookupValue;
//                                }
//                                string Corp_IT_Reviewer = string.Empty;
//                                if (itm34["Corp_IT_Reviewer"] != null)
//                                {
//                                    FieldUserValue Dept_Reviewer_5_1_Corp = itm34["Corp_IT_Reviewer"] as FieldUserValue;
//                                    Corp_IT_Reviewer = Dept_Reviewer_5_1_Corp.LookupValue;
//                                }

//                                string Corp_Legal_Reviewer = string.Empty;
//                                if (itm34["Corp_Legal_Reviewer"] != null)
//                                {
//                                    FieldUserValue Ops_Leader_1_Corp = itm34["Corp_Legal_Reviewer"] as FieldUserValue;
//                                    Corp_Legal_Reviewer = Ops_Leader_1_Corp.LookupValue;
//                                }
//                                string Corp_Ops_Leader_2_Reviewer = string.Empty;
//                                if (itm34["Corp_Ops_Leader_2_Reviewer"] != null)
//                                {
//                                    FieldUserValue Ops_Leader_2_1_Corp = itm34["Corp_Ops_Leader_2_Reviewer"] as FieldUserValue;
//                                    Corp_Ops_Leader_2_Reviewer = Ops_Leader_2_1_Corp.LookupValue;
//                                }
//                                string Corp_Ops_Leader_Reviewer = string.Empty;
//                                if (itm34["Corp_Ops_Leader_Reviewer"] != null)
//                                {
//                                    FieldUserValue Ops_Leader_3_1_Corp = itm34["Corp_Ops_Leader_Reviewer"] as FieldUserValue;
//                                    Corp_Ops_Leader_Reviewer = Ops_Leader_3_1_Corp.LookupValue;
//                                }
                            

//                                ApproverDic.Add("Corp_CEO_Reviewer", Corp_CEO_Reviewer);
//                                ApproverDic.Add("Corp_CFO_Reviewer", Corp_CFO_Reviewer);
//                                ApproverDic.Add("Corp_HR_Reviewer", Corp_HR_Reviewer);
//                                ApproverDic.Add("Corp_IT_Reviewer", Corp_IT_Reviewer);
//                                ApproverDic.Add("Corp_Legal_Reviewer", Corp_Legal_Reviewer);
//                                ApproverDic.Add("Corp_Ops_Leader_2_Reviewer", Corp_Ops_Leader_2_Reviewer);
//                                ApproverDic.Add("Corp_Ops_Leader_Reviewer", Corp_Ops_Leader_Reviewer);
//                            }
                            #endregion
                            List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.Workflow_Tasks);
                            CamlQuery query = new CamlQuery();

                            query.ViewXml = string.Format(@"<View>
                                    <Query>
                                       <Where>
                                            <BeginsWith>
                                              <FieldRef Name='Title' />
                                                <Value Type='Text'>AR#{0}</Value>
                                            </BeginsWith>
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
                                    arId = arId.Replace(",", "").Replace("AR#", "");
                                    var result = double.TryParse(arId, out AR_ID);
                                }

                                DateTime dateApproved = Convert.ToDateTime(itm["Modified"]);

                                var myKey = ApproverDic.FirstOrDefault(x => x.Value == Assign).Key;
                                ApproverDic.Remove(myKey);

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
        public bool CorpListNotContains(string approver)
        {
            if (CorpApproverFirst.Contains(approver))
            {
                CorpApproverFirst.Remove(approver);
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<AR.AppWeb.Models.WorkFlow> CorpAllApprovals
        {
            get
            {
                List<Models.WorkFlow> WorkFlows = new List<Models.WorkFlow>();
                try
                {
                    using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                    {
                        if (clientContext != null)
                        {


                            #region Corp
                            var corpApprovers = GetAllCorporateApprovers;
                            foreach (var c in corpApprovers)
                            {
                                ApproverDic.Add("Corp_CEO_Reviewer", c.Corp_CEO_Reviewer);
                                ApproverDic.Add("Corp_CFO_Reviewer", c.Corp_CFO_Reviewer);
                                ApproverDic.Add("Corp_HR_Reviewer", c.Corp_HR_Reviewer);
                                ApproverDic.Add("Corp_IT_Reviewer", c.Corp_IT_Reviewer);
                                ApproverDic.Add("Corp_Legal_Reviewer", c.Corp_Legal_Reviewer);
                                ApproverDic.Add("Corp_Ops_Leader_2_Reviewer", c.Corp_Ops_Leader_2_Reviewer);
                                ApproverDic.Add("Corp_Ops_Leader_Reviewer", c.Corp_Ops_Leader_Reviewer);
                            }
                            #endregion

                            List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.Workflow_Tasks);
                            CamlQuery query = new CamlQuery();

                            query.ViewXml = string.Format(@"<View>
                                    <Query>
                                       <Where>
                                            <BeginsWith>
                                              <FieldRef Name='Title' />
                                                <Value Type='Text'>AR#{0}</Value>
                                            </BeginsWith>
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
                                    arId = arId.Replace(",", "").Replace("AR#", "");
                                    var result = double.TryParse(arId, out AR_ID);
                                }

                                DateTime dateApproved = Convert.ToDateTime(itm["Modified"]);

                                var myKey = ApproverDic.FirstOrDefault(x => x.Value == Assign).Key;
                                ApproverDic.Remove(myKey);

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
        public List<AR.AppWeb.Models.CompanyApprover> GetAllCompanyApprovers
        {
            get
            {
                var clientContext = _spContext.CreateUserClientContextForSPHost();
                List<SB.AR.AppWeb.Models.CompanyApprover> CompanyApprover  = new List<SB.AR.AppWeb.Models.CompanyApprover>();
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

                            </View>",DivisionName, CompanyName);

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

                                Dept_Reviewer_1 = (ListNotContains(DeptReviewer_1))?DeptReviewer_1:"",
                                Dept_Reviewer_2 =(ListNotContains(DeptReviewer_2))?DeptReviewer_2:"",
                                Dept_Reviewer_3 =(ListNotContains(DeptReviewer_3))?DeptReviewer_3:"",
                                Dept_Reviewer_4 =(ListNotContains(DeptReviewer_4))?DeptReviewer_4:"",
                                Dept_Reviewer_5 =(ListNotContains(DeptReviewer_5))?DeptReviewer_5:"",
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
                var clientContext = _spContext.CreateUserClientContextForSPHost();
                List<SB.AR.AppWeb.Models.CorporateApprovers> CorporateApprover = new List<SB.AR.AppWeb.Models.CorporateApprovers>();
                try
                {
                    if (clientContext != null)
                    {
                        #region  approver
                        CorpApproverFirst = new List<string>();
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
                            CorpApproverFirst.Add(Assign);
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
                                }
                            }
                           
                            string Corp_HR_Reviewer = string.Empty;
                           
                            if (_ar.HR_Review)
                            {
                                if (itm["Corp_HR_Reviewer"] != null)
                                {
                                    FieldUserValue DeptReviewer_4_1 = itm["Corp_HR_Reviewer"] as FieldUserValue;
                                    Corp_HR_Reviewer = DeptReviewer_4_1.LookupValue;
                                }
                           
                            }                            

                            string Corp_Legal_Reviewer = string.Empty;
                           
                            if (_ar.Legal_Review)
                            {
                                if (itm["Corp_Legal_Reviewer"] != null)
                                {
                                    FieldUserValue Ops_Leader_1 = itm["Corp_Legal_Reviewer"] as FieldUserValue;
                                    Corp_Legal_Reviewer = Ops_Leader_1.LookupValue;
                                }
                            }
                            #endregion

                            string Corp_CEO_Reviewer = string.Empty;
                            if (itm["Corp_CEO_Reviewer"] != null)
                            {
                                FieldUserValue DeptReviewer_2_1 = itm["Corp_CEO_Reviewer"] as FieldUserValue;
                                Corp_CEO_Reviewer = DeptReviewer_2_1.LookupValue;
                            }
                            string Corp_CFO_Reviewer = string.Empty;
                            if (itm["Corp_CFO_Reviewer"] != null)
                            {
                                FieldUserValue DeptReviewer_3_1 = itm["Corp_CFO_Reviewer"] as FieldUserValue;
                                Corp_CFO_Reviewer = DeptReviewer_3_1.LookupValue;
                            }
                            
                           
                            string Corp_Ops_Leader_2_Reviewer = string.Empty;
                            if (itm["Corp_Ops_Leader_2_Reviewer"] != null)
                            {
                                FieldUserValue Ops_Leader_2_1 = itm["Corp_Ops_Leader_2_Reviewer"] as FieldUserValue;
                                Corp_Ops_Leader_2_Reviewer = Ops_Leader_2_1.LookupValue;
                            }
                            string Corp_Ops_Leader_Reviewer = string.Empty;
                            if (itm["Corp_Ops_Leader_Reviewer"] != null)
                            {
                                FieldUserValue Ops_Leader_3_1 = itm["Corp_Ops_Leader_Reviewer"] as FieldUserValue;
                                Corp_Ops_Leader_Reviewer = Ops_Leader_3_1.LookupValue;
                            }

                            CorporateApprover.Add(new Models.CorporateApprovers
                            {
                                Corp_CEO_Reviewer = (CorpListNotContains(Corp_CEO_Reviewer)) ? Corp_CEO_Reviewer : "",
                                Corp_CFO_Reviewer = (CorpListNotContains(Corp_CFO_Reviewer)) ? Corp_CFO_Reviewer : "",
                                Corp_HR_Reviewer = (CorpListNotContains(Corp_HR_Reviewer)) ? Corp_HR_Reviewer : "",
                                Corp_IT_Reviewer = (CorpListNotContains(Corp_IT_Reviewer)) ? Corp_IT_Reviewer : "",
                                Corp_Legal_Reviewer = (CorpListNotContains(Corp_Legal_Reviewer)) ? Corp_Legal_Reviewer : "",
                                Corp_Ops_Leader_2_Reviewer = (CorpListNotContains(Corp_Ops_Leader_2_Reviewer)) ? Corp_Ops_Leader_2_Reviewer : "",
                                Corp_Ops_Leader_Reviewer = (CorpListNotContains(Corp_Ops_Leader_Reviewer)) ? Corp_Ops_Leader_Reviewer : "",
                            });
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