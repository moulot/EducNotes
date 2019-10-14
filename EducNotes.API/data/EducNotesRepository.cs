using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EducNotes.API.Dtos;
using EducNotes.API.Helpers;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace EducNotes.API.Data
{
    public class EducNotesRepository : IEducNotesRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;
        string password;
         int teacherTypeId,parentTypeId,studentTypeId,adminTypeId;


        public EducNotesRepository(DataContext context, IConfiguration config, IEmailSender emailSender,
        UserManager<User> userManager)
        {
            _context = context;
            _config = config;
            _emailSender = emailSender;
            _config = config;
            password = _config.GetValue<String>("AppSettings:defaultPassword");
            _userManager =userManager;
            teacherTypeId =  _config.GetValue<int>("AppSettings:teacherTypeId");
            parentTypeId =  _config.GetValue<int>("AppSettings:parentTypeId");
            adminTypeId =  _config.GetValue<int>("AppSettings:adminTypeId");
            studentTypeId =  _config.GetValue<int>("AppSettings:studentTypeId");
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public async void AddAsync<T>(T entity) where T : class
        {
            await _context.AddAsync(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public void DeleteAll<T>(List<T> entities) where T : class
        {
            _context.RemoveRange(entities);
        }

        public async Task<bool> SaveAll()
        {
             return await _context.SaveChangesAsync() > 0;
        }
    
        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<User> GetUser(int id, bool isCurrentUser)
        {
            var query = _context.Users
                        //.Include(i => i.Father)
                        .Include(p => p.Photos).AsQueryable();

            if (isCurrentUser)
                query = query.IgnoreQueryFilters();

            var user = await query.FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<IEnumerable<User>> GetChildren(int parentId)
        {
          var userIds = _context.UserLinks.Where(u => u.UserPId == parentId).Select(s => s.UserId);

          return await _context.Users
                          .Include(i => i.Photos)
                          .Include(i => i.Class)
                          .Where(u => userIds.Contains(u.Id)).ToListAsync();
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.Include(p => p.Photos)
                .OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userParams.userId);

            users = users.Where(u => u.Gender == userParams.Gender);

            if(userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.userId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.userId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if(userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(- userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(- userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId
                && u.LikeeId == recipientId);
        }
        public async Task<User> GetSingleUser(string userName)
        {
            return await _context.Users.FirstAsync(u => u.UserName == userName);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users
                .Include(x => x.Likers)
                .Include(x => x.Likees)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<Class> GetClass(int Id)
        {
            return await _context.Classes.FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task<IEnumerable<Schedule>> GetScheduleDay(int classId, int day)
        {
            return await _context.Schedules
                .Include(i => i.Class)
                .Include(c => c.Course)
                .Where(d => d.Day == day && d.Class.Id == classId)
                .OrderBy(s => s.StartHourMin).ToListAsync();
        }

        public async Task<Agenda> GetAgenda(int agendaId)
        {
            return await _context.Agendas.FirstOrDefaultAsync(a => a.Id == agendaId);
        }
        public async Task<User> GetUserByCode(string code)
        {
        return await  _context.Users.FirstOrDefaultAsync(u=>u.ValidationCode==code);
        }

        public  List<int> GetWeekDays(DateTime date)
        {
            var dayDate = (int)date.DayOfWeek;
            var dayInt = dayDate == 0 ? 7 : dayDate;
            DateTime monday = date.AddDays(1 - dayInt);

            var days = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                days.Add(monday.AddDays(i).Day);
            }

            return days;
        }

        public async Task<IEnumerable<Schedule>> GetClassSchedule(int classId)
        {
            return await _context.Schedules
                    .Include(i => i.Class)
                    .Include(i => i.Course)
                    .Include(i => i.Teacher)
                    .Where(s => s.ClassId == classId)
                    .OrderBy(o => o.Day).ThenBy(o => o.StartHourMin).ToListAsync();
        }

        public async Task<IEnumerable<ClassLevelSchedule>> GetClassLevelSchedule(int classLevelId)
        {
            return await _context.ClassLevelSchedules
                    .Include(i => i.ClassLevel)
                    .Include(i => i.Course)
                    .Where(s => s.ClassLevelId == classLevelId)
                    .OrderBy(o => o.Day).ThenBy(o => o.StartHourMin).ToListAsync();
        }

        public async Task<IEnumerable<Agenda>> GetClassAgenda(int classId, DateTime StartDate, DateTime EndDate)
        {
            return await _context.Agendas
                    .Include(i => i.Course)
                    .Where(a => a.ClassId == classId && a.DueDate.Date >= StartDate.Date
                            && a.DueDate.Date <= EndDate.Date)
                    .OrderBy(o => o.DueDate).ToListAsync();
        }

        public async Task<IEnumerable<Agenda>> GetClassAgendaTodayToNDays(int classId, int toNbDays)
        {
            DateTime today = DateTime.Now.Date;
            DateTime EndDate = today.AddDays(toNbDays).Date;

          return await _context.Agendas
                  .Include(i => i.Course)
                  .Where(a => a.ClassId == classId && a.DueDate.Date >= today && a.DueDate.Date <= EndDate)
                  .OrderBy(o => o.DueDate).ToListAsync();
        }

       public async Task<IEnumerable<User>> GetClassStudents(int classId)
        {
            return await _context.Users
                .Include(i => i.Photos)
                .Include(i => i.Class)
                .Where(u => u.ClassId == classId)
                .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseSkill>> GetCourseSkills(int courseId)
        {
            return await _context.CourseSkills
                            .Include(i => i.Skill)
                            .Include(i => i.Course)
                            .Where(s => s.CourseId == courseId)
                            .OrderBy(o => o.Course.Name).ToListAsync();
        }
        public async Task<User> GetUserByEmail(string email)
        {
          return await _context.Users.FirstOrDefaultAsync(u=>u.Email.ToUpper() == email.ToUpper());
        }

        // public async Task<IEnumerable<IGrouping<DateTime, Agenda>>> GetClassAgenda(int classId)
        // {
        //     return await _context.Agendas
        //             .Include(i => i.Class)
        //             .Include(i => i.Course)
        //             .Where(a => a.ClassId == classId)
        //             .OrderBy(o => o.DueDate)
        //             .GroupBy(g => g.DueDate).ToListAsync();
        // }

        public async Task<IEnumerable<Agenda>> GetClassAgenda(int classId)
        {
            return await _context.Agendas
                    .Include(i => i.Class)
                    .Include(i => i.Course)
                    .Where(a => a.ClassId == classId)
                    .OrderBy(o => o.DueDate).ToListAsync();
        }
        public async Task<IEnumerable<User>> GetStudentsForClass(int classId)
        {
            return await _context.Users
                .Include(i => i.Photos)
                .Include(i => i.Class)
                .Where(u => u.ClassId == classId)
                .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserType>> getUserTypes()
        {
            return await _context.UserTypes.Where(u => u.Name != "Admin").ToListAsync();
        }

       

        public async Task<bool> EmailExist(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Email == email);
            if (user != null)
                return true;
            return
            false;
        }

        // public async Task<List<coursClass>> GetTeacherCoursesAndClasses(int teacherId)
        // {
        //     var cousrsesusers = await _context.CourseUsers.Include(c => c.Course).Where(a => a.TeacherId == teacherId).Select(e => e.Course).ToListAsync();
        //     var classcourses = new List<coursClass>();
        //     foreach (var cours in cousrsesusers)
        //     {
        //         classcourses.Add(new coursClass
        //         {
        //             Course = cours,
        //             classes = _context.ClassCourses.Include(c => c.Class).Where(c => c.CourseId == cours.Id).ToList()

        //         });
        //     }
        //     return classcourses;
        // }

        public async Task<int> AddUserPreInscription(Guid validationCode, User user, int roleId,bool sendEMail)
        {
            user.ValidationCode = validationCode.ToString();
            user.ValidatedCode = false;
            user.EmailConfirmed = false;
            if (user.Email != null)
                user.UserName = validationCode.ToString();
            else
                user.UserName = user.LastName;
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                        var role = await _context.Roles.FirstOrDefaultAsync(a=>a.Id==roleId);
                var appUser = await _userManager.Users
                 .FirstOrDefaultAsync(u => u.NormalizedUserName == user.UserName);

                _userManager.AddToRoleAsync(appUser, role.Name).Wait();

                if(sendEMail && user.Email!=null)
                {
                   var callbackUrl = _config.GetValue<String>("AppSettings:DefaultEmailValidationLink")+user.ValidationCode;
                      await _emailSender.SendEmailAsync(user.Email, "Confirmation de compte",
                      $"veuillez confirmez votre code au lien suivant : <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicker ici</a>.");
                }
                
                return appUser.Id;
            }
            return 0;

        }
        public async Task<Course> GetCourse(int Id)
        {
             return await _context.Courses.FirstOrDefaultAsync(c => c.Id == Id);
        }
        public async Task<bool> SendResetPasswordLink(string email,string code)
    {
          var emailform = new EmailFormDto
                    {
                        toEmail =email,
                        subject = "Réinitialisation de mot passe ",
                        //content ="Votre code de validation: "+ "<b>"+code.ToString()+"</b>"
                        content = ResetPasswordContent(code)
                    };
          try
          {
         var res = await SendEmail(emailform);
              return true;
          }
          catch (System.Exception )
          {
              return false;
          }


    }

         public async Task<bool> SendEmail(EmailFormDto emailFormDto)
        {
              try
              {
                  await _emailSender.SendEmailAsync(emailFormDto.toEmail, emailFormDto.subject,emailFormDto.content);
                  return true;
              }
              catch (System.Exception)
              {            
                  return false;
              }
        }

      private string ResetPasswordContent(string code)
        {
          return "<b>EducNotes</b> a bien enrgistré votre demande de réinitialisation de mot de passe !<br>"+
                  "Vous pouvez utiliser le lien suivant pour réinitialiser votre mot de passe: <br>"+
                  " <a href="+_config.GetValue<String>("AppSettings:DefaultResetPasswordLink")+code+"/>cliquer ici</a><br>"+
                  "Si vous n'utilisez pas ce lien dans les 3 heures, il expirera."+ 
                  "Pour obtenir un nouveau lien de réinitialisation de mot de passe, visitez"+
                  " <a href="+_config.GetValue<String>("AppSettings:DefaultforgotPasswordLink")+"/>réinitialiser son mot de passe</a>.<br>"+
                  "Merci,";
                    
        }
        public bool SendSms(List<string> phoneNumbers,string content)
        {
            try
            {
                foreach (var phonrNumber in phoneNumbers)
                {
                              //envoi sms clickatell :  using restSharp
                var curl = "https://platform.clickatell.com/messages/http/"+
                "send?apiKey=7z94hfu_RnWsCNW-XgDOxw==&to="+phonrNumber+"&content="+content; 
                var client = new RestClient(curl);
                var request = new RestRequest(Method.GET);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("header1", "headerval");
                request.AddParameter("application/x-www-form-urlencoded", "bodykey=bodyval", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                }
                return  true;
            }
            catch (System.Exception)
            {
                
                return false;
            }
        }

    public async Task<IEnumerable<City>> GetAllCities()
    {
     return (await _context.Cities.OrderBy(c=>c.Name).ToListAsync());
    }

    public async Task<IEnumerable<District>> GetAllGetDistrictsByCityIdCities(int id)
    {
     return (await _context.Districts.Where(c=>c.CityId == id).OrderBy(c=>c.Name).ToListAsync());
    }

    public void AddInscription(int levelId, int userId)
    {
      var nouvelle_incrpition = new Inscription {
                              InsertDate = DateTime.Now,
                              ClassLevelId = levelId,
                              UserId = userId,
                              Validated = false
                            };
     Add(nouvelle_incrpition);
    }

    public void AddUserLink(int userId, int parentId)
    {
      var nouveau_link = new UserLink
                             {
                              UserId = userId,
                              UserPId  = parentId
                              };
      Add(nouveau_link);
    }
    private string EditValidationContent(string userName,string code)
    {
      return "<h3><span>Educt'Notes</span></h3> <br>"+
            "bonjour <b>"+userName+",</b>"+
            "<p>Merci de bien vouloir valider votre compte au lien suivant :"+
            " <a href="+_config.GetValue<String>("AppSettings:DefaultEmailValidationLink")+code+" /> cliquer ici</a></p> <br>";
            
    }

  /////////////////////////////////////////////////////////////////////////////////////////////////////
  /////////////////////////////// DATA FROM MOHAMED KABORE ////////////////////////////////////////////
  /////////////////////////////////////////////////////////////////////////////////////////////////////
    public async Task<IEnumerable<ClassType>> GetClassTypes()
    {
      return await _context.ClassTypes.OrderBy(a=>a.Name).ToListAsync();
    }

    public async Task<int> AddSelfRegister(User user,string roleName,bool sendLink)
    {
      
       user.Created = DateTime.Now;

      var result = await _userManager.CreateAsync(user, password);

      if(result.Succeeded)
      {
        var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == user.UserName);

        _userManager.AddToRoleAsync(appUser, roleName).Wait();
        if(sendLink)
        {
          // envoi du lien
           var callbackUrl = _config.GetValue<String>("AppSettings:DefaultSelRegisterLink")+user.ValidationCode;
           // envoi du mail
           await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        }
        return appUser.Id;
      }

      return 0;
    }
    
    public async Task<List<string>> GetEmails()
    {
      return await  _context.Users.Where(a => a.Email != null).Select(a=>a.Email).ToListAsync();
    }

     public async Task<List<string>> GetUserNames()
    {
      return await  _context.Users.Where(a => a.UserName != null).Select(a=>a.UserName).ToListAsync();
    }

    public async Task<List<ClassLevel>> GetLevels()
    {
      return await _context.ClassLevels.OrderBy(c => c.DsplSeq).ToListAsync();
    }

    public async Task<bool> UserNameExist(string userName)
    {
       var user = await _context.Users.FirstOrDefaultAsync(e => e.UserName == userName.ToLower());
            if (user != null)
                return true;
            return
            false;
    }

        public async Task sendOk(int userTypeId, int userId)
        {
            if(userTypeId == studentTypeId)
            {
                // envoi de mail de l'affectation de l'eleve au professeur
                
                // recuperation des emails de ses parents
        
                var parents = await _context.UserLinks
                   .Include(c => c.UserP)
                   .Where(u => u.UserId == userId).Select(c => c.UserP)
                   .ToListAsync();

                // récupération de nom de la classe de l'eleve
                var student = await _context.Users.Include(c => c.Class) 
                               .FirstOrDefaultAsync( u => u.Id == userId);

                // formatage du mail
                foreach (var parent in parents)
                {
                    var emailform = new EmailFormDto{
                        toEmail = parent.Email,
                    subject = "Confirmation mise en salle de votre enfant ",
                    content = "<b> Bonjour "+ parent.LastName + " " + parent.FirstName +"</b>, <br>" +
                              "Votre enfant <b>" + student.FirstName + " " + student.FirstName +
                               " </b> a bien été enregistré(s) dans la classe de <b>" + student.Class.Name
                };
                  await SendEmail(emailform);
                }


            }
        }

        public async Task<List<UserSpaCodeDto>> ParentSelfInscription(int parentId,List<UserForUpdateDto> userToUpdate)
        {
           var usersSpaCode = new List<UserSpaCodeDto>();
           using (var identityContextTransaction = _context.Database.BeginTransaction())
           {
               try
               {
                   var c = await GetChildren(parentId);
                   var children = c.ToList();
                   int cpt =0;
                   foreach (var user in userToUpdate)
                   {

                     if(user.UserTypeId == parentTypeId)
                     {
                        var parentFromRepo =await GetUser(parentId,true);
                        parentFromRepo.UserName = user.UserName.ToLower();
                        parentFromRepo.LastName = user.LastName;
                        parentFromRepo.FirstName = user.FirstName;
                        if(user.DateOfBirth!=null)
                        parentFromRepo.DateOfBirth = user.DateOfBirth;
                        parentFromRepo.CityId = user.CityId;
                        parentFromRepo.DistrictId = user.DistrictId;
                        parentFromRepo.PhoneNumber = user.PhoneNumber;
                        parentFromRepo.SecondPhoneNumber = user.SecondPhoneNumber;
                        // configuration du nouveau mot de passe
                        var newPassword=_userManager.PasswordHasher.HashPassword(parentFromRepo,user.Password);
                        parentFromRepo.PasswordHash = newPassword;
                        parentFromRepo.ValidatedCode = true;
                        parentFromRepo.EmailConfirmed =true;
                        parentFromRepo.ValidationDate = DateTime.Now;
                        var res = await _userManager.UpdateAsync(parentFromRepo);
                        
                         if(res.Succeeded)
                         {
                             // ajout dans la table Email
                             var email = new Email();
                             email.InsertDate = DateTime.Now;
                             email.InsertUserId = parentId;
                             email.UpdateUserId = parentId;
                             email.StatusFlag =0;
                             email.Subject = "Compte confirmé";
                             email.ToAddress = parentFromRepo.Email;
                             email.Body = "<b> "+parentFromRepo.LastName + " "+ parentFromRepo.FirstName + "</b>, votre compte a bien été enregistré";
                             email.FromAddress ="no-reply@educnotes.com";
                             email.EmailTypeId =  _config.GetValue<int>("AppSettings:confirmedtypeId");
                             Add(email);
                             // retour du code du userId et du codeSpa
                             usersSpaCode.Add(new UserSpaCodeDto {UserId = parentFromRepo.Id,SpaCode= Convert.ToInt32(user.SpaCode)});
                         }
                        
                     } 
                     if(user.UserTypeId == studentTypeId)
                     {
                        var child = children[cpt];
                        int classLevelId = Convert.ToInt32(user.LevelId);
                        child.UserName = user.UserName.ToLower();
                        child.LastName = user.LastName;
                        child.FirstName = user.FirstName;
                        if(child.DateOfBirth != null)
                        child.DateOfBirth = Convert.ToDateTime(user.DateOfBirth);
                        child.CityId = user.CityId;
                        child.DistrictId = user.DistrictId;
                        child.PhoneNumber = user.PhoneNumber;
                        child.SecondPhoneNumber = user.SecondPhoneNumber;
                            // configuration du mot de passe
                        var newPass=_userManager.PasswordHasher.HashPassword(child,user.Password);
                        child.PasswordHash = newPass;
                        child.ValidatedCode = true;
                        child.EmailConfirmed =true;
                        child.ValidationDate = DateTime.Now;
                        child.TempData =1;
                        var res = await _userManager.UpdateAsync(child);
                       
                        if(res.Succeeded)
                        {
                           //enregistrement de l inscription
                            var insc = new Inscription {
                            InsertDate = DateTime.Now,
                            ClassLevelId = classLevelId,
                            UserId = child.Id,
                            InsertUserId = parentId,
                            InscriptionTypeId =  _config.GetValue<int>("AppSettings:parentInscTypeId"),

                            Validated = false
                        };
                            Add(insc);
                        usersSpaCode.Add( new UserSpaCodeDto{UserId = child.Id, SpaCode = Convert.ToInt32(user.SpaCode)});
                        cpt=cpt+1;
                        }
                        
                     }   
                   }

                   if(await SaveAll())
                     identityContextTransaction.Commit();
               }
               catch (System.Exception)
               {
                   identityContextTransaction.Rollback();
                   usersSpaCode=new List<UserSpaCodeDto>();
               }
           }
           return usersSpaCode;
           
        }
    }
}