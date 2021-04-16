using System;

namespace EducNotes.API.Models
{
  public class MenuItem : BaseEntity
  {
    public MenuItem()
    {
      IsAlwaysEnabled = false;
    }

    public int ParentMenuId { get; set; }
    public MenuItem ParentMenu { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Url { get; set; }
    public Boolean IsAlwaysEnabled { get; set; }
  }
}