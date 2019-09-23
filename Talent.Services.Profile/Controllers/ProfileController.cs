using Talent.Common.Contracts;
using Talent.Common.Models;
using Talent.Common.Security;
using Talent.Services.Profile.Models.Profile;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using RawRabbit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Driver;
using Talent.Services.Profile.Domain.Contracts;
using Talent.Common.Aws;
using Talent.Services.Profile.Models;

namespace Talent.Services.Profile.Controllers
{
    [Route("profile/[controller]")]
    public class ProfileController : Controller
    {
        private readonly IBusClient _busClient;
        private readonly IAuthenticationService _authenticationService;
        private readonly IProfileService _profileService;
        private readonly IFileService _documentService;
        private readonly IUserAppContext _userAppContext;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserLanguage> _userLanguageRepository;
        private readonly IRepository<UserDescription> _personDescriptionRespository;
        private readonly IRepository<UserAvailability> _userAvailabilityRepository;
        private readonly IRepository<UserSkill> _userSkillRepository;
        private readonly IRepository<UserEducation> _userEducationRepository;
        private readonly IRepository<UserCertification> _userCertificationRepository;
        private readonly IRepository<UserLocation> _userLocationRepository;
        private readonly IRepository<Employer> _employerRepository;
        private readonly IRepository<UserDocument> _userDocumentRepository;
        private readonly IHostingEnvironment _environment;
        private readonly IRepository<Recruiter> _recruiterRepository;
        private readonly IAwsService _awsService;
        private readonly string _profileImageFolder;
        private readonly IFileService _fileService;


        public ProfileController(IBusClient busClient,
            IProfileService profileService,
            IFileService documentService,
            IRepository<User> userRepository,
            IRepository<UserLanguage> userLanguageRepository,
            IRepository<UserDescription> personDescriptionRepository,
            IRepository<UserAvailability> userAvailabilityRepository,
            IRepository<UserSkill> userSkillRepository,
            IRepository<UserEducation> userEducationRepository,
            IRepository<UserCertification> userCertificationRepository,
            IRepository<UserLocation> userLocationRepository,
            IRepository<Employer> employerRepository,
            IRepository<UserDocument> userDocumentRepository,
            IRepository<Recruiter> recruiterRepository,
            IHostingEnvironment environment,
            IAwsService awsService,
            IUserAppContext userAppContext,
            IFileService fileService)
        {
            _busClient = busClient;
            _profileService = profileService;
            _documentService = documentService;
            _userAppContext = userAppContext;
            _userRepository = userRepository;
            _personDescriptionRespository = personDescriptionRepository;
            _userLanguageRepository = userLanguageRepository;
            _userAvailabilityRepository = userAvailabilityRepository;
            _userSkillRepository = userSkillRepository;
            _userEducationRepository = userEducationRepository;
            _userCertificationRepository = userCertificationRepository;
            _userLocationRepository = userLocationRepository;
            _employerRepository = employerRepository;
            _userDocumentRepository = userDocumentRepository;
            _recruiterRepository = recruiterRepository;
            _environment = environment;
            _profileImageFolder = "images\\";
            _awsService = awsService;
            _fileService = fileService;
        }

        #region Talent

