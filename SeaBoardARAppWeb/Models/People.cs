using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SB.AR.AppWeb.Models
{
    public class People
    {
        public string LookupValue { get; set; }
        public string LookupId { get; set; }
    }
    public class EntityData
    {
        public string Title { get; set; }
        public string MobilePhone { get; set; }
        public string SIPAddress { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
    }

    public class RootObject
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public string DisplayText { get; set; }
        public string EntityType { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderName { get; set; }
        public bool IsResolved { get; set; }
        public EntityData EntityData { get; set; }
        public List<object> MultipleMatches { get; set; }
    }
}