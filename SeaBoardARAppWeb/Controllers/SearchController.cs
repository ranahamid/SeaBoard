using SB.AR.AppWeb.Models;
using SB.AR.AppWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SB.AR.AppWeb.Controllers
{


    public class SearchController : SBControllerBase
    {
        public static class SearchMeta
        {
            public const string RelativeSearchPage = "/SitePages/ARSearch.aspx";
            public const string ARDivision = "ARDivision";
            public const string ARCompany = "ARCompany";
            public const string ARCurrentStatus = "ARCurrentStatus";
            public const string ARPMOwner = "ARPMOwner";
            public const string ARCreatedBy = "ARCreatedBy";
            public const string ARCategory = "ARCategory";
            public const string ARTotalCost = "ARTotalCost";
            public const string Created = "Created";
        }

        //
        // GET: /Search/
        public ActionResult Index()
        {
            SearchViewModel svm = new SearchViewModel(SPContext, null);

            return View(svm);
        }

        public ActionResult ViewAllAR()
        {
            return View("ViewAllAR");
        }

        
        public string GetViewALLARLinkURL()
        {
            string viewLink = string.Empty;

            if (Session["SPHostUrl"] != null)
            {
                string hostWebURL = Convert.ToString(Session["SPHostUrl"]);
                if (!hostWebURL.Contains("https"))
                {
                    // only allow https
                    hostWebURL = hostWebURL.Replace("http://", "https://");

                }

                viewLink = hostWebURL + "/SitePages/ARList.aspx";
            }

            return viewLink;
        }

        [HttpPost]
        public string GoSearch(Search searchData)
        {

            StringBuilder searchString = new StringBuilder("?k=");

            if (Session["SPHostUrl"] != null)
            {
                string hostWebURL = Convert.ToString(Session["SPHostUrl"]);
                if (!hostWebURL.Contains("https"))
                {
                    // only allow https
                    hostWebURL = hostWebURL.Replace("http://", "https://");
                }

                if (string.IsNullOrEmpty(searchData.KeywordString))
                {
                    // Append any parameters found filled by user
                    if (!string.IsNullOrEmpty(searchData.DivisionString))
                    {
                        searchString.AppendFormat("{0}:{1}|", SearchMeta.ARDivision, searchData.DivisionString);
                    }
                    if (!string.IsNullOrEmpty(searchData.CompanyString))
                    {
                        searchString.AppendFormat("{0}:{1}|", SearchMeta.ARCompany, searchData.CompanyString);
                    }
                    if (!string.IsNullOrEmpty(searchData.Status))
                    {
                        searchString.AppendFormat("{0}:{1}|", SearchMeta.ARCurrentStatus, searchData.Status);
                    }
                    if (!string.IsNullOrEmpty(searchData.PMOwner))
                    {
                        searchString.AppendFormat("{0}:{1}|", SearchMeta.ARPMOwner, searchData.PMOwner);
                    }
                    if (!string.IsNullOrEmpty(searchData.CreatedBy))
                    {
                        searchString.AppendFormat("{0}:{1}|", SearchMeta.ARCreatedBy, searchData.CreatedBy);
                    }
                    if (!string.IsNullOrEmpty(searchData.CategoryString))
                    {
                        searchString.AppendFormat("{0}:{1}|", SearchMeta.ARCategory, searchData.CategoryString);
                    }
                    if (searchData.AmountFrom.HasValue && searchData.AmountTo.HasValue)
                    {
                        searchString.AppendFormat("{0}>={1}|{0}<={2}|", SearchMeta.ARTotalCost, searchData.AmountFrom.Value, searchData.AmountTo.Value);
                    }
                    if (searchData.SubmittedFrom.HasValue)
                    {
                        searchString.AppendFormat("{0}>={1}|", SearchMeta.Created, searchData.SubmittedFrom.Value.ToShortDateString());
                    }
                    if (searchData.SubmittedTo.HasValue)
                    {
                        searchString.AppendFormat("{0}<={1}|", SearchMeta.Created, searchData.SubmittedTo.Value.ToShortDateString());
                    }
                }
                else
                {
                    // keyword provided so search for keyword only
                    searchString.Append(searchData.KeywordString);
                }

                // Prepare the search parameters
                string _preSearch = searchString.ToString();
                string finalSearchParameters = string.Empty;
                if (_preSearch.Length > 3)
                {
                    string[] individualEntries = _preSearch.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    finalSearchParameters = String.Join(" AND ", individualEntries);
                }

                if (string.IsNullOrEmpty(finalSearchParameters))
                {
                    return string.Empty; // no search
                }
                else
                {
                    return string.Format("{0}{1}{2}&mode=hide", hostWebURL, SearchMeta.RelativeSearchPage, finalSearchParameters);
                }
            }

            return string.Empty;
        }

        public ActionResult FillCompany(int divisionId)
        {
            var vm = new CompanyMasterViewModel(SPContext)
            {
                DivisionId = divisionId,

            };

            return PartialView("~/Views/Maintab/_CompanyList.cshtml", vm);
        }
    }
}