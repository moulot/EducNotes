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

        public EvaluationController(DataContext context, IEducNotesRepository repo, IMapper mapper)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
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

          var evalsForEditDto = _mapper.Map<IEnumerable<EvalsForEditDto>>(evalsFromRepo);

          var usersFromRepo = await _repo.GetClassStudents(classId);
          var users = _mapper.Map<IEnumerable<UserForDetailedDto>>(usersFromRepo);

          List<UserEvaluation> uevals = await _context.UserEvaluations
                          .Where(u => u.Evaluation.ClassId == classId && u.Evaluation.CourseId == courseId &&
                                  u.Evaluation.PeriodId == periodId && u.Evaluation.Graded == true)
                          .OrderBy(o => o.Evaluation.EvalDate).ToListAsync();

          List<UserGradesToReturnDto> usersWithGrades = new List<UserGradesToReturnDto>();

          for (int i = 0; i < users.Count(); i++)
          {
            UserGradesToReturnDto usergrades = new UserGradesToReturnDto();
            var user = users.ElementAt(i);
            int userid = user.Id;
            var userdata = await _repo.GetUser(userid, false);
            string userName = userdata.LastName + ' ' + userdata.FirstName;

            usergrades.UserId = userid;                
            usergrades.StudentName = userName;
            // usergrades.Age = userdata.DateOfBirth.CalculateAge();
            IEnumerable<Photo> photoUrls = _context.Photos.Where(p => p.UserId == userid);
            string photoUrl = "";
            if(photoUrls.Count() > 0)
                photoUrl = photoUrls.SingleOrDefault(s => s.IsMain == true).Url;
            usergrades.PhotoUrl = photoUrl;
            usergrades.Grades = new List<string>();
            usergrades.Comments = new List<string>();
            List<UserEvaluation> ugrades = uevals.FindAll(e => e.UserId == userid);
            for(int j = 0; j < ugrades.Count(); j++)
            {
                UserEvaluation ugrade = ugrades[j];
                //grades
                string grade = ugrade.Grade == null ? "" : ugrade.Grade;
                usergrades.Grades.Add(grade);
                //comments
                string comment = ugrade.Comment == null ? "" : ugrade.Comment;
                usergrades.Comments.Add(comment);
            }
            usersWithGrades.Add(usergrades);
          }

          return Ok(new {
              userGrades = usersWithGrades,
              evals = evalsForEditDto
          });
        }

        [HttpGet("Class/{classId}/CoursesWithEvals/{userId}")]
        public async Task<IActionResult> GetCoursesWithEvals(int userId, int classId)
        {
            double courseAvgSum = 0;
            double courseCoeffSum = 0;
            double GeneralAvg = -1000;

            List<UserEvalsDto> coursesWithEvals = await _repo.GetUserGrades(userId, classId);

            if(coursesWithEvals.Count() > 0)
            {
                foreach (var course in coursesWithEvals)
                {
                    courseAvgSum += course.UserCourseAvg * course.CourseCoeff;
                    courseCoeffSum += course.CourseCoeff;
                }

                if(courseCoeffSum > 0)
                    GeneralAvg = Math.Round(courseAvgSum / courseCoeffSum, 2);
            }

            return Ok(new {
                StudentAvg = GeneralAvg,
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
                                                    select new
                                                    {
                                                        SkillId = skill.SkillId,
                                                        SkillName = skill.Skill.Name,
                                                        ProgElts = (from progElt in _context.ProgramElements
                                                                    where skill.SkillId == progElt.SkillId
                                                                    select new
                                                                    {
                                                                        SkillName = skill.Skill.Name,
                                                                        ProgEltId = progElt.Id, 
                                                                        ProgEltName = progElt.Name,
                                                                        Checked = false
                                                                    }).ToList()
                                                    }).ToList()
                                    }).ToListAsync();

            return Ok(skills);
        }

        [HttpGet("Teacher/{teacherId}/EvalsToCome")]
        public async Task<IActionResult> GetTeacherEvalsToCome(int teacherId)
        {
            var classIds = await _context.ClassCourses
                            .Where(c => c.TeacherId == teacherId)
                            .Select(c => c.ClassId).Distinct().ToListAsync();

            var today = DateTime.Now.Date;
            List<Evaluation> nextEvals = new List<Evaluation>();
            foreach (var classId in classIds)
            {
                var classEvals = await _context.Evaluations
                                .Include(i => i.Course)
                                .Include(i => i.Class)
                                .Include(i => i.EvalType)
                                .Where(e => e.UserId == teacherId && e.ClassId == classId && e.EvalDate.Date >= today)
                                .ToListAsync();
                
                foreach (var eval in classEvals)
                {
                    nextEvals.Add(eval);
                }
            }

            var evalsToCome = _mapper.Map<List<EvaluationForListDto>>(nextEvals).OrderBy(e => e.EvalDate);

            List<Evaluation> prevEvals = new List<Evaluation>();
            foreach (var classId in classIds)
            {
                var classEvals = await _context.Evaluations
                                .Include(i => i.Course)
                                .Include(i => i.Class)
                                .Include(i => i.EvalType)
                                .Where(e => e.UserId == teacherId && e.ClassId == classId && e.EvalDate.Date <= today &&
                                    e.Closed == false).ToListAsync();
                
                foreach (var eval in classEvals)
                {
                    prevEvals.Add(eval);
                }
            }

            var evalsToBeGraded = _mapper.Map<List<EvaluationForListDto>>(prevEvals).OrderBy(e => e.EvalDate);

            return Ok(new {
                evalsToCome = evalsToCome,
                evalsToBeGraded = evalsToBeGraded
            });
        }

        [HttpGet("Class/{classId}/EvalsToCome")]
        public async Task<IActionResult> GetEvalsToCome(int classId)
        {
            var today = DateTime.Now.Date;
            var evals = await _context.Evaluations
                        .Include(i => i.Course)
                        .Include(i => i.EvalType)
                        .Where(e => e.ClassId == classId && e.EvalDate.Date >= today).ToListAsync();

            var evalsToReturn = _mapper.Map<List<EvaluationForListDto>>(evals);

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
                //delete previous evaluation data
                var previousData = _context.UserEvaluations.Where(e => e.EvaluationId == evalId);
                foreach (UserEvaluation ue in previousData)
                {
                    _repo.Delete(ue);
                }

                foreach (UserEvaluation ue in userGrades)
                {
                    _repo.Update(ue);
                }

                //did we close the evaluation grades?
                var evalToBeClosed = await _context.Evaluations.SingleOrDefaultAsync(e => e.Id == evalId);
                if(evalToBeClosed != null)
                {
                    evalToBeClosed.Closed = evalClosed;
                }
                _repo.Update(evalToBeClosed);

                if(await _repo.SaveAll())
                    return NoContent();

                throw new Exception($"l'ajout des notes a échoué");
            }

            return NoContent();
        }

    }

}