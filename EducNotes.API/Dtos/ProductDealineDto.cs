using System;

namespace EducNotes.API.Dtos
{
  public class ProductDealineDto
  {
    public int Id { get; set; }
    public int ProductId { get; set; }
    public DateTime DueDate { get; set; }
    public string strDueDate { get; set; }
    public string DeadLineName { get; set; }
    public decimal Percentage { get; set; }
    public byte Seq { get; set; }
  }
}