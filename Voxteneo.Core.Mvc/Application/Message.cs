using System.ComponentModel;

namespace Voxteneo.Core.Mvc.Application
{
    public class Message
    {
        public string MessageText { get; set; }
        public MessageTypes MessageType { get; set; }

        public Message(string messagetext, MessageTypes messagetype)
        {
            MessageText = messagetext;
            MessageType = messagetype;
        }

        public enum MessageTypes
        {
            [Description("Success")]
            Success,

            [Description("Error")]
            Error,

            [Description("Warning")]
            Warning,

            [Description("Info")]
            Info
        }
    }
}