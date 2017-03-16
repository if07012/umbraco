using System.Linq;
using System.Web.Mvc;
using Learning.Entities;
using Learning.Entities.Repositories;
using Learning.Entities.Services;
using Voxteneo.Core.Attributes;
using Voxteneo.Core.Domains;
using Voxteneo.WebComponents.Logger;

namespace Learning.Api
{
    [Inject]
    public class ProfileController : BaseController
    {
        private readonly IProfileService _profileService;

        public ProfileController(ILogger logger, IProfileService profileService) : base()
        {
            _profileService = profileService;
        }
        [HttpPost]
        public ActionResult GetDataProfile()
        {
            var list = _profileService.GetAllProfile().Select(n => new
            {
                DisplayName = n.FullName,
                Value = n.Id
            });
            return Json(new
            {
                Result = "OK",
                Options = list,
                JsonRequestBehavior.AllowGet
            });
        }
        [HttpGet]
        public ActionResult Index()
        {
            return Json(_profileService.GetAllProfile(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetByName(string name)
        {
            return Json(_profileService.GetAllProfileByName(name), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMessage(BasePagedInput input, int profileId)
        {
            return Json(_profileService.GetPagedListMessageByProfile(_profileService.GetProfileById(profileId), input), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveMessage(Message model, int profileId)
        {
            return Json(_profileService.SaveMessage(model, _profileService.GetProfileById(profileId)), JsonRequestBehavior.AllowGet);
        }
    }
}