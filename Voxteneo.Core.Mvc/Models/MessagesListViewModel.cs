using System.Collections.Generic;

namespace Voxteneo.Core.Mvc.Models
{
    public class MessagesListViewModel
    {

        public MessagesListViewModel()
        {
            Messages = new List<MessageModel>();
        }

        public List<MessageModel> Messages { get; set; }
    }
}
