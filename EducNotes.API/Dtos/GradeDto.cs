namespace EducNotes.API.Dtos
{
    public class GradeDto
    {
        public string EvalDate { get; set; }
        public string EvalType { get; set; }
        public string EvalName { get; set; }
        public double Grade { get; set; }
        public double GradeMax { get; set; }
        public double Coeff { get; set; }
        public double ClassGradeMin { get; set; }
        public double ClassGradeMax { get; set; }
        
    }
}