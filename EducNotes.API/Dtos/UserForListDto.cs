using System;

namespace EducNotes.API.Dtos
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte Gender { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }
          public string PhoneNumber { get; set; }
        public string SecondPhoneNumber { get; set; }
        public string Email { get; set; }
        public string EmailConfirmed { get; set; }
        public bool ValidatedCode { get; set; }


    }
}