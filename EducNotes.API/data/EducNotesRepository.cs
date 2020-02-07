using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.Dtos;
using EducNotes.API.Helpers;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace EducNotes.API.Data
{
    public class EducNotesRepository : IEducNotesRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        string password;
        int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
        int parentRoleId, memberRoleId, moderatorRoleId, adminRoleId, teacherRoleId, schoolInscTypeId;

        public EducNotesRepository(DataContext context, IConfiguration config, IEmailSender emailSender,
            UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _config = config;
            _emailSender = emailSender;
            _mapper = mapper;
            _config = config;
            password = _config.GetValue<String>("AppSettings:defaultPassword");
            _userManager = userManager;
            teacherTypeId = _config.GetValue<int>("AppSettings:teacherTypeId");
            parentTypeId = _config.GetValue<int>("AppSettings:parentTypeId");
            adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
            studentTypeId = _config.GetValue<int>("AppSettings:studentTypeId");
            parentRoleId = _config.GetValue<int>("AppSettings:parentRoleId");
            memberRoleId = _config.GetValue<int>("AppSettings:memberRoleId");
            moderatorRoleId = _config.GetValue<int>("AppSettings:moderatorRoleId");
            adminRoleId = _config.GetValue<int>("AppSettings:adminRoleId");
            teacherRoleId = _config.GetValue<int>("AppSettings:teacherRoleId");
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
                .Include(c => c.Class)
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
                .Include(i => i.UserType)
                .Where(u => userIds.Contains(u.Id) && u.ValidatedCode == true).ToListAsync();
        }

        public async Task<User> GetParent(int ChildId)
        {
            var parent = await _context.UserLinks.FirstOrDefaultAsync(u => u.UserId == ChildId);

            return await _context.Users
                            .Include(i => i.Photos)
                            .FirstOrDefaultAsync(p => p.Id == parent.UserPId);
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos)
                .OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userParams.userId);

            users = users.Where(u => u.Gender == userParams.Gender);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.userId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.userId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
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
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId &&
               u.LikeeId == recipientId);
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
            return await _context.Users.FirstOrDefaultAsync(u => u.ValidationCode == code);
        }

        public async Task<SmsTemplate> GetSmsTemplate(int smsTemplateId)
        {
            return await _context.SmsTemplates.FirstOrDefaultAsync(s => s.Id == smsTemplateId);
        }

        public List<int> GetWeekDays(DateTime date)
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
                        .Where(a => a.ClassId == classId && a.DueDate.Date >= StartDate.Date && a.DueDate.Date <= EndDate.Date)
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
                .Include (i => i.Photos)
                //.Include (i => i.Class)
                .Where (u => u.ClassId == classId)
                .OrderBy (e => e.LastName).ThenBy (e => e.FirstName)
                .ToListAsync ();
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
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == email.ToUpper());
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

        public async Task<List<Course>> GetTeacherCourses(int teacherId)
        {
            var courses = await _context.TeacherCourses
                                    .Where(c => c.TeacherId == teacherId)
                                    .Select(s => s.Course).ToListAsync();

            return courses;
        }

        public async Task<List<TeacherClassesDto>> GetTeacherClasses(int teacherId)
        {
            var classesData = await (from courses in _context.ClassCourses
                                    join classes in _context.Classes
                                    on courses.ClassId equals classes.Id
                                    where courses.TeacherId == teacherId
                                    select new {
                                        ClassId = classes.Id,
                                        ClassLevelId = classes.ClassLevelId,
                                        ClassName = classes.Name,
                                        NbStudents = _context.Users.Where(u => u.ClassId == classes.Id).Count()
                                    })
                                    .OrderBy(o => o.ClassName)
                                    .Distinct().ToListAsync();

            List<TeacherClassesDto> teacherClasses = new List<TeacherClassesDto>();
            foreach (var aclass in classesData)
            {
                TeacherClassesDto tcd = new TeacherClassesDto();
                tcd.ClassId = aclass.ClassId;
                tcd.ClassLevelId = aclass.ClassLevelId;
                tcd.ClassName = aclass.ClassName;
                tcd.NbStudents = aclass.NbStudents;
                teacherClasses.Add(tcd);
            }

            return teacherClasses;
        }

        public async Task<List<ClassesWithEvalsDto>> GetTeacherClassesWithEvalsByPeriod(int teacherId, int periodId)
        {
            var Classes = await _context.ClassCourses
                                    .Include(i => i.Class).ThenInclude(i => i.Students)
                                    .Where(c => c.TeacherId == teacherId).Distinct().ToListAsync();

            List<ClassesWithEvalsDto> classesWithEvals = new List<ClassesWithEvalsDto>();
            foreach (var aclass in Classes)
            {
              List<Evaluation> ClassEvals = await _context.Evaluations
                                .Include(i => i.Course)
                                .Include(i => i.EvalType)
                                .Where(e => e.ClassId == aclass.ClassId && e.PeriodId == periodId).ToListAsync();

              if (ClassEvals.Count > 0)
              {
                    var OpenedEvals = ClassEvals.FindAll(e => e.Closed == false);
                    var OpenedEvalsDto = _mapper.Map<List<EvaluationForListDto>>(OpenedEvals);
                    var ToBeGradedEvals = ClassEvals.FindAll(e => e.Closed == true);
                    var ToBeGradedEvalsDto = _mapper.Map<List<EvaluationForListDto>>(ToBeGradedEvals);
                    var NbEvals = OpenedEvals.Count() + ToBeGradedEvals.Count();

                    ClassesWithEvalsDto classDto = new ClassesWithEvalsDto();
                    classDto.ClassId = Convert.ToInt32(aclass.ClassId);
                    classDto.ClassName = aclass.Class.Name;
                    classDto.NbStudents = aclass.Class.Students.Count();
                    classDto.NbEvals = NbEvals;
                    classDto.OpenedEvals = OpenedEvalsDto;
                    classDto.ToBeGradedEvals = ToBeGradedEvalsDto;

                    classesWithEvals.Add(classDto);
              }
            }

            return classesWithEvals;
        }

        public async Task<List<EvaluationForListDto>> GetEvalsToCome(int classId)
        {
            var today = DateTime.Now.Date;
            var evals = await _context.Evaluations
                        .Include(i => i.Course)
                        .Include(i => i.EvalType)
                        .Where(e => e.ClassId == classId && e.EvalDate.Date >= today).ToListAsync();

            var evalsToReturn = _mapper.Map<List<EvaluationForListDto>>(evals);

            return evalsToReturn;
        }


        public async Task<bool> AddUserPreInscription(UserForRegisterDto userForRegister, int insertUserId)
        {

            var userToCreate = _mapper.Map<User>(userForRegister);
            var code = Guid.NewGuid();
            userToCreate.UserName = code.ToString();
            userToCreate.ValidationCode = code.ToString();
            userToCreate.ValidatedCode = false;
            userToCreate.EmailConfirmed = false;
            userToCreate.UserName = code.ToString();
            bool resultStatus = false;

            using (var identityContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (userToCreate.UserTypeId == teacherTypeId)
                    {
                        //enregistrement du teacher
                        var result = await _userManager.CreateAsync(userToCreate, password);
                        if (result.Succeeded)
                        {
                            // enregistrement du RoleTeacher
                            var role = await _context.Roles.FirstOrDefaultAsync(a => a.Id == teacherRoleId);
                            var appUser = await _userManager.Users
                                .FirstOrDefaultAsync(u => u.NormalizedUserName == userToCreate.UserName);
                            _userManager.AddToRoleAsync(appUser, role.Name).Wait();

                            //enregistrement de des cours du professeur
                            if (userForRegister.CourseIds != null)
                            {
                                foreach (var course in userForRegister.CourseIds)
                                {
                                    Add(new TeacherCourse { CourseId = course, TeacherId = userToCreate.Id });
                                }
                            }

                            // Enregistrement dans la table Email
                            if (userToCreate.Email != null)
                            {
                                var callbackUrl = _config.GetValue<String>("AppSettings:DefaultEmailValidationLink") + userToCreate.ValidationCode;

                                var emailToSend = new Email
                                {
                                    InsertUserId = insertUserId,
                                    UpdateUserId = userToCreate.Id,
                                    StatusFlag = 0,
                                    Subject = "Confirmation de compte",
                                    ToAddress = userToCreate.Email,
                                    Body = $"veuillez confirmez votre code au lien suivant : <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicker ici</a>.",
                                    FromAddress = "no-reply@educnotes.com",
                                    EmailTypeId = _config.GetValue<int>("AppSettings:confirmationEmailtypeId")
                                };
                                Add(emailToSend);
                            }
                            if (await SaveAll())
                            {
                                // fin de la transaction
                                identityContextTransaction.Commit();
                                resultStatus = true;

                            }
                            else
                                resultStatus = true;
                        }
                        else
                            resultStatus = false;
                    }

                }
                catch (System.Exception)
                {

                    return resultStatus = false;
                }
            }
            return resultStatus;
        }
        public async Task<Course> GetCourse(int Id)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.Id == Id);
        }
        public async Task<bool> SendResetPasswordLink(User user, string code)
        {
            var emailform = new EmailFormDto
            {
               // toEmail = email,
                subject = "Réinitialisation de mot passe ",
                //content ="Votre code de validation: "+ "<b>"+code.ToString()+"</b>"
                content = ResetPasswordContent(code)
            };

                var callbackUrl = _config.GetValue<String>("AppSettings:DefaultResetPasswordLink") + code;
                            var email = new Email
                            {

                                InsertUserId = user.Id,
                                UpdateUserId =  user.Id,
                                StatusFlag = 0,
                                Subject = "Réinitialisation de mot passe ",
                                ToAddress = user.Email,
                                Body = $"veuillez cliquer sur le lien suivant pour réinitiliser votre mot de passe : <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicker ici</a>.",
                                FromAddress = "no-reply@educnotes.com",
                                EmailTypeId = _config.GetValue<int>("AppSettings:confirmationEmailtypeId")
                            };
                            Add(email);

            try
            {
                var res = await SendEmail(emailform);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }

        public async Task<bool> SendEmail(EmailFormDto emailFormDto)
        {
            try
            {
                await _emailSender.SendEmailAsync(emailFormDto.toEmail, emailFormDto.subject, emailFormDto.content);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private string ResetPasswordContent(string code)
        {
            return "<b>EducNotes</b> a bien enrgistré votre demande de réinitialisation de mot de passe !<br>" +
                "Vous pouvez utiliser le lien suivant pour réinitialiser votre mot de passe: <br>" +
                " <a href=" + _config.GetValue<String>("AppSettings:DefaultResetPasswordLink") + code + "/>cliquer ici</a><br>" +
                "Si vous n'utilisez pas ce lien dans les 3 heures, il expirera." +
                "Pour obtenir un nouveau lien de réinitialisation de mot de passe, visitez" +
                " <a href=" + _config.GetValue<String>("AppSettings:DefaultforgotPasswordLink") + "/>réinitialiser son mot de passe</a>.<br>" +
                "Merci,";

        }
        public bool SendSms(List<string> phoneNumbers, string content)
        {
            try
            {
                foreach (var phonrNumber in phoneNumbers)
                {
                    //envoi sms clickatell :  using restSharp
                    var curl = "https://platform.clickatell.com/messages/http/" +
                        "send?apiKey=7z94hfu_RnWsCNW-XgDOxw==&to=" + phonrNumber + "&content=" + content;
                    var client = new RestClient(curl);
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("header1", "headerval");
                    request.AddParameter("application/x-www-form-urlencoded", "bodykey=bodyval", ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }
                return true;
            }
            catch (System.Exception)
            {

                return false;
            }
        }

        public async Task<IEnumerable<City>> GetAllCities()
        {
            return (await _context.Cities.OrderBy(c => c.Name).ToListAsync());
        }

        public async Task<IEnumerable<District>> GetAllGetDistrictsByCityIdCities(int id)
        {
            return (await _context.Districts.Where(c => c.CityId == id).OrderBy(c => c.Name).ToListAsync());
        }

        public void AddInscription(int levelId, int userId)
        {
            var nouvelle_incrpition = new Inscription
            {
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
                UserPId = parentId
            };
            Add(nouveau_link);
        }

        private string EditValidationContent(string userName, string code)
        {
            return "<h3><span>Educt'Notes</span></h3> <br>" +
                "bonjour <b>" + userName + ",</b>" +
                "<p>Merci de bien vouloir valider votre compte au lien suivant :" +
                " <a href=" + _config.GetValue<String>("AppSettings:DefaultEmailValidationLink") + code + " /> cliquer ici</a></p> <br>";

        }

        public IEnumerable<ClassAgendaToReturnDto> GetAgendaListByDueDate(IEnumerable<Agenda> agendaItems)
        {
            //selection de toutes les differentes dates
            var dueDates = agendaItems.OrderBy(o => o.DueDate).Select(e => e.DueDate).Distinct();

            var agendasToReturn = new List<ClassAgendaToReturnDto>();
            foreach(var currDate in dueDates) {
                var currentDateAgendas = agendaItems.Where(e => e.DueDate == currDate);
                var agenda = new ClassAgendaToReturnDto();
                agenda.dtDueDate = currDate;
                agenda.DueDate = currDate.ToLongDateString();
                agenda.DueDateDay = currDate.Day;
                agenda.NbTasks = currentDateAgendas.Count();
                var dayInt = (int)currDate.DayOfWeek;
                agenda.DayInt = dayInt == 0 ? 7 : dayInt;
                agenda.Courses = new List<CourseTask>();
                foreach (var a in currentDateAgendas) {
                    agenda.Courses.Add(new CourseTask {
                        CourseId = a.Course.Id,
                        CourseName = a.Course.Name,
                        CourseColor = a.Course.Color,
                        TaskDesc = a.TaskDesc,
                        DateAdded = a.DateAdded.ToLongDateString(),
                        ShortDateAdded = a.DateAdded.ToShortDateString()
                    });
                }

                agendasToReturn.Add(agenda);
            }

            return agendasToReturn;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////// DATA FROM MOHAMED KABORE ////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task<IEnumerable<ClassType>> GetClassTypes()
        {
            return await _context.ClassTypes.OrderBy(a => a.Name).ToListAsync();
        }

        public async Task<int> AddSelfRegister(User user, string roleName, bool sendLink, int currentUserId)
        {
            var userIdToReturn = 0;
            user.Created = DateTime.Now;
            using (var identityContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var result = await _userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == user.UserName);
                        userIdToReturn = appUser.Id;

                        _userManager.AddToRoleAsync(appUser, roleName).Wait();
                        if (sendLink)
                        {
                            // envoi du lien
                            var callbackUrl = _config.GetValue<String>("AppSettings:DefaultSelRegisterLink") + appUser.ValidationCode;
                            var email = new Email
                            {

                                InsertUserId = currentUserId,
                                UpdateUserId = appUser.Id,
                                StatusFlag = 0,
                                Subject = "Confirmation de compte",
                                ToAddress = appUser.Email,
                                Body = $"veuillez confirmez votre code au lien suivant : <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicker ici</a>.",
                                FromAddress = "no-reply@educnotes.com",
                                EmailTypeId = _config.GetValue<int>("AppSettings:confirmationEmailtypeId")
                            };
                            Add(email);
                            await SaveAll();
                        }
                    }

                    identityContextTransaction.Commit();
                }
                catch (System.Exception)
                {
                    identityContextTransaction.Rollback();
                    userIdToReturn = 0;
                }
            }

            return userIdToReturn;
        }

        public async Task<List<string>> GetEmails()
        {
            return await _context.Users.Where(a => a.Email != null).Select(a => a.Email).ToListAsync();
        }

        public async Task<List<string>> GetUserNames()
        {
            return await _context.Users.Where(a => a.UserName != null).Select(a => a.UserName).ToListAsync();
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
            if (userTypeId == studentTypeId)
            {
                // envoi de mail de l'affectation de l'eleve au professeur

                // recuperation des emails de ses parents

                var parents = await _context.UserLinks
                    .Include(c => c.UserP)
                    .Where(u => u.UserId == userId).Select(c => c.UserP)
                    .ToListAsync();

                // récupération de nom de la classe de l'eleve
                var student = await _context.Users.Include(c => c.Class)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                // formatage du mail
                foreach (var parent in parents)
                {
                    var emailform = new EmailFormDto
                    {
                        toEmail = parent.Email,
                        subject = "Confirmation mise en salle de votre enfant ",
                        content = "<b> Bonjour " + parent.LastName + " " + parent.FirstName + "</b>, <br>" +
                        "Votre enfant <b>" + student.LastName + " " + student.FirstName +
                        " </b> a bien été enregistré(s) dans la classe de <b>" + student.Class.Name
                    };
                    await SendEmail(emailform);
                }

            }
        }

        public async Task<List<UserSpaCodeDto>> ParentSelfInscription(int parentId, List<UserForUpdateDto> userToUpdate)
        {
            var usersSpaCode = new List<UserSpaCodeDto>();
            var children = await _context.UserLinks.Where(u =>u.UserPId == parentId).Select(u => u.User).ToListAsync();
            int cpt = 0;
            using (var identityContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var user in userToUpdate)
                    {

                        if (user.UserTypeId == parentTypeId)
                        {
                            var parentFromRepo = await GetUser(parentId, true);
                            parentFromRepo.UserName = user.UserName.ToLower();
                            parentFromRepo.LastName = user.LastName;
                            parentFromRepo.FirstName = user.FirstName;
                            parentFromRepo.Gender = Convert.ToByte(user.Gender);
                            if (user.DateOfBirth != null)
                                parentFromRepo.DateOfBirth = Convert.ToDateTime(user.DateOfBirth);
                            parentFromRepo.CityId = user.CityId;
                            parentFromRepo.DistrictId = user.DistrictId;
                            parentFromRepo.PhoneNumber = user.PhoneNumber;
                            parentFromRepo.SecondPhoneNumber = user.SecondPhoneNumber;
                            // configuration du nouveau mot de passe
                            var newPassword = _userManager.PasswordHasher.HashPassword(parentFromRepo, user.Password);
                            parentFromRepo.PasswordHash = newPassword;
                            parentFromRepo.ValidatedCode = true;
                            parentFromRepo.EmailConfirmed = true;
                            parentFromRepo.ValidationDate = DateTime.Now;
                            var res = await _userManager.UpdateAsync(parentFromRepo);

                            if (res.Succeeded)
                            {
                                // ajout dans la table Email
                                var email = new Email();
                                email.InsertDate = DateTime.Now;
                                email.InsertUserId = parentId;
                                email.UpdateUserId = parentId;
                                email.StatusFlag = 0;
                                email.Subject = "Compte confirmé";
                                email.ToAddress = parentFromRepo.Email;
                                email.Body = "<b> " + parentFromRepo.LastName + " " + parentFromRepo.FirstName + "</b>, votre compte a bien été enregistré";
                                email.FromAddress = "no-reply@educnotes.com";
                                email.EmailTypeId = _config.GetValue<int>("AppSettings:confirmedEmailtypeId");
                                Add(email);
                                // retour du code du userId et du codeSpa
                                usersSpaCode.Add(new UserSpaCodeDto { UserId = parentFromRepo.Id, SpaCode = Convert.ToInt32(user.SpaCode) });
                            }

                        }
                        if (user.UserTypeId == studentTypeId)
                        {
                            var child = children[cpt];
                            int classLevelId = Convert.ToInt32(user.LevelId);
                            child.UserName = user.UserName.ToLower();
                            child.LastName = user.LastName;
                            child.FirstName = user.FirstName;
                            child.Gender = Convert.ToByte(user.Gender);
                            if (child.DateOfBirth != null)
                                child.DateOfBirth = Convert.ToDateTime(user.DateOfBirth);
                            child.CityId = user.CityId;
                            child.DistrictId = user.DistrictId;
                            child.PhoneNumber = user.PhoneNumber;
                            child.SecondPhoneNumber = user.SecondPhoneNumber;
                            // configuration du mot de passe
                            var newPass = _userManager.PasswordHasher.HashPassword(child, user.Password);
                            child.PasswordHash = newPass;
                            child.ValidatedCode = true;
                            child.EmailConfirmed = false;
                            if (!string.IsNullOrEmpty(child.Email))
                                child.EmailConfirmed = true;
                            child.ValidationDate = DateTime.Now;
                            child.TempData = 1;
                            var res = await _userManager.UpdateAsync(child);

                            if (res.Succeeded)
                            {
                                //enregistrement de l inscription
                                var insc = new Inscription
                                {
                                    InsertDate = DateTime.Now,
                                    ClassLevelId = classLevelId,
                                    UserId = child.Id,
                                    InsertUserId = parentId,
                                    InscriptionTypeId = _config.GetValue<int>("AppSettings:parentInscTypeId"),

                                    Validated = false
                                };
                                Add(insc);
                                usersSpaCode.Add(new UserSpaCodeDto { UserId = child.Id, SpaCode = Convert.ToInt32(user.SpaCode) });
                                cpt = cpt + 1;
                            }

                        }
                    }

                    if (await SaveAll())
                        identityContextTransaction.Commit();
                }
                catch (System.Exception)
                {
                    identityContextTransaction.Rollback();
                    usersSpaCode = new List<UserSpaCodeDto>();
                }
            }
            return usersSpaCode;

        }

        public async Task<List<UserEvalsDto>> GetUserGrades(int userId, int classId)
        {
            //get user courses
            var userCourses = await (from course in _context.ClassCourses
                                     join user in _context.Users on course.ClassId equals user.ClassId
                                     where user.ClassId == classId
                                     orderby course.Course.Name
                                     select course.Course).Distinct().ToListAsync();

            var aclass = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);

            List<UserEvalsDto> coursesWithEvals = new List<UserEvalsDto>();

            var periods = await _context.Periods.OrderBy(p => p.StartDate).ToListAsync();

            //loop on user courses - get data for each course 
            for (int i = 0; i < userCourses.Count(); i++)
            {

                var acourse = userCourses[i];

                //get all evaluations of the selected course and current userId
                var userEvals = await _context.UserEvaluations
                                    .Include(e => e.Evaluation).ThenInclude(e => e.EvalType)
                                    .OrderBy(o => o.Evaluation.EvalDate)
                                    .Where(e => e.UserId == userId && e.Evaluation.GradeInLetter == false &&
                                       e.Evaluation.CourseId == acourse.Id && e.Evaluation.Graded == true)
                                    .Distinct().ToListAsync();

                // get general Evals data for the current user course
                UserEvalsDto userEvalsDto = GetUserCourseEvals(userEvals, acourse, aclass);

                // get evals by period for the current user course
                userEvalsDto.PeriodEvals = new List<PeriodEvalsDto>();
                foreach (var period in periods)
                {
                    PeriodEvalsDto ped = new PeriodEvalsDto();
                    ped.PeriodId = period.Id;
                    ped.PeriodName = period.Name;
                    ped.PeriodAbbrev = period.Abbrev;
                    ped.Active = period.Active;

                    var userPeriodEvals = userEvals.Where(e => e.Evaluation.PeriodId == period.Id).ToList();
                    if (userPeriodEvals.Count() > 0)
                    {
                        UserEvalsDto periodEvals = GetUserCourseEvals(userPeriodEvals, acourse, aclass);
                        ped.UserCourseAvg = periodEvals.UserCourseAvg;
                        ped.ClassCourseAvg = periodEvals.ClassCourseAvg;
                        ped.grades = periodEvals.grades;
                    }
                    else
                    {
                        ped.UserCourseAvg = -1000;
                    }

                    userEvalsDto.PeriodEvals.Add(ped);
                }

                coursesWithEvals.Add(userEvalsDto);
            }

            return coursesWithEvals;
        }

        public EvalSmsDto GetUSerSmsEvalData(int ChildId, List<UserEvaluation> ClassEvals)
        {
            EvalSmsDto smsData = new EvalSmsDto();

            return smsData;
        }

        public UserEvalsDto GetUserCourseEvals(List<UserEvaluation> UserEvals, Course acourse, Class aclass)
        {
            UserEvalsDto userEvalsDto = new UserEvalsDto();
            userEvalsDto.CourseId = acourse.Id;
            userEvalsDto.CourseName = acourse.Name;
            userEvalsDto.CourseAbbrev = acourse.Abbreviation;
            userEvalsDto.GradedOutOf = 20;

            double gradesSum = 0;
            double coeffSum = 0;

            // are evals evailable fro the cuurent course?
            if (UserEvals.Count() > 0)
            {
                List<GradeDto> grades = new List<GradeDto>();

                for (int j = 0; j < UserEvals.Count(); j++)
                {
                    // calculate each grade of the selected course
                    var elt = UserEvals[j];
                    if (elt.Grade.IsNumeric())
                    {
                        var evalDate = elt.Evaluation.EvalDate.ToShortDateString();
                        var evalType = elt.Evaluation.EvalType.Name;
                        var evalName = elt.Evaluation.Name;
                        double maxGrade = Convert.ToDouble(elt.Evaluation.MaxGrade);
                        double gradeValue = Convert.ToDouble(elt.Grade.Replace(".", ","));
                        // grade are ajusted to 20 as MAx. Avg is on 20
                        double ajustedGrade = Math.Round(20 * gradeValue / maxGrade, 2);
                        double coeff = elt.Evaluation.Coeff;
                        //data for course average
                        gradesSum += ajustedGrade * coeff;
                        coeffSum += coeff;

                        // get class grades for the current user grade (evaluation)
                        var EvalClassGrades = _context.UserEvaluations
                            .Where(e => e.EvaluationId == elt.EvaluationId &&
                               e.Evaluation.GradeInLetter == false && e.Evaluation.Graded == true && e.Grade.IsNumeric())
                            .Select(e => e.Grade).ToList();

                        double classMin = 999999;
                        double classMax = -999999;
                        //get class min and max of evaluation
                        foreach (var item in EvalClassGrades)
                        {
                            var grade = Convert.ToDouble(item.Replace(".", ","));
                            classMin = grade < classMin ? grade : classMin;
                            classMax = grade > classMax ? grade : classMax;
                        }
                        double EvalGradeMin = classMin;
                        double EvalGradeMax = classMax;

                        //enter grade data "as it is" in the user grades data list
                        GradeDto gradeDto = new GradeDto();
                        gradeDto.EvalType = evalType;
                        gradeDto.EvalDate = evalDate;
                        gradeDto.EvalName = evalName;
                        gradeDto.Grade = Math.Round(gradeValue, 2);
                        gradeDto.GradeMax = maxGrade;
                        gradeDto.Coeff = coeff;
                        gradeDto.ClassGradeMin = EvalGradeMin;
                        gradeDto.ClassGradeMax = EvalGradeMax;
                        grades.Add(gradeDto);
                    }
                }

                //calculate user grade avg for the selected course
                double gradesAvg = Math.Round(gradesSum / coeffSum, 2);
                //data for general user average

                //get course coeff
                var courseCoeffData = _context.CourseCoefficients
                    .FirstOrDefault(c => c.ClassLevelid == aclass.ClassLevelId &&
                        c.CourseId == acourse.Id && c.ClassTypeId == aclass.ClassTypeId);
                double courseCoeff = 1;
                if(courseCoeffData != null)
                    courseCoeff = courseCoeffData.Coefficient;

                //get the class course average - to be compared with the user average
                double ClassCourseAvg = GetClassCourseEvalData(acourse.Id, aclass.Id);

                userEvalsDto.UserCourseAvg = gradesAvg;
                userEvalsDto.ClassCourseAvg = ClassCourseAvg;
                userEvalsDto.CourseCoeff = courseCoeff;
                userEvalsDto.grades = grades;
            }
            else
            {
                // there is no grade for the course - User course AVG set to -1000.
                userEvalsDto.UserCourseAvg = -1000;
            }

            return userEvalsDto;
        }

        public double GetClassEvalAvg(List<UserEvaluation> classGrades, double maxGrade)
        {
            double gradesSum = 0;
            double coeffSum = 0;
            for (int i = 0; i < classGrades.Count(); i++)
            {
                var elt = classGrades[i];
                double gradeMax = maxGrade;
                double gradeValue = Convert.ToDouble(elt.Grade);
                // grade are ajusted to 20 as MAx. Avg is on 20
                double ajustedGrade = Math.Round(20 * gradeValue / gradeMax, 2);
                double coeff = elt.Evaluation.Coeff;
                gradesSum += ajustedGrade * coeff;
                coeffSum += coeff;
            }

            return Math.Round(gradesSum / coeffSum, 2);
        }

        public double GetClassCourseEvalData(int courseId, int classId)
        {
            var ClassEvals = _context.UserEvaluations
                .Include(e => e.Evaluation)
                .OrderBy(o => o.Evaluation.EvalDate)
                .Where(e => e.Evaluation.ClassId == classId && e.Evaluation.GradeInLetter == false &&
                   e.Evaluation.CourseId == courseId && e.Evaluation.Graded == true &&
                   e.Grade.IsNumeric())
                .Distinct().ToList();

            double gradesSum = 0;
            double coeffSum = 0;
            for (int i = 0; i < ClassEvals.Count(); i++)
            {
                var elt = ClassEvals[i];
                double gradeMax = Convert.ToDouble(elt.Evaluation.MaxGrade);
                double gradeValue = Convert.ToDouble(elt.Grade);
                // grade are ajusted to 20 as MAx. Avg is on 20
                double ajustedGrade = Math.Round(20 * gradeValue / gradeMax, 2);
                double coeff = elt.Evaluation.Coeff;
                gradesSum += ajustedGrade * coeff;
                coeffSum += coeff;
            }

            return Math.Round(gradesSum / coeffSum, 2);
        }

        public async Task<List<AgendaForListDto>> GetUserClassAgenda(int classId, DateTime startDate, DateTime endDate)
        {
            List<Agenda> classAgenda = await _context.Agendas
                .Include(i => i.Course)
                .OrderBy(o => o.DueDate)
                .Where(a => a.ClassId == classId && a.DueDate.Date >= startDate && a.DueDate <= endDate)
                .ToListAsync();

            var agendaDates = classAgenda.OrderBy(o => o.DueDate).Select(a => a.DueDate).Distinct().ToList();

            List<AgendaForListDto> AgendaList = new List<AgendaForListDto>();
            foreach (var date in agendaDates)
            {
                AgendaForListDto afld = new AgendaForListDto();
                afld.DueDate = date;
                //CultureInfo frC = new CultureInfo("fr-FR");
                var shortDueDate = date.ToString("ddd dd MMM");
                var longDueDate = date.ToString("dd MMMM yyyy");
                var dueDateAbbrev = date.ToString("ddd dd").Replace(".", "");

                afld.ShortDueDate = shortDueDate;
                afld.LongDueDate = longDueDate;
                afld.DueDateAbbrev = dueDateAbbrev;

                //get agenda tasks Done Status
                afld.AgendaItems = new List<AgendaItemDto>();
                var agendaItems = classAgenda.Where(a => a.DueDate.Date == date.Date).ToList();
                foreach (var item in agendaItems)
                {
                    AgendaItemDto aid = new AgendaItemDto();
                    aid.CourseId = item.CourseId;
                    aid.CourseName = item.Course.Name;
                    aid.CourseAbbrev = item.Course.Abbreviation;
                    aid.CourseColor = item.Course.Color;
                    aid.strDateAdded = item.DateAdded.ToShortDateString();
                    aid.TaskDesc = item.TaskDesc;
                    aid.AgendaId = item.Id;
                    aid.Done = item.Done;
                    afld.AgendaItems.Add(aid);
                }
                afld.NbItems = agendaItems.Count();

                AgendaList.Add(afld);
            }

            return AgendaList;
        }

        public async Task<int> GetAssignedChildrenCount(int parentId)
        {
            var userIds = await _context.UserLinks
                                .Where(u => u.UserPId == parentId)
                                .Select(s => s.UserId).ToListAsync();

            return userIds.Count();
        }

        public async Task<bool> SaveProductSelection(int userPid, int userId, List<ServiceSelectionDto> products)
        {
            // insertion dans la table order 
            int total = products.Sum(x => Convert.ToInt32(x.Price));
            var newOrder = new Order  {
                    TotalHT =total,
                    TotalTTC = total,
                    Discount = 0,
                    TVA = 0,
                    UserPId = userPid,
                    UserId = userId
                };
                _context.Add(newOrder);

            foreach (var prod in products)
            {
                var newOrderLine = new OrderLine  {
                    OrderId = newOrder.Id,
                    ProductId = prod.Id,
                    AmountHT = prod.Price,
                    AmountTTC = prod.Price,
                    Discount = 0,
                    Qty = 1,
                    TVA=0

                };
                _context.Add(newOrderLine);
            }
            if(await _context.SaveChangesAsync()>0)
                return true;
            
            return false;
        }

        public List<string> SendBatchSMS(List<Sms> smsData)
        {
            List<string> result = new List<string>();
            foreach (var sms in smsData)
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                Params.Add("content", sms.Content);
                Params.Add("to", sms.To);
                Params.Add("validityPeriod", "1");

                Params["to"] = CreateRecipientList(Params["to"]);
                string JsonArray = JsonConvert.SerializeObject(Params, Formatting.None);
                JsonArray = JsonArray.Replace("\\\"", "\"").Replace("\"[", "[").Replace("]\"", "]");
                
                string Token = _config.GetValue<string>("AppSettings:CLICKATELL_TOKEN");

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://platform.clickatell.com/messages");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Headers.Add("Authorization", Token);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(JsonArray);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result.Add(streamReader.ReadToEnd());
                }
            }

            return result;
        }

        public void Clickatell_SendSMS(clickatellParamsDto smsData)
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("content", smsData.Content);
            Params.Add("to", smsData.To);
            Params.Add("validityPeriod", "1");

            Params["to"] = CreateRecipientList(Params["to"]);
            string JsonArray = JsonConvert.SerializeObject(Params, Formatting.None);
            JsonArray = JsonArray.Replace("\\\"", "\"").Replace("\"[", "[").Replace("]\"", "]");
            
            string Token = _config.GetValue<string>("AppSettings:CLICKATELL_TOKEN");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://platform.clickatell.com/messages");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Headers.Add("Authorization", Token);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JsonArray);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }

        public List<Sms> SetSmsDataForAbsences(List<AbsenceSmsDto> absences, string SmsContent)
        {
            List<Sms> AbsencesSms = new List<Sms>();
            List<Token> tokens = GetTokens();

            foreach (var abs in absences)
            {
                Sms newSms = new Sms();
                newSms.To = abs.ParentCellPhone;
                newSms.ToUserId = abs.ParentId;
                // replace tokens with dynamic data
                List<TokenDto> tags = GetTokenAbsenceValues(tokens, abs);
                newSms.Content = ReplaceTokens(tags, SmsContent);
                AbsencesSms.Add(newSms);
            }

            return AbsencesSms;
        }

        public string ReplaceTokens(List<TokenDto> tokens, string content)
        {
            foreach (var token in tokens)
            {
                content = content.Replace(token.TokenString, token.Value);
            }
            return content;
        }

        public List<Token> GetTokens()
        {
            var tokens = _context.Tokens.OrderBy(t => t.Name).ToList();
            return tokens;
        }

        public List<TokenDto> GetTokenAbsenceValues(List<Token> tokens, AbsenceSmsDto absSms)
        {
            List<TokenDto> tokenValues = new List<TokenDto>();

            foreach (var token in tokens)
            {
                TokenDto td = new TokenDto();
                td.TokenString = token.TokenString;
                
                switch (td.TokenString)
                {
                    case "<P_ENFANT>":
                        td.Value = absSms.ChildFirstName;
                        break;
                    case "<N_ENFANT>":
                        td.Value = absSms.ChildLastName;
                        break;
                    case "<N_PARENT>":
                        td.Value = absSms.ParentLastName;
                        break;
                    case "<P_PARENT>":
                        td.Value = absSms.ParentFirstName;
                        break;
                    case "<M_MME>":
                        td.Value = absSms.ParentGender == 0 ? "Mme" : "M.";
                        break;
                    case "<COURS>":
                        td.Value = absSms.CourseName;
                        break;
                    case "<DATE_COURS>":
                        td.Value = absSms.SessionDate;
                        break;
                    case "<HORAIRE_COURS>":
                        td.Value = absSms.CourseStartHour + " - " + absSms.CourseEndHour;
                        break;
                    default:
                        break;
                }

                tokenValues.Add(td);
            }

            return tokenValues;
        }

        //This function converts the recipients list into an array string so it can be parsed correctly by the json array.
        public static string CreateRecipientList(string to)
        {
            string[] tmp = to.Split(',');
            to = "[\"";
            to = to + string.Join("\",\"", tmp);
            to = to + "\"]";
            return to;
        }

    }
}