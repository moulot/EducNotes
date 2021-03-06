namespace EducNotes.API.Dtos
{
  public class TeacherDayCoursesDto
  {
    public int TeacherId { get; set; }
    public string TeacherName { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string ItemName { get; set; }
    public int Day { get; set; }
    public string StartHourMin { get; set; }
    public string EndHourMin { get; set; }
  }
}