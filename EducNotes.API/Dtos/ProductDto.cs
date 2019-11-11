using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string  Comment { get; set; }
        public int ProductTypeId { get; set; }
        public bool IsByLevel { get; set; }
        public bool IsPeridic { get; set; }
        public int RecoveryTypeId { get; set; }
        public int PayableAtId { get; set; }
        public int? PeriodicityId { get; set; }
        public int? deadLineCount { get; set; }
        public decimal? Price { get; set; }
        public ProdDeadLineDto[] Deadlines {get; set;}

        public LevelProdDto[] levels { get; set; }
    }
}