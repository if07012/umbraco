using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Voxteneo.Core.Domains.Contracts;
using Voxteneo.Core.Domains.LambdaSqlBuilder;

namespace Voxteneo.Core.Domains.UmbracoExtentions
{
    public interface ICoreQueryable<T> : IEnumerable<T>
    {
        ICoreQueryable<T> Where(Expression<Func<T, bool>> expression);
    }

    public class CoreQueryable<T> : ICoreQueryable<T>
    {
        private readonly Database _database;
        private bool first;
        private int _take;
        private int skip;
        public ICoreQueryable<T> Take
(int take)
        {
            _take = take;
            return this;
        }

        public ICoreQueryable<T> Skip
(int skip)
        {
            this.skip = skip;
            return this;
        }


        public SqlLam<T> Query { get; set; }
        public CoreQueryable(System.Data.Entity.Database database)
        {
            _database = database;
            Query = new SqlLam<T>();
            Query._builder.QueryByFieldNotNull(typeof(T).Name, "Id");
        }
        public IEnumerator<T> GetEnumerator()
        {
            List<T> list = new List<T>();

            var query = Query.QueryString;
            if (_take != 0)
                query = Query.QueryStringPage(_take, skip / _take);
            foreach (var queryQueryParameter in Query.QueryParameters)
            {
                query = query.Replace("@" + queryQueryParameter.Key, "'" + queryQueryParameter.Value + "'");
            }
            var dictionary = new Dictionary<string, PropertyInfo>();
            foreach (var property in typeof(T).GetProperties())
            {
                if (!dictionary.ContainsKey(property.Name))
                    dictionary.Add(property.Name, property);
            }
            foreach (var data in _database.SqlQuery<string>(query))
            {
                var xElement = XElement.Parse(data);
                var obj = Activator.CreateInstance<T>();
                foreach (var key in dictionary)
                {
                    if (key.Key.Equals("Id"))
                    {
                        var xAttribute = xElement.Attribute("id");
                        if (xAttribute != null) key.Value.SetValue(obj, Convert.ChangeType(xAttribute.Value, key.Value.PropertyType));
                    }
                    else
                    {
                        var element = xElement.Element(key.Key[0].ToString().ToLower() + key.Key.Substring(1));
                        if (element != null)
                            key.Value.SetValue(obj, Convert.ChangeType(element.Value, key.Value.PropertyType));
                    }
                }
                list.Add(obj);

            }
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ICoreQueryable<T> Where(Expression<Func<T, bool>> expression)
        {

            Query.Where(expression);
            return this;
        }
        public ICoreQueryable<T> FirstOrDefault
(Expression<Func<T, bool>> expression)
        {
            first = true;
            Query.Where(expression);
            return this;
        }
    }
    public static class Query
    {
        public static ICoreQueryable<object> QueryData(this System.Data.Entity.Database database, string schemaName)
        {
            return new CoreQueryable<object>(database);
        }
        public static ICoreQueryable<T> QueryData<T>(this System.Data.Entity.Database database)
        {
            return new CoreQueryable<T>(database);
        }

        public static BasePagedList<T> PagedQueryable<T>(this IQueryable<T> context, BasePagedInput input) where T : class, IBaseEntity
        {
            try
            {
                if (input.PageCurrent == 0)
                    input.PageCurrent = 1;
                if (input.PageSize == 0)
                    input.PageSize = 10;
                IEnumerable<T> list = null;
                if (!(context is IOrderedQueryable<T>))
                    list = context.OrderBy(n => n.Id)
                .Skip(input.PageSize * (input.PageCurrent - 1))
                    .Take(input.PageSize);
                else
                    list = ((IOrderedQueryable<T>)context)
                        .Skip(input.PageSize * (input.PageCurrent - 1))
                        .Take(input.PageSize);
                var count = context.Count();
                return new BasePagedList<T>()
                {
                    PageSize = input.PageSize,
                    PageCurrent = input.PageCurrent,
                    Records = list.ToList(),
                    TotalRecordCount = count,
                    IsLast = input.PageCurrent == Convert.ToInt16(Math.Ceiling(Convert.ToDouble(count) / Convert.ToDouble(input.PageSize)))
                };
            }
            catch (Exception e)
            {
                throw;
            }

        }
    }
}
