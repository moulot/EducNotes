using System;

namespace EducNotes.API.Models
{
    public class ClassCourseProgress
    {
      public ClassCourseProgress()
      {
        Active = false;
        Completed = false;
      }
        public int Id { get; set; }
        public int ClassId { get; set; }
        public Class Class { get; set; }
        public int? ThemeId { get; set; }
        public Theme Theme { get; set; }
        public int? LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Boolean Active { get; set; }
        public Boolean Completed { get; set; }
    }
}