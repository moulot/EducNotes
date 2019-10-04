using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class UserEvalsDto
    {
        public int GradedOutOf { get; set; }
        public List<double> grades { get; set; }
        public double Avg { get; set; }
        public double MinGrade { get; set; }
        public double MaxGrade { get; set; }
    }
}