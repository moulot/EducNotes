namespace EducNotes.API.Models
{
    public class ClassCourse
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public Class Class { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int TeacherId { get; set; }
        public User Teacher {get; set; }
    }
}