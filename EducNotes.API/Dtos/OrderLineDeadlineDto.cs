using System;

namespace EducNotes.API.Dtos
{
  public class OrderLineDeadlineDto
  {
    public int Id { get; set; }
    public int OrderLineId { get; set; }
    public decimal Percent { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string strDueDate { get; set; }
    public string DeadlineName { get; set; }
    public string Comment { get; set; }
    public byte Seq { get; set; }
  }
}