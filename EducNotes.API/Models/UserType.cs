using System.Collections.Generic;

namespace EducNotes.API.Models
{
    public class UserType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
    }
}