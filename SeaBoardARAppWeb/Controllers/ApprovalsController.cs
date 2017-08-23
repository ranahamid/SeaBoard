using SB.AR.AppWeb.Models;
using SB.AR.AppWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SB.AR.AppWeb.Controllers
{
    public class ApprovalsController : SBControllerBase
    {
        //
        // GET: /Approvals/
        public ActionResult Index()
        {
            var hostUrl = this.SPHostUrl;
            var appUrl = this.SPAppWebUrl;
            var lanUrl = this.SPLanguage;

            if (!string.IsNullOrEmpty(hostUrl))
            {
                hostUrl = hostUrl.Replace("http://", "https://");
            }

            var approvalVM = new ApprovalViewModel(SPContext, this.AR);
            return PartialView("_Approvals", approvalVM);
        }

        public ActionResult LoadApprovalScreen(string workFlowId)
        {
            var approvalWorflow = new WorkFlow
            {
                ID = Convert.ToInt32(workFlowId)
            };
            var approvalVM = new ApprovalViewModel(SPContext, this.AR);
            approvalVM.ApprovalWorkflow = approvalWorflow;
            
            return PartialView("_ApprovalScreen", approvalVM);
        }
        public ActionResult ApprovalList()
        {
            var approvalVM = new ApprovalViewModel(SPContext, this.AR);
            return PartialView("_ApprovalList", approvalVM);
        }
        public bool UpdateWorkflowStatus(string workflowId, string status, string approverComments)
        {
            return UpdateWorkflowTask(Convert.ToInt32(workflowId), status, approverComments);
        }
	}
}