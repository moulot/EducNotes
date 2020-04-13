namespace EducNotes.API.Dtos
{
    public class QuickStudentAssignmentDto
    {
        //  element.lastName = la_ligne.nom_Enfant;
        // element.firstName = la_ligne.prenoms_Enfant,
        // element.idnum = la_ligne.matricule_Enfant,
        // element.phoneNumber = la_ligne.cellulaire_Enfant,
        // element.secondPhoneNumber = la_ligne.second_contact_Enfant,
        // element.email = la_ligne.email_Enfant;
        // element.userTypeId = this.studentTypeId;

        // element.parent = {
        //   lastName: la_ligne.nom_Parent,
        //   firstName: la_ligne.prenoms_Parent,
        //   phoneNumber: la_ligne.cellulaire_Parent,
        //   secondPhoneNumber: la_ligne.second_contact_Parent,
        //   email: la_ligne.email_Parent,
        // };

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Idnum { get; set; }
        public string phoneNumber { get; set; }
        public string secondPhoneNumber { get; set; }
        public bool SendEmail { get; set; }
        public int UserTypeId { get; set; }
        public QuickStudentAssignmentDto Parent { get; set; }
    }
}