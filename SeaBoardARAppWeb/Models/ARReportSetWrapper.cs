using SB.AR.AppWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace SB.AR.AppWeb.Models
{
    public class ARReportSetWrapper
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ReportObjectAR reportObjectAR { get; set; }

        public ReportObjectAR GetAR()
        {
            reportObjectAR = new ReportObjectAR();

             


            // Fill report object from AR

            if (HttpContext.Current.Session["AR"] != null)
            {
                //logger.Info("Start filling ReportObjectAR object ARReportSetWrapper-GetAR()");

                AR ar = HttpContext.Current.Session["AR"] as AR;

                // MAIN
                reportObjectAR.Title = ar.Title;
                reportObjectAR.LocalAR = ar.LocalAR;
                reportObjectAR.ARNumber = ar.AR_ID.ToString(); //ar.ARNumber;
                reportObjectAR.Created = ar.Created;

                reportObjectAR.Division = ar.Division;
                reportObjectAR.Company_Name = ar.Company_Name;
                reportObjectAR.Category = ar.Category;

                reportObjectAR.Engineering_Review = ar.Engineering_Review;
                reportObjectAR.HR_Review = ar.HR_Review;
                reportObjectAR.IT_Review = ar.IT_Review;
                reportObjectAR.Legal_Review = ar.Legal_Review;

                reportObjectAR.Location = ar.Location;

                if (ar.PMUser != null && ar.PMUser.LookupValue != null)
                {
                    reportObjectAR.PMOwner = ar.PMUser.LookupValue; //ar.PMOwner;
                }

                reportObjectAR.Project_Start = ar.Project_Start;
                reportObjectAR.Project_End = ar.Project_End;


                // NARRATIVE
                reportObjectAR.AR_ID = ar.AR_ID;
                reportObjectAR.AR_Type = ar.AR_Type;
                reportObjectAR.Total_Cost = ar.Total_Cost;
                reportObjectAR.Current_Status = ar.Current_Status;

                reportObjectAR.PresentSituationIssue = ar.PresentSituationIssue;
                reportObjectAR.Proposed_Solution = ar.Proposed_Solution;
                reportObjectAR.Other_Potential_Solutions = ar.Other_Potential_Solutions;
                reportObjectAR.Explanation_of_Costs = ar.Explanation_of_Costs;
                reportObjectAR.Financial_Measures = ar.Financial_Measures;


                // FINANCIAL
                reportObjectAR.Purchase_Price = ar.Purchase_Price;
                reportObjectAR.TaxVAT = ar.TaxVAT;
                reportObjectAR.ServicesInstallation = ar.ServicesInstallation;
                reportObjectAR.Other_Cost_or_Savings = ar.Other_Cost_or_Savings;
                reportObjectAR.InvestmentType = ar.InvestmentType;
                reportObjectAR.Economical_Life = ar.Economical_Life;

                reportObjectAR.Total_Cost = ar.Total_Cost;
                reportObjectAR.Budget_Amount = ar.Budget_Amount;
                reportObjectAR.BudgetLineItem = ar.BudgetLineItem;
                reportObjectAR.In_Budget = ar.In_Budget;
                reportObjectAR.Funds_Committed = ar.Funds_Committed;

                // LEASE
                reportObjectAR.LengthOfLease = ar.LengthOfLease;
                reportObjectAR.Cost_Per_Period = ar.Cost_Per_Period;
                reportObjectAR.Other_Cost_or_Savings = ar.Other_Cost_or_Savings;
                reportObjectAR.Current_Year_Cost_Commitment = ar.Current_Year_Cost_Commitment;
                reportObjectAR.LeaseType = ar.LeaseType;
                // Disposal
                reportObjectAR.Condition_of_Assets = ar.Condition_of_Assets;
                reportObjectAR.Original_Cost = ar.Original_Cost;
                reportObjectAR.Market_Value = ar.Market_Value;
                reportObjectAR.Current_Net_Book_Value = ar.Current_Net_Book_Value;
                reportObjectAR.Expected_Proceeds = ar.Expected_Proceeds;

                reportObjectAR.Equity_Description = ar.Equity_Description;
                reportObjectAR.Date_Aquired = ar.Date_Aquired;
                reportObjectAR.Department_to_Charge = ar.Department_to_Charge;

                reportObjectAR.Stated_Interest_Rate = ar.Stated_Interest_Rate;
                reportObjectAR.Maturity_Date = ar.Maturity_Date;
                reportObjectAR.Lender = ar.Lender;
                reportObjectAR.Borrower = ar.Borrower;
                reportObjectAR.Currency_Name = ar.Currency_Name;

                reportObjectAR.Price_Per_Share_Being_Purchased = ar.Price_Per_Share_Being_Purchased;
                reportObjectAR.Number_of_Share_to_Aquire = ar.Number_of_Share_to_Aquire;
                reportObjectAR.Total_Other_Costs = ar.Total_Other_Costs;
                reportObjectAR.Percentage_of_Ownership_at_Close = ar.Percentage_of_Ownership_at_Close;
                reportObjectAR.Transaction_Close_Date = ar.Transaction_Close_Date;
                reportObjectAR.ConsolidatedNonCon = ar.ConsolidatedNonCon;

                reportObjectAR.OriginalAR = ar.OriginalAR;
                reportObjectAR.AdditionaAmountFundsRequired = ar.AdditionaAmountFundsRequired;
                reportObjectAR.Original_AR_Amount = ar.Original_AR_Amount;
            }

            //logger.Info("End filling ReportObjectAR object ARReportSetWrapper-GetAR()");




            return reportObjectAR;
        }
    }

    /// <summary>
    /// Core object that encapsulates AR and flattens it as required by RDLC for some properties
    /// </summary>
    public class ReportObjectAR : SB.AR.AppWeb.Models.AR
    {
        public ReportObjectAR()
        { }

        public string DivisionString
        {
            get
            {
                if (this.Division == null)
                    return string.Empty;
                else
                    return this.Division.Value;
            }
        }

        public string CompanyString
        {
            get
            {
                if (this.Company_Name == null)
                    return string.Empty;
                else
                    return this.Company_Name.Value;

            }
        }
        public string CategoryString
        {
            get
            {
                if (this.Category == null)
                    return string.Empty;
                else
                    return this.Category.Value;
            }
        }

        public string PresentSituationIssueNOHTML
        {
            get
            {
                if (!string.IsNullOrEmpty(PresentSituationIssue))
                {
                    var text = Regex.Replace(PresentSituationIssue, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return string.Empty;
            }
        }

        public string Proposed_SolutionNOHTML
        {
            get
            {
                if (!string.IsNullOrEmpty(Proposed_Solution))
                {
                    var text = Regex.Replace(Proposed_Solution, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return string.Empty;
            }
        }

        public string Other_Potential_SolutionsNOHTML
        {
            get
            {
                if (!string.IsNullOrEmpty(Other_Potential_Solutions))
                {
                    var text = Regex.Replace(Other_Potential_Solutions, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return string.Empty;
            }
        }

        public string Explanation_of_CostsNOHTML
        {
            get
            {
                if (!string.IsNullOrEmpty(Explanation_of_Costs))
                {
                    var text = Regex.Replace(Explanation_of_Costs, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return string.Empty;
            }
        }

        public string Financial_MeasuresNOHTML
        {
            get
            {
                if (!string.IsNullOrEmpty(Financial_Measures))
                {
                    var text = Regex.Replace(Financial_Measures, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return string.Empty;
            }
        }
    }

    public class ReportAttachment
    {
        public string FileName { get; set; }
    }

    // todo: replace with actual code during integration for attachments and discussion TABs
    public class ReportAttachments
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<ReportAttachment> _files;

        public ReportAttachments()
        {
            _files = new List<ReportAttachment>();

            try
            {
                //logger.Info("Start filling ReportAttachments ReportAttachments-ReportAttachments()");
                 
                AR ar = HttpContext.Current.Session["AR"] as AR;

                SharePointContext spContext = (SharePointContext)HttpContext.Current.Session["spContext"];
                ARViewModel arvm = new ARViewModel(spContext);
                ARAttachmentsViewModel attachmentvm = new ARAttachmentsViewModel(ar.Attachment_Folder_Id);

                if (attachmentvm != null && attachmentvm.attachments != null)
                {
                    foreach (Models.ARAttachments thisFile in attachmentvm.attachments)
                    {
                        _files.Add(new ReportAttachment() { FileName = thisFile.FileName });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error filling ReportAttachments ReportAttachments-ReportAttachments()", ex);
            }

            //logger.Info("Stop filling ReportAttachments ReportAttachments-ReportAttachments()");
            
        }

        public List<ReportAttachment> GetAttachments()
        {
            return _files;
        }

    }

    public class ReportDiscussionEntry
    {
        public int RowNumber { set; get; }
        public string Messsage { set; get; }
        public bool AllApprovers { set; get; }
        public bool ProjectManagers { set; get; }
        public bool Orignator { set; get; }
        public bool Public { set; get; }
        public DateTime Created { get; set; }
        public string FromUser { get; set; }
        public string ToUser { get; set; }
    }

    public class ReportDiscussions
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<ReportDiscussionEntry> _discussionEntries;

        public ReportDiscussions()
        {

            _discussionEntries = new List<ReportDiscussionEntry>();

            try
            {
                //logger.Info("Start filling ReportAttachments ReportDiscussions-ReportDiscussions()");

                SharePointContext spContext = (SharePointContext)HttpContext.Current.Session["spContext"];
                AR ar = HttpContext.Current.Session["AR"] as AR;
                ARDiscussionViewModel discussionVM = new ViewModels.ARDiscussionViewModel(spContext, ar);

                var allDiscussiosn = discussionVM.GetARDiscussion();
                int row = 1;

                foreach (ARDiscussions thisDiscussion in allDiscussiosn)
                {
                    string toAddressString = string.Empty;

                    if (thisDiscussion.ToAddress != null && thisDiscussion.ToAddress.Count() > 0)
                    {
                        toAddressString = String.Join(",", thisDiscussion.ToAddress.Select(p => p.Name).ToList());
                    }

                    _discussionEntries.Add(new ReportDiscussionEntry() { RowNumber = row++, Messsage = thisDiscussion.Messsage, 
                        FromUser = thisDiscussion.From.Name, ToUser = toAddressString, Created = thisDiscussion.Created });
                }

            }
            catch (Exception ex)
            {
                logger.Error("Error filling ReportAttachments ReportDiscussions-ReportDiscussions()", ex);
            }

            //logger.Info("Stop filling ReportAttachments ReportDiscussions-ReportDiscussions()");
        }

        public List<ReportDiscussionEntry> GetDiscussionEntries()
        {
            return _discussionEntries;
        }
    }
}