using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class TuitionDataDto
  {
public string FLastName { get; set; }
    public string FFirstName { get; set; }
    public string FCell { get; set; }
    public string FEmail { get; set; }
    public Boolean FActive { get; set; }
    public string MLastName { get; set; }
    public string MFirstName { get; set; }
    public string MCell { get; set; }
    public string MEmail { get; set; }
    public Boolean MActive { get; set; }
    public decimal OrderAmount { get; set; }
    public decimal DueAmount { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime Validity { get; set; }
    public List<TuitionChildDataDto> Children { get; set; }  }
}