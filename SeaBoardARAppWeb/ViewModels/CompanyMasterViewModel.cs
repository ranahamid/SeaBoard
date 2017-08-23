using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NSBD.SharepointAutoMapper;
namespace SB.AR.AppWeb.ViewModels
{
    public class CompanyMasterViewModel:ViewModelBase
    {

        public CompanyMasterViewModel(SharePointContext spContext)
            : base(spContext)
        {
            
        }

        public int CompanyId { get; set; }

        public string Title { get; set; }

        public int DivisionId { get; set; }
        public List<SelectListItem> CompaniesByDivision
        {
            get
            {
                //Field
                List<SelectListItem> _companies = new List<SelectListItem>();
                string key = "_companies";

                if(DivisionId >0)
                    key = key + DivisionId;

                if (null != HttpContext.Current.Session[key])
                {
                    return (List<SelectListItem>)HttpContext.Current.Session[key];
                }
                using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.Company);
                        CamlQuery query = new CamlQuery
                        {
                            ViewXml = string.Format(@"<ViewFields><FieldRef Name='Title' /><FieldRef Name='ID' /></ViewFields>")
                        };
                        var arItems = arList.GetItems(query);

                        clientContext.Load(arItems);
                        clientContext.ExecuteQuery();
                        var listCategories = arItems.ProjectToListEntity<AR.AppWeb.Models.Company>();

                        if (DivisionId > 0)
                        {
                            listCategories = listCategories.Where(m => m.Division != null).Where(l => l.Division.ID == DivisionId).ToList();
                        }


                        foreach (var c in listCategories)
                        {
                            _companies.Add(new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                        }
                    }
                    HttpContext.Current.Session[key] = _companies;
                }


                return _companies;
            }

        }
    }
}