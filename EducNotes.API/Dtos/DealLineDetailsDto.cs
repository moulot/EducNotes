namespace EducNotes.API.Dtos {
    public class DealLineDetailsDto {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public decimal Percentage { get; set; }
        public string DueDate { get; set; }
    }
}