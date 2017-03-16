using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Voxteneo.Core.Attributes;
using Voxteneo.Core.Attributes.Presentations;
using Voxteneo.Core.Domains;
using Voxteneo.Core.Domains.Contracts;
using Voxteneo.Core.Domains.LambdaSqlBuilder.Adapter;
using Voxteneo.Core.Domains.UmbracoExtentions;
using Voxteneo.Core.Domains.Uow;
using Voxteneo.Core.Helper;
namespace Voxteneo.Core.Mvc
{
    [Inject]
    public class GenericController : VxController
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenericController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        public ActionResult Index(string model, string state)
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            BaseEntity input = null;
            try
            {
                var manager = Activator.CreateInstance(SqlUnitOfWork.DbContext.GetType()) as DbContext;
                var propertyInfo = SqlUnitOfWork.DbContext.GetType().GetProperty(model);
                var modelType = propertyInfo.PropertyType.GenericTypeArguments[0];
                // assuming JSON.net/Newtonsoft library from http://json.codeplex.com/
                input = JsonConvert.DeserializeObject(json, modelType) as BaseEntity;
                var property = propertyInfo.GetValue(_unitOfWork.GetContext());
                if (state == string.Empty)
                    property.GetType().GetMethod("Add").Invoke(property, new[] { input });
                else if (state == "Delete" || state == "Update")
                {

                    object[] parametersArray = new object[] { new object[] { input.Id } };
                    var method = property.GetType().GetMethod("Find");
                    var data = property.GetType().GetMethod("Find").Invoke(property, parametersArray);
                    if (_unitOfWork.GetContext().Entry(data).State == EntityState.Detached)
                    {
                        property.GetType().GetMethod("Attach").Invoke(property, new[] { data });
                    }
                    if (state == "Delete")
                    {
                        if (data is ISoftDelete)
                        {
                            (data as ISoftDelete).IsDelete = true;
                        }
                        else
                            property.GetType().GetMethod("Remove").Invoke(property, new[] { data });
                    }
                    else
                    {
                        foreach (var item in modelType.GetProperties())
                        {
                            item.SetValue(data, item.GetValue(input));
                        }

                    }

                }

                _unitOfWork.GetContext().SaveChanges();
            }

