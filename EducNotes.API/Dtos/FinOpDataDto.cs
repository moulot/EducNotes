using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class FinOpDataDto
  {
    public DateTime FinOpDate { get; set; }
    public int OrderId { get; set; }
    public int OrderLineId { get; set; }
    public int PaymentTypeId { get; set; }
    public int InvoiceId { get; set; }
    public int BankId { get; set; }
    public decimal Amount { get; set; }
    public string numCheque { get; set; }
    public string RefDoc { get; set; }
    public string Note { get; set; }
    public List<PaymentDto> Payments { get; set; }
  }
}