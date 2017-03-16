using Voxteneo.Core.Attributes.Presentations;
using Voxteneo.Core.Domains;

namespace Learning.Entities
{
    public class CategoryType : BaseModifiy
    {
        [Property(Name = "title",Value = "Name Category")]
        public string Name { get; set; }
    }
}