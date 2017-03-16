using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Reflection;

namespace Voxteneo.Core.Domains
{
    public class VxSet<TEntity> : DbSet<TEntity> where TEntity : class
    {
        private static readonly FieldInfo InternalSetField =
        typeof(DbSet<TEntity>).GetField(
            "_internalSet",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo InternalQueryField =
            typeof(DbQuery<TEntity>).GetField(
                "_internalQuery",
                BindingFlags.NonPublic | BindingFlags.Instance);

        private DbContext _context;

     
        public override TEntity Add(TEntity entity)
        {
           
            return base.Add(entity);
        }
        public VxSet(VxContext context)
        {
            this._context = context;
            var internalSet = InternalSetField.GetValue(context.Set<TEntity>());
            InternalSetField.SetValue(this, internalSet);
            InternalQueryField.SetValue(this, internalSet);
        }

    }
}
