using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.data;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EducNotes.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PaymentsController : ControllerBase
  {
    private readonly DataContext _context;
    private readonly IEducNotesRepository _repo;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    int chequeTypeId, cashTypeId, bankTransferTypeId, mobileMoneyTypeId;
    int tuitionTypeId, finOpTypeInvoice, finOpTypePayment;
    public readonly ICacheRepository _cache;

    public PaymentsController(DataContext context, IEducNotesRepository repo, IMapper mapper, IConfiguration config,
      ICacheRepository cache)
    {
      _cache = cache;
      _context = context;
      _repo = repo;
      _config = config;
      _mapper = mapper;
      chequeTypeId = _config.GetValue<int>("AppSettings:chequeTypeId");
      cashTypeId = _config.GetValue<int>("AppSettings:cashTypeId");
      bankTransferTypeId = _config.GetValue<int>("AppSettings:bankTransferTypeId");
      mobileMoneyTypeId = _config.GetValue<int>("AppSettings:mobileMoneyTypeId");
      finOpTypeInvoice = _config.GetValue<int>("AppSettings:finOpTypeInvoice");
      finOpTypePayment = _config.GetValue<int>("AppSettings:finOpTypePayment");
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
          // List<User> students = await _cache.GetStudents();
          // List<OrderLine> lines = await _cache.GetOrderLines();
          // List<OrderLineDeadline> lineDeadlines = await _cache.GetOrderLineDeadLines();
          // List<FinOpOrderLine> finOpOrderLines = await _cache.GetFinOpOrderLines();
          // List<Order> orders = await _cache.GetOrders();

          FinOp finOp = new FinOp();
          finOp.FinOpDate = finOpDataDto.FinOpDate;
          finOp.OrderId = finOpDataDto.OrderId;
          finOp.FinOpTypeId = finOpDataDto.FinOpTypeId;
          if (finOpDataDto.InvoiceId != 0)
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
            if (finOpDataDto.BankId != 0)
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
            foreach(var payment in finOpDataDto.Payments)
            {
              FinOpOrderLine finOpLine = new FinOpOrderLine();
              finOpLine.FinOpId = finOp.Id;
              finOpLine.OrderLineId = payment.OrderLineId;
              var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.OrderLineId == payment.OrderLineId);
              if (invoice != null)
                finOpLine.InvoiceId = invoice.Id;
              finOpLine.Amount = payment.Amount;
              _repo.Add(finOpLine);
              _context.SaveChanges();

              var orderPaid = true;
              if (finOp.Cashed)
              {
                var line = await _context.OrderLines.FirstAsync(o => o.Id == payment.OrderLineId);
                var deadlines = await _context.OrderLineDeadlines.Where(d => d.OrderLineId == line.Id)
                                             .OrderBy(o => o.DueDate)
                                             .ToListAsync();

                var allPaid = await _context.FinOpOrderLines
                              .Where(f => f.FinOp.Cashed && f.FinOp.FinOpTypeId == finOpTypePayment && f.OrderLineId == line.Id)
                              .SumAsync(s => s.Amount);
                decimal paidBalance = allPaid;
                if (!line.Validated)
                {
                  // tuition validation
                  var regFee = line.ProductFee;
                  var firstdeadline = deadlines.First();
                  var amount = firstdeadline.Amount;
                  var downpayment = regFee + amount;
                  if (allPaid >= downpayment)
                  {
                    line.Validated = true;
                    _repo.Update(line);

                    firstdeadline.Paid = true;
                    _repo.Update(firstdeadline);

                    // validate user tuition
                    var child = await _context.Users.FirstAsync(u => u.Id == line.ChildId);
                    child.Validated = true;
                    _repo.Update(child);

                    paidBalance -= downpayment;

                    // check if the current payment covers the other due amounts
                    for (int i = 1; i < deadlines.Count(); i++)
                    {
                      var lineD = deadlines[i];
                      var dueTotal = lineD.Amount + lineD.ProductFee;
                      if(paidBalance >= dueTotal)
                      {
                        lineD.Paid = true;
                        _repo.Update(lineD);
                        paidBalance -= dueTotal;
                      }
                      else
                      {
                        orderPaid = false;
                        allLinesValidated = false;
                        break;
                      }
                    }
                  }
                  else
                  {
                    allLinesValidated = false;
                  }
                }

                var lineDueAmount = deadlines.Sum(s => s.Amount + s.ProductFee);
                if (allPaid >= lineDueAmount)
                  line.Paid = true;
                _repo.Update(line);

                // check if the current payment covers the other due amounts
                for (int i = 0; i < deadlines.Count(); i++)
                {
                  var lineD = deadlines[i];
                  var odeadline = lineD.Amount + lineD.ProductFee;
                  if (paidBalance >= odeadline)
                  {
                    lineD.Paid = true;
                    _repo.Update(lineD);
                    paidBalance -= odeadline;
                  }
                  else
                  {
                    orderPaid = false;
                    allLinesValidated = false;
                    break;
                  }
                }
              }
              else
              {
                allLinesValidated = false;
                orderPaid = false;
              }

              if (allLinesValidated)
              {
                var order = await _context.Orders.FirstAsync(o => o.Id == finOpDataDto.OrderId);
                order.Validated = true;
                if (orderPaid)
                {
                  order.Paid = true;
                  order.Completed = true;
                }
                _repo.Update(order);
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
              if (finOp.Cashed == true)
                orderlineHistory.Cashed = true;
              _repo.Add(orderlineHistory);
            }
          }

          if (await _repo.SaveAll())
          {
            // await _cache.LoadOrders();
            // await _cache.LoadFinOps();
            // await _cache.LoadFinOpOrderLines();
            // await _cache.LoadCheques();
            // await _cache.LoadOrderLines();
            // await _cache.LoadOrderLineDeadLines();
            // await _cache.LoadUsers();
            identityContextTransaction.Commit();
            return Ok();
          }
        }
        catch(Exception ex)
        {
          string ddd = ex.Message;
          identityContextTransaction.Rollback();
          return BadRequest("erreur lors de l'ajout du paiement.");
        }

        return BadRequest("problème pour saisir le paiement.");
      }
    }

    [HttpGet("PaymentsToValidate")]
    public async Task<IActionResult> GetPaymentsToBeValidated()
    {
      // List<FinOp> finOps = await _cache.GetFinOps();
      // List<FinOpOrderLine> finOpLines = await _cache.GetFinOpOrderLines();

      var paymentsFromDB = await _context.FinOps
                            .Where(f => f.Rejected == false && f.Cashed == false)
                            .OrderBy(o => o.FinOpDate)
                            .ToListAsync();
      var payments = _mapper.Map<List<FinOpDto>>(paymentsFromDB);
      for (int i = 0; i < payments.Count(); i++)
      {
        var pay = payments[i];
        var linesFromDB = await _context.FinOpOrderLines.Where(f => f.FinOpId == pay.Id).ToListAsync();
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
      // List<FinOp> finOpsCached = await _cache.GetFinOps();
      // List<FinOpOrderLine> finOpLines = await _cache.GetFinOpOrderLines();
      // List<OrderLineDeadline> lineDeadLines = await _cache.GetOrderLineDeadLines();
      // List<OrderLine> lines = await _cache.GetOrderLines();
      // List<User> students = await _cache.GetStudents();
      // List<Order> orders = await _cache.GetOrders();

      foreach (var fo in finOps)
      {
        var finOp = await _context.FinOps.FirstAsync(f => f.Id == fo.Id);
        finOp.Received = fo.Received;
        finOp.DepositedToBank = fo.DepositedToBank;
        finOp.Rejected = fo.Rejected;
        finOp.Cashed = fo.Cashed;
        _repo.Update(finOp);

        if (finOp.Cashed == true)
        {
          _context.SaveChanges();
          var finOplines = await _context.FinOpOrderLines.Where(f => f.FinOpId == finOp.Id).ToListAsync();
          foreach (var finOpline in finOplines)
          {
            var allLinesValidated = true;
            Boolean orderPaid = true;
            var deadlines = await _context.OrderLineDeadlines
                              .Where(d => d.OrderLineId == finOpline.OrderLineId)
                              .OrderBy(o => o.DueDate)
                              .ToListAsync();
            var line = await _context.OrderLines.FirstAsync(o => o.Id == finOpline.OrderLineId);
            var allPaid = await _context.FinOpOrderLines
                                .Where(f => f.FinOp.Cashed && f.FinOp.FinOpTypeId == finOpTypePayment && f.OrderLineId == line.Id)
                                .SumAsync(s => s.Amount);
            decimal paidBalance = allPaid;
            // registration validation
            if (!line.Validated)
            {
              var firstdeadline = deadlines.First();
              var downpayment = firstdeadline.Amount + firstdeadline.ProductFee;
              if (paidBalance >= downpayment)
              {
                line.Validated = true;
                _repo.Update(line);

                firstdeadline.Paid = true;
                _repo.Update(firstdeadline);

                // validate user tuition
                var child = await _context.Users.FirstAsync(u => u.Id == line.ChildId);
                child.Validated = true;
                _repo.Update(child);

                paidBalance -= downpayment;

                // check if the paid amount covers the other due amounts
                for (int i = 1; i < deadlines.Count(); i++)
                {
                  var lineD = deadlines[i];
                  var dueTotal = lineD.Amount + lineD.ProductFee;
                  if (paidBalance >= dueTotal)
                  {
                    lineD.Paid = true;
                    _repo.Update(lineD);
                    paidBalance -= dueTotal;
                  }
                  else
                  {
                    orderPaid = false;
                    break;
                  }
                }
              }
              else
              {
                allLinesValidated = false;
                orderPaid = false;
              }
            }

            var lineDueAmount = deadlines.Sum(s => s.Amount + s.ProductFee);
            if (allPaid >= lineDueAmount)
              line.Paid = true;
            _repo.Update(line);

            // check if the paid amount covers the other due amounts
            for (int i = 0; i < deadlines.Count(); i++)
            {
              var lineD = deadlines[i];
              var dueline = lineD.Amount + lineD.ProductFee;
              if (paidBalance >= dueline)
              {
                lineD.Paid = true;
                _repo.Update(lineD);
                paidBalance -= dueline;
              }
              else
              {
                orderPaid = false;
                allLinesValidated = false;
                break;
              }
            }

            if (allLinesValidated)
            {
              var order = await _context.Orders.FirstAsync(o => o.Id == line.OrderId);
              order.Validated = true;
              if (orderPaid)
              {
                order.Paid = true;
                order.Completed = true;
              }
              _repo.Update(order);
            }

            var lineHistory = await _context.OrderLineHistories
                                      .Include(i => i.OrderLine)
                                      .Where(f => f.FinOpId == finOp.Id && f.OrderLineId == line.Id)
                                      .FirstAsync();
            if (lineHistory != null)
            {
              lineHistory.Cashed = finOp.Cashed;
              lineHistory.Rejected = finOp.Rejected;
              _context.Update(lineHistory);
            }
          }
        }
      }

      if (await _repo.SaveAll())
      {
        // await _cache.LoadFinOps();
        // await _cache.LoadOrderLines();
        // await _cache.LoadOrderLineDeadLines();
        // await _cache.LoadUsers();
        // await _cache.LoadOrders();
        return NoContent();
      }

      return BadRequest("problème pour mettre à jour les paiements");
    }

  }
}