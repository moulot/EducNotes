using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class ClassWithThemesDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseAbbrev { get; set; }
        public List<ThemeDto> Themes { get; set; }
        public List<LessonDto> Lessons { get; set; }
    }
}