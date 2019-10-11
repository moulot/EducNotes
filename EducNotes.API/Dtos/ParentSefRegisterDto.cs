using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class ParentSefRegisterDto
    {
        public UserForUpdateDto user1 { get; set; }
        // public UserForRegisterDto user2 { get; set; }
        public List<UserForUpdateDto> children { get; set; }
    }
}