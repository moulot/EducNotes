using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace EducNotes.API.Dtos
{
    public class CourseShowingDto
    {
        public string TeacherId { get; set; }
        public IFormFile MainPdf { get; set; }
        public IFormFile MainVideo { get; set; }
        public string LessonContentIds { get; set; }
        public string CourseComment { get; set; }
        public List<IFormFile> OtherFiles { get; set; }
    }
}