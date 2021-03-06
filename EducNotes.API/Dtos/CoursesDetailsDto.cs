using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
    public class CoursesDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseTypeId { get; set; }
        public string Abbreviation { get; set; }
        public string Color { get; set; }
        public int NbStudents { get; set; }
        public int NbTeachers { get; set; }
        public int NbClasses { get; set; }
        public List<UserForListDto> Teachers { get; set; }
        public List<Class> Classes { get; set; }
    }
}