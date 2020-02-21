using System;

namespace EducNotes.API.Dtos
{
    public class SessionForCallSheetDto
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public DateTime SessionDate { get; set; }
        public string strSessionDate { get; set; }
        public string Comment { get; set; }
    }
}