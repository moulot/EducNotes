using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class NewThemeDto
    {
        public int ClassLevelId { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public List<NewLessonDto> lessons { get; set; }
    }
}