namespace EducNotes.API.Dtos
{
    public class LessonContentDto
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int NbHours { get; set; }
        public byte? SessionNum { get; set; }
        public int DsplSeq { get; set; }

    }
}