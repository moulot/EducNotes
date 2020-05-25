using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducNotes.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OrdersController : ControllerBase
  {
    private readonly IEducNotesRepository _repo;
    private readonly IMapper _mapper;
    private readonly DataContext _context;

    public OrdersController(IEducNotesRepository repo, IMapper mapper, DataContext context)
    {
      _repo = repo;
      _mapper = mapper;
      _context = context;
    }

    [HttpGet("{id}", Name = "GetOrder")]
    public async Task<IActionResult> GetOrder(int id)
    {
      var orderFromRepo = await _repo.GetOrder(id);
      var order = _mapper.Map<OrderDto>(orderFromRepo);
      return Ok(order);
    }

    [HttpGet("tuitionData")]
    public async Task<IActionResult> GetTuitionData()
    {
      var today = DateTime.Now.Date;

      //outstanding balance
      var balanceFromDB = await _context.OrderHistories.SumAsync(b => b.Delta);
      string balance = balanceFromDB.ToString("N0");

      //n day late outstanding balance
      var from7dayToToday = today.AddDays(-7);
      var from15dayToToday = today.AddDays(-15);
      var from30dayToToday = today.AddDays(-30);
      var b7day = await _context.OrderHistories.Include(i => i.Order)
                        .Where(b => b.Order.Validity <= today && b.Order.Validity >= from7dayToToday).ToListAsync();
      var balance7day = b7day.Sum(b => b.Delta);
      var b15day = await _context.OrderHistories.Include(i => i.Order)
                        .Where(b => b.Order.Validity <= today && b.Order.Validity >= from15dayToToday).ToListAsync();
      var balance15day = b15day.Sum(b => b.Delta);

      return Ok(new {
        openBalance = balance,
        balanceFromDB
      });
    }
  }
}