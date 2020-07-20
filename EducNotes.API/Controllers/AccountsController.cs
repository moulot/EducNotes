using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EducNotes.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountsController : ControllerBase
  {
    private readonly IEducNotesRepository _repo;
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    public IConfiguration _config { get; }
    public UserManager<User> _userManager { get; }
    CultureInfo frC = new CultureInfo("fr-FR");
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    string password;
    int registrationEmailId, tuitionId, nextYearTuitionId, newRegToBePaidEmailId;

    public AccountsController(IEducNotesRepository repo, IMapper mapper, DataContext context,
     IConfiguration config, UserManager<User> userManager)
    {
      _repo = repo;
      _mapper = mapper;
      _context = context;
      _config = config;
      _userManager = userManager;
      teacherTypeId = _config.GetValue<int>("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int>("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int>("AppSettings:studentTypeId");
      registrationEmailId = _config.GetValue<int>("AppSettings:registrationEmailId");
      newRegToBePaidEmailId = _config.GetValue<int>("AppSettings:newRegToBePaidEmailId");
      tuitionId = _config.GetValue<int>("AppSettings:tuitionId");
      nextYearTuitionId = _config.GetValue<int>("AppSettings:nextYearTuitionId");
      password = _config.GetValue<String>("AppSettings:defaultPassword");
    }

    [AllowAnonymous]
    [HttpGet("PhoneCode/{userId}/{num}")]
    public async Task<IActionResult> PhoneCode(int userId, string num)
    {
      bool codeSent = false;
      var user = await _context.Users.FirstAsync(u => u.Id == userId);
      user.PhoneNumber = num;
      _repo.Update(user);
      var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, num);
      Sms sms = new Sms();
      sms.To = num;
      sms.ToUserId = user.Id;
      sms.SmsTypeId = _config.GetValue<int>("AppSettings:SmsValidationTypeId");
      sms.Content = "SVP saisir le code " + token + " pour valider votre numéro sur Educ'Notes. " +
      "Vous ne serez jamais contacter pour ce code. Ne le révéler à persone.";
      sms.InsertUserId = _config.GetValue<int>("AppSettings:Admin");
      sms.InsertDate = DateTime.Now;
      sms.UpdateDate = DateTime.Now;
      _repo.Add(sms);

      if(await _repo.SaveAll())
      {
        codeSent = true;
        return Ok(codeSent);
      }

      return BadRequest("problème lors de l'envoi du sms de validation");
    }

    [AllowAnonymous]
    [HttpPost("ConfirmPhoneNumber")]
    public async Task<IActionResult> ConfirmPhoneNumber(ConfirmEmailPhoneDto confirmEmailPhoneDto)
    {
      int userId = Convert.ToInt32(confirmEmailPhoneDto.UserId);
      string token = confirmEmailPhoneDto.Token;

      bool phoneNumValidated = false;
      var user = await _context.Users.FirstAsync(u => u.Id == userId);
      var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, token);

      if(result.Succeeded)
      {
        phoneNumValidated = true;
      }

      return Ok(new {
        phoneOK = phoneNumValidated,
        user
      });
    }

    [AllowAnonymous]
    [HttpPost("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailPhoneDto confirmEmailPhoneDto)
    {
      string userId = confirmEmailPhoneDto.UserId;
      string token = confirmEmailPhoneDto.Token;
      Boolean success = false;
      Boolean validated = false;

      var user = await _userManager.FindByIdAsync(userId);
      List<UserForDetailedDto> children = new List<UserForDetailedDto>();
      if(user != null)
      {
        if(user.EmailConfirmed)
        {
          success = true;
          validated = user.Validated;
        }
        else
        {
          var result = await _userManager.ConfirmEmailAsync(user, token);
          if(result.Succeeded)
          {
            success = true;
          }
        }

        //get children...
        children = await _repo.GetAccountChildren(user.Id);
      }

      return Ok(new {
        emailOK = success,
        accountValidated = validated,
        user,
        children
      });
    }

  }
}