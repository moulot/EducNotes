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
      Validated = false;
      RegCreated = false;
      NextRegCreated = false;
      Created = DateTime.Now;
      Scholarship = false;
    }

    public string IdNum { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int UserTypeId { get; set; }
    public UserType UserType { get; set; }
    public int? EducLevelId { get; set; }
    public EducationLevel EducLevel { get; set; }
    public byte Gender { get; set; }
    public string SecondPhoneNumber { get; set; }
    public DateTime? ValidationDate { get; set; }
    public int? ClassLevelId { get; set; }
    public ClassLevel ClassLevel { get; set; }
    public int? ClassId { get; set; }
    public Class Class { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public string KnownAs { get; set; } 
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public int? CountryId { get; set; }
    public Country Country { get; set; }
    public int? CityId { get; set; }
    public City City { get; set; }
    public int? DistrictId { get; set; }
    public District District { get; set; }
    public int? BirthCountryId { get; set; }
    public Country BirthCountry { get; set; }
    public int? BirthCityId { get; set; }
    public City BirthCity { get; set; }
    public int? BirthDistrictId { get; set; }
    public District BirthDistrict { get; set; }
    public int? MaritalStatusId { get; set; }
    public MaritalStatus MaritalSatus { get; set; }
    public int ForgotPasswordCount { get; set; }
    public int ResetPasswordCount { get; set; }
    public byte TempData { get; set; }
    public Boolean RegCreated { get; set; }
    public Boolean NextRegCreated { get; set; }
    public byte Active { get; set; }
    public Boolean Validated { get; set; }
    public string ToBeValidatedEmail { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public Boolean AccountDataValidated { get; set; }
    public DateTime? ResetPasswordDate { get; set; }
    public DateTime? ForgotPasswordDate { get; set; }
    public Boolean Scholarship { get; set; }
    public int? NationalityId { get; set; }
    public Nationality Nationality { get; set; }
    public int? JobId { get; set; }
    public Job Job { get; set; }
    public string PostalBox { get; set; }
    public ICollection<Photo> Photos { get; set; }
    public ICollection<Like> Likers { get; set; }
    public ICollection<Like> Likees { get; set; }
    public ICollection<Message> MessagesSent { get; set; }
    public ICollection<Message> MessagesRecceived { get; set; }
  }
}
