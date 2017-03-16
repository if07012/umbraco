using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Voxteneo.Core.Domains.Contracts;
using Voxteneo.WebComponents.Logger;

namespace Voxteneo.Core.Domains.Uow
{
    /// <summary>
    /// Class SqlUnitOfWork.
    /// http://stackoverflow.com/questions/20795897/implementing-the-repository-and-unit-of-work-patterns-in-asp-net
    /// </summary>
    public class SqlUnitOfWork : IUnitOfWork
    {
        private ILogger _logger;
        public static DbContext DbContext { get; set; }
        public SqlUnitOfWork(ILogger logger)
        {
            _logger = logger;
            _context = (DbContext)Activator.CreateInstance(DbContext.GetType());
        }


        //load a context automatically
        // the context is disposed when this UOW obejct is disposed > at end of webrequest (simple injector weblifestyle request)
        private DbContext _context;

        public IGenericRepository<T> GetGenericRepository<T>()
            where T : class
        {
            return new SqlGenericRepository<T>(_context, _logger);
        }

        public IGenericWithAuditRepository<T> GetGenericWithAuditRepository<T, TAuditTrail>()
           where T : class
            where TAuditTrail : class, IAuditTrailEntity
        {
            return new SqlGenericWithAuditRepository<T, TAuditTrail>(_context, _logger);
        }

        /// <summary>
        /// Saves context changes
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                LogEntityValidationErrors(e.EntityValidationErrors);
                throw;
            }
        }

        /// <summary>
        /// Reverts the changes.
        /// </summary>
        public void RevertChanges()
        {
            //overwrite the existing context with a new, fresh one to revert all the changes
            _context = new DbContext(_context.Database.Connection.ConnectionString);
        }

        /// <summary>
        /// Logs the entity validation errors.
        /// </summary>
        /// <param name="entityValidationErrors">The entity validation errors.</param>
        private void LogEntityValidationErrors(IEnumerable<DbEntityValidationResult> entityValidationErrors)
        {
            foreach (var entityValidationError in entityValidationErrors)
            {
                _logger.Error(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    entityValidationError.Entry.Entity.GetType().Name, entityValidationError.Entry.State));
                foreach (var validationErrors in entityValidationError.ValidationErrors)
                {
                    _logger.Error(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                        validationErrors.PropertyName, validationErrors.ErrorMessage));
                }
            }
        }

        private bool disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// the dispose method is called automatically by the injector depending on the lifestyle
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DbContext GetContext()
        {
            return _context;
        }
    }
}