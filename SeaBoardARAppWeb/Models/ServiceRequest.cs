
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SP = Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Configuration;
using System.ComponentModel;
using System.Web;

namespace SB.AR.AppWeb.Models
{
    public class ServiceRequest
    {
        //General Information
        [DisplayName("Select File to Upload")]
        //public HttpPostedFileBase File { get; set; }
        public List<FileDetails> Files { get; set; }
        public string FileNameList { get; set; }
        public int ServiceRequestID { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter Title")]
        public string Title { get; set; }
        public string ForwardRequesTo { get; set; }

        public IEnumerable<SelectListItem> Audience { get; set; }
         [Required(ErrorMessage = "Please select Audience")]
        public string AudienceLookUpID { get; set; }

        public string OnBehalf { get; set; }

        public string ProductionSpecialistNeeded { get; set; }
        public string ProductionSpecialist { get; set; }
        //public string SelectedProductionSpecialist { get; set; }
        public string Designer { get; set; }
        public string BackupDesigner { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateAssigned { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateCompleted { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please enter Expected Completion Date")]
        public DateTime? ExpectedCompletionDate { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please enter Graphics Copy")]
        public DateTime? GraphicsCopy { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Please enter Graphics -1ST Proof	Date ")]
        public DateTime? Graphics1stProof { get; set; }
        [DataType(DataType.Date)]
        public DateTime? FinalApproval { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ToProductionFilm { get; set; }
        [Required(ErrorMessage = "Please enter Printed Delivery")]
        [DataType(DataType.Date)]
        public DateTime? PrintedDelivery { get; set; }
        [DataType(DataType.Date)]
        public DateTime? SubmitedDate { get; set; }
        public string SubmitedBy { get; set; }



        //Additional Information
        public string Descriptions { get; set; }
        public string UploadFiles { get; set; }

        //Remove from latest screen
        public IEnumerable<SelectListItem> Priority { get; set; }


        //SPEC==> More information
        [Required(ErrorMessage = "Please enter Budget Code")]
        public string BudgetCode { get; set; }
        
        public IEnumerable<SelectListItem> ClientName { get; set; }
        [Required(ErrorMessage = "Please select Client Name")]
        public string ClientNameLookUpID { get; set; }

        [Required(ErrorMessage = "Please select Final Output")]
        public string FinalOutputLookUpID { get; set; }
        public IEnumerable<SelectListItem> FinalOutput { get; set; }

        public string AdditionalOutputLookUpID { get; set; }
        public IEnumerable<SelectListItem> AdditionalOutput { get; set; }

        
        [Required(ErrorMessage = "Please enter Print Quantity")]
        public string PrintQuantity { get; set; }

        public string AdditionalInfo { get; set; }//ps
        public string InstructionForDelivery { get; set; }

        public string Paper { get; set; }
        public string Bindery { get; set; }
        public string Pages { get; set; }

        public string ApproxSize { get; set; }

        public string Colors { get; set; }


        public List<KeyCodes> KeyCodeDetails { get; set; }
        public IEnumerable<SelectListItem> ScrapOldKeyCode { get; set; }
        public string SelectedScrapOldKeyCode { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }
        public string RequestStatus { get; set; }
        public List<ServiceRequestHistory> RequestHistory { get; set; }

        public string OldKey { get; set; }
        public string NewKey { get; set; }

        public GroupCollection UserGroups { get; set; }
        public bool ShowProductionSpeciaList { get; set; }
        public bool ShowNewKeyCode { get; set; }
        public bool EditformAuthorisedAccess { get; set; }

        public bool EnableStatusField { get; set; }
        public ServiceRequestAction RequestAction { get; set; }
        public OperationMode Mode { get; set; }

        public bool ShowStatusTab { get; set; }

        public string flag { get; set; }
        public ServiceRequest()
        {

        }

        //public SelectList GetChoices(SP.Field Field, string FieldValue)
        //{
        //    List<SelectListItem> ChoiceFields = new List<SelectListItem>();
        //    switch (Field.FieldTypeKind)
        //    {
        //        case SP.FieldType.Boolean:
        //            ChoiceFields.Add(new SelectListItem() { Text = "Yes", Value = "true", Selected = "true" == FieldValue });
        //            ChoiceFields.Add(new SelectListItem() { Text = "No", Value = "false", Selected = "false" == FieldValue });
        //            break;
        //        case SP.FieldType.Choice:
        //            ChoiceFields = ((SP.FieldChoice)Field).Choices.Select(x => new SelectListItem { Text = x, Value = x, Selected = (x == FieldValue) }).ToList();
        //            break;
        //        case SP.FieldType.MultiChoice:
        //            ChoiceFields = ((SP.FieldMultiChoice)Field).Choices.Select(x => new SelectListItem { Text = x, Value = x, Selected = (x == FieldValue) }).ToList();
        //            break;
        //        default:
        //            break;
        //    }


        //    return new SelectList(ChoiceFields, "Value", "Text");
        //}

    }
    public enum RequestStatus
    {
        Submitted,
        Open,
        Draft,
        BrandReview,
        BrandReviewApproved,
        ClientReview,
        Approved,
        PrintProduction,
        Canceled,
        Archived,
        Completed,
        OnHold

    }
    public enum ServiceRequestAction
    {
        Submit,
        Draft,
        Clone
    }
    public enum OperationMode
    {
        Insert,
        Edit,
        Delete
    }
    public enum NotificationType
    {
        NewRequest,
        ChangeRequest,
        FinalApproval
    }
}