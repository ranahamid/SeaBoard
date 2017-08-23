using NSBD.SharepointAutoMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSBD.SharepointAutoMapper
{
    public class SharepointMapperModel
    {
        public Type EntityType { get; set; }
        public string EntityName { get; set; }
        public string ListName { get; set; }

        public Dictionary<string, string> Parametros { get; set; }
    }
    public static class MappingStore {
        public static List<SharepointMapperModel> MapperModel { get; set; }
    }
    public class ConfigurationStore : IConfiguration
    {


        
        

        public IMappingExpression CreateMap<TSource>(String ListName)
        {
            return CreateMappingExpression(typeof(TSource));
        }
        private IMappingExpression CreateMappingExpression(Type destinationType)
        {
            IMappingExpression mappingExp = new MappingExpression(null, null );

            //TypeInfo destInfo = new TypeInfo(destinationType);
            //foreach (var destProperty in destInfo.GetPublicWriteAccessors())
            //{
            //    object[] attrs = destProperty.GetCustomAttributes(true);
            //    if (attrs.Any(x => x is IgnoreMapAttribute))
            //    {
            //        mappingExp = mappingExp.ForMember(destProperty.Name, y => y.Ignore());
            //    }
            //}

            return mappingExp;
        }
    }
}
