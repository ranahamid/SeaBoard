using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NSBD.SharepointAutoMapper;

namespace SB.AR.AppWeb.ViewModels
{
    public class ARViewModel : ViewModelBase
    {
        private SharePointContext _spContext = null;
        public ARViewModel()
        { 
        }
        public ARViewModel(SharePointContext spContext)
        {
            _spContext = spContext;
            //AR = this.GetARDetail("111");
        }
        public ARTypeViewModel ARTypeViewModel { get; set; }
        public SB.AR.AppWeb.Models.AR AR { get; set; }
        public string AR_Type { get; set; }
        public SB.AR.AppWeb.Models.AR GetARDetail(string arId)
        {
            SB.AR.AppWeb.Models.AR arDetail = new Models.AR();
            try
            {
               
                using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        List arList = clientContext.Web.Lists.GetByTitle(SPListMeta.AR);
                        CamlQuery query = new CamlQuery();
                        query.ViewXml = string.Format(@"<View>  
                                <Query>
                                    <Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>{0}</Value></Eq></Where>
                                </Query> 
                                <ViewFields>
                                        <FieldRef Name='Title' />\
                                        <FieldRef Name='ID' />\
                                        <FieldRef Name='AR__x0023_' />\
                                        <FieldRef Name='AR_ID' />
                                        <FieldRef Name='AR_Type' />\
                                        <FieldRef Name='Current_Status' />
                                        <FieldRef Name='Division' />\
                                        <FieldRef Name='Company_Name' />\
                                        <FieldRef Name='Total_Cost' />
                                </ViewFields>
                                <RowLimit>1</RowLimit> 
                                </View>", arId);
                        var arItem = arList.GetItems(query);
                        clientContext.Load(arItem);
                        clientContext.ExecuteQuery();
                        arDetail = arItem.ProjectToListEntity<Models.AR>().FirstOrDefault();
                        //arListItem.Company_Name = "XYZ";
                        // Need to add the code for the look up fields 

                    }
                }               
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "Get AR Detail or SP Context is null");
            }
            return arDetail;
        }
    }
}