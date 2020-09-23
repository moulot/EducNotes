using System;

namespace EducNotes.API.Dtos
{
    public class NextDueAmountDto
    {
      public decimal DueAmount { get; set; }
      public DateTime Deadline { get; set; }
    }
}