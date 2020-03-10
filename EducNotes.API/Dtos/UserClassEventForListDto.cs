using System;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class UserClassEventForListDto
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string PhotoUrl { get; set; }
    public int ClassEventId { get; set; }
    public string ClassEventName { get; set; }
    public int ClassEventTypeId { get; set; }
    public string ClassEventType { get; set; }
    //public int DoneById { get; set; }
    public string DoneByName { get; set; }
    public DateTime StartDate { get; set; }
    public string strStartDate { get; set; }
    public string StartTime { get; set; }
    public DateTime EndDate { get; set; }
    public string strEndDate { get; set; }
    public string EndTime { get; set; }
    public string Justified { get; set; }
    public string Reason { get; set; }
    public string Comment { get; set; }
  }
}