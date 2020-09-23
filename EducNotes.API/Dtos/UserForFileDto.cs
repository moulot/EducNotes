using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class UserForFileDto
  {
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int OrderLineId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string idNum { get; set; }
    public string Email { get; set; }
    public int ClassLevelId { get; set; }
    public string ClassLevelName { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public byte Gender { get; set; }
    public string strDateOfBirth { get; set; }
    public int Age { get; set; }
    public string PhotoUrl { get; set; }
    public string PhoneNumber { get; set; }
    public int FatherId { get; set; }
    public string FatherLastName { get; set; }
    public string FatherFirstName { get; set; }
    public string FatherEmail { get; set; }
    public string FatherPhoneNumber { get; set; }
    public string Father2ndPhoneNumber { get; set; }
    public string FatherPhotoUrl { get; set; }
    public int MotherId { get; set; }
    public string MotherLastName { get; set; }
    public string MotherFirstName { get; set; }
    public string MotherEmail { get; set; }
    public string MotherPhoneNumber { get; set; }
    public string Mother2ndPhoneNumber { get; set; }
    public string MotherPhotoUrl { get; set; }
    public List<UserForDetailedDto> Siblings { get; set; }
  }
}