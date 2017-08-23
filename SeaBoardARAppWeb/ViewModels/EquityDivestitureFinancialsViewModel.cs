using NSBD.SharepointAutoMapper;
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
    public class EquityDivestitureFinancialsViewModel : ViewModelBase
    {
       
        private arapp.AR _ar;
        public EquityDivestitureFinancialsViewModel(SharePointContext spContext, arapp.AR ar)
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
        public double Original_Purchase_Price { get { return _ar.Original_Purchase_Price; } }
        public int ID { get { return _ar.ID; } }
        [Required]
        public double Number_of_Shares_to_Sell { get { return _ar.Number_of_Shares_to_Sell; } }
        [Required]
        public double Price_Per_Share_Being_Sold { get { return _ar.Price_Per_Share_Being_Sold; } }

        public double Total_Other_Costs { get { return _ar.Total_Other_Costs; } }

        public DateTime? Transaction_Close_Date { get { return _ar.Transaction_Close_Date; } }


        public double Budget_Amount { get { return _ar.Budget_Amount; } }
        public double Total_Cost { get { return _ar.Total_Cost; } }

        public string BudgetLineItem { get { return _ar.BudgetLineItem; } }
        public bool In_Budget { get { return _ar.In_Budget; } }
        public bool Funds_Committed { get { return _ar.Funds_Committed; } }
        public string ConsolidatedNonCon { get { return _ar.ConsolidatedNonCon; } }

        public double PercentageOfOwnership { get { return _ar.PercentageOfOwnership; } }

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