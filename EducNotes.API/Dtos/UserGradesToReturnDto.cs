using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class UserGradesToReturnDto
    {
        public int UserId { get; set; }
        public string PhotoUrl { get; set; }
        public int Age { get; set; }
        public string StudentName { get; set; }
        public List<decimal> Grades { get; set; }
        public List<string> Comments { get; set; }
    }
}