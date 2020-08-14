using System;
using System.Collections.Generic;
using System.Linq;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace EducNotes.API.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
          if (!context.FileTypes.Any())
          {
            var fileTypes = new List<FileType>() {
              new FileType {Name="FILE TYPE 1"},
              new FileType  {Name="FILE TYPE 2"}
            };
            context.AddRange(fileTypes);
            context.SaveChanges();
          }
          if (!context.DocTypes.Any())
          {
            var docTypes = new List<DocType>(){
            new DocType {Name="fichier principal"},
              new DocType  {Name="Autre fichier"}
            };
            context.AddRange(docTypes);
            context.SaveChanges();
          }
          if (!userManager.Users.Any())
          {
              var settings = new List<Setting>() {
                new Setting { Name = "RegistrationDate", DisplayName = "date début des inscriptions" },
                new Setting { Name = "StartHourMin", DisplayName = "heure début des cours" },
                new Setting { Name = "EndHourMin", DisplayName = "heure fin des cours" },
                new Setting { Name = "SchoolName", DisplayName = "nom de l'établissement" },
                new Setting { Name = "SchoolEmail", DisplayName = "email de l'établissement" },
                new Setting { Name = "RegistrationDeadLine", DisplayName = "date limite des inscriptions" },
                new Setting { Name = "SendRegSms", DisplayName = "envoyer sms pour inscription" },
                new Setting { Name = "RegFee", DisplayName = "frais d'inscription" },
                new Setting { Name = "NbTuitionPayments", DisplayName = "nb de versements pour la scolarité" },
                new Setting { Name = "RegFeeNextYear", DisplayName = "frais d'inscription année suivante" },
                new Setting { Name = "DaysToValidateReg", DisplayName = "délai (jours) pour valider une inscription" }
              };
              context.AddRange(settings);

              // var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
              // var users = JsonConvert.DeserializeObject<List<User>>(userData);
              var emailtypes = new List<EmailType>() {
                new EmailType { Name = "Confirmation Email" },
                new EmailType { Name = "Confirmed Email" }
              };
              context.AddRange(emailtypes);

              var classtypes = new List<ClassType>() {
                new ClassType { Name = "scientifique" },
                new ClassType { Name = "littéraire" },
                new ClassType { Name = "semi-littéraire" }
              };

              var productTypes = new List<ProductType>() {
                new ProductType { Name = "scolarité" },
                new ProductType { Name = "services" }
              };
              context.AddRange(productTypes);
              context.SaveChanges();

              var products = new List<Product>() {
                new Product { Name = "scolarité", ProductTypeId = 1, IsByLevel = true, IsPeriodic = false, IsRequired = false, IsPaidCash = false},
                new Product { Name = "scolarité année prochaine", ProductTypeId = 1, IsByLevel = true, IsPeriodic = false, IsRequired = false, IsPaidCash = false},
                new Product { Name = "transport élèves", ProductTypeId = 2, IsByLevel = true, IsPeriodic = true, IsRequired = false, IsPaidCash = true},
                new Product { Name = "cantine", ProductTypeId = 2, IsByLevel = true, IsPeriodic = true, IsRequired = false, IsPaidCash = true}
              };
              context.AddRange(products);

              var roles = new List<Role> {
                new Role { Name = "élève" },
                new Role { Name = "professeur" },
                new Role { Name = "parent" },
                new Role { Name = "admin" }
              };

              foreach (var role in roles)
              {
                roleManager.CreateAsync(role).Wait();
              }

              var city = new City { Name = "Abidjan" };
              context.Add(city);
              context.SaveChanges();

              var smsTypes = new List<SmsType> {
                new SmsType { Name = "absence" },
                new SmsType { Name = "alerte" },
                new SmsType { Name = "comm" },
                new SmsType { Name = "note" }
              };
              context.AddRange(smsTypes);
                context.SaveChanges();

              var districts = new List<District> {
                new District { Name = "Cocody", CityId = 1 },
                new District { Name = "Angré", CityId = 1 },
                new District { Name = "2 Plateaux", CityId = 1 },
                new District { Name = "Djibi", CityId = 1 },
              };
              context.AddRange(districts);

              var AbsenceTypes = new List<AbsenceType> {
                new AbsenceType { Name = "Absence" },
                new AbsenceType { Name = "Retard" }
              };
              context.AddRange(AbsenceTypes);

              var classEvents = new List<ClassEvent> {
                new ClassEvent {Name = "sanction"},
                new ClassEvent {Name = "compliments"}
              };
              context.AddRange(classEvents);

              var inscTypes = new List<InscriptionType> {
                new InscriptionType { Name = "FromParent" },
                new InscriptionType { Name = "FromSchool" }
              };
              context.AddRange(inscTypes);
              context.SaveChanges();


              var educLevels = new List<EducationLevel>() {
                new EducationLevel { Name ="primaire" },
                new EducationLevel { Name ="secondaire" }
              };
              context.AddRange(educLevels);

              var schools = new List<School>() {
                new School { Name = "maternelle", DsplSeq = 1 },
                new School { Name = "élémentaire", DsplSeq = 2 },
                new School { Name = "collège", DsplSeq = 3 },
                new School { Name = "lycée", DsplSeq = 4 },
              };
              context.AddRange(schools);

              var cycles = new List<Cycle>() {
                new Cycle { Name = "cycle 1"},
                new Cycle { Name = "cycle 2"},
                new Cycle { Name = "cycle 3"},
                new Cycle { Name = "cycle 4"}
              };
              context.AddRange(cycles);
              context.SaveChanges();

              var classLevels = new List<ClassLevel> {
                  new ClassLevel { Name = "TPS", DsplSeq = 1, CycleId = 1, SchoolId = 1, EducationLevelId =1 },
                  new ClassLevel { Name = "MS", DsplSeq = 2, CycleId = 1, SchoolId = 1, EducationLevelId =1  },
                  new ClassLevel { Name = "PS", DsplSeq = 3, CycleId = 1, SchoolId = 1, EducationLevelId =1  },
                  new ClassLevel { Name = "CP1", DsplSeq = 4, CycleId = 2, SchoolId = 2, EducationLevelId =1  },
                  new ClassLevel { Name = "CP2", DsplSeq = 5, CycleId = 2, SchoolId = 2, EducationLevelId =1  },
                  new ClassLevel { Name = "CE1", DsplSeq = 6, CycleId = 2, SchoolId = 2, EducationLevelId =1  },
                  new ClassLevel { Name = "CE2", DsplSeq = 7, CycleId = 2, SchoolId = 2, EducationLevelId =1  },
                  new ClassLevel { Name = "CM1", DsplSeq = 8, CycleId = 3, SchoolId = 2, EducationLevelId =1  },
                  new ClassLevel { Name = "CM2", DsplSeq = 9, CycleId = 3, SchoolId = 2, EducationLevelId =1  },
                  new ClassLevel { Name = "6e", DsplSeq = 10, CycleId = 3, SchoolId = 3, EducationLevelId = 2 },
                  new ClassLevel { Name = "5e", DsplSeq = 11, CycleId = 4, SchoolId = 3, EducationLevelId = 2 },
                  new ClassLevel { Name = "4e", DsplSeq = 12, CycleId = 4, SchoolId = 3, EducationLevelId = 2 },
                  new ClassLevel { Name = "3e", DsplSeq = 13, CycleId = 4, SchoolId = 3, EducationLevelId = 2 },
                  new ClassLevel { Name = "2nde", DsplSeq = 14, SchoolId = 4, EducationLevelId = 2 },
                  new ClassLevel { Name = "1ere", DsplSeq = 15, SchoolId = 4, EducationLevelId = 2 },
                  new ClassLevel { Name = "Tle", DsplSeq = 16, SchoolId = 4, EducationLevelId = 2 }
              };
              context.AddRange(classLevels);
              context.SaveChanges();

              var paymentTypes = new List<PaymentType> {
                new PaymentType { Name = "espèces" },
                new PaymentType { Name = "chèque" },
                new PaymentType { Name = "virement" },
                new PaymentType { Name = "paiement mobible" }
              };
              context.AddRange(paymentTypes);

              var pers = new List<Period> {
                new Period { Name = "1er trimestre", Active = false },
                new Period { Name = "2e trimestre", Active = false },
                new Period { Name = "3e trimestre", Active = false }
              };
              context.AddRange(pers);

              var banks = new List<Bank>() {
                new Bank { Name = "Ecobank" },
                new Bank { Name = "NSIA" },
                new Bank { Name = "SIB" },
                new Bank { Name = "SGBCI" },
                new Bank { Name = "" }
              };
              context.AddRange(banks);

              var evalTypes = new List<EvalType> {
                  new EvalType { Name = "devoir maison" },
                  new EvalType { Name = "oral" },
                  new EvalType { Name = "devoir" },
                  new EvalType { Name = "interro. écrite" },
                  new EvalType { Name = "exposé" },
                  new EvalType { Name = "travaux pratiques" },
                  new EvalType { Name = "rapport de stage" },
                  new EvalType { Name = "tenue de cahier" },
                  new EvalType { Name = "examen" }
              };
              context.AddRange(evalTypes);

              var tokens = new List<Token> {
                  new Token {Name = "prénom enfant", TokenString = "<P_ENFANT>"},
                  new Token {Name = "nom enfant", TokenString = "<N_ENFANT>"},
                  new Token {Name = "nom parent", TokenString = "<N_PARENT>"},
                  new Token {Name = "prénom parent", TokenString = "<P_PARENT>"},
                  new Token {Name = "cours", TokenString = "<COURS>"},
                  new Token {Name = "horaire cours", TokenString = "<HORAIRE_COURS>"},
                  new Token {Name = "moyenne cours", TokenString = "<MOY_COURS>"},
                  new Token {Name = "moyenne générale", TokenString = "<MOY_GLE>"},
                  new Token {Name = "classe enfant", TokenString = "<CLASSE_ENFANT>"},
                  new Token {Name = "note cours", TokenString = "<NOTE_COURS>"},
                  new Token {Name = "note cours la plus basse", TokenString = "<NOTE_COURS_BASSE>"},
                  new Token {Name = "note cours la plus haute", TokenString = "<NOTE_COURS_HAUTE>"},
                  new Token {Name = "date session de cours", TokenString = "<DATE_COURS>"},
                  new Token {Name = "préfixe au nom", TokenString = "<M_MME>"},
                  new Token {Name = "retard en min", TokenString = "<RETARD_MIN>"},
                  new Token {Name = "date du jour", TokenString = "<DATE_JOUR>"},
                  new Token {Name = "frais inscription", TokenString = "<FRAIS_INSCR>"},
                  new Token {Name = "acompte frais inscr.", TokenString = "<ACPTE_FRAIS_SCOLARITE>"},
                  new Token {Name = "classe réinscr.", TokenString = "<CLASSE_SUP>"},
                  new Token {Name = "nom de l'école", TokenString = "<NOM_ECOLE>"},
                  new Token {Name = "date limite inscr.", TokenString = "<DATE_LIMITE_INSCR>"},
                  new Token {Name = "préfixe au nom", TokenString = "<M_MME>"},
                  new Token {Name = "pourcentage acompte", TokenString = "<PCT_ACOMPTE>"},
                  new Token {Name = "montant acompte", TokenString = "<MONTANT_ACOMPTE>"},
                  new Token {Name = "infos inscr. enfants", TokenString = "<INFOS_INSCR_ENFANTS>"},
                  new Token {Name = "total des frais", TokenString = "<TOTAL_FRAIS>"},
                  new Token {Name = "date limite frais à payer", TokenString = "<DATE_LIMITE_FRAIS>"},
                  new Token {Name = "token de confirmation", TokenString = "<TOKEN>"},
                  new Token {Name = "id de commande", TokenString = "<CDE_ID>"},
                  new Token {Name = "id du parent", TokenString = "<PARENT_ID>"},
                  new Token {Name = "url site web", TokenString = "<BASE_URL>"}
              };
              context.AddRange(tokens);

              var rewards = new List<Reward> {
                new Reward {Name = "recompense 1"},
                new Reward {Name = "recompense 2"},
                new Reward {Name = "recompense 3"}
              };
              context.AddRange(rewards);

              var sanctions = new List<Sanction> {
                new Sanction {Name = "sanction 1"},
                new Sanction {Name = "sanction 2"},
                new Sanction {Name = "sanction 3"}
              };
              context.AddRange(sanctions);

              var eventTypes = new List<EventType> {
                new EventType {Name = "évaluation" },
                new EventType {Name = "cours" },
                new EventType {Name = "rendz-vous" },
                new EventType {Name = "réunion" }
              };
              context.AddRange(eventTypes);

              var smsCat = new List<SmsCategory> {
                new SmsCategory {Name = "notes"},
                new SmsCategory {Name = "vie de classe"},
                new SmsCategory {Name = "administration"},
                new SmsCategory {Name = "alertes"},
                new SmsCategory {Name = "communication"}
              };
              context.AddRange(smsCat);

              var emailCat = new List<EmailCategory> {
                new EmailCategory {Name = "notes"},
                new EmailCategory {Name = "communication"},
                new EmailCategory {Name = "scolarité"},
                new EmailCategory {Name = "paiement"}
              };
              context.AddRange(smsCat);

              var periodicities = new List<Periodicity> {
                new Periodicity {Name = "mensuel"},
                new Periodicity {Name = "trimestriel"}
              };
              context.AddRange(periodicities);

              var userTypes = new List<UserType> {
                new UserType { Name = "élève" },
                new UserType { Name = "enseignant" },
                new UserType { Name = "parent" },
                new UserType { Name = "admin" }
              };
              context.AddRange(userTypes);
              context.SaveChanges();

              var adminUser = new User
              {
                UserName = "admin",
                FirstName = "admin",
                LastName = "admin",
                Email = "adminUser@educnotes.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                ValidatedCode = true,
                Validated = true,
                UserTypeId = 4
              };

              IdentityResult result = userManager.CreateAsync(adminUser, "password").Result;

              if (result.Succeeded)
              {
                var admin = userManager.FindByNameAsync("Admin").Result;
                userManager.AddToRolesAsync(admin, new[] { "Admin", "Professeur" }).Wait();
              }

          }
        }
    }
}