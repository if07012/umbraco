using System;

namespace Voxteneo.Core.Attributes
{
    public class VPostExecuteMehthodAttribute : VBaseAttribute
    {
        public Type ReturnType { get; internal set; }
        public object ReturnValue { get; set; }
    }
}
