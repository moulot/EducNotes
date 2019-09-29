namespace EducNotes.API.Models
{
    public class EvalProgElt
    {
        public int Id { get; set; }
        public int EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; }
        public int ProgramElementId { get; set; }
        public ProgramElement ProgramElement { get; set; }
    }
}