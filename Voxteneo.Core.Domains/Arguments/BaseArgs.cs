using System;
using Voxteneo.Core.Domains.Contracts;

namespace Voxteneo.Core.Domains.Arguments
{
    public class BaseArgs<TClass> : EventArgs where TClass : class
    {
        public TClass Entity { get; set; }
        public bool Handle { get; set; }
        public IAuditTrailEntity Audit { get; set; }
    }
    public class UpdateArgs<TClass> : BaseArgs<TClass> where TClass : class { }
    public class InsertArgs<TClass> : BaseArgs<TClass> where TClass : class
    {
    }
}
