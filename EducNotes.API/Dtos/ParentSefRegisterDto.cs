using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class ParentSefRegisterDto
    {
        public UserForUpdateDto parent { get; set; }
        // public UserForRegisterDto user2 { get; set; }
        public List<UserForUpdateDto> children { get; set; }
    }
}