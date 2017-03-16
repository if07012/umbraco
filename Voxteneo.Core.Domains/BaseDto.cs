using AutoMapper;
using Voxteneo.Core.Helper;

namespace Voxteneo.Core.Domains
{
    public class BaseDto
    {
        public TDestination ToMap<TDestination>()
        {
            return AopHelper.CreateObject<TDestination>(Mapper.Map<TDestination>(this));
        }
    }
}
