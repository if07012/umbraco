using System.Linq;
using Learning.Entities;
using Learning.Entities.Repositories;
using Voxteneo.Core.Domains;
using Voxteneo.Core.Domains.Contracts;
using Voxteneo.Core.Domains.UmbracoExtentions;

namespace Learning.Service.EntityFramework
{
    public class MessageRepository : BaseRepository, IMessageRepository
    {
        public MessageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

     

        public BasePagedList<Message> GetPagedListByProfile(Profile profile, BasePagedInput input)
        {
            var result = Context.Messages.Where(n => n.PersonId == profile.Id).OrderByDescending(n=>n.CreateDate).PagedQueryable(input);
            return result;
        }

        public Message SaveMessage(Message message)
        {
            
            UnitOfWork.GetGenericRepository<Message>().Insert(message);
            
            return message;
        }
    }
}