using System.Collections.Generic;

namespace EducNotes.API.Models
{
    public class Class
    {
        public int Id { get; set; }
        public int ClassLevelId { get; set; }
        public ClassLevel ClassLevel { get; set; }
        public int? ClassTypeId { get; set; }
        public ClassType ClassType { get; set; }
        public int? MainTeacherId { get; set; }
        public string Name { get; set; }
        public byte Active { get; set; }
        public int MaxStudent { get; set; }
        // public ICollection<User> Students { get; set; }
    }
}