using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Models.Interface;
using System.ComponentModel;
using SB.AR.AppWeb.Helper;

namespace SB.AR.AppWeb.Models
{
    [SharepointListName("AR")]
    public class AR : IEntitySharepointMapper
    {

        [SharepointFieldName("Original_AR_Amount")]
        public double Original_AR_Amount { get; set; }

        [SharepointFieldName("Amount_of_Additional_Funds_Requi"), DisplayName("Amount_of_Additional_Funds_Requi")]
        public double AdditionaAmountFundsRequired { get; set; }

        [SharepointFieldName("Amount_of_Funding_Required_from_"), DisplayName("Amount_of_Funding_Required_from_")]
        public double AmountOfFundingRequiredFrom { get; set; }

        [SharepointFieldName("Author"), DisplayName("Author")]
        public LookupFieldMapper Author { get; set; }

        [SharepointFieldName("AppAuthor"), DisplayName("App Created By")]
        public LookupFieldMapper AppAuthor { get; set; }

        [DisplayName("App Modified By")]
        public LookupFieldMapper AppEditor { get; set; }

        [SharepointFieldName("Title"), DisplayName("AR Title")]
        [Required]
        [UsedIn(ControllerName = SB.AR.AppWeb.Helper.Tabs.MAIN, FieldName="AR Title")]
        public string   Title { get; set; }

        [SharepointFieldName("AR__x0023_"), DisplayName("AR_#")]
        public string ARNumber { get; set; }

        [SharepointFieldName("AR_ID"), DisplayName("AR_ID")]
        public double? AR_ID { get; set; }

        [Required(ErrorMessage = "Please enter AR Type")]
        [SharepointFieldName("AR_Type")]
        [UsedIn(ControllerName=Tabs.MAIN, FieldName="AR Type")]
        public string AR_Type { get; set; }

        [SharepointFieldName("ApprovalStatus")]
        public int ApprovalStatus { get; set; }        

        [SharepointFieldName("Asset_Location")]
        public string Asset_Location { get; set; }

