using System;

namespace EducNotes.API.Dtos
{
    public class ClassIdAndNameDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public Boolean Active { get; set; }
    }
}