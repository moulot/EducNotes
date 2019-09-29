using System.Collections.Generic;

namespace EducNotes.API.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProgramElement> ProgElts { get; set; }
    }
}