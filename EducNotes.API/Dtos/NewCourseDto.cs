using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class NewCourseDto
    {
        public string name { get; set; }
        public string abbreviation { get; set; }
        public List<string> classLevelIds { get; set; }
    }
}