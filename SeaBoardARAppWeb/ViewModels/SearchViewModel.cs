using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arapp = SB.AR.AppWeb.Models;
namespace SB.AR.AppWeb.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private arapp.AR _ar;
        public SearchViewModel(SharePointContext spContext, arapp.AR ar)
            : base(spContext)
        {
            if (ar == null)
                ar = new arapp.AR();
            _ar = ar;
        }

        public SharePointContext SharePointContext
        {
            get
            {
                return base._spContext;
            }
        }

        public List<SelectListItem> StatusChoices
        {
            get
            {
                //Field
                List<SelectListItem> _statuschoices = new List<SelectListItem>();

                if (null != HttpContext.Current.Session["_statuschoices"])
                {

                    return (List<SelectListItem>)HttpContext.Current.Session["_statuschoices"];
                }

                using (var clientContext = this._spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);



                        var currentStatusField = arList.Fields.GetByTitle("Current_Status");
                        FieldChoice currentStatusChoice = clientContext.CastTo<FieldChoice>(currentStatusField);

                        clientContext.Load(currentStatusField);
                        clientContext.Load(currentStatusChoice);

                        clientContext.ExecuteQuery();



                        foreach (var c in currentStatusChoice.Choices)
                        {
                            _statuschoices.Add(new SelectListItem { Value = c, Text = c });
                        }
                    }
                    HttpContext.Current.Session["_statuschoices"] = _statuschoices;
                }
                return _statuschoices;
            }

        }


        public string KeywordString { get; set; }
        public string DivisionString { get; set; }
        public string CompanyString { get; set; }
        public string CategoryString { get; set; }
        public string PMOwner { get; set; }
        public string CreatedBy { get; set; }

        public double? AmountFrom { get; set; }
        public double? AmountTo { get; set; }
        public DateTime? SubmittedFrom { get; set; }
        public DateTime? SubmittedTo { get; set; }
        public string Status { get; set; }

        public string LookupValue
        {
            get
            {
                return _ar.LookupValue;
            }
        }

        public int LookupId
        {
            get
            {
                return _ar.LookupId;
            }
        }



        public int? CompanyId
        {
            get
            {
                if (this._ar.Company_Name == null || _ar == null)
                    return 0;
                return this._ar.Company_Name.ID;
            }
        }

        public int? DivisionId
        {
            get
            {
                if (this._ar.Division == null || _ar == null)
                    return 0;
                return this._ar.Division.ID;
            }
        }

    }
}