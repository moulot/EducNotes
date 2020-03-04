using System;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class UserClassLifeForListDto
    {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public int ClassLifeId { get; set; }
    public string ClassLifeName { get; set; }
    public string ClassLifeType { get; set; }
    public int DoneById { get; set; }
    public string DoneByName { get; set; }
    public DateTime StartDate { get; set; }
    public string strStartDate { get; set; }
    public string StartTime { get; set; }
    public string strEndDate { get; set; }
    public string EndDate { get; set; }
    public string EndTime { get; set; }
    public Boolean Justified { get; set; }
    public string Reason { get; set; }
    public string Comment { get; set; }
    }
}