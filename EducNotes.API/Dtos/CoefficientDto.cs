namespace EducNotes.API.Dtos
{
    public class CoefficientDto
    {
        public int ClassLevelId { get; set; }
        public int COurseId { get; set; }
        public int? ClassTypeId { get; set; }
        public int coefficient { get; set; }
    }
}