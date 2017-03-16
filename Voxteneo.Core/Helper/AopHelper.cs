using Castle.DynamicProxy;

namespace Voxteneo.Core.Helper
{
    public class AopHelper
    {
        public static object CreateObject(object model)
        {
            var generator = new ProxyGenerator();
            //var proxy = generator.CreateClassProxyWithTarget(
            // model.GetType(), generator.CreateClassProxy(model.GetType()), new Interceptor());
            var options = new ProxyGenerationOptions();

            var proxy = generator.CreateClassProxyWithTarget(
            model.GetType(), model, options, new Interceptor());
            foreach (var property in model.GetType().GetProperties())
            {
                var obj = property.GetValue(model);
                property.SetValue(proxy, obj);
            }

            return proxy;
        }

        public static T CreateObject<T>(object model)
        {
            return (T)CreateObject(model);
        }
    }
}
