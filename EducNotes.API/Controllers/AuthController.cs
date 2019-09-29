using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
//using RestSharp;

namespace EducNotes.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEducNotesRepository _repo;
        private readonly DataContext _context;
       int parentRoleId,memberRoleId,moderatorRoleId,adminRoleId,teacherRoleId;
       int teacherTypeId,parentTypeId,studentTypeId,adminTypeId;
       int parentIsncrTypeId,schoolInscrTypeId;

        public AuthController(IConfiguration config, IMapper mapper, IEducNotesRepository repo,
            UserManager<User> userManager, SignInManager<User> signInManager, DataContext context)
        {
            _context = context;
            _repo = repo;
            _config = config;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
             teacherTypeId =  _config.GetValue<int>("AppSettings:teacherTypeId");
            parentTypeId =  _config.GetValue<int>("AppSettings:parentTypeId");
            adminTypeId =  _config.GetValue<int>("AppSettings:adminTypeId");
            studentTypeId =  _config.GetValue<int>("AppSettings:studentTypeId");
            parentRoleId = _config.GetValue<int>("AppSettings:parentRoleId");
            memberRoleId = _config.GetValue<int>("AppSettings:memberRoleId");
            moderatorRoleId = _config.GetValue<int>("AppSettings:moderatorRoleId");
            adminRoleId = _config.GetValue<int>("AppSettings:adminRoleId");
            teacherRoleId = _config.GetValue<int>("AppSettings:teacherRoleId");
             parentIsncrTypeId = _config.GetValue<int>("AppSettings:parentInscTypeId");
            schoolInscrTypeId = _config.GetValue<int>("AppSettings:schoolInscTypeId");
        }

        [HttpPost("{id}/setPassword/{password}")] // edition du mot de passe apres validation du code
        public async Task<IActionResult> setPassword(int id, string password)
        {
            var user =await  _repo.GetUser(id,false);
            if(user!=null)
            {
                 bool emailCOnfirmed = user.EmailConfirmed;
                  var newPassword=_userManager.PasswordHasher.HashPassword(user,password);
                  user.PasswordHash = newPassword;
                  user.ValidatedCode = true;
                  user.EmailConfirmed =true;
                  if(!emailCOnfirmed)
                  user.ValidationDate = DateTime.Now;
                  var res = await _userManager.UpdateAsync(user);
           
                 if(res.Succeeded)
                 {
                   var mail = new EmailFormDto();
                   if(emailCOnfirmed)
                   {
                     // Envoi de mail pour la confirmation de la mise a jour du mot de passe
                      mail.subject ="mot de passe modifié";
                      mail.content ="bonjour <b> "+user.LastName + " "+ user.FirstName + "</b>, votre nouveau mot de passe a bien eté enregistré...";
                      mail.toEmail = user.Email;
                   }
                   else {
                      mail.subject ="Compte confirmé";
                      mail. content ="<b> "+user.LastName + " "+ user.FirstName + "</b>, votre compte a bien été enregistré";
                      mail. toEmail = user.Email;
                   }
                     await _repo.SendEmail(mail);
                    var userToReturn = _mapper.Map<UserForListDto>(user);
                    return Ok(new
                    {
                        token = await GenerateJwtToken(user),
                        user = userToReturn

                    });
                 }
                 return BadRequest("impossible de terminé l'action");
            }
            return NotFound();
        }

        [HttpGet("{email}/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
         // recherche du compte de l 'email
         var user =await _repo.GetUserByEmail(email);
         if(user != null)
         {
           if(!user.EmailConfirmed)
           return BadRequest("compte pas encore activé....");
           // envoi du mail pour le reset du password
           user.ValidationCode = Guid.NewGuid().ToString();
           if(await _repo.SendResetPasswordLink(user.Email,user.ValidationCode))
           {
             // envoi effectuer
             user.ForgotPasswordCount+=1;
             user.ForgotPasswordDate = DateTime.Now;
             await _repo.SaveAll();
             return Ok();

           }
           return BadRequest("echec d'envoi du mail");
         }
         return BadRequest("email non trouvé. Veuillez réessayer");
          
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username.ToLower());
            if (!user.ValidatedCode)
                return BadRequest("Compte non validé pour l'instant...");

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (result.Succeeded)
            {
              var appUser = await _userManager.Users.Include(p => p.Photos)
                  .FirstOrDefaultAsync(u => u.NormalizedUserName ==
                      userForLoginDto.Username.ToUpper());

              var userToReturn = _mapper.Map<UserForListDto>(appUser);

              //get the current period
              Period CurrentPeriod = await _context.Periods.Where(p => p.Active == 1).FirstOrDefaultAsync();


              return Ok(new
              {
                  token = await GenerateJwtToken(appUser),
                  user = userToReturn,
                  currentPeriod = CurrentPeriod
              });
            }

                return BadRequest("Login ou mot de passe incorrecte...");
         }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenhandler = new JwtSecurityTokenHandler();

            var token = tokenhandler.CreateToken(tokenDescriptor);

            return tokenhandler.WriteToken(token);
        }

        private string GenerateJwtTokenForEmail(User user)
        {
            var claims = new List<Claim>
                    {
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(ClaimTypes.Name, user.UserName)
                            };



            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        
          [HttpPost("codeValidation")] // validation du code utilisateur emit a son inscription
         public async Task<IActionResult> codeValidation([FromBody]UserForCodeValidationDto userForCodeValidation)
        {
         var user =await _repo.GetSingleUser(userForCodeValidation.UserName);
            if (user != null)
            {
                if (user.ValidatedCode == true)
                {
                    return BadRequest("compte déjà confirmé...");
                }
                else
                {
                    // user.ValidatedCode = true;
                    // await _userManager.UpdateAsync(user);
                     var userToReturn = _mapper.Map<UserForDetailedDto>(user);

                    // _userManager.AddToRoleAsync(user, "Member").Wait();
                    // user.EmailConfirmed = true;
                    // if (await _repo.SaveAll())
                    // {

                    //     return base.Ok(new
                    //     {
                    //         token = GenerateJwtToken(user).Result,
                    //         user = userToReturn
                    //     });
                    // }

                    return Ok(userToReturn);
                    //return Ok("");

                }


            }
            return NotFound();

        }

         [HttpGet("emailValidation/{code}")]
        public async Task<IActionResult> emailValidation(string code)
        {

            // int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _repo.GetUserByCode(code);
            
            if (user != null)
            {
               if (user.EmailConfirmed == true)
               return BadRequest("cet compte a déja été confirmé...");
               int maxChild = 0;   
               if(user.UserTypeId ==_config.GetValue<int>("AppSettings:parentTypeId") && user.UserName == user.ValidationCode ) 
               {
                  var children = await _repo.GetChildren(user.Id);
                  maxChild = children.Count();
               }  
                    
              return Ok(new {
                            user = _mapper.Map<UserForDetailedDto>(user) ,
                            maxChild = maxChild
                            });
                  
            }
            return NotFound();

        }

        [HttpGet("ResetPassword/{code}")]
        public async Task<IActionResult> ResetPassword(string code)
        {
            var user = await _repo.GetUserByCode(code);
            if (user != null)
            {
               if(DateTime.Now.AddHours(-3)>=user.ForgotPasswordDate.Value) // le delai des 3 Heures a expiré
               return BadRequest("Désolé ce lien a expiré....");
               else
              return Ok(new
                    {
                        user = _mapper.Map<UserForDetailedDto>(user)
                    });

            }
            return BadRequest("lien introuvable");
        }

        [HttpPost("LastValidation")]
        public async Task<IActionResult> LastValidation(int id, UserForUpdateDto userForUpdateDto)
        {
            var userToUpdate = await _userManager.FindByNameAsync(userForUpdateDto.UserName);
            if (userToUpdate != null)
            {
                userToUpdate.EmailConfirmed = true;
                _repo.Update(userToUpdate);

                if(await _repo.SaveAll())
                   return base.Ok(new
                         {
                            token = GenerateJwtToken(userToUpdate).Result,
                            user = _mapper.Map<UserForDetailedDto>(userToUpdate)
                        });
                
                return BadRequest("impossible de mettre a jour ce compte");

            }
             return BadRequest("Compte introuvable");


        }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////MOHAMED KABORE ///////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [HttpGet("GetEmails")] 
        public async Task<IActionResult> GetEmails()
        {
            return Ok(await _repo.GetEmails());
        }

        [HttpGet("GetUserNames")] 
        public async Task<IActionResult> GetUserNames()
        {
            return Ok(await _repo.GetUserNames());
        }

        [HttpGet("GetAllCities")]
        public async Task<IActionResult>GetAllCities()
        {
          return Ok (await _repo.GetAllCities());
        }


        [HttpGet("GetLevels")]
        public async Task<IActionResult>GetLevels()
        {
          return Ok (await _repo.GetLevels());
        }

        [HttpGet("GetDistrictsByCityId/{id}")]
        public async Task<IActionResult>GetAllGetDistrictsByCityIdCities(int id)
        {
        return Ok (await _repo.GetAllGetDistrictsByCityIdCities(id));
        }

        [HttpPost("{id}/ParentSelfPreinscription")] // enregistrement de préinscription : perer , mere et enfants
        public async Task<IActionResult> ParentSelfPreinscription( int id,[FromBody]ParentSefRegisterDto model)
        {
            int user2Id = 0;
                try
                {
                  // mise a jour du user1
                  var userFromRepo =await _repo.GetUser(id,true);
                  model.user1.UserName = model.user1.UserName.ToLower();
                  var user1 = _mapper.Map(model.user1, userFromRepo);
                  var newPassword=_userManager.PasswordHasher.HashPassword(user1,model.user1.Password);
                  user1.Id = id;
                  user1.PasswordHash = newPassword;
                  user1.ValidatedCode = true;
                  user1.EmailConfirmed =true;
                  user1.ValidationDate = DateTime.Now;
                  var res = await _userManager.UpdateAsync(user1);
                                
                
                    //ajout du user 2
                    if(!string.IsNullOrEmpty(model.user2.LastName) &&  !string.IsNullOrEmpty(model.user2.FirstName))
                    {
                      model.user2.UserName = model.user2.UserName.ToLower();
                        var user2 = _mapper.Map<User>(model.user2);
                        user2.UserTypeId =  parentTypeId;
                        user2Id = await _repo.AddUserPreInscription(Guid.NewGuid(),user2,parentRoleId,false);
                    }

                    var  c= await _repo.GetChildren(id);
                    var childrenFromRepo = c.ToList();

                    for (int i = 0; i < model.children.Count(); i++)
                        {
                            var childFromForm = model.children[i];
                            var childFromRepo = childrenFromRepo[i];
                            int classLevelId = Convert.ToInt32(childFromForm.LevelId);
                            var child =  _mapper.Map(childFromForm, childFromRepo);
                            child.UserName = child.UserName.ToLower();
                           var newPass=_userManager.PasswordHasher.HashPassword(child,childFromForm.Password);
                            child.Id = childFromRepo.Id;
                            child.UserTypeId = studentTypeId;
                            child.PasswordHash = newPass;
                            child.ValidatedCode = true;
                            child.EmailConfirmed =true;
                            child.ValidationDate = DateTime.Now;
                            var resultat = await _userManager.UpdateAsync(child);
                           //_repo.Update(child);
                          // child.UserTypeId =  studentTypeId;
                            // int userId = 0;
                            // userId=await _repo.AddUserPreInscription(Guid.NewGuid(),child,memberRoleId);

                            //enregistrement de l inscription
                            var insc = new Inscription {
                              InsertDate = DateTime.Now,
                              ClassLevelId = classLevelId,
                              UserId = child.Id,
                              InsertUserId = id,
                              InscriptionTypeId = parentIsncrTypeId,

                              Validated = false
                            };
                              _repo.Add(insc);

                              if(user2Id>0)
                              {
                                  var userlink = new UserLink{
                                    UserId = child.Id ,
                                    UserPId = user2Id                           
                                  };
                                  _repo.Add(userlink);
                              }
                            
                                                    
                        } 

                        if(!await _repo.SaveAll())
                        return BadRequest();

                         // envoi de l'email pour confirmation de l'activation du compte au premier utilisateur
                        var mail = new EmailFormDto{
                          subject ="Compte confirmé",
                          content ="<b> "+user1.LastName + " "+ user1.FirstName + "</b>, votre compte a bien été enregistré",
                          toEmail = user1.Email
                        };
                        await _repo.SendEmail(mail);
                        Period CurrentPeriod = await _context.Periods.Where(p => p.Active == 1).FirstOrDefaultAsync();
                        return Ok(new
                        {
                            token = await GenerateJwtToken(user1),
                            user = _mapper.Map<UserForListDto>(user1),
                            currentPeriod = CurrentPeriod
                        });
                        
            }
            catch (System.Exception ex)
            {
                
                return BadRequest(ex);
            }
        }
     
     
     
        [HttpPost("{id}/TeacherSelfPreinscription")] // enregistrement de préinscription : perer , mere et enfants
        public async Task<IActionResult> TeacherSelfPreinscription( int id,[FromBody]UserForRegisterDto userToUpdate)
        {
            var userFromRepo =await _repo.GetUser(id,true);
            userToUpdate.UserName = userToUpdate.UserName.ToLower();
            var user= _mapper.Map(userToUpdate, userFromRepo);
            user.Id = id;
            user.UserTypeId = teacherTypeId;
            var newPassword=_userManager.PasswordHasher.HashPassword(user,userToUpdate.Password);
            user.PasswordHash = newPassword;
            user.ValidatedCode = true;
            user.EmailConfirmed =true;
            user.ValidationDate = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);
            if(result.Succeeded)
            {

              // envoi de l'email pour confirmation de l'activation du compte
              var mail = new EmailFormDto{
                subject ="Compte confirmé",
                content ="<b> "+user.LastName + " "+ user.FirstName + "</b>, votre compte a bien été enregistré",
                toEmail = user.Email
              };
              await _repo.SendEmail(mail);
                 Period CurrentPeriod = await _context.Periods.Where(p => p.Active == 1).FirstOrDefaultAsync();
                  return Ok(new
                  {
                      token = await GenerateJwtToken(user),
                      user = _mapper.Map<UserForListDto>(user),
                      currentPeriod = CurrentPeriod
                  });
            }
            return BadRequest("impossible de terminer l'opération");
        }

        [HttpGet("{email}/VerifyEmail")]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            return Ok(await _repo.EmailExist(email));
        }

    }
}