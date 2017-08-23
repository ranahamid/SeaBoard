using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSBD.SharepointAutoMapper.Interfaces
{
    public interface IMemberConfigurationExpression
    {
        void MapFrom(string sourceMember);
        void Ignore();
    }
}
