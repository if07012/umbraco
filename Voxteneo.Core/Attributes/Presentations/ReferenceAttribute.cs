using System;

namespace Voxteneo.Core.Attributes.Presentations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ReferenceAttribute : Attribute
    {
        public enum ReferenceType
        {
            EntityFramework,
            UmbracoDocumentType
        }
        public string PropertyName { get; set; }
        public ReferenceType Type { get; set; }

        public string SortBy { get; set; }
    }
}