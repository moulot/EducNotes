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
            usergrades.Age = userdata.DateOfBirth.CalculateAge();
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
            var courses = await (from course in _context.Courses 
                                        
                                    orderby course.Name
                                    select new
                                    {
                                        CourseId = course.Id,
                                        CourseName = course.Name,
                                        UserEvals = _context.UserEvaluations
                                            .Include(i => i.Evaluation)
                                            .OrderBy(o => o.Evaluation.EvalDate)
                                            .Where(e => e.UserId == userId &&
                                                e.Evaluation.GradeInLetter == false &&
                                                e.Evaluation.CourseId == course.Id &&
                                                e.Evaluation.Graded == true).ToList(),
                                        ClassEvals = 0
                                    }).ToListAsync();

            return Ok(courses);
        }

        private UserEvalsDto GetUserEvals(int userId, int courseId)
        {
            var userEvals = _context.UserEvaluations
                            .Include(i => i.Evaluation)
                            .OrderBy(o => o.Evaluation.EvalDate)
                            .Where(e => e.UserId == userId &&
                                e.Evaluation.GradeInLetter == false &&
                                e.Evaluation.CourseId == courseId &&
                                e.Evaluation.Graded == true).ToList();

            List<double> grades = new List<double>();
            double gradesSum = 0;
            double coeffSum = 0;
            for (int i = 0; i < userEvals.Count(); i++)
            {
                var ue = userEvals[i];
                if(ue.Grade.IsNumeric())
                {
                    double maxGrade = Convert.ToDouble(ue.Evaluation.MaxGrade);
                    double grade = Convert.ToDouble(ue.Grade);
                    // grade are ajusted to 20 as MAx. Avg is on 20
                    double ajustedGrade = 20 * grade / maxGrade;
                    double coeff = ue.Evaluation.Coeff;
                    gradesSum += grade * coeff;
                    coeffSum += coeff;
                    grades.Add(grade);
                }
            }

            double gradesAvg = gradesSum / coeffSum;
            double gradeMin = grades.Min();
            double gradeMax = grades.Max();

            UserEvalsDto userEvalsDto = new UserEvalsDto();
            userEvalsDto.GradedOutOf = 20;
            userEvalsDto.grades = grades;
            userEvalsDto.Avg = gradesAvg;
            userEvalsDto.MinGrade = gradeMin;
            userEvalsDto.MaxGrade = gradeMax;

            return userEvalsDto;
        }

        private async Task<IActionResult> GetClassEvals(int classId, int courseId)
        {
            var classEvals = await _context.UserEvaluations
                            .Include(i => i.Evaluation)
                            .OrderBy(o => o.Evaluation.EvalDate)
                            .Where(e => e.Evaluation.ClassId == classId &&
                                e.Evaluation.GradeInLetter == false &&
                                e.Evaluation.CourseId == courseId &&
                                e.Evaluation.Graded == true).ToListAsync();

            List<double> grades = new List<double>();
            double gradesSum = 0;
            double coeffSum = 0;
            for (int i = 0; i < classEvals.Count(); i++)
            {
                var ue = classEvals[i];
                if(ue.Grade.IsNumeric())
                {
                    double grade = Convert.ToDouble(ue.Grade);
                    double coeff = ue.Evaluation.Coeff;
                    gradesSum += grade * coeff;
                    coeffSum += coeff;
                    grades.Add(grade);
                }
            }

            double gradesAvg = gradesSum / coeffSum;
            double gradeMin = grades.Min();
            double gradeMax = grades.Max();

            return Ok(new {
                UserEvals = grades,
                GradeAvg = gradesAvg,
                GradeMin = gradeMin,
                GradeMax = gradeMax
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

            var OpenEvals = ClassEvals.FindAll(e => e.Closed == 0);
            var ClosedEvals = ClassEvals.FindAll(e => e.Closed == 1);

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

            if(await _repo.SaveAll())
                return NoContent();

            throw new Exception($"l'ajout de l'évaluation a échoué");

        }

        [HttpPut("{evalClosed}/SaveUserGrades")]
        public async Task<IActionResult> SaveUserGrades([FromBody]List<UserEvaluation> userGrades, int evalClosed)
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
                    if(evalClosed == 1)
                        evalToBeClosed.Closed = 1;
                    else
                        evalToBeClosed.Closed = 0;
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