<%-- 
    ActiveEmployeeActionDialog.aspx
    
    Nintex Workflow 2013 configuration page for custom workflow actions

    Copyright (c) 2015 – Nintex UK Ltd. All Rights Reserved.  
    This code released under the terms of the 
    Microsoft Reciprocal License (MS-RL, http://opensource.org/licenses/MS-RL.html.)
--%>

<%-- Page directive required by Nintex Workflow 2013 --%>

<%@ Page Language="C#" DynamicMasterPageFile="~masterurl/default.master" AutoEventWireup="true" CodeBehind="ActiveEmployeeActionDialog.aspx.cs" EnableEventValidation="false"
    Inherits="ActiveEmployeeAction.ActiveEmployeeActionDialog, $SharePoint.Project.AssemblyFullName$" %>

<%-- Register directives required by Nintex Workflow 2013 --%>
<%@ Register TagPrefix="Nintex" Namespace="Nintex.Workflow.ServerControls"
    Assembly="Nintex.Workflow.ServerControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=913f6bae0ca5ae12" %>
<%@ Register TagPrefix="Nintex" Namespace="Nintex.Workflow.ApplicationPages"
    Assembly="Nintex.Workflow.ApplicationPages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=913f6bae0ca5ae12" %>
<%@ Register TagPrefix="Nintex" TagName="ConfigurationPropertySection" Src="~/_layouts/15/NintexWorkflow/ConfigurationPropertySection.ascx" %>
<%@ Register TagPrefix="Nintex" TagName="ConfigurationProperty" Src="~/_layouts/15/NintexWorkflow/ConfigurationProperty.ascx" %>
<%@ Register TagPrefix="Nintex" TagName="DialogLoad" Src="~/_layouts/15/NintexWorkflow/DialogLoad.ascx" %>
<%@ Register TagPrefix="Nintex" TagName="DialogBody" Src="~/_layouts/15/NintexWorkflow/DialogBody.ascx" %>
<%-- Place additional Register directives after this comment. --%>

<%@ Register TagPrefix="Nintex" TagName="SingleLineInput" Src="~/_layouts/15/NintexWorkflow/SingleLineInput.ascx" %>


<asp:content id="ContentHead" contentplaceholderid="PlaceHolderAdditionalPageHead" runat="server">
    <%-- The DialogLoad control must be the first child of this Content control. --%>
    <Nintex:DialogLoad runat="server" />

    <%-- JavaScript functions for reading and writing configuration data. --%>
    <script type="text/javascript" language="javascript">
        function TPARetrieveConfig() {
            // Use this JavaScript function to retrieve configuration settings from
            // the configuration XML and set the values of the corresponding controls
            // on the configuration page.
            var drpOutDef = document.getElementById("<%= IsActiveUserProperty.ClientID %>");
            drpOutDef.value = configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='IsActiveUser']/Variable/@Name").text;

            var drpOutDef2 = document.getElementById("<%= UserAccountNameProperty.ClientID %>");
            drpOutDef2.value = configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='UserAccountName']/Variable/@Name").text;
            
        }
         
        function TPAWriteConfig() {
            // Use this JavaScript function to retrieve configuration settings from
            // controls on the configuration page and set the values of the 
            // corresponding elements in the configuration XML.
            var drpOutDef = document.getElementById("<%= IsActiveUserProperty.ClientID %>");
            var drpOutDef2 = document.getElementById("<%= UserAccountNameProperty.ClientID %>");
            configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='IsActiveUser']/Variable/@Name").text = drpOutDef.value;
            configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='UserAccountName']/Variable/@Name").text = drpOutDef2.value;
            return true; 
        }

        // Register the ConfigurationPropertySection on the page.
        // The dialogSectionsArray determines what sections are displayed in the
        // Configure Action configuration settings dialog. 
        // Set the value to true to initially show the section when the Action button
        // is toggled on; otherwise, set it to false to initially hide the section.
        onLoadFunctions[onLoadFunctions.length] = function () {
            dialogSectionsArray["<%= MainControls1.ClientID %>"] = true;
        };
    </script>
</asp:content>

<asp:content id="ContentBody" contentplaceholderid="PlaceHolderMain" runat="Server">
    <Nintex:ConfigurationPropertySection runat="server" Id="MainControls1">
        <TemplateRowsArea>
            <Nintex:ConfigurationProperty runat="server" FieldTitle="User" RequiredField="True">
                   <TemplateControlArea>                       
                       <Nintex:VariableSelector  ID="UserAccountNameProperty" runat="server" IncludeUserVars="true"></Nintex:VariableSelector>
                  </TemplateControlArea>
                </Nintex:ConfigurationProperty>
            <Nintex:ConfigurationProperty runat="server" FieldTitle="Store Value In" RequiredField="True">
                <TemplateControlArea>
                    <Nintex:VariableSelector  ID="IsActiveUserProperty" runat="server" IncludeBooleanVars="true"></Nintex:VariableSelector>
                </TemplateControlArea>
              </Nintex:ConfigurationProperty>
        </TemplateRowsArea>
    </Nintex:ConfigurationPropertySection>
    
    <%-- The DialogBody control must be the last child of this Content control. --%>    
    <Nintex:DialogBody runat="server" id="DialogBody" />
</asp:content>
