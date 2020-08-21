namespace EducNotes.API.Models
{
  public class PaymentType
  {
    public PaymentType()
    {
      DsplSeq = 0;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public byte DsplSeq { get; set; }
  }
}