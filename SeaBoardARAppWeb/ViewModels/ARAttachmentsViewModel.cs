using Microsoft.SharePoint.Client;
using SB.AR.AppWeb.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NSBD.SharepointAutoMapper;
using Microsoft.SharePoint;
using AutoMapper;
using System.IO;
using arapp = SB.AR.AppWeb.Models;

namespace SB.AR.AppWeb.ViewModels
{
    //[SharePointContextFilter]
    public class ARAttachmentsViewModel : ViewModelBase
    {
        public List<Models.ARAttachments> attachments { get; set; }
        private arapp.AR _ar;
        public ARAttachmentsViewModel() { }

        public ARAttachmentsViewModel(string arNumber) 
        {
            attachments = this.GetARAttachments(arNumber);
        }


        public ARAttachmentsViewModel(SharePointContext _sharePointContext, arapp.AR ar)
            : base(_sharePointContext)
        {
            if (ar == null)
                ar = new arapp.AR();
            _ar = ar;
            if (!string.IsNullOrEmpty(_ar.Attachment_Folder_Id))
            {
                attachments = this.GetARAttachments(_ar.Attachment_Folder_Id.ToString());
            }
        }
        public User CurrentUser
        {
            get
            {
                User spUser = null;
                using (var clientContext = _spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        spUser = clientContext.Web.CurrentUser;
                        clientContext.Load(spUser);
                        clientContext.ExecuteQuery();
                    }
                }
                return spUser;
            }
        }
        public bool IsDeletable
        {
            get
            {
                if (AR != null && AR.Author != null && AR.Author.Value.Trim().ToLower()
              == CurrentUser.Title.Trim().ToLower()
              && (AR.Current_Status != "Pending Approvals"))

                    return true;

                if (AR != null && AR.PMUser != null && AR.PMUser.LookupValue != null && AR.PMUser.LookupValue.Trim().ToLower()
                    == CurrentUser.Title.Trim().ToLower()
                    && (AR.Current_Status != "Pending Approvals"))

                    return true;

                return false;
            }
        }
        public arapp.AR AR
        {
            get
            {
                return _ar;
            }
        }        
        private List<Models.ARAttachments> GetARAttachments(string ARNumber)
        {
            List<Models.ARAttachments> attachments = new List<Models.ARAttachments>();
            string shareFileFolder = Convert.ToString(System.Web.Configuration.WebConfigurationManager.AppSettings["ShareFileFolder"]);
            var ARDirectory = Path.Combine(shareFileFolder, ARNumber);
            if(Directory.Exists(ARDirectory))
            {
                var arDocuments = Directory.GetFiles(ARDirectory);
                foreach(var file in arDocuments)
                {
                    Models.ARAttachments arFile = new Models.ARAttachments();
                    arFile.FileName = Path.GetFileName(file);
                    arFile.FileUrl = file;
                    attachments.Add(arFile);                    
                }
            }

            return attachments;
        }

        public void ValidateAR()
        {
            //// Validate Main Tab
            //// Validate Narrative Tab
            //// Validate Financical tab
        }

    }
    


}