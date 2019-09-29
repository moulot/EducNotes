using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class StudentDetailsDto
    {
        public UserForListDto Student { get; set; }
        public List<UserForListDto> Parents { get; set; }
        public List<Course> Courses { get; set; }
    }
}