            catch (Exception ex)
            {
                // Try and handle malformed POST body
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return Json(input);
        }
        [HttpPost]
        public ActionResult Get(BasePagedInput model)
        {
            var name = model.Schema;
            if (name.EndsWith("s"))
                name = name.Substring(0, name.Length - 1);
            var manager = Activator.CreateInstance(SqlUnitOfWork.DbContext.GetType()) as DbContext;
            var propertyInfo = SqlUnitOfWork.DbContext.GetType().GetProperty(name + "s");
            var modelType = propertyInfo.PropertyType.GenericTypeArguments[0];

            var command = _unitOfWork.GetContext().Database.Connection.CreateCommand();
            command.CommandText = "select  " + model.Schema + ".* from " + model.Schema;
            if (!string.IsNullOrEmpty(model.Sorting))
            {
                var property = modelType.GetProperty(model.Sorting);
                if (property != null)
                {
                    if (property.GetCustomAttributes(true).Any(n => n.GetType() == typeof(ReferenceAttribute)))
                    {
                        var referenceAttribute =
                            property.GetCustomAttributes(true).OfType<ReferenceAttribute>().FirstOrDefault();
                        if (referenceAttribute != null)
                        {
                            var propertyType = modelType.GetProperty(referenceAttribute.PropertyName).PropertyType;

                            switch (referenceAttribute.Type)
                            {
                                case ReferenceAttribute.ReferenceType.EntityFramework:
                                    if (propertyType.GenericTypeArguments.Length > 0)
                                    {
                                        propertyType = propertyType.GenericTypeArguments[0];
                                        command.CommandText +=
                                            " LEFT OUTER JOIN " + propertyType.Name + "s ON " + model.Schema + ".Id"
                                             + " = " + propertyType.Name + "s." + modelType.Name + "_Id";
                                    }
                                    else
                                    {
                                        command.CommandText +=
                                         " LEFT OUTER JOIN " + propertyType.Name + "s ON " + model.Schema + "." +
                                         model.Sorting + " = " + propertyType.Name + "s.Id";
                                    }
                                    model.Sorting = propertyType.Name + "s." + referenceAttribute.SortBy;
                                    break;
                                case ReferenceAttribute.ReferenceType.UmbracoDocumentType:
                                    break;

                            }
                        }
                    }
                }
                command.CommandText = command.CommandText + " Order by " + model.Sorting + " " + model.SortingType;

            }
            _unitOfWork.GetContext().Database.Connection.Open();
            var reader = command.ExecuteReader();
            List<object> list = new List<object>();
            var countRecord = 0;
            while (reader.Read())
            {
                if (
                    !(countRecord >= (model.PageCurrent) &&
                      countRecord < (model.PageCurrent + model.PageSize)))
                {
                    countRecord++;
                    continue;
                }
                countRecord++;
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var typeProperty = modelType.GetProperty(reader.GetName(i));
                    if (typeProperty != null)
                    {
                        if (typeProperty.GetCustomAttributes(true).Any(n => n.GetType() == typeof(ReferenceAttribute)))
                        {
                            var referenceAttribute =
                                typeProperty.GetCustomAttributes(true).OfType<ReferenceAttribute>().FirstOrDefault();
                            if (referenceAttribute != null)
                            {
                                var propertyType = modelType.GetProperty(referenceAttribute.PropertyName);

                                switch (referenceAttribute.Type)
                                {
                                    case ReferenceAttribute.ReferenceType.EntityFramework:
                                        var reference = propertyType.PropertyType.Name;
                                        if (!reference.EndsWith("s"))
                                            reference = propertyType.PropertyType.Name + "s";
                                        var dbSet = SqlUnitOfWork.DbContext.GetType().GetProperty(reference).GetValue(manager);
                                        var sqlQuery = dbSet.GetType().GetMethod("SqlQuery").Invoke(dbSet, new object[] { "select * from " + reference +
                                                       " where Id='" + reader[i] + "'",new string[] {""}}) as IEnumerable;
                                        var listReference = new List<object>();
                                        if (sqlQuery != null)
                                            foreach (var data in sqlQuery)
                                            {
                                                listReference.Add(data);
                                            }
                                        if (listReference.Count == 1)
                                            dictionary.Add(propertyType.Name, listReference.FirstOrDefault());
                                        else
                                            dictionary.Add(propertyType.Name, listReference);
                                        break;
                                    case ReferenceAttribute.ReferenceType.UmbracoDocumentType:
                                        var database = Activator.CreateInstance(SqlUnitOfWork.DbContext.GetType()) as DbContext;
                                        if (database != null)
                                        {
                                            var commandMap = database.Database.Connection.CreateCommand();
                                            var query = new UmbracoQueryAdapter().QueryString("cmsContentXml.xml",
                                                "cmsContentXml",
                                                "Where nodeid= '" +
                                                reader[i] + "'", "", "", "");
                                            commandMap.CommandText = query;
                                            database.Database.Connection.Open();
                                            var readerReferenceTypeReader = commandMap.ExecuteReader();
                                            var obj = Activator.CreateInstance(propertyType.PropertyType);
                                            var dictionaryTempReference = new Dictionary<string, PropertyInfo>();
                                            foreach (var property in propertyType.PropertyType.GetProperties())
                                            {
                                                if (!dictionaryTempReference.ContainsKey(property.Name))
                                                    dictionaryTempReference.Add(property.Name, property);
                                            }
                                            while (readerReferenceTypeReader.Read())
                                            {
                                                var xElement = XElement.Parse(readerReferenceTypeReader[0].ToString());
                                                foreach (var key in dictionaryTempReference)
                                                {
                                                    if (key.Key.Equals("Id"))
                                                    {
                                                        var xAttribute = xElement.Attribute("id");
                                                        if (xAttribute != null) key.Value.SetValue(obj, Convert.ChangeType(xAttribute.Value, key.Value.PropertyType));
                                                    }
                                                    else
                                                    {
                                                        var element = xElement.Element(key.Key[0].ToString().ToLower() + key.Key.Substring(1));
                                                        if (element != null)
                                                            key.Value.SetValue(obj, Convert.ChangeType(element.Value, key.Value.PropertyType));
                                                    }
                                                }

                                                dictionary.Add(propertyType.Name, obj);

                                            }
                                            database.Database.Connection.Close();
                                        }

                                        break;

                                }
                            }
                        }
                    }
                    dictionary.Add(reader.GetName(i), reader[i]);
                }
                list.Add(dictionary);
            }
            reader.Dispose();
            _unitOfWork.GetContext().Database.Connection.Close();

