using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learning.Entities;
using Learning.Entities.Repositories;
using Voxteneo.Core.Domains.Contracts;
using Voxteneo.Core.Domains.UmbracoExtentions;

namespace Learning.Service.EntityFramework
{
    public class ProfileRepository : BaseRepository, IProfileRepository
    {

        public ProfileRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        public IEnumerable<Profile> GetAllPerson()
        {
            return UnitOfWork.GetContext().Database.QueryData<Profile>().ToList();
        }

        public IEnumerable<Profile> GetPersonByName(string name)
        {
            return UnitOfWork.GetContext().Database.QueryData<Profile>().Where(n => n.FullName.Contains(name)).ToList();
        }

        private readonly Dictionary<int, Profile> _cacheProfiles = new Dictionary<int, Profile>();
        public Profile GetPersonById(int profileId)
        {
            if (_cacheProfiles.ContainsKey(profileId))
                return _cacheProfiles[profileId];
            _cacheProfiles.Add(profileId, UnitOfWork.GetContext().Database.QueryData<Profile>().FirstOrDefault(n => n.Id == profileId));
            return _cacheProfiles[profileId];
        }
    }
}
