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
        public int GradedOutOf { get; set; }
        public double UserCourseAvg { get; set; }
        public double ClassCourseAvg { get; set; }
        public List<GradeDto> grades { get; set; }
    }

    public class GradeDto
    {
        public string EvalDate { get; set; }
        public string EvalType { get; set; }
        public string EvalName { get; set; }
        public double Grade { get; set; }
        public double GradeMax { get; set; }
        public double Coeff { get; set; }
        public double ClassGradeMin { get; set; }
        public double ClassGradeMax { get; set; }
    }
}