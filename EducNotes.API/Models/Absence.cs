using System;

namespace EducNotes.API.Models
{
  public class Absence
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int DoneById { get; set; }
    public User DoneBy { get; set; }
    public int AbsenceTypeId { get; set; }
    public AbsenceType AbsenceType { get; set; }
    public int? SessionId { get; set; }
    public Session Session { get; set; }
    public int PeriodId { get; set; }
    public Period Period { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Boolean Justified { get; set; }
    public string Reason { get; set; }
    public string Comment { get; set; }
  }
}