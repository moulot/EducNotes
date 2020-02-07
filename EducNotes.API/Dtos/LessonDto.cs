using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class LessonDto
    {
        public int Id { get; set; }
        public int? ThemeId { get; set; }
        public int? ClassLevelId { get; set; }
        public int? CourseId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public List<LessonContentDto> Contents { get; set; }
    }
}