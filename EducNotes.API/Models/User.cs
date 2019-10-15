using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace EducNotes.API.Models
{
  public class User : IdentityUser<int>
    {
        public User()
        {
            Active = 1;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserTypeId { get; set; }
        public UserType UserType { get; set; }
        public byte Gender { get; set; }
        public int? DistrictId { get; set; }
        public District District { get; set; }
        public string SecondPhoneNumber { get; set; }
        public string ValidationCode { get; set; }
        public DateTime? ValidationDate { get; set; }
        public bool ValidatedCode { get; set; }
        public int? ClassId { get; set; }
        public Class Class { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public string KnownAs { get; set; } 
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string Country { get; set; }
        public string Cell { get; set; }
        public int ForgotPasswordCount { get; set; }
        public int ResetPasswordCount { get; set; }
        public int? CityId { get; set; }
        public City City { get; set; }
        public byte Active { get; set; }
        public DateTime? ResetPasswordDate { get; set; }
        public DateTime? ForgotPasswordDate { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<Like> Likers { get; set; }
        public ICollection<Like> Likees { get; set; }
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesRecceived { get; set; }
    }
}
