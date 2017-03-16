using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Voxteneo.Core.Attributes.Presentations;
using Voxteneo.Core.Domains;
using Voxteneo.Core.Domains.Contracts;

namespace Learning.Entities
{
    [CustomAction(Name = "Edit", Value = "Edit Item", Attribute = "Css:btn-success;Icon:fa-pencil")]
    [CustomAction(Name = "Delete", Value = "Delete Item", Attribute = "Css:btn-danger;Icon:fa-trash")]
    public class Customer : BaseModifiy, ISoftDelete
    {
        [Property(Name = "title", Value = "Full Name")]
        public string FullName { get; set; }
        [Property(Name = "title", Value = "Sur Name")]
        public string SurName { get; set; }
        [Property(Name = "title", Value = "Height")]
        [Property(Name = "width", Value = "5%")]
        public int Height { get; set; }
        [Property(Name = "title", Value = "Weight")]
        [Property(Name = "width", Value = "5%")]
        public double Weight { get; set; }
        [Property(Name = "title", Value = "Birth Date")]
        [Property(Name = "type", Value = "date")]
        [Property(Name = "displayFormat", Value = "dd/mm/yy")]
        public DateTime BirthDate { get; set; }
        [Reference(PropertyName = "Vehicles", SortBy = "Name", Type = ReferenceAttribute.ReferenceType.EntityFramework)]
        public ICollection<Vehicle> Vehicles { get; set; }
        [Property(Name = "title", Value = "Customer Group")]
        [Property(Name = "display", Value = "(function (data) { return data.record.CustomerGroup.Name;})")]
        [Reference(PropertyName = "CustomerGroup", SortBy = "Name", Type = ReferenceAttribute.ReferenceType.EntityFramework)]
        public int CustomerGroupId { get; set; }
        [NotMapped]
        public CustomerGroup CustomerGroup { get; set; }

        [Property(Name = "title", Value = "Active")]
        [Property(Name = "type", Value = "bool")]
        public bool IsDelete
        {
            get; set;
        }
    }
}
