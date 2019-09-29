using System;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class AbsenceForSaveDto
    {
        public int UserId { get; set; }
        public int AbsenceTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Boolean Justified { get; set; }
        public string Comment { get; set; }
    }
}