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
    public class EquityInvestmentsFinancialsViewModel : ViewModelBase
    {
       
        
        private arapp.AR _ar;
        public EquityInvestmentsFinancialsViewModel(SharePointContext spContext, arapp.AR ar)
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


        [Required]
       // [Range(1, int.MaxValue, ErrorMessage = "Price Per Share Being Purchased must be greater than zero")]
        public double Price_Per_Share_Being_Purchased { get { return _ar.Price_Per_Share_Being_Purchased; } }

        [Required]
      //  [Range(1, int.MaxValue, ErrorMessage = "Number of Share to_Aquire must be greater than zero")]
        public double Number_of_Share_to_Aquire { get { return _ar.Number_of_Share_to_Aquire; } }


        public double Total_Other_Costs { get { return _ar.Total_Other_Costs; } }

        public double Percentage_of_Ownership_at_Close { get { return _ar.Percentage_of_Ownership_at_Close; } }
        public DateTime? Transaction_Close_Date { get { return _ar.Transaction_Close_Date; } }
        public double Total_Cost { get { return _ar.Total_Cost; } }
        public double Budget_Amount { get { return _ar.Budget_Amount; } }
        public string BudgetLineItem { get { return _ar.BudgetLineItem; } }

        public bool In_Budget { get { return _ar.In_Budget; } }
        public bool Funds_Committed { get { return _ar.Funds_Committed; } }
        public string ConsolidatedNonCon { get { return _ar.ConsolidatedNonCon; } }
        [Required]
        public string Equity_Description
        {
            get
            {
                if (!string.IsNullOrEmpty(_ar.Equity_Description))
                {
                    var text = Regex.Replace(_ar.Equity_Description, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return _ar.Equity_Description;
            }
        }

    }
}