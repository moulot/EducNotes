using System;

namespace EducNotes.API.Models
{
  public class Agenda
  {
    public Agenda()
    {
        Done = false;
    }
    public int Id { get; set; }
    public int SessionId { get; set; }
    public Session Session { get; set; }
    public DateTime DateAdded { get; set; }
    public string TaskDesc { get; set; }        
    public bool Done { get; set; }
    public int? DoneSetById { get; set; }
    public User DoneSetBy { get; set; }
  }
}