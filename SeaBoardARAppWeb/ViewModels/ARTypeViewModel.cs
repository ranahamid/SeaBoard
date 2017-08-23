using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NSBD.SharepointAutoMapper;
using Microsoft.SharePoint;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using SB.AR.AppWeb.Models;

namespace SB.AR.AppWeb.ViewModels
{
    //[SharePointContextFilter]
    public class ARTypeViewModel : ViewModelBase
    {
        private SharePointContext _spContext = null;
        
        public ARTypeViewModel(SharePointContext spContext)
        {
            _spContext = spContext;
        }

        public List<SelectListItem> GetArTypes()
        {
            List<SelectListItem> choices = new List<SelectListItem>();
            if (null != HttpContext.Current.Session["ARTypeCollection"])
            {
                return (List<SelectListItem>)HttpContext.Current.Session["ARTypeCollection"];
            }
            //Field
            try
            {                
                using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        clientContext.ExecutingWebRequest += ctx_ExecutingWebRequest;
                        List arTypeList = clientContext.Web.Lists.GetByTitle(SPListMeta.ARTYPEDESCRIPTION);
                        Field choiceField = arTypeList.Fields.GetByInternalNameOrTitle(SPListMeta.ARType);
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
                                    choices.Add(new SelectListItem { Value = val, Text = choice });
                                }

                            }
                        }
                    }
                    HttpContext.Current.Session["ARTypeCollection"] = choices;
                }                
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Get AR Type or SP Context is null");
            }
            return choices;
        }

        public List<ARType> GetAllArTypes()
        {
            List<ARType> arTypes = new List<ARType>();
            if (null != HttpContext.Current.Session["ARTypeDescriptionCollection"])
            {
                return (List<ARType>)HttpContext.Current.Session["ARTypeDescriptionCollection"];
            }
            //Field
            try
            {
                using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        List arTypeList = clientContext.Web.Lists.GetByTitle(SPListMeta.ARTYPEDESCRIPTION);
                       
                        

                        CamlQuery query= new CamlQuery();
                        query.ViewXml = string.Format(@" <ViewFields>
                                          <FieldRef Name='Title' />
                                          <FieldRef Name='Type_x0020_Description' />
                                          <FieldRef Name='AR_Type' />
                                          <FieldRef Name='ID' />
                                       </ViewFields>");
                        var arItem = arTypeList.GetItems(query);
                        clientContext.Load(arItem);
                        clientContext.ExecuteQuery();
                    


                        arTypes = arItem.ProjectToListEntity<Models.ARType>();
                        HttpContext.Current.Session["ARTypeDescriptionCollection"] = arTypes;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Get AR Type or SP Context is null");
            }
            return arTypes;
        }
    }
}