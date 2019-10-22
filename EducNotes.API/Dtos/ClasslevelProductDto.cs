using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class ClasslevelProductDto
    {
        public decimal Amount { get; set; }
        public List<int> ClassLevelIds { get; set; }
        public int ProductId { get; set; }

    }
}