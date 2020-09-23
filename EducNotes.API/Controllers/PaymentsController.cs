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

    [HttpPost("AddTuitionPayment")]
    public async Task<IActionResult> AddTuitionPayment(FinOpDataDto finOpDataDto)
    {
      using (var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          FinOp finOp = new FinOp();
          finOp.FinOpDate = finOpDataDto.FinOpDate;
          finOp.OrderId = finOpDataDto.OrderId;
          finOp.FinOpTypeId = finOpDataDto.FinOpTypeId;
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

          if(finOp.PaymentTypeId == bankTransferTypeId)
          {
            if(finOpDataDto.BankId != 0)
            {
              finOp.FromBankId = finOpDataDto.BankId;
            }
          }

          _context.Add(finOp);
          _context.SaveChanges();

          Boolean allLinesValidated = true;
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
              _context.SaveChanges();

              // registration validation
              var line = await _context.OrderLines.FirstAsync(o => o.Id == payment.OrderLineId);
              if(!line.Validated && finOp.Cashed)
              {
                var regFee = line.ProductFee;
                var firstdeadline = await _context.OrderLineDeadlines
                                          .Where(d => d.OrderLineId == line.Id)
                                          .OrderBy(o => o.DueDate)
                                          .FirstAsync();
                var amount = firstdeadline.Amount;
                var downpayment = regFee + amount;
                var allPaid = await _context.FinOpOrderLines
                                    .Where(f => f.FinOp.Cashed && f.OrderLineId == line.Id).SumAsync(s => s.Amount);
                if(allPaid >= downpayment)
                {
                  line.Validated = true;
                  _repo.Update(line);

                  // validate user tuition
                  var child = await _context.Users.FirstAsync(u => u.Id == line.ChildId);
                  child.Validated = true;
                  _repo.Update(child);
                }
                else
                {
                  allLinesValidated = false;
                }
              }

              var oldAmount = _context.OrderLineHistories
                                .OrderByDescending(o => o.OpDate)
                                .FirstOrDefault(h => h.OrderLineId == payment.OrderLineId).NewAmount;

              //add data for orderline history
              OrderLineHistory orderlineHistory = new OrderLineHistory();
              orderlineHistory.OrderLineId = payment.OrderLineId;
              orderlineHistory.FinOpId = finOp.Id;
              orderlineHistory.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
              orderlineHistory.OpDate = finOp.FinOpDate;
              orderlineHistory.Action = "UPD";
              orderlineHistory.OldAmount = oldAmount;
              orderlineHistory.NewAmount = oldAmount - payment.Amount;
              orderlineHistory.Delta = orderlineHistory.NewAmount - orderlineHistory.OldAmount;
              if(finOp.Cashed == true)
                orderlineHistory.Cashed = true;
              _repo.Add(orderlineHistory);
            }
          }

          if(allLinesValidated)
          {
            var order = await _context.Orders.FirstAsync(o => o.Id == finOpDataDto.OrderId);
            order.Validated = true;
            _repo.Update(order);
          }

          if(await _repo.SaveAll())
          {
            // await _repo.ValidateTuition(finOp.Amount, Convert.ToInt32(finOp.OrderId));
            identityContextTransaction.Commit();
            return Ok();
          }
        }
        catch
        {
          identityContextTransaction.Rollback();
          return BadRequest("erreur lors de l'ajout du paiement.");
        }

        return BadRequest("problème pour saisir le paiement.");
      }
    }

    [HttpGet("PaymentsToValidate")]
    public async Task<IActionResult> GetPaymentsToBeValidated()
    {
      var paymentsFromDB = await _context.FinOps
                                  .Where(f => f.Rejected == false && f.Cashed == false)
                                  .Include(i => i.Cheque).ThenInclude(i => i.Bank)
                                  .Include(i => i.PaymentType)
                                  .Include(i => i.FromBank)
                                  .OrderBy(o => o.FinOpDate)
                                  .ToListAsync();
      var payments = _mapper.Map<List<FinOpDto>>(paymentsFromDB);
      for(int i = 0; i < payments.Count(); i++)
      {
        var pay = payments[i];
        var linesFromDB = await _context.FinOpOrderLines
                                .Include(d => d.Invoice)
                                .Include(o => o.OrderLine).ThenInclude(p => p.Product)
                                .Include(o => o.OrderLine).ThenInclude( c => c.Child)
                                .Where(f => f.FinOpId == pay.Id).ToListAsync();
        var lines = _mapper.Map<List<PaymentDto>>(linesFromDB);
        payments[i].LinePayments = new List<PaymentDto>();
        foreach (var line in lines)
        {
          payments[i].LinePayments.Add(line);
        }
      }

      return Ok(payments);
    }

    [HttpPost("UpdatePayments")]
    public async Task<IActionResult> UpdatePaymentsStatus(List<FinOpForUpdateStatusDto> finOps)
    {
      foreach (var fo in finOps)
      {
        var finOp = await _context.FinOps.FirstAsync(f => f.Id == fo.Id);
        finOp.Received = fo.Received;
        finOp.DepositedToBank = fo.DepositedToBank;
        finOp.Rejected = fo.Rejected;
        finOp.Cashed = fo.Cashed;
        _repo.Update(finOp);

        if(finOp.Cashed == true)
        {
          var lineHistories = await _context.OrderLineHistories
                                    .Include(i => i.OrderLine)
                                    .Where(f => f.FinOpId == finOp.Id).ToListAsync();
          foreach (var line in lineHistories)
          {
            // validate user tuition
            var child = await _context.Users
                              .FirstAsync(u => u.Id == line.OrderLine.ChildId);
            child.Validated = true;
            _repo.Update(child);

            var lineHistory = await _context.OrderLineHistories.FirstOrDefaultAsync(o => o.Id == line.Id);
            if(lineHistory != null)
            {
              lineHistory.Cashed = finOp.Cashed;
              lineHistory.Rejected = finOp.Rejected;
              _context.Update(lineHistory);
            }
          }
        }
      }

      if(await _repo.SaveAll())
      {
        return NoContent();
      }

      return BadRequest("problème pour mettre à jour les paiements");
    }

  }
}