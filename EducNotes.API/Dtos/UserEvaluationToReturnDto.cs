namespace EducNotes.API.Dtos
{
    public class UserEvaluationToReturnDto
    {
        public string EvalName { get; set; }
        public string CourseName { get; set; }
        public string EvalTypeName { get; set; }
        public string PeriodName { get; set; }
        public string ClassName { get; set; }
        public string StudentName { get; set; }
        public string EvalDate { get; set; }
        public double Coeff { get; set; }
    }
}