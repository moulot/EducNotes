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
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using EducNotes.API.Helpers;
using Microsoft.Extensions.Options;
using EducNotes.API.data;

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
    private DataContext _context;
    // public readonly MasterDataContext _context1;
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private Cloudinary _cloudinary;
    int parentRoleId, memberRoleId, moderatorRoleId, adminRoleId, teacherRoleId;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    int parentIsncrTypeId, schoolInscrTypeId, updateAccountEmailId, resetPwdEmailId;
    public ICacheRepository _cache { get; }

    public AuthController(IConfiguration config, IMapper mapper, IEducNotesRepository repo,
      UserManager<User> userManager, SignInManager<User> signInManager, DataContext context,
      IOptions<CloudinarySettings> cloudinaryConfig, ICacheRepository cache)
    {
      _cache = cache;
      _context = context;
      _repo = repo;
      _config = config;
      _mapper = mapper;
      _userManager = userManager;
      _signInManager = signInManager;
      teacherTypeId = _config.GetValue<int>("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int>("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int>("AppSettings:studentTypeId");
      parentRoleId = _config.GetValue<int>("AppSettings:parentRoleId");
      memberRoleId = _config.GetValue<int>("AppSettings:memberRoleId");
      moderatorRoleId = _config.GetValue<int>("AppSettings:moderatorRoleId");
      adminRoleId = _config.GetValue<int>("AppSettings:adminRoleId");
      teacherRoleId = _config.GetValue<int>("AppSettings:teacherRoleId");
      parentIsncrTypeId = _config.GetValue<int>("AppSettings:parentInscTypeId");
      schoolInscrTypeId = _config.GetValue<int>("AppSettings:schoolInscTypeId");
      updateAccountEmailId = _config.GetValue<int>("AppSettings:updateAccountEmailId");
      resetPwdEmailId = _config.GetValue<int>("AppSettings:resetPwdEmailId");

      _cloudinaryConfig = cloudinaryConfig;
      Account acc = new Account(
          _cloudinaryConfig.Value.CloudName,
          _cloudinaryConfig.Value.ApiKey,
          _cloudinaryConfig.Value.ApiSecret
      );

      _cloudinary = new Cloudinary(acc);
    }

    [HttpPost("{id}/setPassword/{password}")] // edition du mot de passe apres validation du code
    public async Task<IActionResult> setPassword(int id, string password)
    {
      var user = await _repo.GetUser(id, false);
      if (user != null)
      {
        bool emailCOnfirmed = user.EmailConfirmed;
        var newPassword = _userManager.PasswordHasher.HashPassword(user, password);
        user.PasswordHash = newPassword;
        user.Validated = true;
        user.EmailConfirmed = true;
        if (!emailCOnfirmed)
          user.ValidationDate = DateTime.Now;
        var res = await _userManager.UpdateAsync(user);

        if (res.Succeeded)
        {
          var mail = new EmailFormDto();
          if (emailCOnfirmed)
          {
            // Envoi de mail pour la confirmation de la mise a jour du mot de passe
            mail.subject = "mot de passe modifié";
            mail.content = "bonjour <b> " + user.LastName + " " + user.FirstName + "</b>, votre nouveau mot de passe a bien eté enregistré...";
            mail.toEmail = user.Email;
          }
          else
          {
            mail.subject = "Compte confirmé";
            mail.content = "<b> " + user.LastName + " " + user.FirstName + "</b>, votre compte a bien été enregistré";
            mail.toEmail = user.Email;
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

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword(UserForResetPwdDto userData)
    {
      bool resetOK = false;
      var username = userData.Username;
      var email = userData.Email;
      // find user with username and email as inputs
      var user = await _repo.GetUserByEmailAndLogin(username, email);
      if (user != null)
      {
        if (!user.EmailConfirmed)
          return BadRequest("compte pas encore activé....");
        // send 'token for password reset' email
        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        if (await _repo.SendResetPasswordLink(user, resetToken))
        {
          // envoi effectué
          user.ForgotPasswordCount += 1;
          user.ForgotPasswordDate = DateTime.Now;
          if (await _repo.SaveAll())
          {
            resetOK = true;
            return Ok(resetOK);
          }
          else
          {
            return BadRequest("problème pour envoyer le email");
          }
        }

        return BadRequest("problème pour envoyer le email");
      }
      else
      {
        return Ok(resetOK);
      }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
    {
      // RollbarLocator.RollbarInstance.Configure(new RollbarConfig("13e83bd7d4794996b0535c4990db269e"));
      // RollbarLocator.RollbarInstance.Info("Rollbar is configured properly.");

      // verification de l'existence du userName
      if (await _repo.UserNameExist(userForLoginDto.Username.ToLower(), 0))
      {
        var user = await _userManager.FindByNameAsync(userForLoginDto.Username.ToLower());
        if (!user.AccountDataValidated)
          return BadRequest("l'utilisateur n'a pas confirmé son email/mobile.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, true);

        if (result.Succeeded)
        {
          var appUser = await _userManager.Users.Include(p => p.Photos)
            .FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());

          var userToReturn = _mapper.Map<UserForListDto>(appUser);

          //get the current period
          Period CurrentPeriod = await _repo.GetPeriodFromDate(DateTime.Now);

          //get school settings
          var settings = await _repo.GetSettings();

          return Ok(new
          {
            token = await GenerateJwtToken(appUser),
            user = userToReturn,
            currentPeriod = CurrentPeriod,
            settings
          });
        }

        var lockedOut = false;
        if (result.IsLockedOut)
        {
          lockedOut = true;
          return Ok(new
          {
            userName = user.LastName + " " + user.FirstName,
            lockedOut
          });
        }
        // else
        // {
        //   return BadRequest("login ou mot de passe incorrect...");
        // }
      }

      var loginPwdFailed = true;
      return Ok(new
      {
        failed = loginPwdFailed
      });
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

    [HttpPost("validateChildAccounts")]
    public async Task<IActionResult> validateChildAccounts([FromForm] ChildrenForEditDto users)
    {
      bool usersOk = await _repo.UpdateChildren(users);
      if (usersOk)
        return Ok();
      else
        return BadRequest("problème pour valider les enfants");
    }

    [HttpPost("codeValidation")] // validation du code utilisateur emit a son inscription
    public async Task<IActionResult> codeValidation([FromBody] UserForCodeValidationDto userForCodeValidation)
    {
      var user = await _repo.GetSingleUser(userForCodeValidation.UserName);
      if (user != null)
      {
        if (user.Validated == true)
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

    // [HttpGet("ResetPassword/{code}")]
    // public async Task<IActionResult> ResetPassword(string code)
    // {
    //   var user = await _repo.GetUserByCode(code);
    //   if (user != null)
    //   {
    //     if (DateTime.Now.AddHours(-3) >= user.ForgotPasswordDate.Value) // le delai des 3 Heures a expiré
    //       return BadRequest("Désolé ce lien a expiré....");
    //     else
    //       return Ok(new
    //       {
    //         user = _mapper.Map<UserForDetailedDto>(user)
    //       });
    //   }
    //   return BadRequest("lien introuvable");
    // }

    [HttpPost("LastValidation")]
    public async Task<IActionResult> LastValidation(int id, UserForUpdateDto userForUpdateDto)
    {
      var userToUpdate = await _userManager.FindByNameAsync(userForUpdateDto.UserName);
      if (userToUpdate != null)
      {
        userToUpdate.EmailConfirmed = true;
        _repo.Update(userToUpdate);

        if (await _repo.SaveAll())
          return base.Ok(new
          {
            token = GenerateJwtToken(userToUpdate).Result,
            user = _mapper.Map<UserForDetailedDto>(userToUpdate)
          });

        return BadRequest("impossible de mettre a jour ce compte");

      }
      return BadRequest("Compte introuvable");
    }

    [HttpGet("GetAllCities")]
    public async Task<IActionResult> GetAllCities()
    {
      return Ok(await _repo.GetAllCities());
    }

    [HttpGet("GetLevels")]
    public async Task<IActionResult> GetLevels()
    {
      return Ok(await _repo.GetLevels());
    }

    [HttpGet("GetDistrictsByCityId/{id}")]
    public async Task<IActionResult> GetAllGetDistrictsByCityIdCities(int id)
    {
      return Ok(await _repo.GetAllGetDistrictsByCityIdCities(id));
    }

    // enregistrement de préinscription : pere, mere et enfants
    [HttpPost("{parentId}/ParentSelfPreinscription")]
    public async Task<IActionResult> ParentSelfPreinscription(int parentId, [FromBody] ParentSefRegisterDto model)
    {
      // récuperation de tous les users
      var usersToUpdate = new List<UserForUpdateDto>();
      usersToUpdate.Add(model.parent);
      int total = 0;
      foreach (var children in model.children)
      {
        usersToUpdate.Add(children);
        total += total + children.Products.Sum(x => Convert.ToInt32(x.Price));
      }
      var userscode = await _repo.ParentSelfInscription(parentId, usersToUpdate);

      // enregistrement des services séléctionnés

      foreach (var child in model.children)
      {
        int childId = userscode.FirstOrDefault(s => s.SpaCode == child.SpaCode).UserId;
        var newOrder = new Order
        {
          TotalHT = total,
          AmountTTC = total,
          Discount = 0,
          TVA = 0,
          //TO BE COPED CAREFULLY ON THE NEW REGISTRATION PAGE
          FatherId = parentId,
          ChildId = childId
        };
        _repo.Add(newOrder);

        foreach (var prod in child.Products)
        {
          var newOrderLine = new OrderLine
          {
            OrderId = newOrder.Id,
            ProductId = prod.Id,
            AmountHT = prod.Price,
            AmountTTC = prod.Price,
            Discount = 0,
            Qty = 1,
            TVA = 0
          };
          _repo.Add(newOrderLine);
        }
      }
      if (await _repo.SaveAll())
        return Ok(userscode);

      return BadRequest();
    }

    [HttpPost("{id}/TeacherSelfPreinscription")] // enregistrement de préinscription : professeur
    public async Task<IActionResult> TeacherSelfPreinscription(int id, [FromBody] UserForUpdateDto userToUpdate)
    {
      var userFromRepo = await _repo.GetUser(id, true);
      userToUpdate.UserName = userToUpdate.UserName.ToLower();
      var user = _mapper.Map(userToUpdate, userFromRepo);
      user.Id = id;
      user.UserTypeId = teacherTypeId;
      var newPassword = _userManager.PasswordHasher.HashPassword(user, userToUpdate.Password);
      user.PasswordHash = newPassword;
      user.Validated = true;
      user.EmailConfirmed = true;
      user.ValidationDate = DateTime.Now;
      var result = await _userManager.UpdateAsync(user);
      if (result.Succeeded)
      {

        // envoi de l'email pour confirmation de l'activation du compte
        var mail = new EmailFormDto
        {
          subject = "Compte confirmé",
          content = "<b> " + user.LastName + " " + user.FirstName + "</b>, votre compte a bien été enregistré",
          toEmail = user.Email
        };
        await _repo.SendEmail(mail);
        Period CurrentPeriod = await _context.Periods.Where(p => p.Active == true).FirstOrDefaultAsync();
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

    [HttpGet("VerifyUserName/{currentUserId}/{userName}")]
    public async Task<IActionResult> VerifyUserName(string userName, int currentUserId)
    {
      return Ok(await _repo.UserNameExist(userName, currentUserId));
    }

    [HttpPost("{id}/setUserAccountData")]
    public async Task<IActionResult> setUserAccountData(int id, UserDataToUpdateDto userData)
    {
      // List<EmailTemplate> emailTemplates = await _cache.GetEmailTemplates();

      var user = await _repo.GetUser(id, false);
      if (user != null)
      {
        var newPassword = _userManager.PasswordHasher.HashPassword(user, userData.Pwd);
        user.UserName = userData.UserName.ToLower();
        user.NormalizedUserName = userData.UserName.ToUpper();
        user.PasswordHash = newPassword;
        user.AccountDataValidated = true;
        if (user.UserTypeId != studentTypeId)
          user.Validated = true;
        user.ValidationDate = DateTime.Now;
        var res = await _userManager.UpdateAsync(user);
        if (res.Succeeded)
        {
          // var template = emailTemplates.First(t => t.Id == updateAccountEmailId);
          // var email = await _repo.SetEmailForAccountUpdated(template.Subject, template.Body,
          //   user.LastName, user.Gender, user.Email, user.Id);
          // _context.Add(email);

          var userToReturn = _mapper.Map<UserForListDto>(user);
          await _cache.LoadUsers();
          return Ok(new {
            user = userToReturn
          });
        }

        return BadRequest("problème pour mettre à jour les données");
      }

      return NotFound();
    }

    [HttpPost("{userId}/AddPhotoForUser")]
    public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreationDto)
    {
      var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(a => a.Id == userId);
      user.TempData = 2;
      var file = photoForCreationDto.File;
      var uploadResult = new ImageUploadResult();

      if (file.Length > 0)
      {
        using (var stream = file.OpenReadStream())
        {
          var uploadParams = new ImageUploadParams()
          {
            File = new FileDescription(file.Name, stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
          };

          uploadResult = _cloudinary.Upload(uploadParams);
        }
      }

      photoForCreationDto.Url = uploadResult.SecureUri.ToString();
      photoForCreationDto.PublicId = uploadResult.PublicId;

      var photo = _mapper.Map<Photo>(photoForCreationDto);

      if (!user.Photos.Any(u => u.IsMain))
      {
        photo.IsMain = true;
        photo.IsApproved = true;
      }
      user.Photos.Add(photo);

      if (await _repo.SaveAll())
        return Ok();

      return BadRequest("Could not add the photo");
    }

    [HttpGet("GetClassLvelProducts/{classLevelId}")]
    public async Task<IActionResult> GetProducts(int classLevelId)
    {
      var prods = new List<Product>();
      var prods_for_levels = await _context.ClassLevelProducts
                                  .Where(c => c.ClassLevelId == classLevelId)
                                  .ToListAsync();
      var ids = prods_for_levels.Select(p => p.ProductId);

      if (prods_for_levels != null)
        prods = await _context.Products.Include(p => p.Periodicity).Where(p => p.IsRequired == true || !ids.Contains(p.Id)).ToListAsync();
      else
        prods = await _context.Products.Include(p => p.Periodicity).Where(p => p.IsRequired == true).ToListAsync();

      var prodsToReturn = new List<ServiceDetailsDto>();
      foreach (var prod in prods)
      {
        var produit = new ServiceDetailsDto
        {
          Id = prod.Id,
          Price = Convert.ToDecimal(prod.Price),
          Details = prod.Name + " : " + prod.Price.ToString(),
          IsRequired = prod.IsRequired
        };
        if (prod.IsByLevel)
        {
          var selectedLevelProd = prods_for_levels.FirstOrDefault(p => p.ProductId == prod.Id);
          // produit.Details += prods_for_levels.FirstOrDefault(p => p.ProductId == prod.Id).Price.ToString();
          produit.Details += selectedLevelProd.Price.ToString();
          produit.Price += selectedLevelProd.Price;
        }
        if (prod.IsPeriodic)
          produit.Details += " par période (" + prod.Periodicity.Name + ")";
        else
          produit.Details += " par échéances";
        prodsToReturn.Add(produit);
      }

      return Ok(prodsToReturn);
    }

    [HttpGet("GetClasses")]
    public async Task<IActionResult> GetClasses()
    {
      return Ok(await _context.Classes.ToListAsync());
    }

    [HttpGet("GetUser/{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
      var user = await _repo.GetUser(id, false);
      return Ok(user);
    }

    [HttpGet("LoginPageInfos")]
    public async Task<IActionResult> GetLoginPageInfos()
    {
      var infos = await _context.LoginPageInfos.OrderBy(o => o.Title).ToListAsync();
      return Ok(infos);
    }

    [HttpGet("{userId}/TeacherConfirmEmail")]
    public async Task<IActionResult> SendTeacherConfirmEmail(int userId)
    {
      Boolean emailSent = await _repo.SendTeacherConfirmEmail(userId);
      return Ok(emailSent);
    }

    // [HttpPost("AddFile")]
    // public async Task<IActionResult> AddFile([FromForm]PhotoForCreationDto photoForCreationDto)
    // {
    //     var file = photoForCreationDto.File;

    //     var uploadResult = new ImageUploadResult();

    //     if (file.Length > 0)
    //     {
    //         using (var stream = file.OpenReadStream())
    //         {
    //             var uploadParams = new ImageUploadParams()
    //             {
    //                 File = new FileDescription(file.Name, stream)
    //             };

    //             uploadResult = _cloudinary.Upload(uploadParams);
    //         }
    //     }

    //     photoForCreationDto.Url = uploadResult.SecureUri.ToString();
    //     photoForCreationDto.PublicId = uploadResult.PublicId;


    //     var fichier = _mapper.Map<Fichier>(photoForCreationDto);
    //     fichier.Description = photoForCreationDto.File.FileName;
    //     _repo.Add(fichier);

    //     if (await _repo.SaveAll())
    //         return Ok();

    //     return BadRequest("Could not add the photo");
    // }

    // [HttpGet("GetAllFiles")]
    // public async Task<IActionResult> GetAllFiles()
    // {
    //     return Ok(await _context.Fichiers.ToListAsync());
    // }
  }
}