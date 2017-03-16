using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;

namespace Voxteneo.Core.Mvc
{
    public class VxScriptController : Controller
    {

        protected override void HandleUnknownAction(string actionName)
        {
            var file = this.Request.CurrentExecutionFilePath.Replace("VxScript", "Views");
            if (System.IO.File.Exists(Request.MapPath("~" + file + ".js")))
            {
                var pathFile = Request.MapPath("~" + file + ".js");
                SetCache(pathFile);
                this.Response.WriteFile(pathFile);
                return;
            }
            base.HandleUnknownAction(actionName);
        }

        public VxScriptController()
        {
            var path = Configurations.ScriptPath;
            if (string.IsNullOrEmpty(Configurations.ScriptPath))
            {
                path = System.Web.HttpContext.Current.Request.MapPath("~/Scripts/Language");
            }
            if (!path.EndsWith("\\"))
                path += "\\";
            PathScript = path;
            
        }

        public string PathScript { get; set; }
        private static bool hasCreateCore = false;
        public ActionResult LanguageScript(string language)
        {
            var file = PathScript + language + ".js";
            SetCache(file);
            return File(file, "text/javascript");
        }

        private void SetCache(string pathFile)
        {
            string rawIfModifiedSince = Request.Headers.Get("If-Modified-Since");
            if (rawIfModifiedSince != null)
            {
                DateTime ifModifiedSince = DateTime.Parse(rawIfModifiedSince);
                var lastModified = System.IO.File.GetLastWriteTime(pathFile);
                // HTTP does not provide milliseconds, so remove it from the comparison
                if (lastModified.ToString(CultureInfo.InvariantCulture).Equals(ifModifiedSince.ToString(CultureInfo.InvariantCulture)))
                {
                    // The requested file has not changed
                    Response.StatusCode = 304;
                }
            }
            Response.Cache.SetLastModified(System.IO.File.GetLastWriteTime(pathFile));
        }

        public ActionResult CoreScript()
        {
            if (!hasCreateCore)
            {
                var coreBuilder = new StringBuilder();
                coreBuilder.AppendLine("function ActionLink(actionName,controllerName){");
                coreBuilder.AppendLine("    return \"" + Url.Action().Replace("VxScript/CoreScript", "") +
                                       "\"+controllerName+\"/\"+actionName");
                coreBuilder.AppendLine("}");
                System.IO.File.WriteAllText(PathScript + "vxcore.js", coreBuilder.ToString());
                hasCreateCore = true;
            }
            var pathFile = PathScript + "vxcore.js";
            SetCache(pathFile);
            return File(PathScript + "vxcore.js", "text/javascript");
        }
    }
}