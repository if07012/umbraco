using AutoMapper;
using Voxteneo.Core.Domains.Contracts;

namespace Voxteneo.Core.Domains.Impls
{
    public abstract class BaseAutoMapperConverter<TSource, TDestination> : Profile, IAutoMapperConverter<TSource, TDestination>
    {
        public BaseAutoMapperConverter()
        {
            ToDestination(CreateMap<TSource, TDestination>());
            ToSource(CreateMap<TDestination, TSource>());
        }      
          
        public abstract void ToDestination(IMappingExpression<TSource, TDestination> map);
        public abstract void ToSource(IMappingExpression<TDestination, TSource> map);
    }
}
