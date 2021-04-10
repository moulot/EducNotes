using System;

namespace EducNotes.API.Models
{
  public class BaseClass
  {
    public DateTime InsertDate { get; set; }
    public int InsertUserId { get; set; }
    public DateTime UpdateDate { get; set; }
    public int UpdateUserId { get; set; }
  }
}