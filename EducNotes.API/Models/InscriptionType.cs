using System.Collections.Generic;

namespace EducNotes.API.Models
{
    public class InscriptionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Inscription> Inscriptions { get; set; }
    }
}