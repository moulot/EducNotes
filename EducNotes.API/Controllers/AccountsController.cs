using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.data;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EducNotes.API.Controllers {
  [Route ("api/[controller]")]
  [ApiController]
  public class AccountsController : ControllerBase {
    private readonly IEducNotesRepository _repo;
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    public IConfiguration _config { get; }
    public UserManager<User> _userManager { get; }
    CultureInfo frC = new CultureInfo ("fr-FR");
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    string password;
    int tuitionId, nextYearTuitionId, newRegToBePaidEmailId;
    public ICacheRepository _cache { get; }

    public AccountsController (IEducNotesRepository repo, IMapper mapper, DataContext context,
      IConfiguration config, UserManager<User> userManager, ICacheRepository cache) {
      _cache = cache;
      _repo = repo;
      _mapper = mapper;
      _context = context;
      _config = config;
      _userManager = userManager;
      teacherTypeId = _config.GetValue<int> ("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int> ("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int> ("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int> ("AppSettings:studentTypeId");
      newRegToBePaidEmailId = _config.GetValue<int> ("AppSettings:newRegToBePaidEmailId");
      tuitionId = _config.GetValue<int> ("AppSettings:tuitionId");
      nextYearTuitionId = _config.GetValue<int> ("AppSettings:nextYearTuitionId");
      password = _config.GetValue<String> ("AppSettings:defaultPassword");
    }

    [HttpPost ("PwdCode")]
    public async Task<IActionResult> SendPwdValidationCode (ConfirmTokenDto confirmTokenDto) {
      bool codeSent = false;
      var userId = Convert.ToInt32 (confirmTokenDto.UserId);
      var phoneNumber = confirmTokenDto.PhoneNumber;

      var user = await _context.Users.FirstAsync (u => u.Id == userId);
      var token = await _userManager.GenerateUserTokenAsync (user, "ChangeDataTotpTokenProvider", "ChangeDataTotpTokenProvider");

      Sms sms = new Sms ();
      sms.To = phoneNumber;
      sms.ToUserId = user.Id;
      sms.SmsTypeId = _config.GetValue<int> ("AppSettings:SmsValidationTypeId");
      var smsTemplateId = _config.GetValue<int> ("AppSettings:PwdEditCodeSms");
      var smsTemplate = await _context.SmsTemplates.FirstAsync (t => t.Id == smsTemplateId);
      sms.Content = string.Format (smsTemplate.Content, token);
      sms.InsertUserId = _config.GetValue<int> ("AppSettings:Admin");
      sms.InsertDate = DateTime.Now;
      sms.UpdateDate = DateTime.Now;
      _repo.Add (sms);

      if (await _repo.SaveAll ()) {
        codeSent = true;
        return Ok (codeSent);
      }

      return BadRequest ("problème lors de l'envoi du sms de validation");
    }

    [HttpPost ("EmailCode")]
    public async Task<IActionResult> SendEmailValidationCode(ConfirmTokenDto confirmTokenDto)
    {
      var email = confirmTokenDto.Email;
      var userId = Convert.ToInt32(confirmTokenDto.UserId);
      bool codeSent = false;
      var user = await _context.Users.FirstAsync(u => u.Id == userId);
      var token = await _userManager.GenerateUserTokenAsync(user, "ChangeDataTotpTokenProvider", "ChangeDataTotpTokenProvider");

      var emailTemplateId = _config.GetValue<int>("AppSettings:emailEditCodeEmailId");
      var emailTemplate = await _context.EmailTemplates.FirstAsync(t => t.Id == emailTemplateId);
      Email newEmail = new Email();
      newEmail.EmailTypeId = 1;
      newEmail.ToAddress = email;
      newEmail.FromAddress = "no-reply@educnotes.com";
      newEmail.Subject = emailTemplate.Subject;
      newEmail.Body = string.Format (emailTemplate.Body, token);
      newEmail.InsertUserId = 1;
      newEmail.InsertDate = DateTime.Now;
      newEmail.UpdateUserId = 1;
      newEmail.UpdateDate = DateTime.Now;
      newEmail.ToUserId = userId;
      _repo.Add(newEmail);

      if(await _repo.SaveAll())
      {
        codeSent = true;
        return Ok(codeSent);
      }

      return BadRequest ("problème lors de l'envoi du sms de validation");
    }

    [AllowAnonymous]
    [HttpGet("PhoneCode/{userId}/{num}")]
    public async Task<IActionResult> GetPhoneValidationCode(int userId, string num)
    {
      // List<User> users = await _cache.GetUsers();
      // List<SmsTemplate> smsTemplates = await _cache.GetSmsTemplates();

      bool codeSent = false;
      var user = await _context.Users.FirstAsync(u => u.Id == userId);
      user.PhoneNumber = num;
      var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, num);
      Sms sms = new Sms();
      sms.To = num;
      sms.ToUserId = user.Id;
      sms.SmsTypeId = _config.GetValue<int>("AppSettings:SmsValidationTypeId");
      var smsTemplateId = _config.GetValue<int>("AppSettings:PhoneEditCodeSms");
      var smsTemplate = await _context.SmsTemplates.FirstAsync(t => t.Id == smsTemplateId);
      sms.Content = string.Format(smsTemplate.Content, token);
      sms.InsertUserId = _config.GetValue<int>("AppSettings:Admin");
      sms.InsertDate = DateTime.Now;
      sms.UpdateDate = DateTime.Now;
      _repo.Add(sms);

      if(await _repo.SaveAll())
      {
        codeSent = true;
        return Ok(codeSent);
      }

      return BadRequest ("problème lors de l'envoi du sms de validation");
    }

    [HttpPost("EditPhoneNumber")]
    public async Task<IActionResult> EditPhoneNumber(ConfirmTokenDto confirmTokenDto)
    {
      int userId = Convert.ToInt32(confirmTokenDto.UserId);
      string token = confirmTokenDto.Token;
      string phoneNumber = confirmTokenDto.PhoneNumber;

      bool phoneNumOk = false;
      var user = await _context.Users.FirstAsync(u => u.Id == userId);
      user.PhoneNumber = phoneNumber;
      var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, token);
      if(result.Succeeded)
      {
        await _userManager.UpdateAsync(user);
        phoneNumOk = true;
      }

      // await _cache.LoadUsers();
      return Ok(phoneNumOk);
    }

    [HttpPost("EditEmail")]
    public async Task<IActionResult> EditEmail(ConfirmTokenDto confirmTokenDto)
    {
      int userId = Convert.ToInt32(confirmTokenDto.UserId);
      string token = confirmTokenDto.Token;
      string email = confirmTokenDto.Email;

      bool emailOk = false;
      var user = await _context.Users.FirstAsync(u => u.Id == userId);
      user.Email = email.ToLower ();
      user.NormalizedEmail = email.ToUpper ();
      var result = await _userManager.VerifyUserTokenAsync(user, "ChangeDataTotpTokenProvider", "ChangeDataTotpTokenProvider", token);
      if (result)
      {
        await _userManager.UpdateAsync(user);
        emailOk = true;
      }

      // await _cache.LoadUsers();
      return Ok(emailOk);
    }

    [HttpPost("EditPwd")]
    public async Task<IActionResult> EditPwd(ConfirmTokenDto confirmTokenDto)
    {
      int userId = Convert.ToInt32(confirmTokenDto.UserId);
      string userName = confirmTokenDto.UserName;
      string token = confirmTokenDto.Token;
      string oldPwd = confirmTokenDto.OldPassword;
      string pwd = confirmTokenDto.Password;

      bool pwdOk = false;
      var user = await _context.Users.FirstAsync(u => u.Id == userId);
      var result = await _userManager.VerifyUserTokenAsync(user, "ChangeDataTotpTokenProvider", "ChangeDataTotpTokenProvider", token);
      if (result)
      {
        await _userManager.ChangePasswordAsync(user, oldPwd, pwd);
        user.UserName = userName.ToLower();
        user.NormalizedUserName = userName.ToUpper();
        await _userManager.UpdateAsync(user);
        pwdOk = true;
      }

      // await _cache.LoadUsers();
      return Ok(pwdOk);
    }

    [AllowAnonymous]
    [HttpPost("ConfirmPhoneNumber")]
    public async Task<IActionResult> ConfirmPhoneNumber(ConfirmTokenDto confirmTokenDto)
    {
      int userId = Convert.ToInt32(confirmTokenDto.UserId);
      string token = confirmTokenDto.Token;

      bool phoneNumValidated = false;
      var user = await _context.Users.FirstAsync(u => u.Id == userId);
      var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, token);
      if(result.Succeeded)
      {
        phoneNumValidated = true;
        _repo.Update(user);
      }

      if(phoneNumValidated && await _repo.SaveAll())
      {
        // await _cache.LoadUsers();
      }

      return Ok(new {
        phoneOK = phoneNumValidated,
        user
      });
    }

    [AllowAnonymous]
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(ConfirmTokenDto userData)
    {
      string userId = userData.UserId;
      string token = userData.Token;
      string userName = userData.UserName;
      string pwd = userData.Password;
      Boolean success = false;

      var user = await _userManager.FindByIdAsync(userId);
      var result = await _userManager.ResetPasswordAsync(user, token, pwd);
      if (result.Succeeded)
      {
        await _userManager.SetLockoutEndDateAsync(user, null);
        user.UserName = userName;
        user.NormalizedUserName = userName.ToUpper();
        _repo.Update(user);
        if(!await _repo.SaveAll())
          return BadRequest("'problème lors de la reinitialisation du compte");
        success = true;
      }

      return Ok(new {
        success,
        userName = user.LastName + " " + user.FirstName,
        gender = user.Gender
      });
    }

    [AllowAnonymous]
    [HttpPost ("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(ConfirmTokenDto userData)
    {
      string userId = userData.UserId;
      string token = userData.Token;
      Boolean success = false;
      Boolean validated = false;

      var user = await _userManager.FindByIdAsync(userId);
      List<UserForDetailedDto> children = new List<UserForDetailedDto>();
      bool childrenValidated = true;
      if(user != null)
      {
        if(user.EmailConfirmed)
        {
          success = true;
          validated = user.AccountDataValidated;
        }
        else
        {
          var result = await _userManager.ConfirmEmailAsync(user, token);
          if (result.Succeeded)
          {
            success = true;
          }
        }

        //get children if it's a parent...
        if(user.UserTypeId == parentTypeId)
        {
          children = await _repo.GetAccountChildren(user.Id);
          foreach(var child in children)
          {
            if(!child.AccountDataValidated)
            {
              childrenValidated = false;
              break;
            }
          }
        }
      }

      // await _cache.LoadUsers();
      return Ok(new {
        emailOK = success,
        accountValidated = validated,
        user,
        children,
        childrenValidated
      });
    }

  }
}