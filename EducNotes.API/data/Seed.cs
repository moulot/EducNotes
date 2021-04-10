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
    public static void SeedTables(DataContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
      {
        if (!context.FileTypes.Any())
        {
          var fileTypes = new List<FileType>() {
            new FileType {Name="file type 1"},
            new FileType  {Name="file type 2"}
          };
          context.AddRange(fileTypes);
          // context.SaveChanges();
        }
        if (!context.DocTypes.Any())
        {
          var docTypes = new List<DocType>(){
          new DocType {Name="fichier principal"},
            new DocType  {Name="autre fichier"}
          };
          context.AddRange(docTypes);
          // context.SaveChanges();
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
              new Setting { Name = "DaysToValidateReg", DisplayName = "délai (jours) pour valider une inscription" },
              new Setting { Name = "SubDomain", DisplayName = "domaine de l'école" }
            };
            context.AddRange(settings);

            var emailtypes = new List<EmailType>() {
              new EmailType { Name = "confirmation Email" },
              new EmailType { Name = "confirmed Email" },
              new EmailType { Name = "communication" }
            };
            context.AddRange(emailtypes);

            var tokentypes = new List<TokenType>() {
              new TokenType { Name = "diffusion message" },
            };
            context.AddRange(tokentypes);

            var classtypes = new List<ClassType>() {
              new ClassType { Name = "série A", Code = "A" },
              new ClassType { Name = "série A1", Code = "A1" },
              new ClassType { Name = "série A2", Code = "A2" },
              new ClassType { Name = "série C", Code = "C" },
              new ClassType { Name = "série D", Code = "D" }
            };
            context.AddRange(classtypes);

            var finOpTypes = new List<FinOpType>() {
              new FinOpType { Name = "facturé" },
              new FinOpType { Name = "paiement" },
              new FinOpType { Name = "avoir" },
              new FinOpType { Name = "remboursement" }
            };
            context.AddRange(finOpTypes);

            var productTypes = new List<ProductType>() {
              new ProductType { Name = "scolarité" },
              new ProductType { Name = "services" }
            };
            context.AddRange(productTypes);
            context.SaveChanges();

            // using(var identityContextTransaction = context.Database.BeginTransaction())
            // {
            //   try
            //   {
            //     context.ProductTypes.FromSql.ToList();
            //     // context.ProductTypes.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Customers ON");
            //     // context.SaveChanges();
            //     // context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Customers OFF");
            //   }
            //   finally
            //   {
            //     context.Database.CommitTransaction();
            //   }
            // }

            var products = new List<Product>() {
              new Product { Name = "scolarité", ProductTypeId = 1, IsByLevel = true, IsPeriodic = false, IsRequired = false, IsPaidCash = false},
              new Product { Name = "scolarité année prochaine", ProductTypeId = 1, IsByLevel = true, IsPeriodic = false, IsRequired = false, IsPaidCash = false},
              new Product { Name = "transport élèves", ProductTypeId = 2, IsByLevel = true, IsPeriodic = true, IsRequired = false, IsPaidCash = true},
              new Product { Name = "cantine", ProductTypeId = 2, IsByLevel = true, IsPeriodic = true, IsRequired = false, IsPaidCash = true}
            };
            context.AddRange(products);

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
              new Cycle { Name = "cycle 4"},
              new Cycle { Name = "cycle terminal"}
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

            var classLevelProds = new List<ClassLevelProduct>() {
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 1, Price = 250000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 2, Price = 250000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 3, Price = 250000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 4, Price = 350000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 5, Price = 350000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 6, Price = 350000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 7, Price = 350000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 8, Price = 350000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 9, Price = 350000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 10, Price = 500000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 11, Price = 500000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 12, Price = 500000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 13, Price = 500000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 14, Price = 650000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 15, Price = 650000 },
              new ClassLevelProduct { ProductId = 1, ClassLevelId = 16, Price = 650000 },
            };
            context.AddRange(classLevelProds);

            var prodDeadlines = new List<ProductDeadLine>() {
              new ProductDeadLine { ProductId = 1, DueDate = new DateTime(2020, 9, 1), DeadLineName = "1er acompte", Seq = 1, Percentage = 0.5M},
              new ProductDeadLine { ProductId = 1, DueDate = new DateTime(2020, 11, 15), DeadLineName = "2e acompte", Seq = 2, Percentage = 0.3M},
              new ProductDeadLine { ProductId = 1, DueDate = new DateTime(2021, 2, 15), DeadLineName = "3e acompte", Seq = 3, Percentage = 0.2M}
            };
            context.AddRange(prodDeadlines);
            context.SaveChanges();

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

            var smsTypes = new List<SmsType> {
              new SmsType { Name = "absence" },
              new SmsType { Name = "alerte" },
              new SmsType { Name = "comm" },
              new SmsType { Name = "note" },
              new SmsType { Name = "validation" }
            };
            context.AddRange(smsTypes);
            context.SaveChanges();

            var districts = new List<District> {
              new District { Name = "cocody", CityId = 1 },
              new District { Name = "angré", CityId = 1 },
              new District { Name = "2 Plateaux", CityId = 1 },
              new District { Name = "djibi", CityId = 1 },
            };
            context.AddRange(districts);

            var smsCat = new List<SmsCategory> {
              new SmsCategory {Name = "notes"},
              new SmsCategory {Name = "vie de classe"},
              new SmsCategory {Name = "administration"},
              new SmsCategory {Name = "alertes"},
              new SmsCategory {Name = "communication"},
              new SmsCategory {Name = "diffusion messages"}
            };
            context.AddRange(smsCat);

            var emailCat = new List<EmailCategory> {
              new EmailCategory {Name = "notes"},
              new EmailCategory {Name = "communication"},
              new EmailCategory {Name = "scolarité"},
              new EmailCategory {Name = "paiement"},
              new EmailCategory {Name = "diffusion messages"}
            };
            context.AddRange(emailCat);
            context.SaveChanges();

            var emailTemplates = new List<EmailTemplate> {
              new EmailTemplate {Name = "nouvelle inscription - informations paiement",
                Subject ="<NOM_ECOLE> - confirmation nouvelle inscription / paiement", Body = "",
                EmailCategoryId = 3, Internal = true},
              new EmailTemplate {Name = "re-inscription année prochaine",
                Subject ="<NOM_ECOLE> - re-inscription pour l'année prochaine", Body = "",
                EmailCategoryId = 3, Internal = true},
              new EmailTemplate {Name = "confirmation mise à jour du compte",
                Subject ="<NOM_ECOLE> - confirmation mise à jour de compte", Body = "",
                EmailCategoryId = 2, Internal = true},
              new EmailTemplate {Name = "confirmation compte enseignant",
                Subject ="<NOM_ECOLE> - confirmation de compte enseignant Educ'Notes", Body = "",
                EmailCategoryId = 2, Internal = true},
              new EmailTemplate {Name = "nouvelle note",
                Subject ="<N_ENFANT> <P_ENFANT> a une nouvelle note en <COURS>", Body = "",
                EmailCategoryId = 1, Internal = true},
              new EmailTemplate {Name = "re-initialisation password",
                Subject ="re-initialisation de mot de passe", Body = "",
                EmailCategoryId = 2, Internal = true},
              new EmailTemplate {Name = "envoi code modification email",
                Subject ="validation de votre email", Body = "",
                EmailCategoryId = 2, Internal = true}
            };
            context.AddRange(emailTemplates);

            var smsTemplates = new List<SmsTemplate> {
              new SmsTemplate {Name = "nouvelle note", Content = "", SmsCategoryId = 1, Internal = true},
              new SmsTemplate {Name = "absence", Content = "", SmsCategoryId = 2, Internal = true},
              new SmsTemplate {Name = "devoir de maison", Content = "", SmsCategoryId = 2, Internal = true},
              new SmsTemplate {Name = "alerte note basse", Content = "", SmsCategoryId = 1, Internal = true},
              new SmsTemplate {Name = "congés et sorties", Content = "", SmsCategoryId = 5, Internal = true},
              new SmsTemplate {Name = "retard", Content = "", SmsCategoryId = 2, Internal = true},
              new SmsTemplate {Name = "modification numéro mobile", Content = "", SmsCategoryId = 5, Internal = true},
              new SmsTemplate {Name = "modification mot de passe", Content = "", SmsCategoryId = 5, Internal = true}
            };
            context.AddRange(smsTemplates);

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

            var paymentTypes = new List<PaymentType> {
              new PaymentType { Name = "espèces", DsplSeq = 1 },
              new PaymentType { Name = "chèque", DsplSeq = 2 },
              new PaymentType { Name = "virement", DsplSeq = 3 },
              new PaymentType { Name = "paiement mobile", DsplSeq = 4 }
            };
            context.AddRange(paymentTypes);

            var inscTypes = new List<InscriptionType> {
              new InscriptionType { Name = "FromParent" },
              new InscriptionType { Name = "FromSchool" }
            };
            context.AddRange(inscTypes);
            context.SaveChanges();

            var pers = new List<Period> {
              new Period { Name = "1er trimestre", Abbrev = "1er trim", StartDate = new DateTime(2020, 9, 1), 
                EndDate = new DateTime(2020, 11, 30), Active = false },
              new Period { Name = "2e trimestre", Abbrev = "2e trim",  StartDate = new DateTime(2020, 12, 1), 
                EndDate = new DateTime(2021, 2, 27),Active = false },
              new Period { Name = "3e trimestre", Abbrev = "3e trim", StartDate = new DateTime(2021, 3, 1), 
                EndDate = new DateTime(2021, 6, 30), Active = false }
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
                new EvalType { Name = "devoir maison", Abbrev = "devoir maison" },
                new EvalType { Name = "oral", Abbrev = "oral" },
                new EvalType { Name = "devoir", Abbrev = "devoir" },
                new EvalType { Name = "interro. écrite", Abbrev = "interro. écrite" },
                new EvalType { Name = "exposé", Abbrev = "exposé" },
                new EvalType { Name = "travaux pratiques", Abbrev = "TP" },
                new EvalType { Name = "rapport de stage", Abbrev = "rapport de stage" },
                new EvalType { Name = "tenue de cahier", Abbrev = "tenue de cahier" },
                new EvalType { Name = "examen", Abbrev = "examen" }
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
                new Token {Name = "nom enseignant", TokenString = "<N_ENSEIGNANT>"},
                new Token {Name = "prénom enseignant", TokenString = "<P_ENSEIGNANT>"},
                new Token {Name = "email enseignant", TokenString = "<EMAIL_ENSEIGNANT>"},
                new Token {Name = "mobile enseignant", TokenString = "<CELL_ENSEIGNANT>"},
                new Token {Name = "lien de confirmation", TokenString = "<CONFIRM_LINK>"},
                new Token {Name = "moyens de paiement", TokenString = "<TYPES_PAIEMENT>"}
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

            var periodicities = new List<Periodicity> {
              new Periodicity {Name = "mensuel"},
              new Periodicity {Name = "trimestriel"}
            };
            context.AddRange(periodicities);

            var orderTypes = new List<OrderType> {
              new OrderType { Name = "frais scolarité"},
              new OrderType { Name = "services"},
              new OrderType { Name = "produits divers"}
            };
            context.AddRange(orderTypes);

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
              AccountDataValidated = true,
              Validated = true,
              UserTypeId = 4
            };

            IdentityResult result = userManager.CreateAsync(adminUser, "password").Result;

            if (result.Succeeded)
            {
              var admin = userManager.FindByNameAsync("Admin").Result;
              userManager.AddToRolesAsync(admin, new[] { "Admin", "enseignant" }).Wait();
            }

        }
      }
  }
}