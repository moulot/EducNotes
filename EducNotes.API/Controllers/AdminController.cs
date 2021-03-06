using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
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

namespace EducNotes.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AdminController : ControllerBase
  {
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
    CultureInfo frC = new CultureInfo("fr-FR");
    public readonly ICacheRepository _cache;
    public readonly RoleManager<Role> _roleManager;

    public AdminController(DataContext context, UserManager<User> userManager, IConfiguration config,
      IEducNotesRepository repo, IOptions<CloudinarySettings> cloudinaryConfig, IMapper mapper,
      ICacheRepository cache, RoleManager<Role> roleManager)
    {
      _roleManager = roleManager;
      _cache = cache;
      _userManager = userManager;
      _config = config;
      _context = context;
      _repo = repo;
      _mapper = mapper;
      _cloudinaryConfig = cloudinaryConfig;
      teacherTypeId = _config.GetValue<int>("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int>("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int>("AppSettings:studentTypeId");
      password = _config.GetValue<string>("AppSettings:defaultPassword");
      parentRoleId = _config.GetValue<int>("AppSettings:parentRoleId");
      memberRoleId = _config.GetValue<int>("AppSettings:memberRoleId");
      moderatorRoleId = _config.GetValue<int>("AppSettings:moderatorRoleId");
      adminRoleId = _config.GetValue<int>("AppSettings:adminRoleId");
      teacherRoleId = _config.GetValue<int>("AppSettings:teacherRoleId");
      schoolInscTypeId = _config.GetValue<int>("AppSettings:schoolInscTypeId");
      renewRegEmailId = _config.GetValue<int>("AppSettings:renewRegEmailId");
      tuitionId = _config.GetValue<int>("AppSettings:tuitionId");
      nextYearTuitionId = _config.GetValue<int>("AppSettings:nextYearTuitionId");
      lastClassLevelSeq = _config.GetValue<byte>("AppSettings:lastClassLevelSeq");

      Account acc = new Account(
        _cloudinaryConfig.Value.CloudName,
        _cloudinaryConfig.Value.ApiKey,
        _cloudinaryConfig.Value.ApiSecret
      );

      _cloudinary = new Cloudinary(acc);
    }

    // [Authorize(Policy = "RequireAdminRole")]
    // [HttpGet("usersWithRoles")]
    // public async Task<IActionResult> GetUsersWithRoles () {
    //   var userList = await (from user in _context.Users orderby user.UserName select new {
    //     Id = user.Id,
    //       UserName = user.UserName,
    //       Roles = (from userRole in user.UserRoles join role in _context.Roles on userRole.RoleId equals role.Id select role.Name).ToList ()
    //   }).ToListAsync ();

    //   return Ok (userList);
    // }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("editRoles/{userName}")]
    public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
    {
      var user = await _userManager.FindByNameAsync(userName);

      var userRoles = await _userManager.GetRolesAsync(user);

      var selectedRoles = roleEditDto.RoleNames;

      //selected = selectedRoles != null? selectedRoles : new string[] {};
      selectedRoles = selectedRoles ?? new string[] { };

      var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

      if (!result.Succeeded)
        return BadRequest("Failed to add to roles");

      result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

      if (!result.Succeeded)
        return BadRequest("Failed to remove roles");

      return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photosForModeration")]
    public async Task<IActionResult> GetPhotosForModeration()
    {
      var photos = await _context.Photos
                          .Include(u => u.User)
                          .IgnoreQueryFilters()
                          .Where(p => p.IsApproved == false)
                          .Select(u => new
                          {
                            Id = u.Id,
                            userName = u.User.UserName,
                            Url = u.Url,
                            IsApproved = u.IsApproved
                          })
                          .ToListAsync();

      return Ok(photos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approvePhoto/{photoId}")]
    public async Task<IActionResult> ApprovePhoto(int photoId)
    {
      var photo = await _context.Photos
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(p => p.Id == photoId);

      photo.IsApproved = true;

      await _context.SaveChangesAsync();

      return Ok();
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("rejectPhoto/{photoId}")]
    public async Task<IActionResult> RejectPhoto(int photoId)
    {
      var photo = await _context.Photos
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(p => p.Id == photoId);

      if (photo.IsMain)
        return BadRequest("You cannot reject the main photo");

      if (photo.PublicId != null)
      {
        var deleteParams = new DeletionParams(photo.PublicId);

        var result = _cloudinary.Destroy(deleteParams);

        if (result.Result == "ok")
        {
          _context.Photos.Remove(photo);
        }
      }

      if (photo.PublicId == null)
      {
        _context.Photos.Remove(photo);
      }

      await _context.SaveChangesAsync();

      return Ok();
    }

    [HttpGet("GetAllTeachers")]
    public async Task<IActionResult> GetAllTeachers()
    {
      //recuperation de tous les professeurs
      var teachers = await _context.Users.Where(u => u.UserTypeId == teacherTypeId).ToListAsync();

      return Ok(teachers);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
    {
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      var userFromRepo = await _repo.GetUser(currentUserId, true);

      userParams.userId = currentUserId;

      var users = await _repo.GetUsers(userParams);

      var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

      Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

      return Ok(usersToReturn);
    }

    // [HttpGet("{id}", Name = "GetUser")]
    // public async Task<IActionResult> GetUser(int id)
    // {
    //     var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;

    //     var user = await _repo.GetUser(id, isCurrentUser);

    //     var userToReturn = _mapper.Map<UserForDetailedDto>(user);

    //     return Ok(userToReturn);
    // }

    [HttpGet("CurrentPeriod")]
    public async Task<IActionResult> GetCurrentPeriod()
    {
      // DATES MUST BE UP TO DATE

      // SOLUTION : DETECT THE CURRENT PERIOD WITH STARTING AND ENDING DATES OF THE PERIOD
      var today = DateTime.Now;

      // OTHER SOLUTION : SET THE CURRENT DATE WITH THE ACTIVE FLAG
      Period CurrentPeriod = await _context.Periods.Where(p => p.Active == true).FirstOrDefaultAsync();

      if (CurrentPeriod == null)
        return BadRequest("la période courante n'a pas été paramétrée");

      return Ok(CurrentPeriod);
    }

    [HttpGet("School")]
    public async Task<IActionResult> GetSchool()
    {
      var school = await _context.Establishments.SingleOrDefaultAsync();

      if (school == null)
      {
        var newSchool = new Establishment
        {
          Name = "",
          Location = "",
          Phone = "",
          WebSite = "",
          StartCoursesHour = DateTime.Now.Date,
          EndCoursesHour = DateTime.Now.Date
        };
        _context.Add(newSchool);

        if (await _repo.SaveAll())
          return Ok(newSchool);

        return BadRequest("problème pour créer les infos de l'établissement");
      }

      return Ok(school);
    }

    [HttpPut("SaveSchool")]
    public async Task<IActionResult> SaveEstablishment([FromBody] Establishment school)
    {
      if (school.Id == 0)
        _repo.Add(school);
      else
        _repo.Update(school);

      if (await _repo.SaveAll())
        return NoContent();

      throw new Exception($"l'ajout des infos de l'établissement a échoué");
    }

    [HttpPost("{classId}/StudentAffectation")]
    public async Task<IActionResult> StudentAffectation(int classId, List<StudentPostingDto> studentIds)
    {
      if (studentIds.Count() > 0)
      {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        foreach (var student in studentIds)
        {
          var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == student.Id);
          user.ClassId = classId;
        }

        if (await _repo.SaveAll())
        {
          //envoi du mail d'affectation à chaque parent
          foreach (var student in studentIds)
          {
            await _repo.sendOk(studentTypeId, student.Id);
          }
          return Ok();
        }

        return BadRequest("imposible d'effectuer cette opération");
      }

      return NotFound();
    }

    // enregistrement de préinscription : perer , mere et enfants
    [HttpPost("{id}/SavePreinscription")]
    public async Task<IActionResult> SavePreinscription(int id, [FromBody] PreInscriptionDto model)
    {
      // l'id est celui de l'utilisateur connecté
      try
      {
        int fatherId = 0;
        int motherId = 0;

        //ajout du père
        if (model.father.LastName != "" && model.father.FirstName != "" && model.father.PhoneNumber != "")
        {
          var father = _mapper.Map<User>(model.father);
          father.UserTypeId = parentTypeId;
          var guid = Guid.NewGuid();
          //   fatherId = await _repo.AddUserPreInscription(guid,father,parentRoleId,true);

        }

        //ajout de la mère
        if (!string.IsNullOrEmpty(model.mother.LastName) && string.IsNullOrEmpty(model.mother.FirstName))
        {
          var mother = _mapper.Map<User>(model.mother);
          mother.UserTypeId = parentTypeId;
          //   motherId = await _repo.AddUserPreInscription(Guid.NewGuid(),mother,parentRoleId,true);
        }

        for (int i = 0; i < model.children.Count(); i++)
        {
          var child = new User();
          var levelId = Convert.ToInt32(model.children[i].LevelId);
          child = _mapper.Map<User>(model.children[i]);
          child.UserTypeId = studentTypeId;
          int userId = 0;
          //  userId=await _repo.AddUserPreInscription(Guid.NewGuid(),child,memberRoleId,false);

          //enregistrement de l inscription
          var insc = new Inscription
          {
            InsertDate = DateTime.Now,
            ClassLevelId = levelId,
            InscriptionTypeId = schoolInscTypeId,
            InsertUserId = id,
            UserId = userId,
            Validated = false
          };
          _repo.Add(insc);

          //Ajout dans UserLink
          if (fatherId > 0)
            _repo.AddUserLink(userId, fatherId);
          if (motherId > 0)
            _repo.AddUserLink(userId, motherId);

        }

        await _repo.SaveAll();

        // if(sendEMail && user.Email!=null)
        // {
        //    var callbackUrl = _config.GetValue<String>("AppSettings:DefaultEmailValidationLink")+user.ValidationCode;
        //       await _emailSender.SendEmailAsync(user.Email, "Confirmation de compte",
        //       $"veuillez confirmez votre code au lien suivant : <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicker ici</a>.");
        // }

        return Ok("");
      }
      catch (System.Exception ex)
      {

        return BadRequest(ex);
      }
    }

    [HttpGet("{email}/VerifyEmail")]
    public async Task<IActionResult> VerifyEmail(string email)
    {
      return Ok(await _repo.EmailExist(email));
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

    [HttpPost("SendEmail")]
    public async Task<IActionResult> SendEmail(EmailFormDto model)
    {
      if (await _repo.SendEmail(model))
        return Ok("done");

      return BadRequest("impossible d'envoyer le mail");
    }

    [HttpPost("EmailRegistration/{RegTypeId}")]
    public async Task<IActionResult> SendEmailRegistration(int RegTypeId)
    {
      List<User> parents = await _context.Users
        .Where(u => u.UserTypeId == parentTypeId && u.Active == 1 && u.EmailConfirmed == true)
        .ToListAsync();

      List<Setting> settings = await _context.Settings.ToListAsync();
      var schoolName = settings.First(s => s.Name.ToLower() == "schoolname").Value;
      var smsActive = settings.First(s => s.Name.ToLower() == "sendregsms").Value;
      decimal RegistrationFee = Convert.ToDecimal(settings.First(s => s.Name.ToLower() == "regfee").Value);
      string RegDeadLine = settings.First(s => s.Name == "RegistrationDeadLine").Value;

      Boolean sendSmsToo = smsActive == "1" ? true : false;

      var deadlines = await _context.ProductDeadLines
        .OrderBy(o => o.DueDate)
        .Where(p => p.ProductId == RegTypeId).ToListAsync();

      var firstDeadline = deadlines.First(d => d.Seq == 1);
      List<RegistrationEmailDto> EmailData = new List<RegistrationEmailDto>();
      foreach (var parent in parents)
      {
        //do we already have a in-process registration for parent's children?
        if (RegTypeId == tuitionId)
        {
          if (parent.RegCreated == true)
            continue;
        }
        else
        {
          if (parent.NextRegCreated == true)
            continue;
        }

        RegistrationEmailDto red = new RegistrationEmailDto();
        red.ParentLastName = parent.LastName;
        red.ParentFirstName = parent.FirstName;
        red.ParentEmail = parent.Email;
        red.ParentCellPhone = parent.PhoneNumber;
        red.ParentGender = parent.Gender;
        red.EmailSubject = schoolName + " - inscription pour l'année scolaire prochaine";
        red.DueDate = firstDeadline.DueDate.ToString("dd/MM/yyyy", frC);

        int daysToValidate = Convert.ToInt32((settings.First(s => s.Name == "DaysToValidateReg")).Value);

        //create the corresponding order for the children registration/inscription
        Order order = new Order();
        order.OrderDate = DateTime.Now;
        order.Deadline = Convert.ToDateTime(settings.First(s => s.Name == "RegistrationDeadLine").Value);
        order.Validity = order.OrderDate.AddDays(daysToValidate);
        order.OrderLabel = "ré-inscription";
        if (parent.Gender == 0)
          order.MotherId = parent.Id;
        else
          order.FatherId = parent.Id;
        _repo.Add(order);
        if (!await _repo.SaveAll())
          return BadRequest("problème pour ajouter la scolarité");
        order.OrderNum = order.Id.GetOrderNumber();
        if (RegTypeId == tuitionId)
          order.isReg = true;
        if (RegTypeId == nextYearTuitionId)
          order.isNextReg = true;

        red.Children = new List<ChildRegistrationDto>();
        var children = await _context.UserLinks
          .Where(u => u.UserPId == parent.Id && u.User.Active == 1 && u.User.Validated == true)
          .Select(u => u.User)
          .Include(i => i.Class).ThenInclude(i => i.ClassLevel)
          .Distinct().ToListAsync();
        decimal totalAmount = 0;
        foreach (var child in children)
        {
          int nextClass = child.Class.ClassLevel.DsplSeq + 1;
          //did we reached the last level (terminale)?
          if (nextClass > lastClassLevelSeq)
            continue;
          var nextClassLevel = await _context.ClassLevels.FirstAsync(c => c.DsplSeq == nextClass);
          ChildRegistrationDto crd = new ChildRegistrationDto();
          crd.LastName = child.LastName;
          crd.FirstName = child.FirstName;
          crd.NextClass = nextClassLevel.Name;
          crd.RegistrationFee = RegistrationFee.ToString("N0") + "FCFA";

          var classProduct = await _context.ClassLevelProducts
            .FirstAsync(c => c.ClassLevelId == nextClassLevel.Id && c.ProductId == RegTypeId);
          decimal tuitionFee = Convert.ToDecimal(classProduct.Price);
          decimal DPPct = firstDeadline.Percentage;
          decimal DownPayment = DPPct * tuitionFee;

          crd.TuitionAmount = classProduct.Price.ToString("N0");
          crd.DueAmountPct = (DPPct * 100).ToString("N0") + "%";
          crd.DueAmount = DownPayment.ToString("N0");
          crd.TotalDueForChild = (RegistrationFee + DownPayment).ToString("N0");
          red.Children.Add(crd);

          //add the order line for the child
          OrderLine orderLine = new OrderLine();
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
          _repo.Add(orderLine);
          if (!await _repo.SaveAll())
            return BadRequest("problème pour ajouter la scolarité");

          byte seq = 1;
          foreach (var deadline in deadlines)
          {
            decimal Pct = deadline.Percentage;
            decimal Payment = Pct * tuitionFee;
            OrderLineDeadline orderDeadline = new OrderLineDeadline();
            orderDeadline.OrderLineId = orderLine.Id;
            orderDeadline.Percent = Pct * 100;
            orderDeadline.Amount = Payment;
            orderDeadline.DueDate = deadline.DueDate;
            orderDeadline.Seq = seq;
            _repo.Add(orderDeadline);
            seq++;
          }

          if (RegTypeId == tuitionId)
          {
            if (child.RegCreated == false)
            {
              child.RegCreated = true;
              _repo.Update(child);

              if (parent.RegCreated == false)
              {
                parent.RegCreated = true;
                _repo.Update(parent);
              }
            }
          }
          else
          {
            if (child.NextRegCreated == false)
            {
              child.NextRegCreated = true;
              _repo.Update(child);

              if (parent.RegCreated == false)
              {
                parent.RegCreated = true;
                _repo.Update(parent);
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

        _repo.Update(order);

        red.TotalAmount = totalAmount.ToString("N0");

        EmailData.Add(red);
      }

      if (EmailData.Count() > 0)
      {
        var template = await _context.EmailTemplates.FirstAsync(t => t.Id == renewRegEmailId);
        var RegEmails = await _repo.SetEmailDataForRegistration(EmailData, template.Body, RegDeadLine);
        _context.AddRange(RegEmails);
        if (await _repo.SaveAll())
          return Ok(new
          {
            nbEmailsSent = RegEmails.Count()
          });
      }
      else
      {
        return Ok();
      }

      return BadRequest("problème pour créer les inscriptions et envoyer les emails");
    }

    [HttpGet("SearchInscription")]
    public async Task<IActionResult> SearchInscription([FromQuery] InscriptionSearchParams searchParams)
    {
      List<User> usersFromDB = new List<User>();
      if (searchParams.LastName == null) { searchParams.LastName = ""; }
      if (searchParams.FirstName == null) { searchParams.FirstName = ""; }
      int levelId = Convert.ToInt32(searchParams.LevelId);

      if (levelId > 0)
      {
        usersFromDB = await _context.Users
          .Include(p => p.Photos)
          .Where(u => u.ClassLevelId == levelId && u.ClassId == null &&
           EF.Functions.Like(u.LastName, "%" + searchParams.LastName + "%") &&
           EF.Functions.Like(u.FirstName, "%" + searchParams.FirstName + "%"))
          .ToListAsync();
      }
      else
      {
        usersFromDB = await _context.Users
          .Include(p => p.Photos)
          .Where(u => u.ClassId == null && u.UserTypeId == studentTypeId && EF.Functions.Like(u.LastName, "%" + searchParams.LastName + "%") &&
           EF.Functions.Like(u.FirstName, "%" + searchParams.FirstName + "%"))
          .ToListAsync();
      }

      var users = _mapper.Map<IEnumerable<UserForClassAllocationDto>>(usersFromDB);

      return Ok(users);
    }

    [HttpPost("ImportTeachers")]
    public async Task<IActionResult> ImportTeachers(List<UserFromExelDto> usersFromExcel)
    {

      try
      {
        foreach (var userForRegister in usersFromExcel)
        {
          var userToCreate = _mapper.Map<User>(userForRegister);
          userToCreate.UserTypeId = teacherTypeId;
          var code = Guid.NewGuid();
          userToCreate.UserName = code.ToString();
          //   int userId= await _repo.AddUserPreInscription(code,userToCreate,teacherRoleId,true);

          //       foreach (var course in userForRegister.Courses)
          //       {
          //         var cours = await _context.Courses?.FirstOrDefaultAsync(c => c.Name.ToLower() == course.ToLower());
          //         if(cours != null)
          //         _repo.Add( new TeacherCourse{ TeacherId = userId, CourseId = cours.Id });
          //       }
        }

        if (await _repo.SaveAll())
          return Ok();

        return BadRequest("impossible de terminer l'enregistrement");

        // return Ok(_mapper.Map<UserForListDto>(userToCreate));
      }
      catch (System.Exception ex)
      {
        string mes = ex.Message;

        return BadRequest(ex);
      }

    }

    [HttpGet("GetTeacher/{teacherId}")]
    public async Task<IActionResult> GetTeacher(int teacherId)
    {
      //recuperation du professeur professeurs ainsi que tous ses cours
      User teacher = await _context.Users.FirstOrDefaultAsync(u => u.Id == teacherId);
      if (teacher != null)
      {
        var tdetails = new TeacherForListDto();
        tdetails.PhoneNumber = teacher.PhoneNumber;
        tdetails.SecondPhoneNumber = teacher.SecondPhoneNumber;
        tdetails.Email = teacher.Email;
        tdetails.Id = teacher.Id;
        tdetails.LastName = teacher.LastName;
        tdetails.FirstName = teacher.FirstName;
        tdetails.DateOfBirth = teacher.DateOfBirth.ToString("dd/MM/yyyy", frC);

        tdetails.Courses = await _context.TeacherCourses
          .Where(t => t.TeacherId == teacherId).Select(c => c.Course).ToListAsync();

        tdetails.classIds = await _context.ClassCourses.Where(t => t.TeacherId == teacherId)
          .Select(t => t.ClassId).Distinct().ToListAsync();

        return Ok(tdetails);
      }
      return BadRequest("impossible de trouver l'utilisateur");
    }

    [HttpGet("Settings")]
    public async Task<IActionResult> GetSettings()
    {
      var settings = await _repo.GetSettings();
      return Ok(settings);
    }

    [HttpPost("UpdateSettings")]
    public async Task<IActionResult> UpdateSettings(List<Setting> settings)
    {
      foreach (var item in settings)
      {
        if (item.Name.ToLower() == "subdomain") { continue; }
        _context.Settings.Update(item);
      }

      if (await _repo.SaveAll())
      {
        // await _cache.LoadSettings();
        return Ok();
      }

      return BadRequest("problème pour metre à jour les paramètres");
    }

    [HttpGet("{roleId}/RoleWithUsers")]
    public async Task<IActionResult> GetRoleWithUsers(int roleId)
    {
      List<UserRole> userRoles = await _context.UserRoles.Include(a => a.User)
                                                         .ThenInclude(i => i.Photos)
                                                         .ToListAsync();
      List<RoleCapability> roleCapabilities = await _context.RoleCapabilities.ToListAsync();

      Role currentRole = await _context.Roles.FirstAsync(r => r.Id == roleId);
      RoleWithUsersDto role = new RoleWithUsersDto();
      role.Id = currentRole.Id;
      role.Name = currentRole.Name;

      List<User> users = userRoles.Where(u => u.RoleId == role.Id).Select(s => s.User).ToList();
      foreach(var user in users)
      {
        UserInRoleDto userInRole = new UserInRoleDto();
        userInRole.LastName = user.LastName;
        userInRole.FirstName = user.FirstName;
        Photo photo = user.Photos.FirstOrDefault(p => p.IsMain == true);
        if (photo != null)
          userInRole.PhotoUrl = photo.Url;
        role.Users.Add(userInRole);
      }

      List<RoleCapability> capabilities = roleCapabilities.Where(c => c.RoleId == roleId).ToList();
      role.Capabilities = capabilities;

      return Ok(role);
    }

    // [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("RolesWithUsers")]
    public async Task<IActionResult> GetRolesWithUsers()
    {
      List<Role> rolesCached = await _context.Roles.OrderBy(r => r.Name).ToListAsync();//_cache.GetRoles();//
      List<UserRole> userRoles = await _context.UserRoles
                                        .Include( a=> a.User).ThenInclude(i => i.Photos)
                                            .ToListAsync();

      List<RoleWithUsersDto> roles = new List<RoleWithUsersDto>();
      foreach (var role in rolesCached)
      {
        RoleWithUsersDto roleWithUsers = new RoleWithUsersDto();
        roleWithUsers.Id = role.Id;
        roleWithUsers.Name = role.Name;
        var users = userRoles.Where(u => u.RoleId == role.Id).Select(s => s.User).ToList();
        foreach (var user in users)
        {
          UserInRoleDto userInRole = new UserInRoleDto();
          userInRole.LastName = user.LastName;
          userInRole.FirstName = user.FirstName;
          Photo photo = user.Photos.FirstOrDefault(p => p.IsMain == true);
          if (photo != null)
            userInRole.PhotoUrl = photo.Url;
          roleWithUsers.Users.Add(userInRole);
        }
        roles.Add(roleWithUsers);
      }

      return Ok(roles);
    }

    [HttpGet("Roles")]
    public async Task<IActionResult> GetRoles()
    {
      List<Role> roles = await _cache.GetRoles();
      return Ok(roles);
    }

    [HttpGet("EmpData")]
    public async Task<IActionResult> GetEmpData()
    {
      List<Role> roles = await _cache.GetRoles();
      List<District> districts = await _cache.GetDistricts();
      List<MaritalStatus> maritalStatus = await _cache.GetMaritalStatus();
      return Ok(new
      {
        roles,
        districts,
        maritalStatus
      });
    }

    [HttpGet("{menuItemId}/CapabilitiesByMenuItemId")]
    public async Task<IActionResult> GetCapabilitiesByMenuItelId(int menuItemId)
    {
      List<Capability> capabilitiesCached = await _cache.GetCapabilities();
      List<Capability> capabilities = capabilitiesCached.Where(c => c.MenuItemId == menuItemId)
                                                        .OrderBy(o => o.Name)
                                                        .ToList();
      return Ok(capabilities);
    }

    [HttpGet("{menuItemId}/RoleCapabilitiesByMenuItemId")]
    public async Task<IActionResult> GetRoleCapabilitiesByMenuItelId(int menuItemId)
    {
      List<RoleCapability> roleCapabilitiesCached = await _cache.GetRoleCapabilities();
      List<RoleCapability> roleCapabilities = roleCapabilitiesCached.Where(c => c.Capability.MenuItemId == menuItemId)
                                                                    .ToList();
      return Ok(roleCapabilities);
    }

    [HttpGet("{capabilityId}/RoleCapabilityByCapabilityId/{roleId}")]
    public async Task<IActionResult> GetRoleCapabilityByCapabilityId(int capabilityId, int roleId)
    {
      List<RoleCapability> roleCapabilities = await _cache.GetRoleCapabilities();
      RoleCapability roleCapability = roleCapabilities
                      .FirstOrDefault(c => c.Capability.Id == capabilityId && c.RoleId == roleId);
      return Ok(roleCapability);
    }

    [HttpGet("{roleId}/RoleCapabilitiesByRoleId")]
    public async Task<IActionResult> GetRoleCapabilitiesByRoleId(int roleId)
    {
      List<RoleCapability> roleCapabilitiesCached = await _cache.GetRoleCapabilities();
      List<RoleCapability> roleCapabilities = roleCapabilitiesCached.Where(c => c.RoleId == roleId).ToList();
      return Ok(roleCapabilities);
    }

    [HttpGet("LoadMenu/{userTypeId}")]
    public async Task<IActionResult> GetMenu(int userTypeId)
    {
      int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      List<MenuItemDto> menuItems = await _repo.GetUserTypeMenu(userTypeId, currentUserId);
      return Ok(menuItems);
    }

    [HttpGet("GetMenuCapabilities/{userTypeId}")]
    public async Task<IActionResult> GetMenuCapabilities(int userTypeId)
    {
      int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      List<MenuCapabilitiesDto> MenuCapabilities = await _repo.GetMenuCapabilities(userTypeId, currentUserId);
      return Ok(MenuCapabilities);
    }

    [HttpPost("saveRole")]
    public async Task<IActionResult> saveRole(RoleDto role)
    {
      ErrorDto error = await _repo.SaveRole(role);
      
      if(error.NoError)
        return Ok();
      else
        return BadRequest(error);
    }

    [HttpGet("Employees")]
    public async Task<IActionResult> GetEmployees()
    {
      List<User> users = await _cache.GetEmployees();
      return Ok(users);
    }

    [HttpGet("RoleEmployees/{roleId}")]
    public async Task<IActionResult> GetRoleEmployees(int roleId)
    {
      // List<UserRole> userRoles = await _cache.GetUserRoles();
      List<User> employees = await _context.Users.Include(i => i.Photos)
                                                 .Where(u => u.UserTypeId == adminTypeId).ToListAsync(); //_cache.GetEmployees();

      List<UserInRoleDto> usersInRole = new List<UserInRoleDto>();
      List<UserInRoleDto> usersNotInRole = new List<UserInRoleDto>();
      List<UserRole> roleUsers = await _context.UserRoles.Where(r => r.RoleId == roleId).ToListAsync();
      foreach (User emp in employees)
      {
        UserRole userRole = roleUsers.FirstOrDefault(u => u.UserId == emp.Id);

        UserInRoleDto user = new UserInRoleDto();
        user.Id = emp.Id;
        user.LastName = emp.LastName;
        user.FirstName = emp.FirstName;
        Photo photo = emp.Photos.FirstOrDefault(p => p.IsMain == true);
        if (photo != null)
          user.PhotoUrl = photo.Url;

        if(userRole != null)
        {
          usersInRole.Add(user);
        }
        else
        {
          usersNotInRole.Add(user);
        }
      }

      usersInRole = usersInRole.OrderBy(o => o.LastName).ThenBy(o => o.FirstName).ToList();
      usersNotInRole = usersNotInRole.OrderBy(o => o.LastName).ThenBy(o => o.FirstName).ToList();

      return Ok(new {
        usersInRole,
        usersNotInRole
      });
    }

    [HttpGet("tuitionFees")]
    public async Task<IActionResult> GetTuitionFees()
    {
      List<ClassLevelProduct> levelProducts = await _cache.GetClassLevelProducts();
      List<Product> products = await _cache.GetProducts();
      List<ProductDeadLine> productsDeadlines = await _cache.GetProductDeadLines();

      Product tuition = products.First(p => p.Id == tuitionId);
      List<ClassLevelProduct> tuitionLevels = levelProducts.Where(p => p.ProductId == tuitionId).ToList();
      
      TuitionFeeDto tuitionFees = new TuitionFeeDto();
      tuitionFees.ProductId = tuition.Id;
      tuitionFees.ProductName = tuition.Name;
      List<LevelProductPriceDto> levelFees = new List<LevelProductPriceDto>();
      foreach (var item in tuitionLevels)
      {
        LevelProductPriceDto levelFee = new LevelProductPriceDto();
        levelFee.Id = item.Id;
        levelFee.LevelId = item.ClassLevelId;
        levelFee.LevelName = item.ClassLevel.Name;
        levelFee.Price = item.Price;
        levelFees.Add(levelFee);
      }
      tuitionFees.LevelFees = levelFees;
      
      List<ProductDeadLine> dueDatesFromDB = productsDeadlines.OrderBy(o => o.Seq)
                                                              .Where(p => p.ProductId == tuitionId).ToList();
      List<ProductDeadlineDto> dueDates = _mapper.Map<List<ProductDeadlineDto>>(dueDatesFromDB);
      
      return Ok(new {
        tuitionFees,
        dueDates
      });
    }

    // [HttpPost("SaveClassLevelProducts")]
    // public async Task<IActionResult> SaveClassLevelProducts(List<ClasslevelProductDto> levelProducts)
    // {
    //   ErrorDto error = await _repo.SaveClassLevelProducts(levelProducts);
    //   if(error.NoError == true)
    //     return Ok();
    //   else
    //     return BadRequest(error.Message);
    // }

    [HttpPost("saveLevelTuitionFees")]
    public async Task<IActionResult> SaveLevelTuitionFees(ScheduleDueDateFeeDto tuitionFeeData)
    {
      ErrorDto error = await _repo.SaveLevelTuitionFees(tuitionFeeData);
      if(error.NoError == true)
        return Ok();
      else
        return BadRequest(error.Message);
    }

    [HttpPost("SaveService")]
    public async Task<IActionResult> SaveService(ServiceForSaveDto serviceDto)
    {
      List<Product> products = await _cache.GetProducts();
      List<ClassLevelProduct> classlevelProducts = await _cache.GetClassLevelProducts();
      List<ProductZone> productZones = await _cache.GetProductZones();
      List<ProductDeadLine> productDeadLines = await _cache.GetProductDeadLines();

      using(var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          // delete previous by classlevel prices
          List<ClassLevelProduct> prevLevelPrices = classlevelProducts.Where(c => c.ProductId == serviceDto.Id).ToList();
          _repo.DeleteAll(prevLevelPrices);
          // delete previous by zone prices
          List<ProductZone> prevZonePrices = productZones.Where(z => z.ProductId == serviceDto.Id).ToList();
          _repo.DeleteAll(prevZonePrices);
          //delete previous dueDates
          List<ProductDeadLine> prevDeadlines = productDeadLines.Where(p => p.ProductId == serviceDto.Id).ToList();
          _repo.DeleteAll(prevDeadlines);

          int serviceId = serviceDto.Id;
          Product service = products.Single(p => p.Id == serviceId);
          service.Name = serviceDto.Name;
          // service.ProductTypeId = serviceDto.ProductTypeId;
          service.PayableAtId = serviceDto.PayableAtId;
          var startDateArray = serviceDto.strStartDate.Split("/");
          var dateday = Convert.ToInt32(startDateArray[0]);
          var datemonth = Convert.ToInt32(startDateArray[1]);
          var dateyear = Convert.ToInt32(startDateArray[2]);
          service.ServiceStartDate = new DateTime(dateyear, datemonth, dateday);;

          if(serviceDto.IsPeriodic) {
            service.IsPeriodic = true;
            service.PeriodicityId = serviceDto.PeriodicityId;
          }
          else
          {
            service.IsPeriodic = false;
            service.PeriodicityId = null;
          }

          if(serviceDto.IsByLevel)
          {
            service.IsByLevel = true;
            service.IsByZone = false;
            service.Price = null;
            await _context.AddRangeAsync(serviceDto.LevelPrices);
          }
          else if(serviceDto.IsByZone)
          {
            service.IsByZone = true;
            service.IsByLevel = false;
            service.Price = null;
            await _context.AddRangeAsync(serviceDto.ZonePrices);
          }
          else
          {
            service.IsByLevel = false;
            service.IsByZone = false;
            service.Price = serviceDto.Price;
          }

          if(serviceDto.IsPaidCash)
          {
            service.IsPaidCash = true;
          }
          else
          {
            service.IsPaidCash = false;
            byte i = 1;
            foreach(ProductDeadlineDto dueDate in serviceDto.DueDates)
            {
              ProductDeadLine productDueDate = new ProductDeadLine();
              productDueDate.ProductId = serviceId;
              var dateArray = dueDate.strDueDate.Split("/");
              var day = Convert.ToInt32(dateArray[0]);
              var month = Convert.ToInt32(dateArray[1]);
              var year = Convert.ToInt32(dateArray[2]);
              productDueDate.DueDate = new DateTime(year, month, day);
              productDueDate.DeadLineName = dueDate.DeadLineName;
              productDueDate.Seq = i++;
              productDueDate.Percentage = dueDate.Percentage / 100;
              _repo.Add(productDueDate);
            }
          }

          _repo.Update(service);

          await _repo.SaveAll();
          identityContextTransaction.Commit();
          await _cache.LoadProducts();
          await _cache.LoadClassLevelProducts();
          await _cache.LoadProductDeadLines();
          await _cache.LoadProductZones();
          return Ok();
        }
        catch(Exception ex)
        {
          var dd = ex.Message;
          identityContextTransaction.Rollback();
          return BadRequest("problème pour enregistrer le service.");
        }
      }
    }

    [HttpGet("Zones")]
    public async Task<IActionResult> GetZones()
    {
      List<Zone> zonesCached = await _context.Zones.OrderBy(o => o.Name).ToListAsync();

      List<ZoneDto> zones = new List<ZoneDto>();
      foreach (Zone zone in zonesCached)
      {
        ZoneDto zoneDto = new ZoneDto();
        zoneDto.Id = zone.Id;
        zoneDto.Name = zone.Name;

        zones.Add(zoneDto);
      }

      return Ok(zones);
    }

    [HttpGet("ZonesWithLocations")]
    public async Task<IActionResult> GetZonesWithLocations()
    {
      List<ProductZone> productZones = await _cache.GetProductZones();
      List<Zone> zonesCached = await _cache.GetZones(); // _context.Zones.OrderBy(o => o.Name).ToListAsync();
      List<LocationZone> locationZones = await _cache.GetLocationZones(); // _context.LocationZones.Include(i => i.City)
                                                                    //  .Include(i => i.District)
                                                                    //  .Include(i => i.Country).ToListAsync();

      Boolean zoneUsed = productZones.Count() > 0 ? true : false;
      List<ZoneDto> zones = new List<ZoneDto>();
      foreach (Zone zone in zonesCached)
      {
        ZoneDto zoneDto = new ZoneDto();
        zoneDto.Id = zone.Id;
        zoneDto.Name = zone.Name;
        zoneDto.Used = zoneUsed;

        List<LocationZone> zoneLocations = locationZones.Where(z => z.ZoneId == zone.Id).ToList();
        if(zoneLocations.Count() > 0)
        {
          foreach (LocationZone location in zoneLocations)
          {
            LocationZoneDto locationDto = new LocationZoneDto();
            locationDto.ZoneId = zone.Id;
            if(location.DistrictId != null)
            {
              locationDto.DistrictId = Convert.ToInt32(location.DistrictId);
              locationDto.DistrictName = location.District.Name;
            }
            if(location.CityId != null)
            {
              locationDto.CityId = Convert.ToInt32(location.CityId);
              locationDto.CityName = location.City.Name;
            }
            if(location.CountryId != null)
            {
              locationDto.CountryId = Convert.ToInt32(location.CountryId);
              locationDto.CountryName = location.Country.Name;
            }

            zoneDto.Locations.Add(locationDto);
          }
        }

        zones.Add(zoneDto);
      }

      List<District> districtsFromDB = await _repo.GetDistricts();
      List<DistrictDto> districts = _mapper.Map<List<DistrictDto>>(districtsFromDB);

      return Ok(new {
        zones,
        districts
        });
    }

    [HttpPost("SaveZones")]
    public async Task<IActionResult> SaveZone(List<ZoneDto> zonesDto)
    {
      List<Zone> zones = await _cache.GetZones(); // _context.Zones.OrderBy(o => o.Name).ToListAsync();
      List<District> districts = await _cache.GetDistricts();
      List<LocationZone> locations = await _cache.GetLocationZones(); // _context.LocationZones.ToListAsync();//.GetLocationZones();

      using(var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          foreach (ZoneDto zoneDto in zonesDto)
          {
            int zoneId = zoneDto.Id;
            Zone zone = new Zone();
            if(zoneId == 0)
            {
              zone.Name = zoneDto.Name;
              _repo.Add(zone);
              _context.SaveChanges();
            }
            else
            {
              zone = zones.Single(z => z.Id == zoneId);
              if(zoneDto.Deleted == true)
              {
                _repo.Delete(zone);
              }
              else
              {
                zone.Name = zoneDto.Name;
                _repo.Update(zone);
              }

              List<LocationZone> prevLocations = locations.Where(l => l.ZoneId == zone.Id).ToList();
              _repo.DeleteAll(prevLocations);
            }

            foreach(LocationZoneDto locationDto in zoneDto.Locations)
            {
              LocationZone location = new LocationZone();
              location.ZoneId = zone.Id;
              location.DistrictId = locationDto.DistrictId;
              location.CityId = districts.Single(d => d.Id == locationDto.DistrictId).CityId;
              _repo.Add(location);
            }
          }

          await _repo.SaveAll();
          identityContextTransaction.Commit();
          await _cache.LoadZones();
          await _cache.LoadLocationZones();
          return Ok();
        }
        catch(Exception ex)
        {
          var dd = ex.Message;
          var errorNum = ex.HResult;
          identityContextTransaction.Rollback();
          if(errorNum == -2146233088)
            return BadRequest("les zones sont déjà utilisées pour les produits et services");
          else
            return BadRequest("problème pour enregistrer la zone");
        }
      }
    }

    [HttpGet("Districts")]
    public async Task<IActionResult> GetDistricts()
    {
      List<District> districtsFromDB = await _repo.GetDistricts();
      List<DistrictDto> districts = _mapper.Map<List<DistrictDto>>(districtsFromDB);
      return Ok(districts);
    }

  }
}