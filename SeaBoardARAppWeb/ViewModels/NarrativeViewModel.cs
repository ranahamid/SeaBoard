using NSBD.SharepointAutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using arapp = SB.AR.AppWeb.Models;
namespace SB.AR.AppWeb.ViewModels
{
    public class NarrativeViewModel : ViewModelBase
    {
        
        private arapp.AR spar;
        public NarrativeViewModel(SharePointContext spContext, arapp.AR ar)
            : base(spContext)
        {
            if (ar == null)
                ar = new arapp.AR();
            spar = ar;
        }

        public arapp.AR _ar
        {
            get
            {
                return spar;
            }

        }

        [DisplayName("AR Title")]
        public string Title
        {
            get
            {
                return _ar.Title;
            }
            
        }
        public int ID
        {
            get
            {
                return _ar.ID;
            }
        }

        public double? AR_ID
        {
            get
            {
                return _ar.AR_ID;
            }
        }
        public string AR_Type
        {
            get
            {
                return _ar.AR_Type;
            }
        }

        public string Current_Status
        {
            get
            {
                return _ar.Current_Status;
            }
        }
        public double Total_Cost
        {
            get
            {
                return _ar.Total_Cost;
            }
        }
        public string DivisionName
        {
            get
            {
                if (_ar.Division == null && _ar.DivisionId == null)
                {
                    return string.Empty;
                }
                else if (_ar.Division == null && _ar.DivisionId != null && _ar.DivisionId > 0)
                {
                    if (null != HttpContext.Current.Session["divisions"])
                    {
                        var divisions = (List<SelectListItem>)HttpContext.Current.Session["divisions"];
                        var sItemType = divisions.FirstOrDefault(c => c.Value == _ar.DivisionId.ToString());
                        if (sItemType != null)
                        {
                            _ar.Division = new LookupFieldMapper
                            {

                                ID = Convert.ToInt32(sItemType.Value),
                                Value = sItemType.Text
                            };
                            return sItemType.Text;
                        }
                        
                    }
                }
                    
                return _ar.Division.Value;
            }
        }
        public string CompanyName
        {
            get
            {
                if (_ar.Company_Name == null && _ar.CompanyId == null)
                    return string.Empty;
                else if (_ar.Company_Name == null && _ar.CompanyId != null && _ar.CompanyId > 0)
                {
                    if (null != HttpContext.Current.Session["_companies"])
                    {
                        var _companies = (List<SelectListItem>)HttpContext.Current.Session["_companies"];
                        var sItemType = _companies.FirstOrDefault(c => c.Value == _ar.CompanyId.ToString());
                        if (sItemType != null)
                        {
                            _ar.Company_Name = new LookupFieldMapper
                            {

                                ID = Convert.ToInt32(sItemType.Value),
                                Value = sItemType.Text
                            };
                            return sItemType.Text;
                        }

                    }
                }
                return _ar.Company_Name.Value;
            }
        }
        [Required]
        public string PresentSituationIssue
        {
            get
            {
                if (!string.IsNullOrEmpty(_ar.PresentSituationIssue))
                {
                    var text = Regex.Replace(_ar.PresentSituationIssue, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return _ar.PresentSituationIssue;
            }
        }
        [Required]
        public string Proposed_Solution
        {
            get
            {
                if (!string.IsNullOrEmpty(_ar.Proposed_Solution))
                {
                    var text = Regex.Replace(_ar.Proposed_Solution, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return _ar.Proposed_Solution;
            }
        }
        [Required]
        public string Other_Potential_Solutions
        {
            get
            {
                if (!string.IsNullOrEmpty(_ar.Other_Potential_Solutions))
                {
                    var text = Regex.Replace(_ar.Other_Potential_Solutions, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return _ar.Other_Potential_Solutions;
            }
        }
        [Required]
        public string Explanation_of_Costs
        {
            get
            {
                if (!string.IsNullOrEmpty(_ar.Explanation_of_Costs))
                {
                    var text = Regex.Replace(_ar.Explanation_of_Costs, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return _ar.Explanation_of_Costs;
            }
        }
        [Required]
        public string Financial_Measures
        {
            get
            {
                if (!string.IsNullOrEmpty(_ar.Financial_Measures))
                {
                    var text = Regex.Replace(_ar.Financial_Measures, "<.*?>", string.Empty);
                    text = WebUtility.HtmlDecode(text);
                    return text;
                }
                return _ar.Financial_Measures;
            }
        }

    }
}