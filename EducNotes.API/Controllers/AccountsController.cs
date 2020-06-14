using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    [HttpPost("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(confirmEmailDto confirmEmailDto)
    {
      string userId = confirmEmailDto.UserId;
      string token = confirmEmailDto.Token;

      var user = await _userManager.FindByIdAsync(userId);
      var result = await _userManager.ConfirmEmailAsync(user, token);

      Boolean success = false;
      if(result.Succeeded)
      {
        success = true;
      }

      return Ok(new {
        success,
        user
      });
    }

  }
}