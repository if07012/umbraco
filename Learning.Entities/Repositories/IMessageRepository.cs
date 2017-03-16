using Voxteneo.Core.Domains;

namespace Learning.Entities.Repositories
{
    public interface IMessageRepository
    {
        Message SaveMessage(Message message);
        BasePagedList<Message> GetPagedListByProfile(Profile profile, BasePagedInput input);
    }
}