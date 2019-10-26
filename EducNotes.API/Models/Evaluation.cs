using System;
using System.Collections.Generic;

namespace EducNotes.API.Models
{
  public class Evaluation
  {
    public Evaluation()
    {
      Closed = false;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int ClassId { get; set; }
    public Class Class { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public int PeriodId { get; set; }
    public Period Period { get; set; }
    public int EvalTypeId { get; set; }
    public EvalType EvalType { get; set; }
    public DateTime EvalDate { get; set; }
    public Boolean Graded { get; set; }
    public string MinGrade { get; set; }
    public string MaxGrade { get; set; }
    public double Coeff { get; set; }
    public Boolean Significant { get; set; }
    public Boolean CanBeNegative { get; set; }
    public Boolean GradeInLetter { get; set; }
    public bool Closed { get; set; }
    public ICollection<EvalProgElt> EvalProgElts { get; set; }
  }
}