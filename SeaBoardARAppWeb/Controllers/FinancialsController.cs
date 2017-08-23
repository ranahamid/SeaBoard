using SB.AR.AppWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using SB.AR.AppWeb.Models;
using Newtonsoft.Json;

namespace SB.AR.AppWeb.Controllers
{
    public class FinancialsController : SBControllerBase
    {
        //
        // GET: /Financials/
        public ActionResult Index()
        {
            var financial = new FinancialsViewModel(SPContext, this.AR);
            return PartialView("_Financials", financial);
        }


        [HttpPost]
        public ActionResult SaveARAsDraft(AR.AppWeb.Models.AR finance)
        {
            //var hostUrl = this.SPHostUrl;
            //var result = SaveAR(finance);
            //finance.Current_Status ="";
            //finance.Submit_Action = "Hold as Draft";

            var hostUrl = this.SPHostUrl;
            var result = SaveAR(finance);

            this.AR.Current_Status = "Not Submitted";
            this.AR.Submit_Action = "Hold as Draft";

            SaveARToList(this.AR);
            var retObj = new Result
            {
                Data = AR,
                IsRedirect = true

            };
            var resultData = JsonConvert.SerializeObject(retObj);
            return Json(resultData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveARAsSubmitRMReview(AR.AppWeb.Models.AR finance)
        {
            var hostUrl = this.SPHostUrl;
           
            var result = SaveAR(finance);
            SaveARToList(this.AR);

            this.AR.Current_Status = "Pending Edits";
            this.AR.Submit_Action = "PM Review";

            var retObj = new Result
            {
                Data = AR,
                IsRedirect = true

            };
            var resultData = JsonConvert.SerializeObject(retObj);
            return Json(resultData, JsonRequestBehavior.AllowGet);

        } 

        [HttpPost]
        public ActionResult SaveAR(SB.AR.AppWeb.Models.AR finance)
        {
            ViewBag.ArTypeName = this.ARTypeName;
            SB.AR.AppWeb.Models.AR Ar = null;

            if (this.AR == null)
            {
                Ar = new SB.AR.AppWeb.Models.AR();

             //   Ar.Current_Status = finance.Current_Status;
                Ar.Submit_Action = finance.Submit_Action;
            }
            else
            {
                Ar = this.AR;
                //if (!string.IsNullOrEmpty(this.AR.Current_Status))
                //    Ar.Current_Status = this.AR.Current_Status;

                if (!string.IsNullOrEmpty(this.AR.Submit_Action))
                    Ar.Submit_Action = this.AR.Submit_Action;
            }

            var purPrice = Request["Purchase_Price"];

            if (!string.IsNullOrEmpty(purPrice))
            {
                Ar.Purchase_Price = Convert.ToDouble(HttpUtility.HtmlDecode(purPrice.Replace("$", "")));
            }

            var TaxVAT = Request["TaxVAT"];

            if (!string.IsNullOrEmpty(TaxVAT))
            {
                Ar.TaxVAT = Convert.ToDouble(HttpUtility.HtmlDecode(TaxVAT.Replace("$", "")));
            }
            var ServicesInstallation = Request["ServicesInstallation"];

            if (!string.IsNullOrEmpty(ServicesInstallation))
            {
                Ar.ServicesInstallation = Convert.ToDouble(HttpUtility.HtmlDecode(ServicesInstallation.Replace("$", "")));
            }
            var Other_Cost_or_Savings = Request["Other_Cost_or_Savings"];

            if (!string.IsNullOrEmpty(Other_Cost_or_Savings))
            {
                Ar.Other_Cost_or_Savings = Convert.ToDouble(HttpUtility.HtmlDecode(Other_Cost_or_Savings.Replace("$", "")));
            }
          
            var Original_Cost = Request["Original_Cost"]; if (!string.IsNullOrEmpty(Original_Cost)) { Ar.Original_Cost = Convert.ToDouble(HttpUtility.HtmlDecode(Original_Cost.Replace("$", ""))); }
            var Market_Value = Request["Market_Value"]; if (!string.IsNullOrEmpty(Market_Value)) { Ar.Market_Value = Convert.ToDouble(HttpUtility.HtmlDecode(Market_Value.Replace("$", ""))); }
            var Current_Net_Book_Value = Request["Current_Net_Book_Value"]; if (!string.IsNullOrEmpty(Current_Net_Book_Value)) { Ar.Current_Net_Book_Value = Convert.ToDouble(HttpUtility.HtmlDecode(Current_Net_Book_Value.Replace("$", ""))); }
            var Expected_Proceeds = Request["Expected_Proceeds"]; if (!string.IsNullOrEmpty(Expected_Proceeds)) { Ar.Expected_Proceeds = Convert.ToDouble(HttpUtility.HtmlDecode(Expected_Proceeds.Replace("$", ""))); }
            var Original_Purchase_Price = Request["Original_Purchase_Price"]; if (!string.IsNullOrEmpty(Original_Purchase_Price)) { Ar.Original_Purchase_Price = Convert.ToDouble(HttpUtility.HtmlDecode(Original_Purchase_Price.Replace("$", ""))); }
            var Price_Per_Share_Being_Sold = Request["Price_Per_Share_Being_Sold"]; if (!string.IsNullOrEmpty(Price_Per_Share_Being_Sold)) { Ar.Price_Per_Share_Being_Sold = Convert.ToDouble(HttpUtility.HtmlDecode(Price_Per_Share_Being_Sold.Replace("$", ""))); }
            var Total_Other_Costs = Request["Total_Other_Costs"]; if (!string.IsNullOrEmpty(Total_Other_Costs)) { Ar.Total_Other_Costs = Convert.ToDouble(HttpUtility.HtmlDecode(Total_Other_Costs.Replace("$", ""))); }
            var Budget_Amount = Request["Budget_Amount"]; if (!string.IsNullOrEmpty(Budget_Amount)) { Ar.Budget_Amount = Convert.ToDouble(HttpUtility.HtmlDecode(Budget_Amount.Replace("$", ""))); }
            var Price_Per_Share_Being_Purchased = Request["Price_Per_Share_Being_Purchased"]; if (!string.IsNullOrEmpty(Price_Per_Share_Being_Purchased)) { Ar.Price_Per_Share_Being_Purchased = Convert.ToDouble(HttpUtility.HtmlDecode(Price_Per_Share_Being_Purchased.Replace("$", ""))); }
            var Cost_Per_Period = Request["Cost_Per_Period"]; if (!string.IsNullOrEmpty(Cost_Per_Period)) { Ar.Cost_Per_Period = Convert.ToDouble(HttpUtility.HtmlDecode(Cost_Per_Period.Replace("$", ""))); }
            var Current_Year_Cost_Commitment = Request["Current_Year_Cost_Commitment"]; if (!string.IsNullOrEmpty(Current_Year_Cost_Commitment)) { Ar.Current_Year_Cost_Commitment = Convert.ToDouble(HttpUtility.HtmlDecode(Current_Year_Cost_Commitment.Replace("$", ""))); }
            var LengthOfLease = Request["LengthOfLease"]; if (!string.IsNullOrEmpty(LengthOfLease)) { Ar.LengthOfLease = Convert.ToDouble(HttpUtility.HtmlDecode(LengthOfLease.Replace("$", ""))); }
            var Original_AR_Amount = Request["Original_AR_Amount"]; if (!string.IsNullOrEmpty(Original_AR_Amount)) { Ar.Original_AR_Amount = Convert.ToDouble(HttpUtility.HtmlDecode(Original_AR_Amount.Replace("$", ""))); }
            var AdditionaAmountFundsRequired = Request["AdditionaAmountFundsRequired"]; if (!string.IsNullOrEmpty(AdditionaAmountFundsRequired)) { Ar.AdditionaAmountFundsRequired = Convert.ToDouble(HttpUtility.HtmlDecode(AdditionaAmountFundsRequired.Replace("$", ""))); }
            var Total_Cost = Request["Total_Cost"]; if (!string.IsNullOrEmpty(Total_Cost)) { Ar.Total_Cost = Convert.ToDouble(HttpUtility.HtmlDecode(Total_Cost.Replace("$", ""))); }

            var Principal_Amount = Request["Principal_Amount"]; if (!string.IsNullOrEmpty(Principal_Amount)) { Ar.Principal_Amount = Convert.ToDouble(HttpUtility.HtmlDecode(Principal_Amount.Replace("$", ""))); }

            //request-

            var Date_Aquired = Request["Date_Aquired"];
            if (!string.IsNullOrEmpty(Date_Aquired) && Date_Aquired.Contains(","))
            { 
                    Date_Aquired = Date_Aquired.Split(',')[1]; 
                    if (!string.IsNullOrEmpty(Date_Aquired)) { 
                        Ar.Date_Aquired = Convert.ToDateTime(Date_Aquired); } 
                }

            var Transaction_Close_Date = Request["Transaction_Close_Date"];
            if (!string.IsNullOrEmpty(Transaction_Close_Date) && Transaction_Close_Date.Contains(","))
            { 
                    Transaction_Close_Date = Transaction_Close_Date.Split(',')[1]; 
                    if (!string.IsNullOrEmpty(Transaction_Close_Date)) { 
                        Ar.Transaction_Close_Date = Convert.ToDateTime(Transaction_Close_Date); } 
                }
            var Maturity_Date = Request["Maturity_Date"];
            if (!string.IsNullOrEmpty(Maturity_Date) && Maturity_Date.Contains(","))
            {
                Maturity_Date = Maturity_Date.Split(',')[1];
                if (!string.IsNullOrEmpty(Maturity_Date)) { 
                    Ar.Maturity_Date = Convert.ToString(Maturity_Date); 
                }
            }
            var Lend_Date = Request["Lend_Date"];
            if (!string.IsNullOrEmpty(Lend_Date) && Lend_Date.Contains(","))
            {
                Lend_Date = Lend_Date.Split(',')[1]; 
                if (!string.IsNullOrEmpty(Lend_Date)) { 
                    Ar.Lend_Date = Convert.ToDateTime(Lend_Date); 
                }
            }
             

            if (this.AR != null && this.AR.ID > 0)
            {
                Ar.ID = this.AR.ID;
            }
            else if (finance.ID > 0)
                Ar.ID = finance.ID;

            if (finance.Original_Cost > 0)
                Ar.Original_Cost = finance.Original_Cost;

            if (finance.Market_Value > 0)
                Ar.Market_Value = finance.Market_Value;

            if (finance.Current_Net_Book_Value > 0)
                Ar.Current_Net_Book_Value = finance.Current_Net_Book_Value;

            if (finance.Expected_Proceeds > 0)
                Ar.Expected_Proceeds = finance.Expected_Proceeds;

            if (finance.Date_Aquired != null)

                Ar.Date_Aquired = finance.Date_Aquired;

            if (finance.AdditionaAmountFundsRequired > 0)
                Ar.AdditionaAmountFundsRequired = finance.AdditionaAmountFundsRequired;

            if (!string.IsNullOrEmpty(finance.OriginalAR))
                Ar.OriginalAR = finance.OriginalAR;



            if (finance.Original_AR_Amount > 0)
                Ar.Original_AR_Amount = finance.Original_AR_Amount;



            if (!string.IsNullOrEmpty(finance.ConsolidatedNonCon))
                Ar.ConsolidatedNonCon = finance.ConsolidatedNonCon;


            if (!string.IsNullOrEmpty(finance.LeaseType))
                Ar.LeaseType = finance.LeaseType;

            if (!string.IsNullOrEmpty(finance.Stated_Interest_Rate))
                Ar.Stated_Interest_Rate = finance.Stated_Interest_Rate;

            if (!string.IsNullOrEmpty(finance.Equity_Description))
                Ar.Equity_Description = finance.Equity_Description;

            if (!string.IsNullOrEmpty(finance.Department_to_Charge))
                Ar.Department_to_Charge = finance.Department_to_Charge;

            if (!string.IsNullOrEmpty(finance.Condition_of_Assets))
                Ar.Condition_of_Assets = finance.Condition_of_Assets;

            if (finance.Original_Purchase_Price > 0)
                Ar.Original_Purchase_Price = finance.Original_Purchase_Price;

            if (finance.Number_of_Shares_to_Sell > 0)
                Ar.Number_of_Shares_to_Sell = finance.Number_of_Shares_to_Sell;

            if ((finance.Price_Per_Share_Being_Sold) > 0)
                Ar.Price_Per_Share_Being_Sold = finance.Price_Per_Share_Being_Sold;
            if ((finance.Total_Other_Costs) > 0)
                Ar.Total_Other_Costs = finance.Total_Other_Costs;
            if ((finance.Transaction_Close_Date) != null)
                Ar.Transaction_Close_Date = finance.Transaction_Close_Date;
            if ((finance.Budget_Amount) > 0)
                Ar.Budget_Amount = finance.Budget_Amount;
            if (!string.IsNullOrEmpty(finance.BudgetLineItem))
                Ar.BudgetLineItem = finance.BudgetLineItem;

            if (!finance.IsDisposalFinance)
            {
                Ar.In_Budget = finance.In_Budget;
                Ar.Funds_Committed = finance.Funds_Committed;

            }
            if ((finance.Price_Per_Share_Being_Purchased) > 0)
                Ar.Price_Per_Share_Being_Purchased = finance.Price_Per_Share_Being_Purchased;

            if ((finance.PercentageOfOwnership) > 0)
                Ar.PercentageOfOwnership = finance.PercentageOfOwnership;


            

            if ((finance.Number_of_Share_to_Aquire) > 0)
                Ar.Number_of_Share_to_Aquire = finance.Number_of_Share_to_Aquire;
            if ((finance.Percentage_of_Ownership_at_Close) > 0)
                Ar.Percentage_of_Ownership_at_Close = finance.Percentage_of_Ownership_at_Close;
            if ((finance.TaxVAT) > 0)
                Ar.TaxVAT = finance.TaxVAT;
            if ((finance.ServicesInstallation) > 0)
                Ar.ServicesInstallation = finance.ServicesInstallation;
            if ((finance.Other_Cost_or_Savings) > 0)
                Ar.Other_Cost_or_Savings = finance.Other_Cost_or_Savings;
            if (!string.IsNullOrEmpty(finance.Economical_Life))
                Ar.Economical_Life = finance.Economical_Life;
           
            if ((finance.Total_Cost) > 0)
                Ar.Total_Cost = finance.Total_Cost;

            if (!string.IsNullOrEmpty(finance.InvestmentType))
                Ar.InvestmentType = finance.InvestmentType;

            if ((finance.Cost_Per_Period) > 0)
                Ar.Cost_Per_Period = finance.Cost_Per_Period;
            if ((finance.Current_Year_Cost_Commitment) > 0)
                Ar.Current_Year_Cost_Commitment = finance.Current_Year_Cost_Commitment;
            if ((finance.LengthOfLease) > 0)
                Ar.LengthOfLease = finance.LengthOfLease;
            if (!string.IsNullOrEmpty(finance.Maturity_Date))
                Ar.Maturity_Date = finance.Maturity_Date;
            if (!string.IsNullOrEmpty(finance.Lender))
                Ar.Lender = finance.Lender;
            if ((finance.Lend_Date) != null)
                Ar.Lend_Date = finance.Lend_Date;
            if (!string.IsNullOrEmpty(finance.Borrower))
                Ar.Borrower = finance.Borrower;
            if (!string.IsNullOrEmpty(finance.Currency_Name))
                Ar.Currency_Name = finance.Currency_Name;
            if ((finance.Principal_Amount) > 0)
                Ar.Principal_Amount = finance.Principal_Amount;
            if ((finance.ServicesInstallation) > 0)
                Ar.ServicesInstallation = finance.ServicesInstallation;
            if ((finance.Purchase_Price) > 0)
                Ar.Purchase_Price = finance.Purchase_Price;

            Ar.IsFinanceTab = true;



            if (!Ar.IsApproved)
            {
                Session["AR"] = Ar;
            }
            //var data = SaveARToList(Ar);

            var retObj = new Result
            {
                Data = AR,
                IsRedirect = false

            };
            var resultData = JsonConvert.SerializeObject(retObj);
            return Json(resultData, JsonRequestBehavior.AllowGet);


        }

        public ActionResult LoadFinancialPartial(string id)
        {
            var viewName = string.Empty;
            PartialViewResult returnPartial = null;
            if (string.IsNullOrEmpty(id) && this.AR != null)
            {
                if (null != Session["ARTypeCollection"])
                {
                    var arList = (List<SelectListItem>)Session["ARTypeCollection"];
                    var sItemType = arList.FirstOrDefault(c => this.AR.AR_Type.Trim().ToLowerInvariant() == c.Text.ToLower().Trim());
                    if (sItemType != null)
                        id = sItemType.Value.ToLower();
                }
                // id = 
            }
            switch (id)
            {
                case "purchase":
                    viewName = "_PurchaseFinancials";
                    var Purchase = new PurchaseFinancialsViewModel(SPContext, this.AR);
                    returnPartial = PartialView(viewName, Purchase);
                    break;
                case "expense":
                    viewName = "_ExpenseFinancials";
                    var Expense = new ExpenseFinancialsViewModel(SPContext, this.AR);
                    returnPartial = PartialView(viewName, Expense);
                    break;

                case "lease":
                    viewName = "_LeaseFinancials";
                    var Lease = new LeaseFinancialsViewModel(SPContext, this.AR);
                    returnPartial = PartialView(viewName, Lease);
                    break;
                case "disposal":
                    viewName = "_DisposalFinancials";
                    var Disposa = new DisposalFinancialsViewModel(SPContext, this.AR);
                    returnPartial = PartialView(viewName, Disposa);
                    break;
                case "purchasewithdisposal":
                    viewName = "_PurchaseWDFinancials";
                    var PurchaseWD = new PurchaseWDFinancialsViewModel(SPContext, this.AR);
                    returnPartial = PartialView(viewName, PurchaseWD);
                    break;
                case "leasewithdisposal":
                    viewName = "_LeaseWDFinancials";
                    var LeaseWD = new LeaseWDFinancialsViewModel(SPContext, this.AR);
                    returnPartial = PartialView(viewName, LeaseWD);
                    break;
                case "loansandadvances":
                    viewName = "_LoanAdvancesFinancials";
                    var LoanAdvance = new LoanAdvancesFinancialsViewModel(SPContext, this.AR);
                    returnPartial = PartialView(viewName, LoanAdvance);
                    break;

                case "equityinvestment":
                    viewName = "_EquityInvestmentsFinancials";
                    var EquityInvestment = new EquityInvestmentsFinancialsViewModel(SPContext, this.AR);
                    returnPartial = PartialView(viewName, EquityInvestment);
                    break;
                case "equitydivestiture":
                    viewName = "_EquityDivestitureFinancials";
                    var EquityDivestiture = new EquityDivestitureFinancialsViewModel(SPContext, this.AR);
                    returnPartial = PartialView(viewName, EquityDivestiture);
                    break;
                case "supplemental":
                    viewName = "_SupplementalFinancials";
                    var dFinance = new SupplementalFinancialsViewModel(SPContext, this.AR);
                    returnPartial = PartialView(viewName, dFinance);
                    break;
                default:
                    viewName = "_PurchaseFinancials";
                    var PurchaseFinancials = new PurchaseFinancialsViewModel(SPContext, this.AR);
                    returnPartial = PartialView(viewName, PurchaseFinancials);
                    break;
            }
            return returnPartial;
        }
    }
}