        [Required]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "Borrower", AssociateTab = "Loans and Advances")]
        [SharepointFieldName("Borrower")]
        public string Borrower { get; set; }

        [SharepointFieldName("Budget_Amount")]
        public double Budget_Amount { get; set; }

        [SharepointFieldName("Budget_Line_Item__x0023_")]
        public string BudgetLineItem { get; set; }

        [SharepointFieldName("Category")]
        public LookupFieldMapper Category { get; set; }

        
        public int? CategoryId { get; set; }

        public LookupFieldMapper SyncClientId { get; set; }

        [SharepointFieldName("Company_Name")]
        public LookupFieldMapper Company_Name { get; set; }


        [Required]
        [UsedIn(ControllerName = SB.AR.AppWeb.Helper.Tabs.MAIN, FieldName = "Company")]
        public int? CompanyId { get; set; }


        [SharepointFieldName("Division")]
        public LookupFieldMapper Division { get; set; }

        [Required]
        [UsedIn(ControllerName = SB.AR.AppWeb.Helper.Tabs.MAIN, FieldName = "Division")]
        public int? DivisionId { get; set; }

        [Required]
        [SharepointFieldName("Condition_of_Assets")]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "Condition of Assets", AssociateTab = "Disposal,Purchase with Disposal,Lease With Disposal")]
        public string Condition_of_Assets { get; set; }

        [SharepointFieldName("Consolidated_x002F_Non_x002d_Con")]
        public string ConsolidatedNonCon { get; set; }


        
        [SharepointFieldName("Cost_Per_Period")]
        [Required]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "Cost Per Period", AssociateTab = "Lease,Lease With Disposal")]
        public double Cost_Per_Period { get; set; }

        [SharepointFieldName("Created")]
        public DateTime? Created { get; set; }
        
        [SharepointFieldName("Currency_Name")]
        public string Currency_Name { get; set; }

        [SharepointFieldName("Current_Approver")]
        public string Current_Approver { get; set; }


        [SharepointFieldName("Current_Net_Book_Value")]
        public double Current_Net_Book_Value { get; set; }

        [SharepointFieldName("Current_Status")]
        public string Current_Status { get; set; }

        [SharepointFieldName("Current_Year_Cost_Commitment")]
        public double Current_Year_Cost_Commitment { get; set; }

        [SharepointFieldName("Date_Aquired")]
        public DateTime? Date_Aquired { get; set; }


        [SharepointFieldName("Date_Assigned")]
        public DateTime? Date_Assigned { get; set; }

        [SharepointFieldName("Department_to_Charge")]
        public string Department_to_Charge { get; set; }

       
        
        [SharepointFieldName("Economical_Life")]
        public string Economical_Life { get; set; }


        [SharepointFieldName("Engineering_Review")]
        public bool Engineering_Review { get; set; }

        [Required]
        [SharepointFieldName("Expected_Proceeds")]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "Expected Proceeds", AssociateTab = "Disposal,Purchase with Disposal,Lease With Disposal")]
        public double Expected_Proceeds { get; set; }

        [SharepointFieldName("File_x0020_Type")]
        public string FileType { get; set; }


        [SharepointFieldName("Funds_Committed")]
        public bool Funds_Committed { get; set; }

        [SharepointFieldName("HR_Review")]
        public bool HR_Review { get; set; }
        [SharepointFieldName("ID")]
        public int ID { get; set; }


        [SharepointFieldName("In_Budget")]
        public bool In_Budget { get; set; }

        [SharepointFieldName("InstanceID")]
        public int? InstanceID { get; set; }

        [SharepointFieldName("Investment_Type")]
        public string InvestmentType { get; set; }


        [SharepointFieldName("IT_Review")]
        public bool IT_Review { get; set; }

        [SharepointFieldName("ItemChildCount")]
        public string ItemChildCount { get; set; }
        [SharepointFieldName("FSObjType")]
        public string FSObjType { get; set; }

        [SharepointFieldName("Lease_Type")]
        public string LeaseType { get; set; }

        [SharepointFieldName("Legal_Review")]
        public bool Legal_Review { get; set; }

        [SharepointFieldName("Lend_Date")]
        public DateTime? Lend_Date { get; set; }


        [Required]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "Lender", AssociateTab = "Loans and Advances")]
        [SharepointFieldName("Lender")]
        public string Lender { get; set; }

        [Required]
        [SharepointFieldName("Length_of_Lease__x0028__x0023__o")]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "Length of lease", AssociateTab = "Lease,Lease With Disposal")]
        public double LengthOfLease { get; set; }

        [SharepointFieldName("_Level")]
        public int? Level { get; set; }

        [SharepointFieldName("Local_AR__x0023_")]
        public string LocalAR { get; set; }

        [SharepointFieldName("Location")]
        public string Location { get; set; }

        [SharepointFieldName("Market_Value")]
        public double Market_Value { get; set; }

        [SharepointFieldName("Maturity_Date")]
        public string Maturity_Date { get; set; } 

        [SharepointFieldName("Modified")]
        public DateTime? Modified { get; set; }

        [Required]
        [SharepointFieldName("Number_of_Share_to_Aquire")] [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "No. of shares to acquire", AssociateTab = "Equity Investment")]
        public double Number_of_Share_to_Aquire { get; set; }

        [Required]
        [SharepointFieldName("Number_of_Shares_to_Sell")]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "No. of shares to sell", AssociateTab = "Equity Divestiture")]
        public double Number_of_Shares_to_Sell { get; set; }

        [SharepointFieldName("Order")]
        public double Order { get; set; }

        [SharepointFieldName("Original_AR__x0023_"), DisplayName("Original_AR_#")]
        public string OriginalAR { get; set; }

        [SharepointFieldName("Original_Cost")]
        public double Original_Cost { get; set; }

        [Required]
        [SharepointFieldName("Original_Purchase_Price")]
        public double Original_Purchase_Price { get; set; }

        [SharepointFieldName("Other_Cost_or_Savings")]
        public double Other_Cost_or_Savings { get; set; }

        [SharepointFieldName("owshiddenversion")]
        public int owshiddenversion { get; set; }



        public string FileDirRef { get; set; }

        [SharepointFieldName("Percentage_of_Ownership_x002F_Sh")]
        public double PercentageOfOwnership { get; set; }

        [SharepointFieldName("Percentage_of_Ownership_at_Close")]
        public double Percentage_of_Ownership_at_Close { get; set; }


        [Required]
        [SharepointFieldName("Price_Per_Share_Being_Purchased")]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "Price Per Share Being Purchased", AssociateTab = "Equity Investment")]
        public double Price_Per_Share_Being_Purchased { get; set; }

        [Required]
        [SharepointFieldName("Price_Per_Share_Being_Sold")]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "Price Per Share Being Sold", AssociateTab = "Equity Divestiture")]
        public double Price_Per_Share_Being_Sold { get; set; }

        [Required]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "Principal Amount", AssociateTab = "Loans and Advances")]
        [SharepointFieldName("Principal_Amount")]
        public double Principal_Amount { get; set; }

         [SharepointFieldName("ProgId")]
        public string ProgId { get; set; }

        [SharepointFieldName("Project_End")]
        public DateTime? Project_End { get; set; }

        [SharepointFieldName("Project_Start")]
        public DateTime? Project_Start { get; set; }


        [SharepointFieldName("MetaInfo")]
        public string MetaInfo { get; set; }

        [Required]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "Purchase Price", AssociateTab = "Purchase,Expense,Purchase with Disposal")]
        [SharepointFieldName("Purchase_Price")]
        public double Purchase_Price { get; set; }

        [SharepointFieldName("Response_Due_Date")]
        public DateTime? Response_Due_Date { get; set; }

        [SharepointFieldName("_ModerationStatus")]
        public int ModerationStatus { get; set; }

        [SharepointFieldName("ScopeId")]
        public string ScopeId { get; set; }

        [SharepointFieldName("Services_x002F_Installation")]
        public double ServicesInstallation { get; set; }

        public LookupFieldMapper SortBehavior { get; set; }

        [SharepointFieldName("Stated_Interest_Rate")]
        public string Stated_Interest_Rate { get; set; } //decimal

        [SharepointFieldName("Submit_Action")]
        public string Submit_Action { get; set; }

      
        [SharepointFieldName("Tax_x002F_VAT")]
        public double TaxVAT { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [SharepointFieldName("Total_Cost")]
        public double Total_Cost { get; set; }

        [SharepointFieldName("Total_Other_Costs")]
        public double Total_Other_Costs { get; set; }

        [SharepointFieldName("Transaction_Close_Date")]
        public DateTime? Transaction_Close_Date { get; set; }


        [SharepointFieldName("FileRef")]
        public string FileRef { get; set; }

        [SharepointFieldName("WorkflowVersion")]
        public int WorkflowVersion { get; set; }

        [SharepointFieldName("Attachment_Folder_Id")]
        public string Attachment_Folder_Id { get; set; }

        [SharepointFieldName("Workflow_Instance_ID")]
        public string Workflow_Instance_ID { get; set; }

        [SharepointFieldName("Workflow_Restart_State")]
        public string Workflow_Restart_State { get; set; }

        [SharepointFieldName("Present_Situation_x002F_Issue")]
        [Required]
        [UsedIn(ControllerName = SB.AR.AppWeb.Helper.Tabs.NARRATIVE, FieldName="Present situation issue" )]
        
        public string PresentSituationIssue { get; set; }

       
        [SharepointFieldName("Proposed_Solution")]
        [Required]
        [UsedIn(ControllerName = SB.AR.AppWeb.Helper.Tabs.NARRATIVE, FieldName="Proposed Solution")]
        public string Proposed_Solution { get; set; }


        [SharepointFieldName("Other_Potential_Solutions")]
        [Required]
        [UsedIn(ControllerName = SB.AR.AppWeb.Helper.Tabs.NARRATIVE, FieldName= "Other potential solutions")]
        
        public string Other_Potential_Solutions { get; set; }


        [SharepointFieldName("Explanation_of_Costs")]
        [Required]
        [UsedIn(ControllerName = SB.AR.AppWeb.Helper.Tabs.NARRATIVE, FieldName="Explanation of Costs")]
        
        public string Explanation_of_Costs { get; set; }

     
        [SharepointFieldName("Financial_Measures")]
        public string Financial_Measures { get; set; }

        private string _PMOwner { get; set; }
        [Required]
        [UsedIn(ControllerName = Tabs.MAIN, FieldName = "PM Owner")]
        public string PMOwner
        {
            get;
            set;
        }

        public string PMOwnerLogin
        {           
            get;
            set;
        }



        [SharepointFieldName("PM_x002F_Owner")]
        public FieldUserValue PMUser { get; set; }


        [Required]
        [SharepointFieldName("Equity_Description"), DisplayName("Equity_Description")]
        [UsedIn(ControllerName = Tabs.FINANCIALS, FieldName = "Equity Description", AssociateTab = "Equity Investment,Equity Divestiture")]
        public string Equity_Description { get; set; }

        [SharepointFieldName("Audit")]
        public bool Audit { get; set; }

        [SharepointFieldName("Audit_Updated_By")]
        public FieldUserValue Audit_Updated_By { get; set; }

        public string SPHostUrl
        {
            get;
            set;
        }
        public string SPAppWebUrl
        {
            get;
            set;
        }
        public string SPLanguage
        {
            get;
            set;
        }

        private string _lookupValue = string.Empty;  // Backing store
        public string UserNameLookupValue
        {
            get;
            set;
        }
        public string LookupValue
        {
           
            get
            {
                if (PMUser == null && string.IsNullOrEmpty(UserNameLookupValue))
                    return string.Empty;
                else if (!string.IsNullOrEmpty(UserNameLookupValue))
                    return UserNameLookupValue;

                return this.PMUser.LookupValue;
            }
            set
            {
                _lookupValue = value;
            }
        }
        private int _lookupId = 0;
        public int LookupId
        {
            get
            {
                if (PMUser == null)
                    return 0;

                return this.PMUser.LookupId;
            }
            set
            {
                _lookupId = value;
            }
        }
        public bool IsDisposalFinance { get; set; }
        public bool IsFinanceTab { get; set; }
        public bool IsMaintab { get; set; }

        public bool IsApproved
        {
            get
            {
                if (this.Current_Status == "Approved" 
                 || this.Current_Status == "Pending Approvals"
                 || this.Current_Status == "Pending Edits" || this.Current_Status == "Rejected"
                 || this.Current_Status == "Completed")
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsRejected { get; set; }


    }

    public class UsedInAttribute : Attribute
    {
        public string ControllerName { get; set; }
        public string FieldName { get; set; }
        public string AssociateTab { get; set; }
    }
}