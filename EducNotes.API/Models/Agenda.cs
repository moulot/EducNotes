using System;

namespace EducNotes.API.Models
{
    public class Agenda
    {
        public Agenda()
        {
            Done = false;
        }
        public int Id { get; set; }
        public int ClassId { get; set; }
        public Class Class { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DueDate { get; set; }
        public string TaskDesc { get; set; }        
        public bool Done { get; set; }
        public int DoneSetById { get; set; }
        public User DoneSetBy { get; set; }
    }
}