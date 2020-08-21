using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EducNotes.API.Controllers
{
  [Route ("api/[controller]")]
  [ApiController]
  public class PaymentsController : ControllerBase {
    private readonly DataContext _context;
    private readonly IEducNotesRepository _repo;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public PaymentsController (DataContext context, IEducNotesRepository repo, IMapper mapper, IConfiguration config) {
      _context = context;
      _repo = repo;
      _config = config;
      _mapper = mapper;
    }

    [HttpGet("GetPaymentTypes")]
    public async Task<IActionResult> GetPaymentTypes()
    {
      var types = await _context.PaymentTypes.OrderBy(o => o.Name).ToListAsync();
      return Ok(types);
    }

    [HttpGet("PaymentData")]
    public async Task<IActionResult> GetPaymentData()
    {
      var payTypes = await _repo.GetPaymentTypes();
      var paymentTypes = payTypes.OrderBy(o => o.DsplSeq);
      var banks = await _repo.GetBanks();

      return Ok(new {
        paymentTypes,
        banks
      });
    }
  }
}