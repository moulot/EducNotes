using System;


namespace EducNotes.API.Dtos
{
    public class SelfRegisterDto
    {
        public string Email { get; set; }
        public int UserTypeId { get; set; }
        public int[] CourseIds { get; set; }
        public int? TotalChild { get; set; }
    }
}