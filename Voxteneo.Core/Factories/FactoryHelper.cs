using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using Voxteneo.Core.Helper;

namespace Voxteneo.Core.Factories
{
    public class FactoryHelper
    {
        public static T Create<T>()
        {
            var generator = new ProxyGenerator(new VoxProxyBuilder());
            if (typeof(T).IsClass)
            {
                var proxy = CreateClass<T>(generator);
                return (T)proxy;
            }
            else if (typeof(T).IsInterface)
            {
                var proxy = CreateInterface<T>(generator);
                return (T)proxy;
            }
            return default(T);
        }

        private static object CreateInterface<T>(ProxyGenerator generator)
        {
            var obj = generator.CreateClassProxyWithTarget(typeof(T), DependecyInjection.Get(typeof(T)), new Interceptor());
            obj.GetType().GetField("___internalData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                 | System.Reflection.BindingFlags.Instance
                 | System.Reflection.BindingFlags.SetField
                 | System.Reflection.BindingFlags.GetField
                 | System.Reflection.BindingFlags.CreateInstance)?.SetValue(obj, new Dictionary<string, object>());
            return obj;
        }

        private static object CreateClass<T>(ProxyGenerator generator)
        {
            var options = new ProxyGenerationOptions();
            var pgo = new ProxyGenerationOptions();
            var obj = generator.CreateClassProxyWithTarget(typeof(T), Activator.CreateInstance<T>(), pgo, new Interceptor());
            obj.GetType().GetField("___internalData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                 | System.Reflection.BindingFlags.Instance
                 | System.Reflection.BindingFlags.SetField
                 | System.Reflection.BindingFlags.GetField
                 | System.Reflection.BindingFlags.CreateInstance)?.SetValue(obj, new Dictionary<string, object>());
            return obj;
        }
    }
}