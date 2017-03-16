using System;

namespace Voxteneo.Core.Mvc.Routes
{
    public class RoutesEventArg : EventArgs
    {
        public RoutesEventArg(Type item)
        {
            ItemType = item;
        }
        public Type ItemType { get; set; }
    }
}