using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxteneo.Core.Domains;

namespace Learning.Entities.Services
{
    public interface IProfileService
    {
        IEnumerable<Profile> GetAllProfile();
        IEnumerable<Profile> GetAllProfileByName(string name);
        Message SaveMessage(Message message, Profile profile);
        BasePagedList<Message> GetPagedListMessageByProfile(Profile profile, BasePagedInput input);
        Profile GetProfileById(int profileId);
    }
}
