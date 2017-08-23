/*  IsActiveEmployeeActivity.cs

    ProgressTrackingActivity class for Nintex Workflow 2013 workflow activity.
 
    Copyright (c) 2015 – Nintex UK Ltd. All Rights Reserved.  
    This code released under the terms of the 
    Microsoft Reciprocal License (MS-RL, http://opensource.org/licenses/MS-RL.html.)

*/

using System;
using System.Workflow.ComponentModel;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using Nintex.Workflow;
using Nintex.Workflow.Activities;

namespace ActiveEmployeeAction
{
    public class IsActiveEmployeeActivity : ProgressTrackingActivity
    {
        // Add standard dependency properties. 
        #region Standard dependency properties
#pragma warning disable 618

        // The underscore properties (__ListItem) are necessary for the action to work within the Nintex Workflow context.

        public static DependencyProperty __ListItemProperty = DependencyProperty.Register("__ListItem", typeof(SPItemKey), typeof(IsActiveEmployeeActivity));
        public static DependencyProperty __ContextProperty = DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(IsActiveEmployeeActivity));
        public static DependencyProperty __ListIdProperty = DependencyProperty.Register("__ListId", typeof(string), typeof(IsActiveEmployeeActivity));

        // The next three properites map to the parameters used by the RESTcal in the WebCallAction class.
        // Add activity-specific dependency properties for the workflow activity.
        //public static DependencyProperty UserDomainProperty = DependencyProperty.Register("UserDomain", typeof(string), typeof(IsActiveEmployeeActivity));
        public static DependencyProperty UserAccountNameProperty = DependencyProperty.Register("UserAccountName", typeof(string), typeof(IsActiveEmployeeActivity));
        public static DependencyProperty IsActiveUserProperty = DependencyProperty.Register("IsActiveUser", typeof(bool), typeof(IsActiveEmployeeActivity));

#pragma warning restore 618

        // Add corresponding public properties.
        public SPItemKey __ListItem
        {
            get { return (SPItemKey)base.GetValue(__ListItemProperty); }
            set { SetValue(__ListItemProperty, value); }
        }
        public WorkflowContext __Context
        {
            get { return (WorkflowContext)base.GetValue(__ContextProperty); }
            set { SetValue(__ContextProperty, value); }
        }
        public string __ListId
        {
            get { return (string)base.GetValue(__ListIdProperty); }
            set { SetValue(__ListIdProperty, value); }
        }
        #endregion


        // The next three properites map to the parameters used by the RESTcal in the WebCallAction class.

