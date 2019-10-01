using System.Collections.Generic;

namespace EducNotes.API.Models
{
    public class ClassLevel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte DsplSeq { get; set; }
        public ICollection<Inscription> Inscriptions { get; set; }
        public ICollection<Class> Classes { get; set; }
    }
}