            return Json(new BasePagedList<object>()
            {
                PageSize = model.PageSize,
                PageCurrent = model.PageCurrent,
                TotalRecordCount = countRecord,
                Result = "OK",
                Records = list,
                IsLast = model.PageCurrent == Convert.ToInt16(Math.Ceiling(Convert.ToDouble(countRecord) / Convert.ToDouble(model.PageSize)))
            }, JsonRequestBehavior.AllowGet);
        }
    }
    [Inject]
    public class JTableHelperController : VxControllerBase
    {
        public ActionResult GetSchema(string name)
        {
            if (name.EndsWith("s"))
                name = name.Substring(0, name.Length - 1);
            var propertyInfo = SqlUnitOfWork.DbContext.GetType().GetProperty(name + "s");
            var modelType = propertyInfo.PropertyType.GenericTypeArguments[0];
            var properties = modelType.GetProperties()
                .Where(n => n.GetCustomAttributes(true).Any(m => m.GetType() == typeof(PropertyAttribute)));
            var dictionary = new Dictionary<string, object>();
            var dictionarySchema = new Dictionary<string, object>();
            Dictionary<string, object> dictionaryProperty;
            foreach (var property in properties)
            {
                dictionaryProperty = new Dictionary<string, object>();
                foreach (var attribute in property.GetCustomAttributes(true).OfType<PropertyAttribute>().Where(n => n.Section.Equals("Schema")))
                {
                    dictionaryProperty.Add(attribute.Name, attribute.Value);
                }
                if (property.GetCustomAttributes(true).Any(n => n is DisplayAttribute))
                {
                    var firstOrDefault = property.GetCustomAttributes(true).OfType<DisplayAttribute>().FirstOrDefault();
                    if (firstOrDefault != null)
                        dictionarySchema.Add("\"" + firstOrDefault.Name + "\"", dictionaryProperty);
                    else
                        dictionarySchema.Add(property.Name, dictionaryProperty);
                }
                else
                    dictionarySchema.Add(property.Name, dictionaryProperty);
            }
            dictionaryProperty = new Dictionary<string, object>();
            foreach (var attribute in modelType.GetCustomAttributes<CustomActionAttribute>())
            {
                var dict = new Dictionary<string, object>();
                dict.Add("Title", attribute.Value);
                foreach (var item in attribute.Attribute.Split(new[] { ';' }))
                {
                    try
                    {
                        var split = item.Split(new[] { ':' });
                        if (!dict.ContainsKey(split[0]))
                            dict.Add(split[0], split[1].Split(new[] { ',' }));
                    }
                    catch (Exception)
                    {
                    }

                }
                dictionaryProperty.Add(attribute.Name, dict);
            }
            if (dictionaryProperty.Count != 0)
                dictionarySchema.Add("CustomAction", dictionaryProperty);
            dictionary.Add("Schema", dictionarySchema);
            dictionarySchema = new Dictionary<string, object>();
            foreach (var property in properties)
            {
                dictionaryProperty = new Dictionary<string, object>();
                foreach (var attribute in property.GetCustomAttributes(true).OfType<PropertyAttribute>().Where(n => n.Section.Equals("Form")))
                {
                    dictionaryProperty.Add(attribute.Name, attribute.Value);
                }
                if (dictionaryProperty.Count == 0)
                {
                    dictionaryProperty.Add("type", property.PropertyType.Name);
                    dictionaryProperty.Add("title", property.Name.ToStatement());
                }
                dictionaryProperty.Add("id", property.Name);
                if (property.GetCustomAttributes(true).Any(n => n is DisplayAttribute))
                {
                    var firstOrDefault = property.GetCustomAttributes(true).OfType<DisplayAttribute>().FirstOrDefault();
                    if (firstOrDefault != null)
                        dictionarySchema.Add("\"" + firstOrDefault.Name + "\"", dictionaryProperty);
                    else
                        dictionarySchema.Add(property.Name, dictionaryProperty);
                }
                else
                    dictionarySchema.Add(property.Name, dictionaryProperty);
            }
            dictionary.Add("Forms", dictionarySchema);
            return Json(dictionary, JsonRequestBehavior.AllowGet);
        }
    }
}
