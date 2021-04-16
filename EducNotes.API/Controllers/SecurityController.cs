using AutoMapper;
using EducNotes.API.data;
using EducNotes.API.Data;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EducNotes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  public class SecurityController : ControllerBase
  {
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IEducNotesRepository _repo;
    private DataContext _context;
    int parentRoleId, memberRoleId, moderatorRoleId, adminRoleId, teacherRoleId;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    public ICacheRepository _cache { get; }

    public SecurityController(IConfiguration config, IMapper mapper, IEducNotesRepository repo,
      UserManager<User> userManager, DataContext context, ICacheRepository cache)
    {
      _cache = cache;
      _context = context;
      _repo = repo;
      _config = config;
      _mapper = mapper;
      _userManager = userManager;
      teacherTypeId = _config.GetValue<int>("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int>("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int>("AppSettings:studentTypeId");
      parentRoleId = _config.GetValue<int>("AppSettings:parentRoleId");
      memberRoleId = _config.GetValue<int>("AppSettings:memberRoleId");
      moderatorRoleId = _config.GetValue<int>("AppSettings:moderatorRoleId");
      adminRoleId = _config.GetValue<int>("AppSettings:adminRoleId");
      teacherRoleId = _config.GetValue<int>("AppSettings:teacherRoleId");
    }
  }

  
}