using System;

namespace EducNotes.API.Models
{
    public class UserEvaluation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; }
        public string Grade { get; set; }
        public string Comment { get; set; }
    }
}