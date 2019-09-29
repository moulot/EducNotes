using System;

namespace EducNotes.API.Dtos
{
    public class AgendaForSaveDto
    {
      public int Id { get; set; }
        public int ClassId { get; set; }
        public int CourseId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DueDate { get; set; }
        public string TaskDesc { get; set; }
    }
}