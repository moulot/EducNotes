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
            if(today == 6 || today == 7)
                today = 1;

            return Ok(await _repo.GetScheduleDay(classId, today));
        }

        [HttpGet("{classId}/schedule/{day}")]
        public async Task<IActionResult> GetScheduleDay(int classId, int day)
        {
            var theclass = await GetClass(classId);

            if(theclass == null)
                return BadRequest("la classe n'existe pas");

            if(day < 1 || day > 7)
                return BadRequest("le jour de la semaine est incorrect.");

            var courses = await _context.Schedules
                .Include(i => i.Class).ThenInclude(i => i.ClassLevel)
                .Include(i => i.Course)
                .Where(d => d.Day == day && d.ClassId == classId)
                .OrderBy(s => s.StartHourMin).ToListAsync();

            if(courses.Count == 0)
                return Unauthorized();

            var coursesToReturn = _mapper.Map<IEnumerable<ScheduleForTimeTableDto>>(courses);

            return Ok(coursesToReturn);
        }

        [HttpGet("{classId}/schedule")]
        public async Task<IActionResult> GetClassSchedule(int classId)
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var dayInt = (int)today.DayOfWeek == 0 ? 7 : (int)today.DayOfWeek;
            var monday = today.AddDays(1 - dayInt);
            var sunday = monday.AddDays(6);

            var itemsFromRepo = await _repo.GetClassSchedule(classId);
            var itemsToReturn = _mapper.Map<IEnumerable<ScheduleForTimeTableDto>>(itemsFromRepo);

            var days = new List<string>();
            for(int i = 0; i <= 6; i++)
            {
                DateTime dt = monday.AddDays(i);
                CultureInfo frC = new CultureInfo("fr-FR");
                var shortdate = dt.ToString("ddd dd MMM", frC);
                days.Add(shortdate);
            }

           if (itemsToReturn != null)
           {
                return Ok(new {
                    scheduleItems = itemsToReturn,
                    firstDayWeek = monday,
                    strMonday = monday.ToLongDateString(),
                    strSunday = sunday.ToLongDateString(),
                    weekDays = days
                });
           }
           
           return BadRequest("Aucun emploi du temps trouvé");            
        }

        [HttpGet("{classId}/getClassScheduleMovedWeek")]
        public async Task<IActionResult> getClassScheduleMovedWeek(int classId, [FromQuery]ScheduleParams agendaParams)
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
            for(int i = 0; i <= 6; i++)
            {
                DateTime dt = monday.AddDays(i);
                CultureInfo frC = new CultureInfo("fr-FR");
                var shortdate = dt.ToString("ddd dd MMM", frC);
                days.Add(shortdate);
            }

           if (itemsFromRepo != null)
           {
                return Ok( new {
                    agendaItems = itemsFromRepo,
                    firstDayWeek = monday,
                    strMonday = monday.ToLongDateString(),
                    strSunday = sunday.ToLongDateString(),
                    weekDays = days
                });
           }
           
           return BadRequest("Aucun emploi du temps trouvé");            
        }

        [HttpGet("{classId}/GetClassTeachers")]
        public async Task<IActionResult> GetClassTeachers(int classId)
        {
            var teachersFromRepo = await _repo.GetClassTeachers(classId);
            var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(teachersFromRepo);

            return Ok(usersToReturn);
        }

        [HttpGet("{classId}/Students")]
        public async Task<IActionResult> GetClassStudents(int classId)
        {
            var studentsFromRepo = await _repo.GetClassStudents(classId);
            var usersToReturn = _mapper.Map<IEnumerable<UserForDetailedDto>>(studentsFromRepo);

            return Ok(usersToReturn);
        }

        [HttpGet("{classId}/MovedWeekAgenda")]
        public async Task<IActionResult> getClassMovedWeekAgenda(int classId, [FromQuery]AgendaParams agendaParams)
        {
            var FromDate = agendaParams.DueDate.Date;
            var move = agendaParams.MoveWeek;
            var date = FromDate.AddDays(move);
            var dateDay = (int)date.DayOfWeek;

            var dayInt = dateDay == 0 ? 7 : dateDay;
            DateTime monday = date.AddDays(1 - dayInt);
            var saturday = monday.AddDays(5);

            var itemsFromRepo = await _repo.GetClassAgenda(classId, monday, saturday);
            var items = GetAgendaListByDueDate(itemsFromRepo);

            var days = new List<string>();
            var nbTasks = new List<int>();
            for(int i = 0; i <= 5; i++)
            {
                DateTime dt = monday.AddDays(i);
                CultureInfo frC = new CultureInfo("fr-FR");
                var shortdate = dt.ToString("ddd dd MMM", frC);
                days.Add(shortdate);

                var item = items.FirstOrDefault(a => a.dtDueDate == dt);
                if(item != null)
                    nbTasks.Add(item.NbTasks);
                else
                    nbTasks.Add(0);
            }

           if (itemsFromRepo != null)
           {
                return Ok( new {
                    agendaItems = items,
                    firstDayWeek = monday,
                    strMonday = monday.ToLongDateString(),
                    strSaturday = saturday.ToLongDateString(),
                    weekDays = days,
                    nbDayTasks = nbTasks
                });
           }
           
           return BadRequest("Aucun agenda trouvé");            
        }

        [HttpGet("{classId}/TodayToNDaysAgenda/{toNbDays}")]
        public async Task<IActionResult> getTodayToNDaysAgenda(int classId, int toNbDays)
        {
          var agendasFromRepo = await _repo.GetClassAgendaTodayToNDays(classId, toNbDays);
          var items = GetAgendaListByDueDate(agendasFromRepo);

            var days = new List<string>();
            var nbTasks = new List<int>();

            var today = DateTime.Now.Date;

            for(int i = 0; i < toNbDays; i++)
            {
              DateTime dt = today.AddDays(i);
              CultureInfo frC = new CultureInfo("fr-FR");
              var shortdate = dt.ToString("ddd dd MMM", frC);
              days.Add(shortdate);

              var item = items.FirstOrDefault(a => a.dtDueDate == dt);
              if(item != null)
                  nbTasks.Add(item.NbTasks);
              else
                  nbTasks.Add(0);
            }

            var lastDay = today.AddDays(toNbDays);

            if (agendasFromRepo != null)
            {
              return Ok(new {
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
            var items = GetAgendaListByDueDate(itemsFromRepo);

            var days = new List<string>();
            var nbTasks = new List<int>();
            for(int i = 0; i <= 5; i++)
            {
                DateTime dt = monday.AddDays(i);
                CultureInfo frC = new CultureInfo("fr-FR");
                var shortdate = dt.ToString("ddd dd MMM", frC);
                days.Add(shortdate);

                var item = items.FirstOrDefault(a => a.dtDueDate == dt);
                if(item != null)
                    nbTasks.Add(item.NbTasks);
                else
                    nbTasks.Add(0);
            }

           if (itemsFromRepo != null)
           {
                return Ok(new {
                    agendaItems = items,
                    firstDayWeek = monday,
                    strMonday = monday.ToLongDateString(),
                    strSaturday = saturday.ToLongDateString(),
                    weekDays = days,
                    nbDayTasks = nbTasks
                });
           }
           
           return BadRequest("Aucun agenda trouvé");            
        }

        [HttpGet("GetWeekDays")]
        public IActionResult GetWeekDaysByDate([FromQuery]AgendaParams agendaParams)
        {
            var date = agendaParams.DueDate;

            var weekDays = _repo.GetWeekDays(date);

            return Ok(weekDays);
        }

        [HttpGet("GetSanctions")]
        public async Task<IActionResult> GetSanctions()
        {
            var sanctions = await _context.Sanctions.OrderBy(o => o.Name).ToListAsync();
                        
            return  Ok(sanctions);
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

            return Ok(new {
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

            return Ok( new {
                sanctions = sanctionsToReturn,
                nbSanctions = nbClassSanctions
            });
        }

        [HttpGet("{classId}/GetClassAgenda")]
        public async Task<IActionResult> GetClassAgenda(int classId, DateTime StartDate, DateTime EndDate)
        {
           var agendaItems = await _repo.GetClassAgenda(classId, StartDate, EndDate);

           if (agendaItems != null)
               return Ok(GetAgendaListByDueDate(agendaItems));
           
           return BadRequest("Aucun agenda trouvé");            
        }

        private List<ClassAgendaToReturnDto> GetAgendaListByDueDate(IEnumerable<Agenda> agendaItems)
        {
                //selection de toutes les differentes dates
                var dueDates = agendaItems.OrderBy(o => o.DueDate).Select(e => e.DueDate).Distinct();

                var agendasToReturn = new List<ClassAgendaToReturnDto>();
                    foreach (var currDate in dueDates)
                    {
                        var currentDateAgendas = agendaItems.Where(e => e.DueDate == currDate);
                        var agenda = new ClassAgendaToReturnDto();
                        agenda.dtDueDate = currDate;
                        agenda.DueDate = currDate.ToLongDateString();
                        agenda.DueDateDay = currDate.Day;
                        agenda.NbTasks = currentDateAgendas.Count();
                        var dayInt = (int)currDate.DayOfWeek;
                        agenda.DayInt = dayInt == 0 ? 7 : dayInt;
                        agenda.Courses = new List<CourseTask>();
                        foreach (var a in currentDateAgendas)
                        {
                            agenda.Courses.Add( new CourseTask {
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

        [HttpGet("{claissId}/agendas/{agendaId}")]
        public async Task<IActionResult> GetAgendaById(int agendaId)
        {
            var agenda = await _repo.GetAgenda(agendaId);

            return Ok(agenda);
        }

        [HttpGet("GetAgendaItem")]
        public async Task<IActionResult> GetAgendaItem([FromQuery]AgendaParams agendaParams)
        {
            var classId = agendaParams.ClassId;
            var courseId = agendaParams.CourseId;
            var dueDate = agendaParams.DueDate;
            
            var agendaItem = await _context.Agendas.FirstOrDefaultAsync(i => i.ClassId == classId
                                                    && i.CourseId == courseId && i.DueDate == dueDate);

            //return Ok(agendaItem);
            if(agendaItem != null)
            {
                return Ok(agendaItem);
            }
            else
            {
                Agenda emptyAgendaItem = new Agenda();
                return Ok(emptyAgendaItem);
            }
            
        }

        [HttpPut("SaveAgenda")]
        public async Task<IActionResult> SaveAgendaItem([FromBody]AgendaForSaveDto agendaForSaveDto)
        {
            var id = agendaForSaveDto.Id;
            if(id == 0)
            {
                Agenda newAgendaItem = new Agenda();
                _mapper.Map(agendaForSaveDto, newAgendaItem);
                newAgendaItem.DateAdded = DateTime.Now;
                _repo.Add(newAgendaItem);
            }
            else
            {
                var agendaItemFromRepo = await _repo.GetAgenda(id);
                _mapper.Map(agendaForSaveDto, agendaItemFromRepo);
                agendaItemFromRepo.DateAdded = DateTime.Now;
            }

            if(await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating/Saving agendaItem failed");
        }
        
        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
        {
            var les_cours = _context.Courses.OrderBy(a => a.Name);

            return Ok(await les_cours.ToListAsync());
        }


        [HttpGet("GetAllClasses")]
        public async Task<IActionResult> GetAllClasses()
        {
            var les_classes = _context.Classes.Include(e => e.ClassLevel).OrderBy(a => a.Name);

            return Ok(await les_classes.ToListAsync());
        }

        // [HttpGet("{id}/GetAllTeacherCoursesById")]
        // public async Task<IActionResult> GetAllTeacherCoursesById(int id)
        // {
        //     //recuperation de tous les professeurs ainsi que les cours affectés
        //     var teacher = await _context.Users.Include(p => p.Photos).Include(p => p.UserType).FirstOrDefaultAsync(u => u.Id==id);
        //      var teacherListToReturn =new TeacherForListDto
        //         {
        //             Teacher = _mapper.Map<UserForListDto>(teacher),
        //             Courses = await _repo.GetTeacherCoursesAndClasses(teacher.Id)
        //         };
            

        //     return Ok(teacherListToReturn);

        // }

        // [HttpGet("GetAllCoursesDetails")]
        // public async Task<IActionResult> GetAllCoursesDetails()
        // {
        //     var data = new List<CoursesDetailsDto>();
        //     var allCourses = await _context.Courses.OrderBy(c => c.Name).ToListAsync();
        //     foreach (var item in allCourses)
        //     {
        //         var cc = new List<Class>();
        //         cc = await _context.ClassCourses.Include(c => c.Course).Where(c => c.CourseId == item.Id).Select(c => c.Class).ToListAsync();
        //         var tt = new List<UserForListDto>();
        //         var teachers = _context.CourseUsers.Include(c => c.Teacher).ThenInclude(t => t.Photos).Where(c => c.CourseId == item.Id);

        //         foreach (var t in teachers)
        //         {
        //             tt.Add(_mapper.Map<UserForListDto>(t.Teacher));
        //         }
        //         int total = 0;
        //         foreach (var i in cc)
        //         {
        //             total += _context.Users.Where(c => c.ClassId == i.Id).Count();
        //         }

        //         data.Add(new CoursesDetailsDto
        //         {
        //             Course = item,
        //             Classes = cc,
        //             StudentsNumber = total,
        //             Teachers = tt
        //         });
        //     }
        //     return Ok(data);
        // }

        [HttpPost("{courseId}/UpdateCourse/{courseName}")]
        public async Task<IActionResult> UpdateCourse(int courseId, string courseName)
        {
            var courseFromRepo = await _repo.GetCourse(courseId);
            courseFromRepo.Name = courseName;
            _repo.Update(courseFromRepo);
            if(await _repo.SaveAll())
            return NoContent();

            return BadRequest("impossible de mettre à jour ce cours");

        }

        // [HttpPost("{id}/{courseId}/SaveTeacherAffectation")]
        // public async Task<IActionResult> SaveTeacherAffectation(int id, int courseId, [FromBody]List<int> classIds)
        // {
        //     try
        //     {
        //         var classcourses = await _context.ClassCourses.Where(c => c.CourseId == courseId).ToListAsync();
        //         var tt = classcourses.Select(c => c.ClassId);
        //         foreach (var item in classIds.Except(tt))
        //         {
        //             //ajout d'une nouvelle ligne
        //             _context.Add(new ClassCourse { CourseId = courseId, ClassId = item, TeacherId = id });
        //         }
        //         foreach (var item in classIds)
        //         {
        //             var cc = classcourses.FirstOrDefault(c => c.ClassId == item);
        //             if (cc != null)
        //             {
        //                 cc.TeacherId = id;
        //             }
        //         }
        //         var u = classcourses.Where(teacherTypeId => teacherTypeId.TeacherId == id).Select(c => c.ClassId);
        //         foreach (var item in u.Except(classIds))
        //         {
        //             var cc = classcourses.FirstOrDefault(c => c.ClassId == item);
        //             cc.TeacherId = null;

        //         }

        //         if (await _repo.SaveAll())
        //         {
        //             var teacherToReturn = new TeacherForListDto()
        //             {
        //                 Teacher = _mapper.Map<UserForListDto>(await _repo.GetUser(id, false)),
        //                 Courses = await _repo.GetTeacherCoursesAndClasses(id)
        //             };
        //             return Ok(teacherToReturn);
        //         }
        //         return BadRequest("impossiblle de terminer l'opération");
        //     }
        //     catch (System.Exception)
        //     {

        //         return BadRequest("impossiblle de terminer l'opération");
        //     }
        // }
       

        // [HttpGet("GetCoursesTeachers")]
        // public async Task<IActionResult> GetCoursesTeachers()
        // {
        //     //CoursesTeacherDto
        //     var data = new List<CoursesTeacherDto>();
        //     var courses = await _context.Courses.OrderBy(e => e.Name).ToListAsync();
        //     foreach (var item in courses)
        //     {
        //         data.Add(new CoursesTeacherDto
        //         {
        //             Course = item,
        //             Teachers = _mapper.Map<List<UserForDetailedDto>>
        //         (_context.CourseUsers.Include(c => c.Teacher).ThenInclude(t => t.Photos).Where(c => c.CourseId == item.Id).Select(c => c.Teacher))
        //         });
        //     }
        //     return Ok(data);
        // }

        [HttpGet("GetLevels")]
        public async Task<IActionResult> GetLevels()
        {
           // return Ok(await _context.ClassLevels.OrderBy(e => e.Name).ToListAsync());
          var levels =await _context.ClassLevels
                                    // .Include(i=>i.Inscriptions) 
                                    .Include(c=>c.Classes)
                                    // .ThenInclude(c=>c.Students)
                                    .OrderBy(a=>a.DsplSeq)
                                    .ToListAsync();
           var dataToReturn = new List<ClassLevelDetailDto>();
          foreach (var item in levels)
          {
          var res = new ClassLevelDetailDto();
          res.Id = item.Id;
          res.Name = item.Name;
          res.TotalClasses = item.Classes.Count();
          
          // res.TotalEnrolled = item.Inscriptions.Count();
          // res.TotalStudents = 0;
          // foreach (var c in item.Classes)
          // {
          //     res.TotalStudents+=c.Students.Count();
          // }
          
          res.Classes = item.Classes.ToList();
            dataToReturn.Add(res);

          }
          return Ok(dataToReturn);
        }

        [HttpGet("{classLevelId}/SearchClassesByLevel")]
        public async Task<IActionResult> SearchClassesByLevel(int classLevelId)
        {
            
           var classes = await _context.Classes
                                .Include(s=>s.Students)
                                .Where(c=>c.ClassLevelId == classLevelId)
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
        public async Task<IActionResult> AddCourse([FromBody]NewCourseDto newCourseDto)
        {
            var course = new Course{Name = newCourseDto.name, Abbreviation=newCourseDto.abbreviation};
            _repo.Add(course);
            // foreach (var item in courseDto.classLevelIds)
            // {
            //     var classes = await _context.Classes.Where(a=>a.ClassLevelId == Convert.ToInt32(item)).Select(a=>a.Id).ToListAsync();
            //     foreach (var classId in classes)
            //     {
            //       _repo.Add(new ClassCourse {CourseId = course.Id,ClassId = classId});
            //     }
            // }
            if(await _repo.SaveAll())
              return Ok();
           
            return BadRequest("impossible d'ajouter ce cours");
        }

        [HttpGet("Schedule/{id}")]
        public async Task<IActionResult> GetSchedule(int id)
        {
            var schedule = await _context.Schedules
                                    .Include(i => i.Class)
                                    .Include(i => i.Course)
                                    .FirstOrDefaultAsync(s => s.Id == id);;
            var scheduleToReturn = _mapper.Map<ScheduleToReturnDto>(schedule);

            return Ok(scheduleToReturn);
        }

        [HttpPut("SaveCallSheet/{sessionId}")]
        public async Task<IActionResult> SaveCallSheet(int sessionId, [FromBody]Absence[] absences)
        {
            //delete old absents (update: delete + add)
            if(sessionId > 0){
                List<Absence> oldAbsences = await _context.Absences.Where(a => a.SessionId == sessionId).ToListAsync();
                if(oldAbsences.Count() > 0)
                    _repo.DeleteAll(oldAbsences);
            }

            //add new absents
            for (int i = 0; i < absences.Length; i++)
            {
                Absence absence = absences[i];
                _repo.Add(absence);
            }

            if(await _repo.SaveAll())
                return NoContent();

            throw new Exception($"la validation de l'apppel a échoué");
        }

        [HttpGet("Session/{scheduleId}")]
        public async Task<IActionResult> GetSession(int scheduleId)
        {
            var schedule = _context.Schedules.Where(s => s.Id == scheduleId).FirstOrDefault();
            if(schedule == null)
                return BadRequest("problème pour créer la session du cours.");

            var scheduleDay = schedule.Day;

            var today = DateTime.Now.Date;
            // monday=1, tue=2, ...
            var todayDay = ((int)today.DayOfWeek == 0) ? 7 : (int)DateTime.Now.DayOfWeek;

            if(todayDay != scheduleDay)
                return BadRequest("l'emploi du temps du jour est incohérent.");

            // get session by schedule and date
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.ScheduleId == schedule.Id
                && s.SessionDate.Date == today);

            if(session != null)
            {
                return Ok(session);
            }
            else
            {
                var newSession = _context.Add(new Session {
                    ScheduleId = schedule.Id,
                    SessionDate  = today
                });

                if(await _repo.SaveAll())
                    return Ok(newSession);

                return BadRequest("problème pour récupérer la session");
            }
        }

        [HttpGet("absences/{sessionId}")]
        public async Task<IActionResult> GetAbsencesBySessionId(int sessionId)
        {
            var absences = await _context.Absences.Where(a => a.SessionId == sessionId).ToListAsync();

            return Ok(absences);
        }
    
/////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////// DATA FROM MOHAMED KABORE ////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet("GetAllCoursesDetails")]
        public async Task<IActionResult> GetAllCoursesDetails()
        {
            var data = new List<CoursesDetailsDto>();
            var allCourses = await _context.Courses.OrderBy(c => c.Name).ToListAsync();
            foreach (var cours in allCourses)
            {
                var c = new CoursesDetailsDto{Id=cours.Id,Name= cours.Name};
                    c.TeachersNumber = await _context.ClassCourses.Where(a=>a.CourseId == cours.Id && a.TeacherId != null).Distinct().CountAsync();
                    List<int?> classIds= await _context.ClassCourses.Where(a=>a.CourseId == cours.Id).Select(a=>a.ClassId).ToListAsync();
                    c.ClassesNumber = classIds.Count();
                    c.StudentsNumber = await _context.Users.Where(a=> classIds.Contains(Convert.ToInt32(a.ClassId))).CountAsync();
                    data.Add(c);
            }
            // var allCourses = await _context.Courses.OrderBy(c => c.Name).ToListAsync();
            // foreach (var item in allCourses)
            // {
            //     var cc = new List<Class>();
            //     cc = await _context.ClassCourses.Include(c => c.Course).Where(c => c.CourseId == item.Id).Select(c => c.Class).ToListAsync();
            //     var tt = new List<UserForListDto>();
            //     var teachers = _context.CourseUsers.Include(c => c.Teacher).ThenInclude(t => t.Photos).Where(c => c.CourseId == item.Id);

            //     foreach (var t in teachers)
            //     {
            //         tt.Add(_mapper.Map<UserForListDto>(t.Teacher));
            //     }
            //     int total = 0;
            //     foreach (var i in cc)
            //     {
            //         total += _context.Users.Where(c => c.ClassId == i.Id).Count();
            //     }

            //     data.Add(new CoursesDetailsDto
            //     {
            //         Course = item,
            //         Classes = cc,
            //         StudentsNumber = total,
            //         Teachers = tt
            //     });
            //}
            return Ok(data);
        }

        [HttpGet("GetClassTypes")]
        public async Task<IActionResult>GetClassTypes()
        {
        return Ok (await _repo.GetClassTypes());
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
                                var newClass = new Class { Name = levelName + " " + i, ClassLevelId = model.LevelId,
                                 Active = 1,ClassTypeId = model.classTypeId,MaxStudent = model.maxStudent };
                                await _context.Classes.AddAsync(newClass);
                                // foreach (var item in model.CourseIds)
                                // {
                                //     await _context.ClassCourses.AddAsync(new ClassCourse
                                //     {
                                //         ClassId = newClass.Id,
                                //         CourseId = item,
                                //     });
                                // }
                                compteur++;
                            }
                        }
                    }
                    else
                    {
                        //suffixe numeric
                        for (int i = 1; i <= model.Number; i++)
                        {
                            var newClass = new Class { Name = levelName + " " + i, ClassLevelId = model.LevelId, Active = 1,
                            ClassTypeId = model.classTypeId,MaxStudent = model.maxStudent };
                            await _context.Classes.AddAsync(newClass);
                            // foreach (var item in model.CourseIds)
                            // {
                            //     await _context.ClassCourses.AddAsync(new ClassCourse
                            //     {
                            //         ClassId = newClass.Id,
                            //         CourseId = item,
                            //     });
                            // }
                        }
                    }
                }
                else
                {
                    var newClass = new Class { Name = model.Name, ClassLevelId = model.LevelId,MaxStudent = model.maxStudent, Active = 1,ClassTypeId = model.classTypeId };
                    await _context.Classes.AddAsync(newClass);
                    // foreach (var item in model.CourseIds)
                    // {
                    //     await _context.ClassCourses.AddAsync(new ClassCourse
                    //     {
                    //         ClassId = newClass.Id,
                    //         CourseId = item,
                    //     });
                    // }
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
            var teachers = await _context.Users.Include(p => p.Photos).Where(u => u.UserTypeId == teacherTypeId)
            .OrderBy(t => t.LastName).ThenBy(t => t.FirstName).ToListAsync();

            // var teacherCouses = await _context.TeacherCourse.ToListAsync();

            // var classCourses = await _context.ClassCourses.ToListAsync();
            
            // var classCourses =await  _context.ClassCourses
            //                             .Include(t => t.Teacher)
            //                             .Include(c => c.Class)
            //                             .Include(c => c.Course)
            //                             .ToListAsync();
            
           var teachersToReturn = new List<TeacherForListDto>();
           foreach (var teacher in teachers)
           {
                var tdetails = new TeacherForListDto();
                tdetails.PhoneNumber = teacher.PhoneNumber ;
                tdetails.SeconPhoneNumber =teacher.SecondPhoneNumber;
                tdetails.Email = teacher.Email;
                tdetails.Id = teacher.Id;
                tdetails.LastName = teacher.LastName;
                tdetails.FirstName = teacher.FirstName;  
                tdetails.DateOfBirth = teacher.DateOfBirth;
                tdetails.CourseClasses = new List<TeacherCourseClassesDto>();
               // tdetails.PhotoUrl = teacher.Photos.FirstOrDefault(i => i.IsMain == true).Url;
               var allteacherCourses = await _context.TeacherCourses.Include(c => c.Course).Where(t => t.TeacherId == teacher.Id).ToListAsync();

                foreach (var cours in allteacherCourses)
                {
                    var cdetails = new TeacherCourseClassesDto();
                    cdetails.Course = cours.Course;
                    cdetails.Classes = new List<Class>();
                    var classes = _context.ClassCourses.Include(c => c.Class)
                                  .Where(t => t.TeacherId == teacher.Id && t.CourseId == cours.CourseId && t.ClassId !=null).ToList();
                    if(classes!=null & classes.Count()>0)
                    cdetails.Classes =classes.Select(c => c.Class).ToList();
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
                foreach (var item in teacherForUpdate.CourseIds)
                {
                    courseIds.Add(Convert.ToInt32(item)); 
                }
            
            
                var teacherCourses =await _context.TeacherCourses.Where(t => t.TeacherId == id).ToListAsync();
                // recupartion des courseId du profésseur
                var ccIds = teacherCourses.Select(c => c.CourseId).ToList();
                
                foreach (var courId in courseIds.Except(ccIds))
                {
                    //ajout d'une nouvelle ligne dans TeacheCourses
                    var cl = new TeacherCourse{TeacherId = id, CourseId = courId};
                    _repo.Add(cl);
                }

                foreach (var courId in ccIds.Except(courseIds))
                {
                        var currentLines = _context.ClassCourses.Where(c => c.CourseId == courId && c.TeacherId == id);
                        if (currentLines.Count()==0)
                            _repo.Delete(teacherCourses.FirstOrDefault(t => t.TeacherId == id && t.CourseId == courId));
                        // suppression de la ligne concernée....
                }

                var userFromRepo = await _repo.GetUser(id, false);
                // _mapper.Map(model, userFromRepo);
                userFromRepo.FirstName = teacherForUpdate.FirstName;
                userFromRepo.LastName = teacherForUpdate.LastName;
                if(teacherForUpdate.DateOfBirth != null)
                    userFromRepo.DateOfBirth = Convert.ToDateTime(teacherForUpdate.DateOfBirth);
                userFromRepo.Email = teacherForUpdate.Email;
                userFromRepo.PhoneNumber = teacherForUpdate.PhoneNumber;
                userFromRepo.SecondPhoneNumber = teacherForUpdate.SecondPhoneNumber;
                _repo.Update(userFromRepo);

                if(await _repo.SaveAll())
                    return Ok();

                 return BadRequest("impossible de terminer l'action");

          }
          
            return BadRequest("veuillez selectionner au moins un cours");
        }

        [HttpPost("{id}/{courseId}/{levelId}/SaveTeacherAffectation")]
        public async Task<IActionResult> SaveTeacherAffectation(int id, int courseId,int levelId, [FromBody]List<int?> classIds)
        {
               var classcourses = await _context.ClassCourses.Include(c => c.Class)
                         .Where(c => c.CourseId == courseId && c.Class.ClassLevelId == levelId)
                         .ToListAsync();
                // récupération des classIds
                var tt = classcourses.Select(c => c.ClassId).Distinct().ToList();
                foreach (var item in classIds.Except(tt))
                {
                    //ajout d'une nouvelle ligne
                    _context.Add(new ClassCourse { CourseId = courseId, ClassId = item, TeacherId = id });
                }
                foreach (var item in classIds)
                {
                    var cc = classcourses.FirstOrDefault(c => c.ClassId == item);
                    if (cc != null)
                    {
                        cc.TeacherId = id;
                    }
                }
                var u = classcourses.Where(t => t.TeacherId == id).Select(c => c.ClassId);
                foreach (var item in u.Except(classIds))
                {
                    var cc = classcourses.FirstOrDefault(c => c.ClassId == item);
                    cc.TeacherId = null;

                }

                if (await _repo.SaveAll())
                {
                    return Ok();
                }
                return BadRequest("impossiblle de terminer l'opération");
           
        }

        [HttpGet("TeacherClassCoursByLevel/{teacherId}/{levelId}/{courseId}")]
        public async Task<IActionResult> TeacherClassCoursByLevel(int teacherid, int levelId,int courseId)
        {
            var res = await _context.ClassCourses.Include(c =>c.Class)
                    .Where(c => c.TeacherId == teacherid && c.CourseId == courseId && c.Class.ClassLevelId == levelId)
                    .ToListAsync();

            return Ok(res);
        }

    }
}