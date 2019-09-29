namespace EducNotes.API.Models
{
    public class CourseSkill
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}