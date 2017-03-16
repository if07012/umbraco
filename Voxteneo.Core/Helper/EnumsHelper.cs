using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Voxteneo.Core.Helper
{
    public static class EnumsHelper
    {
        public static System.Resources.ResourceManager ResourceManager { get; set; }
        /// <summary>
        /// Return the correct text depending of Enums
        /// </summary>
        /// <returns></returns>
        public static string GetResourceDisplayEnum(Enum objEnum)
        {
            try
            {
                var message =ResourceManager?.GetString(objEnum.GetType().Name + "_" + objEnum);

                return string.IsNullOrWhiteSpace(message) ? objEnum.ToString() : message;
            }
            catch (Exception)
            {
                return objEnum.ToString();
            }
        }

        public static string GetResourceDisplayEnum(string enumName, string enumValue)
        {
            try
            {
                var message = ResourceManager?.GetString(enumName + "_" + enumValue);

                return string.IsNullOrWhiteSpace(message) ? enumValue : message;
            }
            catch (Exception)
            {
                return enumValue;
            }
        }

        public static Dictionary<int, string> GetEnumIdsAndResourceDisplay<T>() where T : struct
        {
            var enumValues = Enum.GetValues(typeof(T)).Cast<T>().Select(
                t => new { key = Convert.ToInt32(t), value = GetResourceDisplayEnum(t.GetType().Name, t.ToString()) })
                .ToDictionary(t => t.key, t => t.value);
            
            return enumValues;
        }

        public static Dictionary<string, string> GetEnumAndResourceDisplay<T>() where T : struct
        {
            var enumValues = Enum.GetValues(typeof(T))
                                         .Cast<T>()
                                         .Select(t => new
                                         {
                                             key = t.ToString(),
                                             value = GetResourceDisplayEnum(t.GetType().Name, t.ToString())
                                         })
                                        .ToDictionary(t => t.key, t => t.value);
            return enumValues;
        }

        public static string GetDescription(Enum en)
        {
            var type = en.GetType();

            var memInfo = type.GetMember(en.ToString());

            if (memInfo.Length <= 0)
                return en.ToString();

            var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : en.ToString();
        }


        /// <summary>
        /// Filter the enum based on an attribute
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns></returns>
        public static IEnumerable<TEnum> FilterEnumWithAttributeOf<TEnum, TAttribute>()
            where TEnum : struct
            where TAttribute : class
        {
            foreach (var field in
                typeof(TEnum).GetFields(BindingFlags.GetField |
                                         BindingFlags.Public |
                                         BindingFlags.Static))
            {
                if (field.GetCustomAttributes(typeof(TAttribute), false).Length > 0)
                    yield return (TEnum)field.GetValue(null);
            }
        }

        public static string GetEnumCssClass(Enum objEnum)
        {
            var description = GetDescription(objEnum);

            switch (description)
            {
                case "Success":
                    description = "ion-ios-checkmark-outline ";
                    break;

                case "Error":
                    description = "ion-ios-close-outline";
                    break;

                case "Warning":
                    description = "alert-warning";
                    break;

                case "Info":
                    description = "alert-info";
                    break;

                default:
                    description = "";
                    break;
            }

            return description;
        }

        public static T GetEnumValueByName<T>(string enumName)
        {
            return (T) Enum.Parse(typeof (T), enumName);
        }
    }
}