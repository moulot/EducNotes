namespace EducNotes.API.Models
{
  public class Cheque
  {
    public int Id { get; set; }
    public int ChequeNum { get; set; }
    public decimal Amount { get; set; }
    public string PictureUrl { get; set; }
  }
}