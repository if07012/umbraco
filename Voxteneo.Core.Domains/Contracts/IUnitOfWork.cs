using System.Data.Entity;

namespace Voxteneo.Core.Domains.Contracts
{    
    public interface IUnitOfWork
    {
        IGenericRepository<T> GetGenericRepository<T>()
           where T : class;

        IGenericWithAuditRepository<T> GetGenericWithAuditRepository<T, TAuditTrail>()
            where T : class
            where TAuditTrail : class, IAuditTrailEntity;
         /// <summary>
         /// Releases unmanaged and - optionally - managed resources.
         /// the dispose method is called automatically by the injector depending on the lifestyle
         /// </summary>
        void Dispose();

        /// <summary>
        /// Saves current context changes.
        /// </summary>
        void SaveChanges();

        void RevertChanges();

        DbContext GetContext();
    }

}
