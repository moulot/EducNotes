using System;
using System.Collections.Generic;
using System.Linq;
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
    // [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IEducNotesRepository _repo;
        private readonly IMapper _mapper;
        public IConfiguration _config { get; }

        public EvaluationController(DataContext context, IEducNotesRepository repo, IMapper mapper, IConfiguration config)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
            _config = config;
        }

        [HttpGet("FormData")]
        public async Task<IActionResult> GetFormData()
        {
            var periods = await _context.Periods.OrderBy(o => o.Name).ToListAsync();
            var types = await _context.EvalTypes.OrderBy(o => o.Name).ToListAsync();
            return Ok(new {
                periods,
                types
            });
        }

        [HttpGet("Periods")]
        public async Task<IActionResult> GetPeriods()
        {
            var periods = await _context.Periods.OrderBy(o => o.Name).ToListAsync();

            return Ok(periods);
        }

        [HttpGet("EvalTypes")]
        public async Task<IActionResult> GetEvalTypes()
        {
            var types = await _context.EvalTypes.OrderBy(o => o.Name).ToListAsync();

            return Ok(types);
        }

        [HttpGet("ClassEval/{id}")]
        public async Task<IActionResult> GetClassEval(int id)
        {
            var evalFromRepo = await _context.Evaluations
                                        .Include(i => i.Class)
                                        .Include(i => i.Course)
                                        .Include(i => i.EvalType)
                                        .FirstOrDefaultAsync(e => e.Id == id);
            var eval = _mapper.Map<EvalForEditDto>(evalFromRepo);

            var usersEvalFromRepo = await _context.UserEvaluations
                                    .Include(i => i.User).ThenInclude(p => p.Photos)
                                    .Where(u => u.EvaluationId == id)
                                    .OrderBy(o => o.User.LastName).ThenBy(o => o.User.FirstName)
                                    .ToListAsync();
            var usersEval = _mapper.Map<IEnumerable<ClassEvalGradesDto>>(usersEvalFromRepo);

            return Ok(new {
                eval,
                usersEval
            });
        }

        // only graded evaluations are considered here...
        [HttpGet("Class/{classId}/Course/{courseId}/Period/{periodId}")]
        public async Task<IActionResult> GetUserEvaluations(int classId, int courseId, int periodId)
        {
            var evalsFromRepo = await _context.Evaluations
                            .Include(i => i.EvalType)
                            .Where(e => e.ClassId == classId && e.CourseId == courseId &&
                                e.PeriodId == periodId && e.Graded == true)
                            .OrderBy(o => o.EvalDate)
                            .ToListAsync();

            var evals = _mapper.Map<IEnumerable<EvalsForEditDto>>(evalsFromRepo);

            var usersFromRepo = await _repo.GetClassStudents(classId);
            var users = _mapper.Map<IEnumerable<UserForDetailedDto>>(usersFromRepo);

            List<UserEvaluation> uevals = await _context.UserEvaluations
                            .Where(u => u.Evaluation.ClassId == classId && u.Evaluation.CourseId == courseId &&
                                    u.Evaluation.PeriodId == periodId && u.Evaluation.Graded == true)
                            .OrderBy(o => o.Evaluation.EvalDate).ToListAsync();

            List<UserGradesToReturnDto> usersWithGrades = new List<UserGradesToReturnDto>();

            for(int i = 0; i < users.Count(); i++)
            {
                UserGradesToReturnDto usergrades = new UserGradesToReturnDto();
                var user = users.ElementAt(i);
                int userid = user.Id;
                string userName = user.LastName + ' ' + user.FirstName;

                usergrades.UserId = userid;
                usergrades.StudentName = userName;
                usergrades.PhotoUrl = user.PhotoUrl;
                usergrades.Grades = new List<double>();
                usergrades.Comments = new List<string>();
                List<UserEvaluation> ugrades = uevals.FindAll(e => e.UserId == userid);
                for(int j = 0; j < ugrades.Count(); j++)
                {
                    UserEvaluation ugrade = ugrades[j];
                    //grades
                    double grade = ugrade.Grade == null ? -1000 : Convert.ToDouble(ugrade.Grade);
                    usergrades.Grades.Add(grade);
                    //comments
                    string comment = ugrade.Comment == null ? "" : ugrade.Comment;
                    usergrades.Comments.Add(comment);
                }
                usersWithGrades.Add(usergrades);
            }

            return Ok(new {
                userGrades = usersWithGrades,
                evals
            });
        }

        [HttpGet("Class/{classId}/CoursesWithEvals/{userId}")]
        public async Task<IActionResult> GetCoursesWithEvals(int userId, int classId)
        {
            var periods = await _context.Periods
                                .OrderBy(o => o.Abbrev)
                                .ToListAsync();

            // List<Boolean> activatedPeriods = new List<bool>();
                
            List<UserEvalsDto> coursesWithEvals = await _repo.GetUserGrades(userId, classId);

            double courseAvgSum = 0;
            double courseCoeffSum = 0;
            double GeneralAvg = -1000;

            List<PeriodAvgDto> periodAvgs = new List<PeriodAvgDto>();

            if(coursesWithEvals.Count() > 0)
            {
                foreach (var course in coursesWithEvals)
                {
                    courseAvgSum += course.UserCourseAvg * course.CourseCoeff;
                    courseCoeffSum += course.CourseCoeff;
                }

                if(courseCoeffSum > 0)
                    GeneralAvg = Math.Round(courseAvgSum / courseCoeffSum, 2);

                foreach (var period in periods)
                {

                    PeriodAvgDto pad = new PeriodAvgDto();
                    pad.PeriodId = period.Id;
                    pad.PeriodName = period.Name;
                    pad.PeriodAbbrev = period.Abbrev;
                    pad.StartDate = period.StartDate;
                    pad.EndDate = period.EndDate;
                    //set activated period depending on startDate
                    if(DateTime.Now.Date >= period.StartDate)
                    {
                        pad.activated = true;
                    }
                    else
                    {
                        pad.activated = false;
                    }
                    pad.Active = period.Active;
                    pad.Avg = -1000;

                    double periodAvgSum = 0;
                    double coeffSum = 0;
                    foreach (var course in coursesWithEvals)
                    {
                        var periodData = course.PeriodEvals.FirstOrDefault(p => p.PeriodId == period.Id);
                        if(periodData.grades != null)
                        {
                            periodAvgSum += periodData.UserCourseAvg * course.CourseCoeff;
                            coeffSum += course.CourseCoeff;
                        }
                    }

                    if(coeffSum > 0)
                        pad.Avg = Math.Round(periodAvgSum / coeffSum, 2);

                    periodAvgs.Add(pad);
                }
            }

            return Ok(new {
                StudentAvg = GeneralAvg,
                periodAvgs = periodAvgs,
                periods = periods,
                coursesWithEvals = coursesWithEvals
            });
        }

        [HttpGet("CoursesSkills")]
        public async Task<IActionResult> GetCourseSkillsWithProgElts()
        {
            var skills = await (from course in _context.Courses orderby course.Name
                                  select new
                                  {
                                    CourseId = course.Id,
                                    CourseName = course.Name,
                                    Skills = (from skill in course.CourseSkills
                                              where skill.CourseId == course.Id
                                              orderby skill.Skill.Name
                                              select new
                                              {
                                                SkillId = skill.SkillId,
                                                SkillName = skill.Skill.Name,
                                                ProgElts = (from progElt in _context.ProgramElements
                                                              where skill.SkillId == progElt.SkillId
                                                              orderby progElt.Name
                                                              select new
                                                              {
                                                                SkillName = skill.Skill.Name,
                                                                ProgEltId = progElt.Id, 
                                                                ProgEltName = progElt.Name,
                                                                Checked = false
                                                            }).ToList()
                                              }).ToList()
                                  }
                                ).ToListAsync();

            return Ok(skills);
        }

        [HttpGet("Teacher/{teacherId}/EvalsToCome")]
        public async Task<IActionResult> GetTeacherEvalsToCome(int teacherId)
        {
            var classes = await _context.ClassCourses
                                .Where(c => c.TeacherId == teacherId)
                                .Select(c => c.Class).Distinct().ToListAsync();

            var today = DateTime.Now.Date;
            List<ClassEvalForListDto> evalsToCome = new List<ClassEvalForListDto>();
            foreach (var aclass in classes)
            {
                ClassEvalForListDto cefl = new ClassEvalForListDto();
                cefl.ClassId = aclass.Id;
                cefl.ClassName = aclass.Name;

                var classEvals = await _context.Evaluations
                                .Include(i => i.Course)
                                .Include(i => i.Class)
                                .Include(i => i.EvalType)
                                .Where(e => e.UserId == teacherId && e.ClassId == aclass.Id && e.EvalDate.Date >= today)
                                .ToListAsync();
                
                cefl.Evals = _mapper.Map<List<EvaluationForListDto>>(classEvals).OrderBy(e => e.EvalDate);
                evalsToCome.Add(cefl);
            }

            List<ClassEvalForListDto> evalsToBeGraded = new List<ClassEvalForListDto>();
            foreach (var aclass in classes)
            {
                ClassEvalForListDto cefl = new ClassEvalForListDto();
                cefl.ClassId = aclass.Id;
                cefl.ClassName = aclass.Name;

                var classEvals = await _context.Evaluations
                                .Include(i => i.Course)
                                .Include(i => i.Class)
                                .Include(i => i.EvalType)
                                .Where(e => e.UserId == teacherId && e.ClassId == aclass.Id && e.EvalDate.Date <= today &&
                                    e.Closed == false).ToListAsync();
                
                cefl.Evals = _mapper.Map<List<EvaluationForListDto>>(classEvals).OrderBy(e => e.EvalDate);
                evalsToBeGraded.Add(cefl);
            }

            return Ok(new {
                evalsToCome = evalsToCome,
                evalsToBeGraded = evalsToBeGraded
            });
        }

        [HttpGet("Class/{classId}/EvalsToCome")]
        public async Task<IActionResult> GetEvalsToCome(int classId)
        {
            var evalsToReturn = await _repo.GetEvalsToCome(classId);

            return Ok(evalsToReturn);
        }

        [HttpGet("Class/{classId}/AllEvaluations")]
        public async Task<IActionResult> GetClassEvals(int classId)
        {
            var ClassEvals = await _context.Evaluations
                                    .Where(e => e.ClassId == classId)
                                    .OrderBy(o => o.EvalDate).ToListAsync();

            var OpenEvals = ClassEvals.FindAll(e => e.Closed == false);
            var ClosedEvals = ClassEvals.FindAll(e => e.Closed == true);

            return Ok(new {
                AllEvals = ClassEvals,
                OpenEvals = OpenEvals,
                ClosedEvals = ClosedEvals
            });
        }

        [HttpPut("SaveEvaluation")]
        public async Task<IActionResult> SaveEvaluation([FromBody]Evaluation eval, [FromHeader]EvalParams evalParams)
        {
            var progEltIds = evalParams.ProgEltIds;

            _repo.Add(eval);
            int evalId = eval.Id;

            //did we select skills for the evaluation?
            if(progEltIds != null)
            {
                int[] ids = Array.ConvertAll(progEltIds.Split(','), int.Parse);
                for(int i = 0; i < ids.Count(); i++)
                {
                    var eltId = ids[i];
                    EvalProgElt evalProgElt = new EvalProgElt();
                    evalProgElt.EvaluationId = evalId;
                    evalProgElt.ProgramElementId = eltId;
                    _repo.Add(evalProgElt);
                }
            }

            // add eval line for each student of the evaluation
            var students = await _repo.GetClassStudents(eval.ClassId);
            foreach (var item in students)
            {
                _repo.Add(new UserEvaluation {UserId = item.Id, EvaluationId = evalId});
            }

            var evalType = await _context.EvalTypes.FirstOrDefaultAsync(e => e.Id == eval.EvalTypeId);

            //add the event for the user timeline
            Event newEvent = new Event();
            newEvent.ClassId = eval.ClassId;
            newEvent.EventTypeId = 1; // evaluation - to be set in the appsettings or in azure ENV.
            newEvent.EventDate = eval.EvalDate;
            newEvent.EvaluationId = evalId;
            newEvent.Title = "évaluation";
            newEvent.Desc = eval.Name != "" ? eval.Name + " - " + evalType.Name: "";
            _repo.Add(newEvent);

            if(await _repo.SaveAll())
                return NoContent();

            throw new Exception($"l'ajout de l'évaluation a échoué");

        }

        [HttpPut("{evalClosed}/SaveUserGrades")]
        public async Task<IActionResult> SaveUserGrades([FromBody]List<UserEvaluation> userGrades, bool evalClosed)
        {
            if(userGrades.Count() > 0)
            {
                //get the current evaluation id
                int evalId = userGrades[0].EvaluationId;
                var currentEval = await _context.Evaluations.SingleOrDefaultAsync(e => e.Id == evalId);

                //delete previous evaluation data
                List<UserEvaluation> previousData = await _context.UserEvaluations.Where(e => e.EvaluationId == evalId).ToListAsync();
                foreach (UserEvaluation ue in previousData)
                {
                    _repo.Delete(ue);
                }

                _context.UpdateRange(userGrades);

                //send sms to subscribed parents to the service

                // eval Sms data
                int smsId = _config.GetValue<int>("AppSettings:NewEvalSms");
                var NewEvalSms = _context.SmsTemplates.FirstOrDefault(s => s.Id == smsId);

                List<double> classGrades = userGrades.Select(g => Convert.ToDouble(g.Grade)).ToList();
                double classEvalMin = classGrades.Min();
                double classEvalMax = classGrades.Max();
                double classEvalAvg = _repo.GetClassEvalAvg(userGrades, Convert.ToDouble(currentEval.MaxGrade));

                var childIds = userGrades.Select(i => i.UserId);
                var parents = _context.UserLinks.Where(u => childIds.Contains(u.UserId)).Distinct().ToList();
                List<EvalSmsDto> EvalSmsData = new List<EvalSmsDto>();
    
                foreach (var childId in childIds)
                {
                    var child = await _context.Users.FirstAsync(u => u.Id == childId);
                    List<int> parentIds = parents.Where(p => p.UserId == childId).Select(p => p.UserPId).ToList();
                    foreach (var parentId in parentIds)
                    {
                        // is the parent subscribed to the eval sms?
                        var userTemplate = await _context.UserSmsTemplates.FirstOrDefaultAsync(
                                            u => u.ParentId == parentId && u.SmsTemplateId == NewEvalSms.Id &&
                                            u.ChildId == childId);
                        
                        if(userTemplate != null)
                        {
                            var parent = await _context.Users.FirstAsync(p => p.Id == parentId);
                            EvalSmsDto esd = new EvalSmsDto();
                            esd.ChildId = childId;
                            esd.ChildFirstName = child.FirstName;
                            esd.ChildLastName = child.LastName;
                            esd.ParentId = parent.Id;
                            esd.ParentFirstName = parent.FirstName;
                            esd.ParentLastName = parent.LastName.FirstLetterToUpper();
                            esd.ParentGender = parent.Gender;
                            esd.ParentCellPhone = parent.PhoneNumber;
                            EvalSmsData.Add(esd);
                        }
                    }
                }

                //did we close the evaluation grades?
                if(currentEval != null)
                {
                    currentEval.Closed = evalClosed;
                }
                _repo.Update(currentEval);

                if(await _repo.SaveAll())
                    return NoContent();

                throw new Exception($"l'ajout des notes a échoué");
            }

            return NoContent();
        }

    }

}