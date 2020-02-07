using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class ClassWithThemesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ThemeDto> Themes { get; set; }
        public List<LessonDto> Lessons { get; set; }
    }
}