using System;
using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class MenuItemDto
  {
    public int Id { get; set; }
    public int ParentMenuId { get; set; }
    public MenuItem ParentMenu { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Url { get; set; }
    public Boolean IsAlwaysEnabled { get; set; }
    public byte DsplSeq { get; set; }
    public List<MenuItem> ChildMenuItems { get; set; }
  }
}