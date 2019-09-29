using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class TeacherCourseClassesDto
    {
          public int TeacherId { get; set; }
        public Course Course { get; set; }
        public List<Class> Classes { get; set; }
    }
}