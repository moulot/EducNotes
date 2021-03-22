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
    Task<List<Course>> GetCourses();
    Task<List<Course>> LoadCourses();
    Task<List<ClassType>> GetClassTypes();
    Task<List<ClassType>> LoadClassTypes();
    Task<List<ClassLevelClassType>> GetCLClassTypes();
    Task<List<ClassLevelClassType>> LoadCLClassTypes();
    Task<List<EmailTemplate>> GetEmailTemplates();
    Task<List<EmailTemplate>> LoadEmailTemplates();
    Task<List<SmsTemplate>> GetSmsTemplates();
    Task<List<SmsTemplate>> LoadSmsTemplates();
    Task<List<Setting>> GetSettings();
    Task<List<Setting>> LoadSettings();
    Task<List<Token>> GetTokens();
    Task<List<Token>> LoadTokens();
  }
}