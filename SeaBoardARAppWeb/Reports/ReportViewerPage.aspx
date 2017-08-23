<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewerPage.aspx.cs" Inherits="SB.AR.AppWeb.Reports.ReportViewerPage" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer  ID="ReportViewer1" runat="server" 
                Height="1200px" Width="99%"   SizeToReportContent="true"
                Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
                <LocalReport ReportPath="Reports\ARMainReport.rdlc" OnSubreportProcessing="LocalReport_SubreportProcessing" >
                    
                </LocalReport>

            </rsweb:ReportViewer>
            <asp:Label ID="lblReportMessage" runat="server" Text="" EnableViewState="false"></asp:Label>
        </div>
    </form>
</body>
</html>
