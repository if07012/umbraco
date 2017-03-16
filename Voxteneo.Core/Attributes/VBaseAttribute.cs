using System;
using System.Reflection;

namespace Voxteneo.Core.Attributes
{
    public class VBaseAttribute : Attribute
    {
        public object[] Arguments { get; set; }

        public object Object { get; internal set; }

        public ParameterInfo[] Parameters { get; internal set; }

        public object Target { get; internal set; }

        public string MethodName => MethodeInfo.Name.Replace("set_", "").Replace("get_", "");

        public bool Handle { get; set; }

        public MethodInfo MethodeInfo { get; set; }

        protected internal virtual void OnExecute()
        {

        }
    }
}
