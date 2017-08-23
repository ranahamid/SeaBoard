using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using arapp = SB.AR.AppWeb.Models;
namespace SB.AR.AppWeb.ViewModels
{
    public class PurchaseWDFinancialsViewModel : ViewModelBase
    {
       

        private arapp.AR _ar;
        public PurchaseWDFinancialsViewModel(SharePointContext spContext, arapp.AR ar)
            : base(spContext)
        {
            if (ar == null)
                ar = new arapp.AR();
            _ar = ar;
        }

        public arapp.AR AR
        {
            get
            {
                return this._ar;
            }
            
        }

        [DisplayName("AR Title")]
        public string Title
        {
            get
            {
                return _ar.Title;
            }
            
        }       
        public double? AR_ID { get { return _ar.AR_ID; } }
        public int ID { get { return _ar.ID; } }
        public double Original_Cost { get { return _ar.Original_Cost; } }
        public double Market_Value { get { return _ar.Market_Value; } }
        public double Current_Net_Book_Value { get { return _ar.Current_Net_Book_Value; } }

        [Range(1, int.MaxValue, ErrorMessage = "Expected Proceeds must be greater than zero")]
        public double Expected_Proceeds { get { return _ar.Expected_Proceeds; } }

        public double Original_Purchase_Price { get { return _ar.Original_Purchase_Price; } }
        public double TaxVAT { get { return _ar.TaxVAT; } }
        public double ServicesInstallation { get { return _ar.ServicesInstallation; } }
        public double Other_Cost_or_Savings { get { return _ar.Other_Cost_or_Savings; } }
        public DateTime? Date_Aquired { get { return _ar.Date_Aquired; } }
        public string Department_to_Charge { get { return _ar.Department_to_Charge; } }
        public string Economical_Life { get { return _ar.Economical_Life; } }
     
        [Range(1, int.MaxValue, ErrorMessage = "Purchase Price must be greater than zero")]
        public double Purchase_Price { get { return _ar.Purchase_Price; } }

        public double Total_Cost { get { return _ar.Total_Cost; } }
        public double Budget_Amount { get { return _ar.Budget_Amount; } }
        public string BudgetLineItem { get { return _ar.BudgetLineItem; } }
        public bool In_Budget { get { return _ar.In_Budget; } }
        public bool Funds_Committed { get { return _ar.Funds_Committed; } }

        public string InvestmentType { get { return _ar.InvestmentType; } }
        public bool IsDisposalFinance { get { return true; } }


        [Required]
        public string Condition_of_Assets
        {
            get
            {
                if (!string.IsNullOrEmpty(_ar.Condition_of_Assets))
                {
                    var text = Regex.Replace(_ar.Condition_of_Assets, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return _ar.Condition_of_Assets;
            }
        }

    }
}