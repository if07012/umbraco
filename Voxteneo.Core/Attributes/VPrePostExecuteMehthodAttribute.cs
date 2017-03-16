using Castle.DynamicProxy;

namespace Voxteneo.Core.Attributes
{
    public class VPrePostExecuteMehthodAttribute : VPreExecuteMehthodAttribute
    {
        public IInvocation Invoke { get; internal set; }

        public object ReturnValue { get;  set; }

        public virtual void OnPostExecute()
        {

        }
    }
}
