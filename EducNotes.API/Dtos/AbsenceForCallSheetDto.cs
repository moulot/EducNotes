using System;

namespace EducNotes.API.Dtos
{
  public class AbsenceForCallSheetDto
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AbsenceTypeId { get; set; }
    public int SessionId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string strStartDate { get; set; }
    public string strEndDate { get; set; }
    public int LateInMin { get; set; }
    public Boolean Justified { get; set; }
    public string Reason { get; set; }
    public string Comment { get; set; }
  }
}