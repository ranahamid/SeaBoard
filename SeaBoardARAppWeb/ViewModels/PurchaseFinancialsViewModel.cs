using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using arapp = SB.AR.AppWeb.Models;
namespace SB.AR.AppWeb.ViewModels
{
    public class PurchaseFinancialsViewModel : ViewModelBase
    {
         private arapp.AR _ar;
         public PurchaseFinancialsViewModel(SharePointContext spContext, arapp.AR ar)
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
        public string InvestmentType { get { return _ar.InvestmentType; } }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Purchase Price must be greater than zero")]
        public double? Purchase_Price { get { return _ar.Purchase_Price; } }

        public int ID { get { return _ar.ID; } }
        public double? AR_ID { get { return _ar.AR_ID; } }
        public double TaxVAT { get { return _ar.TaxVAT; } }
        public double ServicesInstallation { get { return _ar.ServicesInstallation; } }
        public double Other_Cost_or_Savings { get { return _ar.Other_Cost_or_Savings; } }
        public string Economical_Life { get { return _ar.Economical_Life; } }
        public double Total_Cost { get { return _ar.Total_Cost; } }
        public double Budget_Amount { get { return _ar.Budget_Amount; } }
        public string BudgetLineItem { get { return _ar.BudgetLineItem; } }
        public bool In_Budget { get { return _ar.In_Budget; } }
        public bool Funds_Committed { get { return _ar.Funds_Committed; } }
    }
}