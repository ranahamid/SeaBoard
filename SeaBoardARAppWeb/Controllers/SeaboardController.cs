using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Helper;
using SB.AR.AppWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NSBD.SharepointAutoMapper;
using log4net;
using System.Text.RegularExpressions;

namespace SB.AR.AppWeb.Controllers
{

    public class SeaboardController : SBControllerBase
    {

        //
        // GET: /Seaboard/
        //[SharePointContextFilter]
        public ActionResult Index()
        {
                Uri spUrl = null;
                foreach (string key in Request.QueryString.Keys)
                {
                    if (key == "HostUrl" || key == "SPHostUrl")
                    {
                        spUrl = new Uri(Request.QueryString[key]);
                        break;
                    }
                }
                Session["AR"] = null;
               // ClientContext clientContext = TokenHelper.GetS2SClientContextWithWindowsIdentity(spUrl, Request.LogonUserIdentity);

                ClientContext clientContext = SPContext.CreateUserClientContextForSPHost();
                var vm = new DashboardViewModel(SPContext, null);
                vm.MyARs = GetAllMyARs(clientContext);
                vm.MyApprovals = GetAllMyApprovals(clientContext);
                vm.PendingReviews = GetAllPendingReviews(clientContext);
                
                return View(vm);
            
        }


        public ActionResult MainView()
        {
            var arTypeViewModel = new ARTypeViewModel(SPContext);
            var ar = new ARViewModel(SPContext)
            {
                ARTypeViewModel = arTypeViewModel
            };

            if (Request.QueryString["id"] != null)
            {
                int id = Convert.ToInt32(Request["id"]);
                var arMain = GetARById(id);
                ViewBag.FindARID = id;

                if (Request.QueryString["tab"] != null)
                {
                    ViewBag.ActiveTabForEdit = Convert.ToString(Request["tab"]);
                }
                ar.AR = arMain;
                Session["AR"] = arMain;
            }
            else
            {
                ar.AR = new Models.AR();
                Session["AR"] = null;
            }
            return View("MainView", ar);
        }
        /// <summary>
        /// Get All My ARs
        /// </summary>
        /// <param name="clientContext">ClientContext</param>
        /// <returns></returns>
        /// My ARs
        public List<AR.AppWeb.Models.AR> GetAllMyARs(ClientContext clientContext)
        {
            List<Models.AR> myARs = new List<Models.AR>();
           

                if (clientContext != null)
                {
                    List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);
                    CamlQuery query = new CamlQuery();

                    query.ViewXml = string.Format(@"<View>
                                       <Query>
                                          <Where>
                                        
                                                <And>
													<Or>
													
				                                    <Or>
				                                       <Or>
					                                      <Or>
						                                     <Eq>
							                                    <FieldRef Name='Current_Status' />
							                                    <Value Type='Choice'>Not Submitted</Value>
						                                     </Eq>
						                                     <Eq>
							                                    <FieldRef Name='Current_Status' />
							                                    <Value Type='Choice'>Pending Approvals</Value>
						                                     </Eq>
					                                      </Or>
					                                      <Eq>
						                                     <FieldRef Name='Current_Status' />
						                                     <Value Type='Choice'>Pending Edits</Value>
					                                      </Eq>
				                                       </Or>
				                                       <Eq>
						                                     <FieldRef Name='Current_Status' />
						                                     <Value Type='Choice'>Pending</Value>
					                                      </Eq>
				                                    </Or>
                                                   <Eq>
															 <FieldRef Name='Current_Status' />
						                                     <Value Type='Choice'>Rejected</Value>
                                                   </Eq>
												   </Or>
												   
												   
												     <Or>
						                                    <Eq>
															  <FieldRef Name='Author' LookupId='True' />
															  <Value Type='Integer'>
																 <UserID />
															  </Value>
															</Eq>
															


                                                        <Eq>
                                                    <FieldRef Name='PM_x002F_Owner' LookupId='True' />
                                                    <Value Type='Integer'>
                                                        <UserID />
                                                    </Value>
                                                </Eq>


					                                 </Or>
												   
                                                </And>
                                              
                                          </Where>
                                          <OrderBy>
                                             <FieldRef Name='AR_ID' Ascending='FALSE' />
                                          </OrderBy>
                                       </Query>
                                    </View>");
                    var arItems = arList.GetItems(query);
                    clientContext.Load(arItems);
                    clientContext.ExecuteQuery();
                    foreach (var itm in arItems)
                    {
                        LookupFieldMapper division = null;
                        if (itm["Division"] != null)
                        {
                            FieldLookupValue divi = itm["Division"] as FieldLookupValue;
                            division = new LookupFieldMapper
                            {
                                ID = divi.LookupId,
                                Value = divi.LookupValue
                            };
                        }
                        LookupFieldMapper cmp = null;
                        if (itm["Company_Name"] != null)
                        {
                            FieldLookupValue comp = itm["Company_Name"] as FieldLookupValue;
                            cmp = new LookupFieldMapper
                            {
                                ID = comp.LookupId,
                                Value = comp.LookupValue
                            };
                        }
                        myARs.Add(new Models.AR
                        {

                            AR_ID = itm["AR_ID"] != null ? Convert.ToDouble(itm["AR_ID"]) : 0,
                            AR_Type = itm["AR_Type"] != null ? Convert.ToString(itm["AR_Type"]) : "",
                            Title = itm["Title"] != null ? Convert.ToString(itm["Title"]) : "",
                            ID = itm["ID"] != null ? Convert.ToInt32(itm["ID"]) : 0,
                            Division = division,
                            Company_Name = cmp,

                            Total_Cost = itm["Total_Cost"] != null ? Convert.ToDouble(itm["Total_Cost"]) : 0,
                            Current_Status = itm["Current_Status"] != null ? Convert.ToString(itm["Current_Status"]) : "",
                        });
                    }

                }

          
            return myARs;
        }
        /// <summary>
        /// Get All My Approvals
        /// </summary>
        /// <param name="clientContext"></param>
        /// <returns></returns>
        public List<AR.AppWeb.Models.WorkFlow> GetAllMyApprovals(ClientContext clientContext)
        {

            List<Models.WorkFlow> WorkFlows = new List<Models.WorkFlow>();
           
                if (clientContext != null)
                {
                    // int loggedInUser = CurrentUser.Id;

                    List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.Workflow_Tasks);
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = string.Format(@"<View>
                                    <Query>
                                          <Where>
                                              <And>
                                                 <Eq>
                                                    <FieldRef Name='AssignedTo' />
                                                    <Value Type='Integer'>
                                                       <UserID />
                                                    </Value>
                                                 </Eq>
                                                 <Neq>
                                                    <FieldRef Name='Status' />
                                                    <Value Type='Choice'>Completed</Value>
                                                 </Neq>
                                              </And>
                                           </Where>                               
                                    </Query>
                                </View>");
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
                        double ARID = 0;
                        if (arId.Trim().StartsWith("AR#"))
                        {
                            try
                            {
                                arId = arId.Split('[')[0];
                                arId = arId.Trim().Replace(",", "").Replace("AR#", "");

                            }catch(Exception ex)
                            {
                                arId = arId.Split(' ')[0];
                                arId = arId.Trim().Replace(",", "").Replace("AR#", "");
                            }
                           
                            var result = double.TryParse(arId, out ARID);
                            if(!result)
                            {
                                var resultString = Regex.Match(arId, @"\d+");
                                if (resultString != null)
                                {
                                    result = double.TryParse(resultString.Value, out ARID);
                                }
                            }
                        }

                        WorkFlows.Add(new Models.WorkFlow
                        {

                            PercentComplete = itm["PercentComplete"] != null ? Convert.ToDouble(itm["PercentComplete"]) : 0,
                            Title = itm["Title"] != null ? Convert.ToString(itm["Title"]) : "",
                            Status = itm["Status"] != null ? Convert.ToString(itm["Status"]) : "",
                            ID = itm["ID"] != null ? Convert.ToInt32(itm["ID"]) : 0,
                            Priority = itm["Priority"] != null ? Convert.ToString(itm["Priority"]) : "",
                            DueDate = itm["DueDate"] != null ? Convert.ToDateTime(itm["DueDate"]) : DateTime.Now,
                            WorkflowLink = itm["WorkflowLink"] != null ? Convert.ToString(itm["WorkflowLink"]) : "",
                            WorkflowOutcome = itm["WorkflowOutcome"] != null ? Convert.ToString(itm["WorkflowOutcome"]) : "",
                            AssignedTo = Assign,
                            AR_ID = ARID
                        });
                    }


                }

            
            return WorkFlows;
        }



