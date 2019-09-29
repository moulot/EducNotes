using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class ClassDetailDto
    {
      public int Id { get; set; }
        public string Name { get; set; }
        public int MaxStudent { get; set; }
        public int TotalStudent { get; set; }
    }
}