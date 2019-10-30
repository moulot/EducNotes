namespace EducNotes.API.Dtos {
    public class CoefficientDetailsDto {
        public int Id { get; set; }
        public int ClassLevelid { get; set; }
        public string ClassLevelName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int? ClassTypeId { get; set; }
        public string ClassTypeName { get; set; }
        public double Coefficient { get; set; }
    }
}