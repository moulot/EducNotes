using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Helpers;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace EducNotes.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
         private readonly DataContext _context;
        private readonly IEducNotesRepository _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        int teacherTypeId,parentTypeId,studentTypeId,adminTypeId;
        string password,parentRoleName,memberRoleName,moderatorRoleName,adminRoleName,professorRoleName;
        CultureInfo frC = new CultureInfo ("fr-FR");
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        private readonly UserManager<User> _userManager;

        public UsersController(IConfiguration config,DataContext context, IEducNotesRepository repo,
        UserManager<User> userManager, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _userManager = userManager;
            _config = config;
            _context = context;
            _repo = repo;
            _mapper = mapper;
            teacherTypeId = _config.GetValue<int>("AppSettings:teacherTypeId");
            parentTypeId = _config.GetValue<int>("AppSettings:parentTypeId");
            adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
            studentTypeId = _config.GetValue<int>("AppSettings:studentTypeId");
            password = _config.GetValue<String>("AppSettings:defaultPassword");
            parentRoleName = _config.GetValue<String>("AppSettings:parentRoleName");
            memberRoleName = _config.GetValue<String>("AppSettings:memberRoleName");
            moderatorRoleName = _config.GetValue<String>("AppSettings:moderatorRoleName");
            adminRoleName = _config.GetValue<String>("AppSettings:adminRoleName");
            professorRoleName = _config.GetValue<String>("AppSettings:professorRoleName");
            
            _cloudinaryConfig = cloudinaryConfig;
            Account acc = new Account(
               _cloudinaryConfig.Value.CloudName,
               _cloudinaryConfig.Value.ApiKey,
               _cloudinaryConfig.Value.ApiSecret
           );

            _cloudinary = new Cloudinary(acc);
        }

        // [HttpGet]
        // public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        // {
        //     var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        //     var userFromRepo = await _repo.GetUser(currentUserId, true);

        //     userParams.userId = currentUserId;

        //     var users = await _repo.GetUsers(userParams);

        //     var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

        //     Response.AddPagination(users.CurrentPage, users.PageSize,
        //     users.TotalCount, users.TotalPages);

        //     return Ok(usersToReturn);
        // }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
          var users = await _context.Users
                              .Include(i => i.Class)
                              .Include(i => i.UserType)
                              .Where(u => u.Active == 1 && u.EmailConfirmed == true)
                              .OrderBy(u => u.LastName).ToListAsync();

          var usersToReturn = _mapper.Map<IEnumerable<UserForAutoCompleteDto>>(users);
          return Ok(usersToReturn);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await (from user in _context.Users orderby user.UserName
                                    select new
                                    {
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

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;
            
            var user = await _repo.GetUser(id, isCurrentUser);

            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpGet("Account/{id}")]
        public async Task<IActionResult> GetUserAccount(int id)
        {
            var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;
            
            var user = await _repo.GetUser(id, isCurrentUser);
            var parent = _mapper.Map<UserForAccountDto>(user);

            var ChildrenFromRepo = await _repo.GetChildren(id);
            var children = _mapper.Map<IEnumerable<ChildForAccountDto>>(ChildrenFromRepo);

            var categories = await _context.SmsCategories.OrderBy(s => s.Name).ToListAsync();
            var smsTemplates = await _context.SmsTemplates.OrderBy(s => s.Name).ToListAsync();

            parent.Children = new List<ChildSmsDto>();
            List<ChildSmsDto> childrenSms = new List<ChildSmsDto>();
            List<SmsDataDto> activeSms = new List<SmsDataDto>();
            foreach (var child in children)
            {
                ChildSmsDto childWithSms = new ChildSmsDto();
                childWithSms.Child = child;
                childWithSms.SmsByCategory = new List<SmsByCategoryDto>();

                var userSms = await _context.UserSmsTemplates
                                .Where(s => s.ChildId == child.Id && s.ParentId == parent.Id).ToListAsync();

                List<SmsByCategoryDto> SmsByCategory = new List<SmsByCategoryDto>();
                foreach (var cat in categories)
                {
                    SmsByCategoryDto sbcd = new SmsByCategoryDto();
                    sbcd.CategoryId = cat.Id;
                    sbcd.CategoryName = cat.Name;
                    sbcd.Sms = new List<UserSmsTemplateDto>();

                    var SmsByCat = smsTemplates.FindAll(s => s.SmsCategoryId == cat.Id).OrderBy(s => s.Name);
                    if(SmsByCat.Count() > 0)
                    {
                        SmsDataDto sdd = new SmsDataDto();
                        foreach (var item in SmsByCat)
                        {
                            UserSmsTemplateDto ustd = new UserSmsTemplateDto();
                            
                            ustd.ChildId = child.Id;
                            ustd.ParentId = parent.Id;
                            ustd.SmsTemplateId = item.Id;
                            ustd.SmsName = item.Name;
                            ustd.Content = item.Content;
                            ustd.SmsCategoryId = item.SmsCategoryId;
                            ustd.Active = userSms.FirstOrDefault(u => u.SmsTemplateId == item.Id) != null ? true : false;
                            sbcd.Sms.Add(ustd);

                            if(ustd.Active)
                            {
                                sdd.ChildId = child.Id;
                                sdd.SmsId = item.Id;
                                activeSms.Add(sdd);
                            }
                        }
                        SmsByCategory.Add(sbcd);
                        childWithSms.SmsByCategory.Add(sbcd);
                    }
                }
                parent.Children.Add(childWithSms);
            }

            return Ok(new{
                parent,
                activeSms
            });
        }

        [HttpPut("{id}/saveSMS")]
        public async Task<IActionResult> SaveUserSMS(int id, [FromBody] List<SmsDataDto> smsData)
        {
            List<UserSmsTemplate> newUserSMS = new List<UserSmsTemplate>();
            foreach (var sms in smsData)
            {
                int childId = sms.ChildId;
                int smsId = sms.SmsId;

                List<UserSmsTemplate> oldUserSMS = await _context.UserSmsTemplates.Where(s => s.ChildId == childId).ToListAsync();
                if(oldUserSMS.Count() > 0)
                    _repo.DeleteAll(oldUserSMS);

                UserSmsTemplate ust = new UserSmsTemplate();
                ust.ChildId = childId;
                ust.SmsTemplateId = smsId;
                ust.ParentId = id;
                newUserSMS.Add(ust);
            }
            _context.AddRange(newUserSMS);

            if (await _repo.SaveAll())
                return Ok();

            throw new Exception($"la validation des sms a échoué");
        }

        [HttpGet("Types")]
        public async Task<IActionResult> GetUserTypes()
        {
            var userTypes = await _context.UserTypes.OrderBy(o => o.Name).ToListAsync();
            return Ok(userTypes);
        }

        [HttpGet("GetUserTypesDetails")]
        public  async Task<IActionResult> GetUserTypesDetails()
        {
            var users =await _context.Users.ToListAsync();
             var usertypes = await _context.UserTypes.OrderBy(a=>a.Name).ToListAsync();

            var data = new List<UserTypesDto>();

            foreach (var item in usertypes)
            {
                data.Add( new UserTypesDto {
                    UserType = item,
                    Total = users.Where(a => a.UserTypeId == item.Id).Count(),
                    TotalActive = users.Where(a => a.UserTypeId == item.Id && a.ValidatedCode == true).Count(),
                    
                });
            }

            return Ok(data);
        }

        [HttpGet("{parentId}/Children")]
        public async Task<IActionResult> GetChildren(int parentId)
        {
          var users = await _repo.GetChildren(parentId);

          var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(users);

          var startDate = DateTime.Now.Date;
          var endDate = startDate.AddDays(7).Date;
          
          foreach (var user in usersToReturn)
          {
            double courseAvgSum = 0;
            double courseCoeffSum = 0;

            List<UserEvalsDto> coursesWithEvals = await _repo.GetUserGrades(user.Id, user.ClassId);

            foreach (var course in coursesWithEvals)
            {
              courseAvgSum += course.UserCourseAvg * course.CourseCoeff;
              courseCoeffSum += course.CourseCoeff;
            }
            user.Avg =  Math.Round(courseAvgSum / courseCoeffSum, 2);

            user.AgendaItems = await _repo.GetUserClassAgenda(user.ClassId, startDate, endDate);
          }

          return Ok(usersToReturn);
        }

        [HttpGet("{teacherId}/ScheduleByDay")]
        public async Task<IActionResult> GetTeacherScheduleByDay(int teacherId)
        {
          var scheduleItems = await _context.Schedules
                                    .Include(i => i.Teacher)
                                    .Include(i => i.Class)
                                    .Include(i => i.Course)
                                    .Where(s => s.TeacherId == teacherId).ToListAsync();
          var days = scheduleItems.Select(d => d.Day).Distinct().OrderBy(o => o);

          TeacherScheduleDto teacherSchedule = new TeacherScheduleDto();
          if(scheduleItems.Count() > 0)
          {
            teacherSchedule.TeacherId = teacherId;
            teacherSchedule.TeacherName = scheduleItems[0].Teacher.LastName + " " + scheduleItems[0].Teacher.FirstName;

            teacherSchedule.Days = new List<ScheduleDayDto>();
            foreach (var day in days)
            {
              ScheduleDayDto sdd = new ScheduleDayDto();
              sdd.Day = day;
              sdd.DayName = day.DayIntToName();

              var courses = scheduleItems.Where(s => s.Day == day)
                                          .OrderBy(o => o.StartHourMin.ToString("HH:mm", frC));
              sdd.Courses = new List<ScheduleCourseDto>();
              foreach (var course in courses)
              {
                ScheduleCourseDto courseDto = new ScheduleCourseDto();
                courseDto.CourseId = course.Id;
                courseDto.CourseName = course.Course.Name;
                courseDto.ClassId = course.ClassId;
                courseDto.ClassName = course.Class.Name;
                courseDto.CourseAbbrev = course.Course.Abbreviation;
                courseDto.StartHour = course.StartHourMin;
                courseDto.StartH = course.StartHourMin.ToString("HH:mm", frC);
                courseDto.EndHour = course.EndHourMin;
                courseDto.EndH = course.EndHourMin.ToString("HH:mm", frC);
                courseDto.InConflict = false;
                sdd.Courses.Add(courseDto);
              }

              teacherSchedule.Days.Add(sdd);
            }
          }

          return Ok(teacherSchedule);
        }

        [HttpGet("{teacherId}/ScheduleByClassByDay")]
        public async Task<IActionResult> GetTeacherScheduleByClassByDay(int teacherId)
        {
          var scheduleItems = await _context.Schedules
                                    .Include(i => i.Teacher)
                                    .Include(i => i.Class)
                                    .Include(i => i.Course)
                                    .Where(s => s.TeacherId == teacherId).ToListAsync();
          var classes = scheduleItems.Select(c => c.Class).Distinct().OrderBy(o => o.Name);

          TeacherScheduleDto teacherSchedule = new TeacherScheduleDto();
          if(scheduleItems.Count() > 0)
          {
            teacherSchedule.TeacherId = teacherId;
            teacherSchedule.TeacherName = scheduleItems[0].Teacher.LastName + " " + scheduleItems[0].Teacher.FirstName;

            teacherSchedule.Classes = new List<ScheduleClassDto>();
            foreach (var aclass in classes)
            {
              ScheduleClassDto scd = new ScheduleClassDto();
              scd.ClassId = aclass.Id;
              scd.ClassName = aclass.Name;

              var days = scheduleItems.Where(s => s.ClassId == aclass.Id).Select(d => d.Day).Distinct().OrderBy(o => o);
              scd.Days = new List<ScheduleDayDto>();
              foreach (var day in days)
              {
                ScheduleDayDto sdd = new ScheduleDayDto();
                sdd.Day = day;
                sdd.DayName = day.DayIntToName();

                var courses = scheduleItems.Where(s => s.ClassId == aclass.Id && s.Day == day)
                                            .OrderBy(o => o.StartHourMin.ToString("HH:mm", frC));
                sdd.Courses = new List<ScheduleCourseDto>();
                foreach (var course in courses)
                {
                  ScheduleCourseDto courseDto = new ScheduleCourseDto();
                  courseDto.CourseId = course.Id;
                  courseDto.CourseName = course.Course.Name;
                  courseDto.CourseAbbrev = course.Course.Abbreviation;
                  courseDto.StartHour = course.StartHourMin;
                  courseDto.StartH = course.StartHourMin.ToString("HH:mm", frC);
                  courseDto.EndHour = course.EndHourMin;
                  courseDto.EndH = course.EndHourMin.ToString("HH:mm", frC);
                  sdd.Courses.Add(courseDto);
                }

                scd.Days.Add(sdd);
              }

              teacherSchedule.Classes.Add(scd);
            }
          }

          return Ok(teacherSchedule);
        }

        [HttpGet("{teacherId}/NextCourses")]
        public async Task<IActionResult> GetTeacherNextCourses(int teacherId)
        {
            var nextCourses = 10; // next coming courses
            var today = DateTime.Now;

            // monday=1, tue=2, ...
            var todayDay = ((int)today.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;            
            var todayHourMin = today.TimeOfDay;

            var teacherCourses = await (from courses in _context.ClassCourses
                                        join Schedule in _context.Schedules
                                        on courses.CourseId equals Schedule.CourseId
                                        select new
                                        {
                                            ScheduleId = Schedule.Id,
                                            TeacherId = courses.TeacherId,
                                            TeacherName = courses.Teacher.LastName + ' ' + courses.Teacher.FirstName,
                                            CourseId = courses.CourseId,
                                            CourseName = courses.Course.Name,
                                            ClassId = Schedule.ClassId,
                                            ClassName = Schedule.Class.Name,
                                            Day = Schedule.Day,
                                            CourseStartHM = Schedule.StartHourMin,
                                            CourseEndHM = Schedule.EndHourMin,
                                            StartHourMin = Schedule.StartHourMin.ToString("HH:mm", frC),
                                            EndHourMin = Schedule.EndHourMin.ToString("HH:mm", frC)
                                        })
                                        .Where(w => w.TeacherId == teacherId && w.Day == todayDay)
                                          //&& w.CourseStartHM.TimeOfDay >= todayHourMin)
                                        .OrderBy(o => o.StartHourMin)
                                        .Distinct()
                                        .Take(nextCourses)
                                        .ToListAsync();

            return Ok(teacherCourses);
        }

        [HttpGet("{teacherId}/ScheduleToday")]
        public async Task<IActionResult> GetTeacherScheduleToday(int teacherId)
        {
            // monday=1, tue=2, ...
            var today = ((int)DateTime.Now.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;

            //if saturday or sunday goes to monday schedule
            if(today == 6 || today == 7)
                today = 1;

            return await GetTeacherScheduleDay(teacherId, today);
        }

        [HttpGet("{teacherId}/ScheduleDay/{day}")]
        public async Task<IActionResult> GetTeacherScheduleDay(int teacherId, int day)
        {
            var teacherCourses = await (from courses in _context.ClassCourses
                                    join Schedule in _context.Schedules
                                    on courses.CourseId equals Schedule.CourseId
                                    select new
                                    {
                                        TeacherId = courses.TeacherId,
                                        TeacherName = courses.Teacher.LastName + ' ' + courses.Teacher.FirstName,
                                        CourseId = courses.CourseId,
                                        CourseName = courses.Course.Name,
                                        ClassId = Schedule.ClassId,
                                        ClassName = Schedule.Class.Name,
                                        Day = Schedule.Day,
                                        StartHourMin = Schedule.StartHourMin.ToString("HH:mm", frC),
                                        EndHourMin = Schedule.EndHourMin.ToString("HH:mm", frC)
                                    })
                                    .Where(w => w.TeacherId == teacherId && w.Day == day)
                                    .OrderBy(o => o.StartHourMin)
                                    .ToListAsync();

            return Ok(teacherCourses);
        }

        [HttpGet("{teacherId}/Schedule")]
        public async Task<IActionResult> GetTeacherSchedule(int teacherId)
        {
            var teacherCourses = await (from courses in _context.ClassCourses
                                        join Schedule in _context.Schedules
                                        on courses.CourseId equals Schedule.CourseId
                                        select new
                                        {
                                          TeacherId = courses.TeacherId,
                                          TeacherName = courses.Teacher.LastName + ' ' + courses.Teacher.FirstName,
                                          CourseId = courses.CourseId,
                                          CourseName = courses.Course.Name,
                                          ClassId = Schedule.ClassId,
                                          ClassName = Schedule.Class.Name,
                                          Day = Schedule.Day,
                                          StartHourMin = Schedule.StartHourMin.ToString("HH:mm", frC),
                                          EndHourMin = Schedule.EndHourMin.ToString("HH:mm", frC)
                                        })
                                        .Where(w => w.TeacherId == teacherId)
                                        .OrderBy(o => o.Day).ThenBy(o => o.StartHourMin)
                                        .ToListAsync();

            return Ok(teacherCourses);
        }

        [HttpGet("{teacherId}/CurrWeekSessions/{classId}")]
        public async Task<IActionResult> GetTeacherSessions(int teacherId, int classId)
        {
          var teacherSchedule = await _context.Schedules
                                  .Include(i => i.Course)
                                  .Include(i => i.Teacher)
                                  .Include(i => i.Class)
                                  .Where(s => s.TeacherId == teacherId && s.ClassId == classId)
                                  .ToListAsync();

          var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
          var dayInt = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
          var monday = today.AddDays(1 - dayInt);
          var saturday = monday.AddDays(5);

          var agendas = new List<SessionForListDto>();
          for(int i = 0; i < 7; i++)
          {
            var currentDate = monday.AddDays(i).Date;
            var day = ((int)currentDate.DayOfWeek == 0) ? 7 : (int)currentDate.DayOfWeek;
            if(day == 7) { continue; }
            
            SessionForListDto sfld = new SessionForListDto();
            sfld.DueDate = currentDate;
            sfld.ShortDueDate = currentDate.ToString("ddd dd MMM", frC);
            sfld.LongDueDate = currentDate.ToString("dd MMMM yyyy", frC);
            sfld.DueDateAbbrev = currentDate.ToString("ddd dd", frC).Replace(".", "");
            //get agenda tasks Done Status
            sfld.AgendaItems = new List<AgendaToReturnDto>();
            
            //agenda items retrieved from schedule items
            var daySchedule = teacherSchedule
                                .Where(d => d.Day == day && d.ClassId == classId)
                                .OrderBy(d => d.StartHourMin.Hour)
                                .ThenBy(d => d.StartHourMin.Minute);

            foreach (var scheduleItem in daySchedule)
            {
              string startHour = scheduleItem.StartHourMin.ToString("HH:mm", frC);
              string endHour = scheduleItem.EndHourMin.ToString("HH:mm", frC);
              var tasks = "";
              var id = 0;
              var agenda = await _context.Agendas.SingleOrDefaultAsync(a => a.Session.SessionDate.Date == currentDate &&
                a.Session.TeacherId == teacherId && a.Session.ClassId == scheduleItem.ClassId &&
                a.Session.StartHourMin.ToString("HH:mm", frC) == startHour &&
                a.Session.EndHourMin.ToString("HH:mm", frC) == endHour);
              if(agenda != null) {
                tasks = agenda.TaskDesc;
                id = agenda.Id;
              }

              var session = await _repo.GetSessionFromSchedule(scheduleItem.Id, teacherId, currentDate);
              var newAgenda = new AgendaToReturnDto {
                Id = id,
                SessionId = session.Id,
                TeacherId = Convert.ToInt32(scheduleItem.TeacherId),
                TeacherName = scheduleItem.Teacher.LastName + ' ' + scheduleItem.Teacher.FirstName,
                CourseId = Convert.ToInt32(scheduleItem.CourseId),
                strDayDate = currentDate.ToString("dd/MM/yyyy", frC),
                DayDate = currentDate,
                Day = scheduleItem.Day,
                CourseName = scheduleItem.Course.Name,
                CourseColor = scheduleItem.Course.Color,
                ClassId = Convert.ToInt32(scheduleItem.ClassId),
                ClassName = scheduleItem.Class.Name,
                Tasks = tasks,
                StartHourMin = scheduleItem.StartHourMin.ToString("HH:mm", frC),
                EndHourMin = scheduleItem.EndHourMin.ToString("HH:mm", frC)
              };

              sfld.AgendaItems.Add(newAgenda);
            }

            //retrieve agenda items with lost shceduleId (schedule item has been deleted/updated)
            var itemsWithNoScheduleId = await _context.Agendas
                                              .Include(a => a.Session)
                                              .Where(a => a.Session.SessionDate.Date == currentDate.Date &&
                                                a.Session.ScheduleId == null)
                                              .OrderBy(o => o.Session.StartHourMin.Hour)
                                              .ThenBy(o => o.Session.StartHourMin.Minute)
                                              .ToListAsync();
            
            foreach (var item in itemsWithNoScheduleId)
            {
              var itemDayInt = ((int)item.Session.SessionDate.DayOfWeek == 0) ? 7 : (int)currentDate.DayOfWeek;
              if(day == 7) { continue; }

              var newAgenda = new AgendaToReturnDto {
                Id = item.Id,
                SessionId = item.SessionId,
                TeacherId = Convert.ToInt32(item.Session.TeacherId),
                TeacherName = item.Session.Teacher.LastName + ' ' + item.Session.Teacher.FirstName,
                CourseId = Convert.ToInt32(item.Session.CourseId),
                strDayDate = currentDate.ToString("dd/MM/yyyy", frC),
                DayDate = currentDate,
                Day = itemDayInt,
                CourseName = item.Session.Course.Name,
                CourseColor = item.Session.Course.Color,
                ClassId = item.Session.ClassId,
                ClassName = item.Session.Class.Name,
                Tasks = item.TaskDesc,
                StartHourMin = item.Session.StartHourMin.ToString("HH:mm", frC),
                EndHourMin = item.Session.EndHourMin.ToString("HH:mm", frC)
              };

              sfld.AgendaItems.Add(newAgenda);
            }

            agendas.Add(sfld);
          }

          var days = new List<string>();
          var weekDates = new List<DateTime>();
          var nbTasks = new List<int>();
          for (int i = 0; i <= 5; i++) {
              DateTime dt = monday.AddDays(i);
              var shortdate = dt.ToString("ddd dd MMM", frC);
              days.Add(shortdate);
              weekDates.Add(dt.Date);
          }

          var itemsFromRepo = await _repo.GetClassAgenda(classId, monday, saturday);
          var items = _repo.GetAgendaListByDueDate(itemsFromRepo);

          var classCourses = await _context.ClassCourses
              .Where(c => c.ClassId == classId && c.TeacherId == teacherId)
              .Select(s => s.Course).ToListAsync();

          List<CourseTasksDto> coursesWithTasks = new List<CourseTasksDto>();
          foreach(var course in classCourses) {
              CourseTasksDto ctd = new CourseTasksDto();
              var nbItems = itemsFromRepo.Where(a => a.Session.CourseId == course.Id).ToList().Count();
              ctd.CourseId = course.Id;
              ctd.CourseName = course.Name;
              ctd.CourseAbbrev = course.Abbreviation;
              ctd.CourseColor = course.Color;
              ctd.NbTasks = nbItems;
              coursesWithTasks.Add(ctd);
          }

          foreach (var item in agendas)
          {
            item.AgendaItems = item.AgendaItems.OrderBy(o => o.StartHourMin).ToList();
          }

          return Ok(new {
            agendas,
            monday,
            weekDays = days,
            weekDates,
            coursesWithTasks
          });
        }

        [HttpGet("{teacherId}/SessionsFromToday/{classId}")]
        public async Task<IActionResult> GetTeacherSessionsFromToday(int teacherId, int classId)
        {
          var teacherSchedule = await _context.Schedules
                                  .Include(i => i.Course)
                                  .Include(i => i.Teacher)
                                  .Include(i => i.Class)
                                  .Where(s => s.TeacherId == teacherId && s.ClassId == classId)
                                  .ToListAsync();

          var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
          var dayInt = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
          if(dayInt == 7)
            today = today.AddDays(1);
          // var monday = today.AddDays(1 - dayInt);
          // var saturday = monday.AddDays(5);

          var agendas = new List<SessionForListDto>();
          for(int i = 0; i < 7; i++)
          {
            var currentDate = today.AddDays(i).Date;
            var day = ((int)currentDate.DayOfWeek == 0) ? 7 : (int)currentDate.DayOfWeek;
            if(day == 7) { continue; }
            
            SessionForListDto sfld = new SessionForListDto();
            sfld.DueDate = currentDate;
            sfld.ShortDueDate = currentDate.ToString("ddd dd MMM", frC);
            sfld.LongDueDate = currentDate.ToString("dd MMMM yyyy", frC);
            sfld.DueDateAbbrev = currentDate.ToString("ddd dd", frC).Replace(".", "");
            //get agenda tasks Done Status
            sfld.AgendaItems = new List<AgendaToReturnDto>();
            
            //agenda items retrieved from schedule items
            var daySchedule = teacherSchedule
                                .Where(d => d.Day == day && d.ClassId == classId)
                                .OrderBy(d => d.StartHourMin.Hour)
                                .ThenBy(d => d.StartHourMin.Minute);

            foreach (var scheduleItem in daySchedule)
            {
              string startHour = scheduleItem.StartHourMin.ToString("HH:mm", frC);
              string endHour = scheduleItem.EndHourMin.ToString("HH:mm", frC);
              var tasks = "";
              var id = 0;
              var agenda = await _context.Agendas.SingleOrDefaultAsync(a => a.Session.SessionDate.Date == currentDate &&
                a.Session.TeacherId == teacherId && a.Session.ClassId == scheduleItem.ClassId &&
                a.Session.StartHourMin.ToString("HH:mm", frC) == startHour &&
                a.Session.EndHourMin.ToString("HH:mm", frC) == endHour);
              if(agenda != null) {
                tasks = agenda.TaskDesc;
                id = agenda.Id;
              }

              var session = await _repo.GetSessionFromSchedule(scheduleItem.Id, teacherId, currentDate);
              var newAgenda = new AgendaToReturnDto {
                Id = id,
                SessionId = session.Id,
                TeacherId = Convert.ToInt32(scheduleItem.TeacherId),
                TeacherName = scheduleItem.Teacher.LastName + ' ' + scheduleItem.Teacher.FirstName,
                CourseId = Convert.ToInt32(scheduleItem.CourseId),
                strDayDate = currentDate.ToString("dd/MM/yyyy", frC),
                DayDate = currentDate,
                Day = scheduleItem.Day,
                CourseName = scheduleItem.Course.Name,
                CourseColor = scheduleItem.Course.Color,
                ClassId = Convert.ToInt32(scheduleItem.ClassId),
                ClassName = scheduleItem.Class.Name,
                Tasks = tasks,
                StartHourMin = scheduleItem.StartHourMin.ToString("HH:mm", frC),
                EndHourMin = scheduleItem.EndHourMin.ToString("HH:mm", frC)
              };

              sfld.AgendaItems.Add(newAgenda);
            }

            //retrieve agenda items with lost shceduleId (schedule item has been deleted/updated)
            var itemsWithNoScheduleId = await _context.Agendas
                                              .Include(a => a.Session)
                                              .Where(a => a.Session.SessionDate.Date == currentDate.Date &&
                                                a.Session.ScheduleId == null)
                                              .OrderBy(o => o.Session.StartHourMin.Hour)
                                              .ThenBy(o => o.Session.StartHourMin.Minute)
                                              .ToListAsync();
            
            foreach (var item in itemsWithNoScheduleId)
            {
              var itemDayInt = ((int)item.Session.SessionDate.DayOfWeek == 0) ? 7 : (int)currentDate.DayOfWeek;
              if(day == 7) { continue; }

              var newAgenda = new AgendaToReturnDto {
                Id = item.Id,
                SessionId = item.SessionId,
                TeacherId = Convert.ToInt32(item.Session.TeacherId),
                TeacherName = item.Session.Teacher.LastName + ' ' + item.Session.Teacher.FirstName,
                CourseId = Convert.ToInt32(item.Session.CourseId),
                strDayDate = currentDate.ToString("dd/MM/yyyy", frC),
                DayDate = currentDate,
                Day = itemDayInt,
                CourseName = item.Session.Course.Name,
                CourseColor = item.Session.Course.Color,
                ClassId = item.Session.ClassId,
                ClassName = item.Session.Class.Name,
                Tasks = item.TaskDesc,
                StartHourMin = item.Session.StartHourMin.ToString("HH:mm", frC),
                EndHourMin = item.Session.EndHourMin.ToString("HH:mm", frC)
              };

              sfld.AgendaItems.Add(newAgenda);
            }

            agendas.Add(sfld);
          }

          var days = new List<string>();
          var weekDates = new List<DateTime>();
          var nbTasks = new List<int>();
          for (int i = 0; i <= 5; i++) {
              DateTime dt = today.AddDays(i);
              var shortdate = dt.ToString("ddd dd MMM", frC);
              days.Add(shortdate);
              weekDates.Add(dt.Date);
          }

          var itemsFromRepo = await _repo.GetClassAgenda(classId, today, today.AddDays(5));
          var items = _repo.GetAgendaListByDueDate(itemsFromRepo);

          var classCourses = await _context.ClassCourses
              .Where(c => c.ClassId == classId && c.TeacherId == teacherId)
              .Select(s => s.Course).ToListAsync();

          List<CourseTasksDto> coursesWithTasks = new List<CourseTasksDto>();
          foreach(var course in classCourses) {
              CourseTasksDto ctd = new CourseTasksDto();
              var nbItems = itemsFromRepo.Where(a => a.Session.CourseId == course.Id).ToList().Count();
              ctd.CourseId = course.Id;
              ctd.CourseName = course.Name;
              ctd.CourseAbbrev = course.Abbreviation;
              ctd.CourseColor = course.Color;
              ctd.NbTasks = nbItems;
              coursesWithTasks.Add(ctd);
          }

          foreach (var item in agendas)
          {
            item.AgendaItems = item.AgendaItems.OrderBy(o => o.StartHourMin).ToList();
          }

          return Ok(new {
            agendas,
            today,
            weekDays = days,
            weekDates,
            coursesWithTasks
          });
        }

        [HttpGet ("{teacherId}/MovedWeekSessions/{classId}")]
        public async Task<IActionResult> getClassMovedWeekAgenda (int teacherId, int classId, [FromQuery] AgendaParams agendaParams)
        {
            var teacherSchedule = await _context.Schedules
                                    .Include(i => i.Course)
                                    .Include(i => i.Teacher)
                                    .Include(i => i.Class)
                                    .Where(s => s.TeacherId == teacherId && s.ClassId == classId)
                                    .ToListAsync();

            var FromDate = agendaParams.DueDate.Date;
            var move = agendaParams.MoveWeek;
            var date = FromDate.AddDays(move);
            var dateDay = (int)date.DayOfWeek;

            var dayInt = dateDay == 0 ? 7 : dateDay;
            if(dayInt == 7)
              date = date.AddDays(1);
            // DateTime monday = date.AddDays(1 - dayInt);
            // var saturday = monday.AddDays(5);

            var agendas = new List<SessionForListDto>();

            // cahier de textes - periode de sessions des cours du professeur
            for(int i = 0; i < 7; i++)
            {
                var currentDate = date.AddDays(i);
                var day = ((int)currentDate.DayOfWeek == 0) ? 7 : (int)currentDate.DayOfWeek;
                if(day == 7) { continue; }
                
                SessionForListDto sfld = new SessionForListDto();
                sfld.DueDate = currentDate;
                sfld.ShortDueDate = currentDate.ToString("ddd dd MMM", frC);
                sfld.LongDueDate = currentDate.ToString("dd MMMM yyyy", frC);
                sfld.DueDateAbbrev = currentDate.ToString("ddd dd", frC).Replace(".", "");

                //get agenda tasks Done Status
                sfld.AgendaItems = new List<AgendaToReturnDto>();
                var daySessions = teacherSchedule.Where(d => d.Day == day).OrderBy(d => d.StartHourMin);
                if(daySessions.Count() == 0) { continue; }

                foreach (var session in daySessions)
                {
                  string startHour = session.StartHourMin.ToString("HH:mm", frC);
                  string endHour = session.EndHourMin.ToString("HH:mm", frC);
                  var tasks = "";
                  var id = 0;
                  var agenda = await _context.Agendas.SingleOrDefaultAsync(a => a.Session.SessionDate.Date == currentDate &&
                    a.Session.StartHourMin.ToString("HH:mm", frC) == startHour &&
                    a.Session.EndHourMin.ToString("HH:mm", frC) == endHour);
                  if(agenda != null) {
                    tasks = agenda.TaskDesc;
                    id = agenda.Id;
                  }

                  var newAgenda = new AgendaToReturnDto {
                    Id = id,
                    SessionId = session.Id,
                    TeacherId = Convert.ToInt32(session.TeacherId),
                    TeacherName =  session.Teacher.LastName + ' ' + session.Teacher.FirstName,
                    CourseId = Convert.ToInt32(session.CourseId),
                    strDayDate = currentDate.ToString("dd/MM/yyyy", frC),
                    DayDate = currentDate,
                    Day = session.Day,
                    CourseName = session.Course.Name,
                    CourseColor = session.Course.Color,
                    ClassId = Convert.ToInt32(session.ClassId),
                    ClassName = session.Class.Name,
                    Tasks = tasks,
                    StartHourMin = session.StartHourMin.ToString("HH:mm", frC),
                    EndHourMin = session.EndHourMin.ToString("HH:mm", frC)
                  };

                  sfld.AgendaItems.Add(newAgenda);
                }
                sfld.NbItems = daySessions.Count();

                agendas.Add(sfld);
            }

            var days = new List<string>();
            var weekDates = new List<DateTime>();
            var nbTasks = new List<int>();
            for (int i = 0; i <= 5; i++) {
                DateTime dt = date.AddDays(i);
                var shortdate = dt.ToString("ddd dd MMM", frC);
                days.Add(shortdate);
                weekDates.Add(dt);
            }

            var itemsFromRepo = await _repo.GetClassAgenda(classId, date, date.AddDays(5));
            var items = _repo.GetAgendaListByDueDate(itemsFromRepo);

            var classCourses = await _context.ClassCourses
                .Where(c => c.ClassId == classId && c.TeacherId == teacherId)
                .Select(s => s.Course).ToListAsync();

            List<CourseTasksDto> coursesWithTasks = new List<CourseTasksDto>();
            foreach(var course in classCourses) {
                CourseTasksDto ctd = new CourseTasksDto();
                var nbItems = itemsFromRepo.Where(a => a.Session.CourseId == course.Id).ToList().Count();
                ctd.CourseId = course.Id;
                ctd.CourseName = course.Name;
                ctd.CourseAbbrev = course.Abbreviation;
                ctd.CourseColor = course.Color;
                ctd.NbTasks = nbItems;
                coursesWithTasks.Add(ctd);
            }

            if (itemsFromRepo != null) {
                return Ok(new {
                    agendas,
                    date,
                    weekDays = days,
                    weekDates,
                    coursesWithTasks
                });
            }

            return BadRequest("Aucun agenda trouvé");
        }

        [HttpGet("{teacherId}/GradesData/{periodId}")]
        public async Task<IActionResult> GetGradesData(int teacherId, int periodId)
        {
            var teacherCourses = await _repo.GetTeacherCourses(teacherId);
            var teacherClasses = await _repo.GetTeacherClasses(teacherId);
            var classesWithEvals = await _repo.GetTeacherClassesWithEvalsByPeriod(teacherId, periodId);

            return Ok(new{
                teacherCourses,
                teacherClasses,
                classesWithEvals
            });
        }

        [HttpGet("{teacherId}/Courses")]
        public async Task<IActionResult> GetTeacherCourses(int teacherId)
        {
            var courses = await _repo.GetTeacherCourses(teacherId);
            return Ok(courses);
        }

        [HttpGet("{teacherId}/teacherWithCourses")]
        public async Task<IActionResult> GetTeacherWithCourses(int teacherId)
        {
          var teacherFromDB = await _context.Users
                                    .Include(i => i.Photos)
                                    .FirstOrDefaultAsync(u => u.Id == teacherId);
          TeacherForEditDto teacher = _mapper.Map<TeacherForEditDto>(teacherFromDB);

          var courses = await _repo.GetTeacherCourses(teacherId);
          string listCourses = "";
          teacher.ClassesAssigned = new List<CourseDto>();
          foreach (var course in courses)
          {
            if(listCourses == "")
            {
              listCourses = course.Id.ToString();
            }
            else
            {
              listCourses += "," + course.Id.ToString();
            }

            CourseDto cd = new CourseDto();
            cd.Id = course.Id;
            cd.Name = course.Name;
            cd.ClassesAssigned = false;
            // does this course has classes assigned to it?
            var classesAssigned = await _context.ClassCourses
              .Where(c => c.TeacherId == teacherId && c.CourseId == course.Id).ToListAsync();
            if(classesAssigned.Count() > 0)
            {
              cd.ClassesAssigned = true;
            }
            teacher.ClassesAssigned.Add(cd);
          }
          
          teacher.CourseIds = listCourses;

          return Ok(teacher);
        }

        [HttpGet("{teacherId}/AssignedClasses")]
        public async Task<IActionResult> GetAssignedClasses(int teacherId)
        {
          //get teacher with all his classes
          var teacherFromDB = await _context.Users.FirstOrDefaultAsync(u => u.Id == teacherId);
          var teacher = _mapper.Map<User>(teacherFromDB);
          if (teacherFromDB != null)
          {
            List<AssignedClassesDto> classes = new List<AssignedClassesDto>();
            var courses = await _context.TeacherCourses.Where(t => t.TeacherId == teacherId).Select(c => c.Course).ToListAsync();
            foreach (var course in courses)
            {
              AssignedClassesDto acd = new AssignedClassesDto();
              acd.CourseId = course.Id;
              acd.CourseName = course.Name;

              acd.Levels = new List<LevelWithClassesDto>();
              var levels = await _context.ClassLevels.OrderBy(c => c.DsplSeq).ToListAsync();
              foreach (var level in levels)
              {
                LevelWithClassesDto lwcd = new LevelWithClassesDto();

                var selectedIds = await _context.ClassCourses.Where(c => c.TeacherId == teacherId && c.CourseId == course.Id &&
                                          c.Class.ClassLevelId == level.Id).Select(t => t.ClassId).Distinct().ToListAsync();
                var classesLevel = await _repo.GetClassesByLevelId(level.Id);
                if(classesLevel.Count() > 0)
                {
                  lwcd.LevelId = level.Id;
                  lwcd.LevelName = level.Name;
                  lwcd.Classes = new List<ClassIdAndNameDto>();
                  foreach (var aclass in classesLevel)
                  {
                    ClassIdAndNameDto cd = new ClassIdAndNameDto();
                    cd.ClassId = aclass.Id;
                    cd.ClassName = aclass.Name;
                    cd.Active = selectedIds.IndexOf(aclass.Id) != -1 ? true : false;
                    lwcd.Classes.Add(cd);
                  }
                  acd.Levels.Add(lwcd);
                }
              }
              classes.Add(acd);
            }

            return Ok(new {
              classes,
              teacher
            });
          }

          return BadRequest("l'enseignant est introuvable!");
        }

        [HttpPost("{id}/course/{courseId}/level/{levelId}/AddClasses")]
        public async Task<IActionResult> AddClasses(int id, int courseId, int levelId, [FromBody] List<int> classIds)
        {
          // delete previous classes selection for the teacher
          var prevClasses = await _context.ClassCourses
                                    .Include(c => c.Class)
                                    .Where(c => c.CourseId == courseId && c.Class.ClassLevelId == levelId)
                                    .ToListAsync();
          
          if(prevClasses.Count() > 0)
          {
            _repo.DeleteAll(prevClasses);
          }

          // add new classes selection
          foreach (var classId in classIds)
          {
            ClassCourse cc = new ClassCourse();
            cc.TeacherId = id;
            cc.ClassId = classId;
            cc.CourseId = courseId;
            _repo.Add(cc);
          }

          // // récupération des classIds
          // var cids = classcourses.Select(c => c.ClassId).Distinct().ToList();

          // //add new affection (new lines in DB)
          // foreach (var item in classIds.Except(cids))
          // {
          //   ClassCourse classCourse = new ClassCourse
          //   {
          //     CourseId = courseId,
          //     ClassId = item,
          //     TeacherId = id
          //   };
          //   _context.Add(classCourse);
          //   dataToBeSaved = true;
          // }

          // //set teacher for existing class/course
          // foreach(var item in classIds)
          // {
          //     var cc = classcourses.FirstOrDefault(c => c.ClassId == item && c.TeacherId == null);
          //     if (cc != null)
          //     {
          //       cc.TeacherId = id;
          //       dataToBeSaved = true;
          //     }
          // }

          // //remove teacher from class/course as it's unselected
          // var u = classcourses.Where(t => t.TeacherId == id).Select(c => c.ClassId);
          // foreach(var item in u.Except(classIds))
          // {
          //   var cc = classcourses.FirstOrDefault(c => c.ClassId == item);
          //   cc.TeacherId = null;
          //   dataToBeSaved = true;
          // }

          if(await _repo.SaveAll())
            return Ok();

          return BadRequest("problème pour affecter les classes");
        }

        [HttpGet("{teacherId}/Classes")]
        public async Task<IActionResult> GetTeacherClasses(int teacherId)
        {
            // if(teacherId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            //     return Unauthorized();

            var teacherClasses = await (from courses in _context.ClassCourses
                                    join classes in _context.Classes
                                    on courses.ClassId equals classes.Id
                                    where courses.TeacherId == teacherId
                                    select new {
                                        ClassId = classes.Id,
                                        ClassName = classes.Name,
                                        NbStudents = _context.Users.Where(u => u.ClassId == classes.Id).Count()
                                    })
                                    .OrderBy(o => o.ClassName)
                                    .Distinct().ToListAsync();

            return Ok(teacherClasses);
        }

        [HttpGet("{TeacherId}/period/{periodId}/ClassesWithEvalsByPeriod")]
        public async Task<IActionResult> GetTeacherClassesWithEvalsByPeriod(int teacherId, int periodId)
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

            return Ok(classesWithEvals);
        }

        // [HttpGet("{teacherId}/Courses")]
        // public async Task<IActionResult> GetTeacherCourses(int teacherId)
        // {
        //     var courses = await (from cu in _context.TeacherCourses
        //                             where cu.Teacher.Id == teacherId
        //                             select new{
        //                                 TeacherId = cu.TeacherId,
        //                                 TeacherName = cu.Teacher.FirstName + ' ' + cu.Teacher.LastName,
        //                                 CourseId = cu.CourseId,
        //                                 CourseName = cu.Course.Name
        //                             }).OrderBy(o => o.CourseName).ToListAsync();

        //     return Ok(courses);
        // }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(id, true);

            _mapper.Map(userForUpdateDto, userFromRepo);

            if(await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repo.GetLike(id, recipientId);

            if(like != null)
                return BadRequest("you already liked this user");

            if(await _repo.GetUser(recipientId, false) == null)
                return NotFound();

            like = new Models.Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            _repo.Add<Like>(like);

            if(await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to like user");
        }

        [HttpPut("SaveAbsence")]
        public async Task<IActionResult> SaveAbsence([FromBody]Absence absence)
        {
          var currPeriod = await _repo.GetPeriodFromDate(absence.StartDate);
          absence.PeriodId = currPeriod.Id;
          _repo.Add(absence);

          if(await _repo.SaveAll())
              return NoContent();

          throw new Exception($"l'ajout de l'absence a échoué");
        }

        [HttpGet("{userId}/ClassLifeData")]
        public async Task<IActionResult> GetClassLifeData(int userId)
        {
          List<UserClassEventForListDto> events = new List<UserClassEventForListDto>();

          var absencesFromDB = await _context.Absences
                              .Include(i => i.User)
                              .Include(i => i.DoneBy)
                              .Include(i => i.AbsenceType)
                              .Where(a => a.UserId == userId)
                              .OrderByDescending(o => o.StartDate).ToListAsync();

          var absences = _mapper.Map<IEnumerable<UserClassEventForListDto>>(absencesFromDB);
          events.AddRange(absences);

          var lifeEventsFromDB = await _context.UserClassEvents
                                .Include(i => i.User)
                                .Include(i => i.DoneBy)
                                .Include(i => i.ClassEvent)
                                .Where(a => a.User.Id == userId)
                                .OrderByDescending(a => a.StartDate).ToListAsync();

          var lifeEvents = _mapper.Map<IEnumerable<UserClassEventForListDto>>(lifeEventsFromDB);
          events.AddRange(lifeEvents);
          events = events.OrderByDescending(e => e.StartDate).ToList();

          return Ok(new {
            classLifeEvents = events
          });
        }

        [HttpGet("{userId}/LifeData")]
        public async Task<IActionResult> GetStudentLifeData(int userId)
        {
            var absencesFromDB = await _context.Absences
                                .Include(i => i.User)
                                .Include(i => i.DoneBy)
                                .Include(i => i.AbsenceType)
                                .Where(a => a.UserId == userId)
                                .OrderByDescending(o => o.StartDate).ToListAsync();

            //var nbAbscences = absences.Count();
            var absencesToReturn = _mapper.Map<IEnumerable<AbsencesToReturnDto>>(absencesFromDB);

            var sanctions = await _context.UserSanctions
                .Include(i => i.Sanction)
                .Include(i => i.User)
                .Include(i => i.SanctionedBy)
                .Where(a => a.User.Id == userId)
                .OrderByDescending(a => a.SanctionDate).ToListAsync();

            //var nbSanctions = sanctions.Count();
            var sanctionsToReturn = _mapper.Map<IEnumerable<UserSanctionsToReturnDto>>(sanctions);

            return Ok(new {
                absences = absencesToReturn,
                //nbAbsences = nbAbscences,
                sanctions = sanctionsToReturn,
                //nbSanctions = nbSanctions
            });
        }

        [HttpGet("{userId}/Absences")]
        public async Task<IActionResult> GetAbsences(int userId)
        {
            var absences = await _context.Absences
                                .Include(i => i.User)
                                .Where(a => a.UserId == userId)
                                .OrderByDescending(o => o.StartDate).ToListAsync();

            var nbClassAbscences = absences.Count();

            var absencesToReturn = _mapper.Map<IEnumerable<AbsencesToReturnDto>>(absences);

            return Ok(new {
                absences = absencesToReturn,
                nbAbsences = nbClassAbscences
            });
        }

        [HttpPut("SaveClassEvent")]
        public async Task<IActionResult> SaveClassEvent([FromBody]UserClassEvent userClassLife)
        {
          var currPeriod = await _repo.GetPeriodFromDate(userClassLife.StartDate);
          userClassLife.PeriodId = currPeriod.Id;
          _repo.Add(userClassLife);

          if(await _repo.SaveAll())
            return NoContent();

          throw new Exception($"l'ajout de l'évènement a échoué");
        }

        [HttpPut("SaveSanction")]
        public async Task<IActionResult> SaveSanction([FromBody]UserSanction userSanction)
        {
          _repo.Add(userSanction);

          if(await _repo.SaveAll())
            return NoContent();

          throw new Exception($"l'ajout de la sanction à échoué");
        }

        [HttpPut("SaveReward")]
        public async Task<IActionResult> SaveReward([FromBody]UserReward userReward)
        {
            _repo.Add(userReward);

            if(await _repo.SaveAll())
                return NoContent();

            throw new Exception($"l'ajout de l'encouragement à échoué");
        }
        


         [HttpGet("GetAllClassesCourses")]
        public async Task<IActionResult> GetAllClassesCourses()
        {
           var courses = await _context.Classes.Where(a=>a.Active==1).ToListAsync();
           var cc = new List<ClassCoursesDto>();
           foreach (var c in courses)
           {
               cc.Add( new ClassCoursesDto{
                   Class = c,
                   Courses = _context.ClassCourses.Include(e=>e.Course).Where(e=>e.ClassId==c.Id).Select(e=>e.Course).ToList()
               });
           }
           return Ok(cc);
        }

     
      
        // [HttpPost("SavePreinscription")] // enregistrement de préinscription : perer , mere et enfants
        // public async Task<IActionResult> SavePreinscription([FromBody]PreInscriptionDto model)
        // {
        //         try
        //         {
        //             int fatherId=0,motherId =0;
        //             Random random = new Random();

        //              //ajout du père
        //             if(model.father.LastName!="" && model.father.FirstName!="" && model.father.PhoneNumber!="")
        //             {
        //                 var father = _mapper.Map<User>(model.father);
        //                 father.UserTypeId =  parentTypeId;
        //                 int randomNumber = random.Next(100000, 999999);
        //                 fatherId = await _repo.AddUserPreInscription(randomNumber,father);

        [HttpGet("{email}/VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            return Ok(await _repo.EmailExist(email));
        }

       
        [HttpPost("{id}/updateUserType/{typeName}")]

        public async Task<IActionResult> updateUserType(int id,string typeName)
        {
            var userType = await _context.UserTypes.FirstOrDefaultAsync(a=>a.Id == id);
            userType.Name = typeName;
            _repo.Update(userType);
            if(await _repo.SaveAll())
            return NoContent();
            return BadRequest("impossible de faire la mise à jour");
        }

        [HttpPost("AddUserType")]
        public async Task<IActionResult> AddUserType(UserType userType)
        {
            _repo.Add(userType);
            if(await _repo.SaveAll())
            return Ok(userType);
            return BadRequest("impossible d'ajouter cet élément");
        }

        [HttpPost("{id}/DeleteUserType")]
        public async Task<IActionResult>DeleteUserType(int id)
        {
          var userType = await _context.UserTypes.FirstAsync(a=>a.Id == id);
          _repo.Delete(userType);

          if(await _repo.SaveAll())
            return NoContent();

          return BadRequest("impossible de supprimer cet élement ");
        }

        [HttpPost("{userId}/AddPhoto")]
        public async Task<IActionResult> AddPhoto(int userId, [FromForm]IFormFile photoFile)
        {
          var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(a => a.Id == userId);
          var uploadResult = new ImageUploadResult();

          if (photoFile.Length > 0)
          {
            using (var stream = photoFile.OpenReadStream())
            {
              var uploadParams = new ImageUploadParams()
              {
                File = new FileDescription(photoFile.Name, stream),
                Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
              };

              uploadResult = _cloudinary.Upload(uploadParams);
            }
          }

          Photo photo = new Photo();
          photo.Url = uploadResult.Uri.ToString();
          photo.PublicId = uploadResult.PublicId;
          photo.UserId = userId;
          photo.DateAdded = DateTime.Now;
          if (!user.Photos.Any(u => u.IsMain))
          {
            photo.IsMain = true;
            photo.IsApproved = true;
          }
          user.Photos.Add(photo);

          if (await _repo.SaveAll())
            return Ok();

          return BadRequest("Could not add the photo");
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromForm]TeacherForEditDto user)
        {
          int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
          bool userOK = await _repo.AddUser(user, userId);

          if(userOK)
            return Ok();
          else
            return BadRequest("problème pour ajouter l'utilisateur");
        }

        [HttpGet("GetAdminUserTypes")]
        public async Task<IActionResult> GetAdminUserTypes()
        {
            return Ok(await _context.UserTypes.Where(a=>a.Id >= adminTypeId).OrderBy(a => a.Name).ToListAsync());
        }

        [HttpGet("GetUserByTypeId/{id}")]
        public async Task<IActionResult> GetUserByTypeId(int id)
        {
            var users = await _context.Users.Include(p=>p.Photos).Where(a=>a.UserTypeId == id)
        .OrderBy(a=>a.LastName).ThenBy(a=>a.FirstName).ToListAsync();
        var userToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
        return Ok(userToReturn);
        }

        [HttpPost("{id}/updatePerson")]
        public async Task<IActionResult> updatePerson(int id,UserForRegisterDto model)
        {
            var user =await _repo.GetUser(id,false);
            if(user.Id != id)
            return BadRequest("impossible d'effectuer la mise a jour");
            var userToUpdate = _mapper.Map(model, user);
            if (userToUpdate.ClassId < 1)
            userToUpdate.ClassId = null;
            _repo.Update(userToUpdate);
            if(await _repo.SaveAll())
            return NoContent();
            return BadRequest("impossible d'effectuer la mise a jour");
        }

        [HttpPost("SearchUsers")]
        public async Task<IActionResult> SearchUsers(UserSearchDto model)
        {
            if(model.LastName == null)
            model.LastName ="";
            if(model.FirstName == null)
            model.FirstName ="";
            var query = from s in _context.Users.Include(a=>a.UserType).Include(a=>a.Photos)
                where (EF.Functions.Like(s.LastName, "%"+model.LastName+"%") && EF.Functions.Like(s.LastName, "%"+model.FirstName+"%") )
                select s;
            return Ok(_mapper.Map<IEnumerable<UserForListDto>>(await query.ToListAsync()));
          
        }

        [HttpGet("GetAllCities")]
        public async Task<IActionResult>GetAllCities()
        {
          return Ok (await _repo.GetAllCities());
        }

        [HttpGet("{id}/GetDistrictsByCityId")]
        public async Task<IActionResult>GetAllGetDistrictsByCityIdCities(int id)
        {
        return Ok (await _repo.GetAllGetDistrictsByCityIdCities(id));
        }
    }
}