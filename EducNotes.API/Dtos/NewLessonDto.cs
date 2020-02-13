using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class NewLessonDto
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public int DsplSeq { get; set; }
        public List<NewLessonContentDto> contents { get; set; }
    }
}