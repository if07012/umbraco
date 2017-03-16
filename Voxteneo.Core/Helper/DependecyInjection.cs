using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using SimpleInjector;
using Voxteneo.Core.Attributes;

namespace Voxteneo.Core.Helper
{
    public class DependecyInjection
    {
        private static Container _container;
        private static Dictionary<Type, object> _listDependencyInjection;

        public static void Add<TSource,TImplement>()
        {
            Add(typeof(TSource), typeof(TImplement));
        }

        public static void Add<TSource>(object model)
        {
            Add(typeof(TSource), model);
        }

        public static void Add(Type key, object model)
        {
            if (_listDependencyInjection == null)
                _listDependencyInjection = new Dictionary<Type, object>();
            if (!_listDependencyInjection.ContainsKey(key))
                _listDependencyInjection.Add(key, model);
        }

        public static TSource Get<TSource>() where TSource : class
        {
            return Get(typeof(TSource)) as TSource;
        }

        public static object Get(Type type)
        {
            var generator = new ProxyGenerator();

            if (!_listDependencyInjection.ContainsKey(type))
                return null;

            var result = _listDependencyInjection[type];
            if (result.GetType().Name != "RuntimeType") return 
                    result;

            object proxy = null;

            if (result is Type)
            {
                var ci = (result as Type).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic|BindingFlags.Public, 
                    null, new Type[0], null);

                var resourceObject = ci.Invoke(ci.GetParameters().Select(item => Get(item.ParameterType)).ToArray());
                proxy = generator.CreateClassProxyWithTarget(type, resourceObject, new Interceptor());

                return proxy;
            }

            var fields = GetFieldsInject((Type) result);
            var instance = Activator.CreateInstance((Type)result);
            foreach (var field in fields)
                field.SetValue(instance, Get(field.FieldType));
            proxy = generator.CreateClassProxyWithTarget(
                type, instance, new Interceptor());
            return proxy;
        }

        private static IEnumerable<FieldInfo> GetFieldsInject(Type result)
        {
            var fields = result.GetTypeInfo().DeclaredFields.Where(
                n => n.GetCustomAttributes(true).Any(c => c.GetType() == typeof(InjectAttribute)));
            //repeat fields in current class
            var name = result.Name;
            foreach (var field in fields)
                yield return field;
        }

        public static void Initialize()
        {
            if (_container == null)
                _container = new Container();
        }
        public static void Register<TSource, TImplement>() where TSource : class
            where TImplement : class, TSource
        {
           
        }
    }
}
