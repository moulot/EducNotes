using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class SchoolServicesDto
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public decimal? Price { get; set; }
        public bool IsByLevel { get; set; }
        public bool IsPeriodic { get; set; }
        public Periodicity Periodicity { get; set; }
        public PayableAt PayableAt { get; set; }
        public List<ClassLevelProduct> ClassLevelProducts { get; set; }
        public List<ProductDeadLine> ProductDeadLines { get; set; }

    }
}