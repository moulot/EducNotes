using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class CoursesTeacherDto
    {
        public Course Course { get; set; }
        public List<UserForDetailedDto> Teachers { get; set; }
    }
}