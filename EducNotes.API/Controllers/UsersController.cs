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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EducNotes.API.Controllers {
  [ServiceFilter(typeof(LogUserActivity))]
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase {
    private readonly DataContext _context;
    private readonly IEducNotesRepository _repo;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    string password, parentRoleName, memberRoleName, moderatorRoleName, adminRoleName, professorRoleName;
    CultureInfo frC = new CultureInfo("fr-FR");
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private Cloudinary _cloudinary;
    private readonly UserManager<User> _userManager;
    int tuitionId, nextYearTuitionId, newRegToBePaidEmailId, teacherConfirmEmailId;
    int absenceTypeId, lateTypeId, educLevelPrimary, educLevelSecondary, courseTypeId, activityTypeId;
    public readonly ICacheRepository _cache;

    public UsersController(IConfiguration config, DataContext context, IEducNotesRepository repo, ICacheRepository cache,
      UserManager<User> userManager, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig) {
      _cache = cache;
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
      tuitionId = _config.GetValue<int>("AppSettings:tuitionId");
      newRegToBePaidEmailId = _config.GetValue<int>("AppSettings:newRegToBePaidEmailId");
      teacherConfirmEmailId = _config.GetValue<int>("AppSettings:teacherConfirmEmailId");
      nextYearTuitionId = _config.GetValue<int>("AppSettings:nextYearTuitionId");
      absenceTypeId = _config.GetValue<int>("AppSettings:AbsenceTypeId");
      lateTypeId = _config.GetValue<int>("AppSettings:LateTypeId");
      educLevelPrimary = _config.GetValue<int>("AppSettings:educLevelPrimary");
      educLevelSecondary = _config.GetValue<int>("AppSettings:educLevelSecondary");
      courseTypeId = _config.GetValue<int>("AppSettings:courseTypeId");
      activityTypeId = _config.GetValue<int>("AppSettings:activityTypeId");

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

    // [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("UsersWithRoles")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
      // List<User> usersCached = await _cache.GetEmployees();
      List<User> usersFromDB = await _context.Users.Include(i => i.Photos)
                                                   .OrderBy(o => o.LastName).ThenBy(i => i.FirstName)
                                                   .Where(u => u.UserTypeId == adminTypeId).ToListAsync();
      // List<UserRole> userRoles = await _cache.GetUserRoles();
      List<UserRole> userRoles = await _context.UserRoles.Include(i => i.Role).ToListAsync();

      List<UserWithRolesDto> users = new List<UserWithRolesDto>();
      foreach (var user in usersFromDB)
      {
        UserWithRolesDto userWithRoles = new UserWithRolesDto();
        userWithRoles.Id = user.Id;
        userWithRoles.LastName = user.LastName;
        userWithRoles.FirstName = user.FirstName;
        userWithRoles.Validated = user.Validated;
        Photo photo = user.Photos.FirstOrDefault(p => p.IsMain == true);
        if(photo != null)
          userWithRoles.PhotoUrl = photo.Url;
        List<Role> roles = userRoles.Where(r => r.UserId == user.Id).Select(s => s.Role).ToList();
        userWithRoles.Roles = roles;
        users.Add(userWithRoles);
      }

      return Ok(users);
    }

    [HttpGet("{id}", Name = "GetUser")]
    public async Task<IActionResult> GetUser(int id)
    {
      var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;
      var user = await _repo.GetUser(id, isCurrentUser);
      var userToReturn = _mapper.Map<UserForDetailedDto>(user);
      return Ok(userToReturn);
    }

    [HttpGet("UsersByLevel/{levelId}")]
    public async Task<IActionResult> GetStudentsByClasslevel(int levelId)
    {
      List<ClassLevel> levels =  await _cache.GetClassLevels();

      var usersFromRepo = await _repo.GetUsersByClasslevel(levelId);
      var students = _mapper.Map<List<UserForDetailedDto>>(usersFromRepo);
      ClassLevel level = levels.FirstOrDefault(c => c.Id == levelId);
      return Ok(new {
        students,
        level
      });
    }

    [HttpGet("ParentFile/{parentId}")]
    public async Task<IActionResult> GetParentFile(int parentId) {
      var parentFromRepo = await _repo.GetUser(parentId, false);
      var parent = _mapper.Map<UserForDetailedDto>(parentFromRepo);

      ParentForFileDto userFile = new ParentForFileDto();
      userFile.Id = parentId;
      userFile.LastName = parent.LastName;
      userFile.FirstName = parent.FirstName;
      userFile.idNum = parent.idNum;
      userFile.Email = parent.Email;
      userFile.Gender = parent.Gender;
      userFile.strDateOfBirth = parent.strDateOfBirth;
      userFile.Age = parent.Age;
      userFile.PhotoUrl = parent.PhotoUrl;
      userFile.PhoneNumber = parent.PhoneNumber;

      var childrenFromRepo = await _repo.GetChildren(parentId);
      var children = _mapper.Map<IEnumerable<UserForDetailedDto>>(childrenFromRepo);
      userFile.Children = new List<ParentFileChildDto>();
      foreach(var child in children) {
        ParentFileChildDto pfcd = new ParentFileChildDto();
        pfcd.Child = child;
        var line = await _context.OrderLines
          .Include(i => i.Order)
          .Where(l => l.Order.isReg && l.ChildId == child.Id)
          .FirstOrDefaultAsync();
        if(line != null) {
          pfcd.OrderId = line.OrderId;
          pfcd.OrderLineId = line.Id;
        }
        userFile.Children.Add(pfcd);
      }

      var emails = await _context.Emails
        .Include(i => i.EmailType)
        .Include(i => i.InsertUser)
        .Where(u => u.ToUserId == parentId)
        .ToListAsync();
      // var userEmails = _mapper.Map<List<EmailForListDto>>(emailFromDB);
      userFile.SmsAndEmails = new List<CommForListDto>();
      foreach(var email in emails) {
        CommForListDto commForList = new CommForListDto();
        commForList.CommType = "email";
        commForList.DateSent = email.InsertDate.ToString("dd/MM/yyyy", frC);
        commForList.HourMinSent = email.InsertDate.ToString("HH:MM", frC);
        commForList.SentBy = email.InsertUser.LastName + " " + email.InsertUser.FirstName;
        commForList.EmailType = email.EmailType.Name;
        commForList.Recipient = email.ToUser.LastName + " " + email.ToUser.FirstName;
        commForList.ToAddress = email.ToAddress;
        commForList.Subject = email.Subject;
        commForList.Body = email.Body;
        userFile.SmsAndEmails.Add(commForList);
      }

      var smses = await _context.Sms
        .Include(i => i.SmsType)
        .Include(i => i.InsertUser)
        .Where(u => u.ToUserId == parentId)
        .ToListAsync();
      foreach(var sms in smses) {
        CommForListDto commForList = new CommForListDto();
        commForList.CommType = "sms";
        commForList.DateSent = sms.InsertDate.ToString("dd/MM/yyyy", frC);
        commForList.HourMinSent = sms.InsertDate.ToString("HH:MM", frC);
        commForList.SentBy = sms.InsertUser.LastName + " " + sms.InsertUser.FirstName;
        commForList.SmsType = sms.SmsType.Name;
        commForList.Recipient = sms.ToUser.LastName + " " + sms.ToUser.FirstName;
        commForList.To = sms.To.FormatPhoneNumber();
        commForList.Content = sms.Content; //.Substring(0, 50);
        userFile.SmsAndEmails.Add(commForList);
      }

      userFile.SmsAndEmails = userFile.SmsAndEmails.OrderByDescending(o => o.DateSent).ToList();

      return Ok(userFile);
    }

    [HttpGet("ChildFile/{childId}")]
    public async Task<IActionResult> GetChildFile(int childId)
    {
      // List<OrderLine> lines = await _cache.GetOrderLines();

      // var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == childId;
      // if(!isCurrentUser)
      //   Unauthorized();

      var childFromRepo = await _repo.GetUser(childId, false);
      var child = _mapper.Map<UserForDetailedDto>(childFromRepo);

      ChildForFileDto userFile = new ChildForFileDto();
      userFile.Id = childId;
      userFile.LastName = child.LastName;
      userFile.FirstName = child.FirstName;
      userFile.idNum = child.idNum;
      userFile.Email = child.Email;
      userFile.ClassLevelId = child.ClassLevelId;
      userFile.ClassLevelName = child.ClassLevelName;
      userFile.ClassId = child.ClassId;
      userFile.ClassName = child.ClassName;
      userFile.Gender = child.Gender;
      userFile.strDateOfBirth = child.strDateOfBirth;
      userFile.Age = child.Age;
      userFile.PhotoUrl = child.PhotoUrl;
      userFile.PhoneNumber = child.PhoneNumber;

      var parentsFromRepo = await _repo.GetParents(childId);
      var parents = _mapper.Map<IEnumerable<UserForDetailedDto>>(parentsFromRepo);
      foreach(var parent in parents)
      {
        if(parent.UserTypeId == parentTypeId && parent.Gender == 0)
        {
          userFile.MotherId = parent.Id;
          userFile.MotherLastName = parent.LastName;
          userFile.MotherFirstName = parent.FirstName;
          userFile.MotherPhoneNumber = parent.PhoneNumber.FormatPhoneNumber();
          userFile.Mother2ndPhoneNumber = parent.SecondPhoneNumber.FormatPhoneNumber();
          userFile.MotherEmail = parent.Email;
          userFile.MotherPhotoUrl = parent.PhotoUrl;
        }

        if(parent.UserTypeId == parentTypeId && parent.Gender == 1)
        {
          userFile.FatherId = parent.Id;
          userFile.FatherLastName = parent.LastName;
          userFile.FatherFirstName = parent.FirstName;
          userFile.FatherPhoneNumber = parent.PhoneNumber.FormatPhoneNumber();
          userFile.Father2ndPhoneNumber = parent.SecondPhoneNumber.FormatPhoneNumber();
          userFile.FatherEmail = parent.Email;
          userFile.FatherPhotoUrl = parent.PhotoUrl;
        }
      }

      var childrenFromDB = await _repo.GetParentsChildren(userFile.MotherId, userFile.FatherId);
      var parentsChildren = childrenFromDB.Where(c => c.Id != childFromRepo.Id).ToList();
      var children = _mapper.Map<List<UserForDetailedDto>>(parentsChildren);
      userFile.Siblings = children;

      var line = await _context.OrderLines.Where(l => l.Order.isReg && l.ChildId == childId)
                                          .FirstOrDefaultAsync();
      if(line != null)
      {
        userFile.OrderId = line.OrderId;
        userFile.OrderLineId = line.Id;
      }

      return Ok(userFile);
    }

    [HttpGet("UserInfos/{id}/{parentId}")]
    public async Task<IActionResult> GetStudentInfos(int id, int parentId)
    {
      // agenda
      var toNbDays = 7;
      var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;
      var user = await _repo.GetUser(id, isCurrentUser);
      var classId = Convert.ToInt32(user.ClassId);
      var agendasFromRepo = await _repo.GetClassAgendaTodayToNDays(classId, toNbDays);
      var agendaItems = _repo.GetAgendaListByDueDate(agendasFromRepo);

      // evaluations to come
      var evalsToCome = await _repo.GetEvalsToCome(classId);

      //student grades
      var periods = await _context.Periods.OrderBy(o => o.Abbrev).ToListAsync();
      List<UserCourseEvalsDto> coursesWithEvals = await _repo.GetUserGrades(user.Id, classId);
      double courseAvgSum = 0;
      double courseCoeffSum = 0;
      double GeneralAvg = -1000;
      List<PeriodAvgDto> periodAvgs = new List<PeriodAvgDto>();
      if(coursesWithEvals.Count() > 0)
      {
        foreach(var course in coursesWithEvals) {
          courseAvgSum += course.UserCourseAvg * course.CourseCoeff;
          courseCoeffSum += course.CourseCoeff;
        }

        if(courseCoeffSum > 0)
          GeneralAvg = Math.Round(courseAvgSum / courseCoeffSum, 2);

        Period currPeriod = await _repo.GetPeriodFromDate(DateTime.Now);

        foreach(var period in periods) {
          PeriodAvgDto pad = new PeriodAvgDto();
          pad.PeriodId = period.Id;
          pad.PeriodName = period.Name;
          pad.PeriodAbbrev = period.Abbrev;
          pad.StartDate = period.StartDate;
          pad.EndDate = period.EndDate;
          //set activated period depending on startDate
          if(DateTime.Now.Date >= period.StartDate) {
            pad.activated = true;
          } else {
            pad.activated = false;
          }
          if(currPeriod != null && currPeriod.Id == period.Id)
            pad.Active = true;
          else
            pad.Active = false;
          pad.Avg = -1000;

          double periodAvgSum = 0;
          double coeffSum = 0;
          foreach(var course in coursesWithEvals) {
            var periodData = course.PeriodEvals.FirstOrDefault(p => p.PeriodId == period.Id);
            if(periodData.grades != null) {
              periodAvgSum += periodData.UserCourseAvg * course.CourseCoeff;
              coeffSum += course.CourseCoeff;
            }
          }

          if(coeffSum > 0)
            pad.Avg = Math.Round(periodAvgSum / coeffSum, 2);

          periodAvgs.Add(pad);
        }
      }

      int daysRange = 14;
      DateTime today = DateTime.Now.Date;
      var startDate = today.AddDays(-daysRange);
      var endDate = today.AddDays(daysRange);
      var itemsFromRepo = await _repo.GetClassSchedule(classId);

      int todayIndex = 0;
      List<ClassScheduleNDaysDto> scheduleDays = new List<ClassScheduleNDaysDto>();
      for(int i = 0; i < 2 * daysRange; i++)
      {
        ClassScheduleNDaysDto item = new ClassScheduleNDaysDto();
        var currentDate = startDate.AddDays(i);
        if(currentDate.Date == today) {
          todayIndex = i;
        }
        var dayInt =(int) currentDate.DayOfWeek == 0 ? 7 :(int) currentDate.DayOfWeek;
        item.Day = dayInt;
        item.DayDate = currentDate;
        item.strDayDate = currentDate.ToString("ddd dd MMM", frC);
        item.Courses = new List<ClassDayCoursesDto>();
        var daySchedules = itemsFromRepo.Where(s => s.Day == dayInt);
        item.Courses = _repo.GetCoursesFromSchedules(daySchedules);
        scheduleDays.Add(item);
      }

      // user events
      var userIdEvents = user.Id;
      if(parentId != 0)
        userIdEvents = parentId;
      var events = await _repo.GetUserEvents(userIdEvents);

      // //absences & late arrivals
      // var userAbsences = await _context.Absences.Where(a => a.UserId == user.Id).ToListAsync();
      // var absences = userAbsences.Where(a => a.AbsenceTypeId == absenceTypeId).Count();
      // var lateArrivals = userAbsences.Where(a => a.AbsenceTypeId == lateTypeId).Count();

      return Ok(new {
        agendaItems,
        evalsToCome,
        StudentAvg = GeneralAvg,
        periodAvgs = periodAvgs,
        scheduleDays,
        events,
        todayIndex
      });
    }

    [HttpGet("Account/{id}")]
    public async Task<IActionResult> GetUserAccount(int id) {
      var isCurrentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) == id;
      if(!isCurrentUser)
        return Unauthorized();

      var user = await _repo.GetUser(id, isCurrentUser);
      var parent = _mapper.Map<UserForAccountDto>(user);

      var ChildrenFromRepo = await _repo.GetChildren(id);
      var children = _mapper.Map<List<ChildForAccountDto>>(ChildrenFromRepo);

      var categories = await _context.SmsCategories.OrderBy(s => s.Name).ToListAsync();
      var smsTemplates = await _context.SmsTemplates.OrderBy(s => s.Name).ToListAsync();

      parent.Children = new List<ChildSmsDto>();
      List<ChildSmsDto> childrenSms = new List<ChildSmsDto>();
      List<SmsDataDto> activeSms = new List<SmsDataDto>();
      foreach(var child in children) {
        ChildSmsDto childWithSms = new ChildSmsDto();
        childWithSms.Child = child;
        childWithSms.SmsByCategory = new List<SmsByCategoryDto>();

        var userSms = await _context.UserSmsTemplates
          .Where(s => s.ChildId == child.Id && s.ParentId == parent.Id).ToListAsync();

        List<SmsByCategoryDto> SmsByCategory = new List<SmsByCategoryDto>();
        foreach(var cat in categories) {
          SmsByCategoryDto sbcd = new SmsByCategoryDto();
          sbcd.CategoryId = cat.Id;
          sbcd.CategoryName = cat.Name;
          sbcd.Sms = new List<UserSmsTemplateDto>();

          var SmsByCat = smsTemplates.FindAll(s => s.SmsCategoryId == cat.Id).OrderBy(s => s.Name);
          if(SmsByCat.Count() > 0) {
            SmsDataDto sdd = new SmsDataDto();
            foreach(var item in SmsByCat) {
              UserSmsTemplateDto ustd = new UserSmsTemplateDto();
              ustd.ChildId = child.Id;
              ustd.ParentId = parent.Id;
              ustd.SmsTemplateId = item.Id;
              ustd.SmsName = item.Name;
              ustd.Content = item.Content;
              ustd.SmsCategoryId = item.SmsCategoryId;
              ustd.Active = userSms.FirstOrDefault(u => u.SmsTemplateId == item.Id) != null ? true : false;
              sbcd.Sms.Add(ustd);

              if(ustd.Active) {
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

      //get children registrations(current and next year)
      Order order = new Order();
      if(user.Gender == 0)
        order = await _context.Orders.FirstOrDefaultAsync(o => o.MotherId == user.Id && o.isReg == true);
      else
        order = await _context.Orders.FirstOrDefaultAsync(o => o.FatherId == user.Id && o.isReg == true);
      if(order != null) {
        OrderDto pOrder = _mapper.Map<OrderDto>(order);
        parent.Registration = pOrder;
        var linesFromDB = await _context.OrderLines
          .Include(i => i.Product)
          .Include(i => i.Child).ThenInclude(i => i.Class)
          .Include(i => i.ClassLevel)
          .Where(o => o.OrderId == parent.Registration.Id && o.Cancelled == false).ToListAsync();
        var pOrderlines = _mapper.Map<List<OrderLineDto>>(linesFromDB);
        for(int i = 0; i < pOrderlines.Count(); i++) {
          var line = pOrderlines[i];
          var payments = await _context.OrderLineDeadlines.Where(d => d.OrderLineId == line.Id).ToListAsync();
          line.Payments = _mapper.Map<List<OrderLineDeadlineDto>>(payments);
        }

        //set child classLevel name
        for(int i = 0; i < children.Count(); i++) {
          var child = children[i];
          children[i].ClassLevelName = linesFromDB.First(o => o.ChildId == child.Id).ClassLevel.Name;
        }

        parent.Registration.Lines = new List<OrderLineDto>();
        parent.Registration.Lines = pOrderlines;
      }

      Order nextOrder = new Order();
      if(user.Gender == 0)
        nextOrder = await _context.Orders.FirstOrDefaultAsync(o => o.MotherId == user.Id && o.isNextReg == true);
      else
        nextOrder = await _context.Orders.FirstOrDefaultAsync(o => o.FatherId == user.Id && o.isNextReg == true);
      if(parent.NextRegistration != null) {
        OrderDto pnextOrder = _mapper.Map<OrderDto>(nextOrder);
        parent.NextRegistration = pnextOrder;
        var nextLinesFromDB = await _context.OrderLines
          .Include(i => i.Product)
          .Include(i => i.Child).ThenInclude(i => i.Class)
          .Where(o => o.OrderId == parent.NextRegistration.Id).ToListAsync();
        var pnextOrderlines = _mapper.Map<List<OrderLineDto>>(nextLinesFromDB);
        parent.NextRegistration.Lines = new List<OrderLineDto>();
        parent.NextRegistration.Lines = pnextOrderlines;
      }

      return Ok(new {
        parent,
        activeSms
      });
    }

    [HttpPut("{parentId}/saveSMS")]
    public async Task<IActionResult> SaveUserSMS(int parentId, [FromBody] List<SmsDataDto> smsData) {
      List<UserSmsTemplate> newUserSMS = new List<UserSmsTemplate>();
      foreach(var sms in smsData) {
        int childId = sms.ChildId;
        int smsId = sms.SmsId;

        List<UserSmsTemplate> oldUserSMS = await _context.UserSmsTemplates.Where(s => s.ChildId == childId).ToListAsync();
        if(oldUserSMS.Count() > 0)
          _repo.DeleteAll(oldUserSMS);

        UserSmsTemplate ust = new UserSmsTemplate();
        ust.ChildId = childId;
        ust.SmsTemplateId = smsId;
        ust.ParentId = parentId;
        newUserSMS.Add(ust);
      }
      _context.AddRange(newUserSMS);

      if(await _repo.SaveAll())
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
    public async Task<IActionResult> GetUserTypesDetails() {
      var users = await _context.Users.ToListAsync();
      var usertypes = await _context.UserTypes.OrderBy(a => a.Name).ToListAsync();

      var data = new List<UserTypesDto>();

      foreach(var item in usertypes) {
        data.Add(new UserTypesDto {
          UserType = item,
            Total = users.Where(a => a.UserTypeId == item.Id).Count(),
            TotalActive = users.Where(a => a.UserTypeId == item.Id && a.Validated == true).Count(),

        });
      }

      return Ok(data);
    }

    [HttpGet("{parentId}/AccountChildren")]
    public async Task<IActionResult> GetAccountChildren(int parentId) {
      var children = await _repo.GetAccountChildren(parentId);

      return Ok(new {
        children,
        parentid = parentId
      });
    }

    [HttpGet("{childId}/Siblings")]
    public async Task<IActionResult> GetSiblings(int childId) {
      var siblingsFromRepo = await _repo.GetSiblings(childId);
      var siblings = _mapper.Map<IEnumerable<UserForDetailedDto>>(siblingsFromRepo);
      return Ok(siblings);
    }

    [HttpGet("{parentId}/Children")]
    public async Task<IActionResult> GetChildren(int parentId) {
      var usersFromDB = await _repo.GetChildren(parentId);
      var users = _mapper.Map<IEnumerable<UserForDetailedDto>>(usersFromDB);

      var startDate = DateTime.Now.Date;
      var endDate = startDate.AddDays(7).Date;

      foreach(var user in users) {
        user.Avg = await _repo.GetStudentAvg(user.Id, user.ClassId);
        user.AgendaItems = await _repo.GetUserClassAgenda(user.ClassId, startDate, endDate);
        //absences & late arrivals
        var userAbsences = await _context.Absences.Where(a => a.UserId == user.Id).ToListAsync();
        user.NbAbsences = userAbsences.Where(a => a.AbsenceTypeId == absenceTypeId).Count();
        user.NbLateArrivals = userAbsences.Where(a => a.AbsenceTypeId == lateTypeId).Count();
      }

      return Ok(users);
    }

    [HttpGet("{teacherId}/ScheduleByDay")]
    public async Task<IActionResult> GetTeacherScheduleByDay(int teacherId)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();

      var scheduleItems = scheduleCourses.Where(s => s.TeacherId == teacherId).ToList();
      var days = scheduleItems.Select(d => d.Schedule.Day).Distinct().OrderBy(o => o);

      TeacherScheduleDto teacherSchedule = new TeacherScheduleDto();
      if(scheduleItems.Count() > 0)
      {
        teacherSchedule.TeacherId = teacherId;
        teacherSchedule.TeacherName = scheduleItems[0].Teacher.LastName + " " + scheduleItems[0].Teacher.FirstName;

        teacherSchedule.Days = new List<ScheduleDayDto>();
        foreach(var day in days)
        {
          ScheduleDayDto scheduleDay = new ScheduleDayDto();
          scheduleDay.Day = day;
          scheduleDay.DayName = day.DayIntToName();

          List<ScheduleCourse> dayScheduleItems = scheduleItems.Where(s => s.Schedule.Day == day)
                                                               .OrderBy(o => o.Schedule.StartHourMin).ToList();
          scheduleDay.Courses = new List<ScheduleCourseDto>();
          foreach(var item in dayScheduleItems)
          {
            Course course = item.Course;
            Schedule schedule = item.Schedule;
            ScheduleCourseDto courseDto = new ScheduleCourseDto();
            courseDto.Id = item.Id;
            courseDto.ScheduleId = item.ScheduleId;
            courseDto.CourseId = course.Id;
            courseDto.CourseName = course.Name;
            courseDto.CourseAbbrev = course.Abbreviation;
            courseDto.CourseColor = course.Color;
            courseDto.ClassId = schedule.ClassId;
            courseDto.ClassName = schedule.Class.Name;
            courseDto.StartHour = schedule.StartHourMin;
            courseDto.StartH = schedule.StartHourMin.ToString("HH:mm", frC);
            courseDto.EndHour = schedule.EndHourMin;
            courseDto.EndH = schedule.EndHourMin.ToString("HH:mm", frC);
            courseDto.InConflict = false;
            scheduleDay.Courses.Add(courseDto);
          }

          teacherSchedule.Days.Add(scheduleDay);
        }
      }

      return Ok(teacherSchedule);
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
          TotalActive = item.Users.Where(u => u.AccountDataValidated == true).Count(),
          TotalValidated = item.Users.Where(u => u.Validated == true).Count()
        };

        dataToReturn.Add(userRecap);
      }
      return Ok(dataToReturn);
    }

    [HttpGet("{teacherId}/ScheduleByClassByDay")]
    public async Task<IActionResult> GetTeacherScheduleByClassByDay(int teacherId)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();

      var scheduleItems = scheduleCourses.Where(s => s.TeacherId == teacherId).ToList();
      var classes = scheduleItems.Select(c => c.Schedule.Class).Distinct().OrderBy(o => o.Name);

      TeacherScheduleDto teacherSchedule = new TeacherScheduleDto();
      if(scheduleItems.Count() > 0)
      {
        teacherSchedule.TeacherId = teacherId;
        teacherSchedule.TeacherName = scheduleItems[0].Teacher.LastName + " " + scheduleItems[0].Teacher.FirstName;

        teacherSchedule.Classes = new List<ScheduleClassDto>();
        foreach(var aclass in classes)
        {
          ScheduleClassDto classSchedule = new ScheduleClassDto();
          classSchedule.ClassId = aclass.Id;
          classSchedule.ClassName = aclass.Name;

          var days = scheduleItems.Where(s => s.Schedule.ClassId == aclass.Id)
                                   .Select(d => d.Schedule.Day).Distinct().OrderBy(o => o);
          classSchedule.Days = new List<ScheduleDayDto>();
          foreach(var day in days)
          {
            ScheduleDayDto daySchedule = new ScheduleDayDto();
            daySchedule.Day = day;
            daySchedule.DayName = day.DayIntToName();

            List<ScheduleCourse> dayScheduleItems = scheduleItems
                                  .Where(s => s.Schedule.ClassId == aclass.Id && s.Schedule.Day == day).ToList();
            daySchedule.Courses = new List<ScheduleCourseDto>();
            foreach (var course in dayScheduleItems)
            {
              Schedule schedule = course.Schedule;
              ScheduleCourseDto courseDto = new ScheduleCourseDto();
              courseDto.CourseId = course.Course.Id;
              courseDto.CourseName = course.Course.Name;
              courseDto.CourseAbbrev = course.Course.Abbreviation;
              courseDto.CourseColor = course.Course.Color;
              courseDto.StartHour = schedule.StartHourMin;
              courseDto.StartH = schedule.StartHourMin.ToString("HH:mm", frC);
              courseDto.EndHour = schedule.EndHourMin;
              courseDto.EndH = schedule.EndHourMin.ToString("HH:mm", frC);
              daySchedule.Courses.Add(courseDto);
            }

            classSchedule.Days.Add(daySchedule);
          }

          teacherSchedule.Classes.Add(classSchedule);
        }
      }

      return Ok(teacherSchedule);
    }

    [HttpGet("{teacherId}/NextCourses")]
    public async Task<IActionResult> GetTeacherNextCourses(int teacherId)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();

      // var nextCourses = 10; // next coming courses
      var today = DateTime.Now;
      // monday=1, tue=2, ...
      var todayDay = ((int)today.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;
      var todayHourMin = today.TimeOfDay;

      var scheduleItems = scheduleCourses.Where(c => c.TeacherId == teacherId && c.Schedule.Day == todayDay)
                                         .OrderBy(o => o.Schedule.StartHourMin)
                                        //  .Take(nextCourses)
                                         .ToList();

      List<TeacherClassSessionDto> teacherCourses = new List<TeacherClassSessionDto>();
      foreach(var course in scheduleItems)
      {
        Schedule schedule = course.Schedule;
        TeacherClassSessionDto teacherSession = new TeacherClassSessionDto();
        teacherSession.ScheduleCourseId = course.Id;
        teacherSession.TeacherId = course.TeacherId;
        teacherSession.TeacherName = course.Teacher.LastName + ' ' + course.Teacher.FirstName;
        teacherSession.CourseId = course.CourseId;
        teacherSession.CourseName = course.Course.Name;
        teacherSession.ClassId = schedule.ClassId;
        teacherSession.ClassName = schedule.Class.Name;
        teacherSession.Day = schedule.Day;
        teacherSession.CourseStartHM = schedule.StartHourMin;
        teacherSession.CourseEndHM = schedule.EndHourMin;
        teacherSession.StartHourMin = schedule.StartHourMin.ToString("HH:mm", frC);
        teacherSession.EndHourMin = schedule.EndHourMin.ToString("HH:mm", frC);

        teacherCourses.Add(teacherSession);
      }

      return Ok(teacherCourses);
    }

    [HttpGet("{teacherId}/NextCoursesByClass")]
    public async Task<IActionResult> GetTeacherNextCoursesByClass(int teacherId)
    {
      var teacherCourses = await _repo.GetNextCoursesByClass(teacherId);
      return Ok(teacherCourses);
    }

    [HttpGet("{teacherId}/ScheduleToday")]
    public async Task<IActionResult> GetTeacherScheduleToday(int teacherId) {
      // monday=1, tue=2, ...
      var today =((int) DateTime.Now.DayOfWeek == 0) ? 7 :(int) DateTime.Now.DayOfWeek;

      //if saturday or sunday goes to monday schedule
      if(today == 6 || today == 7)
        today = 1;

      return await GetTeacherScheduleDay(teacherId, today);
    }

    [HttpGet("{teacherId}/ScheduleDay/{day}")]
    public async Task<IActionResult> GetTeacherScheduleDay(int teacherId, int day)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();

      var scheduleItems = scheduleCourses.Where(s => s.TeacherId == teacherId && s.Schedule.Day == day)
                                         .OrderBy(o => o.Schedule.StartHourMin)
                                         .ToList();

      List<TeacherDayCoursesDto> teacherDayCourses = new List<TeacherDayCoursesDto>();
      if(scheduleItems.Count() > 0)
      {
        TeacherDayCoursesDto teacherDayCourse = new TeacherDayCoursesDto();
        foreach (var course in scheduleItems)
        {
          Schedule schedule = course.Schedule;
          teacherDayCourse.TeacherId = course.TeacherId;
          teacherDayCourse.TeacherName = course.Teacher.LastName + " " + course.Teacher.FirstName;
          teacherDayCourse.ClassId = schedule.ClassId;
          teacherDayCourse.ClassName = schedule.Class.Name;
          teacherDayCourse.CourseId = Convert.ToInt32(course.CourseId);
          teacherDayCourse.CourseName = course.Course.Name;
          teacherDayCourse.Day = schedule.Day;
          teacherDayCourse.StartHourMin = schedule.StartHourMin.ToString("HH:mm", frC);
          teacherDayCourse.EndHourMin = schedule.EndHourMin.ToString("HH:mm", frC);
          teacherDayCourses.Add(teacherDayCourse);
        }
      }

      return Ok(teacherDayCourses);
    }

    [HttpGet("{teacherId}/Schedule")]
    public async Task<IActionResult> GetTeacherSchedule(int teacherId)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();

      var scheduleItems = scheduleCourses.Where(s => s.TeacherId == teacherId)
                                         .OrderBy(o => o.Schedule.StartHourMin)
                                         .ToList();

      List<TeacherDayCoursesDto> teacherCourses = new List<TeacherDayCoursesDto>();
      if(scheduleItems.Count() > 0)
      {
        TeacherDayCoursesDto teacherCourse = new TeacherDayCoursesDto();
        foreach (var course in scheduleItems)
        {
          Schedule schedule = course.Schedule;
          teacherCourse.TeacherId = course.TeacherId;
          teacherCourse.TeacherName = course.Teacher.LastName + " " + course.Teacher.FirstName;
          teacherCourse.ClassId = schedule.ClassId;
          teacherCourse.ClassName = schedule.Class.Name;
          teacherCourse.CourseId = course.CourseId;
          teacherCourse.CourseName = course.Course.Name;
          teacherCourse.Day = schedule.Day;
          teacherCourse.StartHourMin = schedule.StartHourMin.ToString("HH:mm", frC);
          teacherCourse.EndHourMin = schedule.EndHourMin.ToString("HH:mm", frC);
          teacherCourses.Add(teacherCourse);
        }
      }

      return Ok(teacherCourses);
    }

    [HttpGet("{teacherId}/CurrWeekSessions/{classId}")]
    public async Task<IActionResult> GetTeacherSessions(int teacherId, int classId)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();
      List<Agenda> agendasCached = await _cache.GetAgendas();
      List<ClassCourse> classCoursesCached = await _cache.GetClassCourses();

      var teacherSchedule = scheduleCourses.Where(s => s.TeacherId == teacherId && s.Schedule.ClassId == classId)
                                           .ToList();

      var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
      var dayInt =(int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
      var monday = today.AddDays(1 - dayInt);
      var saturday = monday.AddDays(5);

      var agendas = new List<SessionForListDto>();
      for(int i = 0; i < 7; i++)
      {
        var currentDate = monday.AddDays(i).Date;
        var day =((int)currentDate.DayOfWeek == 0) ? 7 :(int)currentDate.DayOfWeek;
        if(day == 7) { continue; }

        SessionForListDto sessionDto = new SessionForListDto();
        sessionDto.DueDate = currentDate;
        sessionDto.ShortDueDate = currentDate.ToString("ddd dd MMM", frC);
        sessionDto.LongDueDate = currentDate.ToString("dd MMMM yyyy", frC);
        sessionDto.DueDateAbbrev = currentDate.ToString("ddd dd", frC).Replace(".", "");
        //get agenda tasks Done Status
        sessionDto.AgendaItems = new List<AgendaToReturnDto>();

        //agenda items retrieved from schedule items
        var daySchedule = teacherSchedule.Where(d => d.Schedule.Day == day && d.Schedule.ClassId == classId)
                                         .OrderBy(d => d.Schedule.StartHourMin.Hour)
                                         .ThenBy(d => d.Schedule.StartHourMin.Minute);

        foreach(var course in daySchedule)
        {
          Schedule schedule = course.Schedule;
          string startHour = schedule.StartHourMin.ToString("HH:mm", frC);
          string endHour = schedule.EndHourMin.ToString("HH:mm", frC);
          var tasks = "";
          var id = 0;
          var agenda = agendasCached.SingleOrDefault(a => a.Session.SessionDate.Date == currentDate &&
            a.Session.TeacherId == teacherId && a.Session.ClassId == schedule.ClassId &&
            a.Session.StartHourMin.ToString("HH:mm", frC) == startHour &&
            a.Session.EndHourMin.ToString("HH:mm", frC) == endHour);
          if(agenda != null)
          {
            tasks = agenda.TaskDesc;
            id = agenda.Id;
          }

          var session = await _repo.GetSessionFromSchedule(course, currentDate);
          var newAgenda = new AgendaToReturnDto
          {
            Id = id,
            SessionId = session.Id,
            TeacherId = Convert.ToInt32(course.TeacherId),
            TeacherName = course.Teacher.LastName + ' ' + course.Teacher.FirstName,
            CourseId = Convert.ToInt32(course.CourseId),
            strDayDate = currentDate.ToString("dd/MM/yyyy", frC),
            DayDate = currentDate,
            Day = schedule.Day,
            CourseName = course.Course.Name,
            CourseColor = course.Course.Color,
            ClassId = Convert.ToInt32(schedule.ClassId),
            ClassName = schedule.Class.Name,
            Tasks = tasks,
            StartHourMin = schedule.StartHourMin.ToString("HH:mm", frC),
            EndHourMin = schedule.EndHourMin.ToString("HH:mm", frC)
          };

          sessionDto.AgendaItems.Add(newAgenda);
        }

        //retrieve agenda items with lost shceduleId(schedule item has been deleted/updated)
        var itemsWithNoScheduleId = agendasCached
                                      .Where(a => a.Session.SessionDate.Date == currentDate.Date &&
                                        a.Session.ScheduleCourseId == null)
                                      .OrderBy(o => o.Session.StartHourMin.Hour)
                                      .ThenBy(o => o.Session.StartHourMin.Minute)
                                      .ToList();

        foreach(var item in itemsWithNoScheduleId)
        {
          var itemDayInt =((int) item.Session.SessionDate.DayOfWeek == 0) ? 7 :(int) currentDate.DayOfWeek;
          if(day == 7) { continue; }

          var newAgenda = new AgendaToReturnDto
          {
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

          sessionDto.AgendaItems.Add(newAgenda);
        }

        agendas.Add(sessionDto);
      }

      var days = new List<string>();
      var weekDates = new List<DateTime>();
      var nbTasks = new List<int>();
      for(int i = 0; i <= 5; i++) {
        DateTime dt = monday.AddDays(i);
        var shortdate = dt.ToString("ddd dd MMM", frC);
        days.Add(shortdate);
        weekDates.Add(dt.Date);
      }

      var itemsFromRepo = await _repo.GetClassAgenda(classId, monday, saturday);
      var items = _repo.GetAgendaListByDueDate(itemsFromRepo);

      List<ClassCourse> classCoursesFromDB = classCoursesCached.ToList();
      var classCourses = classCoursesFromDB.Where(c => c.ClassId == classId && c.TeacherId == teacherId)
                                           .Select(s => s.Course).ToList();

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

      foreach(var item in agendas) {
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

    [HttpGet("{teacherId}/ScheduleNDays")]
    public async Task<IActionResult> GetScheduleNDays(int teacherId)
    {
      var events = await _repo.GetTeacherScheduleNDays(teacherId);
      return Ok(events);
    }

    [HttpGet("{teacherId}/SessionsFromToday/{classId}")]
    public async Task<IActionResult> GetTeacherSessionsFromToday(int teacherId, int classId)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();
      List<Agenda> agendasCached = await _cache.GetAgendas();
      List<ClassCourse> classCoursesCached = await _cache.GetClassCourses();

      var teacherSchedule = scheduleCourses.Where(s => s.TeacherId == teacherId && s.Schedule.ClassId == classId)
                                           .ToList();

      var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
      var dayInt = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
      if(dayInt == 7)
        today = today.AddDays(1);

      var agendas = new List<SessionForListDto>();
      for(int i = 0; i < 7; i++)
      {
        var currentDate = today.AddDays(i).Date;
        var day =((int)currentDate.DayOfWeek == 0) ? 7 :(int)currentDate.DayOfWeek;
        if(day == 7) { continue; }

        SessionForListDto sessionDto = new SessionForListDto();
        sessionDto.DueDate = currentDate;
        sessionDto.ShortDueDate = currentDate.ToString("ddd dd MMM", frC);
        sessionDto.LongDueDate = currentDate.ToString("dd MMMM yyyy", frC);
        sessionDto.DueDateAbbrev = currentDate.ToString("ddd dd", frC).Replace(".", "");
        //get agenda tasks Done Status
        sessionDto.AgendaItems = new List<AgendaToReturnDto>();

        //agenda items retrieved from schedule items
        var daySchedule = teacherSchedule.Where(d => d.Schedule.Day == day && d.Schedule.ClassId == classId)
                                         .OrderBy(d => d.Schedule.StartHourMin.Hour)
                                         .ThenBy(d => d.Schedule.StartHourMin.Minute)
                                         .ToList();

        foreach(var course in daySchedule)
        {
          Schedule schedule = course.Schedule;
          string startHour = schedule.StartHourMin.ToString("HH:mm", frC);
          string endHour = schedule.EndHourMin.ToString("HH:mm", frC);
          var tasks = "";
          var id = 0;
          var agenda = agendasCached.SingleOrDefault(a => a.Session.SessionDate.Date == currentDate &&
            a.Session.TeacherId == teacherId && a.Session.ClassId == schedule.ClassId &&
            a.Session.StartHourMin.ToString("HH:mm", frC) == startHour &&
            a.Session.EndHourMin.ToString("HH:mm", frC) == endHour);
          if(agenda != null)
          {
            tasks = agenda.TaskDesc;
            id = agenda.Id;
          }

          var session = await _repo.GetSessionFromSchedule(course, currentDate);
          var newAgenda = new AgendaToReturnDto
          {
            Id = id,
            SessionId = session.Id,
            TeacherId = Convert.ToInt32(course.TeacherId),
            TeacherName = course.Teacher.LastName + ' ' + course.Teacher.FirstName,
            CourseId = Convert.ToInt32(course.CourseId),
            strDayDate = currentDate.ToString("dd/MM/yyyy", frC),
            DayDate = currentDate,
            Day = schedule.Day,
            CourseName = course.Course.Name,
            CourseColor = course.Course.Color,
            ClassId = schedule.ClassId,
            ClassName = schedule.Class.Name,
            Tasks = tasks,
            StartHourMin = schedule.StartHourMin.ToString("HH:mm", frC),
            EndHourMin = schedule.EndHourMin.ToString("HH:mm", frC)
          };

          sessionDto.AgendaItems.Add(newAgenda);
        }

        //retrieve agenda items with lost shceduleId(schedule item has been deleted/updated)
        var itemsWithNoScheduleId = agendasCached
                                    .Where(a => a.Session.SessionDate.Date == currentDate.Date &&
                                      a.Session.ScheduleCourseId == null)
                                    .OrderBy(o => o.Session.StartHourMin.Hour)
                                    .ThenBy(o => o.Session.StartHourMin.Minute)
                                    .ToList();

        foreach(var item in itemsWithNoScheduleId)
        {
          var itemDayInt =((int)item.Session.SessionDate.DayOfWeek == 0) ? 7 :(int)currentDate.DayOfWeek;
          if(day == 7) { continue; }

          var newAgenda = new AgendaToReturnDto
          {
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

          sessionDto.AgendaItems.Add(newAgenda);
        }

        agendas.Add(sessionDto);
      }

      var days = new List<string>();
      var weekDates = new List<DateTime>();
      var nbTasks = new List<int>();
      for(int i = 0; i < 7; i++) {
        DateTime dt = today.AddDays(i);
        var day =((int) dt.DayOfWeek == 0) ? 7 :(int) dt.DayOfWeek;
        if(day == 7) { continue; }
        var shortdate = dt.ToString("ddd dd MMM", frC);
        days.Add(shortdate);
        weekDates.Add(dt.Date);
      }

      var itemsFromRepo = await _repo.GetClassAgenda(classId, today, today.AddDays(6));
      var items = _repo.GetAgendaListByDueDate(itemsFromRepo);

      var classCourses = classCoursesCached.Where(c => c.ClassId == classId && c.TeacherId == teacherId)
                                           .Select(s => s.Course).ToList();

      List<CourseTasksDto> coursesWithTasks = new List<CourseTasksDto>();
      foreach(var course in classCourses)
      {
        CourseTasksDto ctd = new CourseTasksDto();
        var nbItems = itemsFromRepo.Where(a => a.Session.CourseId == course.Id).ToList().Count();
        ctd.CourseId = course.Id;
        ctd.CourseName = course.Name;
        ctd.CourseAbbrev = course.Abbreviation;
        ctd.CourseColor = course.Color;
        ctd.NbTasks = nbItems;
        coursesWithTasks.Add(ctd);
      }

      foreach(var item in agendas)
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

    [HttpGet("{teacherId}/MovedWeekSessions/{classId}")]
    public async Task<IActionResult> getClassMovedWeekAgenda(int teacherId, int classId, [FromQuery] AgendaParams agendaParams)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();
      List<ClassCourse> classCoursesCached = await _cache.GetClassCourses();
      List<Agenda> agendasCached = await _cache.GetAgendas();

      var teacherSchedule = scheduleCourses.Where(s => s.TeacherId == teacherId && s.Schedule.ClassId == classId)
                                           .ToList();

      var FromDate = agendaParams.DueDate.Date;
      var move = agendaParams.MoveWeek;
      var date = FromDate.AddDays(move);
      var dateDay = (int)date.DayOfWeek;

      var dayInt = dateDay == 0 ? 7 : dateDay;
      if(dayInt == 7)
        date = date.AddDays(1);

      var agendas = new List<SessionForListDto>();

      // cahier de textes - periode de sessions des cours du professeur
      for(int i = 0; i < 7; i++)
      {
        var currentDate = date.AddDays(i);
        var day =((int)currentDate.DayOfWeek == 0) ? 7 :(int)currentDate.DayOfWeek;
        if(day == 7) { continue; }

        SessionForListDto sessionDto = new SessionForListDto();
        sessionDto.DueDate = currentDate;
        sessionDto.ShortDueDate = currentDate.ToString("ddd dd MMM", frC);
        sessionDto.LongDueDate = currentDate.ToString("dd MMMM yyyy", frC);
        sessionDto.DueDateAbbrev = currentDate.ToString("ddd dd", frC).Replace(".", "");

        //get agenda tasks Done Status
        sessionDto.AgendaItems = new List<AgendaToReturnDto>();
        var daySchedule = teacherSchedule.Where(d => d.Schedule.Day == day).OrderBy(d => d.Schedule.StartHourMin);
        if(daySchedule.Count() == 0) { continue; }

        foreach(var course in daySchedule)
        {
          Schedule schedule = course.Schedule;
          string startHour = schedule.StartHourMin.ToString("HH:mm", frC);
          string endHour = schedule.EndHourMin.ToString("HH:mm", frC);
          var tasks = "";
          var id = 0;
          var agenda = agendasCached.SingleOrDefault(a => a.Session.SessionDate.Date == currentDate &&
            a.Session.TeacherId == teacherId && a.Session.ClassId == schedule.ClassId &&
            a.Session.StartHourMin.ToString("HH:mm", frC) == startHour &&
            a.Session.EndHourMin.ToString("HH:mm", frC) == endHour);
          if(agenda != null)
          {
            tasks = agenda.TaskDesc;
            id = agenda.Id;
          }

          var session = await _repo.GetSessionFromSchedule(course, currentDate);
          int courseId = 0;
          if(course != null)
            courseId = Convert.ToInt32(course.CourseId);
          var newAgenda = new AgendaToReturnDto
          {
            Id = id,
            SessionId = session.Id,
            TeacherId = course.TeacherId,
            TeacherName = course.Teacher.LastName + ' ' + course.Teacher.FirstName,
            CourseId = Convert.ToInt32(course.CourseId),
            strDayDate = currentDate.ToString("dd/MM/yyyy", frC),
            DayDate = currentDate,
            Day = schedule.Day,
            CourseName = course.Course.Name,
            CourseColor = course.Course.Color,
            ClassId = schedule.ClassId,
            ClassName = schedule.Class.Name,
            Tasks = tasks,
            StartHourMin = schedule.StartHourMin.ToString("HH:mm", frC),
            EndHourMin = schedule.EndHourMin.ToString("HH:mm", frC)
          };

          sessionDto.AgendaItems.Add(newAgenda);
        }

        sessionDto.NbItems = daySchedule.Count();
        agendas.Add(sessionDto);
      }

      var days = new List<string>();
      var weekDates = new List<DateTime>();
      var nbTasks = new List<int>();
      for(int i = 0; i < 7; i++) {
        DateTime dt = date.AddDays(i);
        var day =((int) dt.DayOfWeek == 0) ? 7 :(int) dt.DayOfWeek;
        if(day == 7) { continue; }
        var shortdate = dt.ToString("ddd dd MMM", frC);
        days.Add(shortdate);
        weekDates.Add(dt);
      }

      var itemsFromRepo = await _repo.GetClassAgenda(classId, date, date.AddDays(6));
      var classCourses = classCoursesCached.Where(c => c.ClassId == classId && c.TeacherId == teacherId)
                                           .Select(s => s.Course).ToList();

      List<CourseTasksDto> coursesWithTasks = new List<CourseTasksDto>();
      foreach(var course in classCourses)
      {
        CourseTasksDto ctd = new CourseTasksDto();
        var nbItems = itemsFromRepo.Where(a => a.Session.CourseId == course.Id).ToList().Count();
        ctd.CourseId = course.Id;
        ctd.CourseName = course.Name;
        ctd.CourseAbbrev = course.Abbreviation;
        ctd.CourseColor = course.Color;
        ctd.NbTasks = nbItems;
        coursesWithTasks.Add(ctd);
      }

      if(itemsFromRepo != null)
      {
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
    public async Task<IActionResult> GetGradesData(int teacherId, int periodId) {
      var teacherCourses = await _repo.GetTeacherCourses(teacherId);
      var teacherClasses = await _repo.GetTeacherClasses(teacherId);
      var classesWithEvals = await _repo.GetTeacherClassesWithEvalsByPeriod(teacherId, periodId);

      return Ok(new {
        teacherCourses,
        teacherClasses,
        classesWithEvals
      });
    }

    [HttpGet("{teacherId}/Courses")]
    public async Task<IActionResult> GetTeacherCourses(int teacherId)
    {
      var coursesFromRepo = await _repo.GetTeacherCourses(teacherId);
      List<CourseDto> allCourses = _mapper.Map<List<CourseDto>>(coursesFromRepo);
      List<CourseDto> courses = allCourses.Where(c => c.CourseTypeId == courseTypeId).ToList();
      List<CourseDto> activities = allCourses.Where(c => c.CourseTypeId == activityTypeId).ToList();
      return Ok(new {
        courses,
        activities
      });
    }

    [HttpGet("{teacherId}/teacherWithCourses")]
    public async Task<IActionResult> GetTeacherWithCourses(int teacherId)
    {
      List<User> teachers = await _cache.GetTeachers();
      List<ClassCourse> classCourses = await _cache.GetClassCourses();

      var teacherFromDB = teachers.FirstOrDefault(u => u.Id == teacherId);
      TeacherForEditDto teacher = _mapper.Map<TeacherForEditDto>(teacherFromDB);

      var courses = await _repo.GetTeacherCourses(teacherId);
      string listCourses = "";
      teacher.ClassesAssigned = new List<CourseDto>();
      foreach(var course in courses)
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
        cd.Abbrev = course.Abbreviation;
        // does this course has classes assigned to it?
        cd.ClassesAssigned = false;
        var classesAssigned = classCourses.Where(c => c.TeacherId == teacherId && c.CourseId == course.Id).ToList();
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
      List<User> teacherCached = await _cache.GetTeachers();
      var teacherFromDB = teacherCached.FirstOrDefault(u => u.Id == teacherId);
      var teacher = _mapper.Map<User>(teacherFromDB);
      
      if(teacherFromDB != null)
      {
        List<TeacherCourse> teacherCoursesCached = await _cache.GetTeacherCourses();
        List<ClassLevel> classLevelsCached = await _cache.GetClassLevels();
        List<ClassCourse> classCoursesCached = await _cache.GetClassCourses();
        List<Class> classesCached = await _cache.GetClasses();

        List<AssignedClassesDto> classes = new List<AssignedClassesDto>();
        var courses = teacherCoursesCached.Where(t => t.TeacherId == teacherId).Select(c => c.Course).ToList();
        
        foreach(var course in courses)
        {
          AssignedClassesDto assignedClasses = new AssignedClassesDto();
          assignedClasses.CourseId = course.Id;
          assignedClasses.CourseName = course.Name;

          assignedClasses.Levels = new List<LevelWithClassesDto>();
          var levels = classLevelsCached.Where(c => c.EducationLevelId == teacher.EducLevelId)
                                        .OrderBy(c => c.DsplSeq).ToList();
          foreach(var level in levels)
          {
            LevelWithClassesDto lwcd = new LevelWithClassesDto();
            var selectedIds = classCoursesCached.Where(c => c.TeacherId == teacherId && c.CourseId == course.Id &&
              c.Class.ClassLevelId == level.Id).Select(t => t.ClassId).Distinct().ToList();
              
            List<Class> classesByLevel = classesCached.Where(c => c.ClassLevelId == level.Id)
                                                      .OrderBy(o => o.ClassLevel.DsplSeq).ToList();
            if(classesByLevel.Count() > 0)
            {
              lwcd.LevelId = level.Id;
              lwcd.LevelName = level.Name;
              lwcd.Classes = new List<ClassIdAndNameDto>();
              foreach(var aclass in classesByLevel)
              {
                Boolean assigned = classCoursesCached
                  .Where(c => c.CourseId == course.Id && c.ClassId == aclass.Id && c.TeacherId != teacherId).Count() > 0;

                ClassIdAndNameDto cd = new ClassIdAndNameDto();
                cd.ClassId = aclass.Id;
                cd.ClassName = aclass.Name;
                cd.Active = selectedIds.IndexOf(aclass.Id) != -1 ? true : false;
                if(assigned)
                {
                  cd.Assigned = assigned;
                  cd.Active = true;
                }
                lwcd.Classes.Add(cd);
              }
              assignedClasses.Levels.Add(lwcd);
            }
          }
          classes.Add(assignedClasses);
        }

        return Ok(new {
          classes,
          teacher
        });
      }

      return BadRequest("l'enseignant est introuvable!");
    }

    [HttpPost("{teacherId}/AssignClasses")]
    public async Task<IActionResult> AssignClasses(int teacherId, [FromBody] List<AssignedClassesDto> courses)
    {
      List<ClassCourse> classCourses = await _cache.GetClassCourses();

      Boolean dataToBeSaved = false;
      foreach(var course in courses)
      {
        foreach(var level in course.Levels)
        {
          //delete previous classes selection
          List<ClassCourse> prevClasses = classCourses.Where(c => c.CourseId == course.CourseId && 
                                                        c.Class.ClassLevelId == level.LevelId && c.TeacherId == teacherId)
                                                      .ToList();
          if(prevClasses.Count() > 0)
          {
            _repo.DeleteAll(prevClasses);
            dataToBeSaved = true;
          }

          // add new classes selection
          List<ClassCourse> newSelection = new List<ClassCourse>();
          foreach(var aclass in level.Classes)
          {
            if(aclass.Active == true && !aclass.Assigned)
            {
              ClassCourse classCourse = new ClassCourse() {
              ClassId = aclass.ClassId,
              CourseId = course.CourseId,
              TeacherId = teacherId
              };
              _repo.Add(classCourse);
              dataToBeSaved = true;
            }
          }
        }
      }

      if(dataToBeSaved && await _repo.SaveAll())
      {
        await _cache.LoadClassCourses();
        return Ok();
      }
      else
      {
        return NoContent();
      }
    }

    [HttpPost("{id}/course/{courseId}/level/{levelId}/AddClasses")]
    public async Task<IActionResult> AddClasses(int id, int courseId, int levelId, [FromBody] List<int> classIds) {
      // delete previous classes selection for the teacher
      var prevClasses = await _context.ClassCourses
        .Where(c => c.CourseId == courseId && c.Class.ClassLevelId == levelId)
        .ToListAsync();

      if(prevClasses.Count() > 0)
      {
        _repo.DeleteAll(prevClasses);
      }

      // add new classes selection
      foreach(var classId in classIds)
      {
        ClassCourse cc = new ClassCourse();
        cc.TeacherId = id;
        cc.ClassId = classId;
        cc.CourseId = courseId;
        _repo.Add(cc);
      }

      if(await _repo.SaveAll())
      {
        // await _cache.LoadClassCourses();
        return Ok();
      }

      return BadRequest("problème pour affecter les classes");
    }

    [HttpGet("{teacherId}/Classes")]
    public async Task<IActionResult> GetTeacherClasses(int teacherId)
    {
      // if(teacherId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
      //     return Unauthorized();
      // List<User> users = await _cache.GetUsers();
      var teacherClasses = await(from courses in _context.ClassCourses
          join classes in _context.Classes on courses.ClassId equals classes.Id
          where courses.TeacherId == teacherId select new {
            ClassId = classes.Id,
            ClassName = classes.Name,
            NbStudents = _context.Users.Where(u => u.ClassId == classes.Id &&
              u.UserTypeId == studentTypeId).Count()
        })
        .OrderBy(o => o.ClassName)
        .Distinct().ToListAsync();

      return Ok(teacherClasses);
    }

    [HttpGet("{TeacherId}/period/{periodId}/ClassesWithEvalsByPeriod")]
    public async Task<IActionResult> GetTeacherClassesWithEvalsByPeriod(int teacherId, int periodId)
    {
      var Classes = await _context.ClassCourses
                    .Where(c => c.TeacherId == teacherId).Distinct().ToListAsync();
      // List<User> studentsCached = await _cache.GetStudents();

      List<ClassesWithEvalsDto> classesWithEvals = new List<ClassesWithEvalsDto>();
      foreach(var aclass in Classes) {
        List<Evaluation> ClassEvals = await _context.Evaluations
                                            .Include(i => i.Course)
                                            .Include(i => i.EvalType)
                                            .Where(e => e.ClassId == aclass.ClassId && e.PeriodId == periodId)
                                            .ToListAsync();

        if(ClassEvals.Count > 0) {
          var OpenedEvals = ClassEvals.FindAll(e => e.Closed == false);
          var OpenedEvalsDto = _mapper.Map<List<EvaluationForListDto>>(OpenedEvals);
          var ToBeGradedEvals = ClassEvals.FindAll(e => e.Closed == true);
          var ToBeGradedEvalsDto = _mapper.Map<List<EvaluationForListDto>>(ToBeGradedEvals);
          var NbEvals = OpenedEvals.Count() + ToBeGradedEvals.Count();

          ClassesWithEvalsDto classDto = new ClassesWithEvalsDto();
          classDto.ClassId = Convert.ToInt32(aclass.ClassId);
          classDto.ClassName = aclass.Class.Name;
          classDto.NbStudents = await _context.Users.Where(s => s.ClassId == aclass.Id).CountAsync();
          classDto.NbEvals = NbEvals;
          classDto.OpenedEvals = OpenedEvalsDto;
          classDto.ToBeGradedEvals = ToBeGradedEvalsDto;

          classesWithEvals.Add(classDto);
        }
      }

      return Ok(classesWithEvals);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto) {
      if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      var userFromRepo = await _repo.GetUser(id, true);

      _mapper.Map(userForUpdateDto, userFromRepo);

      if(await _repo.SaveAll())
        return NoContent();

      throw new Exception($"Updating user {id} failed on save");
    }

    [HttpPost("{id}/like/{recipientId}")]
    public async Task<IActionResult> LikeUser(int id, int recipientId) {
      if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      var like = await _repo.GetLike(id, recipientId);

      if(like != null)
        return BadRequest("you already liked this user");

      if(await _repo.GetUser(recipientId, false) == null)
        return NotFound();

      like = new Models.Like {
        LikerId = id,
        LikeeId = recipientId
      };

      _repo.Add<Like>(like);

      if(await _repo.SaveAll())
        return Ok();

      return BadRequest("Failed to like user");
    }

    [HttpPut("SaveAbsence")]
    public async Task<IActionResult> SaveAbsence([FromBody] Absence absence) {
      var currPeriod = await _repo.GetPeriodFromDate(absence.StartDate);
      absence.PeriodId = currPeriod.Id;
      _repo.Add(absence);

      if(await _repo.SaveAll())
        return NoContent();

      throw new Exception($"l'ajout de l'absence a échoué");
    }

    [HttpGet("{userId}/ClassLifeData")]
    public async Task<IActionResult> GetClassLifeData(int userId) {
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
    public async Task<IActionResult> GetStudentLifeData(int userId) {
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
    public async Task<IActionResult> GetAbsences(int userId) {
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
    public async Task<IActionResult> SaveClassEvent([FromBody] UserClassEvent userClassLife) {
      var currPeriod = await _repo.GetPeriodFromDate(userClassLife.StartDate);
      userClassLife.PeriodId = currPeriod.Id;
      _repo.Add(userClassLife);

      if(await _repo.SaveAll())
        return NoContent();

      throw new Exception($"l'ajout de l'évènement a échoué");
    }

    [HttpPut("SaveSanction")]
    public async Task<IActionResult> SaveSanction([FromBody] UserSanction userSanction) {
      _repo.Add(userSanction);

      if(await _repo.SaveAll())
        return NoContent();

      throw new Exception($"l'ajout de la sanction à échoué");
    }

    [HttpPut("SaveReward")]
    public async Task<IActionResult> SaveReward([FromBody] UserReward userReward) {
      _repo.Add(userReward);

      if(await _repo.SaveAll())
        return NoContent();

      throw new Exception($"l'ajout de l'encouragement à échoué");
    }

    [HttpGet("GetAllClassesCourses")]
    public async Task<IActionResult> GetClassesCourses()
    {
      // List<ClassCourse> classCoursesCached = await _cache.GetClassCourses();
      var classes = await _context.Classes.Where(a => a.Active == 1).ToListAsync();
      var ccourses = new List<ClassCoursesDto>();
      foreach(var aclass in classes) {
        ccourses.Add(new ClassCoursesDto {
          Class = aclass,
            Courses = await _context.ClassCourses.Where(e => e.ClassId == aclass.Id).Select(e => e.Course).ToListAsync()
        });
      }
      return Ok(ccourses);
    }

    [HttpGet("{email}/VerifyEmail")]
    public async Task<IActionResult> VerifyEmail(string email) {
      return Ok(await _repo.EmailExist(email));
    }

    [HttpPost("{id}/updateUserType/{typeName}")]

    public async Task<IActionResult> updateUserType(int id, string typeName) {
      var userType = await _context.UserTypes.FirstOrDefaultAsync(a => a.Id == id);
      userType.Name = typeName;
      _repo.Update(userType);
      if(await _repo.SaveAll())
        return NoContent();
      return BadRequest("impossible de faire la mise à jour");
    }

    [HttpPost("AddUserType")]
    public async Task<IActionResult> AddUserType(UserType userType) {
      _repo.Add(userType);
      if(await _repo.SaveAll())
        return Ok(userType);
      return BadRequest("impossible d'ajouter cet élément");
    }

    [HttpPost("{id}/DeleteUserType")]
    public async Task<IActionResult> DeleteUserType(int id) {
      var userType = await _context.UserTypes.FirstAsync(a => a.Id == id);
      _repo.Delete(userType);

      if(await _repo.SaveAll())
        return NoContent();

      return BadRequest("impossible de supprimer cet élement ");
    }

    [HttpPost("{userId}/AddPhoto")]
    public async Task<IActionResult> AddPhoto(int userId, [FromForm] IFormFile photoFile) {
      var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(a => a.Id == userId);
      var uploadResult = new ImageUploadResult();

      if(photoFile.Length > 0) {
        using(var stream = photoFile.OpenReadStream()) {
          var uploadParams = new ImageUploadParams() {
          File = new FileDescription(photoFile.Name, stream),
          Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
          };

          uploadResult = _cloudinary.Upload(uploadParams);
        }
      }

      Photo photo = new Photo();
      photo.Url = uploadResult.SecureUri.ToString();
      photo.PublicId = uploadResult.PublicId;
      photo.UserId = userId;
      photo.DateAdded = DateTime.Now;
      if(!user.Photos.Any(u => u.IsMain)) {
        photo.IsMain = true;
        photo.IsApproved = true;
      }
      user.Photos.Add(photo);

      if(await _repo.SaveAll())
        return Ok();

      return BadRequest("Could not add the photo");
    }

    [HttpPost("EditUserAccount")]
    public async Task<IActionResult> EditUserAccount([FromForm] UserAccountForEditDto user)
    {
      bool userOK = await _repo.EditUserAccount(user);
      return Ok(userOK);
    }

    [HttpPost("AddEmployee")]
    public async Task<IActionResult> AddEmployee([FromForm] EmployeeForEditDto user)
    {
      bool userOK = await _repo.AddEmployee(user);

      if(userOK)
      {
        return Ok();
      } else
        return BadRequest("problème pour ajouter l'employé");
    }

    [HttpPost("AddTeacher")]
    public async Task<IActionResult> AddTeacher([FromForm] TeacherForEditDto user)
    {
      bool userOK = await _repo.AddTeacher(user);

      if(userOK) {
        return Ok();
      } else
        return BadRequest("problème pour ajouter l'enseigant");
    }

    [HttpGet("GetAdminUserTypes")]
    public async Task<IActionResult> GetAdminUserTypes() {
      return Ok(await _context.UserTypes.Where(a => a.Id >= adminTypeId).OrderBy(a => a.Name).ToListAsync());
    }

    [HttpGet("GetUserByTypeId/{id}")]
    public async Task<IActionResult> GetUserByTypeId(int id) {
      var users = await _context.Users.Include(p => p.Photos).Where(a => a.UserTypeId == id)
        .OrderBy(a => a.LastName).ThenBy(a => a.FirstName).ToListAsync();
      var userToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
      return Ok(userToReturn);
    }

    [HttpPost("{id}/updatePerson")]
    public async Task<IActionResult> updatePerson(int id, UserForRegisterDto model) {
      var user = await _repo.GetUser(id, false);
      if(user.Id != id)
        return BadRequest("impossible d'effectuer la mise a jour");
      var userToUpdate = _mapper.Map(model, user);
      if(userToUpdate.ClassId < 1)
        userToUpdate.ClassId = null;
      _repo.Update(userToUpdate);
      if(await _repo.SaveAll())
        return NoContent();
      return BadRequest("impossible d'effectuer la mise a jour");
    }

    [HttpPost("SearchUsers")]
    public async Task<IActionResult> SearchUsers(UserSearchDto model) {
      if(model.LastName == null)
        model.LastName = "";
      if(model.FirstName == null)
        model.FirstName = "";
      var query = from s in _context.Users.Include(a => a.UserType).Include(a => a.Photos)
      where(EF.Functions.Like(s.LastName, "%" + model.LastName + "%") && EF.Functions.Like(s.LastName, "%" + model.FirstName + "%"))
      select s;
      return Ok(_mapper.Map<IEnumerable<UserForListDto>>(await query.ToListAsync()));

    }

    [HttpGet("GetAllCities")]
    public async Task<IActionResult> GetAllCities() {
      return Ok(await _repo.GetAllCities());
    }

    [HttpGet("{id}/GetDistrictsByCityId")]
    public async Task<IActionResult> GetAllGetDistrictsByCityIdCities(int id) {
      return Ok(await _repo.GetAllGetDistrictsByCityIdCities(id));
    }

    [HttpGet("{userId}/events")]
    public async Task<IActionResult> GetUserEvents(int userId) {
      var events = await _repo.GetUserEvents(userId);
      return Ok(events);
    }

    [HttpPost("ValidateRegistration")]
    public async Task<IActionResult> ValidateRegistration(OrderToValidateDto orderToValidate) {
      var orderId = orderToValidate.OrderId;
      var linesToValidate = orderToValidate.OrderlineIds;
      var order = await _context.Orders.FirstAsync(o => o.Id == orderId);
      order.Lines = await _context.OrderLines
        .Include(i => i.Child).ThenInclude(i => i.Class)
        .Include(i => i.Product)
        .Where(o => o.OrderId == orderId).ToListAsync();

      order.TotalHT = 0;
      order.Discount = 0;
      order.AmountHT = 0;
      order.TVAAmount = 0;
      order.AmountTTC = 0;
      OrderDto updatedOrder = new OrderDto();
      List<OrderLineDto> updlines = new List<OrderLineDto>();
      foreach(var ltv in linesToValidate) {
        var line = order.Lines.First(o => o.Id == ltv.OrderlineId);
        line.Cancelled = ltv.Cancelled;
        _repo.Update(line);

        if(line.Cancelled == false) {
          var lineDto = _mapper.Map<OrderLineDto>(line);
          lineDto.Payments = new List<OrderLineDeadlineDto>();
          var payments = await _context.OrderLineDeadlines.Where(d => d.OrderLineId == line.Id).ToListAsync();
          lineDto.Payments = _mapper.Map<List<OrderLineDeadlineDto>>(payments);
          updlines.Add(lineDto);

          order.TotalHT += line.TotalHT;
          order.Discount += line.Discount;
          order.AmountHT += line.AmountHT;
          order.TVAAmount += line.TVAAmount;
          order.AmountTTC += line.AmountTTC;
        }
      }
      order.Validated = true;
      // order.Created = false;
      _repo.Update(order);
      updatedOrder = _mapper.Map<OrderDto>(order);

      updatedOrder.Lines = updlines;

      //add data for order history
      OrderHistory orderHistory = new OrderHistory();
      orderHistory.OrderId = order.Id;
      orderHistory.OpDate = order.OrderDate;
      orderHistory.Action = "UPD";
      orderHistory.OldAmount = 0;
      orderHistory.NewAmount = order.AmountTTC;
      orderHistory.Delta = orderHistory.NewAmount - orderHistory.OldAmount;
      _repo.Add(orderHistory);

      // var orderlines = updatedOrder.Lines.Where(o => o.Cancelled == false).ToList();
      // updatedOrder.Lines.Clear();
      // foreach(var line in orderlines)
      // {
      //   OrderLineDto old = _mapper.Map<OrderLineDto>(line);
      //   updatedOrder.Lines.Add(old);
      // }

      if(await _repo.SaveAll())
        return Ok(updatedOrder);

      return BadRequest("problème pour valider l'inscription");
    }

    [HttpGet("LoadUsers")]
    public async Task<IActionResult> loadUsersData() {
      var users = await _context.Users
        .Include(i => i.Photos)
        .Include(i => i.Class)
        .Include(i => i.UserType)
        .Include(i => i.ClassLevel)
        .Where(u => u.AccountDataValidated &&(u.UserTypeId == studentTypeId || u.UserTypeId == parentTypeId ||
          u.UserTypeId == teacherTypeId))
        .ToListAsync();

      List<SearchUsersDataDto> data = new List<SearchUsersDataDto>();
      foreach(var user in users)
      {
        string fname = user.FirstName == null ? "" : user.FirstName.ToLower().FirstLetterToUpper();
        string lname = user.LastName == null ? "" : user.LastName.ToLower().FirstLetterToUpper();
        string idNum = user.IdNum == null ? "" : user.IdNum;
        int age = user.DateOfBirth == null ? 0 : user.DateOfBirth.CalculateAge();
        string levelname = user.ClassLevel == null? "": user.ClassLevel.Name;
        string className = "";
        if(user.Class != null)
          className = user.Class.Name;
        string photoUrl = "";
        if(user.Photos.Count() > 0)
          photoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url;
        SearchUsersDataDto sudd = new SearchUsersDataDto();
        sudd.UserId = user.Id;
        sudd.UserTypeId = user.UserTypeId;
        sudd.UserType = user.UserType.Name;
        sudd.FirstName = fname;
        sudd.LastName = lname;
        sudd.IDNum = idNum;
        sudd.Age = age;
        sudd.ClassLevelName = levelname;
        sudd.ClassName = className;
        sudd.PhotoUrl = photoUrl;
        data.Add(sudd);
      }

      return Ok(data);
    }

    [HttpPost("LoadUserFile")]
    public async Task<IActionResult> LoadUserFile(UserFileDataDto userFileDataDto)
    {
      var childId = userFileDataDto.UserId;
      var searchData = userFileDataDto.SearchData;

      var childFromRepo = await _repo.GetUser(childId, false);
      var child = _mapper.Map<UserForDetailedDto>(childFromRepo);

      ChildForFileDto userFile = new ChildForFileDto();
      userFile.Id = childId;
      userFile.LastName = child.LastName;
      userFile.FirstName = child.FirstName;
      userFile.Email = child.Email;
      userFile.ClassLevelId = child.ClassLevelId;
      userFile.ClassLevelName = child.ClassLevelName;
      userFile.ClassId = child.ClassId;
      userFile.ClassName = child.ClassName;
      userFile.Gender = child.Gender;
      userFile.strDateOfBirth = child.strDateOfBirth;
      userFile.Age = child.Age;
      userFile.PhotoUrl = child.PhotoUrl;
      userFile.PhoneNumber = child.PhoneNumber;

      var parentsFromRepo = await _repo.GetParents(childId);
      var parents = _mapper.Map<IEnumerable<UserForDetailedDto>>(parentsFromRepo);
      foreach(var parent in parents)
      {
        if(parent.UserTypeId == parentTypeId && parent.Gender == 0) {
          userFile.MotherId = parent.Id;
          userFile.MotherLastName = parent.LastName;
          userFile.MotherFirstName = parent.FirstName;
          userFile.MotherPhoneNumber = parent.PhoneNumber;
          userFile.Mother2ndPhoneNumber = parent.SecondPhoneNumber;
          userFile.MotherPhotoUrl = parent.PhotoUrl;
        }

        if(parent.UserTypeId == parentTypeId && parent.Gender == 1) {
          userFile.FatherId = parent.Id;
          userFile.FatherLastName = parent.LastName;
          userFile.FatherFirstName = parent.FirstName;
          userFile.FatherPhoneNumber = parent.PhoneNumber;
          userFile.Father2ndPhoneNumber = parent.SecondPhoneNumber;
          userFile.FatherPhotoUrl = parent.PhotoUrl;
        }
      }

      return Ok(userFile);
    }

    [HttpGet("UserFiles/{data}")]
    public async Task<IActionResult> GetUserFiles(string data) {
      var words = data.Split(" ");
      string lastNameWhere = "";
      string firstNameWhere = "";
      string idNumWhere = "";

      foreach(var word in words)
      {
        if(lastNameWhere == "")
        {
          lastNameWhere = " LastName like '%" + word + "%' ";
          firstNameWhere = " FirstName like '%" + word + "%' ";
          idNumWhere = " IdNum like '%" + word + "%' ";
        }
        else
        {
          lastNameWhere += " OR LastName like '%" + word + "%' ";
          firstNameWhere += " OR FirstName like '%" + word + "%' ";
          idNumWhere += " OR IdNum like '%" + word + "%' ";
        }
      }

      List<User> children = new List<User>();
      string query = "select * from AspNetUsers where UserTypeId=" + studentTypeId + " AND(";
      query += lastNameWhere + " OR " + firstNameWhere + " OR " + idNumWhere + ")";
      var usersFromRepo = await _context.Users
        .Include(i => i.Photos)
        .Include(i => i.Class)
        .FromSql(query)
        .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
        .ToListAsync();
      var users = _mapper.Map<IEnumerable<UserForDetailedDto>>(usersFromRepo);
      return Ok(users);
    }

    [HttpGet("UsersToValidate")]
    public async Task<IActionResult> GetUsersToValidate()
    {
      List<User> usersCached = await _cache.GetUsers();
      List<Order> orders = await _cache.GetOrders();
      List<UserType> userTypesCached = await _cache.GetUserTypes();

      // users to validate exclude chidren/students as they are coped by parent on their account
      // var usersFromDB = usersCached.Where(u => u.AccountDataValidated == false && u.UserTypeId != studentTypeId).ToList();
      var activeTuitions = orders.Where(t => t.isReg == true && t.Expired == false || t.Cancelled == false).ToList();

      var userTypes = userTypesCached.Where(u => u.Id != studentTypeId)
                                     .OrderBy(o => o.Name)
                                     .ToList();

      List<UserTypeToValidateDto> types = new List<UserTypeToValidateDto>();
      foreach(var type in userTypes)
      {
        if(type.Name.ToLower() == "admin") { continue; }

        UserTypeToValidateDto userType = new UserTypeToValidateDto();
        userType.Id = type.Id;
        userType.Name = type.Name;
        userType.Users = new List<UserToValidateDto>();
        var users = usersCached.OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                               .Where(u => u.AccountDataValidated == false && u.UserTypeId == type.Id).ToList();
        foreach(var user in users)
        {
          UserToValidateDto userToValidate = new UserToValidateDto();
          userToValidate.Id = user.Id;
          userToValidate.LastName = user.LastName;
          userToValidate.FirstName = user.FirstName;
          userToValidate.ClassName = user.Class != null ? user.Class.Name : "";
          userToValidate.ClassLevelName = user.ClassLevel != null ? user.ClassLevel.Name : "";
          userToValidate.UserType = user.UserType.Name;
          userToValidate.Email = user.Email;
          userToValidate.Cell = user.PhoneNumber;
          // if(user.UserTypeId == studentTypeId)
          // {
          //   var parents = await _repo.GetParents(user.Id);
          //   var mother = parents.FirstOrDefault(m => m.Gender == 0);
          //   if(mother != null)
          //   {
          //     userToValidate.MotherId = mother.Id;
          //     userToValidate.MotherLastName = mother.LastName;
          //     userToValidate.MotherFirstName = mother.FirstName;
          //     userToValidate.MotherEmail = mother.Email;
          //     userToValidate.MotherCell = mother.PhoneNumber.FormatPhoneNumber();
          //   }
          //   var father = parents.FirstOrDefault(m => m.Gender == 1);
          //   if(father != null)
          //   {
          //     userToValidate.FatherId = father.Id;
          //     userToValidate.FatherLastName = father.LastName;
          //     userToValidate.FatherFirstName = father.FirstName;
          //     userToValidate.FatherEmail = father.Email;
          //     userToValidate.FatherCell = father.PhoneNumber.FormatPhoneNumber();
          //   }
          // }

          if(user.UserTypeId == parentTypeId)
          {
            var childrenFromDB = await _repo.GetChildren(user.Id);
            var children = _mapper.Map<List<ChildParentDto>>(childrenFromDB);
            userToValidate.Children = new List<ChildParentDto>();
            userToValidate.Children = children;
          }

          Order userTuition = new Order();
          if(type.Id == parentTypeId)
          {
            userTuition = activeTuitions.FirstOrDefault(t => t.MotherId == user.Id || t.FatherId == user.Id);
          }
          else if(type.Id == studentTypeId)
          {
            var parents = await _repo.GetParents(user.Id);
            userTuition = activeTuitions.FirstOrDefault(t => t.MotherId == parents[0].Id || t.FatherId == parents[0].Id);
          }

          if(userTuition != null)
          {
            userToValidate.Tuition = _mapper.Map<OrderUserToValidateDto>(userTuition);
            var lines = await _repo.GetOrderLines(userToValidate.Tuition.Id);
            userToValidate.Tuition.Lines = _mapper.Map<List<LineUserToValidateDto>>(lines);
          }
          userType.Users.Add(userToValidate);
        }

        types.Add(userType);
      }

      return Ok(types);
    }

    [HttpPost("EditUserToValidate")]
    public async Task<IActionResult> EditUserToValidate(EditUserToValidateDto userData)
    {
      List<User> users = await _cache.GetUsers();
      var user = users.First(u => u.Id == userData.Id);
      user.Email = userData.Email;
      user.PhoneNumber = userData.Cell;
      _context.Update(user);
      if(await _repo.SaveAll())
      {
        await _cache.LoadUsers();
        return Ok();
      }
      else
        return BadRequest("problème pour mettre à jour les données");
    }

    [HttpPost("ResendConfirmEmail")]
    public async Task<IActionResult> ResendConfirmEmail(List<SendConfirmEmailDto> usersData)
    {
      List<Setting> settings = await _cache.GetSettings();
      List<Order> orders = await _cache.GetOrders();
      List<ProductDeadLine> productDeadLines = await _cache.GetProductDeadLines();
      List<EmailTemplate> emailTemplates= await _context.EmailTemplates.ToListAsync();
      List<User> users = await _cache.GetUsers();

      List<RegistrationEmailDto> emails = new List<RegistrationEmailDto>();

      var schoolName = settings.First(s => s.Name == "SchoolName").Value;
      string RegDeadLine = settings.First(s => s.Name == "RegistrationDeadLine").Value;

      foreach(var data in usersData)
      {
        var userType = data.UserTypeId;
        if(userType == parentTypeId)
        {
          foreach(var userid in data.UserIds)
          {
            var order = orders.First(o => o.isReg &&(o.MotherId == userid || o.FatherId == userid));
            var reg = await _repo.GetOrder(order.Id);
            User user = users.First(u => u.Id == userid);
            // if(reg.Mother != null)
            //   user = reg.Mother;
            // else
            //   user = reg.Father;

            var firstDeadline = productDeadLines.OrderBy(o => o.DueDate)
                                                .First(p => p.ProductId == tuitionId);
            var firstDeadlineDate = firstDeadline.DueDate;
            decimal DPPct = firstDeadline.Percentage;

            decimal orderDueAmount = 0;
            List<ChildRegistrationDto> children = new List<ChildRegistrationDto>();
            foreach(var line in reg.Lines)
            {
              ChildRegistrationDto crd = new ChildRegistrationDto();
              crd.LastName = line.Child.LastName;
              crd.FirstName = line.Child.FirstName;
              crd.NextClass = line.ClassLevel.Name;
              crd.RegistrationFee = line.ProductFee.ToString("N0");
              crd.TuitionAmount = line.UnitPrice.ToString("N0");
              crd.DueAmountPct =(DPPct * 100).ToString("N0") + "%";
              var downPayment = DPPct * line.UnitPrice;
              crd.DueAmount = downPayment.ToString("N0");
              orderDueAmount += downPayment + line.ProductFee;
              crd.TotalDueForChild =(line.ProductFee + downPayment).ToString("N0");
              children.Add(crd);
            }

            RegistrationEmailDto email = new RegistrationEmailDto();
            email.ParentId = userid;
            email.ParentLastName = user.LastName;
            email.ParentFirstName = user.FirstName;
            email.ParentEmail = user.Email;
            email.ParentCellPhone = user.PhoneNumber;
            email.ParentGender = user.Gender;
            email.EmailSubject = schoolName + " - inscription pour l'année scolaire prochaine";
            email.OrderId = order.Id;
            email.OrderNum = order.OrderNum;
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            email.Token = token;
            email.DueDate = firstDeadlineDate.ToString("dd/MM/yyyy", frC);
            email.TotalAmount = orderDueAmount.ToString("N0");
            email.Children = children;
            emails.Add(email);
          }

          var template = emailTemplates.First(t => t.Id == newRegToBePaidEmailId);
          var RegEmails = await _repo.SetEmailDataForRegistration(emails, template.Body, RegDeadLine);
          _context.AddRange(RegEmails);
        }

        if(userType == teacherTypeId)
        {
          // send the mail to update userName/pwd - add to Email table
          foreach(var userid in data.UserIds)
          {
            var appUser = users.FirstOrDefault(u => u.Id == userid);
            var teacherCode = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            ConfirmTeacherEmailDto emailData = new ConfirmTeacherEmailDto() {
              Id = appUser.Id,
              LastName = appUser.LastName,
              FirstName = appUser.FirstName,
              Cell = appUser.PhoneNumber,
              Gender = appUser.Gender,
              Email = appUser.Email,
              Token = teacherCode
            };

            var template = emailTemplates.First(t => t.Id == teacherConfirmEmailId);
            Email emailToSend = await _repo.SetDataForConfirmTeacherEmail(emailData, template.Body, template.Subject);
            _repo.Add(emailToSend);
          }
        }
      }

      if(!await _repo.SaveAll())
        return BadRequest("problème pour envoyer les emails de confirmaiton");

      return Ok();
    }

  }
}