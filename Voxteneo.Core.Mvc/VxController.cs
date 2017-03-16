using System.Threading;
using System.Web;
using Voxteneo.Core.Mvc;
using Voxteneo.WebComponents.Logger;

namespace Voxteneo.Core.Mvc
{
    public class VxController : VxControllerBase
    {
        public VxController()
        {
            
        }
        public VxController(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.GetLogger(this.GetType().Name);
        }

    }
}
