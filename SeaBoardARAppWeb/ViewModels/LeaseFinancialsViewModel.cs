using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using arapp = SB.AR.AppWeb.Models;
namespace SB.AR.AppWeb.ViewModels
{
    public class LeaseFinancialsViewModel : ViewModelBase
    {
       
        
        private arapp.AR _ar;
        public LeaseFinancialsViewModel(SharePointContext spContext, arapp.AR ar)
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
        public string LeaseType { get { return _ar.LeaseType; } }
        public int ID { get { return _ar.ID; } }
        public double LengthOfLease { get { return _ar.LengthOfLease; } }
        public double? AR_ID { get { return _ar.AR_ID; } }
        public double Cost_Per_Period { get { return _ar.Cost_Per_Period; } }
        public double Other_Cost_or_Savings { get { return _ar.Other_Cost_or_Savings; } }
        public double Current_Year_Cost_Commitment { get { return _ar.Current_Year_Cost_Commitment; } }


        public string Economical_Life { get { return _ar.Economical_Life; } }

        public double Total_Cost { get { return _ar.Total_Cost; } }
        public double Budget_Amount { get { return _ar.Budget_Amount; } }
        public string BudgetLineItem { get { return _ar.BudgetLineItem; } }
        public bool In_Budget { get { return _ar.In_Budget; } }
        public bool Funds_Committed { get { return _ar.Funds_Committed; } }
    }
}