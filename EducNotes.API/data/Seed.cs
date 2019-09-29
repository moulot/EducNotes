using System.Collections.Generic;
using System.Linq;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace EducNotes.API.Data
{
    public class Seed
    {
        // private readonly UserManager<User> _userManager;
        // private readonly RoleManager<Role> _roleManager;

        // public Seed(UserManager<User> userManager, RoleManager<Role> roleManager)
        // {
        //     _userManager = userManager;
        //     _roleManager = roleManager;
        // }

        public static void SeedUsers(DataContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if(!userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                var roles = new List<Role>
                {
                    new Role{Name = "élève"},
                    new Role{Name = "Professeur"},
                    new Role{Name = "Parent"},
                    new Role{Name = "Admin"}
                };

                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).Wait();
                }

                var city = new City {Name = "Abidjan"};
                context.Add(city).Wait();

                var districts = new List<District>
                {
                    new District{Name = "Cocody", CityId = 1},
                    new District{Name = "Angré", CityId = 1},
                    new District{Name = "2 Plateaux", CityId = 1},
                    new District{Name = "Djibi", CityId = 1},
                };
                foreach(var district in districts)
                {
                    context.Add(district).Wait();
                }

                var inscTypes = new List<InscriptionType>
                {
                    new InscriptionType{Name = "FromParent"},
                    new InscriptionType{Name = "FromSchool"}
                };
                foreach(var inscType in inscTypes)
                {
                    context.Add(inscType).Wait();
                }

                foreach (var user in users)
                {
                    user.Photos.SingleOrDefault().IsApproved = true;
                    userManager.CreateAsync(user, "password").Wait();
                    userManager.AddToRoleAsync(user, "Professeur").Wait();
                }

                var adminUser = new User
                {
                    UserName = "Admin"
                };

                IdentityResult result = userManager.CreateAsync(adminUser, "password").Result;

                if(result.Succeeded)
                {
                    var admin = userManager.FindByNameAsync("Admin").Result;
                    userManager.AddToRolesAsync(admin, new[] {"Admin","Professeur"}).Wait();
                }

                //add students
                for(int i = 0; i < 100; i++)
                {
                    var Student = new User
                    {
                        UserName = "User" + i,
                        FirstName = "Fuser" + i,
                        LastName = "Luser" + i,
                        Gender = i % 2 == 0 ? 0 : 1
                    };

                    IdentityResult result = _userManager.CreateAsync(Student, "password").Result;

                    if(result.Succeeded)
                    {
                        _userManager.AddToRolesAsync(Student, new[] {"élève"}).Wait();
                    }
                }
            }
        }
    }
}