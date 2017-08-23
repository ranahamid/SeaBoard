using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using arapp = SB.AR.AppWeb.Models;
namespace SB.AR.AppWeb.ViewModels
{
    public class SupplementalFinancialsViewModel : ViewModelBase
    {
        

        

           private arapp.AR _ar;
           public SupplementalFinancialsViewModel(SharePointContext spContext, arapp.AR ar)
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

        public int ID
        {
            get
            {
                return _ar.ID;
            }
        }
        public double? AR_ID
        {
            get
            {
                return _ar.AR_ID;
            }
        }
        public string AR_Type
        {
            get
            {
                return _ar.AR_Type;
            }
        }
        public string Current_Status
        {
            get
            {
                return _ar.Current_Status;
            }
        }
        public double Total_Cost
        {
            get
            {
                return _ar.Total_Cost;
            }
        }
        public string OriginalAR
        {
            get
            {
                return _ar.OriginalAR;
            }
        }
        public double AdditionaAmountFundsRequired
        {
            get
            {
                return _ar.AdditionaAmountFundsRequired;
            }
        }
        public double Original_AR_Amount
        {
            get
            {
                return _ar.Original_AR_Amount;
            }
        }
        public double Budget_Amount
        {
            get
            {
                return _ar.Budget_Amount;
            }
        }
        public string BudgetLineItem
        {
            get
            {
                return _ar.BudgetLineItem;
            }
        }
        public bool In_Budget
        {
            get
            {
                return _ar.In_Budget;
            }
        }
        public bool Funds_Committed
        {
            get
            {
                return _ar.Funds_Committed;
            }
        }

    }
}