using System;
using Voxteneo.Core.Attributes.Presentations;

namespace Voxteneo.Core.Domains
{
    public class BaseModifiy : BaseEntity
    {
        [Property(Name = "title", Value = "Create Date")]
        [Property(Name = "type", Value = "date")]
        [Property(Name = "displayFormat", Value = "dd/mm/yy")]
        public DateTime CreateDate { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
}