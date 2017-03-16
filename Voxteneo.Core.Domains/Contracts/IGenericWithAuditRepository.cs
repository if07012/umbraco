using System;
using Voxteneo.Core.Domains.Arguments;

namespace Voxteneo.Core.Domains.Contracts
{
    
    public interface IGenericWithAuditRepository<TEntity> : IGenericRepository<TEntity>
       where TEntity : class
    {
        event EventHandler<UpdateArgs<TEntity>> UpdateEntity;
        event EventHandler<InsertArgs<TEntity>> InsertEntity;
    }

}