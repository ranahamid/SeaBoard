using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using arapp = SB.AR.AppWeb.Models;
namespace SB.AR.AppWeb.ViewModels
{
    public class LoanAdvancesFinancialsViewModel : ViewModelBase
    {
        
        
        private arapp.AR _ar;
        public LoanAdvancesFinancialsViewModel(SharePointContext spContext, arapp.AR ar)
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
        public int ID { get { return _ar.ID; } }
        public double? AR_ID { get { return _ar.AR_ID; } }
        public string Maturity_Date { get { return _ar.Maturity_Date; } } //DateTime?
        public string Lender { get { return _ar.Lender; } }
        public DateTime? Lend_Date { get { return _ar.Lend_Date; } }
        public string Borrower { get { return _ar.Borrower; } }
        public string Currency_Name { get { return _ar.Currency_Name; } }

        public double Principal_Amount { get { return _ar.Principal_Amount; } }

        public double Total_Cost { get { return _ar.Total_Cost; } }
        public double Budget_Amount { get { return _ar.Budget_Amount; } }
        public string BudgetLineItem { get { return _ar.BudgetLineItem; } }
        public bool In_Budget { get { return _ar.In_Budget; } }
        public bool Funds_Committed { get { return _ar.Funds_Committed; } }

        public string Stated_Interest_Rate { get { return _ar.Stated_Interest_Rate; } }

    }
}