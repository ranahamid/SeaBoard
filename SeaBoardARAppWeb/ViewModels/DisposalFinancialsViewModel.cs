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
    public class DisposalFinancialsViewModel : ViewModelBase
    {
        
        private arapp.AR _ar;
        public DisposalFinancialsViewModel(SharePointContext spContext, arapp.AR ar):base(spContext)
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

        public bool IsDisposalFinance
        {
            get
            {
                return true;
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
        public double Original_Cost { get { return _ar.Original_Cost; } }
        public double Market_Value { get { return _ar.Market_Value; } }
        public double Current_Net_Book_Value { get { return _ar.Current_Net_Book_Value; } }
        [Required]
        public double Expected_Proceeds { get { return _ar.Expected_Proceeds; } }

        public DateTime? Date_Aquired { get { return _ar.Date_Aquired; } }
        public string Department_to_Charge { get { return _ar.Department_to_Charge; } }
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

        [Required]
        public string Condition_of_Assets {
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