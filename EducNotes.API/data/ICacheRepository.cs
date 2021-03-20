using System.Collections.Generic;
using System.Threading.Tasks;
using EducNotes.API.Models;

namespace EducNotes.API.data
{
  public interface ICacheRepository
  {
    Task<List<User>> GetUsers();
    Task<List<User>> GetStudents();
    Task<List<User>> GetParents();
    Task<List<User>> GetTeachers();
    Task<List<User>> LoadUsers();
    Task<List<TeacherCourse>> GetTeacherCourses();
    Task<List<TeacherCourse>> LoadTeacherCourses();
    Task<List<ClassCourse>> GetClassCourses();
    Task<List<ClassCourse>> LoadClassCourses();
    Task<List<ClassLevel>> GetClassLevels();
    Task<List<ClassLevel>> LoadClassLevels();
    Task<List<Class>> GetClasses();
    Task<List<Class>> LoadClasses();
    Task<List<EducationLevel>> GetEducLevels();
    Task<List<EducationLevel>> LoadEducLevels();
    Task<List<School>> GetSchools();
    Task<List<School>> LoadSchools();
    Task<List<Cycle>> GetCycles();
    Task<List<Cycle>> LoadCycles();
  }
}