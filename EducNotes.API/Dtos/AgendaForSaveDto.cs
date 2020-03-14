using System;

namespace EducNotes.API.Dtos
{
    public class AgendaForSaveDto
    {
      public int Id { get; set; }
      public int SessionId { get; set; }
      public DateTime DateAdded { get; set; }
      public string TaskDesc { get; set; }
    }
}