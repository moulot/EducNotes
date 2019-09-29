namespace EducNotes.API.Models
{
    public class ProgramElement
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
        public string Name { get; set; }
    }
}