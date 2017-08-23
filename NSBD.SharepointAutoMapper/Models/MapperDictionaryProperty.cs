using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSBD.SharepointAutoMapper
{
    public class MapperDictionaryProperty
    {
        public string NameFieldEntity { get; set; }
        public string TypeFieldEntity { get; set; }
        public string NameFieldSharepoint { get; set; }
        public string TypeFieldSharepoint { get; set; }
        public bool AutoConversion { get; set; }
    }
}
