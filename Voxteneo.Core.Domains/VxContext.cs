using System;
using System.Data.Entity;

namespace Voxteneo.Core.Domains
{
    public class VxContext : DbContext
    {
        public VxContext(string connection) : base(connection)
        {

        }
        public VxContext()
        {
            foreach (var item in GetType().GetProperties())
            {
                var a = item.PropertyType.GenericTypeArguments;
                // item.SetValue(this, new VxSet(this));
                Console.WriteLine(item);
            }
        }
    }
}
