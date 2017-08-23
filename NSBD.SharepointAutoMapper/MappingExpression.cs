using Microsoft.SharePoint.Client;
using NSBD.SharepointAutoMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSBD.SharepointAutoMapper
{
    public class MappingExpression : IMappingExpression
    {


        public MappingExpression(string ListName, Type Entity)
        {
            MappingStore.MapperModel.Add(new SharepointMapperModel()
            {
                ListName = ListName,
                EntityType = Entity,
                EntityName = Entity.Name,
                
            });
        }

        public IMappingExpression ForMember(string name, Action<IMemberConfigurationExpression> memberOptions)
        {

            return new MappingExpression("", null);
        }



    }

    
}
