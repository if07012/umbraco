using System;

namespace Voxteneo.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property|AttributeTargets.Class)]
    public class InjectAttribute : Attribute
    {
        public string MapId { get; set; }        
    }
}
