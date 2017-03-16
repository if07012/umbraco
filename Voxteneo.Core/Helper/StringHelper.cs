using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Voxteneo.Core.Helper
{
    public static class StringHelper
    {
        public static string ToJsonInformation(this object model)
        {
            var builder = new StringBuilder();
            try
            {
                foreach (var propertyInfo in model.GetType().GetProperties())
                {
                    builder.Append(propertyInfo.Name + " : \n" + JsonConvert.SerializeObject(propertyInfo.GetValue(model)) + "\n");
                }
            }
            catch (Exception e)
            {
                return "";
            }

            return builder.ToString();
        }
        public static string ToStatement(this string text)
        {
            var arrText = text.ToCharArray();
            var first = true;
            StringBuilder str = new StringBuilder();
            foreach (var i in arrText)
            {
                if (first)
                    str.Append(i.ToString().ToUpper());

                if (!first)
                {
                    if (Char.IsUpper(i))
                    {
                        str.Append(" " + i.ToString().ToUpper());
                    }
                    else
                        str.Append(i);
                }
                first = false;


            }
            return str.ToString();
        }
        public static string CreateValidUrl(this string url)
        {
            if (url.EndsWith(Constants.BackSlash))
                return url;
            return url + Constants.BackSlash;
        }

        public static string ToJson(this object model)
        {
            return JsonConvert.SerializeObject(model);
        }

        public static Dictionary<string, object> FromJsonToDictionary(this string model)
        {
            var dictionary = new Dictionary<string, object>();
            var data = JsonConvert.DeserializeObject(model);
            if (data is JArray)
            {

                var array = data as JArray;
                foreach (var arr in array)
                {
                    Dictionary<string, object> dict = ComposeDictionary(arr as JObject);
                    dictionary.Add((dictionary.Count + 1).ToString(), dict);
                }
            }
            else if (data is JObject)
            {
                dictionary = ComposeDictionary(data as JObject);
            }
            return dictionary;
        }

        private static Dictionary<string, object> ComposeDictionary(JObject jObject)
        {
            var result = new Dictionary<string, object>();
            foreach (dynamic obj in jObject)
            {
                var value = obj.Value as JValue;
                if (value == null) continue;
                if (value.Value is JObject)
                    result.Add(obj.Key, ComposeDictionary((JObject)value.Value));
                else if (value.Value is JArray)
                {
                    var array = value.Value as JArray;
                    foreach (var arr in array)
                    {
                        Dictionary<string, object> dict = ComposeDictionary(arr as JObject);
                        result.Add((result.Count + 1).ToString(), dict);
                    }
                }
                else
                    result.Add(obj.Key, value.Value);

            }
            return result;
        }
    }
}
