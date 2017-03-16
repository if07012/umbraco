using Voxteneo.Core.Attributes.Presentations;
using Voxteneo.Core.Domains.Contracts;

namespace Voxteneo.Core.Domains
{
    public class BaseEntity : IBaseEntity
    {
        [Property(Name = "visibility", Value = "hidden")]
        public int Id { get; set; }
    }
}