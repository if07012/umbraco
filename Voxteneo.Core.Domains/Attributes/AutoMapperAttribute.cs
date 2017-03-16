using System;

namespace Voxteneo.Core.Domains.Attributes
{
    public class AutoMapperAttribute : Attribute
    {
        public Type Map { get; set; }
    }
}
