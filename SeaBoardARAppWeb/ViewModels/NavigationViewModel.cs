using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using arapp = SB.AR.AppWeb.Models;


namespace SB.AR.AppWeb.ViewModels
{
    public class NavigationViewModel : ViewModelBase
    {
        private arapp.AR spar;
        public NavigationViewModel()
        {
            
        }
        public NavigationViewModel(SharePointContext spContext, arapp.AR ar)
            : base(spContext)
        {
            if (ar == null)
                ar = new arapp.AR();
            spar = ar;
        }

        public arapp.AR AR
        {
            get
            {
                return spar;
            }

        }
        public IEnumerable<AR.AppWeb.Models.NavigationAR> Navigation { get; set; }

        public string HostUrl
        {
            get;
            set;
            //get
            //{
            //    if (this.AR != null && this.SPHostUrl!=null)
            //    {
            //        return this.SPHostUrl;
            //    }

            //    else if (HttpContext.Current.Request["SPHostUrl"] != null)
            //    { 
            //        return HttpContext.Current.Request["SPHostUrl"].ToString(); 
            //    }

            //     return null;


            //}
        }
    }
}