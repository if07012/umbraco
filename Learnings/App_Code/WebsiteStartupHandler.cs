using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SimpleInjector;
using Umbraco.Core;
using Voxteneo.Core.Mvc.Routes;
using Voxteneo.WebCompoments.NLogLogger;
using Voxteneo.WebComponents.Logger;

namespace Learnings.App_Code
{
    /// <summary>
    /// Website Startup Handler
    /// </summary>
    public class WebsiteStartupHandler : IApplicationEventHandler
    {
        /// <summary>
        /// On Application Started
        /// </summary>
        /// <param name="umbracoApplication">Umbraco Application</param>
        /// <param name="applicationContext">Application Context</param>
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            umbracoApplication.PreRequestHandlerExecute+=UmbracoApplicationOnPreRequestHandlerExecute;
            
            RouteTable.Routes.MapMvcAttributeRoutes();
            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Create container
           
           
        }

        private void UmbracoApplicationOnPreRequestHandlerExecute(object sender, EventArgs eventArgs)
        {
            
        }

      

        public void OnApplicationInitialized(
            UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarting(
            UmbracoApplicationBase umbracoApplication,
            ApplicationContext applicationContext)
        {

            RouteTable.Routes.MapRoute("GetCart", "Umbraco/Api/Cart", new { controller = "Cart", action = "Index" });
        }
    }
}