        [HttpGet("getProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = _userAppContext.CurrentUserId;
            var user = await _userRepository.GetByIdAsync(userId);
            return Json(new { Username = user.FirstName });
        }

        [HttpGet("getProfileById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> GetProfileById(string uid)
        {
            var userId = uid;
            var user = await _userRepository.GetByIdAsync(userId);
            return Json(new { userName = user.FirstName, createdOn = user.CreatedOn });
        }

        [HttpGet("isUserAuthenticated")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> IsUserAuthenticated()
        {
            if (_userAppContext.CurrentUserId == null)
            {
                return Json(new { IsAuthenticated = false });
            }
            else
            {
                var person = await _userRepository.GetByIdAsync(_userAppContext.CurrentUserId);
                if (person != null)
                {
                    return Json(new { IsAuthenticated = true, Username = person.FirstName, Type = "talent" });
                }
                var employer = await _employerRepository.GetByIdAsync(_userAppContext.CurrentUserId);
                if (employer != null)
                {
                    return Json(new { IsAuthenticated = true, Username = employer.CompanyContact.Name, Type = "employer" });
                }
                var recruiter = await _recruiterRepository.GetByIdAsync(_userAppContext.CurrentUserId);
                if (recruiter != null)
                {
                    return Json(new { IsAuthenticated = true, Username = recruiter.CompanyContact.Name, Type = "recruiter" });
                }
                return Json(new { IsAuthenticated = false, Type = "" });
            }
        }

        [HttpGet("getLanguage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> GetLanguages()
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("addLanguage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public ActionResult AddLanguage([FromBody] AddLanguageViewModel language)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("updateLanguage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<ActionResult> UpdateLanguage([FromBody] AddLanguageViewModel language)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("deleteLanguage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<ActionResult> DeleteLanguage([FromBody] AddLanguageViewModel delete_language_model, String id = "")
        {
            //Your code here;
            String talentId = String.IsNullOrWhiteSpace(id) ? _userAppContext.CurrentUserId : id;
            if (talentId != null)
            {
                User existingTalent = (await _userRepository.GetByIdAsync(talentId));
                var newLanguages = new List<UserLanguage>();
                foreach (var item in existingTalent.Languages)
                {
                    var delete_language = existingTalent.Languages.SingleOrDefault(x => x.Id == delete_language_model.Id);
                    if (item.Id != delete_language_model.Id)
                    {
                        newLanguages.Add(item);
                    }
                }
                existingTalent.Languages = newLanguages;
                existingTalent.UpdatedBy = talentId;
                existingTalent.UpdatedOn = DateTime.Now;
                await _userRepository.Update(existingTalent);
                User UpdateTalent = (await _userRepository.GetByIdAsync(talentId));
                return Json(new { success = true, data = existingTalent });
            }
            return Json(new { success = false });
        }
        [HttpPost("deleteExperience")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<ActionResult> DeleteExperience([FromBody] ExperienceViewModel delete_experince_model, String id = "")
        {
            //Your code here;
            String talentId = String.IsNullOrWhiteSpace(id) ? _userAppContext.CurrentUserId : id;
            if (talentId != null)
            {
                User existingTalent = (await _userRepository.GetByIdAsync(talentId));
                var newExperiences = new List<UserExperience>();
                foreach (var item in existingTalent.Experience)
                {
                    var delete_language = existingTalent.Languages.SingleOrDefault(x => x.Id == delete_experince_model.Id);
                    if (item.Id != delete_experince_model.Id)
                    {
                        newExperiences.Add(item);
                    }
                }
                existingTalent.Experience = newExperiences;
                existingTalent.UpdatedBy = talentId;
                existingTalent.UpdatedOn = DateTime.Now;
                await _userRepository.Update(existingTalent);
                User UpdateTalent = (await _userRepository.GetByIdAsync(talentId));
                return Json(new { success = true, data = existingTalent });
            }
            return Json(new { success = false });
        }

        [HttpGet("getSkill")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> GetSkills()
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("addSkill")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public ActionResult AddSkill([FromBody]AddSkillViewModel skill)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("updateSkill")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> UpdateSkill([FromBody]AddSkillViewModel skill)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("deleteSkill")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> DeleteSkill( [FromBody]AddSkillViewModel delete_skill_model, String id = "")
        {
            
            String talentId = String.IsNullOrWhiteSpace(id) ? _userAppContext.CurrentUserId : id;
            //Console.WriteLine(talentId);
            
            //Your code here;
            if (talentId != null)
            {
                User existingTalent = (await _userRepository.GetByIdAsync(talentId));

                var newRSkills = new List<UserSkill>();

                foreach (var item in existingTalent.Skills)
                {
                    var delete_skill = existingTalent.Skills.SingleOrDefault(x => x.Id == delete_skill_model.Id);
                    // var skill = existingTalent.Skills.SingleOrDefault(x => x.Id == item.Id);
                    if (item.Id != delete_skill_model.Id)
                    {
                        newRSkills.Add(item);
                    }
                }
                existingTalent.Skills = newRSkills;
                existingTalent.UpdatedBy = talentId;
                existingTalent.UpdatedOn = DateTime.Now;
                await _userRepository.Update(existingTalent);
                User UpdateTalent = (await _userRepository.GetByIdAsync(talentId));
                return Json(new { success = true, data = existingTalent });
            }


            return Json(new { success = false});
        }

        [HttpGet("getCertification")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> getCertification()
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("addCertification")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public ActionResult addCertification([FromBody] AddCertificationViewModel certificate)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("updateCertification")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> UpdateCertification([FromBody] AddCertificationViewModel certificate)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("deleteCertification")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> DeleteCertification([FromBody] AddCertificationViewModel certificate)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpGet("getProfileImage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult getProfileImage(string Id)
        {
            var profileUrl = _documentService.GetFileURL(Id, FileType.ProfilePhoto);
            //Please do logic for no image available - maybe placeholder would be fine
            return Json(new { profilePath = profileUrl });
        }

        //[HttpPost("updateProfilePhoto")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        //public async Task<ActionResult> UpdateProfilePhoto()
        //{
        //    //IFormFile file = Request.Form.Files[0];
        //    //Your code here;
        //    return Json(new { success = true });
        //}

        [HttpPost("updateTalentCV")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<ActionResult> UpdateTalentCV()
        {
            IFormFile file = Request.Form.Files[0];
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("updateTalentVideo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> UpdateTalentVideo()
        {
            IFormFile file = Request.Form.Files[0];
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpGet("getInfo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> GetInfo()
        {
            //Your code here;
            throw new NotImplementedException();
        }


        [HttpPost("addInfo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> AddInfo([FromBody] DescriptionViewModel pValue)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpGet("getEducation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> GetEducation()
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("addEducation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public IActionResult AddEducation([FromBody]AddEducationViewModel model)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("updateEducation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> UpdateEducation([FromBody]AddEducationViewModel model)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("deleteEducation")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> DeleteEducation([FromBody] AddEducationViewModel model)
        {
            //Your code here;
            throw new NotImplementedException();
        }

     
        #endregion

        #region EmployerOrRecruiter

        [HttpGet("getEmployerProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "employer, recruiter")]
        public async Task<IActionResult> GetEmployerProfile(String id = "", String role = "")
        {
            try
            {
                string userId = String.IsNullOrWhiteSpace(id) ? _userAppContext.CurrentUserId : id;
                string userRole = String.IsNullOrWhiteSpace(role) ? _userAppContext.CurrentRole : role;

                var employerResult = await _profileService.GetEmployerProfile(userId, userRole);

                return Json(new { Success = true, employer = employerResult });
            }
            catch (Exception e)
            {
                return Json(new { Success = false, message = e });
            }
        }

        [HttpPost("saveEmployerProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "employer, recruiter")]
        public async Task<IActionResult> SaveEmployerProfile([FromBody] EmployerProfileViewModel employer)
        {
            if (ModelState.IsValid)
            {
                if (await _profileService.UpdateEmployerProfile(employer, _userAppContext.CurrentUserId, _userAppContext.CurrentRole))
                {
                    return Json(new { Success = true });
                }
            }
            return Json(new { Success = false });
        }

        [HttpPost("saveClientProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "recruiter")]
        public async Task<IActionResult> SaveClientProfile([FromBody] EmployerProfileViewModel employer)
        {
            if (ModelState.IsValid)
            {
                //check if employer is client 5be40d789b9e1231cc0dc51b
                var recruiterClients =(await _recruiterRepository.GetByIdAsync(_userAppContext.CurrentUserId)).Clients;

                if (recruiterClients.Select(x => x.EmployerId == employer.Id).FirstOrDefault())
                {
                    if (await _profileService.UpdateEmployerProfile(employer, _userAppContext.CurrentUserId, "employer"))
                    {
                        return Json(new { Success = true });
                    }
                }
            }
            return Json(new { Success = false });
        }

        [HttpPost("updateEmployerPhoto")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "employer, recruiter")]
        public async Task<ActionResult> UpdateEmployerPhoto()
        {
            IFormFile file = Request.Form.Files[0];
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpPost("updateEmployerVideo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "employer, recruiter")]
        public async Task<IActionResult> UpdateEmployerVideo()
        {
            IFormFile file = Request.Form.Files[0];
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpGet("getEmployerProfileImage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "employer, recruiter")]
        public async Task<ActionResult> GetWorkSample(string Id)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        [HttpGet("getEmployerProfileImages")]
        public ActionResult GetWorkSampleImage(string Id)
        {
            //Your code here;
            throw new NotImplementedException();
        }
        
        #endregion

        #region TalentFeed

        [HttpGet("getTalentProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent, employer, recruiter")]
        public async Task<IActionResult> GetTalentProfile(String id = "")
        {
            var Success = true;
            String talentId = String.IsNullOrWhiteSpace(id) ? _userAppContext.CurrentUserId : id;
            //Console.WriteLine(talentId);
            var userProfile = await _profileService.GetTalentProfile(talentId);

            return Json(new { Success = true, data = userProfile });
            //return Json(new { Success, data = talentId });
        }

        [HttpGet("getEmpTalentProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent, employer, recruiter")]
        public async Task<IActionResult> getEmpTalentProfile(String id = "")
        {
            var Success = true;
            String talentId = "5c7c6e6d3de7ed2604e17777";
            //Console.WriteLine(talentId);
            var userProfile = await _profileService.GetTalentProfile(talentId);

            return Json(new { Success, data = userProfile });
            //return Json(new { Success, data = talentId });
        }

        [HttpPost("updateTalentProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> UpdateTalentProfile([FromBody]TalentProfileViewModel profile)
        {
            if (ModelState.IsValid)
            {
                if (await _profileService.UpdateTalentProfile(profile, _userAppContext.CurrentUserId))
                {
                    return Json(new { Success = true, data = profile });
                }
            }
            return Json(new { Success = false, data = profile });
        }

        [HttpPost("FileSave")]
        public async Task<IActionResult> FileSave()

        { 
            var files = Request.Form.Files;
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            // var filePath = Path.GetTempFileName();
            var filePath = "C:/Users/Administrator/Desktop/StringTemplateEngine";

            using (var stream = new FileStream(filePath, FileMode.Create))

            {



                await files[0].CopyToAsync(stream);

            }
            return Json(new { Success = true,data= files });

        }

        [HttpPost("updateProfilePhoto")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "talent")]
        public async Task<IActionResult> updateProfilePhoto(String id = "")
        {
            //var date = Request;
            var file = Request.Form.Files[0];
            if (file != null) {
                long size = file.Length;
                string webRootPath = _environment.WebRootPath;
                string contentRootPath = _environment.ContentRootPath;
                string fileExt = Path.GetExtension(file.FileName);
                string newFileName = System.Guid.NewGuid().ToString()+ fileExt;
                var filePath = webRootPath + "/images/" + newFileName;
                string wpath = "C:/Users/Administrator/source/repos/talent-standard-tasks/App/Talent.App.WebApp/wwwroot" + "/images/" + newFileName;
          
                String talentId = _userAppContext.CurrentUserId;
                var user = await _userRepository.GetByIdAsync(talentId);
                var stream = new FileStream(wpath, FileMode.Create);
                await file.CopyToAsync(stream);

                return Json(new { success = true, photoUrl = wpath, photoName=newFileName });
            }

            return Json(new { success = false  });

        }

        //using (var stream = new FileStream(filePath, FileMode.Create)) {

        //    await file.CopyToAsync(stream);
        //}

        //var file = Request.Form.Files[0];
        //String talentId = _userAppContext.CurrentUserId;
        //var fileExtension = Path.GetExtension(file.FileName);
        ////var fileExtension = Path.GetExtension(file.FileName);
        //var profile = (await _userRepository.Get(x => x.Id == talentId)).SingleOrDefault();
        //var user = await _userRepository.GetByIdAsync(talentId);
        //var newFileName = await _fileService.SaveFile(file, FileType.ProfilePhoto);


        //[HttpPost("UpdateProfilePhoto")]
        //public async Task<ActionResult> UpdateProfilePhoto(IFormFile data)
        ////public async Task<ActionResult> UpdateProfilePhoto(String data)
        //{
        //    // var result = await _profileService.GetFullTalentList();
        //    //var allFiles = Request.Form.Files;
        //    //var root = _environment.WebRootPath;
        //    //var extension = Path.GetExtension(files.FileName);
        //    //IFormFile file = Request.Form.Files[0];
        //    //Your code here;
        //    //if (files != null) {
        //    //    var fileName = Path.Combine(_environment.WebRootPath,Path.GetFileName(files.FileName));
        //    //    files.CopyTo(new FileStream(fileName, FileMode.Create));
        //    //}

        //    //var allFiles = Request.Form.Files;
        //    //var root = _environment.WebRootPath;
        //    //var extension = Path.GetExtension(data.file.FileName);
        //    //var guid = Guid.NewGuid().ToString();
        //    //var fullPath = $@"{root}\images\{guid + extension}";
        //    //using (FileStream stream = new FileStream(fullPath, FileMode.Create))
        //    //{
        //    //    await data.file.CopyToAsync(stream);
        //    //}

        //    return Json(new { success = false });
        //}
        //public async Task<IActionResult> UpdateTalentProfile([FromBody]TalentProfileViewModel model)
        //{
        //    //if (ModelState.IsValid)
        //    //{
        //    //    if (await _profileService.UpdateTalentProfile(profile, _userAppContext.CurrentUserId))
        //    //    {
        //    //        return Json(new { Success = true, data = profile });
        //    //    }
        //    //}
        //    User existingTalent = (await _userRepository.GetByIdAsync(model.Id));
        //    var newExperiences = new List<UserExperience>();
        //    foreach (var item in model.Experience)
        //    {
        //        var experience = existingTalent.Experience.SingleOrDefault(x => x.Id == item.Id);
        //        if (experience == null)
        //        {
        //            experience = new UserExperience
        //            {
        //                Id = ObjectId.GenerateNewId().ToString(),
        //            };
        //        }
        //        //return Json(new { Success = true, data = item });
        //        UpdateExperienceFromView(item, experience);
        //        newExperiences.Add(experience);

        //    }
        //    existingTalent.Experience = newExperiences;

        //    existingTalent.UpdatedBy = model.Id;
        //    existingTalent.UpdatedOn = DateTime.Now;

        //    await _userRepository.Update(existingTalent);
        //    return Json(new { Success = true, data = model.Experience });
        //}
        protected void UpdateExperienceFromView(ExperienceViewModel model, UserExperience original)
        {
            original.Company = model.Company;
            original.Position = model.Position;
            original.Start = model.Start;
            original.End = model.End;
            original.Responsibilities = model.Responsibilities;

        }

        protected void UpdateLanguageFromView(AddLanguageViewModel model, UserLanguage original)
        {
            original.Language = model.Name;
            original.LanguageLevel = model.Level;
        }
        [HttpGet("getTalent")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "recruiter, employer")]
        public async Task<IActionResult> GetTalentSnapshots(FeedIncrementModel feed)
        {
            try
            {
                var result = (await _profileService.GetTalentSnapshotList(_userAppContext.CurrentUserId, false, feed.Position, feed.Number)).ToList();

                // Dummy talent to fill out the list once we run out of data
                //if (result.Count == 0)
                //{
                //    result.Add(
                //            new Models.TalentSnapshotViewModel
                //            {
                //                CurrentEmployment = "Software Developer at XYZ",
                //                Level = "Junior",
                //                Name = "Dummy User...",
                //                PhotoId = "",
                //                Skills = new List<string> { "C#", ".Net Core", "Javascript", "ReactJS", "PreactJS" },
                //                Summary = "Veronika Ossi is a set designer living in New York who enjoys kittens, music, and partying.",
                //                Visa = "Citizen"
                //            }
                //        );
                //}
                return Json(new { Success = true, Data = result });
            }
            catch (Exception e)
            {
                return Json(new { Success = false, e.Message });
            }
        }
        #endregion

        #region TalentMatching

        [HttpGet("getTalentList")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "recruiter")]
        public async Task<IActionResult> GetTalentListAsync()
        {
            try
            {
                var result = await _profileService.GetFullTalentList();
                return Json(new { Success = true, Data = result });
            }
            catch (MongoException e)
            {
                return Json(new { Success = false, e.Message });
            }
        }

        [HttpGet("getEmployerList")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "recruiter")]
        public IActionResult GetEmployerList()
        {
            try
            {
                var result = _profileService.GetEmployerList();
                return Json(new { Success = true, Data = result });
            }
            catch (MongoException e)
            {
                return Json(new { Success = false, e.Message });
            }
        }

        [HttpPost("getEmployerListFilter")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "recruiter")]
        public IActionResult GetEmployerListFilter([FromBody]SearchCompanyModel model)
        {
            try
            {
                var result = _profileService.GetEmployerListByFilterAsync(model);//change to filters
                if (result.IsCompletedSuccessfully)
                    return Json(new { Success = true, Data = result.Result });
                else
                    return Json(new { Success = false, Message = "No Results found" });
            }
            catch (MongoException e)
            {
                return Json(new { Success = false, e.Message });
            }
        }

        [HttpPost("getTalentListFilter")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetTalentListFilter([FromBody] SearchTalentModel model)
        {
            try
            {
                var result = _profileService.GetTalentListByFilterAsync(model);//change to filters
                return Json(new { Success = true, Data = result.Result });
            }
            catch (MongoException e)
            {
                return Json(new { Success = false, e.Message });
            }
        }

        [HttpGet("getSuggestionList")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "recruiter")]
        public IActionResult GetSuggestionList(string employerOrJobId, bool forJob)
        {
            try
            {
                var result = _profileService.GetSuggestionList(employerOrJobId, forJob, _userAppContext.CurrentUserId);
                return Json(new { Success = true, Data = result });
            }
            catch (MongoException e)
            {
                return Json(new { Success = false, e.Message });
            }
        }

        [HttpPost("addTalentSuggestions")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "recruiter")]
        public async Task<IActionResult> AddTalentSuggestions([FromBody] AddTalentSuggestionList talentSuggestions)
        {
            try
            {
                if (await _profileService.AddTalentSuggestions(talentSuggestions))
                {
                    return Json(new { Success = true });
                }

            }
            catch (Exception e)
            {
                return Json(new { Success = false, e.Message });
            }
            return Json(new { Success = false });
        }

        #endregion


        #region ManageClients

        [HttpGet("getClientList")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "recruiter")]
        public async Task<IActionResult> GetClientList()
        {
            try
            {
                var result=await _profileService.GetClientListAsync(_userAppContext.CurrentUserId);

                return Json(new { Success = true, result });
            }
            catch(Exception e)
            {
                return Json(new { Success = false, e.Message });
            }
        }

        //[HttpGet("getClientDetailsToSendMail")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "recruiter")]
        //public async Task<IActionResult> GetClientDetailsToSendMail(string clientId)
        //{
        //    try
        //    {
        //            var client = await _profileService.GetEmployer(clientId);

        //            string emailId = client.Login.Username;
        //            string companyName = client.CompanyContact.Name;

        //            return Json(new { Success = true, emailId, companyName });
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(new { Success = false, Message = e.Message });
        //    }
        //}

        #endregion

        public IActionResult Get() => Content("Test");

    }
}
