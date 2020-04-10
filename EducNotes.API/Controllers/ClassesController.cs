using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Helpers;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EducNotes.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ClassesController : ControllerBase
  {
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IEducNotesRepository _repo;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    private readonly IConfiguration _config;
    CultureInfo frC = new CultureInfo("fr-FR");

    public ClassesController(DataContext context, IMapper mapper, IEducNotesRepository repo, IConfiguration config)
    {
        _config = config;
        _context = context;
        _mapper = mapper;
        _repo = repo;
        teacherTypeId = _config.GetValue<int>("AppSettings:teacherTypeId");
        parentTypeId = _config.GetValue<int>("AppSettings:parentTypeId");
        adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
        studentTypeId = _config.GetValue<int>("AppSettings:studentTypeId");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClass(int Id)
    {
        var theclass = await _context.Classes.FirstOrDefaultAsync(c => c.Id == Id);

        return Ok(theclass);
    }

    [HttpGet("{classId}/schedule/today")]
    public async Task<IActionResult> GetScheduleToday(int classId)
    {
        // monday=1, tue=2, ...
        var today = ((int)DateTime.Now.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;

        //if saturday or sunday goes to monday schedule
        if (today == 6 || today == 7)
            today = 1;

        return Ok(await _repo.GetScheduleDay(classId, today));
    }

    [HttpGet("{classId}/schedule/{day}")]
    public async Task<IActionResult> GetScheduleDay(int classId, int day)
    {
        var theclass = await GetClass(classId);

        if (theclass == null)
            return BadRequest("la classe n'existe pas");

        if (day < 1 || day > 7)
            return BadRequest("le jour de la semaine est incorrect.");

        var courses = await _context.Schedules
            .Include(i => i.Class).ThenInclude(i => i.ClassLevel)
            .Include(i => i.Course)
            .Where(d => d.Day == day && d.ClassId == classId)
            .OrderBy(s => s.StartHourMin).ToListAsync();

        if (courses.Count == 0)
            return Unauthorized();

        var coursesToReturn = _mapper.Map<IEnumerable<ScheduleForTimeTableDto>>(courses);

        return Ok(coursesToReturn);
    }

    [HttpGet("{classId}/schedule")]
    public async Task<IActionResult> GetClassSchedule(int classId)
    {
        DateTime today = DateTime.Now.Date;
        var dayInt = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
        var monday = today.AddDays(1 - dayInt);
        var sunday = monday.AddDays(6);

        var itemsFromRepo = await _repo.GetClassSchedule(classId);
        var itemsToReturn = _mapper.Map<IEnumerable<ScheduleForTimeTableDto>>(itemsFromRepo);

        var days = new List<string>();
        for (int i = 0; i <= 6; i++)
        {
          DateTime dt = monday.AddDays(i);
          var shortdate = dt.ToString("ddd dd MMM", frC);
          days.Add(shortdate);
        }

        if (itemsToReturn != null)
        {
            return Ok(new
            {
              scheduleItems = itemsToReturn,
              firstDayWeek = monday,
              strMonday = monday.ToLongDateString(),
              strSunday = sunday.ToLongDateString(),
              weekDays = days
            });
        }

        return BadRequest("Aucun emploi du temps trouvé");
    }

    [HttpPut("DelCourseFromSchedule/{scheduleId}")]
    public async Task<IActionResult> DeleteScheduleItem(int scheduleId)
    {
      var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == scheduleId);
      if(schedule != null)
      {
        _repo.Delete(schedule);
        if(await _repo.SaveAll())
          return Ok();
      }

      return BadRequest("problème pour supprimer le cours de l'emploi du temps");
    }

    [HttpGet("{classId}/getClassScheduleMovedWeek")]
    public async Task<IActionResult> getClassScheduleMovedWeek(int classId, [FromQuery] ScheduleParams agendaParams)
    {
        var FromDate = agendaParams.DueDate.Date;
        var move = agendaParams.MoveWeek;
        var date = FromDate.AddDays(move);
        var dateDay = (int)date.DayOfWeek;

        var dayInt = dateDay == 0 ? 7 : dateDay;
        DateTime monday = date.AddDays(1 - dayInt);
        var sunday = monday.AddDays(6);

        var itemsFromRepo = await _repo.GetClassSchedule(classId);

        var days = new List<string>();
        for (int i = 0; i <= 6; i++)
        {
            DateTime dt = monday.AddDays(i);
            CultureInfo frC = new CultureInfo("fr-FR");
            var shortdate = dt.ToString("ddd dd MMM", frC);
            days.Add(shortdate);
        }

        if (itemsFromRepo != null)
        {
            return Ok(new
            {
                agendaItems = itemsFromRepo,
                firstDayWeek = monday,
                strMonday = monday.ToLongDateString(),
                strSunday = sunday.ToLongDateString(),
                weekDays = days
            });
        }

        return BadRequest("Aucun emploi du temps trouvé");
    }

    [HttpGet("{classId}/teachers")]
    public async Task<IActionResult> GetClassTeachers(int classId)
    {
        var teachers = await _context.ClassCourses
                              .Include(i => i.Teacher)
                              .Where(t => t.ClassId == classId && t.Teacher != null)
                              .Select(t => t.Teacher).Distinct().ToListAsync();

        var teachersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(teachers);

        return Ok(teachersToReturn);
    }

    [HttpGet("{classId}/CourseWithTeacher")]
    public async Task<IActionResult> GetCourseWithTeacher(int classId)
    {
        var teachersData = await _context.ClassCourses
                            .Include(i => i.Teacher).ThenInclude(i => i.Photos)
                            .Include(i => i.Course)
                            .Where(u => u.ClassId == classId)
                            .Distinct().ToListAsync();

        List<TeacherForListDto> coursesWithTeacher = new List<TeacherForListDto>();
        foreach (var data in teachersData)
        {
            TeacherForListDto teacherCourse = new TeacherForListDto();
            teacherCourse.Id = Convert.ToInt32(data.TeacherId);
            teacherCourse.LastName = data.Teacher.LastName;
            teacherCourse.FirstName = data.Teacher.FirstName;
            teacherCourse.Email = data.Teacher.Email;
            teacherCourse.PhotoUrl = ""; //data.Teacher.Photos.FirstOrDefault(p => p.IsMain).Url;
            teacherCourse.PhoneNumber = data.Teacher.PhoneNumber;
            teacherCourse.DateOfBirth = data.Teacher.DateOfBirth.ToString("dd/MM/yyyy", frC);
            teacherCourse.SecondPhoneNumber = data.Teacher.SecondPhoneNumber;
            teacherCourse.Course = data.Course;
            coursesWithTeacher.Add(teacherCourse);
        }

        return Ok(coursesWithTeacher);
    }

    [HttpGet("{classId}/Students")]
    public async Task<IActionResult> GetClassStudents(int classId)
    {
        var studentsFromRepo = await _repo.GetClassStudents(classId);
        var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(studentsFromRepo);

        return Ok(usersToReturn);
    }

    [HttpGet("Agenda/{agendaId}/SetTask/{value}")]
    public async Task<IActionResult> AgendaSetTask(int agendaId, bool value)
    {
        Agenda agenda = await _context.Agendas.FirstOrDefaultAsync(a => a.Id == agendaId);

        if (agenda != null)
        {
            agenda.Done = value;
            //set the current user as the modifier of done status
            agenda.DoneSetById = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _repo.Update(agenda);
            if (await _repo.SaveAll())
                return NoContent();
            else
                return BadRequest("problème de saisie de données");
        }
        else
        {
            return BadRequest("problème de données. voir l'administration si le problème persiste");
        }
    }

    [HttpGet("{classId}/MovedWeekAgenda")]
    public async Task<IActionResult> getClassMovedWeekAgenda(int classId, [FromQuery] AgendaParams agendaParams)
    {
        var FromDate = agendaParams.DueDate.Date;
        var move = agendaParams.MoveWeek;
        var date = FromDate.AddDays(move);
        var dateDay = (int)date.DayOfWeek;

        var dayInt = dateDay == 0 ? 7 : dateDay;
        DateTime monday = date.AddDays(1 - dayInt);
        var saturday = monday.AddDays(5);

        var itemsFromRepo = await _repo.GetClassAgenda(classId, monday, saturday);
        var items = _repo.GetAgendaListByDueDate(itemsFromRepo);

        var courses = await _context.ClassCourses
            .Where(c => c.ClassId == classId)
            .Select(s => s.Course).ToListAsync();

        List<CourseTasksDto> coursesWithTasks = new List<CourseTasksDto>();
        foreach (var course in courses)
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

        var days = new List<string>();
        var nbTasks = new List<int>();
        for (int i = 0; i <= 5; i++)
        {
            DateTime dt = monday.AddDays(i);
            CultureInfo frC = new CultureInfo("fr-FR");
            var shortdate = dt.ToString("ddd dd MMM", frC);
            days.Add(shortdate);

            var item = items.FirstOrDefault(a => a.dtDueDate == dt);
            if (item != null)
                nbTasks.Add(item.NbTasks);
            else
                nbTasks.Add(0);
        }

        if (itemsFromRepo != null)
        {
            return Ok(new
            {
                agendaItems = items,
                firstDayWeek = monday,
                strMonday = monday.ToLongDateString(),
                strSaturday = saturday.ToLongDateString(),
                weekDays = days,
                nbDayTasks = nbTasks,
                coursesWithTasks
            });
        }

        return BadRequest("Aucun agenda trouvé");
    }

    [HttpGet("{classId}/TodayToNDaysAgenda/{toNbDays}")]
    public async Task<IActionResult> getTodayToNDaysAgenda(int classId, int toNbDays)
    {
        var agendasFromRepo = await _repo.GetClassAgendaTodayToNDays(classId, toNbDays);
        var items = _repo.GetAgendaListByDueDate(agendasFromRepo);

        var days = new List<string>();
        var nbTasks = new List<int>();

        var today = DateTime.Now.Date;

        for (int i = 0; i < toNbDays; i++)
        {
            DateTime dt = today.AddDays(i);
            CultureInfo frC = new CultureInfo("fr-FR");
            var shortdate = dt.ToString("ddd dd MMM", frC);
            days.Add(shortdate);

            var item = items.FirstOrDefault(a => a.dtDueDate == dt);
            if (item != null)
                nbTasks.Add(item.NbTasks);
            else
                nbTasks.Add(0);
        }

        var lastDay = today.AddDays(toNbDays);

        if (agendasFromRepo != null)
        {
          return Ok(new
          {
            agendaItems = items,
            firstDay = today,
            strFirstDay = today.ToLongDateString(),
            strLastDayay = lastDay.ToLongDateString(),
            weekDays = days,
            nbDayTasks = nbTasks
          });
        }

        return BadRequest("Aucun agenda trouvé");
    }

    [HttpGet("{classId}/CurrWeekAgenda")]
    public async Task<IActionResult> getClassCurrWeekAgenda(int classId)
    {
        DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        var dayInt = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
        var monday = today.AddDays(1 - dayInt);
        var saturday = monday.AddDays(5);

        var itemsFromRepo = await _repo.GetClassAgenda(classId, monday, saturday);
        var items = _repo.GetAgendaListByDueDate(itemsFromRepo);

        var courses = await _context.ClassCourses
                            .Where(c => c.ClassId == classId)
                            .Select(s => s.Course).ToListAsync();

        List<CourseTasksDto> coursesWithTasks = new List<CourseTasksDto>();
        foreach (var course in courses)
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

        var days = new List<string>();
        var nbTasks = new List<int>();
        for (int i = 0; i <= 5; i++)
        {
          DateTime dt = monday.AddDays(i);
          var shortdate = dt.ToString("ddd dd MMM", frC);
          days.Add(shortdate);

          var item = items.FirstOrDefault(a => a.dtDueDate == dt);
          if (item != null)
            nbTasks.Add(item.NbTasks);
          else
            nbTasks.Add(0);
        }

        if(itemsFromRepo != null)
        {
          return Ok(new
          {
            agendaItems = items,
            firstDayWeek = monday,
            strMonday = monday.ToLongDateString(),
            strSaturday = saturday.ToLongDateString(),
            weekDays = days,
            nbDayTasks = nbTasks,
            coursesWithTasks
          });
        }

        return BadRequest("Aucun agenda trouvé");
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

    [HttpGet("GetWeekDays")]
    public IActionResult GetWeekDaysByDate([FromQuery] AgendaParams agendaParams)
    {
        var date = agendaParams.DueDate;

        var weekDays = _repo.GetWeekDays(date);

        return Ok(weekDays);
    }

    [HttpGet("GetSanctions")]
    public async Task<IActionResult> GetSanctions()
    {
      var sanctions = await _context.Sanctions.OrderBy(o => o.Name).ToListAsync();
      return Ok(sanctions);
    }

    [HttpGet("{classId}/events")]
    public async Task<IActionResult> GetEvents(int classId)
    {
      var absences = await _context.Absences
                            .Include(i => i.User).ThenInclude(p => p.Photos)
                            .Include(i => i.AbsenceType)
                            .Include(i => i.DoneBy)
                            .Where(a => a.User.ClassId == classId)
                            .OrderByDescending(o => o.StartDate).ToListAsync();

      List<UserClassEventForListDto> events = new List<UserClassEventForListDto>();

      foreach (var abs in absences)
      {
        //set the date data to be shown in reporting
        string dateData = "";
        if(abs.StartDate.Date == abs.EndDate.Date)
        {
          dateData = abs.StartDate.ToString("dd/MM/yy", frC) + " de " + 
            abs.StartDate.ToString("HH:mm", frC) + " à " + abs.EndDate.ToString("HH:mm", frC);
        }
        else
        {
          dateData = "de " + abs.StartDate.ToString("dd/MM/yy", frC) + " " + abs.StartDate.ToString("HH:mm", frC) +
            " au " + abs.EndDate.ToString("dd/MM/yy", frC) + " " + abs.EndDate.ToString("HH:mm", frC);
        }

        UserClassEventForListDto userEvent = new UserClassEventForListDto();
        userEvent.Id = abs.Id;
        userEvent.UserId = abs.UserId;
        userEvent.UserName = abs.User.LastName + " " + abs.User.FirstName;
        if(abs.User.Photos.Count() > 0)
          userEvent.PhotoUrl = abs.User.Photos.FirstOrDefault(p => p.IsMain).Url;
        userEvent.ClassEventName = "absence";
        userEvent.ClassEventTypeId = abs.AbsenceTypeId;
        userEvent.ClassEventType = abs.AbsenceType.Name;
        userEvent.DoneByName = abs.DoneBy.LastName + " " + abs.DoneBy.FirstName;
        userEvent.StartDate = abs.StartDate;
        userEvent.strStartDate = dateData;
        userEvent.EndDate = abs.EndDate;
        userEvent.strEndDate = abs.EndDate.ToString("dd/MM/yy", frC);
        userEvent.StartTime = abs.StartDate.ToString("HH:mm", frC);
        userEvent.EndTime = abs.EndDate.ToString("HH:mm", frC);
        userEvent.Justified = abs.Justified == true ? "OUI" : "NON";
        userEvent.Reason = abs.Reason;
        userEvent.Comment = abs.Comment;
        events.Add(userEvent);
      }

      List<ClassEventWithNbDto> eventsWithNb = new List<ClassEventWithNbDto>();
      int absTypeId = Convert.ToInt32(_config.GetSection("AppSettings:AbsenceTypeId").Value);
      int lateTypeId = Convert.ToInt32(_config.GetSection("AppSettings:LateTypeId").Value);
      int nbAbs = events.Where(e => e.ClassEventTypeId == absTypeId).Count();
      int nbLate = events.Where(e => e.ClassEventTypeId == lateTypeId).Count();
      eventsWithNb.Add(new ClassEventWithNbDto {Id = 0, Name = "absence", NbTimes = nbAbs});
      eventsWithNb.Add(new ClassEventWithNbDto {Id = 0, Name = "retard", NbTimes = nbLate});

      var otherEvents = await _context.UserClassEvents
                        .Include(i => i.User).ThenInclude(p => p.Photos)
                        .Include(i => i.ClassEvent)
                        .Include(i => i.DoneBy)
                        .Where(a => a.User.ClassId == classId)
                        .OrderByDescending(o => o.StartDate).ToListAsync();

      foreach (var oe in otherEvents)
      {
        UserClassEventForListDto otherEvent = new UserClassEventForListDto();
        otherEvent.Id = oe.Id;
        otherEvent.UserId = oe.UserId;
        otherEvent.UserName = oe.User.LastName + " " + oe.User.FirstName;
        if(oe.User.Photos.Count() > 0)
          otherEvent.PhotoUrl = oe.User.Photos.FirstOrDefault(p => p.IsMain).Url;
        otherEvent.ClassEventName = oe.ClassEvent.Name;
        otherEvent.ClassEventType = oe.ClassEvent.Name;
        otherEvent.DoneByName = oe.DoneBy.LastName + " " + oe.DoneBy.FirstName;
        otherEvent.StartDate = oe.StartDate;
        otherEvent.strStartDate = oe.StartDate.ToString("dd/MM/yy", frC);
        otherEvent.EndDate = oe.EndDate;
        otherEvent.strEndDate = oe.StartDate.ToString("dd/MM/yy", frC);
        otherEvent.StartTime = oe.StartDate.ToString("HH:mm", frC);
        otherEvent.EndTime = oe.EndDate.ToString("HH:mm", frC);
        otherEvent.Justified =  oe.Justified == true ? "OUI" : "NON";
        otherEvent.Reason = oe.Reason;
        otherEvent.Comment = oe.Comment;
        events.Add(otherEvent);
      }

      var classEvents = await _context.ClassEvents.OrderBy(e => e.Name).ToListAsync();
      foreach (var ce in classEvents)
      {
        int nb = otherEvents.Where(e => e.ClassEventId == ce.Id).ToList().Count();
        eventsWithNb.Add(new ClassEventWithNbDto {Id = ce.Id, Name = ce.Name, NbTimes = nb});
      }

      events = events.OrderByDescending(e => e.StartDate).ToList();
      eventsWithNb = eventsWithNb.OrderBy(e => e.Name).ToList();
      return Ok(new {
        events,
        eventsWithNb
      });
    }

    [HttpGet("{classid}/Absences")]
    public async Task<IActionResult> GetClassAbsences(int classId)
    {
      var studentType = Convert.ToInt32(_config.GetSection("AppSettings:StudentTypeId").Value);
      var absences = await _context.Absences
                            .Include(i => i.User)
                            .Where(a => a.User.ClassId == classId && a.User.UserTypeId == studentType)
                            .OrderByDescending(o => o.StartDate).ToListAsync();

      var nbClassAbscences = absences.Count();
      var absencesToReturn = _mapper.Map<IEnumerable<AbsencesToReturnDto>>(absences);
      return Ok(new
      {
        absences = absencesToReturn,
        nbAbsences = nbClassAbscences
      });
    }

    [HttpGet("{classId}/ClassSanctions")]
    public async Task<IActionResult> GetClassSanctions(int classId)
    {
      var studentType = Convert.ToInt32(_config.GetSection("AppSettings:StudentTypeId").Value);
      var sanctions = await _context.UserSanctions
                            .Include(i => i.Sanction)
                            .Include(i => i.User)
                            .Include(i => i.SanctionedBy)
                            .Where(a => a.User.ClassId == classId && a.User.UserTypeId == studentType)
                            .OrderByDescending(a => a.SanctionDate).ToListAsync();

      var nbClassSanctions = sanctions.Count();
      var sanctionsToReturn = _mapper.Map<IEnumerable<UserSanctionsToReturnDto>>(sanctions);
      return Ok(new
      {
        sanctions = sanctionsToReturn,
        nbSanctions = nbClassSanctions
      });
    }

    [HttpGet("{classId}/GetClassAgenda")]
    public async Task<IActionResult> GetClassAgenda(int classId, DateTime StartDate, DateTime EndDate)
    {
        var agendaItems = await _repo.GetClassAgenda(classId, StartDate, EndDate);

        if (agendaItems != null)
            return Ok(_repo.GetAgendaListByDueDate(agendaItems));

        return BadRequest("Aucun agenda trouvé");
    }

    [HttpGet("{claissId}/agendas/{agendaId}")]
    public async Task<IActionResult> GetAgendaById(int agendaId)
    {
        var agenda = await _repo.GetAgenda(agendaId);

        return Ok(agenda);
    }

    [HttpGet("GetAgendaItem")]
    public async Task<IActionResult> GetAgendaItem([FromQuery] AgendaParams agendaParams)
    {
        var classId = agendaParams.ClassId;
        var courseId = agendaParams.CourseId;
        var dueDate = agendaParams.DueDate;

        var agendaItem = await _context.Agendas.FirstOrDefaultAsync(i => i.Session.ClassId == classId &&
            i.Session.CourseId == courseId && i.Session.SessionDate == dueDate);

        //return Ok(agendaItem);
        if (agendaItem != null)
        {
            return Ok(agendaItem);
        }
        else
        {
            Agenda emptyAgendaItem = new Agenda();
            return Ok(emptyAgendaItem);
        }

    }

    [HttpPut("saveSchedules")]
    public async Task<IActionResult> saveSchedules([FromBody] Schedule[] schedules)
    {
        foreach (var sch in schedules)
        {
            _repo.Add(sch);
        }

        if (await _repo.SaveAll())
            return NoContent();

        return BadRequest("problème d'enregistrement des données");
    }

    [HttpPut("SaveAgenda")]
    public async Task<IActionResult> SaveAgendaItem([FromBody] AgendaForSaveDto agendaForSaveDto)
    {
        var id = agendaForSaveDto.Id;
        if (id == 0)
        {
            Agenda newAgendaItem = new Agenda();
            _mapper.Map(agendaForSaveDto, newAgendaItem);
            newAgendaItem.DateAdded = DateTime.Now;
            newAgendaItem.DoneSetById = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _repo.Add(newAgendaItem);
        }
        else
        {
            var agendaItemFromRepo = await _repo.GetAgenda(id);
            _mapper.Map(agendaForSaveDto, agendaItemFromRepo);
            agendaItemFromRepo.DateAdded = DateTime.Now;
        }

        if (await _repo.SaveAll())
            return NoContent();

        throw new Exception($"Updating/Saving agendaItem failed");
    }

    [HttpGet("GetAllCourses")]
    public async Task<IActionResult> GetAllCourses()
    {
        var courses = await _context.Courses.OrderBy(a => a.Name).ToListAsync();
        return Ok(courses);
    }

    [HttpGet("AllClasses")]
    public async Task<IActionResult> GetAllClasses()
    {
      var les_classes = _context.Classes.Include(e => e.ClassLevel).OrderBy(a => a.Name);
      return Ok(await les_classes.ToListAsync());
    }

    [HttpGet("ClassesByLevel")]
    public async Task<IActionResult> GetClassesByLevel()
    {
      List<ClassByLevelDto> classesByLevel = new List<ClassByLevelDto>();
      var levels = await _context.ClassLevels.OrderBy(c => c.DsplSeq).ToListAsync();
      foreach (var level in levels)
      {
        ClassByLevelDto cbl = new ClassByLevelDto();
        cbl.ClassLevelId = level.Id;
        cbl.LevelName = level.Name;

        cbl.Classes = new List<Class>();
        //var classes = await _context.Classes.Where(c => c.ClassLevelId == level.Id).OrderBy(o => o.Name).ToListAsync();
        var classes = await _repo.GetClassesByLevelId(level.Id);
        foreach (var aclass in classes)
        {
          cbl.Classes.Add(aclass);
        }

        classesByLevel.Add(cbl);
      }

      return Ok(classesByLevel);
    }

    [HttpGet("{classId}/classCourses")]
    public async Task<IActionResult> GetClassCourses(int classId)
    {
        var courses = await _context.ClassCourses
            .Where(c => c.ClassId == classId)
            .Select(s => s.Course).ToListAsync();

        return Ok(courses);
    }

    [HttpGet("{classId}/CoursesWithAgenda/f/{daysToNow}/t/{daysFromNow}")]
    public async Task<IActionResult> GetClassCoursesWithAgenda(int classId, int daysToNow, int daysFromNow)
    {
        var today = DateTime.Now.Date;
        var startDate = today.AddDays(-daysToNow);
        var EndDate = today.AddDays(daysFromNow);

        var courses = await _context.ClassCourses
            .Where(c => c.ClassId == classId)
            .Select(s => s.Course).ToListAsync();

        var classAgenda = await _context.Agendas
            .OrderBy(o => o.Session.SessionDate)
            .Where(a => a.Session.ClassId == classId && a.Session.SessionDate.Date >= startDate && a.Session.SessionDate <= EndDate)
            .ToListAsync();

        List<CourseWithAgendaDto> coursesWithAgenda = new List<CourseWithAgendaDto>();
        foreach (var course in courses)
        {
            CourseWithAgendaDto cwa = new CourseWithAgendaDto();
            cwa.Id = course.Id;
            cwa.Name = course.Name;
            cwa.Abbrev = course.Abbreviation;
            cwa.Color = course.Color;

            var items = classAgenda.Where(a => a.Session.CourseId == course.Id).ToList();
            List<AgendaItemDto> agendaItems = new List<AgendaItemDto>();
            foreach (var item in items)
            {
                AgendaItemDto aid = new AgendaItemDto();

                CultureInfo frC = new CultureInfo("fr-FR");
                var strDateAdded = item.DateAdded.ToString("ddd dd MMM", frC);
                var strDueDate = item.Session.SessionDate.ToString("ddd dd MMM", frC);

                aid.strDateAdded = strDateAdded;
                aid.strDueDate = strDueDate;
                aid.TaskDesc = item.TaskDesc;
                aid.Done = item.Done;
                agendaItems.Add(aid);
            }
            cwa.AgendaItems = agendaItems;
            cwa.NbItems = cwa.AgendaItems.Count();

            coursesWithAgenda.Add(cwa);
        }

        return Ok(coursesWithAgenda.OrderBy(c => c.Name));
    }

    [HttpGet("{classId}/AgendaByDate")]
    public async Task<IActionResult> GetAgendaByDate(int classId, [FromQuery]AgendaParams agendaParams)
    {
        var nbDays = agendaParams.nbDays;
        var IsMovingPeriod = agendaParams.IsMovingPeriod;
        var startDate = agendaParams.CurrentDate.Date;
        var endDate = startDate.AddDays(nbDays).Date;
        if (IsMovingPeriod)
        {
            if (nbDays > 0)
            {
                startDate = agendaParams.CurrentDate.AddDays(1).Date;
                endDate = startDate.AddDays(nbDays).Date;
            }
            else
            {
                startDate = agendaParams.CurrentDate.AddDays(-1).Date;
                endDate = startDate.AddDays(nbDays).Date;
            }
        }

        var classAgenda = new List<Agenda>();
        if (startDate > endDate)
        {
            var temp = startDate;
            startDate = endDate;
            endDate = temp;
        }

        CultureInfo frC = new CultureInfo("fr-FR");
        var strStartDate = startDate.ToString("dd MMM", frC);
        var strEndDate = endDate.ToString("dd MMM", frC);

        List<AgendaForListDto> AgendaList = await _repo.GetUserClassAgenda(classId, startDate, endDate);

        List<bool> dones = new List<bool>();
        foreach (var al in AgendaList)
        {
            foreach (var item in al.AgendaItems)
            {
                dones.Add(item.Done);
            }
        }

        var courses = await _context.ClassCourses
            .Where(c => c.ClassId == classId)
            .Select(s => s.Course).ToListAsync();

        List<CourseTasksDto> coursesWithTasks = new List<CourseTasksDto>();
        foreach (var course in courses)
        {
            CourseTasksDto ctd = new CourseTasksDto();
            var nbItems = classAgenda.Where(a => a.Session.CourseId == course.Id).ToList().Count();
            ctd.CourseId = course.Id;
            ctd.CourseName = course.Name;
            ctd.CourseAbbrev = course.Abbreviation;
            ctd.CourseColor = course.Color;
            ctd.NbTasks = nbItems;
            coursesWithTasks.Add(ctd);
        }

        return Ok(new
        {
            agendaList = AgendaList,
            startDate = startDate,
            endDate = endDate,
            strStartDate = strStartDate,
            strEndDate = strEndDate,
            coursesWithTasks = coursesWithTasks.OrderBy(c => c.CourseName),
            dones = dones
        });
    }

    [HttpPost("UpdateCourse/{courseId}")]
    public async Task<IActionResult> UpdateCourse(int courseId, CourseDto courseDto)
    {
        var courseFromRepo = await _repo.GetCourse(courseId);
        courseFromRepo.Name = courseDto.Name;
        courseFromRepo.Abbreviation = courseDto.Abbreviation;
        courseFromRepo.Color = courseDto.Color;
        _repo.Update(courseFromRepo);
        if (await _repo.SaveAll())
            return NoContent();

        return BadRequest("impossible de mettre à jour ce cours");

    }

    [HttpGet("ClassLevels")]
    public async Task<IActionResult> GetLevels()
    {
      var levels = await _context.ClassLevels.OrderBy(c => c.DsplSeq).ToListAsync();
      return Ok(levels);
    }

    [HttpGet("LevelsWithClasses")]
    public async Task<IActionResult> GetLevelsWithClasses()
    {
        var levels = await _context.ClassLevels
            .Include(c => c.Classes)
            .OrderBy(a => a.DsplSeq)
            .ToListAsync();
        var dataToReturn = new List<ClassLevelDetailDto>();

        foreach (var item in levels)
        {
          var res = new ClassLevelDetailDto();
          res.Id = item.Id;
          res.Name = item.Name;
          res.TotalClasses = item.Classes.Count();
          res.TotalEnrolled = item.Inscriptions.Count();
          res.Classes = new List<ClassDetailDto>();
          foreach (var c in item.Classes)
          {
            res.TotalStudents+=c.Students.Count();
            //add class data
            ClassDetailDto cdd = new ClassDetailDto();
            cdd.Id = c.Id;
            cdd.Name = c.Name;
            cdd.MaxStudent = c.MaxStudent;
            cdd.TotalStudents = c.Students.Count();
            res.Classes.Add(cdd);
          }

          //res.Classes = item.Classes.ToList();
          dataToReturn.Add(res);
        }

        return Ok(dataToReturn);
    }

    [HttpPost("ClassLevelsWithClasses")]
    public async Task<IActionResult> GetClassLevelWithClasses(List<int> CLIds)
    {
        var classLevels = await _context.ClassLevels
            .Where(c => CLIds.Contains(c.Id)).ToListAsync();

        foreach (var cl in classLevels)
        {
            cl.Classes = await _context.Classes
                .OrderBy(c => c.Name)
                .Where(c => c.ClassLevelId == cl.Id).ToListAsync();

            foreach (var aclass in cl.Classes)
            {
                aclass.Students = await _context.Users.Where(u => u.ClassId == aclass.Id).ToListAsync();
            }
        }

        return Ok(classLevels);
    }

    [HttpGet("{classLevelId}/ClassesByLevelId")]
    public async Task<IActionResult> ClassesByLevelId(int classLevelId)
    {
      var classes = await _context.Classes
                          .Include(s => s.Students)
                          .Where(c => c.ClassLevelId == classLevelId)
                          .ToListAsync();

      var classesToReturn = _mapper.Map<IEnumerable<ClassDetailDto>>(classes);
      return Ok(classesToReturn);
    }

    [HttpPost("{classId}/DeleteClass")]
    public async Task<IActionResult> DeleteClass(int classId)
    {
        _repo.Delete(_context.Classes.FirstOrDefault(e => e.Id == classId));
        if (await _repo.SaveAll())
            return Ok(classId);
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
          Abbreviation = courseDto.Abbreviation,
          Color = courseDto.Color
        };
        _repo.Add(course);
      }
      else
      {
        var course = await _context.Courses.FirstAsync(c => c.Id == id);
        course.Name = courseDto.Name;
        course.Abbreviation = courseDto.Abbreviation;
        course.Color = courseDto.Color;
        _repo.Update(course);
      }
      // foreach (var item in courseDto.classLevelIds)
      // {
      //     var classes = await _context.Classes.Where(a=>a.ClassLevelId == Convert.ToInt32(item)).Select(a=>a.Id).ToListAsync();
      //     foreach (var classId in classes)
      //     {
      //       _repo.Add(new ClassCourse {CourseId = course.Id,ClassId = classId});
      //     }
      // }
      if (await _repo.SaveAll())
          return Ok();

      return BadRequest("problème pour ajouter ce cours");
    }

    [HttpGet("SessionData/{sessionId}")]
    public async Task<IActionResult> GetSessionData(int sessionId)
    {
      // get session
      var sessionFromRepo = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
      var session = _mapper.Map<SessionToReturnDto>(sessionFromRepo);

      IEnumerable<AbsenceForCallSheetDto> sessionAbsences = new List<AbsenceForCallSheetDto>();
      if (session != null)
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

    [HttpGet("{classId}/CallSheet/Students")]
    public async Task<IActionResult> GetCallSheetStudents(int classId)
    {
      var studentsFromRepo = await _repo.GetClassStudents(classId);
      var classStudents = _mapper.Map<IEnumerable<UserForCallSheetDto>>(studentsFromRepo);
      return Ok(classStudents);
    }

    // [HttpGet("SessionData/{scheduleId}")]
    // public async Task<IActionResult> GetSessionData(int scheduleId)
    // {
    //   var schedule = await _context.Schedules
    //                         .Include(i => i.Class)
    //                         .Include(i => i.Course)
    //                         .FirstOrDefaultAsync(s => s.Id == scheduleId);

    //   if (schedule == null)
    //     return BadRequest("problème pour créer la session du cours.");

    //   var scheduleDay = schedule.Day;

    //   var today = DateTime.Now.Date;
    //   // monday=1, tue=2, ...
    //   var todayDay = ((int)today.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;

    //   if (todayDay != scheduleDay)
    //     return BadRequest("l'emploi du temps du jour est incohérent.");

    //   // get session by schedule and date
    //   var sessionFromRepo = await _context.Sessions.FirstOrDefaultAsync(s => s.ScheduleId == schedule.Id &&
    //     s.SessionDate.Date == today);
    //   var session = _mapper.Map<SessionForCallSheetDto>(sessionFromRepo);

    //   var studentsFromRepo = await _repo.GetClassStudents(schedule.ClassId);
    //   var classStudents = _mapper.Map<IEnumerable<UserForCallSheetDto>>(studentsFromRepo);

    //   var sessionSchedule = _mapper.Map<ScheduleToReturnDto>(schedule);

    //   IEnumerable<AbsenceForCallSheetDto> sessionAbsences = new List<AbsenceForCallSheetDto>();
    //   if (session != null)
    //   {
    //     var absences = await _context.Absences.Where(a => a.SessionId == session.Id).ToListAsync();
    //     sessionAbsences = _mapper.Map<IEnumerable<AbsenceForCallSheetDto>>(absences);
    //   }

    //   if (session != null)
    //   {
    //     return Ok(new
    //     {
    //       session,
    //       sessionSchedule,
    //       classStudents,
    //       sessionAbsences
    //     });
    //   }
    //   else
    //   {
    //     var newSession = _context.Add(new Session
    //     {
    //       ScheduleId = schedule.Id,
    //       SessionDate = today
    //     });

    //     if (await _repo.SaveAll())
    //     {
    //       return Ok(new
    //       {
    //         session = newSession,
    //         sessionSchedule,
    //         classStudents,
    //         sessionAbsences
    //       });
    //     }

    //     return BadRequest("problème pour récupérer la session");
    //   }
    // }

    [HttpGet("Schedule/{id}")]
    public async Task<IActionResult> GetSchedule(int id)
    {
        var schedule = await _context.Schedules
            .Include(i => i.Class)
            .Include(i => i.Course)
            .FirstOrDefaultAsync(s => s.Id == id); ;
        var scheduleToReturn = _mapper.Map<ScheduleToReturnDto>(schedule);

        return Ok(scheduleToReturn);
    }

    [HttpGet("Sessions/{id}")]
    public async Task<IActionResult> GetSession(int id)
    {
      var sessionFromDB = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id);
      var session = _mapper.Map<SessionToReturnDto>(sessionFromDB);
      return Ok(session);
    }

    // [HttpGet("Schedule/{scheduleId}/Session")]
    // public async Task<IActionResult> GetSessionFromSchedule(int scheduleId)
    // {
    //     var schedule = _context.Schedules.Where(s => s.Id == scheduleId).FirstOrDefault();
    //     if (schedule == null)
    //         return BadRequest("problème pour créer la session du cours.");

    //     var scheduleDay = schedule.Day;

    //     var today = DateTime.Now.Date;
    //     // monday=1, tue=2, ...
    //     var todayDay = ((int)today.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;

    //     if (todayDay != scheduleDay)
    //       return BadRequest("l'emploi du temps du jour est incohérent.");

    //     // get session by schedule and date
    //     var sessionFromDB = await _context.Sessions
    //                         .Include(i => i.Class)
    //                         .Include(i => i.Course)
    //                         .FirstOrDefaultAsync(s => s.ScheduleId == schedule.Id && s.SessionDate.Date == today);
    //     if (sessionFromDB != null)
    //     {
    //       var session = _mapper.Map<SessionToReturnDto>(sessionFromDB);
    //       return Ok(session);
    //     }
    //     else
    //     {
    //       var newSession = _context.Add(new Session
    //       {
    //         ScheduleId = schedule.Id,
    //         TeacherId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
    //         ClassId = schedule.ClassId,
    //         CourseId = schedule.CourseId,
    //         StartHourMin = schedule.StartHourMin,
    //         EndHourMin = schedule.EndHourMin,
    //         SessionDate = today
    //       });

    //       if (await _repo.SaveAll())
    //       {
    //         var session = _mapper.Map<SessionToReturnDto>(newSession);
    //         return Ok(session);
    //       }

    //       return BadRequest("problème pour récupérer la session");
    //     }
    // }

    [HttpGet("courses/{courseId}/teacher/{teacherId}/Program")]
    public async Task<IActionResult> ClassesWithProgram(int courseId, int teacherId)
    {
        // course data
        var course = await _context.Courses.FirstAsync(c => c.Id == courseId);
        //get teacher data (classes & courses)
        List<TeacherClassesDto> teacherClasses = await _repo.GetTeacherClasses(teacherId);

        //for each class get themes and lessons...
        List<ClassWithThemesDto> classesWithProgram = new List<ClassWithThemesDto>();
        foreach (var aclass in teacherClasses)
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
                                        .Where(t => t.ClassLevelId == aclass.ClassLevelId && t.CourseId == courseId)
                                        .ToListAsync();
            foreach (var theme in themesFromDB)
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
                foreach (var lesson in themeLessons)
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
                    var lessonContents = await _context.LessonContents.Where(c => c.LessonId == lesson.Id).ToListAsync();
                    foreach (var content in lessonContents)
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
            cwtd.Lessons = new List<LessonDto>();
            var LessonsFromDB = await _context.Lessons
                                      .Where(t => t.ClassLevelId == aclass.ClassLevelId && t.CourseId == courseId)
                                      .ToListAsync();
            foreach (var lesson in LessonsFromDB)
            {
                LessonDto ld = new LessonDto();
                ld.Id = lesson.Id;
                ld.ThemeId = lesson.ThemeId;
                ld.ClassLevelId = lesson.ClassLevelId;
                ld.CourseId = lesson.CourseId;
                ld.Name = lesson.Name;
                ld.Desc = lesson.Desc;
                cwtd.Lessons.Add(ld);

                ld.Contents = new List<LessonContentDto>();
                var lessonContents = await _context.LessonContents.Where(c => c.LessonId == lesson.Id).ToListAsync();
                foreach (var content in lessonContents)
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
            classesWithProgram.Add(cwtd);
        }

        return Ok(classesWithProgram);
    }

    [HttpPut("SaveCallSheet/{sessionId}")]
    public async Task<IActionResult> SaveCallSheet(int sessionId, [FromBody] Absence[] absences)
    {
      //delete old absents (update: delete + add)
      if (sessionId > 0)
      {
        List<Absence> oldAbsences = await _context.Absences.Where(a => a.SessionId == sessionId).ToListAsync();
        if (oldAbsences.Count() > 0)
          _repo.DeleteAll(oldAbsences);
      }

      // absence Sms data
      int absenceSmsId = _config.GetValue<int>("AppSettings:AbsenceSms");
      int lateSmsId = _config.GetValue<int>("AppSettings:LateSms");
      var AbsenceSms = await _context.SmsTemplates.FirstOrDefaultAsync(s => s.Id == absenceSmsId);
      var LateSms = await _context.SmsTemplates.FirstOrDefaultAsync(s => s.Id == lateSmsId);

      var ids = absences.Select(u => u.UserId);
      var parents = _context.UserLinks.Where(u => ids.Contains(u.UserId)).Distinct().ToList();
      List<AbsenceSmsDto> absSmsData = new List<AbsenceSmsDto>();

      int absTypeId = _config.GetValue<int>("AppSettings:AbsenceTypeId");
      int lateTypeId = _config.GetValue<int>("AppSettings:LateTypeId");

      //set absence sms data
      var sessionFromDB = await _context.Sessions.FirstAsync(s => s.Id == sessionId);
      var session = _mapper.Map<SessionToReturnDto>(sessionFromDB);
      // var scheduleFromDB = await _context.Schedules
      //                         .Include(c => c.Class)
      //                         .Include(c => c.Course)
      //                         .FirstAsync(s => s.Id == session.ScheduleId);
      // var schedule = _mapper.Map<ScheduleToReturnDto>(scheduleFromDB);

      var dateData = session.strSessionDate.Split("/");
      string day = dateData[0];
      string month = dateData[1];
      string year = dateData[2];
      string hourMin = session.EndHourMin;
      string deliveryTime = year + "-" + month + "-" + day + "T" + hourMin + ":00";

      Period currPeriod = await _repo.GetPeriodFromDate(DateTime.Now);

      //add new absents
      for (int i = 0; i < absences.Length; i++)
      {
        Absence absence = absences[i];
        absence.PeriodId = currPeriod.Id;
        _repo.Add(absence);

        int childId = absence.UserId;
        var child = await _context.Users.FirstAsync(u => u.Id == childId);
        List<int> parentIds = parents.Where(p => p.UserId == childId).Select(p => p.UserPId).ToList();
        foreach (var parentId in parentIds)
        {
          //did we already add the sms to DB? if yes remove it (updated one is coming...)
          Sms oldSms = await _context.Sms.FirstOrDefaultAsync(s => s.SessionId == sessionId && s.ToUserId == parentId &&
                              s.StudentId == childId && s.StatusFlag == 0);
          if(oldSms != null)
            _repo.Delete(oldSms);

          // is the parent subscribed to the Absence/Late sms?
          var userTemplate = new UserSmsTemplate();
          if(absence.AbsenceTypeId == absTypeId)
          {
            userTemplate = await _context.UserSmsTemplates.FirstOrDefaultAsync(
                                  u => u.ParentId == parentId && u.SmsTemplateId == absenceSmsId &&
                                  u.ChildId == childId);
          }
          else
          {
            userTemplate = await _context.UserSmsTemplates.FirstOrDefaultAsync(
                                    u => u.ParentId == parentId && u.SmsTemplateId == lateSmsId &&
                                    u.ChildId == childId);
          }

          //set sms data
          if (userTemplate != null)
          {
            var parent = await _context.Users.FirstAsync(p => p.Id == parentId);
            AbsenceSmsDto asd = new AbsenceSmsDto();
            asd.ChildId = childId;
            asd.AbsenceTypeId = absence.AbsenceTypeId;
            asd.ChildFirstName = child.FirstName;
            asd.ChildLastName = child.LastName;
            asd.ParentId = parent.Id;
            asd.ParentFirstName = parent.FirstName;
            asd.ParentLastName = parent.LastName.FirstLetterToUpper();
            asd.ParentGender = parent.Gender;
            asd.CourseName = session.CourseAbbrev;
            asd.SessionDate = session.SessionDate.ToString("dd/MM/yyyy", frC);
            asd.CourseStartHour = session.StartHourMin;
            asd.CourseEndHour = session.EndHourMin;
            asd.ParentCellPhone = parent.PhoneNumber;
            asd.LateInMin = (absence.EndDate - absence.StartDate).TotalMinutes.ToString();
            //asd.scheduledDeliveryTime = deliveryTime;
            absSmsData.Add(asd);
          }
        }
      }

      int currentTeacherId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
      
      List<Sms> absSms = await _repo.SetSmsDataForAbsences(absSmsData, sessionId, currentTeacherId);
      _context.AddRange(absSms);

      if (await _repo.SaveAll())
        return Ok();

      throw new Exception($"la saisie de l'apppel a échoué");
    }

    [HttpGet("absences/{sessionId}")]
    public async Task<IActionResult> GetAbsencesBySessionId(int sessionId)
    {
        var absences = await _context.Absences.Where(a => a.SessionId == sessionId).ToListAsync();
        return Ok(absences);
    }

    [HttpGet("GetAllCoursesDetails")]
    public async Task<IActionResult> GetAllCoursesDetails()
    {
      var data = new List<CoursesDetailsDto>();
      var allCourses = await _context.Courses.OrderBy(c => c.Name).ToListAsync();
      foreach (var cours in allCourses)
      {
        var c = new CoursesDetailsDto {
          Id = cours.Id,
          Name = cours.Name,
          Abbreviation = cours.Abbreviation,
          Color = cours.Color
        };
        c.TeachersNumber = await _context.ClassCourses.Where(a => a.CourseId == cours.Id).Distinct().CountAsync();
        List<int> classIds = await _context.ClassCourses.Where(a => a.CourseId == cours.Id).Select(a => a.ClassId).ToListAsync();
        c.ClassesNumber = classIds.Count();
        c.StudentsNumber = await _context.Users.Where(a => classIds.Contains(Convert.ToInt32(a.ClassId))).CountAsync();
        data.Add(c);
      }

      return Ok(data);
    }

    [HttpGet("GetClassTypes")]
    public async Task<IActionResult> GetClassTypes()
    {
        return Ok(await _repo.GetClassTypes());
    }

    [HttpPost("SaveNewClasses")]
    public async Task<IActionResult> SaveNewClasses(ClassForAddingDto model)
    {
        try
        {
            if (model.suffixe != null)
            {
                var levelName = _context.ClassLevels.FirstOrDefault(e => e.Id == model.LevelId).Name + " " + model.Name;
                //plusieurs classes a ajouter
                if (model.suffixe == 1)
                {
                    //suffixe alphabetic
                    var compteur = 1;
                    for (char i = 'A'; i < 'Z'; i++)
                    {
                        if (compteur <= model.Number)
                        {
                            var newClass = new Class
                            {
                                Name = levelName + " " + i,
                                ClassLevelId = model.LevelId,
                                Active = 1,
                                ClassTypeId = model.classTypeId,
                                MaxStudent = model.maxStudent
                            };
                            await _context.Classes.AddAsync(newClass);

                            compteur++;
                        }
                    }
                }
                else
                {
                    //suffixe numeric
                    for (int i = 1; i <= model.Number; i++)
                    {
                        var newClass = new Class
                        {
                            Name = levelName + " " + i,
                            ClassLevelId = model.LevelId,
                            Active = 1,
                            ClassTypeId = model.classTypeId,
                            MaxStudent = model.maxStudent
                        };
                        await _context.Classes.AddAsync(newClass);
                    }
                }
            }
            else
            {
                var newClass = new Class { Name = model.Name, ClassLevelId = model.LevelId, MaxStudent = model.maxStudent, Active = 1, ClassTypeId = model.classTypeId };
                await _context.Classes.AddAsync(newClass);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (System.Exception ex)
        {

            return BadRequest(ex.Message);
        }
    }

    [HttpGet("GetAllTeachersCourses")]
    public async Task<IActionResult> GetAllTeachersCourses()
    {
        //recuperation de tous les professeurs ainsi que les cours affectés
        var teachers = await _context.Users
                              .Include(p => p.Photos)
                              .Where(u => u.UserTypeId == teacherTypeId && u.ValidatedCode == true)
                              .OrderBy(t => t.LastName).ThenBy(t => t.FirstName).ToListAsync();

        var teachersToReturn = new List<TeacherForListDto>();
        foreach (var teacher in teachers)
        {
            var tdetails = new TeacherForListDto();
            tdetails.PhoneNumber = teacher.PhoneNumber;
            tdetails.SecondPhoneNumber = teacher.SecondPhoneNumber;
            tdetails.Email = teacher.Email;
            tdetails.Id = teacher.Id;
            tdetails.LastName = teacher.LastName;
            tdetails.FirstName = teacher.FirstName;
            tdetails.Gender = teacher.Gender;
            tdetails.DateOfBirth = teacher.DateOfBirth.ToString("dd/MM/yyyy", frC);
            tdetails.CourseClasses = new List<TeacherCourseClassesDto>();
            var photo = teacher.Photos.FirstOrDefault(i => i.IsMain == true);
            if (photo != null)
                tdetails.PhotoUrl = photo.Url;

            var allteacherCourses = await _context.TeacherCourses
                                          .Include(c => c.Course)
                                          .Where(t => t.TeacherId == teacher.Id).ToListAsync();

            foreach (var cours in allteacherCourses)
            {
                var cdetails = new TeacherCourseClassesDto();
                cdetails.Course = cours.Course;
                cdetails.Classes = new List<Class>();
                var classes = _context.ClassCourses.Include(c => c.Class)
                  .Where(t => t.TeacherId == teacher.Id && t.CourseId == cours.CourseId).ToList();
                if (classes != null & classes.Count() > 0)
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
      if (teacherForUpdate.CourseIds.Count() > 0)
      {
        // les cours sont bien renseignés 
        var courseIds = new List<int>();
        foreach (var item in teacherForUpdate.CourseIds)
        {
            courseIds.Add(Convert.ToInt32(item));
        }

        var teacherCourses = await _context.TeacherCourses.Where(t => t.TeacherId == id).ToListAsync();
        // recupartion des courseId du profésseur
        var ccIds = teacherCourses.Select(c => c.CourseId).ToList();

        foreach (var courId in courseIds.Except(ccIds))
        {
            //ajout d'une nouvelle ligne dans TeacheCourses
            var cl = new TeacherCourse { TeacherId = id, CourseId = courId };
            _repo.Add(cl);
        }

        foreach (var courId in ccIds.Except(courseIds))
        {
            var currentLines = _context.ClassCourses.Where(c => c.CourseId == courId && c.TeacherId == id);
            if (currentLines.Count() == 0)
                _repo.Delete(teacherCourses.FirstOrDefault(t => t.TeacherId == id && t.CourseId == courId));
            // suppression de la ligne concernée....
        }

        var userFromRepo = await _repo.GetUser(id, false);
        // _mapper.Map(model, userFromRepo);
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

    [HttpPost("{teacherId}/AssignClasses")]
    public async Task<IActionResult> AssignClasses(int teacherId, [FromBody] List<AssignedClassesDto> courses)
    {
      Boolean dataToBeSaved = false;

      foreach (var course in courses) 
      {
        foreach (var level in course.Levels)
        {
          //delete previous classes selection
          List<ClassCourse> prevClasses = await _context.ClassCourses
                                          .Include(c => c.Class)
                                          .Where(c => c.CourseId == course.CourseId && c.Class.ClassLevelId == level.LevelId)
                                          .ToListAsync();
          if(prevClasses.Count() > 0)
          {
            _repo.DeleteAll(prevClasses);
            dataToBeSaved = true;
          }

         // add new classes selection
          List<ClassCourse> newSelection = new List<ClassCourse>();
          foreach (var aclass in level.Classes)
          {
            if(aclass.Active == true)
            {
              ClassCourse classCourse = new ClassCourse()
              {
                ClassId = aclass.ClassId,
                CourseId = course.CourseId,
                TeacherId = teacherId
              };
              //newSelection.Add(classCourse);
              _repo.Add(classCourse);
              dataToBeSaved = true;
            }
          }
          //_context.AddRange(newSelection);
         }
      }

      if(dataToBeSaved && await _repo.SaveAll())
      {
        return Ok();
      }
      else
      {
        return NoContent();
      }
    }

    [HttpPost("{id}/{courseId}/{levelId}/SaveTeacherAffectation")]
    public async Task<IActionResult> SaveTeacherAffectation(int id, int courseId, int levelId, [FromBody] List<int> classIds)
    {
      var classcourses = await _context.ClassCourses
                                .Include(c => c.Class)
                                .Where(c => c.CourseId == courseId && c.Class.ClassLevelId == levelId)
                                .ToListAsync();

      Boolean dataToBeSaved = false;

      // récupération des classIds
      var cids = classcourses.Select(c => c.ClassId).Distinct().ToList();

      //add new affection (new lines in DB)
      foreach (var item in classIds.Except(cids))
      {
        ClassCourse classCourse = new ClassCourse
        {
          CourseId = courseId,
          ClassId = item,
          TeacherId = id
        };
        _context.Add(classCourse);
        dataToBeSaved = true;
      }

      //set teacher for existing class/course
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

      if(dataToBeSaved && await _repo.SaveAll())
      {
        return Ok();
      }
      else
      {
        return NoContent();
      }

      //return BadRequest("problème pour affecter les classes");
    }

    [HttpGet("TeacherClassCoursByLevel/{teacherId}/{levelId}/{courseId}")]
    public async Task<IActionResult> TeacherClassCoursByLevel(int teacherid, int levelId, int courseId)
    {
        var res = await _context.ClassCourses.Include(c => c.Class)
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
        var classTypes = await _context.ClassTypes.OrderBy(a => a.Name).ToListAsync();
        return Ok(classTypes);
    }

    [HttpPost("CreateCourseCoefficient")]
    public async Task<IActionResult> CreateCourseCoefficient(CreateCoefficientDto coefficientToCreate)
    {
        var coefficient = _mapper.Map<CourseCoefficient>(coefficientToCreate);
        _repo.Add(coefficient);
        if (await _repo.SaveAll())
            return Ok();

        return BadRequest("impossible de faire l'ajout");
    }

    [HttpGet("ClassLevelCoefficients/{classLevelId}")]
    public async Task<IActionResult> ClassLevelCoefficients(int classLevelId)
    {
        var coefficients = await _context.CourseCoefficients
            .Include(c => c.ClassType)
            .Include(c => c.Course)
            .Where(c => c.ClassLevelid == classLevelId).ToListAsync();
        return Ok(coefficients);
    }

    [HttpGet("CourseCoefficient/{id}")]
    public async Task<IActionResult> CourseCoefficient(int id)
    {
        var courseCoef = await _context.CourseCoefficients.FirstOrDefaultAsync(a => a.Id == id);
        return Ok(courseCoef);
    }

    [HttpPost("EditCoefficient/{id}/{coeffificient}")]
    public async Task<IActionResult> EditCoefficient(int id, int coeffificient)
    {
        var coef = await _context.CourseCoefficients.FirstOrDefaultAsync(i => i.Id == id);
        if (coef != null)
        {
            coef.Coefficient = coeffificient;
            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("impossible de faire la mise à jour");
        }
        return NotFound();
    }

    [HttpPost("SaveNewTheme")]
    public async Task<IActionResult> SaveNewTheme(NewThemeDto newThemeDto)
    {
      int themeId = 0;
      if (!string.IsNullOrEmpty(newThemeDto.Name))
      {
        //enregistrement du theme
        var theme = new Theme
        {
          Name = newThemeDto.Name,
          Desc = newThemeDto.Desc,
          CourseId = newThemeDto.CourseId,
          ClassLevelId = newThemeDto.ClassLevelId
        };
        _context.Add(theme);
        themeId = theme.Id;
      }

      foreach (var less in newThemeDto.lessons)
      {
        var lesson = new Lesson { Name = less.Name, Desc = less.Desc, DsplSeq = less.DsplSeq };

        if (themeId != 0)
        {
          lesson.ThemeId = themeId;
        }
        else
        {
          lesson.CourseId = newThemeDto.CourseId;
          lesson.ClassLevelId = newThemeDto.ClassLevelId;
        }
        _context.Add(lesson);


        // enregistrement des contents
        foreach (var cont in less.contents)
        {
          var content = new LessonContent
          {
            Name = cont.Name,
            Desc = cont.Desc,
            NbHours = cont.NbHours,
            LessonId = lesson.Id,
            DsplSeq = cont.DsplSeq
          };
          _context.Add(content);
        }
      }

      if (await _repo.SaveAll())
        return Ok();

      return BadRequest("impossible de faire l'ajout");
    }

    [HttpGet("ClassLevelCourseThemes/{classlevelId}/{courseId}")]
    public async Task<IActionResult> ClassLevelCourseThemes(int classlevelId, int courseId )
    {
      var themes =await _repo.ClassLevelCourseThemes(classlevelId, courseId);
      if(themes.Count()>0)
          return Ok( new{type = "byTheme", themes = themes});

      var lessons  = await _repo.ClassLevelCourseLessons(classlevelId, courseId);
      if(lessons.Count()>0)
      return Ok( new{type = "byLesson", lessons = lessons});
      else
       return Ok();
    }

    // [HttpGet("SearchThemesOrLessons/{classLevelId}/{courseId}")]
    // public async Task<IActionResult> SearchThemesOrLessons(int classLevelId, int courseId)
    // {
    //   // ClassWithThemesDto cwtd = new ClassWithThemesDto();
    //   // List<ClassWithThemesDto> classesWithProgram = new List<ClassWithThemesDto>();
    //   var themesToReturn = new List<Theme>();
    //   themesToReturn = await _context.Themes
    //                       .Include(a => a.Lessons)
    //                       .ThenInclude(a => a.LessonContents)
    //                       .Where(t => t.ClassLevelId == classLevelId && t.CourseId == courseId)
    //                       .ToListAsync();

    //   //Progeam from Lessons (when lessons don't come from themes)
    //   var LessonsFromDB = await _context.Lessons
    //                             .Include(a => a.LessonContents)
    //                             .Where(t => t.ClassLevelId == classLevelId && t.CourseId == courseId)
    //                             .ToListAsync();
    //   var tt = new Theme { Id = 0, Lessons = LessonsFromDB };
    //   themesToReturn.Add(tt);

    //   return Ok(themesToReturn);
    // }

    [HttpGet("Periods")]
    public async Task<IActionResult> GetPeriods()
    {
      var periods = await _context.Periods.OrderBy(o => o.Name).ToListAsync();
      return Ok(periods);
    }

    [HttpGet("ClassEvents")]
    public async Task<IActionResult> GetClassEvents()
    {
      var events = await _context.ClassEvents.OrderByDescending(e => e.Name).ToListAsync();
      return Ok(events);
    }

  }
}