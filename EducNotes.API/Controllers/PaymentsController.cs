using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
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
    int chequeTypeId, cashTypeId, bankTransferTypeId, mobileMoneyTypeId;

    public PaymentsController (DataContext context, IEducNotesRepository repo, IMapper mapper, IConfiguration config) {
      _context = context;
      _repo = repo;
      _config = config;
      _mapper = mapper;
      chequeTypeId = _config.GetValue<int>("AppSettings:chequeTypeId");
      cashTypeId = _config.GetValue<int>("AppSettings:cashTypeId");
      bankTransferTypeId = _config.GetValue<int>("AppSettings:bankTransferTypeId");
      mobileMoneyTypeId = _config.GetValue<int>("AppSettings:mobileMoneyTypeId");
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

      if(finOp.PaymentTypeId == cashTypeId || finOp.PaymentTypeId == mobileMoneyTypeId)
      {
        finOp.Cashed = true;
      }
      if(finOp.PaymentTypeId == bankTransferTypeId || finOp.PaymentTypeId == chequeTypeId)
      {
        finOp.Received = true;
      }

      //order validation
      if(finOpDataDto.OrderId != 0)
      {
        finOp.OrderId = finOpDataDto.OrderId;
        await _repo.ValidateTuition(finOp.Amount, Convert.ToInt32(finOp.OrderId));
      }

      _context.Add(finOp);
      _context.SaveChanges();

      if(finOpDataDto.Payments.Count() > 0)
      {
        foreach (var payment in finOpDataDto.Payments)
        {
          FinOpOrderLine fool = new FinOpOrderLine();
          fool.FinOpId = finOp.Id;
          fool.OrderLineId = payment.OrderLineId;
          var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.OrderLineId == payment.OrderLineId);
          if(invoice != null)
            fool.InvoiceId = invoice.Id;
          fool.Amount = payment.Amount;
          _repo.Add(fool);

          var oldAmount = _context.OrderLineHistories
                            .OrderByDescending(o => o.OpDate)
                            .FirstOrDefault(h => h.OrderLineId == payment.OrderLineId).NewAmount;

          //add data for orderline history
          OrderLineHistory orderlineHistory = new OrderLineHistory();
          orderlineHistory.OrderLineId = payment.OrderLineId;
          orderlineHistory.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
          orderlineHistory.OpDate = finOp.FinOpDate;
          orderlineHistory.Action = "UPD";
          orderlineHistory.OldAmount = oldAmount;
          orderlineHistory.NewAmount = oldAmount - payment.Amount;
          orderlineHistory.Delta = orderlineHistory.NewAmount - orderlineHistory.OldAmount;
          _repo.Add(orderlineHistory);
        }
      }

      if(await _repo.SaveAll())
      {
        return Ok();
      }

      return BadRequest("problème pour saisir le paiement.");
    }

  }
}