namespace EducNotes.API.Models
{
    public class CourseCoefficient
    {
        public int Id { get; set; }
        public int ClassLevelid { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int? ClassTypeId { get; set; }
        public ClassType ClassType { get; set; }
        public double Coefficient { get; set; }
    }
}