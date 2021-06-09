using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ProductForConfigDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Comment { get; set; }
    public int ProductTypeId { get; set; }
    public string TypeName { get; set; }
    public decimal Price { get; set; }
    public Boolean IsPaidCash { get; set; }
    public Boolean IsByLevel { get; set; }
    public Boolean IsPeriodic { get; set; }
    public string PeriodicityName { get; set; }
    public int PayableAtId { get; set; }
    public string PayableAtName { get; set; }
    public int PayableAtDayCount { get; set; }
    public Boolean IsRequired { get; set; }
    public Boolean Active { get; set; }
    public byte DspSeq { get; set; }
    public List<ProductDeadlineDto> DueDates { get; set; }
  }
}