using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using SimpleInjector;
using Voxteneo.Core.Attributes;

namespace Voxteneo.Core.Mvc
{
    public class VxSimpleInjectorResolver : IDependencyResolver
    {
        private readonly Container _container;
        private readonly IDependencyResolver _current;


        public VxSimpleInjectorResolver(Container container)
        {
            _container = container;
            _current = DependencyResolver.Current;
        }

        object IDependencyResolver.GetService(Type serviceType)
        {

            if (serviceType.GetCustomAttributes(true).Any(n => n.GetType() == typeof(InjectAttribute)))
            {
                var baseType = serviceType;
                var container = baseType.GetField("container", BindingFlags.NonPublic | BindingFlags.Static);
                if (container != null)
                {
                    container.SetValue(null, _container);
                }
                while (container == null && baseType != null)
                {

                    baseType = baseType.BaseType;
                    container = baseType.GetField("container", BindingFlags.NonPublic | BindingFlags.Static);
                    if (container != null)
                        container.SetValue(null, _container);
                }

                var paramters = new List<object>();
                foreach (ConstructorInfo ci in serviceType.GetConstructors().Where(n => n.GetParameters().Count() == serviceType.GetConstructors().Max(x => x.GetParameters().Count())))
                {
                    foreach (var parameter in ci.GetParameters())
                    {
                        var type = _container.GetInstance(parameter.ParameterType);
                        paramters.Add(type);
                    }
                    return ci.Invoke(paramters.ToArray());
                }
            }
            return _current.GetService(serviceType);
        }
        IEnumerable<object> IDependencyResolver.GetServices(Type serviceType)
        {
            return _current.GetServices(serviceType);
        }
    }
}