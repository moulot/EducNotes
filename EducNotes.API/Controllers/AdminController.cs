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
    public ICacheRepository _cache { get; }
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

    // [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("RolesWithUsers")]
    public async Task<IActionResult> GetRolesWithUsers()
    {
      List<Role> rolesCached = await _cache.GetRoles();
      List<UserRole> userRoles = await _cache.GetUserRoles();

      List<RoleWithUsersDto> roles = new List<RoleWithUsersDto>();
      foreach (var role in rolesCached)
      {
        RoleWithUsersDto roleWithUsers = new RoleWithUsersDto();
        roleWithUsers.Id = role.Id;
        roleWithUsers.Name = role.Name;
        var users = userRoles.Where(u => u.RoleId == role.Id).Select(s => s.User).ToList();
        roleWithUsers.Users = new List<UserInRole>();
        foreach (var user in users)
        {
          UserInRole userInRole = new UserInRole();
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

    // [HttpGet("{roleId}/CapabilitiesByUserId")]
    // public async Task<IActionResult> GetCapabilitiesByRoleId(int roleId)
    // {
    //   List<RoleCapability> roleCapabilities = await _cache.GetRoleCapabilities();
    //   List<Capability> capabilities = roleCapabilities.Where(c => c.RoleId == roleId)
    //                                                   .Select(s => s.Capability).ToList();
    //   return Ok(capabilities);
    // }

    // [HttpGet("{userId}/RolesByUserId")]
    // public async Task<IActionResult> GetRolesByUserId(int userId)
    // {
    //   List<UserRole> userRoles = await _cache.GetUserRoles();
    //   List<Role> roles = userRoles.Where(r => r.UserId == userId).Select(s => s.Role).ToList();
    //   return Ok(roles);
    // }

    // [HttpGet("{roleId}/UsersByRoleId")]
    // public async Task<IActionResult> GetUsersByRoleId(int roleId)
    // {
    //   List<UserRole> userRoles = await _cache.GetUserRoles();
    //   List<User> users = userRoles.Where(r => r.RoleId == roleId).Select(s => s.User).ToList();
    //   return Ok(users);
    // }

    // [HttpGet("Capabilities")]
    // public async Task<IActionResult> GetCapabilities()
    // {
    //   return Ok(await _cache.GetCapabilities());
    // }

    // [HttpGet("{name}/CapabilityByName")]
    // public async Task<IActionResult> GetCapabilityByName(string name)
    // {
    //   List<Capability> capabilities = await _cache.GetCapabilities();
    //   Capability capability = capabilities.FirstOrDefault(c => c.Name.ToUpper() == name.ToUpper());
    //   return Ok(capability);
    // }

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

    // [HttpGet("{userId}/UserRoleByUserId/{roleId}")]
    // public async Task<IActionResult> GetUserRoleByUserId(int userId, int roleId)
    // {
    //   UserRole userRole = await _repo.GetUserRoleByUserId(userId, roleId);
    //   return Ok(userRole);
    // }

    [HttpGet("LoadMenu/{userTypeId}")]
    public async Task<IActionResult> GetMenu(int userTypeId)
    {
      List<MenuItem> menuItems = await _repo.GetUserTypeMenu(userTypeId);
      return Ok(menuItems);
    }

    [HttpGet("TopMenuItem/{menuItemName}")]
    public MenuItem GetTopMenuItem(string menuItemName, List<MenuItem> menuItems)
    {
      MenuItem menuItem = _repo.GetTopMenuItem(menuItemName, menuItems);
      return menuItem;
    }

    [HttpGet("{userId}/HasAccessToMenu/{menuItemId}")]
    public async Task<IActionResult> HasAccessToMenu(int userId, int menuItemId)
    {
      Boolean userHasAccessToMenu = await _repo.HasAccessToMenu(userId, menuItemId);
      return Ok(userHasAccessToMenu);
    }

    [HttpGet("GetMenuCapabilities/{userTypeId}")]
    public async Task<IActionResult> GetMenuCapabilities(int userTypeId)
    {
      List<MenuCapabilitiesDto> MenuCapabilities = await _repo.GetMenuCapabilities(userTypeId);
      return Ok(MenuCapabilities);
    }

    [HttpPost("saveRole")]
    public async Task<IActionResult> saveRole(RoleDto roleDto)
    {
      List<RoleCapability> roleCapabilities = await _cache.GetRoleCapabilities();
      List<Role> roles = await _cache.GetRoles();
      DateTime today = DateTime.Now;

      using (var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          Role role = new Role();
          role.Id = roleDto.RoleId;
          role.Name = roleDto.RoleName;

          //is it a new role?
          if (role.Id == 0)
          {
            var result = await _roleManager.CreateAsync(role);
            if(!result.Succeeded)
            {
              identityContextTransaction.Rollback();
              return BadRequest("problème pour créer le rôle");
            }

            Role newRole = await _roleManager.FindByNameAsync(role.Name);
            foreach (RoleCapabilityDto  capability in roleDto.Capabilities)
            {
              RoleCapability rc = new RoleCapability();
              rc.RoleId = newRole.Id;
              rc.CapabilityId = capability.CapabilityId;
              rc.AccessFlag = capability.AccesFlag;
              rc.InsertUserId = 1;
              rc.InsertDate = today;
              rc.UpdateUserId = 1;
              rc.UpdateDate = today;
              _repo.Add(rc);
            }
          }
          else
          {
            Role currentRole = roles.First(r => r.Id == roleDto.RoleId);
            currentRole.Name = roleDto.RoleName;
            _repo.Update(currentRole);

            //remove previous role capabilities
            List<RoleCapability> prevRoleCapabilities = roleCapabilities.Where(r => r.RoleId == currentRole.Id).ToList();
            _repo.DeleteAll(prevRoleCapabilities);

            //add new role capabilities
            foreach (RoleCapabilityDto  capability in roleDto.Capabilities)
            {
              RoleCapability rc = new RoleCapability();
              rc.RoleId = currentRole.Id;
              rc.CapabilityId = capability.CapabilityId;
              rc.AccessFlag = capability.AccesFlag;
              rc.InsertUserId = 1;
              rc.InsertDate = today;
              rc.UpdateUserId = 1;
              rc.UpdateDate = today;
              _repo.Add(rc);
            }
          }

          if(await _repo.SaveAll())
          {
            await _cache.LoadRoles();
            await _cache.LoadRoleCapabilities();
            identityContextTransaction.Commit();
            return Ok();
          }
          else
          {
            identityContextTransaction.Rollback();
            return BadRequest("problème pour enregistrer le rôle");
          }
        }
        catch
        {
          identityContextTransaction.Rollback();
          return BadRequest("problème pour enregistrer le rôle");
        }
      }
    }

  }
}