using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using arapp = SB.AR.AppWeb.Models;


namespace SB.AR.AppWeb.ViewModels
{
    public class DashboardViewModel:ViewModelBase
    {
        private arapp.AR _ar;
        public DashboardViewModel(SharePointContext spContext, arapp.AR ar)
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

        public List<arapp.AR> MyARs { get; set; }
        public List<arapp.WorkFlow> MyApprovals { get; set; }
        public List<arapp.AR> PendingReviews { get; set; }
    }
}