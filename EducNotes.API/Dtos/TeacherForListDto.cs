using System;
using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class TeacherForListDto
  {
    public int Id { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public byte Gender { get; set; }
    public int EducLevelId { get; set; }
    public string EducLevelName { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public string Email { get; set; }
    public string PhotoUrl { get; set; }
    public string PhoneNumber { get; set; }
    public string  DateOfBirth { get; set; }
    public string SecondPhoneNumber { get; set; }
    public Boolean Validated { get; set; }
    public List<TeacherCourseClassesDto> CourseClasses { get; set; }
    public List<Course> Courses { get; set; }
    public List<int> classIds { get; set; }
    public Course Course { get; set; }
  }
}