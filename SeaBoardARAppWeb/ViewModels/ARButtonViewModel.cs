using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using arapp = SB.AR.AppWeb.Models;
namespace SB.AR.AppWeb.ViewModels
{
    public class ARButtonViewModel : ViewModelBase
    {

        private arapp.AR _ar;
        public ARButtonViewModel(SharePointContext spContext, arapp.AR ar)
            : base(spContext)
        {
            _ar = ar;
        }

        public SharePointContext SharePointContext
        {
            get
            {
                return base._spContext;
            }
        }

        public arapp.AR AR
        {
            get;
            set;

        }
        
        public User CurrentUser
        {
            get
            {
                User spUser = null;
                using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        spUser = clientContext.Web.CurrentUser;
                        clientContext.Load(spUser);
                        clientContext.ExecuteQuery();
                    }
                }
                return spUser;
            }
        }
        public bool IsEditable
        {
            get
            {               
                      if (AR != null && AR.Author != null && AR.Author.Value.Trim().ToLower() 
                    == CurrentUser.Title.Trim().ToLower()
                    && (AR.IsRejected || AR.Current_Status == "Pending Edits"))
                   
                        return true;

                if (AR != null && AR.PMUser != null && AR.PMUser.LookupValue !=null && AR.PMUser.LookupValue.Trim().ToLower()
                    == CurrentUser.Title.Trim().ToLower()
                    && (AR.IsRejected || AR.Current_Status == "Pending Edits"))
                   
                    return true;
                
                return false;
            }
        }


    }
}