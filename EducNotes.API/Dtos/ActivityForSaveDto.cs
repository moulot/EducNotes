using System;

namespace EducNotes.API.Dtos
{
  public class ActivityForSaveDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Abbrev { get; set; }
    public Boolean ToBeDeleted { get; set; }
  }
}