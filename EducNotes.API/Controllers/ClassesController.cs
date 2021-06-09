using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.data;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Helpers;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EducNotes.API.Controllers {
  [Route ("api/[controller]")]
  [ApiController]
  public class ClassesController : ControllerBase {
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IEducNotesRepository _repo;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    private readonly IConfiguration _config;
    CultureInfo frC = new CultureInfo ("fr-FR");
    int parentRoleId, memberRoleId;
    int absenceTypeId, lateTypeId;
    private readonly ICacheRepository _cache;

    public ClassesController(DataContext context, IMapper mapper, IEducNotesRepository repo,
      IConfiguration config, ICacheRepository cache)
    {
      _cache = cache;
      _config = config;
      _context = context;
      _mapper = mapper;
      _repo = repo;
      teacherTypeId = _config.GetValue<int>("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int>("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int>("AppSettings:studentTypeId");
      parentRoleId = _config.GetValue<int>("AppSettings:parentRoleId");
      memberRoleId = _config.GetValue<int>("AppSettings:memberRoleId");
      absenceTypeId = _config.GetValue<int>("AppSettings:AbsenceTypeId");
      lateTypeId = _config.GetValue<int>("AppSettings:LateTypeId");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClass(int Id)
    {
      List<Class> classes = await _cache.GetClasses();
      var theclass = classes.FirstOrDefault(c => c.Id == Id);
      return Ok(theclass);
    }

    [HttpGet ("{classId}/schedule/today")]
    public async Task<IActionResult> GetScheduleToday(int classId)
    {
      // monday=1, tue=2, ...
      var today = ((int)DateTime.Now.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;

      //if saturday or sunday goes to monday schedule
      if(today == 6 || today == 7)
        today = 1;

      var coursesFromRepo = await _repo.GetScheduleDay(classId, today);
      var courses = _mapper.Map<IEnumerable<ScheduleForTimeTableDto>>(coursesFromRepo);
      return Ok(courses);
    }

    [HttpGet ("{classId}/schedule/{day}")]
    public async Task<IActionResult> GetScheduleDay(int classId, int day)
    {
      var theclass = await GetClass(classId);

      if (theclass == null)
        return BadRequest ("la classe n'existe pas");

      if (day < 1 || day > 7)
        return BadRequest("le jour de la semaine est incorrect.");

      var courses = await _context.Schedules
                          .Include(i => i.Class).ThenInclude(i => i.ClassLevel)
                          .Where(d => d.Day == day && d.ClassId == classId)
                          .OrderBy(s => s.StartHourMin).ToListAsync();

      if(courses.Count == 0)
        return Unauthorized();

      var coursesToReturn = _mapper.Map<IEnumerable<ScheduleForTimeTableDto>>(courses);
      return Ok (coursesToReturn);
    }

    [HttpGet("{classId}/ScheduleByDay")]
    public async Task<IActionResult> GetClassScheduleNDays(int classId)
    {
      int daysRange = 14;
      DateTime today = DateTime.Now.Date;
      var startDate = today.AddDays(-daysRange);
      var endDate = today.AddDays(daysRange);
      var itemsFromRepo = await _repo.GetClassSchedule(classId);

      List<ClassScheduleNDaysDto> items = new List<ClassScheduleNDaysDto>();
      for(int i = 0; i < 2 * daysRange; i++)
      {
        ClassScheduleNDaysDto item = new ClassScheduleNDaysDto();
        var currentDate = startDate.AddDays(i);
        var dayInt = (int)currentDate.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
        item.Day = dayInt;
        item.DayDate = currentDate;
        item.strDayDate = currentDate.ToString("ddd dd MMM", frC);
        item.Courses = new List<ClassDayCoursesDto>();
        var daySchedules = itemsFromRepo.Where(s => s.Day == dayInt);
        // var courses = new List<ClassDayCoursesDto>();
        // foreach(var daySchedule in daySchedules)
        // {
        //   ClassDayCoursesDto dayCourse = new ClassDayCoursesDto();
        //   foreach (var course in daySchedule.Courses)
        //   {
        //     if(course.CourseId > 0)
        //     {
        //       dayCourse.CourseId = course.CourseId;
        //       dayCourse.CourseName = course.CourseName;
        //       dayCourse.CourseAbbrev = course.CourseAbbrev;
        //     }
        //     else
        //     {
        //       dayCourse.ActivityId = course.ActivityId;
        //       dayCourse.ActivityName = course.ActivityName;
        //       dayCourse.ActivityAbbrev = course.ActivityAbbrev;
        //     }
        //     dayCourse.TeacherId = course.TeacherId;
        //     dayCourse.TeacherName = course.TeacherName;
        //     courses.Add(dayCourse);
        //   }
        // }
        item.Courses = _repo.GetCoursesFromSchedules(daySchedules);
        items.Add(item);
      }

      return Ok(items);
    }

    [HttpGet("{classId}/ScheduleCoursesByDay")]
    public async Task<IActionResult> GetClassScheduleByDay(int classId)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();

      var scheduleItems = schedules.Where(s => s.ClassId == classId).ToList();

      ScheduleClassDto classSchedule = new ScheduleClassDto();
      if(scheduleItems.Count() > 0)
      {
        var days = scheduleItems.Select(d => d.Day).Distinct();
        classSchedule.ClassId = classId;
        classSchedule.ClassName = scheduleItems[0].Class.Name;

        classSchedule.Days = new List<ScheduleDayDto>();
        foreach(var day in days)
        {
          ScheduleDayDto scheduleDay = new ScheduleDayDto();
          scheduleDay.Day = day;
          scheduleDay.DayName = day.DayIntToName();

          var scheduleDayItems = scheduleItems.Where(s => s.Day == day).OrderBy(o => o.StartHourMin.ToString("HH:mm", frC));
          scheduleDay.Courses = new List<ScheduleCourseDto>();
          foreach(var item in scheduleDayItems)
          {
            List<ScheduleCourse> courses = scheduleCourses.Where(c => c.ScheduleId == item.Id)
                                                          .OrderBy(o => o.Schedule.StartHourMin).ToList();
            foreach(var course in courses)
            {
              ScheduleCourseDto courseDto = new ScheduleCourseDto();
              courseDto.Id = course.Id;
              courseDto.ScheduleId = course.ScheduleId;
              courseDto.CourseId = course.Course.Id;
              courseDto.CourseName = course.Course.Name;
              courseDto.CourseAbbrev = course.Course.Abbreviation;
              courseDto.CourseColor = course.Course.Color;
              courseDto.ClassId = item.ClassId;
              courseDto.ClassName = item.Class.Name;
              courseDto.TeacherId = course.TeacherId;
              courseDto.TeacherName = course.Teacher.LastName + " " + course.Teacher.FirstName;
              courseDto.IsDarkColor = course.Course.Color.IsDarkColor();
              courseDto.StartHour = item.StartHourMin;
              courseDto.StartH = item.StartHourMin.ToString("HH:mm", frC);
              courseDto.EndHour = item.EndHourMin;
              courseDto.EndH = item.EndHourMin.ToString("HH:mm", frC);
              courseDto.InConflict = false;
              scheduleDay.Courses.Add(courseDto);
            }
          }

          classSchedule.Days.Add(scheduleDay);
        }
      }

      return Ok(classSchedule);
    }

    [HttpGet("{classId}/TimeTable")]
    public async Task<IActionResult> GetClassTimeTable(int classId)
    {
      List<Setting> settings = await _cache.GetSettings();
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();

      var startCourseHourMin = settings.FirstOrDefault(s => s.Name.ToLower() == "starthourmin").Value;
      var endCourseHourMin = settings.FirstOrDefault(s => s.Name.ToLower() == "endhourmin").Value;

      List<int> schoolHours = new List<int>();
      if(startCourseHourMin != null)
      {
        var startHM = startCourseHourMin.Split(":");
        schoolHours.Add(Convert.ToInt32(startHM[0]));
        schoolHours.Add(Convert.ToInt32(startHM[1]));
      }
      if(endCourseHourMin != null)
      {
        var endHM = endCourseHourMin.Split(":");
        schoolHours.Add(Convert.ToInt32(endHM[0]));
        schoolHours.Add(Convert.ToInt32(endHM[1]));
      }

      var scheduleHourSize = _config.GetValue<double>("AppSettings:DimHourSchedule");
      var startToEndHeigth = ((schoolHours[2] * 60 + schoolHours[3]) - (schoolHours[0] * 60 + schoolHours[1])) * scheduleHourSize / 60;
      var height = Convert.ToDouble(Math.Round(startToEndHeigth, 2));
      var scheduleHeight = (height + "px").Replace(",", ".");

      DateTime today = DateTime.Now.Date;
      var dayInt = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
      var monday = today.AddDays(1 - dayInt);
      var sunday = monday.AddDays(6);

      var scheduleItems = await _repo.GetClassSchedule(classId);
      // setup course box top in schedule
      for(int i = 0; i < scheduleItems.Count(); i++)
      {
        ScheduleForTimeTableDto scheduleItem = scheduleItems[i];
        scheduleItem.Top = _repo.CalculateCourseTop(scheduleItem.StartHourMin, startCourseHourMin);
        scheduleItem.Height = scheduleItem.StartHourMin.CalculateHeight(scheduleItem.EndHourMin);
      }

      var days = new List<string>();
      for(int i = 0; i <= 6; i++)
      {
        DateTime dt = monday.AddDays(i);
        var shortdate = dt.ToString("ddd dd MMM", frC);
        days.Add(shortdate);
      }

      if(scheduleItems != null)
      {
        return Ok(new {
          scheduleItems,
          firstDayWeek = monday,
          strMonday = monday.ToString ("dddd dd MMM yyy", frC),
          strSunday = sunday.ToString ("dddd dd MMM yyy", frC),
          strShortMonday = monday.ToString ("ddd dd MMM", frC),
          strShortSunday = sunday.ToString ("ddd dd MMM", frC),
          weekDays = days,
          schoolHours,
          scheduleHeight
        });
      }

      return BadRequest("aucun emploi du temps trouvé");
    }

    [HttpPut("DelCourseFromSchedule/{scheduleCourseId}")]
    public async Task<IActionResult> DeleteScheduleItem(int scheduleCourseId)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();
      bool deleteOK = false;

      ScheduleCourse course = scheduleCourses.FirstOrDefault(s => s.Id == scheduleCourseId);
      if(course != null)
      {
        using(var identityContextTransaction = _context.Database.BeginTransaction())
        {
          try
          {
            // is there another course with the current schedule?
            _repo.Delete(course);
            Boolean scheduleBusy = (scheduleCourses.Where(s => s.ScheduleId == course.ScheduleId).Count()) > 1;
            if(!scheduleBusy)
            {
              _repo.Delete(course.Schedule);
            }

            if(await _repo.SaveAll())
            {
              identityContextTransaction.Commit();
              await _cache.LoadSchedules();
              await _cache.LoadScheduleCourses();
              deleteOK = true;
              return Ok(deleteOK);
            }
          }
          catch
          {
            identityContextTransaction.Rollback();
            return Ok(deleteOK);
          }
        }
      }

      return BadRequest ("problème pour supprimer le cours de l'emploi du temps");
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
          var students = studentsCached.Where(s => s.ClassId == aclass.Id).ToList();
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

    [HttpGet ("{classId}/getClassScheduleMovedWeek")]
    public async Task<IActionResult> getClassScheduleMovedWeek (int classId, [FromQuery] ScheduleParams agendaParams) {
      var FromDate = agendaParams.DueDate.Date;
      var move = agendaParams.MoveWeek;
      var date = FromDate.AddDays (move);
      var dateDay = (int) date.DayOfWeek;

      var dayInt = dateDay == 0 ? 7 : dateDay;
      DateTime monday = date.AddDays (1 - dayInt);
      var sunday = monday.AddDays (6);

      var itemsFromRepo = await _repo.GetClassSchedule (classId);

      var days = new List<string> ();
      for (int i = 0; i <= 6; i++) {
        DateTime dt = monday.AddDays (i);
        CultureInfo frC = new CultureInfo ("fr-FR");
        var shortdate = dt.ToString ("ddd dd MMM", frC);
        days.Add (shortdate);
      }

      if (itemsFromRepo != null) {
        return Ok (new {
          agendaItems = itemsFromRepo,
            firstDayWeek = monday,
            strMonday = monday.ToLongDateString (),
            strSunday = sunday.ToLongDateString (),
            weekDays = days
        });
      }

      return BadRequest ("Aucun emploi du temps trouvé");
    }

    [HttpGet ("{classId}/teachers")]
    public async Task<IActionResult> GetClassTeachers(int classId)
    {
      // List<ClassCourse> classCourses = await _cache.GetClassCourses();
      var teachers = await _context.ClassCourses.Where(t => t.ClassId == classId && t.Teacher != null)
                                                .Select(t => t.Teacher).Distinct().ToListAsync();

      var teachersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(teachers);

      return Ok(teachersToReturn);
    }

    [HttpGet ("{classId}/CourseWithTeacher")]
    public async Task<IActionResult> GetCourseWithTeacher (int classId)
    {
      var teachersData = await _context.ClassCourses.Where(u => u.ClassId == classId)
                                                         .Distinct().ToListAsync();

      List<TeacherForListDto> coursesWithTeacher = new List<TeacherForListDto>();
      foreach(var data in teachersData)
      {
        TeacherForListDto teacherCourse = new TeacherForListDto ();
        teacherCourse.Id = Convert.ToInt32 (data.TeacherId);
        teacherCourse.LastName = data.Teacher.LastName;
        teacherCourse.FirstName = data.Teacher.FirstName;
        teacherCourse.Email = data.Teacher.Email;
        teacherCourse.PhotoUrl = data.Teacher.Photos.FirstOrDefault (p => p.IsMain).Url;
        teacherCourse.PhoneNumber = data.Teacher.PhoneNumber;
        teacherCourse.DateOfBirth = data.Teacher.DateOfBirth.ToString ("dd/MM/yyyy", frC);
        teacherCourse.SecondPhoneNumber = data.Teacher.SecondPhoneNumber;
        teacherCourse.Course = data.Course;
        teacherCourse.EducLevelId = data.Class.ClassLevel.EducationLevel.Id;
        teacherCourse.EducLevelName = data.Class.ClassLevel.EducationLevel.Name;
        teacherCourse.ClassName = data.Class.Name;
        coursesWithTeacher.Add (teacherCourse);
      }

      return Ok (coursesWithTeacher);
    }

    [HttpGet ("{classId}/Students")]
    public async Task<IActionResult> GetClassStudents (int classId) {
      var studentsFromRepo = await _repo.GetClassStudents (classId);
      var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>> (studentsFromRepo);

      return Ok (usersToReturn);
    }

    [HttpGet ("Agenda/{agendaId}/SetTask/{value}")]
    public async Task<IActionResult> AgendaSetTask (int agendaId, bool value) {
      Agenda agenda = await _context.Agendas.FirstOrDefaultAsync (a => a.Id == agendaId);

      if (agenda != null) {
        agenda.Done = value;
        //set the current user as the modifier of done status
        agenda.DoneSetById = int.Parse (User.FindFirst (ClaimTypes.NameIdentifier).Value);
        _repo.Update (agenda);
        if (await _repo.SaveAll ())
          return NoContent ();
        else
          return BadRequest ("problème de saisie de données");
      } else {
        return BadRequest ("problème de données. voir l'administration si le problème persiste");
      }
    }

    [HttpGet ("{classId}/MovedWeekAgenda")]
    public async Task<IActionResult> getClassMovedWeekAgenda (int classId, [FromQuery] AgendaParams agendaParams)
    {
      var FromDate = agendaParams.DueDate.Date;
      var move = agendaParams.MoveWeek;
      var date = FromDate.AddDays(move);
      var dateDay = (int) date.DayOfWeek;

      var dayInt = dateDay == 0 ? 7 : dateDay;
      DateTime monday = date.AddDays (1 - dayInt);
      var saturday = monday.AddDays (5);

      var itemsFromRepo = await _repo.GetClassAgenda(classId, monday, saturday);
      var items = _repo.GetAgendaListByDueDate (itemsFromRepo);

      var courses = await _context.ClassCourses
                            .Where(c => c.ClassId == classId)
                            .Select(s => s.Course).ToListAsync();

      List<CourseTasksDto> coursesWithTasks = new List<CourseTasksDto>();
      foreach (var course in courses)
      {
        CourseTasksDto ctd = new CourseTasksDto ();
        var nbItems = itemsFromRepo.Where (a => a.Session.CourseId == course.Id).ToList().Count();
        ctd.CourseId = course.Id;
        ctd.CourseName = course.Name;
        ctd.CourseAbbrev = course.Abbreviation;
        ctd.CourseColor = course.Color;
        ctd.NbTasks = nbItems;
        coursesWithTasks.Add (ctd);
      }

      var days = new List<string>();
      var nbTasks = new List<int>();
      for (int i = 0; i <= 5; i++)
      {
        DateTime dt = monday.AddDays (i);
        var shortdate = dt.ToString("ddd dd MMM", frC);
        days.Add (shortdate);

        var item = items.FirstOrDefault(a => a.dtDueDate == dt);
        if(item != null)
          nbTasks.Add(item.NbTasks);
        else
          nbTasks.Add(0);
      }

      if(itemsFromRepo != null)
      {
        return Ok(new {
          agendaItems = items,
            firstDayWeek = monday,
            strMonday = monday.ToLongDateString(),
            strSaturday = saturday.ToLongDateString(),
            weekDays = days,
            nbDayTasks = nbTasks,
            coursesWithTasks
        });
      }

      return BadRequest ("Aucun agenda trouvé");
    }

    [HttpGet ("{classId}/TodayToNDaysAgenda/{toNbDays}")]
    public async Task<IActionResult> getTodayToNDaysAgenda (int classId, int toNbDays)
    {
      var agendasFromRepo = await _repo.GetClassAgendaTodayToNDays(classId, toNbDays);
      var items = _repo.GetAgendaListByDueDate(agendasFromRepo);

      var days = new List<string>();
      var nbTasks = new List<int>();
      var today = DateTime.Now.Date;
      for(int i = 0; i < toNbDays; i++)
      {
        DateTime dt = today.AddDays(i);
        var shortdate = dt.ToString("ddd dd MMM", frC);
        days.Add(shortdate);

        var item = items.FirstOrDefault(a => a.dtDueDate == dt);
        if(item != null)
          nbTasks.Add(item.NbTasks);
        else
          nbTasks.Add (0);
      }

      var lastDay = today.AddDays (toNbDays);

      if (agendasFromRepo != null) {
        return Ok (new {
          agendaItems = items,
            firstDay = today,
            strFirstDay = today.ToLongDateString (),
            strLastDayay = lastDay.ToLongDateString (),
            weekDays = days,
            nbDayTasks = nbTasks
        });
      }

      return BadRequest ("Aucun agenda trouvé");
    }

    [HttpGet ("{classId}/CurrWeekAgenda")]
    public async Task<IActionResult> getClassCurrWeekAgenda (int classId) {
      DateTime today = new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
      var dayInt = (int) today.DayOfWeek == 0 ? 7 : (int) today.DayOfWeek;
      var monday = today.AddDays (1 - dayInt);
      var saturday = monday.AddDays (5);

      var itemsFromRepo = await _repo.GetClassAgenda(classId, monday, saturday);
      var items = _repo.GetAgendaListByDueDate(itemsFromRepo);

      var courses = await _context.ClassCourses
                        .Where(c => c.ClassId == classId)
                        .Select(s => s.Course).ToListAsync();

      List<CourseTasksDto> coursesWithTasks = new List<CourseTasksDto> ();
      foreach (var course in courses) {
        CourseTasksDto ctd = new CourseTasksDto ();
        var nbItems = itemsFromRepo.Where (a => a.Session.CourseId == course.Id).ToList ().Count ();
        ctd.CourseId = course.Id;
        ctd.CourseName = course.Name;
        ctd.CourseAbbrev = course.Abbreviation;
        ctd.CourseColor = course.Color;
        ctd.NbTasks = nbItems;
        coursesWithTasks.Add (ctd);
      }

      var days = new List<string> ();
      var nbTasks = new List<int> ();
      for (int i = 0; i <= 5; i++) {
        DateTime dt = monday.AddDays (i);
        var shortdate = dt.ToString ("ddd dd MMM", frC);
        days.Add (shortdate);

        var item = items.FirstOrDefault (a => a.dtDueDate == dt);
        if (item != null)
          nbTasks.Add (item.NbTasks);
        else
          nbTasks.Add (0);
      }

      if (itemsFromRepo != null) {
        return Ok (new {
          agendaItems = items,
            firstDayWeek = monday,
            strMonday = monday.ToLongDateString (),
            strSaturday = saturday.ToLongDateString (),
            weekDays = days,
            nbDayTasks = nbTasks,
            coursesWithTasks
        });
      }

      return BadRequest ("Aucun agenda trouvé");
    }

    // [HttpPost("HtmlToPDF")]
    // public IActionResult ConvertHtmlToPDF([FromBody] PdfDataDto pdfData)
    // {
    //     var html = pdfData.Html;
    //     // Render any HTML fragment or document to HTML
    //     var Renderer = new IronPdf.HtmlToPdf();
    //     var PDF = Renderer.RenderHtmlAsPdf(html);
    //     var OutputPath = "HtmlToPDF.pdf";
    //     PDF.SaveAs(OutputPath);
    //     // This neat trick opens our PDF file so we can see the result in our default PDF viewer
    //     //System.Diagnostics.Process.Start(OutputPath);

    //     // Create a PDF from any existing web page
    //     var Renderer1 = new IronPdf.HtmlToPdf();
    //     var PDF1 = Renderer1.RenderUrlAsPdf("https://ironpdf.com/tutorials/html-to-pdf/#exporting-a-pdf-using-existing-html-url");
    //     PDF1.SaveAs("wikipedia.pdf");
    //     // This neat trick opens our PDF file so we can see the result
    //     //System.Diagnostics.Process.Start("wikipedia.pdf");

    //     return NoContent();
    // }

    [HttpGet ("GetWeekDays")]
    public IActionResult GetWeekDaysByDate ([FromQuery] AgendaParams agendaParams) {
      var date = agendaParams.DueDate;

      var weekDays = _repo.GetWeekDays (date);

      return Ok (weekDays);
    }

    [HttpGet ("GetSanctions")]
    public async Task<IActionResult> GetSanctions () {
      var sanctions = await _context.Sanctions.OrderBy (o => o.Name).ToListAsync ();
      return Ok (sanctions);
    }

    [HttpGet ("{classId}/events")]
    public async Task<IActionResult> GetEvents (int classId) {
      var absences = await _context.Absences
        .Include (i => i.User).ThenInclude (p => p.Photos)
        .Include (i => i.AbsenceType)
        .Include (i => i.DoneBy)
        .Where (a => a.User.ClassId == classId)
        .OrderByDescending (o => o.StartDate).ToListAsync ();

      List<UserClassEventForListDto> events = new List<UserClassEventForListDto> ();

      foreach (var abs in absences) {
        //set the date data to be shown in reporting
        string dateData = "";
        if (abs.StartDate.Date == abs.EndDate.Date) {
          dateData = abs.StartDate.ToString ("dd/MM/yy", frC) + " de " +
            abs.StartDate.ToString ("HH:mm", frC) + " à " + abs.EndDate.ToString ("HH:mm", frC);
        } else {
          dateData = "de " + abs.StartDate.ToString ("dd/MM/yy", frC) + " " + abs.StartDate.ToString ("HH:mm", frC) +
            " au " + abs.EndDate.ToString ("dd/MM/yy", frC) + " " + abs.EndDate.ToString ("HH:mm", frC);
        }

        UserClassEventForListDto userEvent = new UserClassEventForListDto ();
        userEvent.Id = abs.Id;
        userEvent.UserId = abs.UserId;
        userEvent.UserName = abs.User.LastName + " " + abs.User.FirstName;
        if (abs.User.Photos.Count () > 0)
          userEvent.PhotoUrl = abs.User.Photos.FirstOrDefault (p => p.IsMain).Url;
        userEvent.ClassEventName = "absence";
        userEvent.ClassEventTypeId = 100 + abs.AbsenceTypeId;
        userEvent.ClassEventType = abs.AbsenceType.Name;
        userEvent.DoneByName = abs.DoneBy.LastName + " " + abs.DoneBy.FirstName;
        userEvent.StartDate = abs.StartDate;
        userEvent.strStartDate = dateData;
        userEvent.EndDate = abs.EndDate;
        userEvent.strEndDate = abs.EndDate.ToString ("dd/MM/yy", frC);
        userEvent.StartTime = abs.StartDate.ToString ("HH:mm", frC);
        userEvent.EndTime = abs.EndDate.ToString ("HH:mm", frC);
        userEvent.Justified = abs.Justified == true ? "OUI" : "NON";
        userEvent.Reason = abs.Reason;
        userEvent.Comment = abs.Comment;
        events.Add (userEvent);
      }

      List<ClassEventWithNbDto> eventsWithNb = new List<ClassEventWithNbDto> ();
      int absTypeId = Convert.ToInt32 (_config.GetSection ("AppSettings:AbsenceTypeId").Value);
      int lateTypeId = Convert.ToInt32 (_config.GetSection ("AppSettings:LateTypeId").Value);
      int nbAbs = events.Where (e => e.ClassEventTypeId == 100 + absTypeId).Count ();
      int nbLate = events.Where (e => e.ClassEventTypeId == 100 + lateTypeId).Count ();
      eventsWithNb.Add (new ClassEventWithNbDto { Id = 100 + absTypeId, Name = "absence", NbTimes = nbAbs });
      eventsWithNb.Add (new ClassEventWithNbDto { Id = 100 + lateTypeId, Name = "retard", NbTimes = nbLate });

      var otherEvents = await _context.UserClassEvents
        .Include (i => i.User).ThenInclude (p => p.Photos)
        .Include (i => i.ClassEvent)
        .Include (i => i.DoneBy)
        .Where (a => a.User.ClassId == classId)
        .OrderByDescending (o => o.StartDate).ToListAsync ();

      foreach (var oe in otherEvents) {
        UserClassEventForListDto otherEvent = new UserClassEventForListDto ();
        otherEvent.Id = oe.Id;
        otherEvent.UserId = oe.UserId;
        otherEvent.UserName = oe.User.LastName + " " + oe.User.FirstName;
        if (oe.User.Photos.Count () > 0)
          otherEvent.PhotoUrl = oe.User.Photos.FirstOrDefault (p => p.IsMain).Url;
        otherEvent.ClassEventName = oe.ClassEvent.Name;
        otherEvent.ClassEventType = oe.ClassEvent.Name;
        otherEvent.DoneByName = oe.DoneBy.LastName + " " + oe.DoneBy.FirstName;
        otherEvent.StartDate = oe.StartDate;
        otherEvent.strStartDate = oe.StartDate.ToString ("dd/MM/yy", frC);
        otherEvent.EndDate = oe.EndDate;
        otherEvent.strEndDate = oe.StartDate.ToString ("dd/MM/yy", frC);
        otherEvent.StartTime = oe.StartDate.ToString ("HH:mm", frC);
        otherEvent.EndTime = oe.EndDate.ToString ("HH:mm", frC);
        otherEvent.Justified = oe.Justified == true ? "OUI" : "NON";
        otherEvent.Reason = oe.Reason;
        otherEvent.Comment = oe.Comment;
        events.Add (otherEvent);
      }

      var classEvents = await _context.ClassEvents.OrderBy (e => e.Name).ToListAsync ();
      foreach (var ce in classEvents) {
        int nb = otherEvents.Where (e => e.ClassEventId == ce.Id).ToList ().Count ();
        eventsWithNb.Add (new ClassEventWithNbDto { Id = ce.Id, Name = ce.Name, NbTimes = nb });
      }

      events = events.OrderByDescending (e => e.StartDate).ToList ();
      eventsWithNb = eventsWithNb.OrderBy (e => e.Name).ToList ();
      return Ok (new {
        events,
        eventsWithNb
      });
    }

    [HttpGet ("{classid}/Absences")]
    public async Task<IActionResult> GetClassAbsences (int classId) {
      var studentType = Convert.ToInt32 (_config.GetSection ("AppSettings:StudentTypeId").Value);
      var absences = await _context.Absences
        .Include (i => i.User)
        .Where (a => a.User.ClassId == classId && a.User.UserTypeId == studentType)
        .OrderByDescending (o => o.StartDate).ToListAsync ();

      var nbClassAbscences = absences.Count ();
      var absencesToReturn = _mapper.Map<IEnumerable<AbsencesToReturnDto>> (absences);
      return Ok (new {
        absences = absencesToReturn,
          nbAbsences = nbClassAbscences
      });
    }

    [HttpGet ("{classId}/ClassSanctions")]
    public async Task<IActionResult> GetClassSanctions (int classId) {
      var studentType = Convert.ToInt32 (_config.GetSection ("AppSettings:StudentTypeId").Value);
      var sanctions = await _context.UserSanctions
        .Include (i => i.Sanction)
        .Include (i => i.User)
        .Include (i => i.SanctionedBy)
        .Where (a => a.User.ClassId == classId && a.User.UserTypeId == studentType)
        .OrderByDescending (a => a.SanctionDate).ToListAsync ();

      var nbClassSanctions = sanctions.Count ();
      var sanctionsToReturn = _mapper.Map<IEnumerable<UserSanctionsToReturnDto>> (sanctions);
      return Ok (new {
        sanctions = sanctionsToReturn,
          nbSanctions = nbClassSanctions
      });
    }

    [HttpGet ("{classId}/ClassAgenda")]
    public async Task<IActionResult> GetClassAgenda (int classId, DateTime StartDate, DateTime EndDate) {
      var agendaItems = await _repo.GetClassAgenda (classId, StartDate, EndDate);

      if (agendaItems != null)
        return Ok (_repo.GetAgendaListByDueDate (agendaItems));

      return BadRequest ("Aucun agenda trouvé");
    }

    [HttpGet ("{claissId}/agendas/{agendaId}")]
    public async Task<IActionResult> GetAgendaById (int agendaId) {
      var agenda = await _repo.GetAgenda (agendaId);

      return Ok (agenda);
    }

    [HttpGet ("GetAgendaItem")]
    public async Task<IActionResult> GetAgendaItem ([FromQuery] AgendaParams agendaParams) {
      var classId = agendaParams.ClassId;
      var courseId = agendaParams.CourseId;
      var dueDate = agendaParams.DueDate;

      var agendaItem = await _context.Agendas.FirstOrDefaultAsync (i => i.Session.ClassId == classId &&
        i.Session.CourseId == courseId && i.Session.SessionDate == dueDate);

      //return Ok(agendaItem);
      if (agendaItem != null) {
        return Ok (agendaItem);
      } else {
        Agenda emptyAgendaItem = new Agenda ();
        return Ok (emptyAgendaItem);
      }

    }

    [HttpPost("SaveCourseWithConflict")]
    public async Task<IActionResult> SaveCourseWithConflict(ConflictDataDto conflictData)
    {
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();
      int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
      DateTime today = DateTime.Now;

      using(var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          int scheduleId = conflictData.ScheduleId;
          int scheduleCourseId = conflictData.ConflictedCourseId;
          ScheduleCourse oldCourse = scheduleCourses.First(c => c.Id == scheduleCourseId);
          DateTime oldStartHM = oldCourse.Schedule.StartHourMin;
          DateTime oldEndHM = oldCourse.Schedule.EndHourMin;

          Conflict conflict = new Conflict();
          conflict.ClassId = conflictData.ClassId;
          conflict.Day = conflictData.Day;
          // conflict.StartHourMin = oldStartHM;
          // conflict.EndHourMin = oldEndHM;

          //course added with tehe same schedule period
          if(scheduleCourseId == 0)
          {
            // conflict.StartHourMin = oldStartHM < conflictData.StartHourMin ? oldStartHM : conflictData.StartHourMin;
            // conflict.EndHourMin = oldEndHM < conflictData.EndHourMin ? oldEndHM : conflictData.EndHourMin;

            Schedule schedule = new Schedule();
            schedule.ClassId = conflictData.ClassId;
            schedule.Day = conflictData.Day;
            schedule.StartHourMin = conflictData.StartHourMin;
            schedule.EndHourMin = conflictData.EndHourMin;
            schedule.InsertDate = today;
            schedule.InsertUserId = currentUserId;
            schedule.UpdateDate = today;
            schedule.UpdateUserId = currentUserId;
            Guid GUID = Guid.NewGuid();
            schedule.Version = GUID.ToString();
            _repo.Add(schedule);
            scheduleId = schedule.Id;
          }

          _repo.Add(conflict);
          await _context.SaveChangesAsync();

          ScheduleCourse course = new ScheduleCourse();
          course.ScheduleId = scheduleId;
          course.TeacherId = conflictData.TeacherId;
          course.CourseId = conflictData.CourseId;
          course.InsertDate = today;
          course.InsertUserId = currentUserId;
          course.UpdateDate = today;
          course.UpdateUserId = currentUserId;
          Guid GUID1 = Guid.NewGuid();
          course.Version = GUID1.ToString();
          _repo.Add(course);
          await _context.SaveChangesAsync();

          CourseConflict courseConflict1 = new CourseConflict();
          courseConflict1.ConflictId = conflict.Id;
          courseConflict1.ScheduleCourseId = oldCourse.Id;
          _repo.Add(courseConflict1);
          CourseConflict courseConflict2 = new CourseConflict();
          courseConflict2.ConflictId = conflict.Id;
          courseConflict2.ScheduleCourseId = course.Id;
          _repo.Add(courseConflict2);

          if(await _repo.SaveAll())
          {
            identityContextTransaction.Commit();
            await _cache.LoadScheduleCourses();
            await _cache.LoadSchedules();
            return NoContent();
          }
        }
        catch
        {
          identityContextTransaction.Rollback();
        }

        return BadRequest("problème d'enregistrement des données");
      }
    }

    [HttpPut("saveSchedules")]
    public async Task<IActionResult> saveSchedules([FromBody] ScheduleDataDto[] schedules)
    {
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      using(var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          foreach(var sch in schedules)
          {
            //set the dates proprerly
            var StartHour = sch.StartHour;
            var StartMin = sch.StartMin;
            var StartHourMin = new DateTime (1, 1, 1, StartHour, StartMin, 0);
            var EndHour = sch.EndHour;
            var EndMin = sch.EndMin;
            var EndHourMin = new DateTime (1, 1, 1, EndHour, EndMin, 0);

            //is it a course with conflict?
            if(sch.ScheduleId == 0)
            {
              Schedule schedule = new Schedule();

              schedule.ClassId = sch.ClassId;
              schedule.Day = sch.Day;
              schedule.StartHourMin = StartHourMin;
              schedule.EndHourMin = EndHourMin;
              schedule.InsertDate = DateTime.Now;
              schedule.InsertUserId = currentUserId;
              schedule.UpdateDate = DateTime.Now;
              schedule.UpdateUserId = currentUserId;
              var GUID = Guid.NewGuid();
              schedule.Version = GUID.ToString();
              _repo.Add(schedule);
              await _context.SaveChangesAsync();

              ScheduleCourse scheduleCourse = new ScheduleCourse();
              scheduleCourse.ScheduleId = schedule.Id;
              scheduleCourse.TeacherId = sch.TeacherId;
              scheduleCourse.CourseId = sch.CourseId;
              scheduleCourse.InsertDate = DateTime.Now;
              scheduleCourse.InsertUserId = currentUserId;
              scheduleCourse.UpdateDate = DateTime.Now;
              scheduleCourse.UpdateUserId = currentUserId;
              var GUID1 = Guid.NewGuid();
              scheduleCourse.Version = GUID1.ToString();
              _repo.Add(scheduleCourse);
            }
            else //add course with conflict
            {
              List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();
              DateTime today = DateTime.Now;

              int scheduleId = sch.ScheduleId;
              int scheduleCourseId = sch.Id;
              ScheduleCourse oldCourse = scheduleCourses.First(c => c.Id == scheduleCourseId);

              // Conflict conflict = new Conflict();
              // conflict.ClassId = sch.ClassId;
              // conflict.Day = sch.Day;
              // conflict.ScheduleId = scheduleId;
              // _repo.Add(conflict);
              // await _context.SaveChangesAsync();

              ScheduleCourse course = new ScheduleCourse();
              course.ScheduleId = scheduleId;
              course.TeacherId = sch.TeacherId;
              course.CourseId = sch.CourseId;
              course.InsertDate = today;
              course.InsertUserId = currentUserId;
              course.UpdateDate = today;
              course.UpdateUserId = currentUserId;
              Guid GUID = Guid.NewGuid();
              course.Version = GUID.ToString();
              _repo.Add(course);
              // await _context.SaveChangesAsync();

              // CourseConflict courseConflict1 = new CourseConflict();
              // courseConflict1.ConflictId = conflict.Id;
              // courseConflict1.ScheduleCourseId = oldCourse.Id;
              // _repo.Add(courseConflict1);

              // CourseConflict courseConflict2 = new CourseConflict();
              // courseConflict2.ConflictId = conflict.Id;
              // courseConflict2.ScheduleCourseId = course.Id;
              // _repo.Add(courseConflict2);
            }
          }

          await _repo.SaveAll();
          identityContextTransaction.Commit();
          await _cache.LoadSchedules();
          await _cache.LoadScheduleCourses();
          // await _cache.LoadConflicts();
          // await _cache.LoadCourseConflicts();
          return NoContent();
        }
        catch(Exception ex)
        {
          var dd = ex.Message;
          identityContextTransaction.Rollback();
        }
      }

      return BadRequest("problème d'enregistrement des données");
    }

    [HttpPut ("SaveAgenda")]
    public async Task<IActionResult> SaveAgendaItem([FromBody] AgendaForSaveDto agendaForSaveDto)
    {
      var id = agendaForSaveDto.Id;
      Agenda newAgendaItem = new Agenda();
      if(id == 0)
      {
        _mapper.Map (agendaForSaveDto, newAgendaItem);
        newAgendaItem.DateAdded = DateTime.Now;
        newAgendaItem.DoneSetById = int.Parse(User.FindFirst (ClaimTypes.NameIdentifier).Value);
        _repo.Add(newAgendaItem);
      }
      else
      {
        var agendaItemFromRepo = await _repo.GetAgenda(id);
        _mapper.Map (agendaForSaveDto, agendaItemFromRepo);
        agendaItemFromRepo.DateAdded = DateTime.Now;
      }

      if(await _repo.SaveAll())
      {
        await _cache.LoadAgendas();
        if(id == 0)
          return Ok(newAgendaItem.Id);
        else
          return Ok(id);
      }

      throw new Exception ($"Updating/Saving agendaItem failed");
    }

    [HttpGet("GetCourses")]
    public async Task<IActionResult> GetCourses()
    {
      List<Course> coursesCached = await _cache.GetCourses();
      var courses = coursesCached.OrderBy(a => a.Name).ToList();
      return Ok(courses);
    }

    [HttpGet ("AllClasses")]
    public async Task<IActionResult> GetAllClasses()
    {
      List<Class> classes = await _cache.GetClasses();
      var les_classes = classes.OrderBy (a => a.Name).ToList();
      return Ok(les_classes);
    }

    [HttpGet("{teacherId}/FreePrimaryClasses/{educLevelId}")]
    public async Task<IActionResult> GetFreePrimaryClasses(int teacherId, int educLevelId)
    {
      List<ClassLevel> levels = await _cache.GetClassLevels();

      List<ClassByLevelDto> classesByLevel = new List<ClassByLevelDto>();
      if(teacherId != 0)
      {
        levels = levels.Where(c => c.EducationLevelId == educLevelId).OrderBy(c => c.DsplSeq).ToList();
      }

      var availableClasses = await _repo.GetFreePrimaryClasses(teacherId);
      foreach(var level in levels)
      {
        ClassByLevelDto cbl = new ClassByLevelDto();
        cbl.ClassLevelId = level.Id;
        cbl.LevelName = level.Name;
        cbl.EducLevelId = Convert.ToInt32(level.EducationLevelId);

        cbl.Classes = new List<Class>();
        var classes = availableClasses.Where(c => c.ClassLevelId == level.Id);
        foreach(var aclass in classes)
        {
          cbl.Classes.Add(aclass);
        }

        if (cbl.Classes.Count() > 0)
        {
          classesByLevel.Add(cbl);
        }
      }

      return Ok(classesByLevel);
    }

    [HttpGet ("ClassesByLevel")]
    public async Task<IActionResult> GetClassesByLevel()
    {
      List<ClassLevel> classlevelsCached = await _cache.GetClassLevels();

      List<ClassByLevelDto> classesByLevel = new List<ClassByLevelDto>();
      var levels = classlevelsCached.OrderBy(c => c.DsplSeq).ToList();
      foreach(var level in levels)
      {
        ClassByLevelDto cbl = new ClassByLevelDto();
        cbl.ClassLevelId = level.Id;
        cbl.LevelName = level.Name;

        cbl.Classes = new List<Class>();
        var classes = await _repo.GetClassesByLevelId(level.Id);
        foreach(var aclass in classes)
        {
          cbl.Classes.Add(aclass);
        }

        classesByLevel.Add(cbl);
      }

      return Ok(classesByLevel);
    }

    [HttpGet ("{classId}/classCourses")]
    public async Task<IActionResult> GetClassCourses(int classId) {
      var courses = await _context.ClassCourses
                            .Where(c => c.ClassId == classId)
                            .Select(s => s.Course).ToListAsync();

      return Ok (courses);
    }

    [HttpGet ("{classId}/CoursesWithAgenda/f/{daysToNow}/t/{daysFromNow}")]
    public async Task<IActionResult> GetClassCoursesWithAgenda(int classId, int daysToNow, int daysFromNow)
    {
      var today = DateTime.Now.Date;
      var startDate = today.AddDays(-daysToNow);
      var EndDate = today.AddDays(daysFromNow);

      var courses = await _context.ClassCourses
                            .Where(c => c.ClassId == classId)
                            .Select(s => s.Course).ToListAsync();

      var classAgenda = await _context.Agendas
        .OrderBy (o => o.Session.SessionDate)
        .Where (a => a.Session.ClassId == classId && a.Session.SessionDate.Date >= startDate && a.Session.SessionDate <= EndDate)
        .ToListAsync();

      List<CourseWithAgendaDto> coursesWithAgenda = new List<CourseWithAgendaDto>();
      foreach (var course in courses) {
        CourseWithAgendaDto cwa = new CourseWithAgendaDto ();
        cwa.Id = course.Id;
        cwa.Name = course.Name;
        cwa.Abbrev = course.Abbreviation;
        cwa.Color = course.Color;

        var items = classAgenda.Where(a => a.Session.CourseId == course.Id).ToList();
        List<AgendaItemDto> agendaItems = new List<AgendaItemDto>();
        foreach (var item in items)
        {
          AgendaItemDto aid = new AgendaItemDto();

          var strDateAdded = item.DateAdded.ToString ("ddd dd MMM", frC);
          var strDueDate = item.Session.SessionDate.ToString ("ddd dd MMM", frC);

          aid.strDateAdded = strDateAdded;
          aid.strDueDate = strDueDate;
          aid.TaskDesc = item.TaskDesc;
          aid.Done = item.Done;
          agendaItems.Add (aid);
        }
        cwa.AgendaItems = agendaItems;
        cwa.NbItems = cwa.AgendaItems.Count();

        coursesWithAgenda.Add (cwa);
      }

      return Ok (coursesWithAgenda.OrderBy (c => c.Name));
    }

    [HttpGet ("{classId}/AgendaNbDays")]
    public async Task<IActionResult> GetClassAgendaNbDays (int classId) {
      var toNbDays = 7;
      var agendasFromRepo = await _repo.GetClassAgendaTodayToNDays (classId, toNbDays);
      var agendaItems = _repo.GetAgendaListByDueDate (agendasFromRepo);
      return Ok (agendaItems);
    }

    [HttpGet ("{classId}/AgendaByDate")]
    public async Task<IActionResult> GetAgendaByDate (int classId, [FromQuery] AgendaParams agendaParams) {
      var nbDays = agendaParams.nbDays;
      var IsMovingPeriod = agendaParams.IsMovingPeriod;
      var startDate = agendaParams.CurrentDate.Date;
      var endDate = startDate.AddDays (nbDays).Date;
      if (IsMovingPeriod) {
        if (nbDays > 0) {
          startDate = agendaParams.CurrentDate.AddDays (1).Date;
          endDate = startDate.AddDays (nbDays).Date;
        } else {
          startDate = agendaParams.CurrentDate.AddDays (-1).Date;
          endDate = startDate.AddDays (nbDays).Date;
        }
      }

      if (startDate > endDate) {
        var temp = startDate;
        startDate = endDate;
        endDate = temp;
      }

      var strStartDate = startDate.ToString ("dd MMM", frC);
      var strEndDate = endDate.ToString ("dd MMM", frC);

      List<AgendaForListDto> AgendaList = await _repo.GetUserClassAgenda (classId, startDate, endDate);

      List<bool> dones = new List<bool> ();
      foreach (var al in AgendaList) {
        foreach (var item in al.AgendaItems) {
          dones.Add (item.Done);
        }
      }

      var courses = await _context.ClassCourses
                            .Where(c => c.ClassId == classId)
                            .Select(s => s.Course).ToListAsync();

      List<CourseTasksDto> coursesWithTasks = new List<CourseTasksDto>();
      foreach (var course in courses)
      {
        var nbItems = 0;
        foreach(var al in AgendaList)
        {
          foreach(var item in al.AgendaItems)
          {
            if(item.CourseId == course.Id)
              nbItems++;
          }
        }

        if (nbItems == 0)
          continue;

        CourseTasksDto ctd = new CourseTasksDto();
        ctd.CourseId = course.Id;
        ctd.CourseName = course.Name;
        ctd.CourseAbbrev = course.Abbreviation;
        ctd.CourseColor = course.Color;
        ctd.NbTasks = nbItems;
        coursesWithTasks.Add(ctd);
      }

      return Ok(new {
        agendaList = AgendaList,
          startDate = startDate,
          endDate = endDate,
          strStartDate = strStartDate,
          strEndDate = strEndDate,
          coursesWithTasks = coursesWithTasks.OrderBy(c => c.CourseName),
          dones = dones
      });
    }

    [HttpPost ("UpdateCourse/{courseId}")]
    public async Task<IActionResult> UpdateCourse (int courseId, CourseDto courseDto) {
      var courseFromRepo = await _repo.GetCourse (courseId);
      courseFromRepo.Name = courseDto.Name;
      courseFromRepo.Abbreviation = courseDto.Abbrev;
      courseFromRepo.Color = courseDto.Color;
      _repo.Update (courseFromRepo);
      if (await _repo.SaveAll ())
        return NoContent ();

      return BadRequest ("impossible de mettre à jour ce cours");

    }

    [HttpGet ("ClassLevels")]
    public async Task<IActionResult> GetLevels()
    {
      var levels = await _context.ClassLevels.OrderBy(c => c.DsplSeq).ToListAsync();
      return Ok(levels);
    }

    [HttpGet ("ClassLevels/{id}")]
    public async Task<IActionResult> GetLevelById(int id)
    {
      List<ClassLevel> levels = await _cache.GetClassLevels();
      ClassLevel level = levels.First(c => c.Id == id);
      return Ok(level);
    }

    [HttpGet("ClassLevelCourses")]
    public async Task<IActionResult> GetClassLevelCourses()
    {
      var levelcourses = await _context.ClassLevelCourses.ToListAsync();
      return Ok(levelcourses);
    }

    [HttpPost("saveCLCourses")]
    public async Task<IActionResult> SaveCLCourses(List<ClassLevelCourseDto> clCourses)
    {
      Boolean dbUpdated = false;
      foreach(var clcourse in clCourses)
      {
        var oldCourses = await _context.ClassLevelCourses.Where(c => c.ClassLevelId == clcourse.LevelId).ToListAsync();
        List<int> oldCourseIds = new List<int>();
        foreach(var course in oldCourses)
        {
          oldCourseIds.Add (course.CourseId);
        }

        List<int> newCourseIds = new List<int>();
        foreach(var course in clcourse.Courses)
        {
          newCourseIds.Add (course.Id);
        }

        //add new selected courses for the current classLevel
        foreach(var courseid in newCourseIds.Except (oldCourseIds))
        {
          ClassLevelCourse newCLCourse = new ClassLevelCourse();
          newCLCourse.ClassLevelId = clcourse.LevelId;
          newCLCourse.CourseId = courseid;
          _repo.Add(newCLCourse);
          dbUpdated = true;
        }

        //remove old selection
        if(oldCourseIds.Count() > 0)
        {
          List<ClassLevelCourse> oldCLCourses = new List<ClassLevelCourse>();
          foreach(var courseid in oldCourseIds.Except(newCourseIds))
          {
            ClassLevelCourse levelcourse = await _context.ClassLevelCourses
              .FirstAsync(c => c.ClassLevelId == clcourse.LevelId && c.CourseId == courseid);
            oldCLCourses.Add(levelcourse);
          }
          if(oldCLCourses.Count() > 0)
          {
            dbUpdated = true;
            _repo.DeleteAll(oldCLCourses);
          }
        }
      }

      if(dbUpdated && await _repo.SaveAll())
        return Ok();
      else
        return BadRequest("problème pour enregistrer les courses");
    }

    [HttpGet ("CoursesByLevelId/{levelId}")]
    public async Task<IActionResult> GetCoursesByLevelId (int levelId) {
      var courses = await _context.ClassLevelCourses.Where (c => c.ClassLevelId == levelId).ToListAsync ();
      return Ok (courses);
    }

    [HttpGet ("ActiveClasslevels")]
    public async Task<IActionResult> GetActiveClassLevels()
    {
      var classLevels = await _repo.GetActiveClassLevels();
      return Ok (classLevels);
    }

    [HttpGet ("EducLevels")]
    public async Task<IActionResult> GetEducLevels () {
      var educlevels = await _repo.GetEducationLevels ();
      return Ok (educlevels);
    }

    [HttpGet ("LevelsWithClasses")]
    public async Task<IActionResult> GetLevelsWithClasses()
    {
      List<Class> classesCached = await _cache.GetClasses();
      List<User> studentsCached = await _cache.GetStudents();

      var classLevels = classesCached
                              .OrderBy(o => o.ClassLevel.DsplSeq)
                              .Select(s => s.ClassLevel)
                              .Distinct().ToList();

      var dataToReturn = new List<ClassLevelDetailDto>();
      foreach(var item in classLevels)
      {
        var res = new ClassLevelDetailDto();
        res.Id = item.Id;
        res.Name = item.Name;
        List<Class> classes = classesCached.Where(c => c.ClassLevelId == item.Id).ToList();
        res.TotalClasses = classes.Count();
        res.Classes = new List<ClassDetailDto>();
        foreach (var aclass in classes)
        {
          List<User> students = studentsCached.Where(u => u.ClassId == aclass.Id).ToList();
          res.TotalStudents += students.Count();
          //add class data
          ClassDetailDto cdd = new ClassDetailDto();
          cdd.Id = aclass.Id;
          cdd.Name = aclass.Name;
          cdd.ClassLevelId = item.Id;
          cdd.CycleId = Convert.ToInt32(item.CycleId);
          cdd.EducationLevelId = Convert.ToInt32(item.EducationLevelId);
          cdd.SchoolId = Convert.ToInt32(item.SchoolId);
          cdd.MaxStudent = aclass.MaxStudent;
          cdd.TotalStudents = students.Count();
          res.Classes.Add(cdd);
        }

        dataToReturn.Add(res);
      }

      return Ok(dataToReturn);
    }

    [HttpPost("ClassLevelsWithClasses")]
    public async Task<IActionResult> GetClassLevelWithClasses(List<int> CLIds)
    {
      // List<Class> classesCached = await _cache.GetClasses();
      // List<ClassLevel> classLevelsCached = await _cache.GetClassLevels();

      List<ClassLevel> classLevels = await _context.ClassLevels.Where(c => CLIds.Contains(c.Id)).ToListAsync();
      List<ClassLevelDto> classLevelsDto = _mapper.Map<List<ClassLevelDto>>(classLevels);

      foreach(var cl in classLevelsDto)
      {
        cl.Classes = await _context.Classes
                      .OrderBy(c => c.Name)
                      .Where(c => c.ClassLevelId == cl.Id).ToListAsync();
      }

      return Ok(classLevels);
    }

    [HttpGet("{classLevelId}/ClassesByLevelId")]
    public async Task<IActionResult> ClassesByLevelId(int classLevelId)
    {
      List<Class> classesCached = await _cache.GetClasses();

      List<Class> classes = classesCached.OrderBy(o => o.Name)
                                         .Where(c => c.ClassLevelId == classLevelId)
                                         .ToList();

      var classesToReturn = _mapper.Map<IEnumerable<ClassDetailDto>>(classes);
      return Ok(classesToReturn);
    }

    [HttpPost ("{classId}/DeleteClass")]
    public async Task<IActionResult> DeleteClass (int classId)
    {
      _repo.Delete(_context.Classes.FirstOrDefault(e => e.Id == classId));
      if (await _repo.SaveAll())
        return Ok (classId);
      return BadRequest("impossible de supprimer cette classe");
    }

    [HttpPost("AddCourse")]
    public async Task<IActionResult> AddCourse([FromBody] CourseDto courseDto)
    {
      int id = courseDto.Id;
      if(id == 0)
      {
        var course = new Course {
          Name = courseDto.Name,
          CourseTypeId = courseDto.CourseTypeId,
          Abbreviation = courseDto.Abbrev,
          Color = courseDto.Color
        };
        _repo.Add(course);
      }
      else
      {
        var course = await _context.Courses.FirstAsync(c => c.Id == id);
        course.Name = courseDto.Name;
        course.Abbreviation = courseDto.Abbrev;
        course.Color = courseDto.Color;
        _repo.Update(course);
      }

      if(await _repo.SaveAll())
      {
        await _cache.LoadCourses();
        return Ok();
      }

      return BadRequest("impossible d'ajouter ce cours");
    }

    [HttpGet("SessionData/{sessionId}")]
    public async Task<IActionResult> GetSessionData(int sessionId)
    {
      List<Session> sessions = await _cache.GetSessions();

      // get session
      var sessionFromRepo = sessions.FirstOrDefault(s => s.Id == sessionId);
      var session = _mapper.Map<SessionToReturnDto>(sessionFromRepo);

      IEnumerable<AbsenceForCallSheetDto> sessionAbsences = new List<AbsenceForCallSheetDto>();
      if(session != null)
      {
        var absences = await _context.Absences.Where(a => a.SessionId == session.Id).ToListAsync();
        sessionAbsences = _mapper.Map<IEnumerable<AbsenceForCallSheetDto>>(absences);
      }

      var studentsFromRepo = await _repo.GetClassStudents(session.ClassId);
      var classStudents = _mapper.Map<IEnumerable<UserForCallSheetDto>>(studentsFromRepo);

      return Ok(new {
        session,
        classStudents,
        sessionAbsences
      });
    }

    [HttpGet ("{classId}/CallSheet/Students")]
    public async Task<IActionResult> GetCallSheetStudents (int classId) {
      var studentsFromRepo = await _repo.GetClassStudents (classId);
      var classStudents = _mapper.Map<IEnumerable<UserForCallSheetDto>> (studentsFromRepo);
      return Ok (classStudents);
    }

    [HttpGet ("{classId}/ScheduleNDays")]
    public async Task<IActionResult> GetScheduleNDays (int classId)
    {
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
        if(currentDate.Date == today)
        {
          todayIndex = i;
        }
        var dayInt = (int)currentDate.DayOfWeek == 0 ? 7 : (int)currentDate.DayOfWeek;
        item.Day = dayInt;
        item.DayDate = currentDate;
        item.strDayDate = currentDate.ToString("ddd dd MMM", frC);
        item.Courses = new List<ClassDayCoursesDto>();
        var daySchedules = itemsFromRepo.Where(s => s.Day == dayInt);
        item.Courses = _repo.GetCoursesFromSchedules(daySchedules);
        scheduleDays.Add(item);
      }
      return Ok(new {
        scheduleDays,
        todayIndex
      });
    }

    [HttpGet ("Schedule/{id}")]
    public async Task<IActionResult> GetSchedule(int id)
    {
      var schedule = await _context.Schedules
                            .Include (i => i.Class)
                            .FirstOrDefaultAsync (s => s.Id == id);;
      var scheduleToReturn = _mapper.Map<ScheduleToReturnDto> (schedule);

      return Ok (scheduleToReturn);
    }

    [HttpGet ("Sessions/{id}")]
    public async Task<IActionResult> GetSession (int id) {
      var sessionFromDB = await _context.Sessions.FirstOrDefaultAsync (s => s.Id == id);
      var session = _mapper.Map<SessionToReturnDto> (sessionFromDB);
      return Ok (session);
    }

    [HttpGet("Course/{courseId}/Session")]
    public async Task<IActionResult> GetSessionFromSchedule(int courseId)
    {
      List<Schedule> schedules = await _cache.GetSchedules();
      List<ScheduleCourse> scheduleCourses = await _cache.GetScheduleCourses();
      List<Session> sessions = await _cache.GetSessions();

      var course = scheduleCourses.FirstOrDefault(s => s.Id == courseId);
      if(course == null)
        return BadRequest("problème pour créer la session du cours.");

      var scheduleDay = course.Schedule.Day;
      var today = DateTime.Now.Date;
      // monday=1, tue=2, ...
      var todayDay = ((int)today.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;

      // get session by schedule and date
      var sessionFromDB = sessions.FirstOrDefault(s => s.ScheduleCourseId == course.Id && s.SessionDate.Date == today);
      if(sessionFromDB != null)
      {
        var session = _mapper.Map<SessionToReturnDto>(sessionFromDB);
        return Ok(session);
      }
      else
      {
        int teacherId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        ScheduleCourse scheduleCourse = scheduleCourses.First(s => s.Id == course.Id && s.TeacherId == teacherId);

        var newSession = new Session {
          ScheduleCourseId = scheduleCourse.Id,
          TeacherId = teacherId,
          ClassId = scheduleCourse.Schedule.ClassId,
          CourseId = course.CourseId,
          StartHourMin = scheduleCourse.Schedule.StartHourMin,
          EndHourMin = scheduleCourse.Schedule.EndHourMin,
          SessionDate = today
        };

        _context.Add(newSession);

        if(await _repo.SaveAll())
        {
          await _cache.LoadSessions();
          var session = _mapper.Map<SessionToReturnDto>(newSession);
          return Ok(session);
        }

        return BadRequest("problème pour récupérer la session");
      }
    }

    [HttpGet("courses/{courseId}/teacher/{teacherId}/Program")]
    public async Task<IActionResult> ClassesWithProgram(int courseId, int teacherId)
    {
      // course data
      var course = await _context.Courses.FirstAsync(c => c.Id == courseId);
      //get teacher data (classes & courses)
      List<TeacherClassesDto> teacherClasses = await _repo.GetTeacherClasses(teacherId);

      //for each class get themes and lessons...
      List<ClassWithThemesDto> classesWithProgram = new List<ClassWithThemesDto>();
      foreach(var aclass in teacherClasses)
      {
        ClassWithThemesDto cwtd = new ClassWithThemesDto();
        cwtd.ClassId = aclass.ClassId;
        cwtd.ClassName = aclass.ClassName;
        cwtd.CourseId = courseId;
        cwtd.CourseName = course.Name;
        cwtd.CourseAbbrev = course.Abbreviation;

        //Progeam from Theme
        cwtd.Themes = new List<ThemeDto>();
        var themesFromDB = await _context.Themes
          .Where (t => t.ClassLevelId == aclass.ClassLevelId && t.CourseId == courseId)
          .ToListAsync();
        foreach(var theme in themesFromDB)
        {
          ThemeDto td = new ThemeDto();
          td.Id = theme.Id;
          td.Name = theme.Name;
          td.ClassLevelId = theme.ClassLevelId;
          td.CourseId = theme.CourseId;
          td.Desc = theme.Desc;
          cwtd.Themes.Add(td);

          td.Lessons = new List<LessonDto>();
          var themeLessons = await _context.Lessons.Where(i => i.ThemeId == theme.Id).ToListAsync();
          foreach(var lesson in themeLessons)
          {
            LessonDto ld = new LessonDto();
            ld.Id = lesson.Id;
            ld.ThemeId = lesson.ThemeId;
            ld.ClassLevelId = lesson.ClassLevelId;
            ld.CourseId = lesson.CourseId;
            ld.Name = lesson.Name;
            ld.Desc = lesson.Desc;
            td.Lessons.Add(ld);

            ld.Contents = new List<LessonContentDto>();
            var lessonContents = await _context.LessonContents.Where (c => c.LessonId == lesson.Id).ToListAsync();
            foreach(var content in lessonContents)
            {
              LessonContentDto lcd = new LessonContentDto();
              lcd.Id = content.Id;
              lcd.LessonId = content.LessonId;
              lcd.Name = content.Name;
              lcd.Desc = content.Desc;
              lcd.NbHours = content.NbHours;
              lcd.SessionNum = content.SessionNum;
              ld.Contents.Add(lcd);
            }
          }
        }

        //Progeam from Lessons (when lessons don't come from themes)
        cwtd.Lessons = new List<LessonDto> ();
        var LessonsFromDB = await _context.Lessons
          .Where (t => t.ClassLevelId == aclass.ClassLevelId && t.CourseId == courseId)
          .ToListAsync ();
        foreach (var lesson in LessonsFromDB) {
          LessonDto ld = new LessonDto ();
          ld.Id = lesson.Id;
          ld.ThemeId = lesson.ThemeId;
          ld.ClassLevelId = lesson.ClassLevelId;
          ld.CourseId = lesson.CourseId;
          ld.Name = lesson.Name;
          ld.Desc = lesson.Desc;
          cwtd.Lessons.Add (ld);

          ld.Contents = new List<LessonContentDto> ();
          var lessonContents = await _context.LessonContents.Where (c => c.LessonId == lesson.Id).ToListAsync ();
          foreach (var content in lessonContents) {
            LessonContentDto lcd = new LessonContentDto ();
            lcd.Id = content.Id;
            lcd.LessonId = content.LessonId;
            lcd.Name = content.Name;
            lcd.Desc = content.Desc;
            lcd.NbHours = content.NbHours;
            lcd.SessionNum = content.SessionNum;
            ld.Contents.Add (lcd);
          }
        }
        classesWithProgram.Add (cwtd);
      }

      return Ok (classesWithProgram);
    }

    [HttpPut ("SaveCallSheet/{sessionId}")]
    public async Task<IActionResult> SaveCallSheet(int sessionId, [FromBody] Absence[] absences)
    {
      //delete old absents (update: delete + add)
      if(sessionId > 0)
      {
        List<Absence> oldAbsences = await _context.Absences.Where(a => a.SessionId == sessionId).ToListAsync();
        if (oldAbsences.Count () > 0)
          _repo.DeleteAll(oldAbsences);
      }

      // absence Sms data
      int absenceSmsId = _config.GetValue<int>("AppSettings:AbsenceSms");
      int lateSmsId = _config.GetValue<int>("AppSettings:LateSms");
      var AbsenceSms = await _context.SmsTemplates.FirstOrDefaultAsync(s => s.Id == absenceSmsId);
      var LateSms = await _context.SmsTemplates.FirstOrDefaultAsync(s => s.Id == lateSmsId);

      var ids = absences.Select (u => u.UserId);
      var parents = _context.UserLinks.Where (u => ids.Contains (u.UserId)).Distinct ().ToList ();
      List<AbsenceSmsDto> absSmsData = new List<AbsenceSmsDto> ();

      int absTypeId = _config.GetValue<int> ("AppSettings:AbsenceTypeId");
      int lateTypeId = _config.GetValue<int> ("AppSettings:LateTypeId");

      //set absence sms data
      var sessionFromDB = await _context.Sessions
        .Include (i => i.Course)
        .FirstAsync (s => s.Id == sessionId);
      var session = _mapper.Map<SessionToReturnDto> (sessionFromDB);

      var dateData = session.strSessionDate.Split("/");
      string day = dateData[0];
      string month = dateData[1];
      string year = dateData[2];
      string hourMin = session.EndHourMin;
      string deliveryTime = year + "-" + month + "-" + day + "T" + hourMin + ":00";

      Period currPeriod = await _repo.GetPeriodFromDate(DateTime.Now);

      //add new absents
      for(int i = 0; i < absences.Length; i++)
      {
        Absence absence = absences[i];
        absence.PeriodId = currPeriod.Id;
        _repo.Add(absence);

        int childId = absence.UserId;
        var child = await _context.Users.FirstAsync (u => u.Id == childId);
        List<int> parentIds = parents.Where (p => p.UserId == childId).Select (p => p.UserPId).ToList ();
        foreach (var parentId in parentIds) {
          //did we already add the sms to DB? if yes remove it (updated one is coming...)
          Sms oldSms = await _context.Sms.FirstOrDefaultAsync (s => s.SessionId == sessionId && s.ToUserId == parentId &&
            s.StudentId == childId && s.StatusFlag == 0);
          if (oldSms != null)
            _repo.Delete (oldSms);

          // is the parent subscribed to the Absence/Late sms?
          var userTemplate = new UserSmsTemplate ();
          if (absence.AbsenceTypeId == absTypeId) {
            userTemplate = await _context.UserSmsTemplates.FirstOrDefaultAsync (
              u => u.ParentId == parentId && u.SmsTemplateId == absenceSmsId &&
              u.ChildId == childId);
          } else {
            userTemplate = await _context.UserSmsTemplates.FirstOrDefaultAsync (
              u => u.ParentId == parentId && u.SmsTemplateId == lateSmsId &&
              u.ChildId == childId);
          }

          //set sms data
          if (userTemplate != null) {
            var parent = await _context.Users.FirstAsync(p => p.Id == parentId);
            AbsenceSmsDto asd = new AbsenceSmsDto ();
            asd.ChildId = childId;
            asd.AbsenceTypeId = absence.AbsenceTypeId;
            asd.ChildFirstName = child.FirstName;
            asd.ChildLastName = child.LastName;
            asd.ParentId = parent.Id;
            asd.ParentFirstName = parent.FirstName;
            asd.ParentLastName = parent.LastName.FirstLetterToUpper ();
            asd.ParentGender = parent.Gender;
            asd.CourseName = session.CourseAbbrev;
            asd.SessionDate = session.SessionDate.ToString ("dd/MM/yyyy", frC);
            asd.CourseStartHour = session.StartHourMin;
            asd.CourseEndHour = session.EndHourMin;
            asd.ParentCellPhone = parent.PhoneNumber;
            asd.LateInMin = (absence.EndDate - absence.StartDate).TotalMinutes.ToString ();
            //asd.scheduledDeliveryTime = deliveryTime;
            absSmsData.Add (asd);
          }
        }
      }

      int currentTeacherId = int.Parse (User.FindFirst (ClaimTypes.NameIdentifier).Value);

      List<Sms> absSms = await _repo.SetSmsDataForAbsences (absSmsData, sessionId, currentTeacherId);
      _context.AddRange (absSms);

      if (await _repo.SaveAll ())
        return Ok ();

      throw new Exception($"la saisie de l'apppel a échoué");
    }

    [HttpGet ("absences/{sessionId}")]
    public async Task<IActionResult> GetAbsencesBySessionId (int sessionId)
    {
      var absences = await _context.Absences.Where(a => a.SessionId == sessionId).ToListAsync();
      return Ok(absences);
    }

    [HttpGet("GetAllCoursesDetails")]
    public async Task<IActionResult> GetAllCoursesDetails()
    {
      List<Course> coursesCached = await _cache.GetCourses();
      List<ClassCourse> classcourses = await _context.ClassCourses.ToListAsync();
      List<User> studentsCached = await _cache.GetStudents();

      var data = new List<CoursesDetailsDto>();
      var courses = coursesCached.OrderBy(c => c.Name).ToList();
      foreach(var course in courses)
      {
        var courseDto = new CoursesDetailsDto {
          Id = course.Id,
          Name = course.Name,
          Abbreviation = course.Abbreviation,
          Color = course.Color
        };
        courseDto.NbTeachers = classcourses.Where(a => a.CourseId == course.Id).Select(s => s.TeacherId).Distinct().Count();
        List<int> classIds = classcourses.Where(a => a.CourseId == course.Id).Select(a => a.ClassId).ToList();
        courseDto.NbClasses = classIds.Count();
        courseDto.NbStudents = studentsCached.Where(a => classIds.Contains(Convert.ToInt32(a.ClassId))).Count();
        data.Add(courseDto);
      }

      return Ok (data);
    }

    [HttpPost("AddClassToLevel")]
    public async Task<IActionResult> AddClassToClassLevel(AddClassDataDto classData)
    {
      int levelId = classData.LevelId;
      int classTypeId = classData.ClassTypeId;
      int maxStudent = classData.MaxStudent;

      List<Class> classes = await _cache.GetClasses();
      List<ClassType> classTypes = await _cache.GetClassTypes();
      List<ClassLevel> classLevels = await _cache.GetClassLevels();

      Class lastClass = classes.OrderByDescending(o => o.Name)
                               .Where(c => c.ClassLevelId == levelId).First();
      string className = lastClass.Name;
      var suffix = className.Last();

      string newSuffix = "";
      if(suffix.ToString().IsNumeric())
      {
        newSuffix = (Convert.ToInt32(suffix.ToString()) + 1).ToString();
      }
      else
      {
        newSuffix = ((char)(((int)suffix) + 1)).ToString();
      }

      string classTypeCode = "";
      if(classTypeId != 0)
      {
        classTypeCode = (classTypes.First(t => t.Id == classTypeId)).Code;
      }

      string levelName = (classLevels.First(c => c.Id == levelId)).NameAbbrev;

      string newClassName = "";
      if(classTypeId != 0)
      {
        newClassName = levelName + " " + classTypeCode + " " + newSuffix;
      }
      else
      {
        newClassName = levelName + " " + newSuffix;
      }

      Class newClass = new Class ();
      newClass.ClassLevelId = levelId;
      if(classTypeId != 0)
        newClass.ClassTypeId = classTypeId;
      newClass.Name = newClassName;
      newClass.Active = 1;
      newClass.MaxStudent = maxStudent;
      _repo.Add(newClass);

      if(await _repo.SaveAll())
      {
        await _cache.LoadClasses();
        return Ok();
      }

      return BadRequest("problème pour ajouter la classe");
    }

    [HttpPost("SaveClasses")]
    public async Task<IActionResult> SaveClasses(ClassForSaveDto classToSave)
    {
      try
      {
        List<ClassLevel> classlevels = await _cache.GetClassLevels();
        var levelName = "";
        string levelAbbrev = (classlevels.FirstOrDefault(e => e.Id == classToSave.LevelId)).NameAbbrev;
        if(classToSave.Name != "")
        {
          levelName = levelAbbrev + " " + classToSave.Name;
        }
        else
        {
          levelName = levelAbbrev;
        }

        if (classToSave.suffixe != null)
        {
          //suffixe is a letter
          if (classToSave.suffixe == 1)
          {
            //suffixe alphabetic
            var compteur = 1;
            for (char i = 'A'; i < 'Z'; i++)
            {
              if(compteur <= classToSave.NbClass)
              {
                var newClass = new Class {
                  Name = levelName + " " + i,
                  ClassLevelId = classToSave.LevelId,
                  Active = 1,
                  ClassTypeId = classToSave.classTypeId,
                  MaxStudent = classToSave.maxStudent
                };
                await _context.Classes.AddAsync(newClass);
                compteur++;
              }
            }
          }
          else
          {
            //suffixe numeric
            for(int i = 1; i <= classToSave.NbClass; i++)
            {
              var newClass = new Class {
                Name = levelName + " " + i,
                ClassLevelId = classToSave.LevelId,
                Active = 1,
                ClassTypeId = classToSave.classTypeId,
                MaxStudent = classToSave.maxStudent
              };
              await _context.Classes.AddAsync(newClass);
            }
          }
        }
        else
        {
          var newClass = new Class {
            Name = classToSave.Name != "" ? levelName + " " + classToSave.ClassTypeCode + " " + classToSave.Name :
              levelName + " " + classToSave.ClassTypeCode,
            ClassLevelId = classToSave.LevelId,
            MaxStudent = classToSave.maxStudent,
            Active = 1,
            ClassTypeId = classToSave.classTypeId
          };
          await _context.Classes.AddAsync(newClass);
        }
        
        await _repo.SaveAll();
        await _cache.LoadClasses();
        return Ok();
      } 
      catch
      {
        return BadRequest("problème pour enregistrer les classes");
      }
    }

    [HttpGet("TeachersWithCourses")]
    public async Task<IActionResult> GetTeachersWithCourses()
    {
      List<ClassCourse> classCoursesCached = await _cache.GetClassCourses();
      List<TeacherCourse> teacherCoursesCached = await _cache.GetTeacherCourses();
      List<User> teachersCached = await _cache.GetTeachers();

      List<User> teachers = teachersCached
                                                // .Include(i => i.EducLevel)
                                                // .Include(i => i.Class)
                                                // .Include(i => i.Photos)
                                                // .Where(u => u.UserTypeId == teacherTypeId)
                                                .OrderBy(t => t.LastName).ThenBy(t => t.FirstName).ToList();

      var teachersToReturn = new List<TeacherForListDto>();
      foreach(var teacher in teachers)
      {
        var tdetails = new TeacherForListDto();
        tdetails.PhoneNumber = teacher.PhoneNumber;
        tdetails.SecondPhoneNumber = teacher.SecondPhoneNumber;
        tdetails.Email = teacher.Email;
        if (teacher.EducLevelId != null)
        {
          tdetails.EducLevelId = Convert.ToInt32(teacher.EducLevelId);
          tdetails.EducLevelName = teacher.EducLevel.Name;
        }
        if (teacher.ClassId != null) 
        {
          tdetails.ClassId = Convert.ToInt32 (teacher.ClassId);
          tdetails.ClassName = teacher.Class.Name;
        }
        tdetails.Id = teacher.Id;
        tdetails.LastName = teacher.LastName;
        tdetails.FirstName = teacher.FirstName;
        tdetails.Gender = teacher.Gender;
        tdetails.DateOfBirth = teacher.DateOfBirth.ToString("dd/MM/yyyy", frC);
        tdetails.Validated = teacher.Validated;
        var photo = teacher.Photos.FirstOrDefault(i => i.IsMain == true);
        if(photo != null)
          tdetails.PhotoUrl = photo.Url;

        tdetails.CourseClasses = new List<TeacherCourseClassesDto>();
        var teacherCourses = teacherCoursesCached
                                                  // .Include(i => i.Course)
                                                          .Where(t => t.TeacherId == teacher.Id).ToList();
        foreach(var course in teacherCourses)
        {
          var cdetails = new TeacherCourseClassesDto();
          cdetails.Course = course.Course;

          cdetails.Classes = new List<Class>();
          var classes = classCoursesCached
                                        // .Include(i => i.Class)
                                                   .Where(t => t.TeacherId == teacher.Id && t.CourseId == course.CourseId)
                                                   .OrderBy(o => o.Class.ClassLevel.DsplSeq)
                                                   .ToList();
          
          if(classes != null & classes.Count() > 0)
            cdetails.Classes = classes.Select(c => c.Class).ToList();
          tdetails.CourseClasses.Add(cdetails);
        }
        teachersToReturn.Add(tdetails);
      }

      return Ok(teachersToReturn);
    }

    [HttpPost("{id}/UpdateTeacher")]
    public async Task<IActionResult> UpdateTeacher(int id, UserForUpdateDto teacherForUpdate)
    {
      if(teacherForUpdate.CourseIds.Count() > 0)
      {
        // les cours sont bien renseignés 
        var courseIds = new List<int>();
        foreach(var item in teacherForUpdate.CourseIds)
        {
          courseIds.Add(Convert.ToInt32(item));
        }

        var teacherCourses = await _context.TeacherCourses.Where(t => t.TeacherId == id).ToListAsync();
        // recupartion des courseId du profésseur
        var ccIds = teacherCourses.Select(c => c.CourseId).ToList();

        foreach(var courId in courseIds.Except(ccIds))
        {
          //ajout d'une nouvelle ligne dans TeacheCourses
          var cl = new TeacherCourse { TeacherId = id, CourseId = courId };
          _repo.Add (cl);
        }

        foreach(var courId in ccIds.Except(courseIds)) {
          var currentLines = _context.ClassCourses.Where(c => c.CourseId == courId && c.TeacherId == id);
          if(currentLines.Count() == 0)
            _repo.Delete(teacherCourses.FirstOrDefault(t => t.TeacherId == id && t.CourseId == courId));
        }

        var userFromRepo = await _repo.GetUser(id, false);
        userFromRepo.FirstName = teacherForUpdate.FirstName;
        userFromRepo.LastName = teacherForUpdate.LastName;
        if (teacherForUpdate.DateOfBirth != null)
          userFromRepo.DateOfBirth = Convert.ToDateTime(teacherForUpdate.DateOfBirth);
        userFromRepo.Email = teacherForUpdate.Email;
        userFromRepo.PhoneNumber = teacherForUpdate.PhoneNumber;
        userFromRepo.SecondPhoneNumber = teacherForUpdate.SecondPhoneNumber;
        _repo.Update(userFromRepo);

        if (await _repo.SaveAll())
          return Ok();

        return BadRequest("impossible de terminer l'action");
      }
      return BadRequest("veuillez selectionner au moins un cours");
    }

    [HttpPost("{id}/{courseId}/{levelId}/SaveTeacherAffectation")]
    public async Task<IActionResult> SaveTeacherAffectation (int id, int courseId, int levelId, [FromBody] List<int> classIds)
    {
      var classcourses = await _context.ClassCourses
                      .Where(c => c.CourseId == courseId && c.Class.ClassLevelId == levelId)
                      .ToListAsync();

      Boolean dataToBeSaved = false;

      // récupération des classIds
      var cids = classcourses.Select(c => c.ClassId).Distinct ().ToList ();

      //add new affection (new lines in DB)
      foreach (var item in classIds.Except (cids)) {
        ClassCourse classCourse = new ClassCourse {
          CourseId = courseId,
          ClassId = item,
          TeacherId = id
        };
        _context.Add (classCourse);
        dataToBeSaved = true;
      }

      if(dataToBeSaved && await _repo.SaveAll())
      {
        // await _cache.LoadUsers();
        return Ok();
      } else {
        return NoContent();
      }

      //return BadRequest("problème pour affecter les classes");
    }

    [HttpGet ("TeacherClassCoursByLevel/{teacherId}/{levelId}/{courseId}")]
    public async Task<IActionResult> TeacherClassCoursByLevel (int teacherid, int levelId, int courseId)
    {
      var res = await _context.ClassCourses
        .Where(c => c.TeacherId == teacherid && c.CourseId == courseId && c.Class.ClassLevelId == levelId)
        .ToListAsync();

      return Ok(res);
    }

    [HttpGet("Course/{courseId}")]
    public async Task<IActionResult> Course(int courseId)
    {
      var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
      return Ok(course);
    }

    [HttpGet("ClassTypes")]
    public async Task<IActionResult> ClassTypes()
    {
      var classTypes = await _repo.GetClassTypes();
      return Ok(classTypes);
    }

    [HttpGet("CLClassTypes")]
    public async Task<IActionResult> CLClassTypes()
    {
      var clclasstypes =  await _context.ClassLevelClassTypes.ToListAsync();
      return Ok(clclasstypes);
    }

    [HttpGet("ClassTypesByLevel")]
    public async Task<IActionResult> GetClassTypesByLevel()
    {
      List<ClassLevelClassType> CLClassTypesCached = await _context.ClassLevelClassTypes.Include(i => i.ClassType).ToListAsync();
      List<ClassLevel> levels = await _context.ClassLevels.ToListAsync();

      List<ClassTypesLevelDto> levelsWithClassTypes = new List<ClassTypesLevelDto>();
      foreach (var level in levels)
      {
        ClassTypesLevelDto levelDto = new ClassTypesLevelDto();
        levelDto.LevelId = level.Id;
        levelDto.LevelName = level.Name;
        if(level.CycleId != null)
          levelDto.CycleId = Convert.ToInt32(level.CycleId);
        
        levelDto.ClassTypes = new List<ClassType>();
        levelDto.ClassTypes = CLClassTypesCached.Where(c => c.ClassLevelId == level.Id).Select(s => s.ClassType).ToList();

        levelsWithClassTypes.Add(levelDto);
      }

      return Ok(levelsWithClassTypes);
    }

    [HttpPost("CreateCourseCoefficient")]
    public async Task<IActionResult> CreateCourseCoefficient(CreateCoefficientDto coefficientToCreate)
    {
      var coefficient = _mapper.Map<CourseCoefficient>(coefficientToCreate);
      _repo.Add(coefficient);
      if (await _repo.SaveAll())
        return Ok();

      return BadRequest ("impossible de faire l'ajout");
    }

    [HttpGet ("ClassLevelCoefficients/{classLevelId}")]
    public async Task<IActionResult> ClassLevelCoefficients (int classLevelId)
    {
      var coefficients = await _context.CourseCoefficients
        .Include (c => c.ClassType)
        .Include (c => c.Course)
        .Where (c => c.ClassLevelid == classLevelId).ToListAsync ();
      return Ok (coefficients);
    }

    [HttpGet ("CourseCoefficient/{id}")]
    public async Task<IActionResult> CourseCoefficient (int id)
    {
      var courseCoef = await _context.CourseCoefficients.FirstOrDefaultAsync (a => a.Id == id);
      return Ok (courseCoef);
    }

    [HttpPost ("EditCoefficient/{id}/{coeffificient}")]
    public async Task<IActionResult> EditCoefficient (int id, int coeffificient)
    {
      var coef = await _context.CourseCoefficients.FirstOrDefaultAsync (i => i.Id == id);
      if (coef != null) {
        coef.Coefficient = coeffificient;
        if (await _repo.SaveAll ())
          return Ok ();

        return BadRequest ("impossible de faire la mise à jour");
      }
      return NotFound ();
    }

    [HttpPost ("SaveNewTheme")]
    public async Task<IActionResult> SaveNewTheme (NewThemeDto newThemeDto)
    {
      int themeId = 0;
      if (!string.IsNullOrEmpty (newThemeDto.Name)) {
        //enregistrement du theme
        var theme = new Theme {
          Name = newThemeDto.Name,
          Desc = newThemeDto.Desc,
          CourseId = newThemeDto.CourseId,
          ClassLevelId = newThemeDto.ClassLevelId
        };
        _context.Add (theme);
        themeId = theme.Id;
      }

      foreach (var less in newThemeDto.lessons) {
        var lesson = new Lesson { Name = less.Name, Desc = less.Desc, DsplSeq = less.DsplSeq };

        if (themeId != 0) {
          lesson.ThemeId = themeId;
        } else {
          lesson.CourseId = newThemeDto.CourseId;
          lesson.ClassLevelId = newThemeDto.ClassLevelId;
        }
        _context.Add (lesson);

        // enregistrement des contents
        foreach (var cont in less.contents) {
          var content = new LessonContent {
            Name = cont.Name,
            Desc = cont.Desc,
            NbHours = cont.NbHours,
            LessonId = lesson.Id,
            DsplSeq = cont.DsplSeq
          };
          _context.Add (content);
        }
      }

      if (await _repo.SaveAll ())
        return Ok ();

      return BadRequest ("impossible de faire l'ajout");
    }

    [HttpGet ("ClassLevelCourseThemes/{classlevelId}/{courseId}")]
    public async Task<IActionResult> ClassLevelCourseThemes (int classlevelId, int courseId) {
      var themes = await _repo.ClassLevelCourseThemes (classlevelId, courseId);
      if (themes.Count () > 0)
        return Ok (new { type = "byTheme", themes = themes });

      var lessons = await _repo.ClassLevelCourseLessons (classlevelId, courseId);
      if (lessons.Count () > 0)
        return Ok (new { type = "byLesson", lessons = lessons });
      else
        return Ok ();
    }

    [HttpGet ("Periods")]
    public async Task<IActionResult> GetPeriods () {
      var periods = await _context.Periods.OrderBy (o => o.Name).ToListAsync ();
      return Ok (periods);
    }

    [HttpGet ("ClassEvents")]
    public async Task<IActionResult> GetClassEvents()
    {
      _context.ProductTypes.FromSql("");
      var events = await _context.ClassEvents.OrderByDescending(e => e.Name).ToListAsync();
      return Ok(events);
    }

    [HttpPost ("courseShowing")]
    public async Task<IActionResult> courseShowing ([FromForm] CourseShowingDto courseShowingDto) {
      int res = await _repo.CreateLessonDoc (courseShowingDto);
      if (res > 0) {
        // envoi du lien aux élèves et au aux parents d'eleves
        await _repo.SendCourseShowingLink (res);
        return Ok ();
      }

      return BadRequest ();
    }

    [HttpPost ("WeekAbsences")]
    public async Task<IActionResult> GetWeekAbsences (WeeklyAbsenceDto data) {
      var fromDate = data.StartDate;
      var dateDay = (int) fromDate.DayOfWeek;
      int move = data.MoveDays;
      var startDate = fromDate.AddDays (move);
      var endDate = startDate.AddDays (5);

      AdminAbsenceDto absences = new AdminAbsenceDto ();
      absences.StartDate = startDate;
      absences.strStartDate = startDate.ToString ("ddd dd MMM", frC);
      absences.EndDate = endDate.ToString ("ddd dd MMM", frC);
      absences.LngStartDate = startDate.ToString ("dddd dd MMMM yyyy", frC);
      absences.LngEndDate = endDate.ToString ("dddd dd MMMM yyyy", frC);

      absences.Days = new List<AbsenceDayDto> ();
      for (int i = 0; i < 5; i++) {
        var day = startDate.AddDays (i);
        AbsenceDayDto add = new AbsenceDayDto ();
        add.Day = i + 1;
        add.strDay = day.ToString ("ddd dd MMM yyyy");
        add.strDayLong = day.ToString ("dddd dd MMMM yyyy", frC);

        var allAbsences = await _context.Absences.ToListAsync ();

        add.Classes = new List<AbsenceClassDto> ();
        var absencesByDate = await _repo.GetAbsencesByDate (day);
        var classes = absencesByDate.Select (a => a.User.Class).Distinct ();
        foreach (var aclass in classes) {
          AbsenceClassDto acd = new AbsenceClassDto ();
          acd.ClassId = aclass.Id;
          acd.ClassName = aclass.Name;
          acd.Children = new List<AbsenceChildDto> ();
          var childrenFromDB = absencesByDate.Where (a => a.User.ClassId == aclass.Id).Select (u => u.User);
          var children = _mapper.Map<List<UserForDetailedDto>> (childrenFromDB);
          int nbAbs = 0;
          int nbLates = 0;
          foreach (var child in children) {
            AbsenceChildDto childDto = new AbsenceChildDto ();
            childDto.Id = child.Id;
            childDto.LastName = child.LastName;
            childDto.FirstName = child.FirstName;
            childDto.PhotoUrl = child.PhotoUrl;
            childDto.TotalAbsences = allAbsences.Where (a => a.UserId == child.Id && a.AbsenceTypeId == absenceTypeId).Count ();
            childDto.TotalLates = allAbsences.Where (a => a.UserId == child.Id && a.AbsenceTypeId == lateTypeId).Count ();
            var childAbsences = absencesByDate.Where (a => a.UserId == child.Id).ToList ();
            nbAbs += childAbsences.Where (c => c.AbsenceTypeId == absenceTypeId).Count ();
            nbLates += childAbsences.Where (c => c.AbsenceTypeId == lateTypeId).Count ();
            childDto.Absences = new List<AbsenceChildDetailsDto> ();
            foreach (var abs in childAbsences) {
              AbsenceChildDetailsDto childAbs = new AbsenceChildDetailsDto ();
              childAbs.CourseName = abs.Session.Course.Name;
              childAbs.CourseAbbrev = abs.Session.Course.Abbreviation;
              childAbs.DoneBy = abs.DoneBy.LastName + ' ' + abs.DoneBy.FirstName;
              childAbs.AbsenceTypeId = abs.AbsenceTypeId;
              childAbs.AbsenceType = abs.AbsenceType.Name.ToLower ();
              childAbs.StartTime = abs.StartDate.ToShortTimeString ();
              childAbs.EndTime = abs.EndDate.ToShortTimeString ();
              TimeSpan diff = abs.EndDate.Subtract (abs.StartDate);
              childAbs.LateMins = diff.TotalMinutes;
              childAbs.Justified = abs.Justified;
              childAbs.Reason = abs.Reason;
              childAbs.Comment = abs.Comment;
              childDto.Absences.Add (childAbs);
            }

            acd.Children.Add (childDto);
          }

          acd.TotalAbs = nbAbs;
          acd.TotalLates = nbLates;
          add.Classes.Add (acd);
        }

        absences.Days.Add (add);
      }

      return Ok (absences);
    }

    [HttpGet ("UserClassLife/{childId}")]
    public async Task<IActionResult> GetChildClassLife (int childId) {
      var absencesFromDB = await _context.Absences.Where (a => a.UserId == childId).ToListAsync ();
      int nbAbs = absencesFromDB.Where (a => a.AbsenceTypeId == absenceTypeId).Count ();
      int nbLates = absencesFromDB.Where (a => a.AbsenceTypeId == lateTypeId).Count ();

      var nbRewards = await _context.UserRewards.Where (r => r.UserId == childId).CountAsync ();
      var nbSanctions = await _context.UserSanctions.Where (r => r.UserId == childId).CountAsync ();

      ChildClassLifeDto userLife = new ChildClassLifeDto ();
      userLife.Id = childId;
      userLife.TotalAbsences = nbAbs;
      userLife.TotalLates = nbLates;
      userLife.TotalRewards = nbRewards;
      userLife.TotalSanctions = nbSanctions;

      return Ok (userLife);
    }

    [HttpGet ("CurrentWeekAbsences")]
    public async Task<IActionResult> GetAbsences () {
      DateTime today = DateTime.Now.Date;
      var dayInt = (int) today.DayOfWeek == 0 ? 7 : (int) today.DayOfWeek;
      var monday = today.AddDays (1 - dayInt);
      var saturday = monday.AddDays (5);

      AdminAbsenceDto absences = new AdminAbsenceDto ();
      absences.StartDate = monday;
      absences.strStartDate = monday.ToString ("ddd dd MMM", frC);
      absences.EndDate = saturday.ToString ("ddd dd MMM", frC);
      absences.LngStartDate = monday.ToString ("dddd dd MMMM yyyy", frC);
      absences.LngEndDate = saturday.ToString ("dddd dd MMMM yyyy", frC);

      absences.Days = new List<AbsenceDayDto> ();
      for (int i = 0; i < 5; i++) {
        var day = monday.AddDays (i);
        AbsenceDayDto add = new AbsenceDayDto ();
        add.Day = i + 1;
        add.strDay = day.ToString ("ddd dd MMM yyyy");
        add.strDayLong = day.ToString ("dddd dd MMMM yyyy", frC);

        var allAbsences = await _context.Absences.ToListAsync ();

        add.Classes = new List<AbsenceClassDto> ();
        var absencesByDate = await _repo.GetAbsencesByDate (day);
        var classes = absencesByDate.Select (a => a.User.Class).Distinct ();
        foreach (var aclass in classes) {
          AbsenceClassDto acd = new AbsenceClassDto ();
          acd.ClassId = aclass.Id;
          acd.ClassName = aclass.Name;
          acd.Children = new List<AbsenceChildDto> ();
          var childrenFromDB = absencesByDate.Where (a => a.User.ClassId == aclass.Id).Select (u => u.User);
          var children = _mapper.Map<List<UserForDetailedDto>> (childrenFromDB);
          int nbAbs = 0;
          int nbLates = 0;
          foreach (var child in children) {
            AbsenceChildDto childDto = new AbsenceChildDto ();
            childDto.Id = child.Id;
            childDto.LastName = child.LastName;
            childDto.FirstName = child.FirstName;
            childDto.PhotoUrl = child.PhotoUrl;
            childDto.TotalAbsences = allAbsences.Where (a => a.UserId == child.Id && a.AbsenceTypeId == absenceTypeId).Count ();
            childDto.TotalLates = allAbsences.Where (a => a.UserId == child.Id && a.AbsenceTypeId == lateTypeId).Count ();
            var childAbsences = absencesByDate.Where (a => a.UserId == child.Id).ToList ();
            nbAbs += childAbsences.Where (c => c.AbsenceTypeId == absenceTypeId).Count ();
            nbLates += childAbsences.Where (c => c.AbsenceTypeId == lateTypeId).Count ();
            childDto.Absences = new List<AbsenceChildDetailsDto> ();
            foreach (var abs in childAbsences) {
              AbsenceChildDetailsDto childAbs = new AbsenceChildDetailsDto ();
              childAbs.CourseName = abs.Session.Course.Name;
              childAbs.CourseAbbrev = abs.Session.Course.Abbreviation;
              childAbs.DoneBy = abs.DoneBy.LastName + ' ' + abs.DoneBy.FirstName;
              childAbs.AbsenceTypeId = abs.AbsenceTypeId;
              childAbs.AbsenceType = abs.AbsenceType.Name.ToLower ();
              childAbs.StartTime = abs.StartDate.ToShortTimeString ();
              childAbs.EndTime = abs.EndDate.ToShortTimeString ();
              TimeSpan diff = abs.EndDate.Subtract (abs.StartDate);
              childAbs.LateMins = diff.TotalMinutes;
              childAbs.Justified = abs.Justified;
              childAbs.Reason = abs.Reason;
              childAbs.Comment = abs.Comment;
              childDto.Absences.Add (childAbs);
            }

            acd.Children.Add (childDto);
          }

          acd.TotalAbs = nbAbs;
          acd.TotalLates = nbLates;
          add.Classes.Add (acd);
        }

        absences.Days.Add (add);
      }

      return Ok (absences);
    }

    [HttpPost ("DayAbsences")]
    public async Task<IActionResult> GetDayAbsences (DateTime day) {
      var absences = new AbsenceDayDto ();
      absences.strDay = day.ToString ("ddd dd MMM");
      absences.strDayShort = day.ToString ("dd/MM/yyyy", frC);
      absences.strDayLong = day.ToString ("dddd dd MMMM yyyy", frC);

      var allAbsences = await _context.Absences.ToListAsync ();

      absences.Classes = new List<AbsenceClassDto> ();
      var absencesByDate = await _repo.GetAbsencesByDate (day);
      var classes = absencesByDate.Select (a => a.User.Class).Distinct ();
      foreach (var aclass in classes) {
        AbsenceClassDto acd = new AbsenceClassDto ();
        acd.ClassId = aclass.Id;
        acd.ClassName = aclass.Name;
        acd.Children = new List<AbsenceChildDto> ();
        var childrenFromDB = absencesByDate.Where (a => a.User.ClassId == aclass.Id).Select (u => u.User);
        var children = _mapper.Map<List<UserForDetailedDto>> (childrenFromDB);
        int nbAbs = 0;
        int nbLates = 0;
        foreach (var child in children) {
          AbsenceChildDto childDto = new AbsenceChildDto ();
          childDto.Id = child.Id;
          childDto.LastName = child.LastName;
          childDto.FirstName = child.FirstName;
          childDto.PhotoUrl = child.PhotoUrl;
          childDto.TotalAbsences = allAbsences.Where (a => a.UserId == child.Id && a.AbsenceTypeId == absenceTypeId).Count ();
          childDto.TotalLates = allAbsences.Where (a => a.UserId == child.Id && a.AbsenceTypeId == lateTypeId).Count ();
          var childAbsences = absencesByDate.Where (a => a.UserId == child.Id).ToList ();
          nbAbs += childAbsences.Where (c => c.AbsenceTypeId == absenceTypeId).Count ();
          nbLates += childAbsences.Where (c => c.AbsenceTypeId == lateTypeId).Count ();
          childDto.Absences = new List<AbsenceChildDetailsDto> ();
          foreach (var abs in childAbsences) {
            AbsenceChildDetailsDto childAbs = new AbsenceChildDetailsDto ();
            childAbs.CourseName = abs.Session.Course.Name;
            childAbs.CourseAbbrev = abs.Session.Course.Abbreviation;
            childAbs.DoneBy = abs.DoneBy.LastName + ' ' + abs.DoneBy.FirstName;
            childAbs.AbsenceTypeId = abs.AbsenceTypeId;
            childAbs.AbsenceType = abs.AbsenceType.Name.ToLower ();
            childAbs.StartTime = abs.StartDate.ToShortTimeString ();
            childAbs.EndTime = abs.EndDate.ToShortTimeString ();
            TimeSpan diff = abs.EndDate.Subtract (abs.StartDate);
            childAbs.LateMins = diff.TotalMinutes;
            childAbs.Justified = abs.Justified;
            childAbs.Reason = abs.Reason;
            childAbs.Comment = abs.Comment;
            childDto.Absences.Add (childAbs);
          }

          acd.Children.Add (childDto);
        }

        acd.TotalAbs = nbAbs;
        acd.TotalLates = nbLates;
        absences.Classes.Add (acd);
      }

      return Ok (absences);
    }

    [HttpGet("CourseTypes")]
    public async Task<IActionResult> GetCourseTypes()
    {
      List<CourseType> courseTypes = await _cache.GetCourseTypes();
      return Ok(courseTypes.OrderBy(o => o.Name));
    }

    [HttpPost("saveCourseTypes")]
    public async Task<IActionResult> SaveCourseTypes(List<CourseTypeForSaveDto> courseTypes)
    {
      List<CourseType> courseTypesCached = await _cache.GetCourseTypes();
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      using(var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          foreach(var item in courseTypes)
          {
            if(item.Id != 0)
            {
              CourseType type = courseTypesCached.First(a => a.Id == item.Id);
              if(item.ToBeDeleted)
              {
                _repo.Delete(type);
              }
              else
              {
                type.Name = item.Name;
                type.UpdateDate = DateTime.Now;
                type.UpdateUserId = currentUserId;
                Guid GUID = Guid.NewGuid();
                type.Version = GUID.ToString();
                _repo.Update(type);
              }
            }
            else
            {
              CourseType type = new CourseType();
              type.Name = item.Name;
              type.InsertDate = DateTime.Now;
              type.InsertUserId = currentUserId;
              type.UpdateDate = DateTime.Now;
              type.UpdateUserId = currentUserId;
              Guid GUID = Guid.NewGuid();
              type.Version = GUID.ToString();
              _repo.Add(type);
            }
          }

          await _repo.SaveAll();
          identityContextTransaction.Commit();
          await _cache.LoadCourseTypes();
          return Ok();
        }
        catch
        {
          identityContextTransaction.Rollback();
          return BadRequest("problème pour enregistrer les types de cours");
        }
      }
    }
  }
}