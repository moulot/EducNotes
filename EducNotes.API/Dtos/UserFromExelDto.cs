using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class UserFromExelDto
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int UserTypeId { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public List<string> Courses { get; set; }

        public UserFromExelDto()
        {
            {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
        }
    }
}