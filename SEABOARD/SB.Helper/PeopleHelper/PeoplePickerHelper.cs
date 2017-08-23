using Microsoft.SharePoint.ApplicationPages.ClientPickerQuery;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace ACI.CreativeSvcs.SP.ServiceRequestAppWeb
{
    public class PeoplePickerHelper
    {
        private static int GroupID = -1;

        public static string GetPeoplePickerSearchData()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext.Current);
            using (var context = spContext.CreateUserClientContextForSPHost())
            {
                return GetPeoplePickerSearchData(context);
            }
        }

        public static string GetPeoplePickerSearchData(ClientContext context)
        {
            //get searchstring and other variables
            var searchString = (string)HttpContext.Current.Request["SearchString"];
            int principalType = Convert.ToInt32(HttpContext.Current.Request["PrincipalType"]);
            string spGroupName = (string)HttpContext.Current.Request["SPGroupName"];

            ClientPeoplePickerQueryParameters querryParams = new ClientPeoplePickerQueryParameters();
            querryParams.AllowMultipleEntities = false;
            querryParams.MaximumEntitySuggestions = 2000;
            querryParams.PrincipalSource = PrincipalSource.All;
            querryParams.PrincipalType = (PrincipalType)principalType;
            querryParams.QueryString = searchString;

            if (!string.IsNullOrEmpty(spGroupName))
            {
                if (PeoplePickerHelper.GroupID == -1)
                {
                    var group = context.Web.SiteGroups.GetByName(spGroupName);
                    if (group != null)
                    {
                        context.Load(group, p => p.Id);
                        context.ExecuteQuery();

                        PeoplePickerHelper.GroupID = group.Id;

                        querryParams.SharePointGroupID = group.Id;
                    }
                }
                else
                {
                    querryParams.SharePointGroupID = PeoplePickerHelper.GroupID;
                }
            }

            //execute query to Sharepoint
            ClientResult<string> clientResult = Microsoft.SharePoint.ApplicationPages.ClientPickerQuery.ClientPeoplePickerWebServiceInterface.ClientPeoplePickerSearchUser(context, querryParams);
            context.ExecuteQuery();
            return clientResult.Value;
        }

        public static void FillPeoplePickerValue(HiddenField peoplePickerHiddenField, Microsoft.SharePoint.Client.User user)
        {
            List<PeoplePickerUser> peoplePickerUsers = new List<PeoplePickerUser>(1);
            peoplePickerUsers.Add(new PeoplePickerUser() { Name = user.Title, Email = user.Email, Login = user.LoginName });
            peoplePickerHiddenField.Value = JsonHelper.Serialize<List<PeoplePickerUser>>(peoplePickerUsers);
        }

        public static void FillPeoplePickerValue(HiddenField peoplePickerHiddenField, Microsoft.SharePoint.Client.User[] users)
        {
            List<PeoplePickerUser> peoplePickerUsers = new List<PeoplePickerUser>();
            foreach (var user in users)
            {
                peoplePickerUsers.Add(new PeoplePickerUser() { Name = user.Title, Email = user.Email, Login = user.LoginName });
            }
            peoplePickerHiddenField.Value = JsonHelper.Serialize<List<PeoplePickerUser>>(peoplePickerUsers);
        }

        public static List<PeoplePickerUser> GetValuesFromPeoplePicker(HiddenField peoplePickerHiddenField)
        {
            return JsonHelper.Deserialize<List<PeoplePickerUser>>(peoplePickerHiddenField.Value);
        }

        public static User SPEnsueruser(ClientContext clientContext, Int32 userID)
        {
            User user = null;
            try
            {

                user = clientContext.Web.GetUserById(userID);
                clientContext.Load(user);
                clientContext.ExecuteQuery();
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "SPEnsueruser Get UserEmail Address");
            }

            return user;
        }
        public static List<string> SPEnsureMultiUserEmailAddress(ClientContext clientContext, FieldUserValue[] userIds)
        {
            List<string> users = null;
            try
            {
                if(userIds!= null && userIds.Length > 0)
                {
                    users = new List<string>();
                    User user = null;
                    for(Int32 i =0;i< userIds.Length;i++)
                    {
                        Int32 userID =userIds[i].LookupId;

                        user = clientContext.Web.GetUserById(userID);
                        clientContext.Load(user);
                        clientContext.ExecuteQuery();
                        if(user != null && !string.IsNullOrEmpty(user.Email))
                        {
                            if(!users.Contains(user.Email))
                            {
                                users.Add(user.Email);
                            }
                        }
                    }
                    
                }

               
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "SPEnsureMultiUserEmailAddress Get UserEmail Address");
            }

            return users;
        }
        public static Group SPEnsuerGroup(ClientContext clientContext, string groupName)
        {
            Group group = null;
            try
            {

                group = clientContext.Web.SiteGroups.GetByName(groupName);
                clientContext.Load(group.Users);
                clientContext.ExecuteQuery();

            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "SPEnsuerGroup Get Group Members");
            }

            return group;
        }
        public static FieldUserValue SPEnsureUser(ClientContext clientContext, string userjosn)
        {
            FieldUserValue field = null;
            try
            {

                var uservalue = JsonHelper.Deserialize<List<PeoplePickerUser>>(userjosn);
                User user = null;
                if (uservalue[0].Login.Split('|').Length == 1)
                {
                    user = clientContext.Web.EnsureUser(uservalue[0].Login.Split('|')[0]);
                }
                else
                {
                    user = clientContext.Web.EnsureUser(uservalue[0].Login.Split('|')[1]);
                }

                clientContext.Load(user);
                clientContext.ExecuteQuery();
                field = new FieldUserValue();
                field.LookupId = user.Id;
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "SPEnsureUser");
            }



            return field;


        }


        public static FieldUserValue[] SPEnsureMultiUser(ClientContext clientContext, string userjosn)
        {
            FieldUserValue[] fields = null;
            try
            {
                if (!string.IsNullOrEmpty(userjosn))
                {
                    var uservalues = JsonHelper.Deserialize<List<PeoplePickerUser>>(userjosn);
                    fields = new FieldUserValue[uservalues.Count];

                    for (int i = 0; i < uservalues.Count; i++)
                    {
                        FieldUserValue field = null;

                        User user = null;
                        if (uservalues[i].Login.Split('|').Length == 1)
                        {
                            user = clientContext.Web.EnsureUser(uservalues[i].Login.Split('|')[0]);
                        }
                        else
                        {
                            user = clientContext.Web.EnsureUser(uservalues[i].Login.Split('|')[1]);
                        }

                        clientContext.Load(user);
                        clientContext.ExecuteQuery();
                        field = new FieldUserValue();
                        field.LookupId = user.Id;
                        fields[i] = new FieldUserValue();
                        fields[i] = field;

                    }
                    return fields;
                }
            }

            catch(Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "SPEnsureMultiUser");
            }


            return fields;


        }

        public static string GetSPUserJson(ClientContext clientContext, FieldUserValue field)
        {
            try
            {
                if (field != null)
                {
                    List<PeoplePickerUser> peoplePickerUsers = new List<PeoplePickerUser>();
                    User user = clientContext.Web.GetUserById(field.LookupId);
                    clientContext.Load(user);
                    clientContext.ExecuteQuery();
                    peoplePickerUsers.Add(new PeoplePickerUser() { Name = user.Title, Email = user.Email, Login = user.LoginName });
                    return JsonHelper.Serialize<List<PeoplePickerUser>>(peoplePickerUsers);
                }
            }
            catch (Exception ex)
            {
                Utility.Logging.LogErrorException(ex, "SPEnsureUser");
            }

            return string.Empty;
        }

        public static string GetSPMultiUserJson(ClientContext clientContext, FieldUserValue[] field)
        {
            if (field != null && field.Count() > 0)
            {
                List<PeoplePickerUser> peoplePickerUsers = new List<PeoplePickerUser>();
                for (int i = 0; i < field.Count(); i++)
                {
                    User user = clientContext.Web.GetUserById(field[i].LookupId);
                    clientContext.Load(user);
                    clientContext.ExecuteQuery();
                    peoplePickerUsers.Add(new PeoplePickerUser() { Name = user.Title, Email = user.Email, Login = user.LoginName });
                }
                return JsonHelper.Serialize<List<PeoplePickerUser>>(peoplePickerUsers);
            }
            return string.Empty;
        }
    }
}
