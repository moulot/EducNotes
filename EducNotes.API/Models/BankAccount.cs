namespace EducNotes.API.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public byte Active { get; set; }
    }
}