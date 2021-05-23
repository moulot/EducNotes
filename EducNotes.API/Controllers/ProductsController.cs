using System.Globalization;
using AutoMapper;
using EducNotes.API.data;
using EducNotes.API.Data;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EducNotes.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ProductsController : ControllerBase
  {
    private readonly DataContext _context;
    private readonly IEducNotesRepository _repo;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    CultureInfo frC = new CultureInfo("fr-FR");
    public ICacheRepository _cache { get; }
    int tuitionId;

    public ProductsController(DataContext context, UserManager<User> userManager, IConfiguration config,
      IEducNotesRepository repo, IMapper mapper, ICacheRepository cache)
    {
      _cache = cache;
      _userManager = userManager;
      _config = config;
      _context = context;
      _repo = repo;
      _mapper = mapper;
      tuitionId = _config.GetValue<int>("AppSettings:tuitionId");
    }

    

  }
}