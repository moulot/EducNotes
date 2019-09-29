
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class PreInscriptionDto
    {
        public UserForRegisterDto father { get; set; }
        public UserForRegisterDto mother { get; set; }
        public List<UserForRegisterDto> children { get; set; }

    }
}