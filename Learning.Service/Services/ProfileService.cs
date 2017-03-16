using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Learning.Entities;
using Learning.Entities.Repositories;
using Learning.Entities.Services;
using Voxteneo.Core.Domains;
using Voxteneo.Core.Domains.Contracts;

namespace Learning.Service.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProfileService(IProfileRepository profileRepository, IMessageRepository messageRepository, IUnitOfWork unitOfWork)
        {
            _profileRepository = profileRepository;
            _messageRepository = messageRepository;
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<Profile> GetAllProfile()
        {
            return _profileRepository.GetAllPerson();
        }

        public IEnumerable<Profile> GetAllProfileByName(string name)
        {
            return _profileRepository.GetPersonByName(name);
        }

        public Message SaveMessage(Message message, Profile profile)
        {
            message.PersonId = profile.Id;
            _messageRepository.SaveMessage(message);
            _unitOfWork.SaveChanges();
            message.Person = GetProfileById(profile.Id);
            return message;
        }

        public BasePagedList<Message> GetPagedListMessageByProfile(Profile profile, BasePagedInput input)
        {
            var result = _messageRepository.GetPagedListByProfile(profile, input);
            result.Records.ForEach(n =>
            {
                n.Person = _profileRepository.GetPersonById(n.PersonId);
            });
            return result;
        }

        public Profile GetProfileById(int profileId)
        {
            return _profileRepository.GetPersonById(profileId);
        }
    }
}
