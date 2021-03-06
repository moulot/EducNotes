using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using EducNotes.API.data;
using EducNotes.API.Dtos;
using EducNotes.API.Helpers;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private readonly UserManager<User> _userManager;
    private Cloudinary _cloudinary;
    string password, baseUrl;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId, teacherConfirmEmailId, resetPwdEmailId;
    int parentRoleId, memberRoleId, moderatorRoleId, adminRoleId, teacherRoleId, employeeConfirmEmailId,
    schoolInscTypeId, updateAccountEmailId, broadcastTokenTypeId;
    CultureInfo frC = new CultureInfo("fr-FR");
    public readonly ICacheRepository _cache;
    public readonly IHttpContextAccessor _httpContext;
    public readonly RoleManager<Role> _roleManager;

    public EducNotesRepository(DataContext context, IConfiguration config, IEmailSender emailSender,
      UserManager<User> userManager, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig,
      ICacheRepository cache, IHttpContextAccessor httpContext, RoleManager<Role> roleManager)
    {
      _roleManager = roleManager;
      _httpContext = httpContext;
      _cache = cache;
      _context = context;
      _config = config;
      _emailSender = emailSender;
      _mapper = mapper;
      _cloudinaryConfig = cloudinaryConfig;
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
      teacherConfirmEmailId = _config.GetValue<int>("AppSettings:teacherConfirmEmailId");
      employeeConfirmEmailId = _config.GetValue<int>("AppSettings:employeeConfirmEmailId");
      baseUrl = _config.GetValue<String>("AppSettings:DefaultLink");
      resetPwdEmailId = _config.GetValue<int>("AppSettings:resetPwdEmailId");
      updateAccountEmailId = _config.GetValue<int>("AppSettings:updateAccountEmailId");
      broadcastTokenTypeId = _config.GetValue<int>("AppSettings:broadcastTokenTypeId");

      _cloudinaryConfig = cloudinaryConfig;
      Account acc = new Account(
        _cloudinaryConfig.Value.CloudName,
        _cloudinaryConfig.Value.ApiKey,
        _cloudinaryConfig.Value.ApiSecret
      );
      _cloudinary = new Cloudinary(acc);
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
      var photo = await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
      return photo;
    }

    public Boolean DeletePhotoFromCloudinary(string publicId)
    {
      var deleteParams = new DeletionParams(publicId);
      var result = _cloudinary.Destroy(deleteParams);
      if (result.Result != "ok")
      {
        return false;
      }

      return true;
    }

    public async Task<ErrorDto> DeletePhoto(int userId, int id)
    {
      ErrorDto error = new ErrorDto();
      error.NoError = true;

      var user = await GetUser(userId, true);
      var photoFromRepo = await GetPhoto(id);

      if (photoFromRepo.IsMain)
      {
        error.NoError = false;
        error.Message = "You cannot delete your main photo";
      }

      if (photoFromRepo.PublicId != null)
      {
        var deleteParams = new DeletionParams(photoFromRepo.PublicId);
        var result = _cloudinary.Destroy(deleteParams);
        if (result.Result == "ok")
        {
          Delete(photoFromRepo);
        }
      }

      if (photoFromRepo.PublicId == null)
      {
        Delete(photoFromRepo);
      }

      if (await SaveAll())
      {
        error.NoError = true;
        return error;
      }

      error.NoError = false;
      error.Message = "failed to delete the photo";
      return error;
    }

    public async Task<Photo> GetMainPhotoForUser(int userId)
    {
      return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
    }

    public async Task<User> GetUser(int id, bool isCurrentUser)
    {
      var query = _context.Users
        .Include(c => c.Class)
        .Include(c => c.ClassLevel)
        .Include(c => c.District)
        .Include(c => c.City)
        .Include(p => p.Photos).AsQueryable();

      if (isCurrentUser) { query = query.IgnoreQueryFilters(); }
      var user = await query.FirstOrDefaultAsync(u => u.Id == id);
      return user;
    }

    public async Task<List<UserForDetailedDto>> GetAccountChildren(int parentId)
    {
      List<Order> orders = await _cache.GetOrders();
      List<OrderLine> lines = await _cache.GetOrderLines();

      // MANAGEMENT OF TUITION AND NEXT YEAR TUITION TO BE TREATED!!!!!!!!!!!!!!
      var order = orders.First(o => o.isReg == true && (o.MotherId == parentId || o.FatherId == parentId));
      var usersFromDB = lines
        // .Include(i => i.Child)
        // .Include(i => i.ClassLevel)
        .Where(o => o.OrderId == order.Id)
        .ToList();
      List<UserForDetailedDto> children = new List<UserForDetailedDto>();
      for (int i = 0; i < usersFromDB.Count(); i++)
      {
        var user = usersFromDB[i];
        UserForDetailedDto child = new UserForDetailedDto();
        child = _mapper.Map<UserForDetailedDto>(user.Child);
        child.ClassLevelId = Convert.ToInt32(user.ClassLevelId);
        child.ClassLevelName = user.ClassLevel.Name;
        children.Add(child);
      }

      return children;
    }

    public async Task<IEnumerable<User>> GetSiblings(int childId)
    {
      var childFromRepo = await GetUser(childId, false);
      var parents = await GetParents(childId);
      int motherId = parents.First(p => p.UserTypeId == parentTypeId && p.Gender == 0).Id;
      int fatherId = parents.First(p => p.UserTypeId == parentTypeId && p.Gender == 1).Id;
      var siblings = await GetParentsChildren(motherId, fatherId);
      // siblings.Remove(childFromRepo);
      return siblings;
    }

    public async Task<IEnumerable<User>> GetChildren(int parentId)
    {
      List<UserLink> userLinks = await _cache.GetUserLinks();
      List<User> users = await _cache.GetUsers();

      var userIds = userLinks.Where(u => u.UserPId == parentId).Select(s => s.UserId).ToList();
      return users.Where(u => userIds.Contains(u.Id)).ToList();
    }

    public async Task<List<User>> GetParentsChildren(int motherId, int fatherId)
    {
      List<UserLink> userLinks = await _cache.GetUserLinks();
      List<User> users = await _cache.GetUsers();

      var userIds = userLinks.Where(u => u.UserPId == motherId || u.UserPId == fatherId)
        .Select(s => s.UserId)
        .ToList();

      return users.Where(u => userIds.Contains(u.Id)).Distinct().ToList();
    }

    public async Task<List<User>> GetParents(int ChildId)
    {
      List<User> parentsCached = await _cache.GetParents();
      List<UserLink> userLinks = await _cache.GetUserLinks();

      var links = userLinks.Where(u => u.UserId == ChildId).ToList();

      List<User> parents = new List<User>();
      foreach (var link in links)
      {
        var parent = parentsCached.FirstOrDefault(p => p.Id == link.UserPId);
        parents.Add(parent);
      }

      return parents;
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
      return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
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
      // List<Class> classes = await _cache.GetClasses();
      return await _context.Classes.FirstOrDefaultAsync(c => c.Id == Id);
    }

    public async Task<List<Class>> GetClassesByLevelId(int levelId)
    {
      // List<Class> classes = await _cache.GetClasses();
      return await _context.Classes.Where(c => c.ClassLevelId == levelId)
        .OrderBy(o => o.Name)
        .ToListAsync();
    }

    public async Task<List<Class>> GetFreePrimaryClasses(int teacherId)
    {
      List<ClassCourse> classCourses = await _context.ClassCourses.ToListAsync();
      // List<Class> classesCached = await _cache.GetClasses();

      var assignedClassIds = classCourses.Select(c => c.ClassId).Distinct().ToList();
      var teacherClassIds = classCourses.Where(c => c.TeacherId == teacherId).Select(c => c.ClassId).Distinct().ToList();
      var availableClasses = await _context.Classes
        .Where(c => !assignedClassIds.Contains(c.Id) || teacherClassIds.Contains(c.Id))
        .ToListAsync();
      return availableClasses;
    }

    public async Task<IEnumerable<ScheduleCourse>> GetScheduleDay(int classId, int day)
    {
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();

      return scheduleCourses.Where(d => d.Schedule.Day == day && d.Schedule.Class.Id == classId)
                      .OrderBy(s => s.Schedule.StartHourMin).ToList();
    }

    public async Task<Agenda> GetAgenda(int agendaId)
    {
      List<Agenda> agendas = await _cache.GetAgendas();
      return agendas.FirstOrDefault(a => a.Id == agendaId);
    }

    public async Task<List<User>> GetUsersByClasslevel(int levelId)
    {
      // List<User> users = await _cache.GetUsers();
      return await _context.Users.Where(u => u.ClassLevelId == levelId)
        .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
        .ToListAsync();
    }

    public async Task<List<Product>> GetActiveProducts()
    {
      List<Product> productsCached = await _cache.GetProducts();
      var products = productsCached.Where(p => p.Active)
                                   .OrderBy(o => o.DsplSeq).ThenBy(p => p.Name)
                                   .ToList();
      return products;
    }

    public async Task<EmailTemplate> GetEmailTemplate(int id)
    {
      List<EmailTemplate> templates = await _cache.GetEmailTemplates();
      return templates.FirstOrDefault(s => s.Id == id);
    }

    public async Task<SmsTemplate> GetSmsTemplate(int id)
    {
      List<SmsTemplate> templates = await _context.SmsTemplates.ToListAsync();
      return templates.FirstOrDefault(s => s.Id == id);
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

    public async Task<List<ScheduleForTimeTableDto>> GetClassSchedule(int classId)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();

      List<Schedule> classSchedule = schedules.Where(s => s.ClassId == classId)
                                              .OrderBy(o => o.Day).ThenBy(o => o.StartHourMin).ToList();
      List<ScheduleForTimeTableDto> classCourses = new List<ScheduleForTimeTableDto>();
      foreach (var schedule in classSchedule)
      {
        ScheduleForTimeTableDto scheduleItem = new ScheduleForTimeTableDto();
        scheduleItem.Id = schedule.Id;
        scheduleItem.ClassId = schedule.ClassId;
        scheduleItem.ClassName = schedule.Class.Name;
        scheduleItem.ClassLevelName = schedule.Class.ClassLevel.Name;
        scheduleItem.Day = schedule.Day;
        scheduleItem.StartHourMin = schedule.StartHourMin;
        scheduleItem.strStartHourMin = schedule.StartHourMin.ToString("HH:mm", frC);
        scheduleItem.EndHourMin = schedule.EndHourMin;
        scheduleItem.strEndHourMin = schedule.EndHourMin.ToString("HH:mm", frC);
        scheduleItem.Courses = new List<ScheduleCourseDto>();
        List<ScheduleCourse> courses = scheduleCourses.Where(s => s.ScheduleId == schedule.Id).ToList();
        foreach (var course in courses)
        {
          ScheduleCourseDto courseDto = new ScheduleCourseDto();
          courseDto.Id = course.Id;
          courseDto.ScheduleId = course.ScheduleId;
          courseDto.CourseId = course.CourseId;
          courseDto.CourseName = course.Course.Name;
          courseDto.CourseColor = course.Course.Color;
          courseDto.CourseAbbrev = course.Course.Abbreviation;
          courseDto.DelInfo = course.Course.Name + " de " + schedule.StartHourMin.ToString("HH:mm", frC) +
            " à " + schedule.EndHourMin.ToString("HH:mm", frC);
          courseDto.TeacherId = course.TeacherId;
          courseDto.Gender = course.Teacher.Gender;
          courseDto.TeacherName = (course.Teacher.LastName + " " + course.Teacher.FirstName).UppercaseWords();
          courseDto.TeacherLastName = course.Teacher.LastName.UppercaseWords();
          courseDto.TeacherFirstName = course.Teacher.FirstName.UppercaseWords();
          scheduleItem.Courses.Add(courseDto);
        }

        classCourses.Add(scheduleItem);
      }

      return classCourses;
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
        .Include(i => i.Session.Course)
        .Where(a => a.Session.ClassId == classId && a.Session.SessionDate.Date >= StartDate.Date &&
          a.Session.SessionDate.Date <= EndDate.Date)
        .OrderBy(o => o.Session.SessionDate).ToListAsync();
    }

    public async Task<IEnumerable<Agenda>> GetClassAgendaTodayToNDays(int classId, int toNbDays)
    {
      List<Agenda> agendas = await _cache.GetAgendas();
      DateTime today = DateTime.Now.Date;
      DateTime EndDate = today.AddDays(toNbDays).Date;

      return agendas.Where(a => a.Session.ClassId == classId &&
                            a.Session.SessionDate.Date >= today && a.Session.SessionDate.Date <= EndDate)
                    .OrderBy(o => o.Session.SessionDate).ToList();
    }

    public async Task<IEnumerable<User>> GetClassStudents(int classId)
    {
      // List<User> users = await _cache.GetUsers();
      return await _context.Users.Where(u => u.ClassId == classId && u.UserTypeId == studentTypeId)
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
      // List<User> users = await _cache.GetUsers();
      return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == email.ToUpper());
    }

    public async Task<User> GetUserByEmailAndLogin(string username, string email)
    {
      // List<User> users = await _cache.GetUsers();
      return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == email.ToUpper() &&
        u.UserName.ToUpper() == username.ToUpper());
    }

    public async Task<Session> GetSessionFromSchedule(ScheduleCourse course, DateTime sessionDate)
    {
      List<Schedule> scheduleCourses = await _cache.GetSchedules();
      List<Session> sessions = await _cache.GetSessions();

      // get session by schedule and date
      var sessionFromDB = sessions.FirstOrDefault(s => s.ScheduleCourseId == course.Id && s.SessionDate.Date == sessionDate);
      if (sessionFromDB != null)
      {
        return (sessionFromDB);
      }
      else
      {
        var newSession = new Session
        {
          ScheduleCourseId = course.Id,
          TeacherId = Convert.ToInt32(course.TeacherId),
          ClassId = course.Schedule.ClassId,
          CourseId = course.CourseId,
          StartHourMin = course.Schedule.StartHourMin,
          EndHourMin = course.Schedule.EndHourMin,
          SessionDate = sessionDate
        };

        _context.Add(newSession);

        if (await SaveAll())
        {
          return newSession;
        }

        return null;
      }
    }

    public async Task<IEnumerable<Agenda>> GetClassAgenda(int classId)
    {
      List<Agenda> agendas = await _cache.GetAgendas();

      return agendas.Where(a => a.Session.ClassId == classId)
                    .OrderBy(o => o.Session.SessionDate).ToList();
    }

    // public async Task<IEnumerable<User>> GetStudentsForClass(int classId)
    // {
    //   List<User> students = await _cache.GetStudents();

    //   return students.Where(u => u.ClassId == classId)
    //                  .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
    //                  .ToList();
    // }

    public async Task<IEnumerable<UserType>> getUserTypes()
    {
      List<UserType> userTypes = await _cache.GetUserTypes();
      return userTypes.Where(u => u.Name != "Admin").ToList();
    }

    public async Task<bool> EmailExist(string email)
    {
      List<User> users = await _cache.GetUsers();
      var user = users.FirstOrDefault(e => e.NormalizedEmail == email.ToUpper());

      if (user != null)
        return true;

      return false;
    }

    public async Task<List<Course>> GetTeacherCourses(int teacherId)
    {
      List<TeacherCourse> teacherCourses = await _cache.GetTeacherCourses();
      var courses = teacherCourses.Where(c => c.TeacherId == teacherId)
                                  .Select(s => s.Course).ToList();
      return courses;
    }

    public async Task<List<TeacherClassesDto>> GetTeacherClasses(int teacherId)
    {
      List<User> users = await _cache.GetStudents();
      List<Class> classesCached = await _cache.GetClasses();
      List<ClassCourse> classesCourses = await _cache.GetClassCourses();

      var classesData = (from courses in classesCourses
                         join classes in classesCached
on courses.ClassId equals classes.Id
                         where courses.TeacherId == teacherId
                         select new
                         {
                           ClassId = classes.Id,
                           ClassLevelId = classes.ClassLevelId,
                           ClassName = classes.Name,
                           NbStudents = users.Where(u => u.ClassId == classes.Id).Count()
                         })
        .OrderBy(o => o.ClassName)
        .Distinct().ToList();

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

    public async Task<List<NextCoursesByClassDto>> GetNextCoursesByClass(int teacherId)
    {
      List<User> teachers = await _cache.GetTeachers();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();

      var nextCourses = 10; // next coming courses
      var today = DateTime.Now;
      // monday=1, tue=2, ...
      var todayDay = ((int)today.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;
      var todayHourMin = today.TimeOfDay;

      var teacher = teachers.First(u => u.Id == teacherId);
      var teacherClasses = await GetTeacherClasses(teacherId);
      List<NextCoursesByClassDto> nextCoursesByClass = new List<NextCoursesByClassDto>();
      foreach (var aclass in teacherClasses)
      {
        NextCoursesByClassDto coursesByClass = new NextCoursesByClassDto();
        coursesByClass.TeacherId = teacherId;
        coursesByClass.TeacherName = teacher.LastName + ' ' + teacher.FirstName;
        coursesByClass.ClassId = aclass.ClassId;
        coursesByClass.ClassName = aclass.ClassName;
        coursesByClass.Courses = new List<ClassNextCoursesDto>();

        var teacherCourses = scheduleCourses
          .Where(w => w.TeacherId == teacherId && w.Schedule.ClassId == aclass.ClassId && w.Schedule.Day == todayDay)
          .OrderBy(o => o.Schedule.StartHourMin)
          .Distinct().Take(nextCourses).ToList();

        foreach (var course in teacherCourses)
        {
          Schedule schedule = course.Schedule;
          ClassNextCoursesDto cncd = new ClassNextCoursesDto();
          cncd.ScheduleId = course.ScheduleId;
          cncd.ClassId = schedule.ClassId;
          cncd.ClassName = schedule.Class.Name;
          cncd.CourseId = course.CourseId;
          cncd.CourseName = course.Course.Name;
          cncd.Day = schedule.Day;
          cncd.CourseStartHM = schedule.StartHourMin;
          cncd.CourseEndHM = schedule.EndHourMin;
          cncd.StartHourMin = schedule.StartHourMin.ToString("hh:MM", frC);
          cncd.EndHourMin = schedule.EndHourMin.ToString("hh:MM", frC);
          coursesByClass.Courses.Add(cncd);
        }
        nextCoursesByClass.Add(coursesByClass);
      }

      return nextCoursesByClass;
    }

    public async Task<List<ClassesWithEvalsDto>> GetTeacherClassesWithEvalsByPeriod(int teacherId, int periodId)
    {
      // List<User> studentsCached = await _cache.GetStudents();
      // List<ClassCourse> classCoursesCached = await _cache.GetClassCourses();
      var Classes = await _context.ClassCourses.Where(c => c.TeacherId == teacherId).Distinct().ToListAsync();

      List<ClassesWithEvalsDto> classesWithEvals = new List<ClassesWithEvalsDto>();
      foreach (var aclass in Classes)
      {
        List<Evaluation> ClassEvals = await _context.Evaluations
          .Include(i => i.Course)
          .Include(i => i.EvalType)
          .Where(e => e.ClassId == aclass.ClassId && e.PeriodId == periodId)
          .ToListAsync();

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
          classDto.NbStudents = await _context.Users.Where(s => s.ClassId == aclass.ClassId).CountAsync();
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
        .Where(e => e.ClassId == classId && e.EvalDate.Date > today).ToListAsync();
      var evalsToReturn = _mapper.Map<List<EvaluationForListDto>>(evals);
      return evalsToReturn;
    }

    public async Task<bool> UpdateChildren(ChildrenForEditDto users)
    {
      List<User> usersCached = await _cache.GetUsers();
      List<EmailTemplate> emailTemplates = await _cache.GetEmailTemplates();

      bool resultStatus = false;
      using (var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          for (int i = 0; i < users.Id.Count(); i++)
          {
            var childid = users.Id[i];
            var child = usersCached.First(u => u.Id == childid);
            child.UserName = users.UserName[i];
            child.NormalizedUserName = users.UserName[i].ToUpper();
            child.LastName = users.LastName[i];
            child.FirstName = users.FirstName[i];
            child.Gender = users.Gender[i];
            var dateArray = users.strDateOfBirth[i].Split("/");
            var day = Convert.ToInt32(dateArray[0]);
            var month = Convert.ToInt32(dateArray[1]);
            var year = Convert.ToInt32(dateArray[2]);
            child.DateOfBirth = new DateTime(year, month, day);
            child.Email = users.Email[i];
            if (child.NormalizedEmail != null)
              child.NormalizedEmail = users.Email[i].ToUpper();
            child.PhoneNumber = users.PhoneNumber[i];
            var pwd = users.Password[i];
            // validate user
            var newPassword = _userManager.PasswordHasher.HashPassword(child, users.Password[i]);
            child.PasswordHash = newPassword;
            if (child.Email != "" && child.Email != null)
              child.EmailConfirmed = true;
            child.AccountDataValidated = true;
            Update(child);
            resultStatus = true;

            //does the current user has photo?
            if (users.PhotoIndex != null)
            {
              var photoIndex = users.PhotoIndex.IndexOf(childid);
              if (photoIndex != -1)
              {
                var photoFile = users.PhotoFiles[photoIndex];
                if (photoFile.Length > 0)
                {
                  var uploadResult = new ImageUploadResult();
                  using (var stream = photoFile.OpenReadStream())
                  {
                    var uploadParams = new ImageUploadParams()
                    {
                      File = new FileDescription(photoFile.Name, stream),
                      Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                    if (uploadResult.StatusCode == HttpStatusCode.OK)
                    {
                      //remove tag 'Main' from child photo if it exists
                      if (child.Photos.Any(u => u.IsMain))
                      {
                        var oldPhoto = await _context.Photos.FirstAsync(p => p.UserId == child.Id && p.IsMain == true);
                        oldPhoto.IsMain = false;
                        Update(oldPhoto);
                      }
                      Photo photo = new Photo();
                      photo.Url = uploadResult.SecureUri.ToString();
                      photo.PublicId = uploadResult.PublicId;
                      photo.UserId = child.Id;
                      photo.DateAdded = DateTime.Now;
                      photo.IsMain = true;
                      photo.IsApproved = true;
                      Add(photo);
                    }
                  }
                }
              }
              else
              {
                resultStatus = true;
              }
            }
          }

          User parent = usersCached.First(u => u.Id == users.ParentId);
          parent.Validated = true;
          Update(parent);

          var template = emailTemplates.First(t => t.Id == updateAccountEmailId);
          var email = await SetEmailForAccountUpdated(template.Subject, template.Body,
            parent.LastName, parent.Gender, parent.Email, parent.Id);
          Add(email);

          if (await SaveAll())
          {
            // validate transaction
            identityContextTransaction.Commit();
            await _cache.LoadUsers();
            resultStatus = true;
          }
          else
            resultStatus = false;
        }
        catch (System.Exception ex)
        {
          var sde = ex.Message;
          identityContextTransaction.Rollback();
          resultStatus = false;
        }
      }
      return resultStatus;
    }

    public async Task<bool> EditUserAccount(UserAccountForEditDto user)
    {
      List<User> users = await _cache.GetUsers();
      bool resultStatus = false;
      using (var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          User appUser = users.FirstOrDefault(u => u.Id == user.Id);
          appUser.LastName = user.LastName;
          appUser.FirstName = user.FirstName;
          appUser.Gender = user.Gender;
          var dateArray = user.strDateOfBirth.Split("/");
          int year = Convert.ToInt32(dateArray[2]);
          int month = Convert.ToInt32(dateArray[1]);
          int day = Convert.ToInt32(dateArray[0]);
          DateTime birthDay = new DateTime(year, month, day);
          appUser.DateOfBirth = birthDay;
          appUser.SecondPhoneNumber = user.SecondPhoneNumber;
          if (user.CityId > 0)
            appUser.CityId = user.CityId;
          if (user.DistrictId > 0)
            appUser.DistrictId = user.DistrictId;

          //add user photo
          var photoFile = user.PhotoFile;
          if (photoFile != null)
          {
            if (photoFile.Length > 0)
            {
              var uploadResult = new ImageUploadResult();
              using (var stream = photoFile.OpenReadStream())
              {
                var uploadParams = new ImageUploadParams()
                {
                  File = new FileDescription(photoFile.Name, stream),
                  Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };
                var subdomain = GetAppSubDomain();
                if (subdomain != "")
                  uploadParams.Folder = subdomain + "/";
                else
                  uploadParams.Folder = "localDemo/";

                uploadResult = _cloudinary.Upload(uploadParams);
                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                  Photo photo = new Photo();
                  photo.Url = uploadResult.SecureUri.ToString();
                  photo.PublicId = uploadResult.PublicId;
                  photo.UserId = appUser.Id;
                  photo.DateAdded = DateTime.Now;
                  if (appUser.Photos.Any(u => u.IsMain))
                  {
                    var oldPhoto = await _context.Photos.FirstAsync(p => p.UserId == user.Id && p.IsMain == true);
                    oldPhoto.IsMain = false;
                    Update(oldPhoto);
                  }
                  photo.IsMain = true;
                  photo.IsApproved = true;
                  Add(photo);
                }
                else
                {
                  resultStatus = false;
                }
              }
            }
          }
          else
          {
            resultStatus = true;
          }

          if (photoFile != null && await SaveAll())
          {
            var result = await _userManager.UpdateAsync(appUser);
            if (result.Succeeded)
            {
              // fin de la transaction
              identityContextTransaction.Commit();
              await _cache.LoadUsers();
              resultStatus = true;
            }
            else
            {
              identityContextTransaction.Rollback();
              resultStatus = false;
            }
          }
          else
          {
            var result = await _userManager.UpdateAsync(appUser);
            if (result.Succeeded)
            {
              // fin de la transaction
              identityContextTransaction.Commit();
              await _cache.LoadUsers();
              resultStatus = true;
            }
            else
            {
              identityContextTransaction.Rollback();
              resultStatus = false;
            }
          }
        }
        catch
        {
          identityContextTransaction.Rollback();
          return resultStatus = false;
        }
      }
      return resultStatus;
    }

    public async Task<bool> AddEmployee(EmployeeForEditDto user)
    {
      // List<User> employees = await _cache.GetEmployees();
      // List<UserRole> userRoles = await _cache.GetUserRoles();

      // bool resultStatus = true;
      using (var identityContextTransaction = _context.Database.BeginTransaction())
      {
        string publicId = "";
        Boolean photoExists = false;
        try
        {
          User appUser = new User();

          var dateArray = user.strDateOfBirth.Split("/");
          int year = Convert.ToInt32(dateArray[2]);
          int month = Convert.ToInt32(dateArray[1]);
          int day = Convert.ToInt32(dateArray[0]);
          DateTime birthDay = new DateTime(year, month, day);

          //is it a new user
          if (user.Id == 0)
          {
            var userToSave = _mapper.Map<User>(user);
            string strDoB = user.strDateOfBirth;
            userToSave.DateOfBirth = birthDay;
            var code = Guid.NewGuid();
            userToSave.UserName = code.ToString();
            userToSave.Validated = false;
            userToSave.EmailConfirmed = false;

            var result = await _userManager.CreateAsync(userToSave, password);
            if (result.Succeeded)
            {
              appUser = await _userManager.Users.Include(i => i.Photos)
                                                .FirstOrDefaultAsync(u => u.NormalizedUserName == userToSave.UserName.ToUpper());
              appUser.IdNum = GetUserIDNumber(appUser.Id, appUser.LastName, appUser.FirstName);
              await _userManager.UpdateAsync(appUser);
            }
          }
          else
          {
            appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            appUser.LastName = user.LastName;
            appUser.FirstName = user.FirstName;
            appUser.Gender = user.Gender;
            appUser.DateOfBirth = birthDay;
            appUser.PhoneNumber = user.PhoneNumber;
            appUser.SecondPhoneNumber = user.SecondPhoneNumber;
            appUser.Email = user.Email;
            appUser.DistrictId = user.DistrictId;
            appUser.MaritalStatusId = user.MaritalStatusId;
            Update(appUser);

            //delete previous employee roles
            List<UserRole> prevRoles = await _context.UserRoles.Where(c => c.UserId == appUser.Id).ToListAsync();
            DeleteAll(prevRoles);
          }

          if (user.Roles != null)
          {
            List<string> rolesList = user.Roles.Split(",").ToList();
            if (rolesList.Count() > 0)
            {
              List<Role> roles = await _context.Roles.ToListAsync();
              IEnumerable<string> roleNames = roles.Where(r => rolesList.Contains(r.Name)).Select(r => r.Name);
              var result = _userManager.AddToRolesAsync(appUser, roleNames);
              if (!result.Result.Succeeded)
              {
                identityContextTransaction.Rollback();
                return false;
              }
            }
          }

          //add user photo
          var photoFile = user.PhotoFile;
          if (photoFile != null)
          {
            if (photoFile.Length > 0)
            {
              photoExists = true;
              var uploadResult = new ImageUploadResult();
              using (var stream = photoFile.OpenReadStream())
              {
                var uploadParams = new ImageUploadParams()
                {
                  File = new FileDescription(photoFile.Name, stream),
                  Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };

                uploadResult = _cloudinary.Upload(uploadParams);
                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                  publicId = uploadResult.PublicId;

                  Photo photo = new Photo();
                  photo.UserId = appUser.Id;
                  photo.Url = uploadResult.SecureUri.ToString();
                  photo.PublicId = uploadResult.PublicId;
                  photo.DateAdded = DateTime.Now;
                  if (appUser.Photos.Any(u => u.IsMain))
                  {
                    var oldPhoto = await _context.Photos.FirstAsync(p => p.UserId == appUser.Id && p.IsMain == true);
                    oldPhoto.IsMain = false;
                    Update(oldPhoto);
                  }
                  photo.IsMain = true;
                  photo.IsApproved = true;
                  Add(photo);
                }
                // else
                // {
                //   identityContextTransaction.Rollback();
                //   return false;
                // }
              }
            }
          }

          // send the mail to update userName/pwd - add to Email table
          if (appUser.Email != null)
          {
            ConfirmEmployeeEmailDto emailData = new ConfirmEmployeeEmailDto()
            {
              Id = appUser.Id,
              LastName = appUser.LastName,
              FirstName = appUser.FirstName,
              Cell = appUser.PhoneNumber,
              Gender = appUser.Gender,
              Email = appUser.Email,
              Token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser)
            };

            List<EmailTemplate> emailTemplates = await _cache.GetEmailTemplates();
            var template = emailTemplates.First(t => t.Id == employeeConfirmEmailId);
            Email emailToSend = await SetDataForConfirmEmployeeEmail(emailData, template.Body, template.Subject);
            Add(emailToSend);
          }

          await SaveAll();
          identityContextTransaction.Commit();
          return true;
        }
        catch (Exception ex)
        {
          var dd = ex.Message;
          identityContextTransaction.Rollback();
          if(photoExists)
            DeletePhotoFromCloudinary(publicId);
          return false;
        }
        finally
        {
          await _cache.LoadUsers();
          await _cache.LoadUserRoles();
          await _cache.LoadPhotos();
        }
      }
    }

    public async Task<bool> AddTeacher(TeacherForEditDto user)
    {
      List<User> teachers = await _cache.GetTeachers();
      List<TeacherCourse> teacherCourses = await _cache.GetTeacherCourses();
      List<Course> coursesCached = await _cache.GetCourses();
      List<EmailTemplate> emailTemplates = await _cache.GetEmailTemplates();

      bool resultStatus = false;
      using (var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          var ids = user.CourseIds.Split(",").ToList();
          User appUser = new User();

          //is it a new user
          if (user.Id == 0)
          {
            var userToSave = _mapper.Map<User>(user);
            string strDoB = user.strDateOfBirth;
            var dateArray = user.strDateOfBirth.Split("/");
            int year = Convert.ToInt32(dateArray[2]);
            int month = Convert.ToInt32(dateArray[1]);
            int day = Convert.ToInt32(dateArray[0]);
            DateTime birthDay = new DateTime(year, month, day);
            userToSave.DateOfBirth = birthDay;
            var code = Guid.NewGuid();
            userToSave.UserName = code.ToString();
            userToSave.Validated = false;
            userToSave.EmailConfirmed = false;
            if (userToSave.ClassId == 0)
              userToSave.ClassId = null;

            var result = await _userManager.CreateAsync(userToSave, password);
            var teacherCode = "";
            if (result.Succeeded)
            {
              // enregistrement du RoleTeacher
              appUser = await _userManager.Users.Include(i => i.Photos)
                                                .FirstOrDefaultAsync(u => u.NormalizedUserName == userToSave.UserName);

              appUser.IdNum = GetUserIDNumber(appUser.Id, appUser.LastName, appUser.FirstName);
              teacherCode = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
              Update(appUser);

              var courses = coursesCached.Where(c => ids.Contains(c.Id.ToString())).ToList();
              var coursesName = courses.Select(s => s.Name).ToList();
              string coursenames = "";
              foreach (var name in coursesName)
              {
                if (coursenames == "")
                  coursenames = name;
                else
                  coursenames = "," + name;
              }

              // send the mail to update userName/pwd - add to Email table
              if (appUser.Email != null)
              {
                ConfirmTeacherEmailDto emailData = new ConfirmTeacherEmailDto()
                {
                  Id = appUser.Id,
                  LastName = appUser.LastName,
                  FirstName = appUser.FirstName,
                  Cell = appUser.PhoneNumber,
                  Gender = appUser.Gender,
                  Email = appUser.Email,
                  Token = teacherCode,
                  Courses = coursenames
                };

                var template = emailTemplates.First(t => t.Id == teacherConfirmEmailId);
                Email emailToSend = await SetDataForConfirmTeacherEmail(emailData, template.Body, template.Subject);
                Add(emailToSend);
              }
            }
          }
          else
          {
            appUser = teachers.FirstOrDefault(u => u.Id == user.Id);
            appUser.LastName = user.LastName;
            appUser.FirstName = user.FirstName;
            appUser.Gender = user.Gender;
            var dateArray = user.strDateOfBirth.Split("/");
            int year = Convert.ToInt32(dateArray[2]);
            int month = Convert.ToInt32(dateArray[1]);
            int day = Convert.ToInt32(dateArray[0]);
            DateTime birthDay = new DateTime(year, month, day);
            appUser.DateOfBirth = birthDay;
            appUser.EducLevelId = user.EducLevelId;
            if (user.ClassId != 0)
              appUser.ClassId = user.ClassId;
            appUser.PhoneNumber = user.PhoneNumber;
            appUser.SecondPhoneNumber = user.SecondPhoneNumber;
            appUser.Email = user.Email;
            Update(appUser);

            //delete previous teacher courses
            List<TeacherCourse> prevCourses = teacherCourses.Where(c => c.TeacherId == appUser.Id).ToList();
            DeleteAll(prevCourses);
          }

          if (ids.Count() > 0)
          {
            foreach (var courseId in ids)
            {
              // add new selected courses
              TeacherCourse tc = new TeacherCourse();
              tc.CourseId = Convert.ToInt32(courseId);
              tc.TeacherId = appUser.Id;
              Add(tc);

              // add class courses if it's a primary class
              if (user.EducLevelId == 1)
              {
                ClassCourse cc = new ClassCourse();
                cc.ClassId = Convert.ToInt32(appUser.ClassId);
                cc.TeacherId = appUser.Id;
                cc.CourseId = Convert.ToInt32(courseId);
                Add(cc);
              }
            }
          }

          //add user photo
          var photoFile = user.PhotoFile;
          if (photoFile != null)
          {
            if (photoFile.Length > 0)
            {
              var uploadResult = new ImageUploadResult();
              using (var stream = photoFile.OpenReadStream())
              {
                var uploadParams = new ImageUploadParams()
                {
                  File = new FileDescription(photoFile.Name, stream),
                  Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };

                uploadResult = _cloudinary.Upload(uploadParams);
                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                  Photo photo = new Photo();
                  photo.Url = uploadResult.SecureUri.ToString();
                  photo.PublicId = uploadResult.PublicId;
                  photo.UserId = appUser.Id;
                  photo.DateAdded = DateTime.Now;
                  if (appUser.Photos.Any(u => u.IsMain))
                  {
                    var oldPhoto = await _context.Photos.FirstAsync(p => p.UserId == user.Id && p.IsMain == true);
                    oldPhoto.IsMain = false;
                    Update(oldPhoto);
                  }
                  photo.IsMain = true;
                  photo.IsApproved = true;
                  Add(photo);
                }
              }
            }
          }
          else
          {
            resultStatus = true;
          }

          if (await SaveAll())
          {
            identityContextTransaction.Commit();
            await _cache.LoadUsers();
            await _cache.LoadTeacherCourses();
            await _cache.LoadCourses();
            await _cache.LoadClassCourses();
            //fin de la transaction
            resultStatus = true;
          }
          else
            resultStatus = false;
        }
        catch
        {
          identityContextTransaction.Rollback();
          return resultStatus = false;
        }
      }
      return resultStatus;
    }

    public async Task<bool> AddPhoto(int userId, IFormFile photoFile)
    {
      if (photoFile.Length > 0)
      {
        var uploadResult = new ImageUploadResult();
        var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(a => a.Id == userId);
        using (var stream = photoFile.OpenReadStream())
        {
          var uploadParams = new ImageUploadParams()
          {
            File = new FileDescription(photoFile.Name, stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
          };

          uploadResult = _cloudinary.Upload(uploadParams);
          if (uploadResult.StatusCode == HttpStatusCode.OK)
          {
            Photo photo = new Photo();
            photo.Url = uploadResult.SecureUri.ToString();
            photo.PublicId = uploadResult.PublicId;
            photo.UserId = userId;
            photo.DateAdded = DateTime.Now;
            if (user.Photos.Any(u => u.IsMain))
            {
              var oldPhoto = await _context.Photos.FirstAsync(p => p.UserId == user.Id && p.IsMain == true);
              oldPhoto.IsMain = false;
              Update(oldPhoto);
            }
            photo.IsMain = true;
            photo.IsApproved = true;
            Add(photo);

            if (await SaveAll())
              return true;
            else
              return false;
          }
        }
      }
      return true;
    }

    public async Task<Course> GetCourse(int Id)
    {
      return await _context.Courses.FirstOrDefaultAsync(c => c.Id == Id);
    }

    public async Task<Boolean> SendTeacherConfirmEmail(int userId)
    {
      List<User> teachers = await _cache.GetTeachers();
      List<EmailTemplate> templates = await _cache.GetEmailTemplates();

      var appUser = teachers.FirstOrDefault(u => u.Id == userId);
      var teacherCode = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

      ConfirmTeacherEmailDto emailData = new ConfirmTeacherEmailDto()
      {
        Id = appUser.Id,
        LastName = appUser.LastName,
        FirstName = appUser.FirstName,
        Cell = appUser.PhoneNumber,
        Gender = appUser.Gender,
        Email = appUser.Email,
        Token = teacherCode
      };

      var template = templates.First(t => t.Id == teacherConfirmEmailId);
      Email emailToSend = await SetDataForConfirmTeacherEmail(emailData, template.Body, template.Subject);
      Add(emailToSend);

      if (!await SaveAll())
        return false;

      return true;
    }

    public async Task<bool> SendResetPasswordLink(User user, string token)
    {
      var template = await _context.EmailTemplates.FirstAsync(t => t.Id == resetPwdEmailId);
      var email = await SetEmailForResetPwdLink(template.Subject, template.Body, user.Id,
        user.LastName, user.FirstName, user.Gender, user.Email, token);
      _context.Add(email);

      if (!await SaveAll())
        return false;
      else
        return true;
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

    // private string ResetPasswordContent(string code)
    // {
    //     return "<b>EducNotes</b> a bien enrgistré votre demande de réinitialisation de mot de passe !<br>" +
    //         "Vous pouvez utiliser le lien suivant pour réinitialiser votre mot de passe: <br>" +
    //         " <a href=" + _config.GetValue<String>("AppSettings:ResetPasswordLink") + code + "/>cliquer ici</a><br>" +
    //         "Si vous n'utilisez pas ce lien dans les 3 heures, il expirera." +
    //         "Pour obtenir un nouveau lien de réinitialisation de mot de passe, visitez" +
    //         " <a href=" + _config.GetValue<String>("AppSettings:DefaultforgotPasswordLink") + "/>réinitialiser son mot de passe</a>.<br>" +
    //         "Merci,";
    // }

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

    // public void AddInscription(int levelId, int userId)
    // {
    //     var nouvelle_incrpition = new Inscription
    //     {
    //         InsertDate = DateTime.Now,
    //         ClassLevelId = levelId,
    //         UserId = userId,
    //         Validated = false
    //     };
    //     Add(nouvelle_incrpition);
    // }

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
      return "<h3><span>Educt'Notes</span></h3> <br>" + "bonjour <b>" + userName + ",</b>" +
        "<p>Merci de bien vouloir valider votre compte au lien suivant :" +
        " <a href=" + _config.GetValue<String>("AppSettings:DefaultEmailValidationLink") + code + " /> cliquer ici</a></p> <br>";
    }

    public IEnumerable<ClassAgendaToReturnDto> GetAgendaListByDueDate(IEnumerable<Agenda> agendaItems)
    {
      //select all distinct dates
      var dueDates = agendaItems.OrderBy(o => o.Session.SessionDate).Select(e => e.Session.SessionDate).Distinct();

      var agendasToReturn = new List<ClassAgendaToReturnDto>();
      foreach (var currDate in dueDates)
      {
        var currentDateAgendas = agendaItems.Where(e => e.Session.SessionDate == currDate);
        var agenda = new ClassAgendaToReturnDto();
        agenda.dtDueDate = currDate;
        agenda.DueDate = currDate.ToLongDateString();
        agenda.ShortDueDate = currDate.ToString("ddd dd MMM", frC);
        agenda.DueDateDay = currDate.Day;
        agenda.NbTasks = currentDateAgendas.Count();
        var dayInt = (int)currDate.DayOfWeek;
        agenda.DayInt = dayInt == 0 ? 7 : dayInt;
        agenda.Courses = new List<CourseTask>();
        foreach (var a in currentDateAgendas)
        {
          agenda.Courses.Add(new CourseTask
          {
            CourseId = a.Session.Course.Id,
            CourseName = a.Session.Course.Name,
            CourseColor = a.Session.Course.Color,
            TaskDesc = a.TaskDesc,
            DateAdded = a.DateAdded.ToLongDateString(),
            ShortDateAdded = a.DateAdded.ToString("dd/MM/yyyy", frC)
          });
        }

        agendasToReturn.Add(agenda);
      }

      return agendasToReturn;
    }

    public async Task<IEnumerable<ClassType>> GetClassTypes()
    {
      List<ClassType> classTypesCached = await _cache.GetClassTypes();
      List<ClassType> classTypes = classTypesCached.OrderBy(c => c.Name).ToList();
      return classTypes;
    }

    public async Task<List<string>> GetEmails()
    {
      return await _context.Users.Where(a => a.Email != null).Select(a => a.Email).ToListAsync();
    }

    public async Task<List<string>> GetUserNames()
    {
      return await _context.Users.Where(a => a.UserName != null).Select(a => a.UserName).ToListAsync();
    }

    public async Task<List<Period>> GetPeriods()
    {
      return await _context.Periods.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<List<ClassLevel>> GetLevels()
    {
      return await _context.ClassLevels.OrderBy(c => c.DsplSeq).ToListAsync();
    }

    public async Task<bool> UserNameExist(string userName, int currentUserId)
    {
      List<User> users = await _cache.GetUsers();

      var user = users.Where(u => u.Id != currentUserId)
                      .FirstOrDefault(e => e.UserName.ToLower() == userName.ToLower());

      if (user != null)
      {
        return true;
      }
      return false;
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
          var email = new Email();
          email.InsertDate = DateTime.Now;
          email.InsertUserId = userId;
          email.UpdateUserId = userId;
          email.StatusFlag = 0;
          email.Subject = "Confirmation mise en salle de votre enfant";
          email.ToAddress = parent.Email;
          email.Body = "<b> Bonjour " + parent.LastName + " " + parent.FirstName + "</b>, <br>" +
            "Votre enfant <b>" + student.LastName + " " + student.FirstName +
            " </b> a bien été enregistré(s) dans la classe de <b>" + student.Class.Name;
          email.FromAddress = "no-reply@educnotes.com";
          email.EmailTypeId = _config.GetValue<int>("AppSettings:confirmedEmailtypeId");
          email.ToUserId = parent.Id;
          Add(email);
        }

      }
    }

    public async Task<List<UserSpaCodeDto>> ParentSelfInscription(int parentId, List<UserForUpdateDto> userToUpdate)
    {
      var usersSpaCode = new List<UserSpaCodeDto>();
      var children = await _context.UserLinks.Where(u => u.UserPId == parentId).Select(u => u.User).ToListAsync();
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
              parentFromRepo.Validated = true;
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
              child.Validated = true;
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
        catch(System.Exception)
        {
          identityContextTransaction.Rollback();
          usersSpaCode = new List<UserSpaCodeDto>();
        }
      }
      return usersSpaCode;
    }

    public async Task<List<Course>> GetUserCourses(int classId)
    {
      var userCourses = await (from course in _context.ClassCourses join user in _context.Users on course.ClassId equals user.ClassId where user.ClassId == classId orderby course.Course.Name select course.Course).Distinct().ToListAsync();
      return userCourses;
    }

    public async Task<List<UserCourseEvalsDto>> GetUserGrades(int userId, int classId)
    {
      //get user courses
      var userCourses = await GetUserCourses(classId);
      var aclass = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);
      var periods = await _context.Periods.OrderBy(p => p.StartDate).ToListAsync();
      Period currPeriod = await GetPeriodFromDate(DateTime.Now);
      List<UserCourseEvalsDto> coursesWithEvals = new List<UserCourseEvalsDto>();

      //loop on user courses - get data for each course 
      for (int i = 0; i < userCourses.Count(); i++)
      {
        var acourse = userCourses[i];

        //get all evaluations of the selected course and current userId
        var userEvals = await _context.UserEvaluations
          .Include(e => e.Evaluation).ThenInclude(e => e.EvalType)
          .Include(e => e.Evaluation).ThenInclude(e => e.Course)
          .OrderBy(o => o.Evaluation.EvalDate)
          .Where(e => e.UserId == userId && e.Evaluation.GradeInLetter == false &&
            e.Evaluation.CourseId == acourse.Id && e.Evaluation.Graded == true && e.Grade.IsNumeric())
          .Distinct().ToListAsync();

        // get general Evals data for the current user course
        UserCourseEvalsDto userEvalsDto = await GetUserCourseEvals(userEvals, acourse, aclass);

        // get evals by period for the current user course
        userEvalsDto.PeriodEvals = new List<PeriodEvalsDto>();
        foreach (var period in periods)
        {
          PeriodEvalsDto ped = new PeriodEvalsDto();
          ped.PeriodId = period.Id;
          ped.PeriodName = period.Name;
          ped.PeriodAbbrev = period.Abbrev;
          if (currPeriod != null && currPeriod.Id == period.Id)
            ped.Active = true;
          else
            ped.Active = false;

          var userPeriodEvals = userEvals.Where(e => e.Evaluation.PeriodId == period.Id).ToList();
          if (userPeriodEvals.Count() > 0)
          {
            UserCourseEvalsDto periodEvals = await GetUserCourseEvals(userPeriodEvals, acourse, aclass);
            ped.UserCourseAvg = periodEvals.UserCourseAvg;
            ped.ClassCourseAvg = periodEvals.ClassCourseAvg;
            ped.grades = periodEvals.Grades;
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

    public async Task<ClassAvgDto> GetClassAvgs(int classId)
    {
      double avgMin = 1000;
      double avgMax = -1000;
      var students = await GetClassStudents(classId);
      foreach (var student in students)
      {
        var avg = await GetStudentAvg(student.Id, Convert.ToInt32(student.ClassId));
        if (avg > avgMax)
          avgMax = avg;
        if (avg < avgMin)
          avgMin = avg;
      }

      ClassAvgDto classAvgs = new ClassAvgDto();
      classAvgs.ClassAvgMin = avgMin;
      classAvgs.ClassAvgMax = avgMax;

      return classAvgs;
    }

    public async Task<double> GetStudentCourseAvg(int courseId, int userId)
    {
      //get all grades of the selected course and current userId
      var evals = await _context.UserEvaluations
        .Include(e => e.Evaluation)
        .Where(e => e.UserId == userId && e.Evaluation.GradeInLetter == false &&
          e.Evaluation.CourseId == courseId && e.Evaluation.Graded == true && e.Grade.IsNumeric())
        .Distinct().ToListAsync();
      double gradeSum = 0;
      double coeffSum = 0;
      double courseAvg = -1000;
      if (evals.Count() > 0)
      {
        foreach (var eval in evals)
        {
          gradeSum += Convert.ToDouble(eval.Grade) * eval.Evaluation.Coeff;
          coeffSum += eval.Evaluation.Coeff;
        }

        courseAvg = Math.Round(gradeSum / coeffSum, 2);
      }

      return courseAvg;
    }

    public async Task<List<GradeDto>> GetStudentLastGrades(int userId, int nbOfGrades)
    {
      var lastGradesFromDB = await _context.UserEvaluations
        .Include(i => i.Evaluation).ThenInclude(i => i.EvalType)
        .Include(i => i.Evaluation).ThenInclude(i => i.Course)
        .OrderByDescending(o => o.Evaluation.EvalDate)
        .Where(e => e.UserId == userId && e.Grade.IsNumeric() && e.Evaluation.Closed)
        .Take(nbOfGrades)
        .ToListAsync();
      var lastGrades = _mapper.Map<List<GradeDto>>(lastGradesFromDB);
      for (int i = 0; i < lastGrades.Count(); i++)
      {
        var grade = lastGrades[i];
        int evalId = lastGradesFromDB[i].EvaluationId;
        var gradeMinMax = await GetEvalMinMax(evalId);
        // var gradeMax = await GetEvalMax(evalId);
        grade.ClassGradeMin = gradeMinMax[0];
        grade.ClassGradeMax = gradeMinMax[1];
        grade.GradeOK = Convert.ToDouble(grade.Grade) >= Convert.ToDouble(grade.GradeMax) / 2;
      }

      return lastGrades;
    }

    public async Task<double> GetStudentAvg(int userId, int classId)
    {
      var userCourses = await GetUserCourses(classId);
      var userClass = await GetClass(classId);

      double courseAvgSum = 0;
      double courseCoeffSum = 0;
      double GeneralAvg = -1000;
      List<CourseAvgDto> coursesAvg = new List<CourseAvgDto>();
      if (userCourses.Count() > 0)
      {
        foreach (var course in userCourses)
        {
          var userEvals = await _context.UserEvaluations
            .Include(e => e.Evaluation)
            .OrderBy(o => o.Evaluation.EvalDate)
            .Where(e => e.UserId == userId && e.Evaluation.GradeInLetter == false &&
              e.Evaluation.CourseId == course.Id && e.Evaluation.Graded == true && e.Grade.IsNumeric())
            .Distinct().ToListAsync();

          double gradesSum = 0;
          double coeffSum = 0;
          foreach (var userEval in userEvals)
          {
            if (userEval.Grade.IsNumeric())
            {
              double maxGrade = Convert.ToDouble(userEval.Evaluation.MaxGrade);
              double gradeValue = Convert.ToDouble(userEval.Grade.Replace(".", ","));
              // grade are ajusted to 20 as MAx. Avg is on 20
              double ajustedGrade = Math.Round(20 * gradeValue / maxGrade, 2);
              double coeff = userEval.Evaluation.Coeff;
              //data for course average
              gradesSum += ajustedGrade * coeff;
              coeffSum += coeff;
            }
          }

          double courseAvg = 0;
          if (coeffSum > 0)
          {
            courseAvg = Math.Round(gradesSum / coeffSum, 2);
            CourseAvgDto courseAvgDto = new CourseAvgDto();
            courseAvgDto.Name = course.Name;
            courseAvgDto.Abbrev = course.Abbreviation;
            courseAvgDto.Avg = courseAvg;
            coursesAvg.Add(courseAvgDto);

            //get course coeff
            var courseCoeffData = await _context.CourseCoefficients
              .FirstOrDefaultAsync(c => c.ClassLevelid == userClass.ClassLevelId &&
                c.CourseId == course.Id && c.ClassTypeId == userClass.ClassTypeId);
            double courseCoeff = 1;
            if (courseCoeffData != null)
              courseCoeff = courseCoeffData.Coefficient;

            courseAvgSum += courseAvg * courseCoeff;
            courseCoeffSum += courseCoeff;
          }
        }

        if (courseCoeffSum > 0)
          GeneralAvg = Math.Round(courseAvgSum / courseCoeffSum, 2);
      }

      return GeneralAvg;
    }

    public EvalSmsDto GetUSerSmsEvalData(int ChildId, List<UserEvaluation> ClassEvals)
    {
      EvalSmsDto smsData = new EvalSmsDto();
      return smsData;
    }

    public async Task<UserCourseEvalsDto> GetUserCourseEvals(List<UserEvaluation> userEvals, Course acourse, Class aclass)
    {
      double gradedOutOf = 20;
      UserCourseEvalsDto userEvalsDto = new UserCourseEvalsDto();
      userEvalsDto.CourseId = acourse.Id;
      userEvalsDto.CourseName = acourse.Name;
      userEvalsDto.CourseAbbrev = acourse.Abbreviation;
      userEvalsDto.GradedOutOf = gradedOutOf;

      double gradesSum = 0;
      double coeffSum = 0;
      // are evals evailable for the current course?
      if (userEvals.Count() > 0)
      {
        List<GradeDto> grades = new List<GradeDto>();
        for (int j = 0; j < userEvals.Count(); j++)
        {
          // calculate each grade of the selected course
          var elt = userEvals[j];
          if (elt.Grade.IsNumeric())
          {
            var evalDate = elt.Evaluation.EvalDate.ToString("dd/MM/yyyy", frC);
            var evalType = elt.Evaluation.EvalType.Name;
            var evalName = elt.Evaluation.Name;
            double maxGrade = Convert.ToDouble(elt.Evaluation.MaxGrade);
            double gradeValue = Convert.ToDouble(elt.Grade.Replace(".", ","));
            // grade are ajusted to 20 as MAX. Avg is on 20
            double ajustedGrade = Math.Round(20 * gradeValue / maxGrade, 2);
            double coeff = elt.Evaluation.Coeff;
            //data for course average
            gradesSum += ajustedGrade * coeff;
            coeffSum += coeff;

            // get class grades for the current user grade(evaluation)
            var evalClassGrades = await _context.UserEvaluations
              .Where(e => e.EvaluationId == elt.EvaluationId &&
                e.Evaluation.GradeInLetter == false && e.Evaluation.Graded == true && e.Grade.IsNumeric())
              .Select(e => e.Grade).ToListAsync();

            double classMin = 999999;
            double classMax = -999999;
            //get class min and max of evaluation
            foreach (var item in evalClassGrades)
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

        //get course coeff
        var courseCoeffData = await _context.CourseCoefficients.FirstOrDefaultAsync(c => c.ClassLevelid == aclass.ClassLevelId &&
          c.CourseId == acourse.Id && c.ClassTypeId == aclass.ClassTypeId);
        double courseCoeff = 1;
        if (courseCoeffData != null)
          courseCoeff = courseCoeffData.Coefficient;

        //get the class course average - to be compared with the user average
        double ClassCourseAvg = await GetClassCourseAvg(acourse.Id, aclass.Id);

        userEvalsDto.UserCourseAvg = gradesAvg;
        userEvalsDto.ClassCourseAvg = ClassCourseAvg;
        userEvalsDto.CourseCoeff = courseCoeff;
        userEvalsDto.Grades = grades;
      }
      else
      {
        // there is no grade for the course - User course AVG set to -1000.
        userEvalsDto.UserCourseAvg = -1000;
      }

      return userEvalsDto;
    }

    public async Task<List<double>> GetEvalMinMax(int evalId)
    {
      var evalFromDB = await _context.UserEvaluations
        .Where(e => e.EvaluationId == evalId && e.Grade.IsNumeric())
        .ToListAsync();
      var gradeMin = evalFromDB.Min(e => Convert.ToDouble(e.Grade));
      var gradeMax = evalFromDB.Max(e => Convert.ToDouble(e.Grade));
      List<double> minmax = new List<double>();
      minmax.Add(gradeMin);
      minmax.Add(gradeMax);
      return minmax;
    }

    public async Task<double> GetEvalMax(int evalId)
    {
      var evalFromDB = await _context.UserEvaluations
        .Where(e => e.EvaluationId == evalId && e.Grade.IsNumeric())
        .ToListAsync();
      var gradeMax = evalFromDB.Max(e => Convert.ToDouble(e.Grade));
      return Convert.ToDouble(gradeMax);
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

    public async Task<double> GetClassAvg(int classId)
    {
      var aclass = await _context.Classes.FirstAsync(c => c.Id == classId);
      var classCourses = await _context.ClassCourses
        .Where(c => c.ClassId == classId).Select(c => c.Course)
        .Distinct().ToListAsync();

      double AvgSum = 0;
      double CoeffSum = 0;
      double Avg = -1000;
      // double AvgMin = -1000;
      // double AvgMax = -1000;
      foreach (var course in classCourses)
      {
        var courseAvg = await GetClassCourseAvg(course.Id, classId);
        // do we have grades for this course?
        if (courseAvg != -1000)
        {
          //get course coeff
          double courseCoeff = 1;
          var courseCoeffData = await _context.CourseCoefficients
            .FirstOrDefaultAsync(c => c.ClassLevelid == aclass.ClassLevelId &&
              c.CourseId == course.Id && c.ClassTypeId == aclass.ClassTypeId);
          if (courseCoeffData != null)
            courseCoeff = courseCoeffData.Coefficient;

          AvgSum += courseAvg * courseCoeff;
          CoeffSum += courseCoeff;
        }
      }

      Avg = Math.Round(AvgSum / CoeffSum, 2);
      return Avg;
    }

    public async Task<ClassAvgDto> GetClassCourseAvgs(int courseId, int classId)
    {
      double avgMin = 1000;
      double avgMax = -1000;
      var students = await GetClassStudents(classId);
      foreach (var student in students)
      {
        var avg = await GetStudentCourseAvg(courseId, student.Id);
        // does the student have grades to have a avg?(avg value = -1000)
        if (avg > 0)
        {
          if (avg > avgMax)
            avgMax = avg;
          if (avg < avgMin)
            avgMin = avg;
        }
      }

      ClassAvgDto classAvgs = new ClassAvgDto();
      classAvgs.ClassAvgMin = avgMin;
      classAvgs.ClassAvgMax = avgMax;

      return classAvgs;
    }

    public async Task<double> GetClassCourseAvg(int courseId, int classId)
    {
      var classEvals = await _context.UserEvaluations
        .Include(e => e.Evaluation)
        .OrderBy(o => o.Evaluation.EvalDate)
        .Where(e => e.Evaluation.ClassId == classId && e.Evaluation.GradeInLetter == false &&
          e.Evaluation.CourseId == courseId && e.Evaluation.Graded == true && e.Grade.IsNumeric())
        .Distinct().ToListAsync();

      double gradesSum = 0;
      double coeffSum = 0;
      double avg = -1000;
      if (classEvals.Count() > 0)
      {
        for (int i = 0; i < classEvals.Count(); i++)
        {
          var elt = classEvals[i];
          double gradeMax = Convert.ToDouble(elt.Evaluation.MaxGrade);
          double gradeValue = Convert.ToDouble(elt.Grade);
          // grade are ajusted to 20 as MAx. Avg is on 20
          double ajustedGrade = Math.Round(20 * gradeValue / gradeMax, 2);
          double coeff = elt.Evaluation.Coeff;
          gradesSum += ajustedGrade * coeff;
          coeffSum += coeff;
        }
        avg = gradesSum / coeffSum;
      }

      return Math.Round(avg, 2);
    }

    public async Task<List<AgendaForListDto>> GetUserClassAgenda(int classId, DateTime startDate, DateTime endDate)
    {
      List<Agenda> agendas = await _cache.GetAgendas();

      List<Agenda> classAgenda = agendas.OrderBy(o => o.Session.SessionDate)
                                        .Where(a => a.Session.ClassId == classId && a.Session.SessionDate.Date >= startDate &&
                                          a.Session.SessionDate <= endDate)
                                        .ToList();

      var agendaDates = classAgenda.OrderBy(o => o.Session.SessionDate).Select(a => a.Session.SessionDate).Distinct().ToList();

      List<AgendaForListDto> AgendaList = new List<AgendaForListDto>();
      foreach (var date in agendaDates)
      {
        AgendaForListDto agendaForList = new AgendaForListDto();
        agendaForList.DueDate = date;
        var shortDueDate = date.ToString("ddd dd MMM", frC);
        var longDueDate = date.ToString("dd MMMM yyyy", frC);
        var dueDateAbbrev = date.ToString("ddd dd", frC).Replace(".", "");

        agendaForList.ShortDueDate = shortDueDate;
        agendaForList.LongDueDate = longDueDate;
        agendaForList.DueDateAbbrev = dueDateAbbrev;

        //get agenda tasks Done Status
        agendaForList.AgendaItems = new List<AgendaItemDto>();
        var agendaItems = classAgenda.Where(a => a.Session.SessionDate.Date == date.Date).ToList();
        agendaForList.NbItems = agendaItems.Count();
        foreach (var item in agendaItems)
        {
          AgendaItemDto agendaItem = new AgendaItemDto();
          if (item.Session.Course != null)
          {
            agendaItem.CourseId = Convert.ToInt32(item.Session.CourseId);
            agendaItem.CourseName = item.Session.Course.Name;
          }

          agendaItem.CourseAbbrev = item.Session.Course.Abbreviation;
          agendaItem.CourseColor = item.Session.Course.Color;
          agendaItem.strDateAdded = item.DateAdded.ToString("dd/MM/yyyy", frC);
          agendaItem.TaskDesc = item.TaskDesc;
          agendaItem.AgendaId = item.Id;
          agendaItem.Done = item.Done;
          agendaForList.AgendaItems.Add(agendaItem);
        }

        AgendaList.Add(agendaForList);
      }

      return AgendaList;
    }

    public async Task<int> GetAssignedChildrenCount(int parentId)
    {
      var userIds = await _context.UserLinks.Where(u => u.UserPId == parentId)
        .Select(s => s.UserId).ToListAsync();

      return userIds.Count();
    }

    public async Task<bool> SaveProductSelection(int userPid, int userId, List<ServiceSelectionDto> products)
    {
      // insertion dans la table order 
      int total = products.Sum(x => Convert.ToInt32(x.Price));
      var newOrder = new Order
      {
        TotalHT = total,
        AmountTTC = total,
        Discount = 0,
        TVA = 0,
        // TO BE COPED ON THE NEW REGISTRATION PAGE
        FatherId = userPid,
        ChildId = userId
      };
      _context.Add(newOrder);

      foreach (var prod in products)
      {
        var newOrderLine = new OrderLine
        {
          OrderId = newOrder.Id,
          ProductId = prod.Id,
          AmountHT = prod.Price,
          AmountTTC = prod.Price,
          Discount = 0,
          Qty = 1,
          TVA = 0
        };
        _context.Add(newOrderLine);
      }

      if (await _context.SaveChangesAsync() > 0)
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

    public async Task<List<Absence>> GetAbsencesByDate(DateTime date)
    {
      var absences = await _context.Absences
        .Include(i => i.User).ThenInclude(i => i.Class)
        .Include(i => i.User).ThenInclude(i => i.Photos)
        .Include(i => i.DoneBy)
        .Include(i => i.AbsenceType)
        .Include(i => i.Session).ThenInclude(i => i.Course)
        .Where(a => a.StartDate.Date == date.Date)
        .ToListAsync();
      return absences;
    }

    public async Task<MsgRecipientsDto> GetMsgRecipientsForUsers(List<User> users, List<int> educLevelIds,
      List<int> schoolIds, List<int> classLevelIds, List<int> classIds, int msgChoice, Boolean sendToNotValidated)
    {
      List<ClassCourse> classCourses = await _context.ClassCourses
        .Include(i => i.Class).ThenInclude(i => i.ClassLevel).ThenInclude(i => i.School)
        .Include(i => i.Class).ThenInclude(i => i.ClassLevel).ThenInclude(i => i.EducationLevel)
        .ToListAsync();
      MsgRecipientsDto recipients = new MsgRecipientsDto();
      recipients.UsersOK = new List<User>();
      recipients.UsersNOK = new List<User>();
      List<User> filteredUsers = new List<User>();

      foreach (var user in users)
      {
        List<Class> userClassCourses = classCourses.Where(c => c.TeacherId == user.Id).Select(s => s.Class).ToList();

        //selected Education levels?
        List<User> userEducLevels = new List<User>();
        if (educLevelIds.Count() > 0)
        {
          if (user.ClassId != null)
          {
            if (educLevelIds.Contains(Convert.ToInt32(user.Class.ClassLevel.EducationLevelId)))
              userEducLevels.Add(user);
          }

          if (user.ClassLevelId != null)
          {
            if (educLevelIds.Contains(Convert.ToInt32(user.ClassLevel.EducationLevelId)))
              userEducLevels.Add(user);
          }

          if (user.EducLevelId != null)
          {
            if (educLevelIds.Contains(Convert.ToInt32(Convert.ToInt32(user.EducLevelId))))
              userEducLevels.Add(user);
          }

          foreach (var aclass in userClassCourses)
          {
            if (educLevelIds.Contains(Convert.ToInt32(aclass.ClassLevel.EducationLevelId)))
              userEducLevels.Add(user);
          }

          var selectedUserIds = userEducLevels.Select(u => u.Id);
          filteredUsers = users.Where(u => selectedUserIds.Contains(u.Id)).ToList();
        }

        //selected schools?
        List<User> userSchools = new List<User>();
        if (schoolIds.Count() > 0)
        {
          if (user.ClassId != null)
          {
            if (schoolIds.Contains(Convert.ToInt32(user.Class.ClassLevel.SchoolId)))
              userSchools.Add(user);
          }

          foreach (var aclass in userClassCourses)
          {
            if (schoolIds.Contains(Convert.ToInt32(aclass.ClassLevel.SchoolId)))
              userEducLevels.Add(user);
          }

          var selectedUserIds = userSchools.Select(u => u.Id);
          List<User> selectedUsers = users.Where(u => selectedUserIds.Contains(u.Id)).ToList();
          filteredUsers.AddRange(selectedUsers);
        }

        //selected classlevels?
        List<User> userClassLevels = new List<User>();
        if (classLevelIds.Count() > 0)
        {
          if (user.ClassId != null)
          {
            if (classLevelIds.Contains(Convert.ToInt32(user.Class.ClassLevelId)))
              userClassLevels.Add(user);
          }
          if (user.ClassLevelId != null)
          {
            if (classLevelIds.Contains(Convert.ToInt32(user.ClassLevelId)))
              userClassLevels.Add(user);
          }

          foreach (var aclass in userClassCourses)
          {
            if (classLevelIds.Contains(Convert.ToInt32(aclass.ClassLevelId)))
              userEducLevels.Add(user);
          }

          var selectedUserIds = userClassLevels.Select(u => u.Id);
          List<User> selectedUsers = users.Where(u => selectedUserIds.Contains(u.Id)).ToList();
          filteredUsers.AddRange(selectedUsers);
        }

        //selected classes?
        List<User> userClasses = new List<User>();
        if (classIds.Count() > 0)
        {
          if (user.ClassId != null)
          {
            if (classIds.Contains(Convert.ToInt32(user.ClassId)))
              userClasses.Add(user);
          }

          foreach (var aclass in userClassCourses)
          {
            if (classIds.Contains(Convert.ToInt32(aclass.Id)))
              userClasses.Add(user);
          }

          var selectedUserIds = userClasses.Select(u => u.Id);
          List<User> selectedUsers = users.Where(u => selectedUserIds.Contains(u.Id)).ToList();
          filteredUsers.AddRange(selectedUsers);
        }
      }

      filteredUsers = filteredUsers.Distinct().ToList();
      foreach (var user in filteredUsers)
      {
        if (msgChoice == 1) //email
        {
          if (!string.IsNullOrEmpty(user.Email) && (user.EmailConfirmed || sendToNotValidated))
          {
            recipients.UsersOK.Add(user);
          }
          else
          {
            recipients.UsersNOK.Add(user);
          }
        }
        else //sms
        {
          if (!string.IsNullOrEmpty(user.PhoneNumber) && (user.PhoneNumberConfirmed || sendToNotValidated))
          {
            recipients.UsersOK.Add(user);
          }
          else
          {
            recipients.UsersNOK.Add(user);
          }
        }
      }

      return recipients;
    }

    public MsgRecipientsDto setRecipientsList(List<User> users, int msgChoice, Boolean sendToNotValidated)
    {
      MsgRecipientsDto recipients = new MsgRecipientsDto();
      recipients.UsersOK = new List<User>();
      recipients.UsersNOK = new List<User>();
      foreach (var user in users)
      {
        if (msgChoice == 1) //email
        {
          if (!string.IsNullOrEmpty(user.Email) && (user.EmailConfirmed || sendToNotValidated))
          {
            recipients.UsersOK.Add(user);
          }
          else
          {
            recipients.UsersNOK.Add(user);
          }
        }
        else //sms
        {
          if (!string.IsNullOrEmpty(user.PhoneNumber) && (user.PhoneNumberConfirmed || sendToNotValidated))
          {
            recipients.UsersOK.Add(user);
          }
          else
          {
            recipients.UsersNOK.Add(user);
          }
        }
      }

      return recipients;
    }
    public MsgRecipientsDto GetMsgRecipientsForClasses(List<User> users, int msgType, Boolean sendToNotValidated)
    {
      MsgRecipientsDto recipients = new MsgRecipientsDto();

      foreach (var user in users)
      {
        if (msgType == 1) //email
        {
          if (user.EmailConfirmed || sendToNotValidated)
          {
            recipients.UsersOK.Add(user);
          }
          else
          {
            recipients.UsersNOK.Add(user);
          }
        }
        else // sms
        {
          if (user.PhoneNumberConfirmed || sendToNotValidated)
          {
            recipients.UsersOK.Add(user);
          }
          else
          {
            recipients.UsersNOK.Add(user);
          }
        }
      }
      return recipients;
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

    public async Task<List<Sms>> SetSmsDataForAbsences(List<AbsenceSmsDto> absences, int sessionId, int teacherId)
    {
      List<Sms> AbsencesSms = new List<Sms>();
      var tokens = await GetTokens();
      int absenceSmsId = _config.GetValue<int>("AppSettings:AbsenceSms");
      int lateSmsId = _config.GetValue<int>("AppSettings:LateSms");
      var AbsenceSms = await _context.SmsTemplates.FirstAsync(s => s.Id == absenceSmsId);
      var LateSms = await _context.SmsTemplates.FirstAsync(s => s.Id == lateSmsId);
      int absTypeId = _config.GetValue<int>("AppSettings:AbsenceTypeId");
      int lateTypeId = _config.GetValue<int>("AppSettings:LateTypeId");
      int smsAbsTypeId = _config.GetValue<int>("AppSettings:SmsAbsTypeId");

      foreach (var abs in absences)
      {
        //did you already sent the sms?
        Sms oldSms = await _context.Sms.FirstOrDefaultAsync(s => s.SessionId == sessionId && s.ToUserId == abs.ParentId &&
          s.StudentId == abs.ChildId && s.StatusFlag == 1);
        if (oldSms != null)
          continue;

        Sms newSms = new Sms();
        newSms.SmsTypeId = smsAbsTypeId;
        newSms.To = abs.ParentCellPhone;
        newSms.StudentId = abs.ChildId;
        newSms.ToUserId = abs.ParentId;
        newSms.SessionId = sessionId;
        newSms.validityPeriod = 1;
        string smsContent;
        if (abs.AbsenceTypeId == absTypeId)
        {
          smsContent = AbsenceSms.Content;
        }
        else
        {
          smsContent = LateSms.Content;
        }
        // replace tokens with dynamic data
        List<TokenDto> tags = GetTokenAbsenceValues(tokens.ToList(), abs);
        newSms.Content = ReplaceTokens(tags, smsContent);
        newSms.InsertUserId = teacherId;
        newSms.InsertDate = DateTime.Now;
        newSms.UpdateDate = DateTime.Now;
        AbsencesSms.Add(newSms);
      }

      return AbsencesSms;
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
          case "<RETARD_MIN>":
            td.Value = absSms.LateInMin;
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public async Task<Email> SetDataForConfirmEmployeeEmail(ConfirmEmployeeEmailDto emailData, string content, string subject)
    {
      List<Setting> settings = await _cache.GetSettings();

      var tokens = await GetTokens();
      var schoolName = settings.First(s => s.Name == "SchoolName").Value;
      Email newEmail = new Email();
      newEmail.EmailTypeId = 1;
      newEmail.ToAddress = emailData.Email;
      newEmail.FromAddress = "no-reply@educnotes.com";
      newEmail.Subject = subject.Replace("<NOM_ECOLE>", schoolName);
      List<TokenDto> tags = GetEmployeeEmailTokenValues(tokens, emailData);
      newEmail.Body = ReplaceTokens(tags, content);
      newEmail.InsertUserId = 1;
      newEmail.InsertDate = DateTime.Now;
      newEmail.UpdateUserId = 1;
      newEmail.UpdateDate = DateTime.Now;
      newEmail.ToUserId = emailData.Id;

      return newEmail;
    }

    public async Task<Email> SetDataForConfirmTeacherEmail(ConfirmTeacherEmailDto emailData, string content, string subject)
    {
      List<Setting> settings = await _cache.GetSettings();

      var tokens = await GetTokens();
      var schoolName = settings.First(s => s.Name == "SchoolName").Value;
      Email newEmail = new Email();
      newEmail.EmailTypeId = 1;
      newEmail.ToAddress = emailData.Email;
      newEmail.FromAddress = "no-reply@educnotes.com";
      newEmail.Subject = subject.Replace("<NOM_ECOLE>", schoolName);
      List<TokenDto> tags = GetTeacherEmailTokenValues(tokens, emailData);
      newEmail.Body = ReplaceTokens(tags, content);
      newEmail.InsertUserId = 1;
      newEmail.InsertDate = DateTime.Now;
      newEmail.UpdateUserId = 1;
      newEmail.UpdateDate = DateTime.Now;
      newEmail.ToUserId = emailData.Id;

      return newEmail;
    }

    public async Task<IEnumerable<Email>> SetEmailDataForRegistration(IEnumerable<RegistrationEmailDto> emailData,
      string content, string RegDeadLine)
    {
      List<Email> RegEmails = new List<Email>();
      var tokens = await GetTokens();

      foreach (var data in emailData)
      {
        Email newEmail = new Email();
        newEmail.EmailTypeId = 1;
        newEmail.OrderId = data.OrderId;
        newEmail.ToAddress = data.ParentEmail;
        newEmail.FromAddress = "no-reply@educnotes.com";
        newEmail.Subject = data.EmailSubject;
        List<TokenDto> tags = await GetRegistrationTokenValues(tokens, data, RegDeadLine);
        newEmail.Body = ReplaceTokens(tags, content);
        newEmail.InsertUserId = 1;
        newEmail.InsertDate = DateTime.Now;
        newEmail.UpdateUserId = 1;
        newEmail.UpdateDate = DateTime.Now;
        newEmail.ToUserId = data.ParentId;
        RegEmails.Add(newEmail);
      }

      return RegEmails;
    }

    public List<TokenDto> GetMessageTokenValues(List<Token> tokens, UserToSendMsgDto user)
    {
      List<TokenDto> tokenValues = new List<TokenDto>();

      foreach(var token in tokens)
      {
        TokenDto td = new TokenDto();
        td.TokenString = token.TokenString;

        switch (td.TokenString)
        {
          case "#N_UTILISATEUR#":
            td.Value = user.LastName.FirstLetterToUpper();
            break;
          case "#P_UTILISATEUR#":
            td.Value = user.FirstName.FirstLetterToUpper();
            break;
          case "#M_MME#":
            td.Value = user.Gender == 0 ? "Mme" : "M.";
            break;
          case "#CELL_UTILISATEUR#":
            td.Value = user.Mobile;
            break;
          case "#EMAIL_UTILISATEUR#":
            td.Value = user.Email;
            break;
          case "#TOKEN#":
            td.Value = user.Token;
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public List<TokenDto> GetEmployeeEmailTokenValues(List<Token> tokens, ConfirmEmployeeEmailDto emailData)
    {
      List<TokenDto> tokenValues = new List<TokenDto>();

      foreach (var token in tokens)
      {
        TokenDto td = new TokenDto();
        td.TokenString = token.TokenString;
        var subDomain = GetAppSubDomain();
        string teacherId = emailData.Id.ToString();

        switch (td.TokenString)
        {
          case "<N_USER>":
            td.Value = emailData.LastName.FirstLetterToUpper();
            break;
          case "<P_USER>":
            td.Value = emailData.FirstName.FirstLetterToUpper();
            break;
          case "<M_MME>":
            td.Value = emailData.Gender == 0 ? "Mme" : "M.";
            break;
          case "<CELL_USER>":
            td.Value = emailData.Cell;
            break;
          case "<EMAIL_USER>":
            td.Value = emailData.Email;
            break;
          case "<TOKEN>":
            td.Value = emailData.Token;
            break;
          case "<ROLE>":
            td.Value = emailData.Role;
            break;
          case "<CONFIRM_LINK>":
            string url = "";
            if (subDomain != "")
              url = string.Format(baseUrl, subDomain + ".");
            else
              url = string.Format(baseUrl, "");
            td.Value = string.Format("{0}/confirmUserEmail?id={1}&token={2}", url,
              teacherId, HttpUtility.UrlEncode(emailData.Token));
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public List<TokenDto> GetTeacherEmailTokenValues(List<Token> tokens, ConfirmTeacherEmailDto emailData)
    {
      List<TokenDto> tokenValues = new List<TokenDto>();

      foreach (var token in tokens)
      {
        TokenDto td = new TokenDto();
        td.TokenString = token.TokenString;
        var subDomain = GetAppSubDomain();
        string teacherId = emailData.Id.ToString();

        switch (td.TokenString)
        {
          case "<N_USER>":
            td.Value = emailData.LastName.FirstLetterToUpper();
            break;
          case "<P_USER>":
            td.Value = emailData.FirstName.FirstLetterToUpper();
            break;
          case "<M_MME>":
            td.Value = emailData.Gender == 0 ? "Mme" : "M.";
            break;
          case "<CELL_USER>":
            td.Value = emailData.Cell;
            break;
          case "<EMAIL_USER>":
            td.Value = emailData.Email;
            break;
          case "<TOKEN>":
            td.Value = emailData.Token;
            break;
          case "<COURS_PROF>":
            td.Value = emailData.Courses;
            break;
          case "<CONFIRM_LINK>":
            string url = "";
            if (subDomain != "")
              url = string.Format(baseUrl, subDomain + ".");
            else
              url = string.Format(baseUrl, "");
            td.Value = string.Format("{0}/confirmUserEmail?id={1}&token={2}", url,
              teacherId, HttpUtility.UrlEncode(emailData.Token));
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public List<TokenDto> GetRecoverySmsTokenValues(IEnumerable<Token> tokens, RecoveryForParentDto parent)
    {
      var subDomain = GetAppSubDomain();
      List<TokenDto> tokenValues = new List<TokenDto>();

      //set children registration data
      string dueAmountsInfos = "";
      byte num = 1;
      foreach(var child in parent.Children)
      {
        string childFirstName = child.FirstName.FirstLetterToUpper();
        string childLastName = child.LastName.FirstLetterToUpper();
        string className = child.ClassName != "" ? child.ClassName : child.LevelName;
        dueAmountsInfos += num + ". " + childLastName + " " + childFirstName + " " + className + "\n";
        foreach (var product in child.ProductRecovery)
        {
          dueAmountsInfos += product.ProductName + " (" + product.NbDaysLate + "j) : " +
            product.LateAmounts.TotalLateAmount.ToString("N0", frC) + " F\n";
        };
        num++;
      }

      int parentId = 0;
      string parentLastName = "";
      byte parentGender = 0;
      if(parent.FatherId != 0)
      {
        parentId = parent.FatherId;
        parentLastName = parent.FatherLastName;
        parentGender = parent.FatherGender;
      }
      else
      {
        parentId = parent.MotherId;
        parentLastName = parent.MotherLastName;
        parentGender = parent.MotherGender;
      }

      foreach (var token in tokens)
      {
        TokenDto td = new TokenDto();
        td.TokenString = token.TokenString;

        switch (td.TokenString)
        {
          case "<N_PARENT>":
            td.Value = parentLastName.FirstLetterToUpper();
            break;
          case "<M_MME>":
            td.Value = parentGender == 0 ? "Mme" : "M.";
            break;
          case "<MONTANT_TOTAL_DU>":
            td.Value = parent.LateDueAmount.ToString("N0", frC) + " F";
            break;
          case "<DETAILS_MONTANTS_DUS>":
            td.Value = dueAmountsInfos;
            break;
          case "<LIEN_MONTANTS_DUS>":
            string url = "";
            if (subDomain != "")
              url = string.Format(baseUrl, subDomain + ".");
            else
              url = string.Format(baseUrl, "");
            td.Value = string.Format("{0}/viewAccountHistory?id={1}", url, parentId);
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public List<TokenDto> GetRecoveryMsgTokenValues(IEnumerable<Token> tokens, RecoveryForParentDto parent)
    {
      List<TokenDto> tokenValues = new List<TokenDto>();

      //set children registration data
      string accountInfos = "";
      byte num = 1;
      foreach(var child in parent.Children)
      {
        string childFirstName = child.FirstName.FirstLetterToUpper();
        string childLastName = child.LastName.FirstLetterToUpper();
        string className = child.ClassName != null ? child.ClassName : child.LevelName;
        accountInfos += "<div><br></div><div><span style=\"font-size: 1rem;\"><b>" + num + ". " + childLastName + " " + childFirstName +
          ", " + className + ".</b></span><span style=\"color: #ff0000;\"> montant total dû : " +
          child.LateDueAmount.ToString("N0", frC) + " F</span></div>";
        
        foreach (var product in child.ProductRecovery)
        {
          accountInfos += "<div>" +  product.ProductName + " (" + product.NbDaysLate + " jours) : " +
            product.LateAmounts.TotalLateAmount.ToString("N0", frC) + " F</div>";
        }

        num++;
      }

      string parentLastName = "";
      byte parentGender = 0;
      if(parent.FatherId != 0)
      {
        parentLastName = parent.FatherLastName;
        parentGender = parent.FatherGender;
      }
      else
      {
        parentLastName = parent.MotherLastName;
        parentGender = parent.MotherGender;
      }

      foreach (var token in tokens)
      {
        TokenDto td = new TokenDto();
        td.TokenString = token.TokenString;

        switch (td.TokenString)
        {
          case "<N_PARENT>":
            td.Value = parentLastName.FirstLetterToUpper();
            break;
          case "<M_MME>":
            td.Value = parentGender == 0 ? "Mme" : "M.";
            break;
          case "<MAX_JOURS_RETARD>":
            td.Value = "0";
            break;
          case "<NB_JOURS_MAX_PAIEMENT>":
            td.Value = "3";
            break;
          case "<INFOS_COMPTE_PARENT>":
            td.Value = accountInfos;
            break;
          case "<MONTANT_TOTAL_DU>":
            td.Value = parent.LateDueAmount.ToString("N0", frC) + " F";
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public async Task<List<TokenDto>> GetRegistrationTokenValues(IEnumerable<Token> tokens, RegistrationEmailDto regEmail, string RegDeadLine)
    {
      List<PaymentType> paymentTypesCached = await _cache.GetPaymentTypes();
      List<TokenDto> tokenValues = new List<TokenDto>();

      var subDomain = GetAppSubDomain();

      //set children registration data
      string childrenInfos = "";
      byte num = 1;
      foreach (var child in regEmail.Children)
      {
        string childFirstName = child.FirstName.FirstLetterToUpper();
        string childLastName = child.LastName.FirstLetterToUpper();
        childrenInfos += "<div><br></><div><span style=\"font-size: 1rem;\">" + num + ". <b>" + childLastName + " " +
          childFirstName + ".</b></span><b style=\"font-size: 1rem;\"> classe " + child.NextClass +
          "</b><span style=\"font-size: 1rem;\">" + ".</span></div><div><ul><li><span style=\"font-size: 1rem;\">" +
          "frais de scolarité pour l'année : " + child.TuitionAmount + " F CFA</span>" + "</li><li>" +
          "<span style=\"font-size: 1rem;\">frais d'inscription : " + child.RegistrationFee + " F CFA</span></li><li>" +
          "<span style=\"font-size: 1rem;\">acompte(" + child.DueAmountPct + ")&nbsp; : " + child.DueAmount +
          " F CFA</span></li></ul>montant total dû pour valider l'inscription de l'élève " + childFirstName + " : <u>" + child.TotalDueForChild + " F CFA</u></div>";
        num++;
      }

      string payments = "<ul>";
      var paymentTypes = paymentTypesCached.OrderBy(o => o.DsplSeq).ToList();
      foreach (var type in paymentTypes)
      {
        payments += "<li>" + type.Name + "</li>";
      }
      payments += "</ul>";

      foreach (var token in tokens)
      {
        TokenDto td = new TokenDto();
        td.TokenString = token.TokenString;

        string parentId = regEmail.ParentId.ToString();
        string orderid = regEmail.OrderId.ToString();

        switch (td.TokenString)
        {
          case "<USER_ID>":
            td.Value = parentId;
            break;
          case "<N_PARENT>":
            td.Value = regEmail.ParentLastName.FirstLetterToUpper();
            break;
          case "<P_PARENT>":
            td.Value = regEmail.ParentFirstName.FirstLetterToUpper();
            break;
          case "<M_MME>":
            td.Value = regEmail.ParentGender == 0 ? "Mme" : "M.";
            break;
          case "<DATE_LIMITE_INSCR>":
            td.Value = RegDeadLine;
            break;
          case "<TOTAL_FRAIS>":
            td.Value = regEmail.TotalAmount;
            break;
          case "<DATE_LIMITE_FRAIS>":
            td.Value = regEmail.DueDate;
            break;
          case "<INFOS_INSCR_ENFANTS>":
            td.Value = childrenInfos;
            break;
          case "<ORDER_ID>":
            td.Value = orderid;
            break;
          case "<ORDER_NUM>":
            td.Value = regEmail.OrderNum.ToString();
            break;
          case "<TOKEN>":
            td.Value = regEmail.Token;
            break;
          case "<TYPES_PAIEMENT>":
            td.Value = payments;
            break;
          case "<CONFIRM_LINK>":
            string url = "";
            if (subDomain != "")
              url = string.Format(baseUrl, subDomain + ".");
            else
              url = string.Format(baseUrl, "");
            td.Value = string.Format("{0}/confirmEmail?id={1}&orderid={2}&token={3}", url,
              parentId, orderid, HttpUtility.UrlEncode(regEmail.Token));
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public async Task<Email> SetEmailForAccountUpdated(string subject, string content, string lastName,
      byte gender, string parentEmail, int userId)
    {
      List<Setting> settings = await _context.Settings.ToListAsync();
      var schoolName = (settings.First(s => s.Name.ToLower() == "schoolname")).Value;
      var tokens = await GetTokens();

      Email newEmail = new Email();
      newEmail.EmailTypeId = 1;
      newEmail.ToAddress = parentEmail;
      newEmail.FromAddress = "no-reply@educnotes.com";
      newEmail.Subject = subject.Replace("<NOM_ECOLE>", schoolName);
      List<TokenDto> tags = GetAccountUpdatedTokenValues(tokens, lastName, gender);
      newEmail.Body = ReplaceTokens(tags, content);
      newEmail.InsertUserId = 1;
      newEmail.InsertDate = DateTime.Now;
      newEmail.UpdateUserId = 1;
      newEmail.UpdateDate = DateTime.Now;
      newEmail.ToUserId = userId;

      return newEmail;
    }

    public async Task<Email> SetEmailForResetPwdLink(string subject, string content, int userId, string lastName,
      string firstName, byte gender, string userEmail, string resetToken)
    {
      var tokens = await GetTokens();

      Email newEmail = new Email();
      newEmail.EmailTypeId = 1;
      newEmail.ToAddress = userEmail;
      newEmail.FromAddress = "no-reply@educnotes.com";
      newEmail.Subject = subject;
      List<TokenDto> tags = GetResetPwdLinkTokenValues(tokens, userId, lastName, firstName, gender, resetToken);
      newEmail.Body = ReplaceTokens(tags, content);
      newEmail.InsertUserId = 1;
      newEmail.InsertDate = DateTime.Now;
      newEmail.UpdateUserId = 1;
      newEmail.UpdateDate = DateTime.Now;
      newEmail.ToUserId = userId;

      return newEmail;
    }

    public List<TokenDto> GetAccountUpdatedTokenValues(IEnumerable<Token> tokens, string lastName, byte gender)
    {
      List<TokenDto> tokenValues = new List<TokenDto>();

      foreach (var token in tokens)
      {
        TokenDto td = new TokenDto();
        td.TokenString = token.TokenString;
        switch (td.TokenString)
        {
          case "<N_PARENT>":
            td.Value = lastName;
            break;
          case "<M_MME>":
            td.Value = gender == 0 ? "Mme" : "M.";
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public List<TokenDto> GetResetPwdLinkTokenValues(IEnumerable<Token> tokens, int userId, string lastName,
      string firstName, byte gender, string resetToken)
    {
      var subDomain = GetAppSubDomain();
      List<TokenDto> tokenValues = new List<TokenDto>();

      foreach (var token in tokens)
      {
        TokenDto td = new TokenDto();
        td.TokenString = token.TokenString;
        switch (td.TokenString)
        {
          case "<N_USER>":
            td.Value = lastName;
            break;
          case "<P_USER>":
            td.Value = firstName;
            break;
          case "<M_MME>":
            td.Value = gender == 0 ? "Mme" : "M.";
            break;
          case "<SUBDOMAIN>":
            td.Value = subDomain == "" ? "" : subDomain + ".";
            break;
          case "<CONFIRM_LINK>":
            string url = "";
            if (subDomain != "")
              url = string.Format(baseUrl, subDomain + ".");
            else
              url = string.Format(baseUrl, "");
            td.Value = string.Format("{0}/resetPassword?id={1}&token={2}", url, userId, HttpUtility.UrlEncode(resetToken));
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public async Task<List<Sms>> SetSmsDataForNewGrade(List<EvalSmsDto> grades, string content, int teacherId)
    {
      List<Sms> GradesSms = new List<Sms>();
      var tokens = await GetTokens();
      int SmsGradeTypeId = _config.GetValue<int>("AppSettings:SmsGradeTypeId");

      foreach (var grade in grades)
      {
        var oldSms = await _context.Sms.FirstOrDefaultAsync(s => s.StudentId == grade.ChildId &&
          s.ToUserId == grade.ParentId && s.EvaluationId == grade.EvaluationId);
        if (oldSms == null || grade.ForUpdate)
        {
          Sms newSms = new Sms();
          newSms.EvaluationId = grade.EvaluationId;
          newSms.SmsTypeId = SmsGradeTypeId;
          newSms.To = grade.ParentCellPhone;
          newSms.StudentId = grade.ChildId;
          newSms.ToUserId = grade.ParentId;
          newSms.validityPeriod = 1;
          // replace tokens with dynamic data
          List<TokenDto> tags = GetTokenGradeValues(tokens, grade, grade.ForUpdate);
          newSms.Content = ReplaceTokens(tags, content);
          newSms.InsertUserId = teacherId;
          newSms.InsertDate = DateTime.Now;
          newSms.UpdateDate = DateTime.Now;
          GradesSms.Add(newSms);
        }
      }

      return GradesSms;
    }

    public List<TokenDto> GetTokenGradeValues(IEnumerable<Token> tokens, EvalSmsDto gradeSms, Boolean forUpdate)
    {
      List<TokenDto> tokenValues = new List<TokenDto>();

      foreach (var token in tokens)
      {
        TokenDto td = new TokenDto();
        td.TokenString = token.TokenString;

        string grade;
        if (forUpdate)
        {
          grade = gradeSms.OldEvalGrade.ToString() + "/" + gradeSms.GradedOutOf + " -> " +
            gradeSms.EvalGrade.ToString() + "/" + gradeSms.GradedOutOf;
        }
        else
        {
          grade = gradeSms.EvalGrade.ToString() + "/" + gradeSms.GradedOutOf;
        }

        switch (td.TokenString)
        {
          case "<P_ENFANT>":
            td.Value = gradeSms.ChildFirstName;
            break;
          case "<N_ENFANT>":
            td.Value = gradeSms.ChildLastName;
            break;
          case "<N_PARENT>":
            td.Value = gradeSms.ParentLastName;
            break;
          case "<P_PARENT>":
            td.Value = gradeSms.ParentFirstName;
            break;
          case "<M_MME>":
            td.Value = gradeSms.ParentGender == 0 ? "Mme" : "M.";
            break;
          case "<COURS>":
            td.Value = gradeSms.CourseAbbrev;
            break;
          case "<NOTE_COURS>":
            td.Value = grade;
            break;
          case "<NOTE_COURS_BASSE>":
            td.Value = gradeSms.ClassMinGrade.ToString();
            break;
          case "<NOTE_COURS_HAUTE>":
            td.Value = gradeSms.ClassMaxGrade.ToString();
            break;
          case "<MOY_COURS>":
            td.Value = gradeSms.ClassAvg.ToString();
            break;
          case "<MOY_GLE>":
            td.Value = gradeSms.ChildAvg.ToString();
            break;
          case "<DATE_JOUR>":
            td.Value = gradeSms.ForUpdate == true ? gradeSms.CapturedDate + ". modif. de note" :
              "note du " + gradeSms.CapturedDate;
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public string ReplaceTokens(List<TokenDto> tokens, string content)
    {
      foreach (var token in tokens)
      {
        content = content.Replace(token.TokenString, token.Value);
      }
      return content;
    }

    public async Task<List<Token>> GetTokens()
    {
      var tokens = await _context.Tokens.OrderBy(t => t.Name).ToListAsync();
      return tokens;
    }

    public async Task<List<Token>> GetBroadcastTokens()
    {
      List<Token> tokensCached = await _cache.GetTokens();

      var tokens = tokensCached
        .Where(t => t.TokenTypeId == broadcastTokenTypeId)
        .OrderBy(t => t.Name).ToList();
      return tokens;
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

    public async Task<Period> GetPeriodFromDate(DateTime date)
    {
      var shortDate = date.Date;
      var periods = await _context.Periods.OrderBy(p => p.StartDate).ToListAsync();
      foreach (var period in periods)
      {
        if (shortDate >= period.StartDate && date.Date <= period.EndDate.Date)
        {
          return period;
        }
      }

      return null;
    }

    public async Task<IEnumerable<Theme>> ClassLevelCourseThemes(int classLevelId, int courseId)
    {
      var themes = await _context.Themes.Include(a => a.Lessons).ThenInclude(a => a.LessonContents)
        .Where(a => a.ClassLevelId == classLevelId && a.CourseId == courseId).ToListAsync();
      return themes;
    }

    public async Task<IEnumerable<Lesson>> ClassLevelCourseLessons(int classLevelId, int courseId)
    {
      var lessons = await _context.Lessons.Include(a => a.LessonContents)
        .Where(a => a.ClassLevelId == classLevelId && a.CourseId == courseId).ToListAsync();
      return lessons;
    }

    public async Task<int> CreateLessonDoc(CourseShowingDto courseShowingDto)
    {
      var lessDoc = new LessonDoc
      {
        TeacherId = Convert.ToInt32(courseShowingDto.TeacherId)
      };
      if (courseShowingDto.CourseComment != null)
        lessDoc.Comment = courseShowingDto.CourseComment;
      Add(lessDoc);

      if (courseShowingDto.MainVideo != null)
      {
        var uploadResult = new VideoUploadResult();
        using (var stream = courseShowingDto.MainVideo.OpenReadStream())
        {
          var uploadParams = new VideoUploadParams()
          {
            File = new FileDescription(courseShowingDto.MainVideo.Name, stream)

          };

          uploadResult = _cloudinary.Upload(uploadParams);
          if (uploadResult.StatusCode == HttpStatusCode.OK)
          {
            var MainDoc = new Document
            {
              DocTypeId = _config.GetValue<int>("AppSettings:mainDocTypes"),
              PublicId = uploadResult.PublicId,
              Url = uploadResult.SecureUri.ToString(),
              FileTypeId = 1
            };
            Add(MainDoc);
            var lessDocDoc = new LessonDocDocument
            {
              LessonDocId = lessDoc.Id,
              DocumentId = MainDoc.Id
            };
            Add(lessDocDoc);
          }

        }
      }

      if (courseShowingDto.MainPdf != null)
      {
        var uploadResult = new ImageUploadResult();
        using (var stream = courseShowingDto.MainPdf.OpenReadStream())
        {
          var uploadParams = new ImageUploadParams()
          {
            File = new FileDescription(courseShowingDto.MainPdf.Name, stream)

          };

          uploadResult = _cloudinary.Upload(uploadParams);
          if (uploadResult.StatusCode == HttpStatusCode.OK)
          {
            int docTypeId = _config.GetValue<int>("AppSettings:mainDocTypes");
            var MainDoc = new Document
            {
              DocTypeId = docTypeId,
              PublicId = uploadResult.PublicId,
              Url = uploadResult.SecureUri.ToString(),
              FileTypeId = 1
            };
            Add(MainDoc);
            var lessDocDoc = new LessonDocDocument
            {
              LessonDocId = lessDoc.Id,
              DocumentId = MainDoc.Id
            };
            Add(lessDocDoc);
          }
          else
            return 0;
        }
      }

      // ajouts des autres fichiers
      foreach (var otherFile in courseShowingDto.OtherFiles)
      {

        var uploadResult = new ImageUploadResult();
        using (var stream = otherFile.OpenReadStream())
        {
          var uploadParams = new ImageUploadParams()
          {
            File = new FileDescription(otherFile.Name, stream)

          };

          uploadResult = _cloudinary.Upload(uploadParams);
          if (uploadResult.StatusCode == HttpStatusCode.OK)
          {
            var otherDoc = new Document
            {
              DocTypeId = _config.GetValue<int>("AppSettings:otherDocTypes"),
              PublicId = uploadResult.PublicId,
              Url = uploadResult.SecureUri.ToString(),
              FileTypeId = 1

            };
            Add(otherDoc);

            var lessDocDoc = new LessonDocDocument
            {
              LessonDocId = lessDoc.Id,
              DocumentId = otherDoc.Id
            };
            Add(lessDocDoc);
          }

        }
      }

      var ids = courseShowingDto.LessonContentIds.Split(",");
      if (ids.Count() > 0)
      {
        foreach (var lessonsContentId in ids)
        {
          var lessonContenDoc = new LessonContentDoc
          {
            LessonDocId = lessDoc.Id,
            LessonContentId = Convert.ToInt32(lessonsContentId)
          };
          Add(lessonContenDoc);
        }
      }
      if (await SaveAll())
        return lessDoc.Id;

      return 0;

    }

    public async Task<bool> SendCourseShowingLink(int lessonDocId)
    {
      var callbackUrl = _config.GetValue<String>("AppSettings:DefaultCourseShowingLink") + lessonDocId;

      var lessonContent = await _context.LessonContentDocs.Include(a => a.LessonDoc)
        .Include(a => a.LessonContent)
        .ThenInclude(a => a.Lesson)
        .ThenInclude(a => a.Theme)
        .FirstOrDefaultAsync(a => a.LessonDocId == lessonDocId);
      if (lessonContent != null)
      {
        string courseName = "";
        var classLevelId = 0; // pour le niveau
        int teacherId = lessonContent.LessonDoc.TeacherId;
        if (lessonContent.LessonContent.Lesson.ClassLevelId != null)
        {
          classLevelId = Convert.ToInt32(lessonContent.LessonContent.Lesson.ClassLevelId);
          courseName = (await _context.Courses.FirstOrDefaultAsync(a => a.Id == lessonContent.LessonContent.Lesson.CourseId)).Name;
        }
        else
        {
          classLevelId = Convert.ToInt32(lessonContent.LessonContent.Lesson.Theme.ClassLevelId);
          courseName = (await _context.Courses.FirstOrDefaultAsync(a => a.Id == lessonContent.LessonContent.Lesson.Theme.CourseId)).Name;
        }

        var teacherClasses = await GetTeacherClasses(teacherId);
        var current_classLevelClassIds = teacherClasses.Where(a => a.ClassLevelId == classLevelId).Select(a => a.ClassId);
        foreach (var classId in current_classLevelClassIds)
        {
          var students = await GetClassStudents(classId);
          foreach (var student in students)
          {
            var parents = await GetParents(student.Id);
            foreach (var parent in parents)
            {
              if (parent != null && parent.Email != null && parent.EmailConfirmed)
              {
                var parentEmail = new Email
                {
                  EmailTypeId = 1,
                  ToAddress = parent.Email,
                  Subject = "<b> Bonjour, un conténu de cours de <b>" + courseName + "<b> vient d'etre ajouté pour enfant " +
                student.LastName + " " + student.FirstName,
                  Body = $"veuillez cliquer sur le lien suivant pour acceder au contenu : <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicker ici</a>.",
                  InsertUserId = teacherId,
                  UpdateUserId = teacherId
                };
                Add(parentEmail);
              }
            }

            if (student.Email != null && student.EmailConfirmed)
            {
              var studentEmail = new Email
              {
                EmailTypeId = 1,
                ToAddress = student.Email,
                Subject = "<b> Bonjour, un conténu de cours de <b>" + courseName + "<b> vient d'etre ajouté." +
              student.LastName + " " + student.FirstName,
                Body = $"veuillez cliquer sur le lien suivant pour acceder au contenu : <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicker ici</a>.",
                InsertUserId = teacherId,
                UpdateUserId = teacherId
              };
              Add(studentEmail);

            }
          }

          if (await SaveAll())
            return true;
          else
            return false;
        }

      }
      return false;

    }

    public string GetUserIDNumber(int userId, string lastName, string firstName)
    {
      int randomVal = 300631;
      int val = userId * 2 + randomVal;
      // string idNum = lastName.Substring(0, 1).ToUpper() + firstName.Substring(0,1).ToUpper() + val.ToString().To5Digits();
      string idNum = val.ToString().To5Digits();
      return idNum;
    }

    public async Task<List<UserScheduleNDaysDto>> GetTeacherScheduleNDays(int teacherId)
    {
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();

      int nbDays = 7;
      var today = DateTime.Now.Date;

      var teacher = await GetUser(teacherId, true);
      List<UserScheduleNDaysDto> eventsForNDays = new List<UserScheduleNDaysDto>();
      //courses on the schedule
      for (int i = 0; i < nbDays; i++)
      {
        var date = today.AddDays(i);
        var dayDate = ((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek;
        UserScheduleNDaysDto userScheduleDay = new UserScheduleNDaysDto();
        userScheduleDay.UserId = teacherId;
        userScheduleDay.Day = dayDate;
        userScheduleDay.DayDate = date;
        userScheduleDay.strDayDate = date.ToString("ddd dd MMM", frC);
        var dayCourses = scheduleCourses.Where(s => s.Schedule.Day == dayDate && s.TeacherId == teacherId)
                                        .ToList();

        userScheduleDay.Events = new List<UserDayEventsDto>();
        foreach (var course in dayCourses)
        {
          Schedule schedule = course.Schedule;
          UserDayEventsDto uded = new UserDayEventsDto();
          uded.EventDate = date;
          uded.strEventDate = date.ToString("dd/MM/yy", frC);
          uded.Title = course.Course.Name;
          uded.EventTypeName = "cours";
          uded.ClassName = course.Schedule.Class.Name;
          uded.StartHour = course.Schedule.StartHourMin.Hour;
          uded.StartMin = course.Schedule.StartHourMin.Minute;
          uded.strStartHourMin = course.Schedule.StartHourMin.ToString("hh:MM", frC);
          uded.strEndHourMin = course.Schedule.EndHourMin.ToString("hh:MM", frC);
          userScheduleDay.Events.Add(uded);
        }

        userScheduleDay.Events = userScheduleDay.Events.OrderBy(e => e.StartHour).ThenBy(e => e.StartMin).ToList();
        eventsForNDays.Add(userScheduleDay);
      }

      return eventsForNDays;
    }

    public async Task<List<EventDto>> GetUserEvents(int userId)
    {
      List<User> users = await _cache.GetUsers();

      var user = users.First(u => u.Id == userId);
      List<EventDto> events = new List<EventDto>();

      //events dedicated to user
      var userEvents = await _context.Events.Where(e => e.UserId == user.Id && e.EventDate.Date >= DateTime.Now.Date)
                                            .ToListAsync();
      foreach (var item in userEvents)
      {
        EventDto eventDto = new EventDto();
        eventDto.EventDate = item.EventDate;
        eventDto.strEventDate = item.EventDate.ToString("dd/MM/yyyy", frC);
        eventDto.Title = item.Title;
        eventDto.Desc = item.Desc;
        events.Add(eventDto);
      }

      //children events for the parent
      if (user.UserTypeId == parentTypeId)
      {
        var children = await GetChildren(userId);
        foreach (var child in children)
        {
          if (child.ClassId != null)
          {
            var items = await _context.Events
                              .Include(i => i.Class)
                              .Where(e => e.ClassId == child.ClassId && e.EventDate.Date >= DateTime.Now.Date)
                              .ToListAsync();
            foreach (var aevent in items)
            {
              EventDto eventDto = new EventDto();
              eventDto.EventDate = aevent.EventDate;
              eventDto.strEventDate = aevent.EventDate.ToString("dd/MM/yyyy", frC);
              eventDto.Title = aevent.Title;
              eventDto.Desc = aevent.Desc;
              eventDto.ChildFirstName = child.FirstName;
              eventDto.ChildLastName = child.LastName;
              eventDto.ChildInitials = child.LastName.ToUpper().Substring(0, 1) +
                child.FirstName.ToUpper().Substring(0, 1);
              eventDto.ClassName = child.Class.Name;
              events.Add(eventDto);
            }
          }
        }
      }

      //student(child) events
      if (user.UserTypeId == studentTypeId)
      {
        if (user.ClassId != null)
        {
          var items = await _context.Events.Include(i => i.Class)
                                           .Where(e => e.ClassId == user.ClassId && e.EventDate.Date >= DateTime.Now.Date)
                                           .ToListAsync();
          foreach (var aevent in items)
          {
            EventDto eventDto = new EventDto();
            eventDto.EventDate = aevent.EventDate;
            eventDto.strEventDate = aevent.EventDate.ToString("dd/MM/yyyy", frC);
            eventDto.Title = aevent.Title;
            eventDto.Desc = aevent.Desc;
            events.Add(eventDto);
          }
        }
      }

      events = events.OrderBy(o => o.EventDate).ToList();
      return events;
    }

    public async Task<IEnumerable<Setting>> GetSettings()
    {
      var settings = await _context.Settings.OrderBy(s => s.Name).ToListAsync();
      return settings;
    }

    public async Task<List<EducationLevel>> GetEducationLevels()
    {
      List<EducationLevel> educationLevels = await _cache.GetEducLevels();
      var educlevels = educationLevels.OrderBy(s => s.Name).ToList();
      return educlevels;
    }

    public async Task<IEnumerable<EducationLevelDto>> GetEducationLevelsWithClasses()
    {
      List<EducationLevel> educlevelsCached = await _cache.GetEducLevels();
      List<ClassLevel> classlevelsCached = await _cache.GetClassLevels();
      List<Class> classesCached = await _cache.GetClasses();
      List<User> studentsCached = await _cache.GetStudents();

      var educLevelsFromDB = educlevelsCached.OrderBy(s => s.Name).ToList();

      List<EducationLevelDto> educLevels = new List<EducationLevelDto>();
      foreach (var educLevel in educLevelsFromDB)
      {
        EducationLevelDto eld = new EducationLevelDto();
        eld.Id = educLevel.Id;
        eld.Name = educLevel.Name;

        var classLevels = classlevelsCached.Where(cl => cl.EducationLevelId == educLevel.Id)
                                           .OrderBy(o => o.DsplSeq)
                                           .ToList();
        eld.Classes = new List<ClassDetailDto>();
        foreach (var cl in classLevels)
        {
          var classes = classesCached.Where(c => c.ClassLevelId == cl.Id).ToList();
          foreach (var aclass in classes)
          {
            ClassDetailDto cdd = new ClassDetailDto();
            cdd.Id = aclass.Id;
            cdd.Name = aclass.Name;
            cdd.ClassLevelId = cl.Id;
            cdd.CycleId = Convert.ToInt32(cl.CycleId);
            cdd.EducationLevelId = Convert.ToInt32(cl.EducationLevelId);
            cdd.SchoolId = Convert.ToInt32(cl.SchoolId);
            cdd.TotalStudents = studentsCached.Where(s => s.ClassId == aclass.Id).Count();
            eld.Classes.Add(cdd);
          }
        }
        educLevels.Add(eld);
      }
      return educLevels;
    }

    public async Task<IEnumerable<SchoolDto>> GetSchools()
    {
      List<School> schoolsCached = await _cache.GetSchools();
      List<ClassLevel> classlevelsCached = await _cache.GetClassLevels();
      List<Class> classesCached = await _cache.GetClasses();

      var schoolsFromDB = schoolsCached.OrderBy(s => s.DsplSeq).ToList();

      List<SchoolDto> schools = new List<SchoolDto>();
      foreach (var school in schoolsFromDB)
      {
        SchoolDto sd = new SchoolDto();
        sd.Id = school.Id;
        sd.Name = school.Name;

        var classLevels = classlevelsCached.Where(cl => cl.SchoolId == school.Id)
                                           .OrderBy(o => o.DsplSeq)
                                           .ToList();
        sd.Classes = new List<ClassDetailDto>();
        foreach (var cl in classLevels)
        {
          var classes = classesCached.Where(c => c.ClassLevelId == cl.Id).ToList();
          foreach (var aclass in classes)
          {
            ClassDetailDto cdd = new ClassDetailDto();
            cdd.Id = aclass.Id;
            cdd.Name = aclass.Name;
            cdd.ClassLevelId = cl.Id;
            cdd.CycleId = Convert.ToInt32(cl.CycleId);
            cdd.EducationLevelId = Convert.ToInt32(cl.EducationLevelId);
            cdd.SchoolId = Convert.ToInt32(cl.SchoolId);
            sd.Classes.Add(cdd);
          }
        }
        schools.Add(sd);
      }
      return schools;
    }

    public async Task<IEnumerable<CycleDto>> GetCycles()
    {
      List<Cycle> cyclesCached = await _cache.GetCycles();
      List<ClassLevel> classlevelsCached = await _cache.GetClassLevels();
      List<Class> classesCached = await _cache.GetClasses();

      var cyclesFromDB = await _context.Cycles.OrderBy(s => s.Name).ToListAsync();

      List<CycleDto> cycles = new List<CycleDto>();
      foreach (var cycle in cyclesFromDB)
      {
        CycleDto cd = new CycleDto();
        cd.Id = cycle.Id;
        cd.Name = cycle.Name;

        var classLevels = classlevelsCached.Where(cl => cl.EducationLevelId == cycle.Id).OrderBy(o => o.DsplSeq).ToList();
        cd.Classes = new List<ClassDetailDto>();
        foreach (var cl in classLevels)
        {
          var classes = classesCached.Where(c => c.ClassLevelId == cl.Id).ToList();
          foreach (var aclass in classes)
          {
            ClassDetailDto cdd = new ClassDetailDto();
            cdd.Id = aclass.Id;
            cdd.Name = aclass.Name;
            cdd.ClassLevelId = cl.Id;
            cdd.CycleId = Convert.ToInt32(cl.CycleId);
            cdd.EducationLevelId = Convert.ToInt32(cl.EducationLevelId);
            cdd.SchoolId = Convert.ToInt32(cl.SchoolId);
            cd.Classes.Add(cdd);
          }
        }
        cycles.Add(cd);
      }
      return cycles;
    }

    public async Task<Order> GetOrder(int id)
    {
      List<Order> orders = await _cache.GetOrders();
      List<OrderLine> lines = await _cache.GetOrderLines();

      var order = orders.FirstOrDefault(o => o.Id == id);
      order.Lines = lines.Where(o => o.OrderId == order.Id).ToList();

      return order;
    }

    public async Task<List<FinOpOrderLine>> GetChildPayments(int childId)
    {
      List<FinOpOrderLine> finOpLines = await _cache.GetFinOpOrderLines();
      var payments = finOpLines.Where(f => f.OrderLine.ChildId == childId)
                               .OrderBy(o => o.FinOp.FinOpDate)
                               .ToList();
      return payments;
    }

    public async Task<List<FinOpDto>> GetOrderPayments(int orderId)
    {
      List<FinOp> finOps = await _cache.GetFinOps();
      var paymentsFromDB = finOps.Where(f => f.OrderId == orderId).ToList();
      var payments = _mapper.Map<List<FinOpDto>>(paymentsFromDB);
      return payments;
    }

    public async Task<List<OrderLine>> GetOrderLines(int orderId)
    {
      List<OrderLine> linesCached = await _cache.GetOrderLines();
      var lines = linesCached.Where(ol => ol.OrderId == orderId).ToList();
      return lines;
    }

    public string GetInvoiceNumber(int invoiceId)
    {
      var today = DateTime.Now;
      string year = today.Year.ToString().Substring(2);
      var todaymonth = today.Month;
      string month = todaymonth.ToString().Length == 1 ? "0" + todaymonth : todaymonth.ToString();
      var todayday = today.Day;
      string day = todayday.ToString().Length == 1 ? "0" + todayday : todayday.ToString();

      var num = year + month + day + "-" + invoiceId.ToString();
      return num;
    }

    public async Task<IEnumerable<PaymentType>> GetPaymentTypes()
    {
      List<PaymentType> paymentTypesCached = await _cache.GetPaymentTypes();
      var types = paymentTypesCached.ToList();
      return types;
    }

    public async Task<IEnumerable<ClassLevel>> GetClasslevels()
    {
      var levels = await _cache.GetClassLevels();
      return levels.ToList();
    }

    public async Task<IEnumerable<Bank>> GetBanks()
    {
      List<Bank> banks = await _cache.GetBanks();
      return banks;
    }

    public async Task<NextDueAmountDto> GetChildDueAmount(int lineId, decimal paidAmount)
    {
      var lineDeadlinesCached = await _cache.GetOrderLineDeadLines();
      var today = DateTime.Now.Date;

      var lineDeadlines = lineDeadlinesCached.Where(o => o.OrderLineId == lineId)
                                             .OrderBy(o => o.DueDate)
                                             .ToList();
      decimal amountOK = lineDeadlines.Where(o => o.Paid == true).Sum(s => s.Amount + s.ProductFee);
      var deadline = new DateTime();
      var firstUnPaid = lineDeadlines.Where(o => o.Paid == false).FirstOrDefault();
      if (firstUnPaid != null)
      {
        amountOK += firstUnPaid.Amount + firstUnPaid.ProductFee;
        deadline = lineDeadlines.Where(o => o.Paid == false).First().DueDate;
      }

      NextDueAmountDto nextDueAmount = new NextDueAmountDto();
      nextDueAmount.DueAmount = amountOK - paidAmount;
      nextDueAmount.Deadline = deadline;

      return nextDueAmount;
    }

    public async Task<List<ClassLevel>> GetActiveClassLevels()
    {
      List<Class> classes = await _cache.GetClasses();
      var classLevels = classes.OrderBy(o => o.ClassLevel.DsplSeq).Select(s => s.ClassLevel)
                                                                  .Distinct().ToList();
      return classLevels;
    }

    public async Task<decimal> GetPrevChildProductPaid(DateTime currentDueDate, int childId, int productId)
    {
      List<DueDateWithLinesDto> duedates = await GetDueDatesWithLines();

      decimal prevPayments = 0;
      foreach (var duedate in duedates)
      {
        var lines = duedate.LineDeadlines.Where(d => d.OrderLine.ProductId == productId && d.OrderLine.ChildId == childId);
        var linesPaid = await GetOrderLinesPaid();

        if(duedate.DueDate != currentDueDate)
        {
          var lineids = lines.Select(s => s.OrderLineId);
          decimal paid = 0;
          foreach(var lineid in lineids)
          {
            var linePaid = linesPaid.Single(f => f.OrderLineId == lineid).Amount;
            var lined = lines.Single(d => d.OrderLineId == lineid);
            var lineDueAmount = lined.Amount + lined.ProductFee;
            decimal amountpaid = 0;
            if (linePaid >= lineDueAmount)
            {
              paid += lineDueAmount;
              amountpaid = lineDueAmount;
            }
            else
            {
              paid += linePaid;
              amountpaid = linePaid;
            }
            linesPaid.First(f => f.OrderLineId == lineid).Amount -= amountpaid;
          }

          prevPayments += paid;
        }
        else
        {
          break;
        }
      }

      return prevPayments;
    }

    public async Task<List<OrderLinePaidDto>> GetChildProductLinesPaid(List<OrderLineDeadline> childLineDeadlines, int childId,
      int productId)
    {
      List<OrderLine> lines = await _cache.GetOrderLines();
      List<FinOpOrderLine> finOpLines = await _cache.GetFinOpOrderLines();

      List<OrderLinePaidDto> linesPaid = new List<OrderLinePaidDto>();
      foreach(var lineDeadline in childLineDeadlines)
      {
        OrderLinePaidDto linePaid = new OrderLinePaidDto();
        linePaid.OrderLineId = lineDeadline.OrderLineId;
        linePaid.Amount = finOpLines
                        .Where(f => f.OrderLineId == lineDeadline.OrderLineId && f.OrderLine.ProductId == productId && f.FinOp.Cashed)
                        .Sum(s => s.Amount);
        linesPaid.Add(linePaid);
      }
      return linesPaid;
    }

    public async Task<List<OrderLinePaidDto>> GetChildOrderLinesPaid(int childId)
    {
      List<OrderLine> lines = await _cache.GetOrderLines();
      List<FinOpOrderLine> finOpLines = await _cache.GetFinOpOrderLines();

      List<OrderLinePaidDto> linesPaid = new List<OrderLinePaidDto>();
      List<OrderLine> childLines = lines.Where(o => o.ChildId == childId).ToList();
      foreach(var line in childLines)
      {
        OrderLinePaidDto linePaid = new OrderLinePaidDto();
        linePaid.OrderLineId = line.Id;
        linePaid.Amount = finOpLines.Where(f => f.OrderLineId == line.Id && f.FinOp.Cashed).Sum(s => s.Amount);
        linesPaid.Add(linePaid);
      }
      return linesPaid;
    }

    public async Task<List<OrderLinePaidDto>> GetOrderLinesPaid()
    {
      List<OrderLine> lines = await _cache.GetOrderLines();
      List<FinOpOrderLine> finOpLines = await _cache.GetFinOpOrderLines();

      List<OrderLinePaidDto> linesPaid = new List<OrderLinePaidDto>();
      foreach(var line in lines)
      {
        OrderLinePaidDto linePaid = new OrderLinePaidDto();
        if(line.ChildId != null)
          linePaid.ChildId = Convert.ToInt32(line.ChildId);
        linePaid.ProductId = line.ProductId;
        linePaid.OrderLineId = line.Id;
        linePaid.Amount = finOpLines.Where(f => f.OrderLineId == line.Id && f.FinOp.Cashed).Sum(s => s.Amount);
        linesPaid.Add(linePaid);
      }
      return linesPaid;
    }

    public string CalculateCourseTop(DateTime startHourMin, string startCourseHourMin)
    {
      var scheduleHourSize = Convert.ToDouble(Startup.StaticConfig.GetSection("AppSettings:DimHourSchedule").Value);

      var startData = startCourseHourMin.Split(":");
      int startCourseHour = Convert.ToInt32(startData[0]);
      int startCourseMin = Convert.ToInt32(startData[1]);
      DateTime startCourseHM = new DateTime(1, 01, 01, Convert.ToInt32(startCourseHour), Convert.ToInt32(startCourseMin), 0);

      TimeSpan span = startHourMin.Subtract(startCourseHM);
      double totalMins = span.TotalMinutes;
      // var netHours = Math.Abs(startHourMin.Hour - startCourseHour);
      // var netMins = Math.Abs(startCourseMin - startHourMin.Minute);
      var top = scheduleHourSize * (totalMins / 60); // + 1 * netHours;
      top = Math.Round(top, 2);
      return (top + "px").Replace(",", ".");
    }

    public async Task<LateAmountsDto> GetLateAmountsDue()
    {
      List<OrderLine> lines = await _cache.GetOrderLines();
      List<OrderLineDeadline> lineDeadLinesCached = await _cache.GetOrderLineDeadLines();
      List<FinOpOrderLine> finOpLines = await _cache.GetFinOpOrderLines();
      var today = DateTime.Now.Date;

      LateAmountsDto lateAmounts = new LateAmountsDto();
      // get amount due today
      foreach(var line in lines)
      {
        var lineDeadlines = lineDeadLinesCached.Where(o => o.OrderLineId == line.Id)
                                               .OrderBy(o => o.DueDate)
                                               .ToList();
        var amountPaid = finOpLines.Where(f => f.OrderLineId == line.Id && f.FinOp.Cashed)
                                   .Sum(s => s.Amount);

        decimal lineDueAmount = 0;
        decimal balance = amountPaid;
        if(lineDeadlines.Count() > 0)
        {
          foreach(var lineD in lineDeadlines)
          {
            lineDueAmount = lineD.Amount + lineD.ProductFee;

            if(lineD.DueDate.Date < today && !lineD.Paid)
            {
              if(lineDueAmount >= balance)
              {
                decimal lateAmount = lineDueAmount - balance;
                lateAmounts.TotalLateAmount += lateAmount;
                balance = 0;

                // split late amount in amounts of days late
                var nbDaysLate = Math.Abs((today - lineD.DueDate.Date).TotalDays);
                if (nbDaysLate <= 7)
                  lateAmounts.LateAmount7Days += lateAmount;
                else if (nbDaysLate > 7 && nbDaysLate <= 15)
                  lateAmounts.LateAmount15Days += lateAmount;
                else if (nbDaysLate > 15 && nbDaysLate <= 30)
                  lateAmounts.LateAmount30Days += lateAmount;
                else if (nbDaysLate > 30 && nbDaysLate <= 60)
                  lateAmounts.LateAmount60Days += lateAmount;
                else if (nbDaysLate > 60)
                  lateAmounts.LateAmount60DaysPlus += lateAmount;
              }
              else
              {
                balance = balance - lineDueAmount;
              }
            }
            else
            {
              if(lineD.Paid)
              {
                balance = balance - lineDueAmount;
              }
            }
          }
        }
        else // orderline has only one deadline(no split payments)
        {
          if(line.Deadline.Date < today)
          {
            if(line.AmountTTC >= balance)
            {
              decimal lateAmount = line.AmountTTC - balance;
              lateAmounts.TotalLateAmount += lateAmount;
              balance = 0;

              // split late amount in amounts of days late
              var nbDaysLate = Math.Abs((today - line.Deadline.Date).TotalDays);
              if (nbDaysLate <= 7)
                lateAmounts.LateAmount7Days += lateAmount;
              else if (nbDaysLate > 7 && nbDaysLate <= 15)
                lateAmounts.LateAmount15Days += lateAmount;
              else if (nbDaysLate > 15 && nbDaysLate <= 30)
                lateAmounts.LateAmount30Days += lateAmount;
              else if (nbDaysLate > 30 && nbDaysLate <= 60)
                lateAmounts.LateAmount60Days += lateAmount;
              else if (nbDaysLate > 60)
                lateAmounts.LateAmount60DaysPlus += lateAmount;
            }
            else
            {
              balance = balance - line.AmountTTC;
            }
          }
        }
      }

      return lateAmounts;
    }

    public async Task<LateAmountsDto> GetProductLateAmountsDue(int productId, int levelId)
    {
      List<OrderLine> linesCached = await _cache.GetOrderLines();
      List<OrderLineDeadline> lineDeadLinesCached = await _cache.GetOrderLineDeadLines();
      List<FinOpOrderLine> finOpLines = await _cache.GetFinOpOrderLines();
      var today = DateTime.Now.Date;

      LateAmountsDto lateAmounts = new LateAmountsDto();
      // get amount due today
      var lines = linesCached.Where(o => o.ClassLevelId == levelId && o.ProductId == productId).ToList();
      foreach(var line in lines)
      {
        var lineDeadlines = lineDeadLinesCached.Where(o => o.OrderLineId == line.Id)
                                               .OrderBy(o => o.DueDate)
                                               .ToList();
        var amountPaid = finOpLines.Where(f => f.OrderLineId == line.Id && f.FinOp.Cashed)
                                   .Sum(s => s.Amount);

        decimal lineDueAmount = 0;
        decimal balance = amountPaid;
        if(lineDeadlines.Count() > 0)
        {
          foreach (var lineD in lineDeadlines)
          {
            lineDueAmount = lineD.Amount + lineD.ProductFee;

            if (lineD.DueDate.Date < today && !lineD.Paid)
            {
              if (lineDueAmount >= balance)
              {
                decimal lateAmount = lineDueAmount - balance;
                lateAmounts.TotalLateAmount += lateAmount;
                balance = 0;

                // split late amount in amounts of days late
                var nbDaysLate = Math.Abs((today - lineD.DueDate.Date).TotalDays);
                if (nbDaysLate <= 7)
                  lateAmounts.LateAmount7Days += lateAmount;
                else if (nbDaysLate > 7 && nbDaysLate <= 15)
                  lateAmounts.LateAmount15Days += lateAmount;
                else if (nbDaysLate > 15 && nbDaysLate <= 30)
                  lateAmounts.LateAmount30Days += lateAmount;
                else if (nbDaysLate > 30 && nbDaysLate <= 60)
                  lateAmounts.LateAmount60Days += lateAmount;
                else if (nbDaysLate > 60)
                  lateAmounts.LateAmount60DaysPlus += lateAmount;
              }
              else
              {
                balance = balance - lineDueAmount;
              }
            }
            else
            {
              if(lineD.Paid)
              {
                balance = balance - lineDueAmount;
              }
            }
          }
        }
        else // orderline has only one deadline(no split payments)
        {
          if (line.Deadline.Date < today)
          {
            if (line.AmountTTC >= balance)
            {
              decimal lateAmount = line.AmountTTC - balance;
              lateAmounts.TotalLateAmount += lateAmount;
              balance = 0;

              // split late amount in amounts of days late
              var nbDaysLate = Math.Abs((today - line.Deadline.Date).TotalDays);
              if (nbDaysLate <= 7)
                lateAmounts.LateAmount7Days += lateAmount;
              else if (nbDaysLate > 7 && nbDaysLate <= 15)
                lateAmounts.LateAmount15Days += lateAmount;
              else if (nbDaysLate > 15 && nbDaysLate <= 30)
                lateAmounts.LateAmount30Days += lateAmount;
              else if (nbDaysLate > 30 && nbDaysLate <= 60)
                lateAmounts.LateAmount60Days += lateAmount;
              else if (nbDaysLate > 60)
                lateAmounts.LateAmount60DaysPlus += lateAmount;
            }
            else
            {
              balance = balance - line.AmountTTC;
            }
          }
        }
      }

      return lateAmounts;
    }

    public async Task<LateAmountsDto> GetChildLateAmountsDue(int productId, int childId)
    {
      List<OrderLine> linesCached = await _cache.GetOrderLines();
      List<OrderLineDeadline> lineDeadLinesCached = await _cache.GetOrderLineDeadLines();
      List<FinOpOrderLine> finOpLines = await _cache.GetFinOpOrderLines();
      var today = DateTime.Now.Date;

      LateAmountsDto lateAmounts = new LateAmountsDto();
      // get amount due today
      var lines = linesCached.Where(o => o.ChildId == childId && o.ProductId == productId)
                             .ToList();
      foreach (var line in lines)
      {
        var lineDeadlines = lineDeadLinesCached.Where(o => o.OrderLineId == line.Id)
                                               .OrderBy(o => o.DueDate)
                                               .ToList();
        var amountPaid = finOpLines.Where(f => f.OrderLineId == line.Id && f.FinOp.Cashed)
                                   .Sum(s => s.Amount);

        decimal lineDueAmount = 0;
        decimal balance = amountPaid;
        if (lineDeadlines.Count() > 0)
        {
          foreach (var lineD in lineDeadlines)
          {
            lineDueAmount = lineD.Amount + lineD.ProductFee;

            if (lineD.DueDate.Date < today && !lineD.Paid)
            {
              if (lineDueAmount >= balance)
              {
                decimal lateAmount = lineDueAmount - balance;
                lateAmounts.TotalLateAmount += lateAmount;
                balance = 0;

                // split late amount in amounts of days late
                var nbDaysLate = Math.Abs((today - lineD.DueDate.Date).TotalDays);
                if (nbDaysLate <= 7)
                  lateAmounts.LateAmount7Days += lateAmount;
                else if (nbDaysLate > 7 && nbDaysLate <= 15)
                  lateAmounts.LateAmount15Days += lateAmount;
                else if (nbDaysLate > 15 && nbDaysLate <= 30)
                  lateAmounts.LateAmount30Days += lateAmount;
                else if (nbDaysLate > 30 && nbDaysLate <= 60)
                  lateAmounts.LateAmount60Days += lateAmount;
                else if (nbDaysLate > 60)
                  lateAmounts.LateAmount60DaysPlus += lateAmount;
              }
              else
              {
                balance = balance - lineDueAmount;
              }
            }
            else
            {
              if(lineD.Paid)
              {
                balance = balance - lineDueAmount;
              }
            }
          }
        }
        else // orderline has only one deadline(no split payments)
        {
          if (line.Deadline.Date < today)
          {
            if (line.AmountTTC >= balance)
            {
              decimal lateAmount = line.AmountTTC - balance;
              lateAmounts.TotalLateAmount += lateAmount;
              balance = 0;

              // split late amount in amounts of days late
              var nbDaysLate = Math.Abs((today - line.Deadline.Date).TotalDays);
              if (nbDaysLate <= 7)
                lateAmounts.LateAmount7Days += lateAmount;
              else if (nbDaysLate > 7 && nbDaysLate <= 15)
                lateAmounts.LateAmount15Days += lateAmount;
              else if (nbDaysLate > 15 && nbDaysLate <= 30)
                lateAmounts.LateAmount30Days += lateAmount;
              else if (nbDaysLate > 30 && nbDaysLate <= 60)
                lateAmounts.LateAmount60Days += lateAmount;
              else if (nbDaysLate > 60)
                lateAmounts.LateAmount60DaysPlus += lateAmount;
            }
            else
            {
              balance = balance - line.AmountTTC;
            }
          }
        }
      }

      return lateAmounts;
    }

    public List<ClassDayCoursesDto> GetCoursesFromSchedules(IEnumerable<ScheduleForTimeTableDto> schedules)
    {
      var courses = new List<ClassDayCoursesDto>();
      foreach (var daySchedule in schedules)
      {
        ClassDayCoursesDto dayCourse = new ClassDayCoursesDto();
        foreach (var course in daySchedule.Courses)
        {
          dayCourse.CourseId = course.CourseId;
          dayCourse.CourseName = course.CourseName;
          dayCourse.CourseAbbrev = course.CourseAbbrev;
          dayCourse.TeacherId = course.TeacherId;
          dayCourse.TeacherName = course.TeacherName;
          courses.Add(dayCourse);
        }
      }
      return courses.ToList();
    }

    public string GetAppSubDomain()
    {
      string subdomain = "";
      //To get subdomain
      string[] fullAddress = _httpContext.HttpContext?.Request?.Headers?["Host"].ToString()?.Split('.');
      if (fullAddress != null)
      {
        subdomain = fullAddress[0].ToLower();
        if (subdomain == "localhost:5000" || subdomain == "www" || subdomain == "educnotes")
        {
          subdomain = "";
        }
      }

      return subdomain;
    }

    public async Task<Boolean> UserInRole(int userId, int roleId)
    {
      List<UserRole> userRolesCached = await _cache.GetUserRoles();
      UserRole userRole = userRolesCached.FirstOrDefault(u => u.UserId == userId && u.RoleId == roleId);
      return (userRolesCached != null);
    }

    public MenuItemDto FindOrLoadParent(List<MenuItemDto> menuItems, List<MenuItemDto> userMenuItems, int parentMenuItemId)
    {
      //find the menu item in the entity list
      MenuItemDto parentMenuItem = menuItems.Single(m => m.Id == parentMenuItemId);

      //check if it has a parent
      if (parentMenuItem.ParentMenuId != null)
      {
        //since this has a parent it should be added to its parent's children.
        //try to find the parent in the list already
        MenuItemDto parent = userMenuItems.FirstOrDefault(m => m.Id == Convert.ToInt32(parentMenuItem.ParentMenuId));
        if (parent == null)
        {
          //this one's parent wasn't found, so add it
          MenuItemDto newParent = FindOrLoadParent(menuItems, userMenuItems, Convert.ToInt32(parentMenuItem.ParentMenuId));
          newParent.ChildMenuItems.Add(parentMenuItem);
        }
      }

      return parentMenuItem;
    }

    public async Task<UserWithRolesDto> GetUserWithRoles(int userId)
    {
      User user = await _context.Users.Include(i => i.Photos).FirstAsync(u => u.Id == userId);
      UserWithRolesDto userWithRoles = new UserWithRolesDto();
      userWithRoles.Id = user.Id;
      userWithRoles.LastName = user.LastName;
      userWithRoles.FirstName = user.FirstName;
      Photo photo = user.Photos.FirstOrDefault(p => p.IsMain == true);
      if (photo != null)
        userWithRoles.PhotoUrl = photo.Url;
      List<UserRole> rolesFromDB = await _context.UserRoles.Include(i => i.Role)
                                                           .Where(r => r.UserId == user.Id)
                                                           .ToListAsync();
      List<Role> roles = rolesFromDB.Select(s => s.Role).ToList();
      userWithRoles.Roles = roles;

      return userWithRoles;
    }

    public async Task<List<MenuItemDto>> GetMenu(int userTypeId)
    {
      List<Menu> menus = await _cache.GetMenus();
      List<MenuItem> menuItemsCached = await _cache.GetMenuItems();

      //get userType menu
      Menu menu = menus.First(m => m.UserTypeId == userTypeId);
      List<MenuItem> menuItemsByMenuId = menuItemsCached.Where(m => m.MenuId == menu.Id).ToList();
      List<MenuItemDto> userMenuItems = _mapper.Map<List<MenuItemDto>>(menuItemsByMenuId);

      List<MenuItemDto> menuItems = new List<MenuItemDto>();
      foreach (var menuItem in userMenuItems)
      {
        //check if the menu already exists in this object
        Boolean menuExists = menuItems.FirstOrDefault(m => m.Id == menuItem.Id) != null;
        if (menuExists == false)
        {
          //doesn't exist so now check if this is a top level item
          if (menuItem.ParentMenuId == null)
          {
            //top level item so just add it
            menuItems.Add(menuItem);
          }
          else
          {
            //get the parent menu item from this object if it exists
            MenuItemDto parent = menuItems.FirstOrDefault(m => m.Id == Convert.ToInt32(menuItem.ParentMenuId));
            if (parent == null)
            {
              //if it gets here then the parent isn't in the list yet. find the parent in the list
              MenuItemDto newParentMenuItem = FindOrLoadParent(userMenuItems, menuItems, Convert.ToInt32(menuItem.ParentMenuId));

              //add the current child menu item to the newly added parent
              newParentMenuItem.ChildMenuItems.Add(menuItem);
              menuItems.Add(newParentMenuItem);
            }
            else
            {
              //parent already existed in this object. add this menu to the child of the parent
              parent.ChildMenuItems.Add(menuItem);
            }
          }
        }
      }

      return menuItems;
    }

    public async Task<List<MenuItemDto>> GetUserTypeMenu(int userTypeId, int userId)
    {
      //get userType menu
      Menu menu = await _context.Menus.FirstAsync(m => m.UserTypeId == userTypeId);
      List<MenuItem> menuItemsByMenuId = await _context.MenuItems
                                          .OrderBy(o => o.ParentMenuId).ThenBy(o => o.DsplSeq).ThenBy(o => o.DisplayName)
                                          .Where(m => m.MenuId == menu.Id).ToListAsync();
      List<MenuItemDto> userMenuItems = _mapper.Map<List<MenuItemDto>>(menuItemsByMenuId);
      List<UserRole> userRolesFromDB = await _context.UserRoles.Include(i => i.Role).Where(r => r.UserId == userId).ToListAsync();
      List<Role> userRoles = userRolesFromDB.Select(u => u.Role).ToList();
      List<RoleCapability> capabilities = await _context.RoleCapabilities.Include(i => i.Capability).ToListAsync();

      List<MenuItemDto> menuItems = new List<MenuItemDto>();
      foreach (var menuItem in userMenuItems)
      {
        Boolean hasAccessToMenuItem = await HasAccessToMenu(userId, menuItem, userRoles, userMenuItems, capabilities);
        if (hasAccessToMenuItem)
        {
          //check if the menu already exists in this object
          Boolean menuExists = menuItems.FirstOrDefault(m => m.Id == menuItem.Id) != null;
          if (menuExists == false)
          {
            //doesn't exist so now check if this is a top level item
            if (menuItem.ParentMenuId == null)
            {
              //top level item so just add it
              menuItems.Add(menuItem);
            }
            else
            {
              //get the parent menu item from this object if it exists
              MenuItemDto parent = menuItems.FirstOrDefault(m => m.Id == Convert.ToInt32(menuItem.ParentMenuId));
              if (parent == null)
              {
                //if it gets here then the parent isn't in the list yet. find the parent in the list
                MenuItemDto newParentMenuItem = FindOrLoadParent(userMenuItems, menuItems, Convert.ToInt32(menuItem.ParentMenuId));

                //add the current child menu item to the newly added parent
                newParentMenuItem.ChildMenuItems.Add(menuItem);
                menuItems.Add(newParentMenuItem);
              }
              else
              {
                //parent already existed in this object. add this menu to the child of the parent
                parent.ChildMenuItems.Add(menuItem);
              }
            }
          }
        }
      }

      return menuItems;
    }

    public async Task<Boolean> HasAccessToMenu(int userId, MenuItemDto menuItem, List<Role> userRoles, List<MenuItemDto> menuItems, List<RoleCapability> capabilities)
    {
      if (menuItem.IsAlwaysEnabled)
      {
        return true;
      }
      else
      {
        //Loop through all the roles this user is in. The first time the user has
        //access to the menu item return true. If you get through all the
        //roles then the user does not have access to this menu item.
        foreach (Role role in userRoles)
        {
          //check if the user is in this role
          // Boolean userInRole = await UserInRole(userId, role.Id);
          // if(userInRole)
          // {
          // }
          //try to find the capability with the menu item id
          List<RoleCapability> capabilitiesList = capabilities.Where(r => r.Capability.MenuItemId == menuItem.Id && r.RoleId == role.Id)
                                                              .ToList();
          foreach (RoleCapability capability in capabilitiesList)
          {
            if ((capability != null) && (capability.AccessFlag != (byte)Enums.CapabilityAccessFlag.None))
            {
              //If the record is in the table and the user has access other
              //then None then return true.
              return true;
            }
          }
        }
      }

      //If it gets here then the user didn’t have access to this menu item. BUT they
      //may have access to one of its children, now check the children and if they
      //have access to any of them return true.
      List<MenuItemDto> menuChildItems = menuItems.Where(m => m.ParentMenuId == menuItem.Id).ToList();
      if (menuChildItems.Count > 0)
      {
        foreach (MenuItemDto child in menuChildItems)
        {
          Boolean childInRole = await HasAccessToMenu(userId, child, userRoles, menuItems, capabilities);
          if (childInRole)
          {
            return true;
          }
        }
      }

      //if it never found a role with any capability then return false.
      return false;
    }

    public async Task<List<MenuCapabilitiesDto>> GetMenuCapabilities(int userTypeId, int userId)
    {
      List<Capability> capabilities = await _cache.GetCapabilities();
      List<MenuItem> menuItemsCached = await _cache.GetMenuItems();
      List<MenuItemDto> menuItems = await GetMenu(userTypeId);

      List<MenuCapabilitiesDto> menuCapabilities = new List<MenuCapabilitiesDto>();
      foreach (MenuItemDto menuItem in menuItems)
      {
        MenuCapabilitiesDto item = new MenuCapabilitiesDto();
        item.MenuItemId = menuItem.Id;
        item.MenuItemName = menuItem.DisplayName;
        List<Capability> itemCapabilities = capabilities.Where(c => c.MenuItemId == menuItem.Id).ToList();
        item.Capabilities = itemCapabilities;
        menuCapabilities.Add(item);

        if (menuItem.ChildMenuItems.Count() > 0)
        {
          foreach (MenuItemDto child in menuItem.ChildMenuItems)
          {
            MenuCapabilitiesDto childItem = new MenuCapabilitiesDto();
            childItem.MenuItemId = child.Id;
            childItem.MenuItemName = child.DisplayName;
            List<Capability> childItemCapabilities = capabilities.Where(c => c.MenuItemId == child.Id).ToList();
            childItem.Capabilities = childItemCapabilities;
            item.ChildMenuItems.Add(childItem);
          }
        }
      }

      return menuCapabilities;
    }

    public async Task<ErrorDto> SaveRole(RoleDto roleDto)
    {
      DateTime today = DateTime.Now;

      using(var identityContextTransaction = _context.Database.BeginTransaction())
      {
        ErrorDto error = new ErrorDto();
        error.NoError = true;
        try
        {
          Role role = new Role();
          role.Id = roleDto.RoleId;
          role.Name = roleDto.RoleName;

          //is it a new role?
          if (role.Id == 0)
          {
            var result = await _roleManager.CreateAsync(role);
          }
          else
          {
            role = await _context.Roles.FirstAsync(r => r.Id == roleDto.RoleId);
            role.Name = roleDto.RoleName;
            role.NormalizedName = roleDto.RoleName.ToUpper();
            // role.ConcurrencyStamp =  Guid.NewGuid().ToString();
            Update(role);

            //remove previous role capabilities
            List<RoleCapability> prevRoleCapabilities = await _context.RoleCapabilities.Where(r => r.RoleId == role.Id).ToListAsync();
            DeleteAll(prevRoleCapabilities);

            //remove previous users in the role
            List<UserRole> prevUsersInRole = await _context.UserRoles.Where(r => r.RoleId == role.Id).ToListAsync();
            DeleteAll(prevUsersInRole);
          }

          //add role capabilities
          foreach (RoleCapabilityDto capability in roleDto.Capabilities)
          {
            if (capability.AccessFlag > 0)
            {
              RoleCapability rc = new RoleCapability();
              rc.RoleId = role.Id;
              rc.CapabilityId = capability.CapabilityId;
              rc.AccessFlag = capability.AccessFlag;
              rc.InsertUserId = 1;
              rc.InsertDate = today;
              rc.UpdateUserId = 1;
              rc.UpdateDate = today;
              Add(rc);
            }
          }

          foreach (UserInRoleDto user in roleDto.UsersInRole)
          {
            UserRole userRole = new UserRole();
            userRole.UserId = user.Id;
            userRole.RoleId = role.Id;
            Add(userRole);
          }

          await SaveAll();
          identityContextTransaction.Commit();
          return error;
        }
        catch (Exception ex)
        {
          var de = ex.Message;
          identityContextTransaction.Rollback();
          error.Message = ex.Message;
          error.NoError = false;
          return error;
        }
        finally
        {
          await _cache.LoadRoles();
          await _cache.LoadRoleCapabilities();
          await _cache.LoadUserRoles();
        }
      }
    }

    public async Task<ErrorDto> SaveLevelTuitionFees(ScheduleDueDateFeeDto tuitionFeeData)
    {
      List<ProductDeadLine> productDeadlinesCached = await _cache.GetProductDeadLines();
      List<ClassLevelProduct> classlevelProducts = await _cache.GetClassLevelProducts();

      List<ClasslevelProductDto> levelProducts = tuitionFeeData.LevelProducts;
      List<ProductDeadlineDto> productDealines = tuitionFeeData.ProductDeadlines;

      ErrorDto error = new ErrorDto();
      error.NoError = true;

      using(var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          //SAVE CLASSLEVELPRODUCTS
          foreach(var levelProd in levelProducts)
          {
            if(levelProd.Id == 0)
            {
              ClassLevelProduct classlevelProd = new ClassLevelProduct {
                ProductId = levelProd.ProductId,
                ClassLevelId = levelProd.ClassLevelId,
                Price = levelProd.Price
              };
              Add(classlevelProd);
            }
            else
            {
              ClassLevelProduct classlevelProd = classlevelProducts.First(c => c.Id == levelProd.Id);
              classlevelProd.ProductId = levelProd.ProductId;
              classlevelProd.ClassLevelId = levelProd.ClassLevelId;
              classlevelProd.Price = levelProd.Price;
              Update(classlevelProd);
            }
          }

          //delete previous dueDates
          List<ProductDeadLine> prevDueDates = productDeadlinesCached.Where(p => p.ProductId == productDealines[0].ProductId).ToList();
          DeleteAll(prevDueDates);

          foreach (var item in productDealines)
          {
            string strDueDate = item.strDueDate;
            var dateData = strDueDate.Split("/");
            int day = Convert.ToInt32(dateData[0]);
            int month = Convert.ToInt32(dateData[1]);
            int year = Convert.ToInt32(dateData[2]);
            DateTime dueDate = new DateTime(year, month, day);

            ProductDeadLine newProdDeadline = new ProductDeadLine {
              ProductId = item.ProductId,
              DueDate = dueDate,
              DeadLineName = item.DeadLineName,
              Percentage = item.Percentage / 100,
              Seq = item.Seq
            };
            Add(newProdDeadline);
          }

          await SaveAll();
          identityContextTransaction.Commit();
          await _cache.LoadClassLevelProducts();
          await _cache.LoadProductDeadLines();
          return error;
        }
        catch (System.Exception)
        {
          identityContextTransaction.Rollback();
          error.NoError = false;
          error.Message = "problème pour enregistrer les données";
          return error;
        }
      }

    }

    public async Task<ErrorDto> SaveProductDeadLines(List<ProductDeadlineDto> productDeadLines)
    {
      List<ProductDeadLine> productDeadlinesCached = await _cache.GetProductDeadLines();

      ErrorDto error = new ErrorDto();
      error.NoError = true;

      foreach (var item in productDeadLines)
      {
        string strDueDate = item.strDueDate;
        var dateData = strDueDate.Split("/");
        int day = Convert.ToInt32(dateData[0]);
        int month = Convert.ToInt32(dateData[1]);
        int year = Convert.ToInt32(dateData[2]);
        DateTime dueDate = new DateTime(year, month, day);

        if(item.Id == 0)
        {
          ProductDeadLine newProdDeadline = new ProductDeadLine {
            ProductId = item.ProductId,
            DueDate = dueDate,
            DeadLineName = item.DeadLineName,
            Percentage = item.Percentage,
            Seq = item.Seq
          };
          Add(newProdDeadline);
        }
        else
        {
          ProductDeadLine prodDeadline = productDeadlinesCached.First(c => c.Id == item.Id);
          prodDeadline.ProductId = item.ProductId;
          prodDeadline.DueDate = dueDate;
          prodDeadline.DeadLineName = item.DeadLineName;
          prodDeadline.Percentage = item.Percentage;
          prodDeadline.Seq = item.Seq;
          Update(prodDeadline);
        }
      }

      if(!await SaveAll())
      {
        error.NoError = false;
        error.Message = "problème pour enregistrer les données";
      }

      return error;
    }

    public async Task<List<District>> GetDistricts()
    {
      List<District> districts = await _cache.GetDistricts();
      return districts;
    }

    public async Task<List<Zone>> GetZones()
    {
      List<Zone> zones = await _cache.GetZones();
      return zones;
    }

    public async Task<List<DueDateWithLinesDto>> GetDueDatesWithLines()
    {
      List<OrderLineDeadline> lineDeadlinesCached = await _cache.GetOrderLineDeadLines();

      var linesPaid = await GetOrderLinesPaid();
      var today = DateTime.Now.Date;
      var duedates = lineDeadlinesCached.OrderBy(o => o.DueDate).Select(s => s.DueDate).Distinct();
      List<DueDateWithLinesDto> dueDateLines = new List<DueDateWithLinesDto>();
      int i = 0;
      var olddate = new DateTime();
      foreach (var duedate in duedates)
      {
        var linedeadlines = new List<OrderLineDeadline>();
        if (i == 0)
        {
          linedeadlines = lineDeadlinesCached.Where(o => o.DueDate <= duedate).ToList();
        }
        else
        {
          linedeadlines = lineDeadlinesCached.OrderBy(o => o.DueDate)
                                             .Where(o => o.DueDate > olddate && o.DueDate <= duedate)
                                             .ToList();
        }

        decimal invoiced = linedeadlines.Sum(s => s.Amount + s.ProductFee);

        var lineids = linedeadlines.Select(s => s.OrderLineId);
        decimal paid = 0;
        foreach(var lineid in lineids)
        {
          var linePaid = linesPaid.Single(f => f.OrderLineId == lineid).Amount;
          var lined = linedeadlines.Single(d => d.OrderLineId == lineid);
          var lineDueAmount = lined.Amount + lined.ProductFee;
          decimal amountpaid = 0;
          if (linePaid >= lineDueAmount)
          {
            paid += lineDueAmount;
            amountpaid = lineDueAmount;
          }
          else
          {
            paid += linePaid;
            amountpaid = linePaid;
          }
          linesPaid.First(f => f.OrderLineId == lineid).Amount -= amountpaid;
        }

        decimal balance = invoiced - paid;

        DueDateWithLinesDto dueDateDto = new DueDateWithLinesDto();
        dueDateDto.DueDate = duedate;
        dueDateDto.strDueDate = duedate.ToString("dd/MM/yyyy", frC);
        dueDateDto.Invoiced = invoiced;
        dueDateDto.Paid = paid;
        dueDateDto.Balance = balance;
        dueDateDto.IsLate = duedate.Date < today ? true : false;
        dueDateDto.LineDeadlines = linedeadlines;
        dueDateLines.Add(dueDateDto);

        olddate = duedate;
        i++;
      }

      return dueDateLines;
    }

    public async Task<List<Periodicity>> GetPeriodicities()
    {
      List<Periodicity> periodicities = await _cache.GetPeriodicities();
      return periodicities;
    }

    public async Task<List<PayableAt>> GetPayableAts()
    {
      List<PayableAt> payableAts= await _cache.GetPayableAts();
      return payableAts;
    }

  }
}