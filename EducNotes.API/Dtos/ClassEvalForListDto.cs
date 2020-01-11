using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class ClassEvalForListDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public IEnumerable<EvaluationForListDto> Evals { get; set; }
    }
}