using System;

namespace EducNotes.API.Dtos
{
    public class InscriptionDetailDto
    {
       public int Id { get; set; }
        public DateTime InsertDate { get; set; }
        public int ClassLevelId { get; set; }
        public int UserId { get; set; }
        public UserForDetailedDto User { get; set; }
     
    }
}