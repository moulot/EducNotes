using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class ClassForUpdateDto
    {
        public Class Class { get; set; }
        public List<int> CourseIds { get; set; }
    }
}