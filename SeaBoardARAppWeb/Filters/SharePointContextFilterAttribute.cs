using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace SB.AR.AppWeb
{
    /// <summary>
    /// SharePoint action filter attribute.
    /// </summary>
    public class SharePointContextFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //ServicePointManager.ServerCertificateValidationCallback =
            //        delegate(object s, X509Certificate certificate,
            //                 X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //        { return true; };
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(filterContext.HttpContext, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    filterContext.Result = new RedirectResult(redirectUrl.AbsoluteUri);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    filterContext.Result = new ViewResult { ViewName = "Error" };
                    break;
            }
        }
    }
}
