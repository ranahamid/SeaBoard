using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSBD.SharepointAutoMapper
{
    public class LookupFieldMapper
    {
        public Int32? ID { get; set; }
        public String Value { get; set; }

         
    }

    public class ChoiceFieldMapper
    {
        public string Text { get; set; }
        public string Value { get; set; }


    } 
}
