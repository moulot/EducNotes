using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class ClassForAddingDto
    {
     public int LevelId { get; set; }   
     public string Name { get; set; }
     public int? suffixe { get; set; }
     public int maxStudent { get; set; }
     public int? classTypeId { get; set; }
     public int? Number { get; set; }
    // public List<int> CourseIds { get; set; }
    }
}