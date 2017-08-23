using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using arapp = SB.AR.AppWeb.Models;
using NSBD.SharepointAutoMapper;
using System.ComponentModel.DataAnnotations;

namespace SB.AR.AppWeb.ViewModels
{
    public class ViewModelBase
    {
        protected SharePointContext _spContext = null;
        public ViewModelBase()
        {
        }
        public ViewModelBase(SharePointContext spContext)
        {
            _spContext = spContext;
        }
        [Required]
        public string ARTypeID { get; set; }

        public void ctx_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {
            try
            {
                e.WebRequestExecutor.WebRequest.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                this.AccessToken = e.WebRequestExecutor.RequestHeaders.Get("Authorization");
            }
            catch
            { throw; }

        }
        public void RetrieveAccessToken()
        {
            ClientContext ctx = _spContext.CreateUserClientContextForSPHost();
            ctx.ExecutingWebRequest += ctx_ExecutingWebRequest;
            ctx.ExecuteQuery();
        }
        public string AccessToken { get; set; }
        public List<SelectListItem> Categories
        {
            get
            {
                //Field
                List<SelectListItem> categories = new List<SelectListItem>();

                if (null != HttpContext.Current.Session["Categories"])
                {
                    return (List<SelectListItem>)HttpContext.Current.Session["Categories"];
                }
                if (_spContext != null)
                {
                    using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                    {
                        if (clientContext != null)
                        {
                            List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.Category);
                            CamlQuery query = new CamlQuery
                            {
                                ViewXml = string.Format(@"<ViewFields><FieldRef Name='Title' /><FieldRef Name='ID' /></ViewFields>")
                            };
                            var arItems = arList.GetItems(query);

                            clientContext.Load(arItems);
                            clientContext.ExecuteQuery();
                            var listCategories = arItems.ProjectToListEntity<AR.AppWeb.Models.Category>();

                            foreach (var c in listCategories)
                            {
                                categories.Add(new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                            }
                        }
                        HttpContext.Current.Session["Categories"] = categories;
                    }
                }
                return categories;
            }

        }

        public List<SelectListItem> Division
        {
            get
            {
                //Field
                List<SelectListItem> divisions = new List<SelectListItem>();

                if (null != HttpContext.Current.Session["divisions"])
                {
                    return (List<SelectListItem>)HttpContext.Current.Session["divisions"];
                }
                if (_spContext != null)
                {
                    using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                    {
                        if (clientContext != null)
                        {
                            List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.Division);
                            CamlQuery query = new CamlQuery
                            {
                                ViewXml = string.Format(@"<ViewFields><FieldRef Name='Title' /><FieldRef Name='ID' /></ViewFields>")
                            };
                            var arItems = arList.GetItems(query);

                            clientContext.Load(arItems);
                            clientContext.ExecuteQuery();
                            var listCategories = arItems.ProjectToListEntity<AR.AppWeb.Models.Division>();

                            foreach (var c in listCategories)
                            {
                                divisions.Add(new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                            }
                        }
                        HttpContext.Current.Session["divisions"] = divisions;
                    }
                }
                return divisions;
            }

        }

        public List<SelectListItem> Companies
        {
            get
            {
                //Field
                List<SelectListItem> _companies = new List<SelectListItem>();

                if (null != HttpContext.Current.Session["_companies"])
                {
                    return (List<SelectListItem>)HttpContext.Current.Session["_companies"];
                }
                if (_spContext != null)
                {
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

                            foreach (var c in listCategories)
                            {
                                _companies.Add(new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                            }
                        }
                        HttpContext.Current.Session["_companies"] = _companies;
                    }
                }
                return _companies;
            }

        }

        public List<SelectListItem> InvestmentTypes
        {
            get
            {
                //Field
                List<SelectListItem> _companies = new List<SelectListItem>();

                if (null != HttpContext.Current.Session["Investment_Type"])
                {
                    return (List<SelectListItem>)HttpContext.Current.Session["Investment_Type"];
                }
                List<SelectListItem> types = new List<SelectListItem>();
                if (_spContext != null)
                {
                    using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                    {
                        if (clientContext != null)
                        {
                            List arTypeList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);
                            Field choiceField = arTypeList.Fields.GetByInternalNameOrTitle("Investment_Type");
                            clientContext.Load(choiceField);
                            clientContext.ExecuteQuery();
                            if (choiceField.FieldTypeKind == FieldType.Choice)
                            {
                                FieldChoice myChoices = clientContext.CastTo<FieldChoice>(choiceField);
                                foreach (string choice in myChoices.Choices)
                                {
                                    if (choice.ToLower().Trim() != "draft")
                                    {
                                        var val = !string.IsNullOrEmpty(choice) ? choice.Replace(" ", "").ToLower() : "";
                                        types.Add(new SelectListItem { Value = val, Text = choice });
                                    }

                                }
                            }
                        }
                        HttpContext.Current.Session["Investment_Type"] = types;
                    }
                }
                return types;
            }
        }
        public List<SelectListItem> LeaseTypes
        {
            get
            {
                //Field
                List<SelectListItem> _companies = new List<SelectListItem>();

                if (null != HttpContext.Current.Session["LeaseType"])
                {
                    return (List<SelectListItem>)HttpContext.Current.Session["Lease_Type"];
                }
                List<SelectListItem> types = new List<SelectListItem>();
                using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        List arTypeList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);
                        Field choiceField = arTypeList.Fields.GetByInternalNameOrTitle("Lease_Type");
                        clientContext.Load(choiceField);
                        clientContext.ExecuteQuery();
                        if (choiceField.FieldTypeKind == FieldType.Choice)
                        {
                            FieldChoice myChoices = clientContext.CastTo<FieldChoice>(choiceField);
                            foreach (string choice in myChoices.Choices)
                            {
                                if (choice.ToLower().Trim() != "draft")
                                {
                                    var val = !string.IsNullOrEmpty(choice) ? choice.Replace(" ", "").ToLower() : "";
                                    types.Add(new SelectListItem { Value = val, Text = choice });
                                }

                            }
                        }
                    }
                    HttpContext.Current.Session["Lease_Type"] = types;
                }
                return types;
            }
        }

        public List<SelectListItem> ConsolidatedNonCons
        {
            get
            {
                //Field
               

                if (null != HttpContext.Current.Session["ConsolidatedNonCons"])
                {
                    return (List<SelectListItem>)HttpContext.Current.Session["ConsolidatedNonCons"];
                }
                List<SelectListItem> types = new List<SelectListItem>();
                using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        List arTypeList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);
                        Field choiceField = arTypeList.Fields.GetByInternalNameOrTitle("Consolidated_x002F_Non_x002d_Con");
                        clientContext.Load(choiceField);
                        clientContext.ExecuteQuery();
                        if (choiceField.FieldTypeKind == FieldType.Choice)
                        {
                            FieldChoice myChoices = clientContext.CastTo<FieldChoice>(choiceField);
                            foreach (string choice in myChoices.Choices)
                            {
                                if (choice.ToLower().Trim() != "draft")
                                {
                                    var val = !string.IsNullOrEmpty(choice) ? choice.Replace(" ", "").ToLower() : "";
                                    types.Add(new SelectListItem { Value = val, Text = choice });
                                }

                            }
                        }
                    }
                    HttpContext.Current.Session["ConsolidatedNonCons"] = types;
                }
                return types;
            }
        }


    }
}