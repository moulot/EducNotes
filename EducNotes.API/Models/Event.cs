using System;

namespace EducNotes.API.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int EventTypeId { get; set; }
        public EventType EventType { get; set; }
        public DateTime EventDate { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public int EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
    }
}