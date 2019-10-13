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

        private readonly UserManager<User> _userManager;

        public UsersController(IConfiguration config,DataContext context, IEducNotesRepository repo,
        UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _config = config;
            _context = context;
            _repo = repo;
            _mapper = mapper;
            teacherTypeId =  _config.GetValue<int>("AppSettings:teacherTypeId");
            parentTypeId =  _config.GetValue<int>("AppSettings:parentTypeId");
            adminTypeId =  _config.GetValue<int>("AppSettings:adminTypeId");
            studentTypeId =  _config.GetValue<int>("AppSettings:studentTypeId");
            password = _config.GetValue<String>("AppSettings:defaultPassword");
            parentRoleName = _config.GetValue<String>("AppSettings:parentRoleName");
            memberRoleName = _config.GetValue<String>("AppSettings:memberRoleName");
            moderatorRoleName = _config.GetValue<String>("AppSettings:moderatorRoleName");
            adminRoleName = _config.GetValue<String>("AppSettings:adminRoleName");
            professorRoleName = _config.GetValue<String>("AppSettings:professorRoleName");
            
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(currentUserId, true);

            userParams.userId = currentUserId;

           
            var users = await _repo.GetUsers(userParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize,
            users.TotalCount, users.TotalPages);

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

            return Ok(usersToReturn);
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
                                            StartHourMin = Schedule.StartHourMin.ToShortTimeString(),
                                            EndHourMin = Schedule.EndHourMin.ToShortTimeString()
                                        })
                                        .Where(w => w.TeacherId == teacherId && w.Day == todayDay
                                            && w.CourseStartHM.TimeOfDay >= todayHourMin)
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
                                        StartHourMin = Schedule.StartHourMin.ToShortTimeString(),
                                        EndHourMin = Schedule.EndHourMin.ToShortTimeString()
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
                                        StartHourMin = Schedule.StartHourMin.ToShortTimeString(),
                                        EndHourMin = Schedule.EndHourMin.ToShortTimeString()
                                    })
                                    .Where(w => w.TeacherId == teacherId)
                                    .OrderBy(o => o.Day).ThenBy(o => o.StartHourMin)
                                    .ToListAsync();

            return Ok(teacherCourses);
        }

        [HttpGet("{teacherId}/Sessions")]
        public async Task<IActionResult> GetTeacherSessions(int teacherId)
        {
            var agendas = new List<AgendaToReturnDto>();

            var teacherSchedule = await (from courses in _context.ClassCourses
                                    join Schedule in _context.Schedules
                                    on courses.CourseId equals Schedule.CourseId
                                    select new
                                    {
                                        TeacherId = courses.TeacherId,
                                        TeacherName = courses.Teacher.LastName + ' ' + courses.Teacher.FirstName,
                                        CourseId = courses.CourseId,
                                        CourseName = courses.Course.Name,
                                        CourseColor = courses.Course.Color,
                                        ClassId = courses.ClassId,
                                        ClassName = courses.Class.Name,
                                        Day = Schedule.Day,
                                        strDayDate = "",
                                        DayDate = DateTime.Now.Date,
                                        StartHourMin = Schedule.StartHourMin.ToShortTimeString(),
                                        EndHourMin = Schedule.EndHourMin.ToShortTimeString()
                                    })
                                    .Where(w => w.TeacherId == teacherId)
                                    .ToListAsync();

            var sessionsDto = teacherSchedule;//_mapper.Map<List<SessionsToReturnDto>>(teacherSchedule);

            // cahier de textes - periode de sessions des cours du professeur
            var today = DateTime.Now;

            for(int i = 0; i < 7; i++)
            {
                var currentDate = today.AddDays(i);
                var day = ((int)currentDate.DayOfWeek == 0) ? 7 : (int)currentDate.DayOfWeek;

                if(day == 6 || day == 7)
                    continue;
                
                var daySessions = sessionsDto.Where(d => d.Day == day).OrderBy(d => d.StartHourMin);
                var dayDate = Convert.ToDateTime(currentDate.ToShortDateString());
                foreach (var session in daySessions)
                {
                  var tasks = "";
                  var id = 0;
                  var agenda = await _context.Agendas.SingleOrDefaultAsync(a => a.ClassId == session.ClassId &&
                    a.CourseId == session.CourseId && a.DueDate == dayDate);
                  if(agenda != null) {
                    tasks = agenda.TaskDesc;
                    id = agenda.Id;
                  }

                  var newAgenda = new AgendaToReturnDto {
                    Id = id,
                    TeacherId = Convert.ToInt32(session.TeacherId),
                    TeacherName = session.TeacherName,
                    CourseId = Convert.ToInt32(session.CourseId),
                    strDayDate = currentDate.ToShortDateString(),
                    DayDate = dayDate,
                    Day = session.Day,
                    CourseName = session.CourseName,
                    CourseColor = session.CourseColor,
                    ClassId = Convert.ToInt32(session.ClassId),
                    ClassName = session.ClassName,
                    Tasks = tasks,
                    StartHourMin = session.StartHourMin,
                    EndHourMin = session.EndHourMin
                  };

                  agendas.Add(newAgenda);
                }
            }

            return Ok(agendas);
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
            
            // var tclasses = await _context.ClassCourses
            //             //.Include(i =>  i.Class)
            //             .Where(c => c.TeacherId == teacherId)
            //             .Select(s => s.Class).Distinct().ToListAsync();


            // var teacherClasses = await _context.ClassCourses
            //                                 .Include(i => i.Class).ThenInclude(i => i.Students).Distinct()
            //                                 .Where(c => c.TeacherId == teacherId).Distinct().ToListAsync();

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
                    var OpenedEvals = ClassEvals.FindAll(e => e.Closed == 0);
                    var OpenedEvalsDto = _mapper.Map<List<EvaluationForListDto>>(OpenedEvals);
                    var ToBeGradedEvals = ClassEvals.FindAll(e => e.Closed == 1);
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

        [HttpGet("{teacherId}/Courses")]
        public async Task<IActionResult> GetTeacherCourses(int teacherId)
        {
            var courses = await _context.TeacherCourses
                                    .Where(c => c.TeacherId == teacherId)
                                    .Select(s => s.Course).ToListAsync();

            return Ok(courses);
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
            _repo.Add(absence);

            if(await _repo.SaveAll())
                return NoContent();

            throw new Exception($"l'ajout de l'absence a échoué");
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

            

            // [HttpPost("AddUser")]
            // public async Task<IActionResult> AddUser(UserForRegisterDto userForRegisterDto)
            // {
        
            //     var userToCreate = _mapper.Map<User>(userForRegisterDto);
            //     userToCreate.UserName = userForRegisterDto.Email;
            // try
            // {
            //         await _repo.AddUserPreInscription(Guid.NewGuid(),userToCreate,professorRoleName);
            //     if(userForRegisterDto.courseIds!=null)
            //     {
            //         foreach (var course in userForRegisterDto.courseIds)
            //         {
            //             _context.Add( new CourseUser {CourseId = course, TeacherId = userToCreate.Id});
            //         }
            //         await _context.SaveChangesAsync();
            //         var teacherToReturn = new TeacherForListDto(){
            //         Teacher =  _mapper.Map<UserForListDto>(await _repo.GetUser(userToCreate.Id,false)),
            //         Courses = await _repo.GetTeacherCoursesAndClasses(userToCreate.Id)
            //     };
            //     return Ok(teacherToReturn);
            //     }
            //     else
            //     return Ok(_mapper.Map<UserForListDto>(userToCreate));
                
            // }
            // catch (System.Exception ex)
            // {
            //     string mes = ex.Message;

            //     return BadRequest(ex);
            // }

            
            // }
        
            // [HttpPost("{id}/DeleteTeacher")]
            // public async Task<IActionResult> DeleteTeacher(int id)
            // {
            //     var teacher =await _repo.GetUser(id,false);
            //     if(teacher.ValidatedCode == true)
            //     return BadRequest("Désolé ce compte ne peut etre supprimé...");
            //     var classcourses = await _context.ClassCourses.Where(t=>t.TeacherId==id).ToListAsync();
            //     if(classcourses.Count()==0)
            //     {
            //         //possible de supprimer
            //         _context.CourseUsers.RemoveRange(_context.CourseUsers.Where(t=>t.TeacherId==id));
            //         _context.Users.Remove(_context.Users.FirstOrDefault(u=>u.Id==id));
            //     }
            //     if(await _repo.SaveAll())
            //     return Ok("");
            //     return BadRequest("impossible de supprimer cet utilisateur ");


            // }

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