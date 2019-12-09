using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
    public class UserForUpdateDto
    {
        public string Introduction { get; set; }
        public string Password { get; set; }
        public int UserTypeId { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public int? ClassLevelId { get; set; }
        public string PhoneNumber { get; set; }
        public string SecondPhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<int?> CourseIds { get; set; }
        public int? ClassId { get; set; }
        public int? LevelId { get; set; }
        public int? DistrictId { get; set; }
        public int? CityId { get; set; }
        public byte? Gender { get; set; }
        public int? SpaCode { get; set; }
        public List<ServiceSelectionDto> Products { get; set; }
    }
}