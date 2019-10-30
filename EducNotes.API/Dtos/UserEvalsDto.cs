using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class UserEvalsDto
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseAbbrev { get; set; }
        public double CourseCoeff { get; set; }
        public int GradedOutOf { get; set; }
        public double UserCourseAvg { get; set; }
        public double ClassCourseAvg { get; set; }
        public List<GradeDto> grades { get; set; }
    }
}