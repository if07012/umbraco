using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using SimpleInjector;
using Voxteneo.Core.Attributes;
using Voxteneo.Core.Exceptions;
using Voxteneo.Core.Helper;
using Voxteneo.Core.Mvc.Application;
using Voxteneo.Core.Mvc.Models;
using Voxteneo.WebComponents.Logger;

namespace Voxteneo.Core.Mvc
{
    public class VxControllerBase : Controller
    {
        protected ILogger Logger;
        protected List<Message> ListMessage { get; set; }
        [Inject]
        private static Container container;
        public VxControllerBase()
        {
            
            var properties =
                this.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance);
            foreach (var property in properties.Where(n => n.GetCustomAttributes(true).Any(x => x.GetType() == typeof(InjectAttribute))))
            {
                var obj = container.GetInstance(property.PropertyType);
                property.SetValue(this, obj);
            }

            var fields =
                this.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance);
            foreach (var field in fields.Where(n => n.GetCustomAttributes(true).Any(x => x.GetType() == typeof(InjectAttribute))))
            {
                var obj = container.GetInstance(field.FieldType);
                field.SetValue(this, obj);
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Configurations.UseAopModeling)
                if (filterContext.ActionParameters.ContainsKey("model"))
                {
                    var model = filterContext.ActionParameters["model"];
                    filterContext.ActionParameters["model"] = AopHelper.CreateObject(model);
                }
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Handles the exceptions if any. Sets the property Success to true if no error thrown.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        protected void HandleExceptions(System.Action func)
        {
            Success = false;
            try
            {
                func();
                Success = true;
            }
            catch (FileSizeExceededException fseex)
            {
                Logger.Error("FileSizeExceededException: ", fseex.Message);
                AddMessage(fseex.Message, Message.MessageTypes.Error);
                ModelState.AddModelError(String.Empty, fseex.Message);
            }
            catch (FileExtensionException feex)
            {
                Logger.Error("FileExtensionException: ", feex.Message);
                AddMessage(feex.Message, Message.MessageTypes.Error);
                ModelState.AddModelError(String.Empty, feex.Message);
            }
            //Business Level Logic Exception
            catch (BLLException bex)
            {
                string message = null;
                if (bex.Message == "" && bex.ErrorCodes.Count > 0)
                {
                    message = string.Join("<br/>", bex.ErrorCodes.ToArray());
                }
                else
                {
                    message = bex.Message;
                }
                Logger.Error("BllException: ", message);
                AddMessage(message, Message.MessageTypes.Error);
                ModelState.AddModelError(String.Empty, bex.Message);

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Logger.Error(ex.StackTrace);
                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    Logger.Error(innerException.Message);
                    Logger.Error(innerException.StackTrace);

                    innerException = innerException.InnerException;
                }
                AddMessage("An unexpected error occurred", Message.MessageTypes.Error);
                ModelState.AddModelError(String.Empty, ex);

            }
        }

        protected bool Success { get; set; }
        /// <summary>
        /// Adds the message into tempdata list
        /// </summary>
        /// <param name="Message">The message.</param>
        private void AddMessage(Message message)
        {
            ListMessage = (List<Message>)TempData["Message"] ?? new List<Message>();
            ListMessage.Add(message);
            TempData["Message"] = ListMessage;

            Logger.Debug(" : " + message.MessageType.ToString() + ": " + message.MessageText);
        }

        /// <summary>
        /// Add a message in the queue
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messagetype">The messagetype.</param>
        public void AddMessage(string message, Message.MessageTypes messagetype)
        {
            if (!string.IsNullOrEmpty(message))
                AddMessage(new Message(message, messagetype));
        }

        public MessagesListViewModel GetMessagesList()
        {
            var model = new MessagesListViewModel();

            List<Message> messages = (List<Message>)TempData["Message"];
            if (messages != null)
            {
                List<MessageModel> messageModels = messages.Select(m => new MessageModel
                {
                    Title = EnumsHelper.GetResourceDisplayEnum(m.MessageType),
                    Body = m.MessageText,
                    CssClass = EnumsHelper.GetEnumCssClass(m.MessageType)
                }).ToList();

                model.Messages = messageModels;
            }

            return model;
        }
    }
}