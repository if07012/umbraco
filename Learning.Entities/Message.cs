using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Voxteneo.Core.Attributes.Presentations;
using Voxteneo.Core.Domains;

namespace Learning.Entities
{
    public class Message : BaseModifiy
    {
        [NotMapped]
        public Profile Person { get; set; }

        [Property(Name = "title", Value = "Person")]
        [Property(Name = "display", Value = "(function (data) {return data.record.Person.FullName;})")]
        [Property(Name = "sorting", Value = "false")]
        [Reference(PropertyName = "Person", Type = ReferenceAttribute.ReferenceType.UmbracoDocumentType)]
        public int PersonId { get; set; }
        [Property(Name = "title", Value = "Content Message")]
        public string ContentMessage { get; set; }
    }
}