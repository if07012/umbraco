using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;

namespace Voxteneo.Core.Attributes
{
    public class NotNullAttribute : VPostExecuteMehthodAttribute
    {
        protected internal override void OnExecute()
        {
            if (ReturnValue == null)
            {
                throw new ArgumentException(MethodName + Voxteneo.Core.Properties.Resources.IsNullValue);
            }
        }
    }

    public class BetweenAttribute : VPostExecuteMehthodAttribute
    {
        public object MinDate { get; set; }
        public object MaxDate { get; set; }

        private Delegate CreateExpression(Type objectType, string expression)
        {
            // TODO - add caching
            var lambdaExpression = System.Linq.Dynamic.DynamicExpression.ParseLambda(objectType, typeof(bool), expression);
            var func = lambdaExpression.Compile();
            return func;
        }

        protected internal override void OnExecute()
        {
            if (ReturnType != typeof(DateTime))
                return;

            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = true
            };

            // True - exe file generation, false - dll file generation
            var results = provider.CompileAssemblyFromSource(parameters, MaxDate.ToString());

            var lambdaExpression = System.Linq.Dynamic.DynamicExpression.ParseLambda(MethodeInfo.DeclaringType, 
                typeof(bool), "Between > DateTime.Now");
            var func = lambdaExpression.Compile();
                
            var result = (bool)func.DynamicInvoke(this.Target);
            //Delegate conditionFunction = CreateExpression(this.MethodeInfo.DeclaringType, );
            //bool conditionMet = (bool)conditionFunction.DynamicInvoke(validationContext.ObjectInstance);
            var currentValue = (DateTime)ReturnValue;
            //if (currentValue < minDate)
            //{
            //    throw new ArgumentException(MethodName + Voxteneo.Core.Properties.Resources.LessMinValue);
            //}

            //if (currentValue > maxDate)
            //{
            //    throw new ArgumentException(MethodName + Voxteneo.Core.Properties.Resources.GreaterMaxValue);
            //}
        }
    }
}
