using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
    int teacherTypeId,parentTypeId,studentTypeId,adminTypeId;
    int parentRoleId,memberRoleId,moderatorRoleId,adminRoleId,teacherRoleId,schoolInscTypeId;

    public AdminController(DataContext context, UserManager<User> userManager, IConfiguration config,
      IEducNotesRepository repo, IOptions<CloudinarySettings> cloudinaryConfig, IMapper mapper)
    {
      _userManager = userManager;
      _config = config;
      _context = context;
      _repo = repo;
      _mapper = mapper;
      _cloudinaryConfig = cloudinaryConfig;
      teacherTypeId =  _config.GetValue<int>("AppSettings:teacherTypeId");
      parentTypeId =  _config.GetValue<int>("AppSettings:parentTypeId");
      adminTypeId =  _config.GetValue<int>("AppSettings:adminTypeId");
      studentTypeId =  _config.GetValue<int>("AppSettings:studentTypeId");
      password = _config.GetValue<string>("AppSettings:defaultPassword");
      parentRoleId = _config.GetValue<int>("AppSettings:parentRoleId");
      memberRoleId = _config.GetValue<int>("AppSettings:memberRoleId");
      moderatorRoleId = _config.GetValue<int>("AppSettings:moderatorRoleId");
      adminRoleId = _config.GetValue<int>("AppSettings:adminRoleId");
      teacherRoleId = _config.GetValue<int>("AppSettings:teacherRoleId");
      schoolInscTypeId = _config.GetValue<int>("AppSettings:schoolInscTypeId");

      Account acc = new Account(
          _cloudinaryConfig.Value.CloudName,
          _cloudinaryConfig.Value.ApiKey,
          _cloudinaryConfig.Value.ApiSecret
      );

      _cloudinary = new Cloudinary(acc);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("usersWithRoles")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
      var userList = await (from user in _context.Users orderby user.UserName
                              select new {
                                Id = user.Id,
                                UserName = user.UserName,
                                Roles = (from userRole in user.UserRoles
                                          join role in _context.Roles
                                          on userRole.RoleId
                                          equals role.Id
                                          select role.Name).ToList()
                            }).ToListAsync();

      return Ok(userList);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("editRoles/{userName}")]
    public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
    {
        var user = await _userManager.FindByNameAsync(userName);

        var userRoles = await _userManager.GetRolesAsync(user);

        var selectedRoles = roleEditDto.RoleNames;

        //selected = selectedRoles != null? selectedRoles : new string[] {};
        selectedRoles = selectedRoles ?? new string[] {};
        
        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if(!result.Succeeded)
            return BadRequest("Failed to add to roles");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if(!result.Succeeded)
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
            }).ToListAsync();

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

        if(photo.PublicId != null)
        {
            var deleteParams = new DeletionParams(photo.PublicId);

            var result = _cloudinary.Destroy(deleteParams);

            if(result.Result == "ok")
            {
                _context.Photos.Remove(photo);
            }
        }

        if(photo.PublicId == null)
        {
            _context.Photos.Remove(photo);
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("GetUserTypes")]
    public async Task<IActionResult> GetUserTypes()
    {
        return Ok(await _context.UserTypes.OrderBy(e => e.Name).ToListAsync());
    }
    

    [HttpGet("GetAllTeachers")]
    public async Task<IActionResult> GetAllTeachers()
    {
      //recuperation de tous les professeurs
      var teachers =await _context.Users.Where(u=>u.UserTypeId == teacherTypeId).ToListAsync();

      return Ok(teachers);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var userFromRepo = await _repo.GetUser(currentUserId, true);

        userParams.userId = currentUserId;

        // if(string.IsNullOrEmpty(userParams.Gender))
        // {
        //     userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
        // }

        var users = await _repo.GetUsers(userParams);

        var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

        Response.AddPagination(users.CurrentPage, users.PageSize,
        users.TotalCount, users.TotalPages);

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
      Period CurrentPeriod = await _context.Periods.Where(p => p.Active == 1).FirstOrDefaultAsync();

      if(CurrentPeriod == null)
        return BadRequest("la période courante n'a pas été paramétrée");

      return Ok(CurrentPeriod);
    }

    [HttpGet("School")]
    public async Task<IActionResult> GetSchool()
    {
      var school = await _context.Establishments.SingleOrDefaultAsync();

      if(school == null)
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

        if(await _repo.SaveAll())
          return Ok(newSchool);
        
        return BadRequest("problème pour créer les infos de l'établissement");
      }

      return Ok(school);
    }

    [HttpPut("SaveSchool")]
    public async Task<IActionResult> SaveEstablishment([FromBody]Establishment school)
    {
      if(school.Id == 0)
        _repo.Add(school);
      else
        _repo.Update(school);

        if(await _repo.SaveAll())
            return NoContent();

        throw new Exception($"l'ajout des infos de l'établissement a échoué");
    }


  /////////////////////////////////////////////////////////////////////////////////////////////////////
  /////////////////////////////// DATA FROM MOHAMED KABORE ////////////////////////////////////////////
  /////////////////////////////////////////////////////////////////////////////////////////////////////
    [HttpPost("{classId}/StudentAffectation")]
    public async Task<IActionResult> StudentAffectation(int classId,List<StudentPostingDto> model)
    {
      if(model.Count() > 0)
      {
          foreach (var student in model)
          {
              var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == student.UserId);
              user.ClassId = classId;

              var inscription = await _context.Inscriptions.FirstOrDefaultAsync(i => i.Id == student.Id);
              inscription.Validated = true;
              inscription.ValidatedDate = DateTime.Now;
          }
          
          if(await _repo.SaveAll())
          {
            //envoi du mail d'affectation à chaque parent
           foreach (var student in model)
           {
                await _repo.sendOk(studentTypeId, student.UserId);
           }
            return Ok();
          }

          return BadRequest("imposible d'effectuer cette opération");
      }

      return NotFound();
    }




    [HttpPost("AddUser")]
    public async Task<IActionResult> AddUser(UserForRegisterDto userForRegisterDto)
    {

        var userToCreate = _mapper.Map<User>(userForRegisterDto);
        var code  = Guid.NewGuid();
        userToCreate.UserName = code.ToString();
        try
        {
            await _repo.AddUserPreInscription(code,userToCreate,teacherRoleId,true);
            
            if(userForRegisterDto.CourseIds!=null)
            {
                    foreach (var course in userForRegisterDto.CourseIds)
                    {
                        var newCourse =  new TeacherCourse {CourseId = course, TeacherId = userToCreate.Id};
                        _repo.Add(newCourse);
                    }
                //     var teacherToReturn = new TeacherForListDto(){
                //     // Teacher =  _mapper.Map<UserForListDto>(await _repo.GetUser(userToCreate.Id,false)),
                //     // Courses = await _repo.GetTeacherCoursesAndClasses(userToCreate.Id)
                // };
                
                // return Ok(teacherToReturn);
                 if( await _repo.SaveAll())
                return Ok();

                return BadRequest("impossible de terminer l'enregistrement");
            }

            else
            return Ok();
            // return Ok(_mapper.Map<UserForListDto>(userToCreate));
            
        }
        catch (System.Exception ex)
        {
            string mes = ex.Message;

            return BadRequest(ex);
        }    
    }
  
  
    [HttpGet("ClassLevelDetails")]
    public async Task<IActionResult> ClassLevelDetails()
    {
      var levels =await _context.ClassLevels
      .Include(i=>i.Inscriptions) 
      .Include(c=>c.Classes)
      .ThenInclude(c=>c.Students)
      .OrderBy(a=>a.Name)
      .ToListAsync();
      var dataToReturn = new List<ClassLevelDetailDto>();
      foreach (var item in levels)
      {
       var res = new ClassLevelDetailDto();
       res.Id = item.Id;
       res.Name = item.Name;
       res.TotalEnrolled = item.Inscriptions.Count();
       res.TotalValdated = item.Inscriptions.Where(a=>a.Validated == true).Count();
       res.AvailableClasses = 0;
       res.AvailablePlaces = 0;
       foreach (var c in item.Classes)
       {
          int diff = Convert.ToInt32(c.MaxStudent )- c.Students.Count();
          if(diff>0)
          {
            res.AvailableClasses ++;
            res.AvailablePlaces += diff;
          }   
       }
        dataToReturn.Add(res);

      }
      return Ok(dataToReturn);
    }
 
    [HttpGet("LastUsersAdded")]
    public async Task<IActionResult> LastUsersAdded()
    {
      var usersToReturn = await _context.Users
      .Include(t=>t.UserType)
      .OrderByDescending(a=>a.Created)
      .Take(20)
      .ToListAsync()
      ;
      return Ok(_mapper.Map<IEnumerable<UserForDetailedDto>>(usersToReturn));
    }

    [HttpGet("LastUsersActivated")]
    public async Task<IActionResult> LastUsersActivated()
    {
      var usersToReturn = await _context.Users
      .Include(t=>t.UserType)
      .OrderByDescending(a=>a.ValidationDate)
      .Take(20)
      .ToListAsync()
      ;
      return Ok(_mapper.Map<IEnumerable<UserForDetailedDto>>(usersToReturn));
    }

    [HttpGet("GetClassLevels")]
    public  async Task<IActionResult> GetClassLevels()
    {
          var levels =await _context.ClassLevels
                    .Include(i => i.Inscriptions) 
                    .Include(c => c.Classes).ThenInclude(c => c.Students)
                    .OrderBy(a => a.DsplSeq)
                    .ToListAsync();

          var dataToReturn = new List<ClassLevelDetailDto>();
          foreach (var item in levels)
          {
          var res = new ClassLevelDetailDto();
          res.Id = item.Id;
          res.Name = item.Name;
          
          res.TotalEnrolled = item.Inscriptions.Count();
          res.TotalStudents = 0;
          foreach (var c in item.Classes)
          {
              res.TotalStudents+=c.Students.Count();
          }
          
          res.Classes = item.Classes.ToList();
            dataToReturn.Add(res);

          }
          return Ok(dataToReturn);
    }
  
      // enregistrement de préinscription : perer , mere et enfants
   [HttpPost("{id}/SavePreinscription")]
    public async Task<IActionResult> SavePreinscription(int id,[FromBody]PreInscriptionDto model)
    {
      // l'id est celui de l'utilisateur connecté
            try
            {
                int fatherId = 0;
                int motherId = 0;
                
                  //ajout du père
                if(model.father.LastName!="" && model.father.FirstName!="" && model.father.PhoneNumber!="")
                {
                    var father = _mapper.Map<User>(model.father);
                    father.UserTypeId =  parentTypeId;
                    var guid =Guid.NewGuid();
                    fatherId = await _repo.AddUserPreInscription(guid,father,parentRoleId,true);
                        
                }
            
                //ajout de la mère
                if(!string.IsNullOrEmpty(model.mother.LastName) && string.IsNullOrEmpty(model.mother.FirstName) )
                {
                    var mother = _mapper.Map<User>(model.mother);
                    mother.UserTypeId =  parentTypeId;
                    motherId = await _repo.AddUserPreInscription(Guid.NewGuid(),mother,parentRoleId,true);
                }

                
                for (int i = 0; i < model.children.Count(); i++)
                    {
                        var child = new User();
                        var levelId = Convert.ToInt32(model.children[i].LevelId);
                        child = _mapper.Map<User>(model.children[i]);
                        child.UserTypeId =  studentTypeId;
                        int userId = 0;
                        userId=await _repo.AddUserPreInscription(Guid.NewGuid(),child,memberRoleId,false);

                        //enregistrement de l inscription
                        var insc = new Inscription {
                          InsertDate = DateTime.Now,
                          ClassLevelId = levelId,
                          InscriptionTypeId = schoolInscTypeId,
                          InsertUserId = id,
                          UserId = userId,
                          Validated = false
                        };
                          _repo.Add(insc);

                          //Ajout dans UserLink
                          if(fatherId>0)
                          _repo.AddUserLink(userId,fatherId);
                          if(motherId>0)
                          _repo.AddUserLink(userId,motherId);
                                                        
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

    [HttpPost("SendRegisterEmail")]
    public async Task<IActionResult> SendRegisterEmail(SelfRegisterDto model)
    {
     
       if(model.UserTypeId ==teacherTypeId)
      {
          var TeacherRole   = await _context.Roles.FirstOrDefaultAsync(a=>a.Id==teacherRoleId);
         
          string code = Guid.NewGuid().ToString();
          var teacher = new User{
              ValidationCode = code,
              Email = model.Email,
              UserName = code,
              UserTypeId =teacherTypeId
          };
          // enregistrement du professeur
          int userId= await _repo.AddSelfRegister(teacher,TeacherRole.Name,true);
          if(userId>0)
          {
             foreach (var courseId in model.CourseIds)
             {
                 var ClassCourse = new ClassCourse{
                    TeacherId = userId,
                    CourseId = courseId
                 };

                 _repo.Add(ClassCourse);
             }
          }
          else
          return BadRequest("impossible d'ajouter cet utlisateur");
          
      }

      if(model.UserTypeId == parentTypeId)
      {
          var ParentRole   = await _context.Roles.FirstOrDefaultAsync(a=>a.Id==parentTypeId);
         string code = Guid.NewGuid().ToString();
          var parent = new User{
            ValidationCode = code,
            Email = model.Email,
            UserName = code,
            UserTypeId = parentTypeId
          };

        int parentId= await _repo.AddSelfRegister(parent,ParentRole.Name,true);

        if(parentId>0)
        {
          for (int i = 0; i < model.TotalChild; i++)
          {
              var MemberRole   = await _context.Roles.FirstOrDefaultAsync(a=>a.Id==memberRoleId);
                code = Guid.NewGuid().ToString();
              var child = new User{
                  ValidationCode = code,
                  UserName = code,
                  UserTypeId = studentTypeId
                };
              int childId =await _repo.AddSelfRegister(child,MemberRole.Name,false);
              if(childId>0)
              {
                var userLink = new UserLink{
                  UserPId = parentId,
                  UserId = childId
                };
                _repo.Add(userLink);
              }
          }
        
        }

        else
        return BadRequest("impossible d'ajouter ce compte");
      }

      if(await _repo.SaveAll())
      return NoContent();

      return BadRequest();
    }

    [HttpPost("SendEmail")]
    public  async Task<IActionResult> SendEmail(EmailFormDto model)
    {
        if(await _repo.SendEmail(model))
        return Ok("done");

        return BadRequest("impossible d'envoyer le mail");
    }

    [HttpPost("SendBatchEmail")]
    public async Task<IActionResult> SendBatchEmail(DataForEmail dataForEmail)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        Email newEmail = new Email();
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

        _context.Add(newEmail);

        if(await _repo.SaveAll())
        {
          return NoContent();
        }
        else
        {
          return BadRequest("problème pour envoyer l\' email");
        }
    }

    [HttpPost("Broadcast")]
    public async Task<IActionResult> Broadcast(DataForBroadcastDto dataForEmailDto)
    {
      List<int> userTypeIds = dataForEmailDto.UserTypeIds;
      List<int> classLevelIds = dataForEmailDto.ClassLevelIds;
      List<int> classIds = dataForEmailDto.ClassIds;
      string body = dataForEmailDto.Body;
      string subject = dataForEmailDto.Subject;
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      List<string> recipientEmails = new List<string>();

      var users = new List<User>();
      foreach (var ut in userTypeIds)
      {
        if(ut == studentTypeId || ut == parentTypeId)
        {
          if(users.Count() == 0)
          {
            users = await _context.Users
                            .Where(u => classIds.Contains(Convert.ToInt32(u.ClassId)) && u.Active == 1 &&
                              u.UserTypeId == studentTypeId && u.EmailConfirmed == true)
                            .ToListAsync();
          }

          if(ut == studentTypeId)
          {
            foreach (var user in users)
            {
              if(!string.IsNullOrEmpty(user.Email))
                recipientEmails.Add(user.Email);
            }
          }

          if(ut == parentTypeId)
          {
            var ids = users.Select(u => u.Id);
            var parents = _context.UserLinks
                          .Where(u => ids.Contains(u.UserId)).Select(u => u.UserP).Distinct();

            foreach (var user in parents)
            {
              if(!string.IsNullOrEmpty(user.Email))
                recipientEmails.Add(user.Email);
            }
          }
        }

        if(ut == teacherTypeId)
        {
          var teachers = await _context.ClassCourses
                          .Where(t => classLevelIds.Contains(t.Class.ClassLevelId) && t.Teacher.Active == 1 &&
                              t.Teacher.UserTypeId == teacherTypeId && t.Teacher.EmailConfirmed == true)
                          .Select(t => t.Teacher)
                          .Distinct()
                          .ToListAsync();

          foreach (var user in teachers)
          {
            if(!string.IsNullOrEmpty(user.Email))
              recipientEmails.Add(user.Email);
          }
        }
      }

      List<Email> emailsToBeSent = new List<Email>();
      //save emails to Emails table
      foreach (var email in recipientEmails)
      {
          Email newEmail = new Email();
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
          emailsToBeSent.Add(newEmail);
      }
      _context.AddRange(emailsToBeSent);

      if(await _repo.SaveAll())
      {
        return NoContent();
      }
      else
      {
        return BadRequest("problème pour envoyer les emails");
      }
    }

    [HttpGet("UsersRecap")]
    public async Task<IActionResult> UsersRecap()
    {
        var dataToReturn = new List<UsersRecapDto>();
        var userTypes= await _context.UserTypes.Include(u => u.Users).ToListAsync();
        foreach (var item in userTypes)
        {
            dataToReturn.Add( new UsersRecapDto {
                UserTypeId = item.Id,
                UserTypeName = item.Name,
                TotalAccount = item.Users.Count(),
                TotalActive = item.Users.Where(u => u.EmailConfirmed == true).Count()
            });
        }
        return Ok(dataToReturn);
    }

    [HttpGet("SearchInscription")]
    public async Task<IActionResult> SearchInscription([FromQuery]InscriptionSearchParams parametre)
    {
          IEnumerable<Inscription> inscr = new List<Inscription>();
          if(parametre.LastName == null)
            parametre.LastName ="";
          if(parametre.FirstName == null)
            parametre.FirstName ="";
            int levelId = Convert.ToInt32(parametre.LevelId);
          
          if(levelId>0)
          {
          // var studentIds =await _context.Inscriptions
          // .Where(c=>c.ClassLevelId == parametre.LevelId && c.Validated == false)
          // .Select(u=>u.StudentId)
          // .ToListAsync();
          inscr =await _context.Inscriptions
                  .Include(u=>u.User)
                  .ThenInclude(p=>p.Photos)
                  .Where(u=>u.ClassLevelId==levelId && u.Validated ==false
                                        && EF.Functions.Like(u.User.LastName, "%"+parametre.LastName+"%")
                                          && EF.Functions.Like(u.User.FirstName, "%"+parametre.FirstName+"%"))
                  .ToListAsync();
          }
          else 
          {
            inscr =await _context.Inscriptions
                  .Include(u=>u.User)
                  .ThenInclude(p=>p.Photos)
                  .Where(u=>u.Validated==false && EF.Functions.Like(u.User.LastName, "%"+parametre.LastName+"%") 
                        && EF.Functions.Like(u.User.FirstName, "%"+parametre.FirstName+"%"))
                  .ToListAsync();
          }

          var res = new List<InscriptionDetailDto>();
          foreach (var item in inscr)
          {
              res.Add(new InscriptionDetailDto {
                Id = item.Id,
                InsertDate = item.InsertDate,
                ClassLevelId = item.ClassLevelId,
                UserId = item.UserId,
                User = _mapper.Map<UserForDetailedDto>(item.User)

              });
          }

            return Ok(res);    
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
                  var code  = Guid.NewGuid();
                  userToCreate.UserName = code.ToString();
                  int userId= await _repo.AddUserPreInscription(code,userToCreate,teacherRoleId,true);
                  

                  foreach (var course in userForRegister.Courses)
                  {
                    var cours = await _context.Courses?.FirstOrDefaultAsync(c => c.Name.ToLower() == course.ToLower());
                    if(cours != null)
                    _repo.Add( new TeacherCourse{ TeacherId = userId, CourseId = cours.Id });
                  }
            }

            if( await _repo.SaveAll())
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

  }

}