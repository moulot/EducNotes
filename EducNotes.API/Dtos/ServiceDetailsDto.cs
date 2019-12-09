namespace EducNotes.API.Dtos
{
    public class ServiceDetailsDto
    {
        public int Id { get; set; }
        public string Details { get; set; }
        public decimal Price { get; set; }
        public bool IsRequired { get; set; }
    }
}