        //public string UserDomain
        //{
        //    get { return (string)base.GetValue(IsActiveEmployeeActivity.UserDomainProperty); }
        //    set { base.SetValue(IsActiveEmployeeActivity.UserDomainProperty, value); }
        //}
        public string UserAccountName
        {
            get { return (string)base.GetValue(IsActiveEmployeeActivity.UserAccountNameProperty); }
            set { base.SetValue(IsActiveEmployeeActivity.UserAccountNameProperty, value); }
        }
        public bool IsActiveUser
        {
            get { return (bool)base.GetValue(IsActiveEmployeeActivity.IsActiveUserProperty); }
            set { base.SetValue(IsActiveUserProperty, value); }
        }


        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {
            // Confirm that the workflow activity is allowed to execute.
            ActivityActivationReference.IsAllowed(this, __Context.Web);

            // Get the workflow context for the workflow activity.
            NWWorkflowContext ctx = NWWorkflowContext.GetContext(
                __Context,
                new Guid(__ListId),
                __ListItem.Id,
                WorkflowInstanceId,
                this);

            // Report the start of the workflow activity.
            base.LogProgressStart(ctx);

            // In addind repurposing for your own code, the next two lines are used to pass context paramaters to the functional activity, in this case, Instantiation the WebCallAction class.

            //string resolvedUserAccountName = ctx.AddContextDataToString(this.UserAccountName); // context 
            //string resolvedUserDomain = ctx.AddContextDataToString(this.UserDomain); // context

            // Perform the workflow activity. This calls the class that contains the functionality being added to Nintex Workflow contaiend in the WebCallAction class. 
            // Note it is passing the two context parameters, and then a third (OutDef) receives the result of the functionality.

            string data_user = string.Empty;
            string data_domain = string.Empty;

            try
            {

                //var fieldValue = FieldValue.StartsWith("{") ? ctx.AddContextDataToString(FieldValue, true) : FieldValue; 
                string actualUserName = string.Empty;

                if (UserAccountName != null)
                {
                    actualUserName = UserAccountName.StartsWith("{") ? ctx.AddContextDataToString(UserAccountName, true) : UserAccountName;
                }

                if (string.IsNullOrEmpty(actualUserName))
                {
                    //Empty parameter, so return false
                    this.IsActiveUser = false;
                    // Report the successful execution of the workflow activity.
                    base.LogProgressEnd(ctx, executionContext);
                    // Set the execution status of the workflow activity.
#pragma warning disable 618
                    return ActivityExecutionStatus.Closed;
#pragma warning disable 618

                }

                GetUserNameAndDomainFromPersonFieldString(actualUserName, out data_domain, out data_user);

                Nintex.Workflow.Diagnostics.EventLogger.Log("Checking Employee is active for " + data_domain + "/" + data_user,
                    Microsoft.SharePoint.Administration.TraceSeverity.Verbose, "General");

                ActiveEmployeeActionAdapter employeeAction = new ActiveEmployeeActionAdapter();

                //bool outResult = UserHelper.IsEmployeeActive(actualUserName, actualUserDomain);
                bool outResult = UserHelper.IsEmployeeActive(data_user, data_domain);

                this.IsActiveUser = outResult;

                Nintex.Workflow.Diagnostics.EventLogger.Log("Result for " + data_domain + "/" + data_user + " is " + outResult.ToString(),
                    Microsoft.SharePoint.Administration.TraceSeverity.Verbose, "General");
            }
            catch
            {
                Nintex.Workflow.Diagnostics.EventLogger.Log("An Error occured while Checking Employee is active for " + data_domain + "/" + data_user,
                    Microsoft.SharePoint.Administration.TraceSeverity.Verbose, "General");
                // If an exception occurs that should stop the workflow, 
                // then throw the error.
                this.IsActiveUser = false;                
            }

            // Report the successful execution of the workflow activity.
            base.LogProgressEnd(ctx, executionContext);

            // Set the execution status of the workflow activity.
#pragma warning disable 618
            return ActivityExecutionStatus.Closed;
#pragma warning restore 618
        }

        static void GetUserNameAndDomainFromPersonFieldString(string personFieldValue, out string domain, out string sAMAccountName)
        {
            Nintex.Workflow.Diagnostics.EventLogger.Log("Retrieving Domain and AccountName for person field value " + personFieldValue,
                Microsoft.SharePoint.Administration.TraceSeverity.Verbose, "General");

            domain = string.Empty;
            sAMAccountName = string.Empty;
            if (personFieldValue.Contains("|"))
            {
                string accountName = personFieldValue.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[1];
                string[] data = accountName.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length == 2)
                {
                    domain = data[0];
                    sAMAccountName = data[1];
                }
            }
            else if (personFieldValue.Contains("\\"))
            {
                Nintex.Workflow.Diagnostics.EventLogger.Log("Domain and AccountName received in Domain\\AccountName format" + personFieldValue,
                Microsoft.SharePoint.Administration.TraceSeverity.Verbose, "General");
                string[] data = personFieldValue.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length > 1)
                {
                    domain = data[0];
                    sAMAccountName = data[1];
                }
            }
        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext,
            Exception exception)
        {
            // TODO: Provide activity-specific text introducing the exception.
            string activityErrorIntro = "Error in IsActiveEmployeeActivity";

            Nintex.Workflow.Diagnostics.ActivityErrorHandler.HandleFault(
                executionContext,
                exception,
                this.WorkflowInstanceId,
                activityErrorIntro,
                __ListItem.Id,
                __ListId,
                __Context);

            return base.HandleFault(executionContext, exception);
        }
    }
}
