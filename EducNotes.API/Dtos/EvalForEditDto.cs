using System;
using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class EvalForEditDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string CourseName { get; set; }
        public string PeriodName { get; set; }
        public string EvalTypeName { get; set; }
        public string EvalDate { get; set; }
        public Boolean EvalDateExpired { get; set; }
        public Boolean Graded { get; set; }
        public string MaxGrade { get; set; }
        public double Coeff { get; set; }
        public Boolean Significant { get; set; }
        public Boolean CanBeNegative { get; set; }
        public Boolean GradeInLetter { get; set; }
        public bool Closed { get; set; }
        public ICollection<EvalProgElt> EvalProgElts { get; set; }
    }
}