
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ACI.CreativeSvcs.SP.ServiceRequestAppWeb
{
    [DataContract]
    public class PeoplePickerUser
    {
        [DataMember]
        internal int LookupId;
        [DataMember]
        internal string Login;
        [DataMember]
        internal string Name;
        [DataMember]
        internal string Email;
    }
}