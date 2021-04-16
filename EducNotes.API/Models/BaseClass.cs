using System;
using System.ComponentModel.DataAnnotations;

namespace EducNotes.API.Models
{
  public class BaseClass
  {
    public BaseClass()
    {
      InsertUserId = 1;
      UpdateUserId = 1;
      InsertDate = DateTime.Now;
      UpdateDate = DateTime.Now;
      Version = Guid.NewGuid().ToString();
    }

    public int Id { get; set; }
    public DateTime InsertDate { get; set; }
    public int InsertUserId { get; set; }
    public DateTime UpdateDate { get; set; }
    public int UpdateUserId { get; set; }
    [Timestamp]
    public string Version { get; set; }
  }
}