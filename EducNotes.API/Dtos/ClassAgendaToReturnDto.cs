using System;
using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class ClassAgendaToReturnDto
    {
        public List<CourseTask> Courses { get; set; }
        public DateTime dtDueDate { get; set; }
        public string DueDate { get; set; }
        public int DueDateDay { get; set; }
        public int DayInt { get; set; }
        public int NbTasks { get; set; }
     }

    public class CourseTask
    {
        public string   CourseName { get; set; }
        public string CourseColor { get; set; }
        public string DateAdded { get; set; }
        public string ShortDateAdded { get; set; }
         public string TaskDesc { get; set; }  
    }
}