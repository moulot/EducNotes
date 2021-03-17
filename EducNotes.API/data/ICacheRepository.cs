using System.Collections.Generic;
using System.Threading.Tasks;
using EducNotes.API.Models;

namespace EducNotes.API.data
{
  public interface ICacheRepository
  {
    Task<List<User>> GetUsers();
    Task<List<User>> LoadUsers();
    Task<List<TeacherCourse>> GetTeacherCourses();
    Task<List<TeacherCourse>> LoadTeacherCourses();
    Task<List<ClassCourse>> GetClassCourses();
    Task<List<ClassCourse>> LoadClassCourses();
  }
}