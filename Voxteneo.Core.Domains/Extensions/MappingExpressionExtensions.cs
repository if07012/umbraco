using AutoMapper;

namespace Voxteneo.Core.Domains.Extensions
{
    public static class MappingExpressionExtensions
    {
        /// <summary>
        /// Ignores all unmapped.
        /// </summary>
        /// <typeparam name="TSource">The type of the attribute source.</typeparam>
        /// <typeparam name="TDest">The type of the attribute dest.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>IMappingExpression{``0``1}.</returns>
        public static IMappingExpression<TSource, TDest> IgnoreAllUnmapped<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        {
            expression.ForAllMembers(opt => opt.Ignore());
            return expression;
        }

        /// <summary>
        /// Ignores all non specifically defined in mapping.
        /// </summary>
        /// <typeparam name="TSource">The type of the attribute source.</typeparam>
        /// <typeparam name="TDestination">The type of the attribute destination.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>IMappingExpression{``0``1}.</returns>
        public static IMappingExpression<TSource, TDestination>
            IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            //foreach (var property in expression.TypeMap.GetUnmappedPropertyNames())
            //{
            //    expression.ForMember(property, opt => opt.Ignore());
            //}

            return expression;
        }
    }
}
