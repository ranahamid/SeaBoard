/*  ActiveEmployeeActionAdapter.cs

    Copyright (c) 2015 – Nintex UK Ltd. All Rights Reserved.  
    This code released under the terms of the 
    Microsoft Reciprocal License (MS-RL, http://opensource.org/licenses/MS-RL.html.)

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel;
using System.Collections;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WorkflowActions;

using Nintex.Workflow;
using Nintex.Workflow.Activities.Adapters;

namespace ActiveEmployeeAction
{
    /// <summary>
    /// The workflow action adapter for the NW2013Template workflow action.
    /// </summary>
    public class ActiveEmployeeActionAdapter : GenericRenderingAction
    {
        /// <summary>
        /// Get the default configuration for the workflow action.
        /// </summary>
        /// <param name="context">The context in which the method was invoked.</param>
        /// <returns>The default configuration for the workflow action.</returns>
        /// <remarks></remarks>
        /// 
        //private const string DomainPropertyName = "UserDomain";
        private const string UserPropertyName = "UserAccountName";
        private const string IsActivePropertyName = "IsActiveUser";

        public override NWActionConfig GetDefaultConfig(GetDefaultConfigContext context)
        {
            // Instantiate the NWActionConfig object that represents the default
            // configuration for the workflow action.
            NWActionConfig config = new NWActionConfig(this);

            // Build the default configuration for the workflow action, by
            // populating an array of ActivityParameters, each of which represents
            // a single activity parameter. 
            // Instantiate and configure an ActivityParameter object for each
            // dependency property.
            config.Parameters = new ActivityParameter[2];

            //config.Parameters[0] = new ActivityParameter();
            //config.Parameters[0].Name = DomainPropertyName;
            //config.Parameters[0].PrimitiveValue = new PrimitiveValue();
            //config.Parameters[0].PrimitiveValue.Value = string.Empty;
            //config.Parameters[0].PrimitiveValue.ValueType = SPFieldType.Text.ToString();

            //config.Parameters[0] = new ActivityParameter();
            //config.Parameters[0].Name = UserPropertyName;
            //config.Parameters[0].PrimitiveValue = new PrimitiveValue();
            //config.Parameters[0].PrimitiveValue.Value = string.Empty;
            //config.Parameters[0].PrimitiveValue.ValueType = SPFieldType.Text.ToString();
            config.Parameters[0] = new ActivityParameter();
            config.Parameters[0].Name = UserPropertyName;
            config.Parameters[0].Variable = new NWWorkflowVariable();

            // Note using NWWorkflowVariable is necessary for the API return to be stored in a Nintex Workflow variable set in the action configuration screen.
            config.Parameters[1] = new ActivityParameter();
            config.Parameters[1].Name = IsActivePropertyName;
            config.Parameters[1].Variable = new NWWorkflowVariable();

            // If this custom workflow action supports error handling or multiple output,
            // add any necessary code here.

            // Set the default top label text for the workflow action.
            config.TLabel = ActivityReferenceCollection.FindByAdapter(this).Name;

            // Return the default configuration.
            return config;
        }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <param name="context">The context in which the method was invoked.</param>
        /// <returns><b>true</b> if the configuration is valid; otherwise, <b>false</b>.</returns>
        /// <remarks></remarks>
        public override bool ValidateConfig(ActivityContext context)
        {
            bool isValid = true;

            // Prepare a keyed collection of ActivityParameterHelper objects.
            Dictionary<string, ActivityParameterHelper> parameters =
                context.Configuration.GetParameterHelpers();

            //if (!parameters[DomainPropertyName].Validate(typeof(string), context))
            //{
            //    isValid &= false;
            //    validationSummary.AddError("Domain", ValidationSummaryErrorType.CannotBeBlank);
            //}

            if (!parameters[UserPropertyName].Validate(typeof(string), context))
            {
                isValid &= false;
                validationSummary.AddError("User", ValidationSummaryErrorType.CannotBeBlank);
            }
            
            return isValid;
        }

        /// <summary>
        /// Instantiates and configures the workflow activity to be added to the workflow.
        /// </summary>
        /// <param name="context">The context in which the method was invoked.</param>
        /// <returns>A null reference for single actions, or a CompositeActivity object for composite actions.</returns>
        /// <remarks>For more information about the return value of this method, see the
        /// Nintex Workflow 2013 Software Development Kit documentation.</remarks>
        public override CompositeActivity AddActivityToWorkflow(PublishContext context)
        {
            // Prepare a keyed collection of ActivityParameterHelper objects.
            Dictionary<string, ActivityParameterHelper> parameters =
                context.Config.GetParameterHelpers();

            IsActiveEmployeeActivity activity = new IsActiveEmployeeActivity();

            //parameters[DomainPropertyName].AssignTo(activity, IsActiveEmployeeActivity.UserDomainProperty, context);
            parameters[UserPropertyName].AssignTo(activity, IsActiveEmployeeActivity.UserAccountNameProperty, context);
            parameters[IsActivePropertyName].AssignTo(activity, IsActiveEmployeeActivity.IsActiveUserProperty, context);

            activity.SetBinding(IsActiveEmployeeActivity.__ContextProperty, new ActivityBind(context.ParentWorkflow.Name, StandardWorkflowDataItems.__context));
            activity.SetBinding(IsActiveEmployeeActivity.__ListItemProperty, new ActivityBind(context.ParentWorkflow.Name, StandardWorkflowDataItems.__item));
            activity.SetBinding(IsActiveEmployeeActivity.__ListIdProperty, new ActivityBind(context.ParentWorkflow.Name, StandardWorkflowDataItems.__list));


            ActivityFlags f = new ActivityFlags();
            f.AddLabelsFromConfig(context.Config);
            f.AssignTo(activity);

            context.ParentActivity.Activities.Add(activity);


            return null;
        }

        /// <summary>
        /// Gets the current configuration from the workflow action.
        /// </summary>
        /// <param name="context">The context in which the method was invoked.</param>
        /// <returns>The current configuration.</returns>
        public override NWActionConfig GetConfig(RetrieveConfigContext context)
        {
            // Retrieve the default configuration, by invoking the GetDefaultConfig
            // method using the current context.
            NWActionConfig config = this.GetDefaultConfig(context);

            // Prepare a keyed collection of ActivityParameterHelper objects.
            Dictionary<string, ActivityParameterHelper> parameters = config.GetParameterHelpers();

            // TODO: For each property, retrieve and update the values in the configuration.
            //parameters[PropertyName].RetrieveValue(context.Activity, 
            //    ActivityClass.PropertyNameProperty, context);
            //parameters[DomainPropertyName].RetrieveValue(context.Activity, IsActiveEmployeeActivity.UserDomainProperty, context);
            parameters[UserPropertyName].RetrieveValue(context.Activity, IsActiveEmployeeActivity.UserAccountNameProperty, context);
            parameters[IsActivePropertyName].RetrieveValue(context.Activity, IsActiveEmployeeActivity.IsActiveUserProperty, context);

            // If this custom workflow action supports error handling or multiple output,
            // add any necessary code here.

            // Return the configuration.
            return config;
        }

        /// <summary>
        /// Gets the action summary from the workflow action, if the workflow action is 
        /// successfully configured.
        /// </summary>
        /// <param name="context">The context in which the method was invoked.</param>
        /// <returns>The action summary.</returns>
        /// <remarks>This method is invoked after the ValidateConfig method is 
        /// invoked to confirm that the workflow action is successfully configured.</remarks>
        public override ActionSummary BuildSummary(ActivityContext context)
        {
            string displayMessage = "";

            // Prepare a keyed collection of ActivityParameterHelper objects.
            Dictionary<string, ActivityParameterHelper> parameters = context.Configuration.GetParameterHelpers();
            
            displayMessage = string.Format("Check if '{0}' is an active user.", parameters[UserPropertyName].Value);

            // Return the action summary.
            return new ActionSummary(displayMessage);
        }
    }
}