using System;
using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class UserForDetailedDto
    {
        private ICollection<Photo> photos;

        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public byte Gender { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        // public DateTime Created { get; set; }
        // public DateTime LastActive { get; set; }
        // public string Introduction { get; set; }
        // public string LookingFor { get; set; }
        // public string Interests { get; set; }
        // public string City { get; set; }
        // public string Country { get; set; }
        public string PhotoUrl { get; set; }
          public string PhoneNumber { get; set; }
        public string SecondPhoneNumber { get; set; }
        public string userTypeName { get; set; }
        public int userTypeId { get; set; }
        public ICollection<PhotosForDetailedDto> Photos { get; set; }
    }
}