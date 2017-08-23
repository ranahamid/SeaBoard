using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using arapp = SB.AR.AppWeb.Models;

namespace SB.AR.AppWeb.ViewModels
{
    public class FinancialsViewModel : ViewModelBase
    {
        private arapp.AR _ar;
        public FinancialsViewModel(SharePointContext spContext, arapp.AR ar) : base(spContext)
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
        public string AR_Type { get { return _ar.AR_Type; } }
        public int ID { get { return _ar.ID; } }
        public double? AR_ID { get { return _ar.AR_ID; } }
        public ChoiceFieldMapper Current_Status { get; set; }
        public double Total_Cost { get { return _ar.Total_Cost; } }
        public LookupFieldMapper Company_Name { get { return _ar.Company_Name; } }
       // public LookupFieldMapper Division { get; set; }
    }
}