        /// <summary>
        /// Get All Pending Reviews
        /// </summary>
        /// <param name="clientContext">ClientContext</param>
        /// <returns></returns>
        public List<AR.AppWeb.Models.AR> GetAllPendingReviews(ClientContext clientContext)
        {
            List<Models.AR> myARs = new List<Models.AR>();
           
                if (clientContext != null)
                {
                    List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);
                    CamlQuery query = new CamlQuery();
                    //int loggedInUser = CurrentUser.Id;

                    //var query = new CamlBuilder().Where().LookupField("Current_Status").ValueAsText().EqualTo("Pending Edits").And().UserField("PM_x002F_Owner").EqualToCurrentUser().OrderBy("AR_ID").ToString();

                    query.ViewXml = string.Format(@"
                            <View>
                               <Query>
                              <Where>
                                  <And>
                                     <And>
                                        <And>
                                           <IsNotNull>
                                              <FieldRef Name='PM_x002F_Owner' />
                                           </IsNotNull>
                                          		<Eq>
                                                    <FieldRef Name='PM_x002F_Owner' LookupId='True' />
                                                    <Value Type='Integer'>
                                                        <UserID />
                                                    </Value>
                                                </Eq>
                                        </And>
                                        <IsNotNull>
                                           <FieldRef Name='Current_Status' />
                                        </IsNotNull>
                                     </And>
                                     <Eq>
                                        <FieldRef Name='Current_Status' />
                                        <Value Type='Choice'>Pending Edits</Value>
                                     </Eq>
                                  </And>
                               </Where>
  
                            </Query>

                            </View>");

                    var arItems = arList.GetItems(query);
                    clientContext.Load(arItems);
                    clientContext.ExecuteQuery();
                    foreach (var itm in arItems)
                    {
                        LookupFieldMapper division = null;
                        if (itm["Division"] != null)
                        {
                            FieldLookupValue divi = itm["Division"] as FieldLookupValue;
                            division = new LookupFieldMapper
                            {
                                ID = divi.LookupId,
                                Value = divi.LookupValue
                            };
                        }
                        LookupFieldMapper cmp = null;
                        if (itm["Company_Name"] != null)
                        {
                            FieldLookupValue comp = itm["Company_Name"] as FieldLookupValue;
                            cmp = new LookupFieldMapper
                            {
                                ID = comp.LookupId,
                                Value = comp.LookupValue
                            };
                        }

                        myARs.Add(new Models.AR
                        {

                            AR_ID = itm["AR_ID"] != null ? Convert.ToDouble(itm["AR_ID"]) : 0,
                            ID = itm["ID"] != null ? Convert.ToInt32(itm["ID"]) : 0,
                            AR_Type = itm["AR_Type"] != null ? Convert.ToString(itm["AR_Type"]) : "",
                            Title = itm["Title"] != null ? Convert.ToString(itm["Title"]) : "",

                            Division = division,
                            Company_Name = cmp,

                            Total_Cost = itm["Total_Cost"] != null ? Convert.ToDouble(itm["Total_Cost"]) : 0,
                            Current_Status = itm["Current_Status"] != null ? Convert.ToString(itm["Current_Status"]) : "",
                        });
                    }
                }
           
            return myARs;
        }

       
       

   
    }

}