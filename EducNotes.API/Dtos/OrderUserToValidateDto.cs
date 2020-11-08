using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class OrderUserToValidateDto
  {
    public int Id { get; set; }
    public int OrderNum { get; set; }
    public string strOrderDate { get; set; }
    public string strDeadline { get; set; }
    public string strValidity { get; set; }
    public int NbDaysLate { get; set; }
    public string strAmountToValidate { get; set; }
    public string strAmountTTC { get; set; }
    public decimal DownPayment { get; set; }
    public byte Status { get; set; }
    public Boolean Validated { get; set; }
    public Boolean Overdue { get; set; }
    public Boolean Paid { get; set; }
    public Boolean Completed { get; set; }
    public byte NbChildren { get; set; }
    public List<LineUserToValidateDto> Lines { get; set; }
  }
}