using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using Voxteneo.Core.Domains.Arguments;
using Voxteneo.Core.Domains.Contracts;
using Voxteneo.WebComponents.Logger;

namespace Voxteneo.Core.Domains.Uow
{
    public class SqlGenericRepository<Class> : IGenericRepository<Class> where Class : class
    {
        protected internal DbContext Context;
        protected internal DbSet<Class> DbSet;

        protected ILogger Logger;

        public SqlGenericRepository(DbContext context, ILogger logger)
        {
            Logger = logger;
            this.Context = context;
            this.DbSet = context.Set<Class>();
        }

        public virtual IEnumerable<Class> Get(
            Expression<Func<Class, bool>> filter = null,
            Func<IQueryable<Class>, IOrderedQueryable<Class>> orderBy = null,
            string includeProperties = "")
        {
            return GetWithSelect<Class>(filter, orderBy, includeProperties);
        }

        public virtual int Count(Expression<Func<Class, bool>> filter = null)
        {
            IQueryable<Class> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query.Count();
        }

        public virtual Class GetByID(object id)
        {
            return DbSet.Find(id);
        }

        /// <summary>
        /// Gets the by identifier.
        /// !! Custom modif
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns></returns>
        public virtual Class GetByID(params object[] keyValues)
        {
            return DbSet.Find(keyValues);
        }

        public virtual void Insert(Class entity)
        {
            if (entity is BaseModifiy)
                (entity as BaseModifiy).CreateDate = DateTime.Now;
            DbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            Class entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(Class entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
            
        }



        protected void OnUpdate(UpdateArgs<Class> e)
        {
            UpdateEntity?.Invoke(this, e);
        }
        public event EventHandler<UpdateArgs<Class>> UpdateEntity;
        public event EventHandler<InsertArgs<Class>> InsertEntity;
        protected void OnInsert(InsertArgs<Class> e)
        {
            InsertEntity?.Invoke(this, e);
        }
        public virtual void Update(Class entity)
        {
            var entry = Context.Entry(entity);
            if (entity is BaseModifiy)
                (entity as BaseModifiy).LastUpdated = DateTime.Now;
            entry.State = EntityState.Modified;
        }

        public void InsertOrUpdate(Class entity)
        {
            if (!Exists(entity))
                Insert(entity);
            else
                Update(entity);
        }

        /// <summary>
        /// check if the specified entity exists
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Boolean.</returns>
        public bool Exists(Class entity)
        {
            var objContext = ((IObjectContextAdapter)this.Context).ObjectContext;
            var objSet = objContext.CreateObjectSet<Class>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

            Object foundEntity;
            var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);
            //// TryGetObjectByKey attaches a found entity
            //// Detach it here to prevent side-effects
            //if (exists)
            //{
            //    objContext.Detach(foundEntity);
            //}

            return (exists);
        }




        public virtual IEnumerable<Class> GetChild(Class newClass,
           Expression<Func<Class, bool>> filter = null,
           Func<IQueryable<Class>, IOrderedQueryable<Class>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<Class> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
             (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.AsNoTracking().Include(includeProperty);
            }



            var q = query.First();
            //dbSet.Add(q);

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public IEnumerable<Class> GetByDynamicQuery(string dynamicQuery, object[] values, string includedProperties)
        {
            if (values.Length == 0)
                return null;
            IQueryable<Class> query = DbSet.AsNoTracking();
            if (!string.IsNullOrEmpty(dynamicQuery))
            {
                if (!string.IsNullOrEmpty(includedProperties))
                {
                    foreach (var includeProperty in includedProperties.Split
                   (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProperty);
                    }
                }
                query = query.Where(dynamicQuery, values);
            }
            return query.ToList();
        }

        public virtual IEnumerable<TResult> GetWithSelect<TResult>(Expression<Func<Class, bool>> filter = null, Func<IQueryable<Class>, IOrderedQueryable<Class>> orderBy = null, string includeProperties = "",
             Expression<Func<Class, TResult>> select = null)
        {
            IQueryable<Class> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                if (select != null)
                {
                    return orderBy(query).Select(select);
                }
                return orderBy(query).OfType<TResult>().ToList();
            }
            else
            {
                if (select != null)
                {
                    return query.Select(select);
                }
                return query.OfType<TResult>().ToList();
            }
        }

        /// <summary>
        /// Get first entity with included properties.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="includeProperties">The included properties.</param>
        /// <returns></returns>
        public Class GetFirst(Expression<Func<Class, bool>> filter, string[] includeProperties = null)
        {
            IQueryable<Class> query = DbSet;

            if (filter != null)
                query = query.Where(filter);

            if (includeProperties != null && includeProperties.Length > 0)
                query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            return query.FirstOrDefault();
        }

        public void DeleteRange(IEnumerable<Class> entityToDeletes)
        {
            var toDeletes = entityToDeletes as IList<Class> ?? entityToDeletes.ToList();

            foreach (var entityToDelete in toDeletes)
            {
                if (Context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    DbSet.Attach(entityToDelete);
                }
            }
            DbSet.RemoveRange(toDeletes);
            //this.context.SaveChanges();
        }

        public object Max(Func<Class, object> func)
        {
            return DbSet.Max(func);
        }
    }
}
