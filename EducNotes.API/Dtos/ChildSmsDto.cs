using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class ChildSmsDto
    {
        public ChildForAccountDto Child { get; set; }
        public List<SmsByCategoryDto> SmsByCategory { get; set; }
    }
}