using System;

namespace Voxteneo.Core.Attributes
{
    public class VExceptionAttribute : VBaseAttribute
    {
        public Exception Exception { get; set; }
    }
}
