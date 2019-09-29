using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class CoursesDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StudentsNumber { get; set; }
        public int TeachersNumber { get; set; }
        public int ClassesNumber { get; set; }
        public List<UserForListDto> Teachers { get; set; }
        public List<Class> Classes { get; set; }
    }
}