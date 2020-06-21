using System;

namespace EducNotes.API.Dtos
{
    public class ChildForAccountDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Boolean EmailConfirmed { get; set; }
        public string Email { get; set; }
        public int ClassLevelId { get; set; }
        public string ClassLevelName { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public byte Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string strDateOfBirth { get; set; }
        public int Age { get; set; }
        public string PhotoUrl { get; set; }
          public string PhoneNumber { get; set; }
        public string SecondPhoneNumber { get; set; }
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }
    }
}