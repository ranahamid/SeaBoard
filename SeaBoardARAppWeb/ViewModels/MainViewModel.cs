using Microsoft.SharePoint.Client;
using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using model = SB.AR.AppWeb.Models;
using arapp = SB.AR.AppWeb.Models;
namespace SB.AR.AppWeb.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private arapp.AR _ar;
        public MainViewModel(SharePointContext spContext, arapp.AR ar)
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

      //  public LookupFieldMapper Division{ get;set;}

    }
}