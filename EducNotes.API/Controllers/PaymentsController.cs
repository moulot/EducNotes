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
    int chequeTypeId;

    public PaymentsController (DataContext context, IEducNotesRepository repo, IMapper mapper, IConfiguration config) {
      _context = context;
      _repo = repo;
      _config = config;
      _mapper = mapper;
      chequeTypeId = _config.GetValue<int>("AppSettings:chequeTypeId");
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

    [HttpPost("AddFinOp")]
    public async Task<IActionResult> AddFinOp(FinOpDataDto finOpDataDto)
    {
      FinOp finOp = new FinOp();
      finOp.FinOpDate = finOpDataDto.FinOpDate;
      if(finOpDataDto.InvoiceId != 0)
      {
        finOp.InvoiceId = finOpDataDto.InvoiceId;
      }
      finOp.PaymentTypeId = finOpDataDto.PaymentTypeId;
      finOp.Amount = finOpDataDto.Amount;
      finOp.Note = finOpDataDto.Note;
      finOp.DocRef = finOpDataDto.RefDoc;

      if(finOp.PaymentTypeId == chequeTypeId)
      {
        Cheque cheque = new Cheque();
        cheque.ChequeNum = finOpDataDto.numCheque;
        cheque.Amount = finOp.Amount;
        if(finOpDataDto.BankId != 0)
          cheque.BankId = finOpDataDto.BankId;

        _context.Add(cheque);
        _context.SaveChanges();
        finOp.ChequeId = cheque.Id;
      }

      //order validation
      if(finOpDataDto.OrderId != 0)
      {
        finOp.OrderId = finOpDataDto.OrderId;
        await _repo.ValidateTuition(finOp.Amount, Convert.ToInt32(finOp.OrderId));
      }

      _context.Add(finOp);

      if(await _repo.SaveAll())
      {
        return Ok();
      }

      return BadRequest("problème pour saisir le paiement.");
    }

  }
}