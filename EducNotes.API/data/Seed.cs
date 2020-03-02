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
          if (!userManager.Users.Any())
            {
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

                context.Add(new ProductType { Name = "School Service" });

                var school = new Establishment
                {
                    Name = "",
                    Location = "",
                    Phone = "",
                    WebSite = "",
                    StartCoursesHour = DateTime.Now.Date,
                    EndCoursesHour = DateTime.Now.Date
                };
                context.Add(school);

                var roles = new List<Role> {
                    new Role { Name = "élève" },
                    new Role { Name = "Professeur" },
                    new Role { Name = "Parent" },
                    new Role { Name = "Admin" }
                };

                foreach (var role in roles)
                {
                  roleManager.CreateAsync(role).Wait();
                }

                var city = new City { Name = "Abidjan" };
                context.Add(city);
                context.SaveChanges();

                var smsTypes = new List<SmsType> {
                    new SmsType { Name = "Absence" },
                    new SmsType { Name = "Alerte" },
                    new SmsType { Name = "Comm" },
                    new SmsType { Name = "Note" }
                };
                context.AddRange(smsTypes);

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

                var inscTypes = new List<InscriptionType> {
                    new InscriptionType { Name = "FromParent" },
                    new InscriptionType { Name = "FromSchool" }
                };
                context.AddRange(inscTypes);

                var classLevels = new List<ClassLevel> {
                    new ClassLevel { Name = "TPS", DsplSeq = 1 },
                    new ClassLevel { Name = "MS", DsplSeq = 2 },
                    new ClassLevel { Name = "PS", DsplSeq = 3 },
                    new ClassLevel { Name = "CP1", DsplSeq = 4 },
                    new ClassLevel { Name = "CP2", DsplSeq = 5 },
                    new ClassLevel { Name = "CE1", DsplSeq = 6 },
                    new ClassLevel { Name = "CE2", DsplSeq = 7 },
                    new ClassLevel { Name = "CM1", DsplSeq = 8 },
                    new ClassLevel { Name = "CM2", DsplSeq = 9 },
                    new ClassLevel { Name = "6e", DsplSeq = 10 },
                    new ClassLevel { Name = "5e", DsplSeq = 11 },
                    new ClassLevel { Name = "4e", DsplSeq = 12 },
                    new ClassLevel { Name = "3e", DsplSeq = 13 },
                    new ClassLevel { Name = "2nde", DsplSeq = 14 },
                    new ClassLevel { Name = "1ere", DsplSeq = 15 },
                    new ClassLevel { Name = "Tle", DsplSeq = 16 }
                };
                context.AddRange(classLevels);

                var skills = new List<Skill> {
                    new Skill { Name = "skill1" },
                    new Skill { Name = "skill2" },
                    new Skill { Name = "skill3" },
                    new Skill { Name = "skill4" },
                    new Skill { Name = "skill5" },
                    new Skill { Name = "skill6" },
                    new Skill { Name = "skill7" },
                    new Skill { Name = "skill8" },
                    new Skill { Name = "skill9" },
                    new Skill { Name = "skill10" },
                    new Skill { Name = "skill11" },
                    new Skill { Name = "skill12" }
                };
                context.AddRange(skills);
                context.SaveChanges();

                var progElts = new List<ProgramElement> {
                    new ProgramElement { Name = "progElt1", SkillId = 1 },
                    new ProgramElement { Name = "progElt2", SkillId = 1 },
                    new ProgramElement { Name = "progElt3", SkillId = 2 },
                    new ProgramElement { Name = "progElt4", SkillId = 2 },
                    new ProgramElement { Name = "progElt5", SkillId = 3 },
                    new ProgramElement { Name = "progElt6", SkillId = 3 },
                    new ProgramElement { Name = "progElt7", SkillId = 1 },
                    new ProgramElement { Name = "progElt8", SkillId = 4 },
                    new ProgramElement { Name = "progElt9", SkillId = 4 },
                    new ProgramElement { Name = "progElt10", SkillId = 11 },
                    new ProgramElement { Name = "progElt11", SkillId = 9 },
                    new ProgramElement { Name = "progElt12", SkillId = 1 }
                };
                context.AddRange(progElts);

                var paymentTypes = new List<PaymentType> {
                    new PaymentType { Name = "espèces" },
                    new PaymentType { Name = "chèque" },
                    new PaymentType { Name = "virement" },
                    new PaymentType { Name = "paiement mobible" }
                };
                context.AddRange(paymentTypes);

                var pers = new List<Period> {
                    new Period { Name = "1er trimestre", Active = true },
                    new Period { Name = "2e trimestre", Active = false },
                    new Period { Name = "3e trimestre", Active = false }
                };
                context.AddRange(pers);

                // var feeTypes = new List<FeeType>
                // {
                //     new FeeType{Name = "cantine"},
                //     new FeeType{Name = "scolarité"},
                //     new FeeType{Name = "bibliothèque"},
                //     new FeeType{Name = "transport"}
                // };
                // context.AddRange(feeTypes);

                var evalTypes = new List<EvalType> {
                    new EvalType { Name = "devoir maison" },
                    new EvalType { Name = "oral" },
                    new EvalType { Name = "écrit" },
                    new EvalType { Name = "interro. écrite" },
                    new EvalType { Name = "exposé" },
                    new EvalType { Name = "travaux pratiques" },
                    new EvalType { Name = "rapport de stage" },
                    new EvalType { Name = "tenue de cahier" }
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
                    new Token {Name = "préfixe au nom", TokenString = "<M_MME>"}
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

                // foreach (var user in users)
                // {
                //     user.Photos.SingleOrDefault().IsApproved = true;
                //     userManager.CreateAsync(user, "password").Wait();
                //     userManager.AddToRoleAsync(user, "Professeur").Wait();
                // }

                var adminUser = new User
                {
                    UserName = "Admin",
                    FirstName = "admin",
                    LastName = "admin",
                    Email = "adminUser@educnotes.com",
                    EmailConfirmed = true,
                    ValidatedCode = true,
                    UserTypeId = 4
                };

                IdentityResult result = userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = userManager.FindByNameAsync("Admin").Result;
                    userManager.AddToRolesAsync(admin, new[] { "Admin", "Professeur" }).Wait();
                }

                //add students
                // for(int i = 0; i < 100; i++)
                // {
                //     byte sex = 0;
                //     if(i % 2 == 0)
                //         sex = 1;
                //     var Student = new User
                //     {
                //         UserName = "User" + i,
                //         FirstName = "Fuser" + i,
                //         LastName = "Luser" + i,
                //         Gender = sex,
                //         UserTypeId = 1
                //     };

                //     IdentityResult result1 = userManager.CreateAsync(Student, "password").Result;

                //     if(result1.Succeeded)
                //     {
                //         userManager.AddToRolesAsync(Student, new[] {"élève"}).Wait();
                //     }
                // }

            }
        }
    }
}