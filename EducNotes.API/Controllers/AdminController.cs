using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EducNotes.API.data;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Helpers;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EducNotes.API.Controllers {
  [ApiController]
  [Route ("api/[controller]")]
  public class AdminController : ControllerBase {
    private readonly DataContext _context;
    private readonly IEducNotesRepository _repo;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private Cloudinary _cloudinary;
    string password;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    int parentRoleId, memberRoleId, moderatorRoleId, adminRoleId, teacherRoleId, schoolInscTypeId;
    int renewRegEmailId, tuitionId, nextYearTuitionId;
    byte lastClassLevelSeq;
    CultureInfo frC = new CultureInfo ("fr-FR");
    public ICacheRepository _cache { get; }

    public AdminController (DataContext context, UserManager<User> userManager, IConfiguration config,
      IEducNotesRepository repo, IOptions<CloudinarySettings> cloudinaryConfig, IMapper mapper,
      ICacheRepository cache) {
      _cache = cache;
      _userManager = userManager;
      _config = config;
      _context = context;
      _repo = repo;
      _mapper = mapper;
      _cloudinaryConfig = cloudinaryConfig;
      teacherTypeId = _config.GetValue<int> ("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int> ("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int> ("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int> ("AppSettings:studentTypeId");
      password = _config.GetValue<string> ("AppSettings:defaultPassword");
      parentRoleId = _config.GetValue<int> ("AppSettings:parentRoleId");
      memberRoleId = _config.GetValue<int> ("AppSettings:memberRoleId");
      moderatorRoleId = _config.GetValue<int> ("AppSettings:moderatorRoleId");
      adminRoleId = _config.GetValue<int> ("AppSettings:adminRoleId");
      teacherRoleId = _config.GetValue<int> ("AppSettings:teacherRoleId");
      schoolInscTypeId = _config.GetValue<int> ("AppSettings:schoolInscTypeId");
      renewRegEmailId = _config.GetValue<int> ("AppSettings:renewRegEmailId");
      tuitionId = _config.GetValue<int> ("AppSettings:tuitionId");
      nextYearTuitionId = _config.GetValue<int> ("AppSettings:nextYearTuitionId");
      lastClassLevelSeq = _config.GetValue<byte> ("AppSettings:lastClassLevelSeq");

      Account acc = new Account (
        _cloudinaryConfig.Value.CloudName,
        _cloudinaryConfig.Value.ApiKey,
        _cloudinaryConfig.Value.ApiSecret
      );

      _cloudinary = new Cloudinary (acc);
    }

    [Authorize (Policy = "RequireAdminRole")]
    [HttpGet ("usersWithRoles")]
    public async Task<IActionResult> GetUsersWithRoles () {
      var userList = await (from user in _context.Users orderby user.UserName select new {
        Id = user.Id,
          UserName = user.UserName,
          Roles = (from userRole in user.UserRoles join role in _context.Roles on userRole.RoleId equals role.Id select role.Name).ToList ()
      }).ToListAsync ();

      return Ok (userList);
    }

    [Authorize (Policy = "RequireAdminRole")]
    [HttpPost ("editRoles/{userName}")]
    public async Task<IActionResult> EditRoles (string userName, RoleEditDto roleEditDto) {
      var user = await _userManager.FindByNameAsync (userName);

      var userRoles = await _userManager.GetRolesAsync (user);

      var selectedRoles = roleEditDto.RoleNames;

      //selected = selectedRoles != null? selectedRoles : new string[] {};
      selectedRoles = selectedRoles ?? new string[] { };

      var result = await _userManager.AddToRolesAsync (user, selectedRoles.Except (userRoles));

      if (!result.Succeeded)
        return BadRequest ("Failed to add to roles");

      result = await _userManager.RemoveFromRolesAsync (user, userRoles.Except (selectedRoles));

      if (!result.Succeeded)
        return BadRequest ("Failed to remove roles");

      return Ok (await _userManager.GetRolesAsync (user));
    }

    [Authorize (Policy = "ModeratePhotoRole")]
    [HttpGet ("photosForModeration")]
    public async Task<IActionResult> GetPhotosForModeration () {
      var photos = await _context.Photos
        .Include (u => u.User)
        .IgnoreQueryFilters ()
        .Where (p => p.IsApproved == false)
        .Select (u => new {
          Id = u.Id,
            userName = u.User.UserName,
            Url = u.Url,
            IsApproved = u.IsApproved
        }).ToListAsync ();

      return Ok (photos);
    }

    [Authorize (Policy = "ModeratePhotoRole")]
    [HttpPost ("approvePhoto/{photoId}")]
    public async Task<IActionResult> ApprovePhoto (int photoId) {
      var photo = await _context.Photos
        .IgnoreQueryFilters ()
        .FirstOrDefaultAsync (p => p.Id == photoId);

      photo.IsApproved = true;

      await _context.SaveChangesAsync ();

      return Ok ();
    }

    [Authorize (Policy = "ModeratePhotoRole")]
    [HttpPost ("rejectPhoto/{photoId}")]
    public async Task<IActionResult> RejectPhoto (int photoId) {
      var photo = await _context.Photos
        .IgnoreQueryFilters ()
        .FirstOrDefaultAsync (p => p.Id == photoId);

      if (photo.IsMain)
        return BadRequest ("You cannot reject the main photo");

      if (photo.PublicId != null) {
        var deleteParams = new DeletionParams (photo.PublicId);

        var result = _cloudinary.Destroy (deleteParams);

        if (result.Result == "ok") {
          _context.Photos.Remove (photo);
        }
      }

      if (photo.PublicId == null) {
        _context.Photos.Remove (photo);
      }

      await _context.SaveChangesAsync ();

      return Ok ();
    }

    [HttpGet ("GetUserTypes")]
    public async Task<IActionResult> GetUserTypes () {
      return Ok (await _context.UserTypes.OrderBy (e => e.Name).ToListAsync ());
    }

    [HttpGet ("GetAllTeachers")]
    public async Task<IActionResult> GetAllTeachers () {
      //recuperation de tous les professeurs
      var teachers = await _context.Users.Where (u => u.UserTypeId == teacherTypeId).ToListAsync ();

      return Ok (teachers);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers ([FromQuery] UserParams userParams) {
      var currentUserId = int.Parse (User.FindFirst (ClaimTypes.NameIdentifier).Value);

      var userFromRepo = await _repo.GetUser (currentUserId, true);

      userParams.userId = currentUserId;

      var users = await _repo.GetUsers (userParams);

      var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>> (users);

      Response.AddPagination (users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

      return Ok (usersToReturn);
    }

    // [HttpGet("{id}", Name = "GetUser")]
    // public async Task<IActionResult> GetUser(int id)
    // {
    //     var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;

    //     var user = await _repo.GetUser(id, isCurrentUser);

    //     var userToReturn = _mapper.Map<UserForDetailedDto>(user);

    //     return Ok(userToReturn);
    // }

    [HttpGet ("CurrentPeriod")]
    public async Task<IActionResult> GetCurrentPeriod () {
      // DATES MUST BE UP TO DATE

      // SOLUTION : DETECT THE CURRENT PERIOD WITH STARTING AND ENDING DATES OF THE PERIOD
      var today = DateTime.Now;

      // OTHER SOLUTION : SET THE CURRENT DATE WITH THE ACTIVE FLAG
      Period CurrentPeriod = await _context.Periods.Where (p => p.Active == true).FirstOrDefaultAsync ();

      if (CurrentPeriod == null)
        return BadRequest ("la période courante n'a pas été paramétrée");

      return Ok (CurrentPeriod);
    }

    [HttpGet ("School")]
    public async Task<IActionResult> GetSchool () {
      var school = await _context.Establishments.SingleOrDefaultAsync ();

      if (school == null) {
        var newSchool = new Establishment {
        Name = "",
        Location = "",
        Phone = "",
        WebSite = "",
        StartCoursesHour = DateTime.Now.Date,
        EndCoursesHour = DateTime.Now.Date
        };
        _context.Add (newSchool);

        if (await _repo.SaveAll ())
          return Ok (newSchool);

        return BadRequest ("problème pour créer les infos de l'établissement");
      }

      return Ok (school);
    }

    [HttpPut ("SaveSchool")]
    public async Task<IActionResult> SaveEstablishment ([FromBody] Establishment school) {
      if (school.Id == 0)
        _repo.Add (school);
      else
        _repo.Update (school);

      if (await _repo.SaveAll ())
        return NoContent ();

      throw new Exception ($"l'ajout des infos de l'établissement a échoué");
    }

    [HttpPost ("{classId}/StudentAffectation")]
    public async Task<IActionResult> StudentAffectation (int classId, List<StudentPostingDto> studentIds) {
      if (studentIds.Count () > 0) {
        var currentUserId = int.Parse (User.FindFirst (ClaimTypes.NameIdentifier).Value);

        foreach (var student in studentIds) {
          var user = await _context.Users.FirstOrDefaultAsync (u => u.Id == student.Id);
          user.ClassId = classId;
        }

        if (await _repo.SaveAll ()) {
          //envoi du mail d'affectation à chaque parent
          foreach (var student in studentIds) {
            await _repo.sendOk (studentTypeId, student.Id);
          }
          return Ok ();
        }

        return BadRequest ("imposible d'effectuer cette opération");
      }

      return NotFound ();
    }

    [HttpGet ("LastUsersAdded")]
    public async Task<IActionResult> LastUsersAdded () {
      var usersToReturn = await _context.Users
        .Include (t => t.UserType)
        .OrderByDescending (a => a.Created)
        .Take (20)
        .ToListAsync ();
      return Ok (_mapper.Map<IEnumerable<UserForDetailedDto>> (usersToReturn));
    }

    [HttpGet ("LastUsersActivated")]
    public async Task<IActionResult> LastUsersActivated () {
      var usersToReturn = await _context.Users
        .Include (t => t.UserType)
        .OrderByDescending (a => a.ValidationDate)
        .Take (20)
        .ToListAsync ();
      return Ok (_mapper.Map<IEnumerable<UserForDetailedDto>> (usersToReturn));
    }

    [HttpGet("GetClassLevels")]
    public async Task<IActionResult> GetClassLevels()
    {
      List<Class> classesCached = await _cache.GetClasses();
      List<User> studentsCached = await _cache.GetStudents();
      List<ClassLevel> classlevels = await _cache.GetClassLevels();

      var levels = classlevels.ToList();
      var dataToReturn = new List<ClassLevelDetailDto>();
      foreach(var item in levels)
      {
        var res = new ClassLevelDetailDto();
        res.Id = item.Id;
        res.Name = item.Name;
        res.TotalEnrolled = item.Inscriptions.Count();
        res.TotalStudents = 0;
        res.Classes = new List<ClassDetailDto>();
        List<Class> classes = classesCached.Where(c => c.ClassLevelId == item.Id).ToList();
        foreach (var aclass in classes)
        {
          var students = studentsCached.Where(s => s.UserTypeId == studentTypeId && s.ClassId == aclass.Id);
          var nbStudents = students.Count();
          res.TotalStudents += nbStudents;
          //add class data
          ClassDetailDto cdd = new ClassDetailDto ();
          cdd.Id = aclass.Id;
          cdd.Name = aclass.Name;
          cdd.MaxStudent = aclass.MaxStudent;
          cdd.TotalStudents = nbStudents;
          res.Classes.Add(cdd);
        }

        dataToReturn.Add(res);
      }
      return Ok(dataToReturn);
    }

    // enregistrement de préinscription : perer , mere et enfants
    [HttpPost ("{id}/SavePreinscription")]
    public async Task<IActionResult> SavePreinscription (int id, [FromBody] PreInscriptionDto model) {
      // l'id est celui de l'utilisateur connecté
      try {
        int fatherId = 0;
        int motherId = 0;

        //ajout du père
        if (model.father.LastName != "" && model.father.FirstName != "" && model.father.PhoneNumber != "") {
          var father = _mapper.Map<User> (model.father);
          father.UserTypeId = parentTypeId;
          var guid = Guid.NewGuid ();
          //   fatherId = await _repo.AddUserPreInscription(guid,father,parentRoleId,true);

        }

        //ajout de la mère
        if (!string.IsNullOrEmpty (model.mother.LastName) && string.IsNullOrEmpty (model.mother.FirstName)) {
          var mother = _mapper.Map<User> (model.mother);
          mother.UserTypeId = parentTypeId;
          //   motherId = await _repo.AddUserPreInscription(Guid.NewGuid(),mother,parentRoleId,true);
        }

        for (int i = 0; i < model.children.Count (); i++) {
          var child = new User ();
          var levelId = Convert.ToInt32 (model.children[i].LevelId);
          child = _mapper.Map<User> (model.children[i]);
          child.UserTypeId = studentTypeId;
          int userId = 0;
          //  userId=await _repo.AddUserPreInscription(Guid.NewGuid(),child,memberRoleId,false);

          //enregistrement de l inscription
          var insc = new Inscription {
            InsertDate = DateTime.Now,
            ClassLevelId = levelId,
            InscriptionTypeId = schoolInscTypeId,
            InsertUserId = id,
            UserId = userId,
            Validated = false
          };
          _repo.Add (insc);

          //Ajout dans UserLink
          if (fatherId > 0)
            _repo.AddUserLink (userId, fatherId);
          if (motherId > 0)
            _repo.AddUserLink (userId, motherId);

        }

        await _repo.SaveAll ();

        // if(sendEMail && user.Email!=null)
        // {
        //    var callbackUrl = _config.GetValue<String>("AppSettings:DefaultEmailValidationLink")+user.ValidationCode;
        //       await _emailSender.SendEmailAsync(user.Email, "Confirmation de compte",
        //       $"veuillez confirmez votre code au lien suivant : <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicker ici</a>.");
        // }

        return Ok ("");
      } catch (System.Exception ex) {

        return BadRequest (ex);
      }
    }

    [HttpGet ("{email}/VerifyEmail")]
    public async Task<IActionResult> VerifyEmail (string email) {
      return Ok (await _repo.EmailExist (email));
    }

    // [HttpPost("{currentUserId}/SendRegisterEmail")]
    // public async Task<IActionResult> SendRegisterEmail(int currentUserId, SelfRegisterDto model)
    // {
    //   if (model.UserTypeId == teacherTypeId)
    //     {
    //         var TeacherRole = await _context.Roles.FirstOrDefaultAsync(a => a.Id == teacherRoleId);

    //         string code = Guid.NewGuid().ToString();
    //         var teacher = new User
    //         {
    //             //ValidationCode = code,
    //             Email = model.Email,
    //             UserName = code,
    //             UserTypeId = teacherTypeId
    //         };
    //         // enregistrement du professeur
    //         int userId = await _repo.AddSelfRegister(teacher, TeacherRole.Name, true, currentUserId);
    //         if (userId > 0)
    //         {
    //             foreach (var courseId in model.CourseIds)
    //             {
    //                 var ClassCourse = new TeacherCourse
    //                 {
    //                     TeacherId = userId,
    //                     CourseId = courseId
    //                 };

    //                 _repo.Add(ClassCourse);
    //             }
    //         }
    //         else
    //             return BadRequest("impossible d'ajouter cet utlisateur");

    //     }

    //   if (model.UserTypeId == parentTypeId)
    //     {
    //         var ParentRole = await _context.Roles.FirstOrDefaultAsync(a => a.Id == parentTypeId);
    //         string code = Guid.NewGuid().ToString();

    //         var parent = new User
    //         {
    //             ValidationCode = code,
    //             Email = model.Email,
    //             UserName = code,
    //             UserTypeId = parentTypeId
    //         };

    //         int parentId = await _repo.AddSelfRegister(parent, ParentRole.Name, true, currentUserId);

    //         if (parentId > 0)
    //         {
    //             for (int i = 0; i < model.TotalChild; i++)
    //             {
    //                 var MemberRole = await _context.Roles.FirstOrDefaultAsync(a => a.Id == memberRoleId);
    //                 code = Guid.NewGuid().ToString();
    //                 var child = new User
    //                 {
    //                     ValidationCode = code,
    //                     UserName = code,
    //                     UserTypeId = studentTypeId
    //                 };
    //                 int childId = await _repo.AddSelfRegister(child, MemberRole.Name, false, currentUserId);
    //                 if (childId > 0)
    //                 {
    //                     var userLink = new UserLink
    //                     {
    //                         UserPId = parentId,
    //                         UserId = childId
    //                     };
    //                     _repo.Add(userLink);
    //                 }
    //             }
    //         }
    //         else // le parent n a pas pu etre inséré
    //             return BadRequest("impossible d'ajouter ce compte");
    //     }

    //   if (await _repo.SaveAll())
    //     return NoContent();

    //   return BadRequest();
    // }

    [HttpPost ("SendEmail")]
    public async Task<IActionResult> SendEmail (EmailFormDto model) {
      if (await _repo.SendEmail (model))
        return Ok ("done");

      return BadRequest ("impossible d'envoyer le mail");
    }

    [HttpPost ("SendBatchEmail")]
    public async Task<IActionResult> SendBatchEmail (DataForEmail dataForEmail) {
      var currentUserId = int.Parse (User.FindFirst (ClaimTypes.NameIdentifier).Value);

      Email newEmail = new Email ();
      newEmail.EmailTypeId = 1;
      newEmail.FromAddress = "no-reply@educnotes.com";
      newEmail.Subject = dataForEmail.Subject;
      newEmail.Body = dataForEmail.Body;
      newEmail.ToAddress = dataForEmail.Tos;
      newEmail.CCAddress = dataForEmail.Ccs;
      newEmail.TimeToSend = DateTime.Now;
      newEmail.InsertUserId = currentUserId;
      newEmail.InsertDate = DateTime.Now;
      newEmail.UpdateUserId = currentUserId;
      newEmail.UpdateDate = DateTime.Now;

      _context.Add (newEmail);

      var apiKey = _config.GetValue<string> ("AppSettings:SENDGRID_APIKEY");
      var client = new SendGridClient (apiKey);
      var from = new EmailAddress ("no-reply@educnotes.com");
      var subject = "first email with attached file from EducNotes";
      var to = new EmailAddress ("georges.moulot@albatrostechnologies.com");
      var body = "hmmmmm... getting ready for the covid-19 battle!";
      var msg = MailHelper.CreateSingleEmail (from, to, subject, body, "");
      //var bytes = System.IO.File.ReadAllBytes("");
      // msg.AddAttachment("moulot.jpg", file);
      // var response = await client.SendEmailAsync(msg);
      //var req = System.Net.WebRequest.Create("https://res.cloudinary.com/educnotes/image/upload/v1578173397/d2zw9ozmtxgtaqrtvbss.jpg");
      // WebClient wc = new WebClient();
      // using (Stream stream = wc.OpenRead("http://res.cloudinary.com/educnotes/image/upload/v1578173397/d2zw9ozmtxgtaqrtvbss.jpg"))
      // {
      //   await msg.AddAttachmentAsync("pic.jpg", stream);
      //   var response = await client.SendEmailAsync(msg);
      // }
      var httpWebRequest = (HttpWebRequest) WebRequest.Create ("http://res.cloudinary.com/educnotes/image/upload/v1578173397/e4m74eppwjyv2eei88d6.jpg");
      var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse ();
      using (var contentStream = httpResponse.GetResponseStream ()) {
        var contentLength = 230400;
        var streamBytes = new byte[contentLength];
        var output = new StringBuilder ();
        int bytesRead = 0;
        do {
          // read one block from the input stream
          bytesRead = contentStream.Read (streamBytes, 0, streamBytes.Length);
          if (bytesRead > 0) {
            // encode the base64 string
            string base64String = Convert.ToBase64String (streamBytes, 0, bytesRead);
            output.Append (base64String);
          }
        } while (bytesRead > 0);

        // await contentStream.ReadAsync(streamBytes, 0, contentLength);
        // var base64Content = Convert.ToBase64String(streamBytes);

        msg.AddAttachment ("pic.jpg", output.ToString (), "image/jpeg");

        //await msg.AddAttachmentAsync("pic.jpg", stream);
        var response = await client.SendEmailAsync (msg);
      }

      if (await _repo.SaveAll ()) {
        return NoContent ();
      } else {
        return BadRequest ("problème pour envoyer l\' email");
      }
    }

    [HttpGet ("EmailTemplatesData")]
    public async Task<IActionResult> GetEmailTemplatesData () {
      var emailTemplates = await _context.EmailTemplates
        .Include (i => i.EmailCategory)
        .OrderBy (o => o.Name).ToListAsync ();

      List<EmailBroadcastDataDto> templates = new List<EmailBroadcastDataDto> ();
      foreach (var tpl in emailTemplates) {
        EmailBroadcastDataDto ebdd = new EmailBroadcastDataDto ();
        ebdd.Id = tpl.Id;
        ebdd.Name = tpl.Name;
        ebdd.Subject = tpl.Subject;
        ebdd.EmailCategoryName = tpl.EmailCategory.Name;
        templates.Add (ebdd);
      }
      return Ok (templates);
    }

    [HttpPost ("EmailBroadcast")]
    public async Task<IActionResult> EmailBroadcast (DataForBroadcastDto dataForEmailDto) {
      var currentUserId = int.Parse (User.FindFirst (ClaimTypes.NameIdentifier).Value);
      List<int> userTypeIds = dataForEmailDto.UserTypeIds;
      List<int> classLevelIds = new List<int> (); //dataForEmailDto.ClassLevelIds;
      List<int> classIds = dataForEmailDto.ClassIds;
      int templateId = dataForEmailDto.EmailTemplateId;
      string subject = "";
      string body = "";

      List<string> recipientEmails = new List<string> ();

      var users = new List<User> ();
      foreach (var ut in userTypeIds) {
        if (ut == studentTypeId || ut == parentTypeId) {
          if (users.Count () == 0) {
            users = await _context.Users
              .Where (u => classIds.Contains (Convert.ToInt32 (u.ClassId)) && u.AccountDataValidated == true &&
                u.UserTypeId == studentTypeId).ToListAsync ();
          }

          if (ut == studentTypeId) {
            foreach (var user in users) {
              if (string.IsNullOrEmpty (user.Email))
                recipientEmails.Add (user.Email);
            }
          }

          if (ut == parentTypeId) {
            var ids = users.Select (u => u.Id);
            var parents = _context.UserLinks.Where (u => ids.Contains (u.UserId)).Select (u => u.UserP).Distinct ();

            foreach (var user in parents) {
              if (user.AccountDataValidated && user.EmailConfirmed)
                recipientEmails.Add (user.Email);
            }
          }
        }

        if (ut == teacherTypeId) {
          List<ClassCourse> classCoursesCached = await _cache.GetClassCourses();
          var teachers = classCoursesCached
            .Where(t => classIds.Contains (t.ClassId) && t.Teacher.AccountDataValidated == true &&
              t.Teacher.UserTypeId == teacherTypeId && t.Teacher.EmailConfirmed == true)
            .Select(t => t.Teacher).Distinct().ToList();

          foreach (var user in teachers) {
            if (user.AccountDataValidated && user.EmailConfirmed)
              recipientEmails.Add (user.Email);
          }
        }
      }

      List<Setting> settings = await _context.Settings.ToListAsync ();
      var schoolName = settings.First (s => s.Name.ToLower () == "schoolname");

      //did we select a template?
      if (templateId != 0) {
        var template = await _context.EmailTemplates.FirstAsync (t => t.Id == templateId);
        subject = dataForEmailDto.Subject;

        return NoContent ();
      } else {
        body = dataForEmailDto.Body;
        subject = dataForEmailDto.Subject;

        List<Email> emailsToBeSent = new List<Email> ();
        //save emails to Emails table
        foreach (var email in recipientEmails) {
          Email newEmail = new Email ();
          newEmail.EmailTypeId = 1;
          newEmail.FromAddress = "no-reply@educnotes.com";
          newEmail.Subject = subject;
          newEmail.Body = body;
          newEmail.ToAddress = email;
          newEmail.TimeToSend = DateTime.Now;
          newEmail.InsertUserId = currentUserId;
          newEmail.InsertDate = DateTime.Now;
          newEmail.UpdateUserId = currentUserId;
          newEmail.UpdateDate = DateTime.Now;
          emailsToBeSent.Add (newEmail);
        }
        _context.AddRange (emailsToBeSent);

        if (await _repo.SaveAll ()) {
          return NoContent ();
        } else {
          return BadRequest ("problème pour envoyer les emails");
        }
      }
    }

    [HttpGet ("EmailBroadCastData")]
    public async Task<IActionResult> GetEmailBroadCastData () {
      var schools = await _repo.GetSchools ();
      var cycles = await _repo.GetCycles ();
      var educLevels = await _repo.GetEducationLevelsWithClasses ();
      return Ok (new {
        schools,
        cycles,
        educLevels
      });
    }

    [HttpPost ("EmailRegistration/{RegTypeId}")]
    public async Task<IActionResult> SendEmailRegistration (int RegTypeId) {
      List<User> parents = await _context.Users
        .Where (u => u.UserTypeId == parentTypeId && u.Active == 1 && u.EmailConfirmed == true)
        .ToListAsync ();

      List<Setting> settings = await _context.Settings.ToListAsync ();
      var schoolName = settings.First (s => s.Name.ToLower () == "schoolname").Value;
      var smsActive = settings.First (s => s.Name.ToLower () == "sendregsms").Value;
      decimal RegistrationFee = Convert.ToDecimal (settings.First (s => s.Name.ToLower () == "regfee").Value);
      string RegDeadLine = settings.First (s => s.Name == "RegistrationDeadLine").Value;

      Boolean sendSmsToo = smsActive == "1" ? true : false;

      var deadlines = await _context.ProductDeadLines
        .OrderBy (o => o.DueDate)
        .Where (p => p.ProductId == RegTypeId).ToListAsync ();

      var firstDeadline = deadlines.First (d => d.Seq == 1);
      List<RegistrationEmailDto> EmailData = new List<RegistrationEmailDto> ();
      foreach (var parent in parents) {
        //do we already have a in-process registration for parent's children?
        if (RegTypeId == tuitionId) {
          if (parent.RegCreated == true)
            continue;
        } else {
          if (parent.NextRegCreated == true)
            continue;
        }

        RegistrationEmailDto red = new RegistrationEmailDto ();
        red.ParentLastName = parent.LastName;
        red.ParentFirstName = parent.FirstName;
        red.ParentEmail = parent.Email;
        red.ParentCellPhone = parent.PhoneNumber;
        red.ParentGender = parent.Gender;
        red.EmailSubject = schoolName + " - inscription pour l'année scolaire prochaine";
        red.DueDate = firstDeadline.DueDate.ToString ("dd/MM/yyyy", frC);

        int daysToValidate = Convert.ToInt32 ((settings.First (s => s.Name == "DaysToValidateReg")).Value);

        //create the corresponding order for the children registration/inscription
        Order order = new Order ();
        order.OrderDate = DateTime.Now;
        order.Deadline = Convert.ToDateTime (settings.First (s => s.Name == "RegistrationDeadLine").Value);
        order.Validity = order.OrderDate.AddDays (daysToValidate);
        order.OrderLabel = "ré-inscription";
        if (parent.Gender == 0)
          order.MotherId = parent.Id;
        else
          order.FatherId = parent.Id;
        _repo.Add (order);
        if (!await _repo.SaveAll ())
          return BadRequest ("problème pour ajouter la scolarité");
        order.OrderNum = order.Id.GetOrderNumber ();
        if (RegTypeId == tuitionId)
          order.isReg = true;
        if (RegTypeId == nextYearTuitionId)
          order.isNextReg = true;

        red.Children = new List<ChildRegistrationDto> ();
        var children = await _context.UserLinks
          .Where (u => u.UserPId == parent.Id && u.User.Active == 1 && u.User.Validated == true)
          .Select (u => u.User)
          .Include (i => i.Class).ThenInclude (i => i.ClassLevel)
          .Distinct ().ToListAsync ();
        decimal totalAmount = 0;
        foreach (var child in children) {
          int nextClass = child.Class.ClassLevel.DsplSeq + 1;
          //did we reached the last level (terminale)?
          if (nextClass > lastClassLevelSeq)
            continue;
          var nextClassLevel = await _context.ClassLevels.FirstAsync (c => c.DsplSeq == nextClass);
          ChildRegistrationDto crd = new ChildRegistrationDto ();
          crd.LastName = child.LastName;
          crd.FirstName = child.FirstName;
          crd.NextClass = nextClassLevel.Name;
          crd.RegistrationFee = RegistrationFee.ToString ("N0") + "FCFA";

          var classProduct = await _context.ClassLevelProducts
            .FirstAsync (c => c.ClassLevelId == nextClassLevel.Id && c.ProductId == RegTypeId);
          decimal tuitionFee = Convert.ToDecimal (classProduct.Price);
          decimal DPPct = firstDeadline.Percentage;
          decimal DownPayment = DPPct * tuitionFee;

          crd.TuitionAmount = classProduct.Price.ToString ("N0");
          crd.DueAmountPct = (DPPct * 100).ToString ("N0") + "%";
          crd.DueAmount = DownPayment.ToString ("N0");
          crd.TotalDueForChild = (RegistrationFee + DownPayment).ToString ("N0");
          red.Children.Add (crd);

          //add the order line for the child
          OrderLine orderLine = new OrderLine ();
          orderLine.OrderId = order.Id;
          orderLine.OrderLineLabel = "scolarité classe de " + nextClassLevel.Name + " de " +
            child.LastName + " " + child.FirstName;
          orderLine.ProductId = RegTypeId;
          orderLine.ProductFee = RegistrationFee;
          orderLine.Qty = 1;
          orderLine.UnitPrice = classProduct.Price;
          orderLine.TotalHT = orderLine.Qty * orderLine.UnitPrice + orderLine.ProductFee;
          orderLine.AmountHT = orderLine.TotalHT - orderLine.Discount;
          orderLine.TVA = 0;
          orderLine.TVAAmount = orderLine.TVA * orderLine.AmountHT;
          orderLine.AmountTTC = orderLine.AmountHT + orderLine.TVAAmount;
          orderLine.ChildId = child.Id;
          _repo.Add (orderLine);
          if (!await _repo.SaveAll ())
            return BadRequest ("problème pour ajouter la scolarité");

          byte seq = 1;
          foreach (var deadline in deadlines) {
            decimal Pct = deadline.Percentage;
            decimal Payment = Pct * tuitionFee;
            OrderLineDeadline orderDeadline = new OrderLineDeadline ();
            orderDeadline.OrderLineId = orderLine.Id;
            orderDeadline.Percent = Pct * 100;
            orderDeadline.Amount = Payment;
            orderDeadline.DueDate = deadline.DueDate;
            orderDeadline.Seq = seq;
            _repo.Add (orderDeadline);
            seq++;
          }

          if (RegTypeId == tuitionId) {
            if (child.RegCreated == false) {
              child.RegCreated = true;
              _repo.Update (child);

              if (parent.RegCreated == false) {
                parent.RegCreated = true;
                _repo.Update (parent);
              }
            }
          } else {
            if (child.NextRegCreated == false) {
              child.NextRegCreated = true;
              _repo.Update (child);

              if (parent.RegCreated == false) {
                parent.RegCreated = true;
                _repo.Update (parent);
              }
            }
          }

          order.TotalHT += orderLine.TotalHT;
          order.Discount += orderLine.Discount;
          order.AmountHT += orderLine.AmountHT;
          order.TVAAmount += orderLine.TVAAmount;
          order.AmountTTC += orderLine.AmountTTC;

          totalAmount += DownPayment + RegistrationFee;
        }

        _repo.Update (order);

        red.TotalAmount = totalAmount.ToString ("N0");

        EmailData.Add (red);
      }

      if (EmailData.Count () > 0) {
        var template = await _context.EmailTemplates.FirstAsync (t => t.Id == renewRegEmailId);
        var RegEmails = await _repo.SetEmailDataForRegistration (EmailData, template.Body, RegDeadLine);
        _context.AddRange (RegEmails);
        if (await _repo.SaveAll ())
          return Ok (new {
            nbEmailsSent = RegEmails.Count ()
          });
      } else {
        return Ok ();
      }

      return BadRequest ("problème pour créer les inscriptions et envoyer les emails");
    }

    [HttpGet ("UsersRecap")]
    public async Task<IActionResult> UsersRecap()
    {
      List<UserType> userTypes = await _cache.GetUserTypes();

      var dataToReturn = new List<UsersRecapDto>();
      foreach(var item in userTypes)
      {
        UsersRecapDto userRecap = new UsersRecapDto {
          UserTypeId = item.Id,
          UserTypeName = item.Name,
          TotalAccount = item.Users.Count(),
          TotalActive = item.Users.Where(u => u.AccountDataValidated == true).Count()
        };

        dataToReturn.Add(userRecap);
      }
      return Ok(dataToReturn);
    }

    [HttpGet ("SearchInscription")]
    public async Task<IActionResult> SearchInscription ([FromQuery] InscriptionSearchParams searchParams) {
      List<User> usersFromDB = new List<User> ();
      if (searchParams.LastName == null) { searchParams.LastName = ""; }
      if (searchParams.FirstName == null) { searchParams.FirstName = ""; }
      int levelId = Convert.ToInt32 (searchParams.LevelId);

      if (levelId > 0) {
        usersFromDB = await _context.Users
          .Include (p => p.Photos)
          .Where (u => u.ClassLevelId == levelId && u.ClassId == null &&
            EF.Functions.Like (u.LastName, "%" + searchParams.LastName + "%") &&
            EF.Functions.Like (u.FirstName, "%" + searchParams.FirstName + "%"))
          .ToListAsync ();
      } else {
        usersFromDB = await _context.Users
          .Include (p => p.Photos)
          .Where (u => u.ClassId == null && u.UserTypeId == studentTypeId && EF.Functions.Like (u.LastName, "%" + searchParams.LastName + "%") &&
            EF.Functions.Like (u.FirstName, "%" + searchParams.FirstName + "%"))
          .ToListAsync ();
      }

      var users = _mapper.Map<IEnumerable<UserForClassAllocationDto>> (usersFromDB);

      return Ok (users);
    }

    [HttpPost ("ImportTeachers")]
    public async Task<IActionResult> ImportTeachers (List<UserFromExelDto> usersFromExcel) {

      try {
        foreach (var userForRegister in usersFromExcel) {
          var userToCreate = _mapper.Map<User> (userForRegister);
          userToCreate.UserTypeId = teacherTypeId;
          var code = Guid.NewGuid ();
          userToCreate.UserName = code.ToString ();
          //   int userId= await _repo.AddUserPreInscription(code,userToCreate,teacherRoleId,true);

          //       foreach (var course in userForRegister.Courses)
          //       {
          //         var cours = await _context.Courses?.FirstOrDefaultAsync(c => c.Name.ToLower() == course.ToLower());
          //         if(cours != null)
          //         _repo.Add( new TeacherCourse{ TeacherId = userId, CourseId = cours.Id });
          //       }
        }

        if (await _repo.SaveAll ())
          return Ok ();

        return BadRequest ("impossible de terminer l'enregistrement");

        // return Ok(_mapper.Map<UserForListDto>(userToCreate));
      } catch (System.Exception ex) {
        string mes = ex.Message;

        return BadRequest (ex);
      }

    }

    [HttpGet ("GetTeacher/{teacherId}")]
    public async Task<IActionResult> GetTeacher (int teacherId)
    {
      //recuperation du professeur professeurs ainsi que tous ses cours
      User teacher = (await _cache.GetUsers()).FirstOrDefault(u => u.Id == teacherId);
      if (teacher != null) {
        var tdetails = new TeacherForListDto ();
        tdetails.PhoneNumber = teacher.PhoneNumber;
        tdetails.SecondPhoneNumber = teacher.SecondPhoneNumber;
        tdetails.Email = teacher.Email;
        tdetails.Id = teacher.Id;
        tdetails.LastName = teacher.LastName;
        tdetails.FirstName = teacher.FirstName;
        tdetails.DateOfBirth = teacher.DateOfBirth.ToString ("dd/MM/yyyy", frC);

        tdetails.Courses = (await _cache.GetTeacherCourses())
          .Where(t => t.TeacherId == teacherId).Select(c => c.Course).ToList();
        
        tdetails.classIds = (await _cache.GetClassCourses()).Where(t => t.TeacherId == teacherId)
          .Select(t => t.ClassId).Distinct().ToList();
        
        return Ok(tdetails);
      }
      return BadRequest ("impossible de trouver l'utilisateur");
    }

    // [HttpPost("ImportedUsers/{insertUserId}")]
    // public async Task<IActionResult> Importedteachers(int insertUserId, List<ImportUserDto> importedUsers)
    // {
    //     if (importedUsers[0].UserTypeId == teacherTypeId)
    //     {
    //         var TeacherRole = await _context.Roles.FirstOrDefaultAsync(a => a.Id == teacherRoleId);
    //         foreach (var importUser in importedUsers)
    //         {
    //             string code = Guid.NewGuid().ToString();
    //             var teacher = _mapper.Map<User>(importUser);
    //             teacher.UserName = code;
    //             //teacher.ValidationCode = code;
    //             // enregistrement du professeur
    //             int userId = await _repo.AddSelfRegister(teacher, TeacherRole.Name, true, insertUserId);
    //         }
    //         return NoContent();
    //     }

    //     if (importedUsers[0].UserTypeId == parentTypeId)
    //     {
    //         var ParentRole = await _context.Roles.FirstOrDefaultAsync(a => a.Id == parentTypeId);
    //         foreach (var importUser in importedUsers)
    //         {
    //             string code = Guid.NewGuid().ToString();
    //             var parent = _mapper.Map<User>(importUser);
    //             parent.UserName = code;
    //             // parent.ValidationCode = code;

    //             int parentId = await _repo.AddSelfRegister(parent, ParentRole.Name, true, insertUserId);

    //             if (parentId > 0)
    //             {
    //                 for (int i = 0; i < importUser.MaxChild; i++)
    //                 {
    //                     var MemberRole = await _context.Roles.FirstOrDefaultAsync(a => a.Id == memberRoleId);
    //                     code = Guid.NewGuid().ToString();
    //                     var child = new User
    //                     {
    //                         // ValidationCode = code,
    //                         UserName = code,
    //                         UserTypeId = studentTypeId
    //                     };
    //                     int childId = await _repo.AddSelfRegister(child, MemberRole.Name, false, insertUserId);
    //                     if (childId > 0)
    //                     {
    //                         var userLink = new UserLink
    //                         {
    //                             UserPId = parentId,
    //                             UserId = childId
    //                         };
    //                         _repo.Add(userLink);
    //                     }
    //                 }
    //             }
    //         }
    //         if (await _repo.SaveAll())
    //             return NoContent();
    //     }

    //     return BadRequest();
    // }

    [HttpGet ("Settings")]
    public async Task<IActionResult> GetSettings () {
      var settings = await _repo.GetSettings ();
      return Ok (settings);
    }

    [HttpPost("UpdateSettings")]
    public async Task<IActionResult> UpdateSettings(List<Setting> settings)
    {
      _context.UpdateRange(settings);
      if(await _repo.SaveAll())
      {
        await _cache.LoadSettings();
        return Ok();
      }

      return BadRequest("problème pour metre à jour les paramètres");
    }

    // [HttpGet("GetToken")]
    // public List<string> GetOrangeAccessToken()
    // {
    //   ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    //   var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.orange.com/oauth/v2/token");
    //   httpWebRequest.ContentType = "application/json";
    //   httpWebRequest.Method = "POST";
    //   httpWebRequest.Accept = "application/json";
    //   httpWebRequest.PreAuthenticate = true;
    //   httpWebRequest.Headers.Add("Authorization", "Basic YlppdEs0aHhKdjl0Q2tkc1p1ZHVMVFdkU3E3anJUN0E6OUFtUWVRd0hOQ0cyNFVQQQ==");

    //   using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
    //   {
    //     streamWriter.Write("");
    //     streamWriter.Flush();
    //     streamWriter.Close();
    //   }

    //   List<string> result = new List<string>();
    //   var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
    //   using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
    //   {
    //     result.Add(streamReader.ReadToEnd());
    //   }

    //   return result;
    // }

    // [HttpGet("TestJson")]
    // public void GetTest()
    // {
    //   SmsDataRequest data = new SmsDataRequest();
    //   data.outboundSMSMessageRequest = new OutboundSMSMessageRequest();
    //   data.outboundSMSMessageRequest.Address = "tel+22507104446";
    //   data.outboundSMSMessageRequest.SenderAddress = "tel+22507104446";
    //   data.outboundSMSMessageRequest.outboundSMSTextMessage = new OutboundSMSTextMessage();
    //   data.outboundSMSMessageRequest.outboundSMSTextMessage.Message = "hello!";
    //   string JsonArray = JsonConvert.SerializeObject(data);
    //   Dictionary<string, string> Params = new Dictionary<string, string>();
    //   Params.Add("content", "hello!");
    //   Params.Add("to", "07104446,22334455");
    //   Params.Add("validityPeriod", "1");

    //   Params["to"] = CreateRecipientList(Params["to"]);
    //   string JsonArray1 = JsonConvert.SerializeObject(Params, Formatting.None);
    //   JsonArray1 = JsonArray1.Replace("\\\"", "\"").Replace("\"[", "[").Replace("]\"", "]");
    // }

    // public static string CreateRecipientList(string to)
    // {
    //   string[] tmp = to.Split(new char[] { ',' });
    //   to = "[\"";
    //   to = to + string.Join("\",\"", tmp);
    //   to = to + "\"]";
    //   return to;
    // }

  }

}