using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class ClassCoursesDto
    {
        public Class Class { get; set; }
        public List<Course> Courses { get; set; }
    }
}