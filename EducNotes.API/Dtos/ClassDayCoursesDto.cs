namespace EducNotes.API.Dtos
{
  public class ClassDayCoursesDto
  {
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string CourseAbbrev { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; }
    public string StartHourMin { get; set; }
    public string EndHourMin { get; set; }
    public int StartHourMinNum { get; set; }
    public int EndHourMinNum { get; set; }
    public string Color { get; set; }
  }
}