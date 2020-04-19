using System;

namespace EducNotes.API.Dtos
{
  public class EventDto
  {
    public DateTime EventDate { get; set; }
    public string strEventDate { get; set; }
    public string Title { get; set; }
    public string Desc { get; set; }
    public string ChildFirstName { get; set; }
    public string ChildLastName { get; set; }
    public string ChildInitials { get; set; }
    public string ClassName { get; set; }

  }
}