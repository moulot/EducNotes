using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class ThemeDto
    {
        public int Id { get; set; }
        public  int ClassLevelId { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public List<LessonDto> Lessons { get; set; }
    }
}