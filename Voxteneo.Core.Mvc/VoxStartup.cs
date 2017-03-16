using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using Voxteneo.Core.Attributes;
using Voxteneo.Core.Domains.Attributes;
using Voxteneo.Core.Mvc.Routes;

namespace Voxteneo.Core.Mvc
{
    public class VoxStartup
    {
        public static event EventHandler<RoutesEventArg> RegisterRoutes;
        public static void Initialize()
        {
            var currentDomain = typeof(string).GetTypeInfo().Assembly.GetType("System.AppDomain")
                .GetRuntimeProperty("CurrentDomain").GetMethod.Invoke(null, new object[] { });
            var getAssemblies = currentDomain.GetType().GetRuntimeMethod("GetAssemblies", new Type[] { });
            var assemblies = getAssemblies.Invoke(currentDomain, new object[] { }) as Assembly[];

            if (assemblies == null)
                return;

            var list = new List<string>();
            var cacheLanguages = new Dictionary<string, StringBuilder>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    var languages = Enum.GetValues(typeof(Enums.LanguageFormat)).Cast<Enums.LanguageFormat>().Select(x => x.ToString()).ToList();

                    foreach (var item in assembly.GetTypes())
                    {
                        var resource = item.GetField("resourceMan", BindingFlags.NonPublic | BindingFlags.Static);
                        list.Add(item.FullName);
                        if (resource != null)
                        {
                            if (item.GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.Static) !=
                                null)
                            {
                                var resouceManager =
                                    (ResourceManager)
                                    item.GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.Static)
                                        .GetValue(item);
                                foreach (var language in languages)
                                {
                                    StringBuilder stringBuilder;
                                    if (!cacheLanguages.TryGetValue(language, out stringBuilder))
                                    {
                                        stringBuilder = new StringBuilder();
                                        stringBuilder.AppendLine("//" + DateTime.Now);
                                        stringBuilder.AppendLine("//Generate By Vox Tenoe Core");
                                        stringBuilder.AppendLine("//");
                                        cacheLanguages.Add(language, stringBuilder);
                                    }
                                    stringBuilder.Append("var " + item.FullName.Replace(".", "_") + " = {");

                                    var first = true;
                                    foreach (
                                        var fieldInfo in
                                        item.GetProperties(BindingFlags.Public | BindingFlags.Static))
                                    {

                                        var name = resouceManager.GetString(fieldInfo.Name,
                                                        System.Globalization.CultureInfo.GetCultureInfo(language)) ??
                                                    "";
                                        if (first)
                                            stringBuilder.Append("    " + fieldInfo.Name + ":\"" + name.Replace("\"", "\\\"").Trim() + "\"\n");
                                        else
                                            stringBuilder.Append("    , " + fieldInfo.Name + ":\"" + name.Replace("\"", "\\\"").Trim() + "\"\n");
                                        first = false;
                                    }
                                    stringBuilder.Append("} \n");
                                }
                            }
                        }

                        if (item.GetCustomAttributes(true).Any(n => n.GetType() == typeof(AutoMapperAttribute)))
                        {
                            foreach (var profile in item.GetCustomAttributes(true).OfType<AutoMapperAttribute>())
                            {
                                Mapper.Initialize(x =>
                                {
                                    x.AddProfile((Profile)Activator.CreateInstance(profile.Map));
                                });
                            }

                        }

                        if (item.GetCustomAttributes(true).Any(n => n.GetType() == typeof(InjectAttribute)))
                        {
                            OnRegisterRoutes(new RoutesEventArg(item));

                        }

                    }
                }
                catch (Exception exception)
                {
                    list.Add("Error " + assembly.FullName + " " + exception);
                }

            }
            try
            {
                var path = Configurations.ScriptPath;
                if (string.IsNullOrEmpty(Configurations.ScriptPath))
                {
                    if (HttpContext.Current.Request != null)
                        path = HttpContext.Current.Request.MapPath("~/Scripts/Language");
                }
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!path.EndsWith("\\"))
                    path += "\\";

                foreach (var cacheLanguage in cacheLanguages)
                {
                    File.WriteAllText(path + cacheLanguage.Key + ".js", cacheLanguage.Value.ToString());
                }
            }
            catch (Exception e)
            {
                
            }
           
        }

        protected static void OnRegisterRoutes(RoutesEventArg e)
        {
            RegisterRoutes?.Invoke(null, e);
        }
    }
}
