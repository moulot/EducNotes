
namespace EducNotes.API.Dtos
{
    public class ClassForSaveDto
    {
     public int LevelId { get; set; }   
     public string Name { get; set; }
     public int? suffixe { get; set; }
     public int maxStudent { get; set; }
     public int? classTypeId { get; set; }
     public int? NbClass { get; set; }
    }
}