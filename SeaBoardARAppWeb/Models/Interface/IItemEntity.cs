using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SharePoint.Client;

namespace SB.AR.AppWeb.Models.Interface
{
   
    public partial interface IItemEntity
    {
        int Id { get; }
        Guid UniqueId { get; }
        DateTime Created { set; get; }
        DateTime Modified { set; get; }
        AttachmentCollection Attachments { get; }
        string Title { set; get; }
        FieldUserValue Author { set; get; }
        double? Number { set; get; }
        FieldUserValue Editor { set; get; }
    }
}