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

namespace SB.AR.AppWeb.Models
{

    public class ARApprovers //:IItemEntity
    {
        [SharepointFieldName("Amount_of_Additional_Funds_Requi"), DisplayName("Amount_of_Additional_Funds_Requi")]
        public double Amount_of_Additional_Funds_Requi { get; set; }

        [SharepointFieldName("Amount_of_Funding_Required_from_"), DisplayName("Amount_of_Funding_Required_from_")]
        public double Amount_of_Funding_Required_from_ { get; set; }

        [SharepointFieldName("AppAuthor"), DisplayName("App Created By")]
        public LookupFieldMapper AppAuthor { get; set; }

        [DisplayName("App Modified By")]
        public LookupFieldMapper AppEditor { get; set; }

        [SharepointFieldName("Title"), DisplayName("AR Title")]
        public string   Title { get; set; }

        [SharepointFieldName("AR__x0023_"), DisplayName("AR_#")]
        public string ARNumber { get; set; }

        [SharepointFieldName("AR_ID"), DisplayName("AR_ID")]
        public double? AR_ID { get; set; }

        [Required(ErrorMessage = "Please enter AR Type")]
        [SharepointFieldName("AR_Type")]
        public string AR_Type { get; set; }

        [SharepointFieldName("_ModerationStatus")]
        public int ApprovalStatus { get; set; }        

        [SharepointFieldName("Asset_Location")]
        public string Asset_Location { get; set; }

        [SharepointFieldName("Borrower")]
        public string Borrower { get; set; }

        [SharepointFieldName("Budget_Amount")]
        public double Budget_Amount { get; set; }

        [SharepointFieldName("Budget_Line_Item__x0023_")]
        public string Budget_Line_Item__x0023_ { get; set; }

        public LookupFieldMapper Category { get; set; }

        public LookupFieldMapper SyncClientId { get; set; }

        [SharepointFieldName("Company_Name")]
        public LookupFieldMapper Company_Name { get; set; }

        [SharepointFieldName("Condition_of_Assets")]
        public string Condition_of_Assets { get; set; }

        [SharepointFieldName("Consolidated_x002F_Non_x002d_Con")]
        public string Consolidated_x002F_Non_x002d_Con { get; set; }

        [SharepointFieldName("Cost_Per_Period")]
        public double Cost_Per_Period { get; set; }

        [SharepointFieldName("Created")]
        public DateTime? Created { get; set; }

        public string Created_x0020_Date { get; set; }

        [SharepointFieldName("Currency_Name")]
        public string Currency_Name { get; set; }

        [SharepointFieldName("Current_Approver")]
        public string Current_Approver { get; set; }


        [SharepointFieldName("Current_Net_Book_Value")]
        public double Current_Net_Book_Value { get; set; }

        [SharepointFieldName("Current_Status")]
        public ChoiceFieldMapper Current_Status { get; set; }

        [SharepointFieldName("Current_Year_Cost_Commitment")]
        public double Current_Year_Cost_Commitment { get; set; }

        [SharepointFieldName("Date_Aquired")]
        public DateTime? Date_Aquired { get; set; }


        [SharepointFieldName("Date_Assigned")]
        public DateTime? Date_Assigned { get; set; }

        [SharepointFieldName("Department_to_Charge")]
        public string Department_to_Charge { get; set; }

         [SharepointFieldName("Division")]
        public LookupFieldMapper Division { get; set; }


        [SharepointFieldName("Economical_Life")]
        public string Economical_Life { get; set; }


        [SharepointFieldName("Engineering_Review")]
        public bool Engineering_Review { get; set; }

        [SharepointFieldName("Expected_Proceeds")]
        public double Expected_Proceeds { get; set; }

        [SharepointFieldName("File_x0020_Type")]
        public string File_x0020_Type { get; set; }


        public string FolderChildCount { get; set; }

        [SharepointFieldName("Funds_Committed")]
        public bool Funds_Committed { get; set; }

        [SharepointFieldName("GUID")]
        public Guid GUID { get; set; }


        [SharepointFieldName("HR_Review")]
        public bool HR_Review { get; set; }
        [SharepointFieldName("ID")]
        public int ID { get; set; }


        [SharepointFieldName("In_Budget")]
        public bool In_Budget { get; set; }

        [SharepointFieldName("InstanceID")]
        public int? InstanceID { get; set; }

        [SharepointFieldName("Investment_Type")]
        public string Investment_Type { get; set; }


        [SharepointFieldName("IT_Review")]
        public bool IT_Review { get; set; }

        public string ItemChildCount { get; set; }

        public string FSObjType { get; set; }

        [SharepointFieldName("Lease_Type")]
        public string Lease_Type { get; set; }

        [SharepointFieldName("Legal_Review")]
        public bool Legal_Review { get; set; }

        [SharepointFieldName("Lend_Date")]
        public DateTime? Lend_Date { get; set; }

