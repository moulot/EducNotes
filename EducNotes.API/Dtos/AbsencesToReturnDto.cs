namespace EducNotes.API.Dtos
{
    public class AbsencesToReturnDto
    {
        public string UserName { get; set; }
        public string AbsenceType { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string Justified { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
    }
}