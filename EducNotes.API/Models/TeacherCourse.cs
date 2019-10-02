namespace EducNotes.API.Models
{
    public class TeacherCourse
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public User Teacher { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }

    }
}