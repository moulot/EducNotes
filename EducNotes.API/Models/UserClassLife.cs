using System;

namespace EducNotes.API.Models
{
  public class UserClassLife
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int ClassLifeId { get; set; }
    public ClassLife ClassLife { get; set; }
    public int DoneById { get; set; }
    public User DoneBy { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Boolean Justified { get; set; }
    public string Reason { get; set; }
    public string Comment { get; set; }
  }
}