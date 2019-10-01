using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class ParentSefRegisterDto
    {
        public UserForRegisterDto user1 { get; set; }
        // public UserForRegisterDto user2 { get; set; }
        public List<UserForRegisterDto> children { get; set; }
    }
}