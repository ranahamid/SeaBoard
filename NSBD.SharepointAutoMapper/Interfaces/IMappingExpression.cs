using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace NSBD.SharepointAutoMapper.Interfaces
{
    public interface IMappingExpression
    {
        IMappingExpression ForMember(string name, Action<IMemberConfigurationExpression> memberOptions);
    }
}
