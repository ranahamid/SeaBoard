using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using SB.AR.AppWeb.Models;
using SB.AR.AppWeb.Controllers;

namespace SB.AR.AppWeb.Reports
{
    public partial class ReportViewerPage : System.Web.UI.Page
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                try
                {
                    
                    if (Session["AR"] != null)
                    {
                        //logger.Info("Report load-ReportViewerPage-Page_Load()");
                        ObjectDataSource ods = new ObjectDataSource();
                        ods.TypeName = "SB.AR.AppWeb.Models.ARReportSetWrapper";
                        ods.SelectMethod = "GetAR";                        

                        ReportDataSource rds = new ReportDataSource();
                        rds.Name = "DataSet1";
                        rds.Value = ods;
                        ReportViewer1.LocalReport.DataSources.Add(rds);

                        this.ReportViewer1.LocalReport.Refresh();
                    }
                    else
                    {
                        //logger.Info("Report load session AR null-ReportViewerPage-Page_Load()");
                        ReportViewer1.Visible = false;
                        lblReportMessage.Text = "AR data not available";
                    }
                    

                }
                catch (Exception ex)
                {
                    lblReportMessage.Text = "System encountered an error while trying to generate print preview for AR";
                    logger.Error("Report load Exception-ReportViewerPage-Page_Load()", ex);
                }




            }
        }

        protected void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            //logger.Info("Start LocalReport_SubreportProcessing - ReportViewerPage-LocalReport_SubreportProcessing()");

            LocalReport lr = (LocalReport)sender;

            //logger.Info("LOADING SUBREPORT " + e.ReportPath + " - ReportViewerPage-LocalReport_SubreportProcessing()");

            if (e.ReportPath.Equals("ARAttachmentReport") || e.ReportPath.Contains("ARAttachmentReport"))
            {
                //logger.Info("ARAttachmentReport - ReportViewerPage-LocalReport_SubreportProcessing()");
                ReportAttachments ra = new ReportAttachments();
                e.DataSources.Add(new ReportDataSource("DataSet2", ra.GetAttachments()));
            }
            else if (e.ReportPath.Equals("ARDiscussionReport") || e.ReportPath.Contains("ARDiscussionReport"))
            {
                //logger.Info("ARDiscussionReport - ReportViewerPage-LocalReport_SubreportProcessing()");
                ReportDiscussions rd = new ReportDiscussions();
                e.DataSources.Add(new ReportDataSource("DataSet3", rd.GetDiscussionEntries()));
            }

            //logger.Info("Stop LocalReport_SubreportProcessing - ReportViewerPage-LocalReport_SubreportProcessing()");
        }
    }
}