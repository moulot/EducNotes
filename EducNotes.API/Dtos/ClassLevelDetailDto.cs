using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class ClassLevelDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalEnrolled { get; set; }
        public int TotalStudents { get; set; }
        public int TotalClasses { get; set; }
        public List<Class> Classes { get; set; }
        public int TotalValdated { get; set; }
        public int AvailableClasses { get; set; }
        public int AvailablePlaces { get; set; }
    }
}