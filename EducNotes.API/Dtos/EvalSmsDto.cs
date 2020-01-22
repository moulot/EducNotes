namespace EducNotes.API.Dtos
{
    public class EvalSmsDto
    {
        public int ChildId { get; set; }
        public string ChildFirstName { get; set; }
        public string ChildLastName { get; set; }
        public int ParentId { get; set; }
        public string ParentFirstName { get; set; }
        public string ParentLastName { get; set; }
        public byte ParentGender { get; set; }
        public string EvalDate { get; set; }
        public string CapturedDate { get; set; }
        public string CourseName { get; set; }
        public string CourseAbbrev { get; set; }
        public decimal EvalGrade { get; set; }
        public byte GradedOutOf { get; set; }
        public decimal ClassMinGrade { get; set; }
        public decimal ClassMaxGrade { get; set; }
        public decimal ChildCourseAvg { get; set; }
        public decimal ChildAvg { get; set; }
        public decimal ClassCourseAvg { get; set; }
        public decimal ClassAvg { get; set; }
        public string ParentCellPhone { get; set; }

    }
}