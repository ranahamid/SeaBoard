using Microsoft.SharePoint.Client;
using NSBD.SharepointAutoMapper;
using SB.AR.AppWeb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using arapp = SB.AR.AppWeb.Models;


namespace SB.AR.AppWeb.ViewModels
{
    public class MaintabViewModel : ViewModelBase
    {

        private arapp.AR _ar;
        public MaintabViewModel(SharePointContext spContext, arapp.AR ar): base(spContext)
        {
            _ar = ar;
        }
        public SharePointContext SharePointContext
        {
            get
            {
                return base._spContext;
            }
        }

        public arapp.AR AR
        {
            get
            {
                return this._ar;
            }

        }
        public bool IsMaintab
        {
            get
            {
                return true;
            }

        }
        public string Location
        {
            get
            {
                return _ar.Location;
            }
        }
        [DisplayName("AR Title")]
        [Required]
        public string Title
        {
            get
            {
                return _ar.Title;
            }
            
        }
        public int? CategoryId
        {
            get
            {
                if (_ar == null || this._ar.Category == null)
                    return 0;
                return this._ar.Category.ID;
            }
        }

        public int? CompanyId
        {
            get
            {
                if (this._ar.Company_Name == null || _ar == null)
                    return 0;
                return this._ar.Company_Name.ID;
            }
        }

        public int? DivisionId
        {
            get
            {
                if (this._ar.Division == null || _ar == null)
                    return 0;
                return this._ar.Division.ID;
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

        public string LocalAR
        {
            get
            {
                return _ar.LocalAR;
            }
        }


        public string LookupValue
        {
            get
            {
                return _ar.LookupValue;
            }
        }

        public int LookupId
        {
            get
            {
                return _ar.LookupId;
            }
        }

        public string ARNumber
        {
            get
            {
                return _ar.ARNumber;
            }
        }
        public string PMOwner
        {
            get
            {
                return _ar.PMOwner;
            }
        }
        public DateTime? Project_Start
        {
            get
            {
                return _ar.Project_Start;
            }
        }
        public DateTime? Project_End
        {
            get
            {
                return _ar.Project_End;
            }
        }
        public bool Engineering_Review
        {
            get
            {
                return _ar.Engineering_Review;
            }
        }
        public bool HR_Review
        {
            get
            {
                return _ar.HR_Review;
            }
        }
        public bool IT_Review
        {
            get
            {
                return _ar.IT_Review;
            }
        }
        public bool Legal_Review
        {
            get
            {
                return _ar.Legal_Review;
            }
        }

        public bool Audit
        {
            get
            {
                return _ar.Audit;
            }
        }

        public string Audit_Updated_By
        {
            get
            {
                return _ar.Audit_Updated_By == null ? string.Empty : _ar.Audit_Updated_By.LookupValue;
            }
        }
    }
}