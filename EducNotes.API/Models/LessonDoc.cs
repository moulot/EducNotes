using System;

namespace EducNotes.API.Models
{
    public class LessonDoc
    {
        public LessonDoc()
        {
            InsertDate = DateTime.Now;
        }
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public User Teacher { get; set; }
        public string Comment { get; set; }
        public DateTime InsertDate { get; set; }
    }
}