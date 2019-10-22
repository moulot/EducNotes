using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class CourseWithAgendaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbrev { get; set; }
        public string Color { get; set; }
        public int NbItems { get; set; }
        public List<AgendaItemDto> AgendaItems { get; set; }
    }
}