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
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
       public string PhoneNumber { get; set; }
       public string DateOfBirth { get; set; }
       public string SeconPhoneNumber { get; set; }
       public Course Course { get; set; }
        public List<TeacherCourseClassesDto> CourseClasses { get; set; }
    }

}