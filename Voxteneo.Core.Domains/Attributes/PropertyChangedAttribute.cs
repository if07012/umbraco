using System.Collections.Generic;
using Voxteneo.Core.Attributes;

namespace Voxteneo.Core.Domains.Attributes
{
    public class PropertyChangedAttribute : VPostExecuteMehthodAttribute
    {
        protected override void OnExecute()
        {
            var obj = this.Object;
            var dictionaryField = obj.GetType().GetField("___internalData", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.SetField
                    | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.CreateInstance);
            var dictionary = (Dictionary<string, object>)dictionaryField?.GetValue(obj);

            object data = Parameters[0].GetType();

            var isModified = false;

            if (dictionary != null && !dictionary.TryGetValue(this.Parameters[0].Name, out data))
            {
                isModified = true;
                dictionary.Add(Parameters[0].Name, this.Arguments[0]);
            }
            else
            {
                if (Arguments[0] != data)
                    isModified = true;
            }

            if (isModified)
            {
                var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                                | System.Reflection.BindingFlags.Instance
                                | System.Reflection.BindingFlags.SetField
                                | System.Reflection.BindingFlags.GetField
                                | System.Reflection.BindingFlags.CreateInstance;
                var method = this.Object.GetType().GetMethod("OnPropertyChanged", flags);
                method.Invoke(this.Object, flags, null, new object[] { MethodName }, null);
            }

            base.OnExecute();
        }
    }
}
