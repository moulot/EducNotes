using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class AgendaForListDto
    {
        public DateTime DueDate { get; set; }
        public string ShortDueDate { get; set; }
        public string LongDueDate { get; set; }
        public string DueDateAbbrev { get; set; }
        public int NbItems { get; set; }
        public List<AgendaItemDto> AgendaItems { get; set; }
    }
}