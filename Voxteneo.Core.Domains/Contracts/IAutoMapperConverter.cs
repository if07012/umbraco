using AutoMapper;

namespace Voxteneo.Core.Domains.Contracts
{
    public interface IAutoMapperConverter<TSource, TDestination>
    {
        void ToDestination(IMappingExpression<TSource, TDestination> map);
        void ToSource(IMappingExpression<TDestination, TSource> map);
    }
}
