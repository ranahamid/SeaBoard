using NSBD.SharepointAutoMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSBD.SharepointAutoMapper
{
    public interface IConfiguration
    {
        IMappingExpression CreateMap<TSource>(String ListName);
    }
}
