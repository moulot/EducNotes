using System;

namespace EducNotes.API.Dtos
{
    public class UserForClassAllocationDto
    {
      public int Id { get; set; }
      public DateTime InsertDate { get; set; }
      public int ClassLevelId { get; set; }
      public int UserId { get; set; }
      public string LastName { get; set; }
      public string FirstName { get; set; }
      public byte Gender { get; set; }
      public string PhotoUrl { get; set; }
      public string DateOfBirth { get; set; }
      public int Age { get; set; }
    }
}