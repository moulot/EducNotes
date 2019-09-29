using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class ParentDetailsDto
    {
        public UserForListDto Parent { get; set; }
        public List<UserForListDto> Children { get; set; }
    }
}