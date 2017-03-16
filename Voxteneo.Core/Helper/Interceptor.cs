using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using Voxteneo.Core.Attributes;

namespace Voxteneo.Core.Helper
{
    public class Interceptor : IInterceptor
    {
        private List<TAttribute> GetAttributes<TAttribute>(IInvocation invocation)
        {
            var field = invocation.Proxy.GetType().GetField("__target",
                 System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                 | System.Reflection.BindingFlags.Instance
                 | System.Reflection.BindingFlags.SetField
                 | System.Reflection.BindingFlags.GetField
                 | System.Reflection.BindingFlags.CreateInstance);
            var attributes = invocation.MethodInvocationTarget.GetCustomAttributes(true).OfType<TAttribute>().ToList();

            if (invocation.MethodInvocationTarget.ReflectedType != null)
                attributes.AddRange(invocation.MethodInvocationTarget.ReflectedType.GetCustomAttributes(true).OfType<TAttribute>());

            if (field == null)
                return attributes;

            var property = field.GetValue(invocation.Proxy).GetType().GetProperty(invocation.Method.Name.Replace("set_", ""));

            if (property != null)
                attributes.AddRange(property.GetCustomAttributes(true).OfType<TAttribute>());

            return attributes;
        }

        public void Intercept(IInvocation invocation)
        {
            var exceptionAttributes = GetAttributes<VExceptionAttribute>(invocation);
            var preAttributes = GetAttributes<VPreExecuteMehthodAttribute>(invocation);
            var postAttributes = GetAttributes<VPostExecuteMehthodAttribute>(invocation);
            try
            {
                var handle = false;
                if (preAttributes.Any())
                    foreach (var item in preAttributes)
                    {
                        if (invocation.Method.GetParameters().Any())
                        {
                            item.Parameters = invocation.Method.GetParameters();
                            item.Arguments = invocation.Arguments;
                        }

                        handle = ExecutePreIntercept(invocation, handle, item);

                        if (!(item is VPrePostExecuteMehthodAttribute))
                            continue;

                        if ((item as VPrePostExecuteMehthodAttribute).ReturnValue != null)
                        {
                            invocation.ReturnValue = (item as VPrePostExecuteMehthodAttribute).ReturnValue;
                            return;
                        }

                        (item as VPrePostExecuteMehthodAttribute).Invoke = invocation;
                    }

                if (!handle)
                    invocation.Proceed();

                ExecutePrePostIntercept(preAttributes);
            }
            catch (Exception exception)
            {
                ExecuteCatchExecption(invocation, exceptionAttributes, exception);
            }
            finally
            {
                ExecutePostIntercept(invocation, postAttributes);
            }
        }

        private static void ExecutePostIntercept(IInvocation invocation, List<VPostExecuteMehthodAttribute> postAttributes)
        {
            if (!postAttributes.Any())
                return;

            foreach (var item in postAttributes)
            {
                item.MethodeInfo = invocation.Method;
                item.ReturnValue = invocation.ReturnValue;
                item.Arguments = invocation.Arguments;
                if (invocation.Method.Name.Contains("set_"))
                {
                    if (invocation.Method.GetParameters().Any())
                    {
                        item.ReturnType = invocation.Method.GetParameters()[0].ParameterType;
                        item.Parameters = invocation.Method.GetParameters();
                        item.Arguments = invocation.Arguments;
                    }

                }
                item.Object = invocation.Proxy;

                var field = invocation.GetType().GetField("target",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.SetField
                    | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.CreateInstance);
                //   item.ReturnType = invocation.MethodInvocationTarget.
                if (field != null)
                    item.Target = field.GetValue(invocation);
                invocation.ReturnValue = item.ReturnValue;
                item.OnExecute();
            }
        }

        private static void ExecuteCatchExecption(IInvocation invocation, List<VExceptionAttribute> exceptionAttributes, Exception exception)
        {
            if (exceptionAttributes.Any())
            {
                foreach (var item in exceptionAttributes)
                {
                    item.MethodeInfo = invocation.Method;
                    item.Exception = exception;
                    item.OnExecute();
                }
            }
            else
            {
                throw exception;
            }
        }

        private static void ExecutePrePostIntercept(List<VPreExecuteMehthodAttribute> preAttributes)
        {
            if (!preAttributes.OfType<VPrePostExecuteMehthodAttribute>().Any())
                return;

            foreach (var item in preAttributes.OfType<VPrePostExecuteMehthodAttribute>())
            {
                item.OnPostExecute();
            }
        }

        private static bool ExecutePreIntercept(IInvocation invocation, bool handle, VPreExecuteMehthodAttribute item)
        {
            item.MethodeInfo = invocation.Method;
            item.Object = invocation.Proxy;
            item.OnExecute();
            if (item.Handle)
                handle = true;
            return handle;
        }
    }

}
