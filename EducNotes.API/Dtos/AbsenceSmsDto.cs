using System;

namespace EducNotes.API.Dtos
{
    public class AbsenceSmsDto
    {
        public int ChildId { get; set; }
        public string ChildFirstName { get; set; }
        public string ChildLastName { get; set; }
        public int ParentId { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public byte ParentGender { get; set; }
        public string SessionDate { get; set; }
        public string CourseName { get; set; }
        public string CourseStartHour { get; set; }
        public string CourseEndHour { get; set; }
        public string ParentCellPhone { get; set; }
    }
}