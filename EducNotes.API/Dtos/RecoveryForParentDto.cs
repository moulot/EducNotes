using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class RecoveryForParentDto
  {
    public RecoveryForParentDto()
    {
      Children = new List<RecoveryForChildDto>();
    }

    public int FatherId { get; set; }
    public string FatherLastName { get; set; }
    public string FatherFirstName { get; set; }
    public string FatherEmail { get; set; }
    public string FatherMobile { get; set; }
    public byte FatherGender { get; set; }
    public int MotherId { get; set; }
    public string MotherLastName { get; set; }
    public string MotherFirstName { get; set; }
    public string MotherEmail { get; set; }
    public string MotherMobile { get; set; }
    public byte MotherGender { get; set; }
    public decimal LateDueAmount { get; set; }
    public Boolean ByEmail { get; set; }
    public Boolean BySms { get; set; }
    public List<RecoveryForChildDto> Children { get; set; }
  }
}