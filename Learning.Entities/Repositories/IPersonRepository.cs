using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxteneo.Core.Domains.Contracts;

namespace Learning.Entities.Repositories
{
    public interface IProfileRepository
    {
        IEnumerable<Profile> GetAllPerson();
        IEnumerable<Profile> GetPersonByName(string name);
        Profile GetPersonById(int profileId);
    }
}
