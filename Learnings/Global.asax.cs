using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Learning.Entities.Repositories;
using Learning.Entities.Services;
using Learning.Service.EntityFramework;
using Learning.Service.Services;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using Umbraco.Web;
using Voxteneo.Core.Domains.Contracts;
using Voxteneo.Core.Domains.LambdaSqlBuilder;
using Voxteneo.Core.Domains.LambdaSqlBuilder.Adapter;
using Voxteneo.Core.Domains.Uow;
using Voxteneo.Core.Mvc;
using Voxteneo.Core.Mvc.Routes;
using Voxteneo.WebCompoments.NLogLogger;
using Voxteneo.WebComponents.Logger;

namespace Learnings
{
    public class MvcApplication : UmbracoApplication
    {
        protected void Application_Start()
        {


        }
        public void Init(HttpApplication application)
        {
            application.PreRequestHandlerExecute += application_PreRequestHandlerExecute;
            application.BeginRequest += this.Application_BeginRequest;
            application.EndRequest += this.Application_EndRequest;
            application.Error += Application_Error;
        }

        private void application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            try
            {
                if (Session != null && Session.IsNewSession)
                {
                    // Your code here
                }
            }
            catch (Exception ex) { }
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                // You begin request code here
            }
            catch { }
        }

        private void Application_EndRequest(object sender, EventArgs e)
        {
            // Your code here
        }

        protected new void Application_Error(object sender, EventArgs e)
        {
            // Your error handling here
        }

        protected override void OnApplicationStarted(object sender, EventArgs e)
        {
            SqlUnitOfWork.DbContext = new LearningContext();
            SqlLamBase._defaultAdapter = new UmbracoQueryAdapter();
            var webLifestyle = new WebRequestLifestyle();
            var container = new Container();
            container.Register<ILogger, NLogLogger>(webLifestyle);
            container.Register<IProfileRepository, ProfileRepository>();
            container.Register<IMessageRepository, MessageRepository>();
            
            container.Register<IProfileService, ProfileService>();
            container.Register<IUnitOfWork, SqlUnitOfWork>(webLifestyle);

            container.Verify();
            DependencyResolver.SetResolver(new VxSimpleInjectorResolver(container));
            base.OnApplicationStarted(sender, e);
            Voxteneo.Core.Mvc.VoxStartup.RegisterRoutes += VoxStartupOnRegisterRoutes;
            Voxteneo.Core.Mvc.VoxStartup.Initialize();

            // Your code here
        }
        private void VoxStartupOnRegisterRoutes(object sender, RoutesEventArg e)
        {
            var name = e.ItemType.Name.Replace("Controller", "");
            RouteTable.Routes.MapRoute(
           name,
           "VoxTeneo/" + name + "s/{action}/{id}",
           new
           {
               controller = name,
               action = "Index",
               id = UrlParameter.Optional
           });
        }


    }
}
