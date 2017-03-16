using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxteneo.Core.Attributes.Presentations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CustomActionAttribute : PropertyAttribute
    {
        public string Attribute { get; set; }

        public override string Section
        {
            get
            {
                return "CustomAction";
            }

            set
            {
                base.Section = value;
            }
        }
    }
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class PropertyAttribute : Attribute
    {
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }

        public virtual string Section { get; set; } = "Schema";
    }
}