        [SharepointFieldName("Lender")]
        public string Lender { get; set; }

        [SharepointFieldName("Length_of_Lease__x0028__x0023__o")]
        public double Length_of_Lease__x0028__x0023__o { get; set; }

        [SharepointFieldName("_Level")]
        public int? _Level { get; set; }

        [SharepointFieldName("Local_AR__x0023_")]
        public string Local_AR__x0023_ { get; set; }

        [SharepointFieldName("Location")]
        public string Location { get; set; }

        [SharepointFieldName("Market_Value")]
        public double Market_Value { get; set; }

        [SharepointFieldName("Maturity_Date")]
        public string Maturity_Date { get; set; } //DateTime?

        [SharepointFieldName("Modified")]
        public DateTime? Modified { get; set; }

        public string Last_x0020_Modified { get; set; }

        [SharepointFieldName("Number_of_Share_to_Aquire")]
        public double Number_of_Share_to_Aquire { get; set; }

        [SharepointFieldName("Number_of_Shares_to_Sell")]
        public double Number_of_Shares_to_Sell { get; set; }

        [SharepointFieldName("Order")]
        public double Order { get; set; }

        [SharepointFieldName("Original_AR__x0023_")]
        public string Original_AR__x0023_ { get; set; }

        [SharepointFieldName("Original_Cost")]
        public double Original_Cost { get; set; }

        [SharepointFieldName("Original_Purchase_Price")]
        public double Original_Purchase_Price { get; set; }

        [SharepointFieldName("Other_Cost_or_Savings")]
        public double Other_Cost_or_Savings { get; set; }

        [SharepointFieldName("owshiddenversion")]
        public int owshiddenversion { get; set; }



        public string FileDirRef { get; set; }

        [SharepointFieldName("Percentage_of_Ownership_x002F_Sh")]
        public double Percentage_of_Ownership_x002F_Sh { get; set; }

        [SharepointFieldName("Percentage_of_Ownership_at_Close")]
        public double Percentage_of_Ownership_at_Close { get; set; }

        [SharepointFieldName("Price_Per_Share_Being_Purchased")]
        public double Price_Per_Share_Being_Purchased { get; set; }

        [SharepointFieldName("Price_Per_Share_Being_Sold")]
        public double Price_Per_Share_Being_Sold { get; set; }

        [SharepointFieldName("Principal_Amount")]
        public double Principal_Amount { get; set; }


        public string ProgId { get; set; }

        [SharepointFieldName("Project_End")]
        public DateTime? Project_End { get; set; }

        [SharepointFieldName("Project_Start")]
        public DateTime? Project_Start { get; set; }



        public string MetaInfo { get; set; }

        [SharepointFieldName("Purchase_Price")]
        public double Purchase_Price { get; set; }


        [SharepointFieldName("Response_Due_Date")]
        public DateTime? Response_Due_Date { get; set; }

        public int _ModerationStatus { get; set; }


        public string ScopeId { get; set; }

        [SharepointFieldName("Services_x002F_Installation")]
        public double Services_x002F_Installation { get; set; }



        public LookupFieldMapper SortBehavior { get; set; }

        [SharepointFieldName("Stated_Interest_Rate")]
        public string Stated_Interest_Rate { get; set; } //decimal

        [SharepointFieldName("Submit_Action")]
        public string Submit_Action { get; set; }

        [SharepointFieldName("Tax_x002F_VAT")]
        public double Tax_x002F_VAT { get; set; }

        [SharepointFieldName("Total_Cost")]
        public double Total_Cost { get; set; }

        [SharepointFieldName("Total_Other_Costs")]
        public double Total_Other_Costs { get; set; }

        [SharepointFieldName("Transaction_Close_Date")]
        public DateTime? Transaction_Close_Date { get; set; }

        public Guid UniqueId { get; set; }

        public string FileRef { get; set; }

        [SharepointFieldName("WorkflowInstanceID")]
        public Guid WorkflowInstanceID { get; set; }

        [SharepointFieldName("WorkflowVersion")]
        public int WorkflowVersion { get; set; }

        [SharepointFieldName("Workflow_Instance_ID")]
        public string Workflow_Instance_ID { get; set; }

        [SharepointFieldName("Workflow_Restart_State")]
        public string Workflow_Restart_State { get; set; }


        [SharepointFieldName("Present_Situation_x002F_Issue")]
        public string Present_Situation_x002F_Issue { get; set; }

        [SharepointFieldName("Proposed_Solution")]
        public string Proposed_Solution { get; set; }

        [SharepointFieldName("Other_Potential_Solutions")]
        public string Other_Potential_Solutions { get; set; }

        [SharepointFieldName("Explanation_of_Costs")]
        public string Explanation_of_Costs { get; set; }

        [SharepointFieldName("Financial_Measures")]
        public string Financial_Measures { get; set; }

        [SharepointFieldName("PM_x002F_Owner")]
        public string PM_x002F_Owner { get; set; }

    }
}