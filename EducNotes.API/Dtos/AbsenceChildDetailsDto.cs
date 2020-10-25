using System;

namespace EducNotes.API.Dtos
{
  public class AbsenceChildDetailsDto
  {
    public string CourseName { get; set; }
    public string CourseAbbrev { get; set; }
    public string DoneBy { get; set; }
    public int AbsenceTypeId { get; set; }
    public string AbsenceType { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public double LateMins { get; set; }
    public Boolean Justified { get; set; }
    public string Reason { get; set; }
    public string Comment { get; set; }
  }
}