using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class SmsByCategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<UserSmsTemplateDto> UserSmsTemplates { get; set; }
    }
}