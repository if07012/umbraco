using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Voxteneo.Core.Domains.Contracts
{
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        void Delete(object id);

        void Delete(TEntity entityToDelete);

        void DeleteRange(IEnumerable<TEntity> entityToDeletes);

        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            string includeProperties = "");

        IEnumerable<TResult> GetWithSelect<TResult>(Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "",
             Expression<Func<TEntity, TResult>> select = null);


        TEntity GetByID(object id);

        TEntity GetByID(params object[] keyValues);

        void Insert(TEntity entity);

        void Update(TEntity entityToUpdate);

        void InsertOrUpdate(TEntity entity);

        bool Exists(TEntity entity);

        int Count(Expression<Func<TEntity, bool>> filter = null);

        IEnumerable<TEntity> GetChild(TEntity newClass, 
            Expression<Func<TEntity, bool>> filter = null, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
            string includeProperties = "");

        IEnumerable<TEntity> GetByDynamicQuery(string dynamicQuery, object[] values, string includedProperties = null);

        /// <summary>
        /// Get first entity with included properties.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="includeProperties">The included properties.</param>
        /// <returns></returns>
        TEntity GetFirst(Expression<Func<TEntity, bool>> filter, string[] includeProperties = null);

        /// <summary>
        /// Maximums the specified function.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        object Max(Func<TEntity, object> func);
    }
}