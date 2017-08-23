using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSBD.SharepointAutoMapper
{
   
    [System.AttributeUsage(System.AttributeTargets.All |
                       System.AttributeTargets.Struct,
                       AllowMultiple = true)]
    public class SharepointFieldName : System.Attribute
    {
        string name;
        public double version;

        public SharepointFieldName(string name)
        {
            this.name = name;

            // Default value.
            version = 1.0;
        }

        public string GetName()
        {
            return name;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.All |
                      System.AttributeTargets.Struct,
                      AllowMultiple = true)]
    public class IgnorePropertyInSharepoint : System.Attribute
    {
 

        public IgnorePropertyInSharepoint()
        {
          
        }

    }


    [System.AttributeUsage(System.AttributeTargets.All |
                       System.AttributeTargets.Struct,
                       AllowMultiple = true)]
    public class MapperLookupId : System.Attribute
    {
        string _LookupName;
        public double version;

        public MapperLookupId(string LookupName)
        {
            this._LookupName = LookupName;

            // Default value.
            version = 1.0;
        }

        public string GetName()
        {
            return _LookupName;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.All |
                   System.AttributeTargets.Struct,
                   AllowMultiple = true)]
    public class MapperLookupValue : System.Attribute
    {
        string _LookupName;
        public double version;

        public MapperLookupValue(string LookupName)
        {
            this._LookupName = LookupName;

            // Default value.
            version = 1.0;
        }

        public string GetName()
        {
            return _LookupName;
        }
    }


    [System.AttributeUsage(System.AttributeTargets.All |
               System.AttributeTargets.Struct,
               AllowMultiple = true)]
    public class SharepointListName : System.Attribute
    {
        string _ListName;
        public double version;

        public SharepointListName(string ListName)
        {
            this._ListName = ListName;

            // Default value.
            version = 1.0;
        }

        public string GetName()
        {
            return _ListName;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.All |
                   System.AttributeTargets.Struct,
                   AllowMultiple = true)]
    public class ChoiceField : System.Attribute
    {
        string _ChoiceName;
        public double version;

        public ChoiceField(string ChoiceName)
        {
            this._ChoiceName = ChoiceName;

            // Default value.
            version = 1.0;
        }

        public string GetName()
        {
            return _ChoiceName;
        }
    }
}
