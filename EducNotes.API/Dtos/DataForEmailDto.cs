using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class DataForEmailDto
    {
        public List<int> UserTypeIds { get; set; }
        public List<int> ClassLevelIds { get; set; }
        public List<int> ClassIds { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}