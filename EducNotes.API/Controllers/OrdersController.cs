using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EducNotes.API.Helpers;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace EducNotes.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class OrdersController : ControllerBase
  {
    private readonly IEducNotesRepository _repo;
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    public IConfiguration _config { get; }
    public UserManager<User> _userManager { get; }
    CultureInfo frC = new CultureInfo("fr-FR");
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    int tuitionTypeId, finOpTypeInvoice, finOpTypePayment;
    int parentRoleId, memberRoleId, moderatorRoleId, adminRoleId, teacherRoleId;
    string password;
    int tuitionId, nextYearTuitionId, newRegToBePaidEmailId;

    public OrdersController(IEducNotesRepository repo, IMapper mapper, DataContext context,
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
      tuitionTypeId = _config.GetValue<int>("AppSettings:tuitionTypeId");
      newRegToBePaidEmailId = _config.GetValue<int>("AppSettings:newRegToBePaidEmailId");
      tuitionId = _config.GetValue<int>("AppSettings:tuitionId");
      nextYearTuitionId = _config.GetValue<int>("AppSettings:nextYearTuitionId");
      password = _config.GetValue<String>("AppSettings:defaultPassword");
      parentRoleId = _config.GetValue<int>("AppSettings:parentRoleId");
      memberRoleId = _config.GetValue<int>("AppSettings:memberRoleId");
      moderatorRoleId = _config.GetValue<int>("AppSettings:moderatorRoleId");
      adminRoleId = _config.GetValue<int>("AppSettings:adminRoleId");
      teacherRoleId = _config.GetValue<int>("AppSettings:teacherRoleId");
      finOpTypeInvoice = _config.GetValue<int>("AppSettings:finOpTypeInvoice");
      finOpTypePayment = _config.GetValue<int>("AppSettings:finOpTypePayment");
    }

    [HttpGet("tuitionOrderData")]
    public async Task<IActionResult> GetTuitionOrderData()
    {
      // tuition product ids
      int tuitionId = _config.GetValue<int>("AppSettings:tuitionId");
      int nextYearTuitionId = _config.GetValue<int>("AppSettings:nextTuitionId");
      var settings = await _context.Settings.ToListAsync();
      int daysToValidate = Convert.ToInt32((settings.First(s => s.Name == "DaysToValidateReg")).Value);

      var deadlines = await _context.ProductDeadLines
                      .OrderBy(o => o.DueDate)
                      .Where(p => p.ProductId == tuitionId || p.ProductId == nextYearTuitionId).ToListAsync();

      var classProducts = await _context.ClassLevelProducts
                          .Where(c => c.ProductId == tuitionId || c.ProductId == nextYearTuitionId).ToListAsync();

      //order dates
      var todayDate = DateTime.Now;
      var today = todayDate.ToString("dd/MM/yyyy", frC);
      var date = todayDate.AddDays(daysToValidate);
      var tuitionValidity = date.ToString("dd/MM/yyyy", frC);

      return Ok(new {
        deadlines,
        classProducts,
        today,
        tuitionValidity
      });
    }

    [HttpGet("tuitionFromChild/{id}")]
    public async Task<IActionResult> GetTuitionFromChild(int id)
    {
      var parents = await _repo.GetParents(id);
      var fatherId = parents.FirstOrDefault(p => p.Gender == 1).Id;
      var motherId = parents.FirstOrDefault(p => p.Gender == 0).Id;
      var order = await _context.Orders
                          .Include(i => i.Child)
                          .Include(i => i.Father)
                          .Include(i => i.Mother)
                          .FirstOrDefaultAsync(t => t.isReg && (t.MotherId == motherId || t.FatherId == fatherId));
      var tuition = _mapper.Map<OrderDto>(order);

      var lines = await _repo.GetOrderLines(tuition.Id);
      tuition.Lines = _mapper.Map<List<OrderLineDto>>(lines);
      var payments = await _repo.GetChildPayments(id);
      tuition.NbPayRejected = payments.Where(p => p.FinOp.Rejected).Count();
      tuition.LinePayments = _mapper.Map<List<PaymentDto>>(payments);
      tuition.AmountPaid = tuition.LinePayments.Where(f => f.FinOpTypeId == finOpTypePayment && f.Cashed).Sum(a => a.Amount);
      tuition.strAmountPaid = tuition.AmountPaid.ToString("N0") + " F";

      var child = tuition.Lines.Where(c => c.ChildId == id).First();
      tuition.ChildLevelName = child.ClassLevelName;
      if(!tuition.Paid)
      {
        var nextDueAmount = await _repo.GetChildDueAmount(child.Id, tuition.AmountPaid);
        tuition.NextDueAmount = nextDueAmount.DueAmount;
        tuition.strNextDeadline = nextDueAmount.Deadline.ToString("dd/MM/yyyy", frC);
      }

      tuition.AmountInvoiced = tuition.Lines.Where(f => f.ChildId == id).Sum(a => a.AmountTTC);
      tuition.strAmountInvoiced = tuition.AmountInvoiced.ToString("N0") + " F";
      tuition.Balance = tuition.AmountInvoiced - tuition.AmountPaid;
      var baba = _context.OrderLineHistories.Sum(s => s.Delta);
      tuition.strBalance = tuition.Balance.ToString("N0") + " F";
      tuition.AmountToValidate = tuition.LinePayments
                                .Where(p => p.FinOpTypeId == finOpTypePayment && p.Cashed == false && p.Rejected == false)
                                .Sum(s => s.Amount);
      tuition.strAmountToValidate = tuition.AmountToValidate.ToString("N0") + " F";
      var lineDeadline = await _context.OrderLineDeadlines
                              .Where(l => l.OrderLine.ChildId == id)
                              .OrderBy(o => o.DueDate)
                              .FirstAsync();
      tuition.DownPayment = lineDeadline.Amount + lineDeadline.ProductFee;
      tuition.Validated = (await _context.Users.Where(u => u.Id == id).FirstAsync()).Validated;

      return Ok(tuition);
    }

    [HttpGet("{id}", Name = "GetOrder")]
    public async Task<IActionResult> GetOrder(int id)
    {
      var orderFromRepo = await _repo.GetOrder(id);
      var order = _mapper.Map<OrderDto>(orderFromRepo);

      var lines = await _repo.GetOrderLines(order.Id);
      order.Lines = _mapper.Map<List<OrderLineDto>>(lines);
      order.Payments = await _repo.GetOrderPayments(order.Id);

      order.AmountPaid = order.Payments.Where(f => f.Cashed).Sum(a => a.Amount);
      order.strAmountPaid = order.AmountPaid.ToString("N0") + " F";
      order.Balance = order.AmountTTC - order.AmountPaid;
      order.strBalance = order.Balance.ToString("N0") + " F";
      order.AmountToValidate = order.Payments.Where(p => p.Received == true || p.DepositedToBank == true).Sum(s => s.Amount);
      order.strAmountToValidate = order.AmountToValidate.ToString("N0") + " F";

      return Ok(order);
    }

    [HttpGet("balanceData")]
    public async Task<IActionResult> GetBalanceData()
    {
      var today = DateTime.Now.Date;

      // invoiced amount
      var invoiced = await _context.Orders.SumAsync(s => s.AmountTTC);
      // cashed amount
      var cashed = await _context.FinOps
                          .Where(o => o.Cashed == true && o.FinOpTypeId == finOpTypePayment)
                          .SumAsync(s => s.Amount);
      // in validation amount
      var toBeValidated = await _context.FinOps
                                .Where(o => o.Cashed == false && o.Rejected == false && o.FinOpTypeId == finOpTypePayment)
                                .SumAsync(s => s.Amount);
      // outstanding balance
      var balance = await _context.OrderLineHistories.Where(f => f.Cashed == true).SumAsync(b => b.Delta);

      decimal todayDueAmount = 0;
      decimal lateAmount7Days = 0;
      decimal lateAmount15Days = 0;
      decimal lateAmount30Days = 0;
      decimal lateAmount60Days = 0;
      decimal lateAmount60DaysPlus = 0;
      // get amount due today
      var lines = await _context.OrderLines.ToListAsync();
      foreach(var line in lines)
      {
        var lineDeadlines = await _context.OrderLineDeadlines
                                    .Where(o => o.OrderLineId == line.Id)
                                    .OrderBy(o => o.DueDate)
                                    .ToListAsync();
        var amountPaid = await _context.FinOpOrderLines
                                .Where(f => f.OrderLineId == line.Id && f.FinOp.Cashed)
                                .SumAsync(s => s.Amount);
        decimal lineDueAmount = 0;
        if(lineDeadlines.Count() > 0)
        {
          foreach(var lineD in lineDeadlines)
          {
            if(lineD.DueDate.Date < today && !lineD.Paid)
            {
              lineDueAmount = lineD.Amount + lineD.ProductFee - amountPaid;
              todayDueAmount += lineDueAmount;

              // split late amount in amounts of days late
              var nbDaysLate = Math.Abs((today - lineD.DueDate.Date).TotalDays);
              if(nbDaysLate <= 7)
                lateAmount7Days += lineDueAmount;
              else if(nbDaysLate > 7 && nbDaysLate <= 15)
                lateAmount15Days += lineDueAmount;
              else if(nbDaysLate > 15 && nbDaysLate <= 30)
                lateAmount30Days += lineDueAmount;
              else if(nbDaysLate > 30 && nbDaysLate <= 60)
                lateAmount60Days += lineDueAmount;
              else if(nbDaysLate > 60)
                lateAmount60DaysPlus += lineDueAmount;
            }
          }
        }
        else
        {
          if(line.Deadline.Date < today)
          {
            todayDueAmount += line.AmountTTC - amountPaid;

            // split late amount in amounts of days late
            var nbDaysLate = Math.Abs((today - line.Deadline.Date).TotalDays);
            if(nbDaysLate <= 7)
              lateAmount7Days += line.AmountTTC;
            else if(nbDaysLate > 7 && nbDaysLate <= 15)
              lateAmount15Days += line.AmountTTC;
            else if(nbDaysLate > 15 && nbDaysLate <= 30)
              lateAmount30Days += line.AmountTTC;
            else if(nbDaysLate > 30 && nbDaysLate <= 60)
              lateAmount60Days += line.AmountTTC;
            else if(nbDaysLate > 60)
              lateAmount60DaysPlus += line.AmountTTC;
          }
        }
      }

      return Ok(new {
        invoiced,
        toBeValidated,
        cashed,
        openBalance = balance,
        lateAmount = todayDueAmount,
        lateAmount7Days,
        lateAmount15Days,
        lateAmount30Days,
        lateAmount60Days,
        lateAmount60DaysPlus
      });
    }

    [HttpPost("NewTuition")]
    public async Task<IActionResult> AddNewTuition(TuitionDataDto newTuition)
    {
      List<RegistrationEmailDto> emails = new List<RegistrationEmailDto>();
      using (var identityContextTransaction = _context.Database.BeginTransaction())
      {
        try
        {
          List<Setting> settings = await _context.Settings.ToListAsync();
          var schoolName = settings.First(s => s.Name.ToLower() == "schoolname").Value;
          string RegDeadLine = settings.First(s => s.Name == "RegistrationDeadLine").Value;

          var deadlines = await _context.ProductDeadLines
                                .OrderBy(o => o.DueDate)
                                .Where(p => p.ProductId == tuitionId).ToListAsync();
          var firstDeadline = deadlines.First(d => d.Seq == 1);

          //tuition order
          Order order = new Order();
          order.OrderTypeId = tuitionTypeId;
          order.OrderDate = DateTime.Now;
          order.OrderLabel = "inscription";
          order.Deadline = newTuition.Deadline;
          order.Validity = newTuition.Validity;
          order.TotalHT = newTuition.OrderAmount;
          order.AmountHT = order.TotalHT - order.Discount;
          order.AmountTTC = order.TotalHT;
          order.Created = true;
          order.isReg = true;
          _repo.Add(order);

          if(! await _repo.SaveAll())
            return BadRequest("problème pour ajouter l'inscription");
          order.OrderNum = order.Id.GetOrderNumber();

          //children
          List<TuitionChildDataDto> childTuitions = newTuition.Children;
          List<ChildRegistrationDto> children = new List<ChildRegistrationDto>();
          List<OrderLine> lines = new List<OrderLine>();
          List<OrderLineDto> linesDto = new List<OrderLineDto>();
          List<User> ChildList = new List<User>();
          foreach (var child in childTuitions)
          {
            User user = new User();
            var GUID = Guid.NewGuid();
            user.UserName = GUID.ToString();
            user.UserTypeId = studentTypeId;
            user.LastName = child.LastName;
            user.FirstName = child.FirstName;
            user.Gender = child.Sex;
            user.DateOfBirth = child.DateOfBirth;
            user.ClassLevelId = child.ClassLevelId;
            var result = await _userManager.CreateAsync(user, password);
            if(result.Errors.Count() > 0)
            {
              identityContextTransaction.Rollback();
              return BadRequest("erreur lors de l'ajout de l'inscription.");
            }
            // add user role
            var role = await _context.Roles.FirstOrDefaultAsync(a => a.Id == memberRoleId);
            _userManager.AddToRoleAsync(user, role.Name).Wait();

            user.IdNum = _repo.GetUserIDNumber(user.Id, user.LastName, user.FirstName);
            _repo.Update(user);
            ChildList.Add(user);

            var nextClassLevel = await _context.ClassLevels.FirstAsync(c => c.Id == child.ClassLevelId);
            var classProduct = await _context.ClassLevelProducts
                                .FirstAsync(c => c.ClassLevelId == nextClassLevel.Id && c.ProductId == tuitionId);
            decimal tuitionFee = Convert.ToDecimal(classProduct.Price);
            decimal DPPct = firstDeadline.Percentage;
            decimal DownPayment = DPPct * tuitionFee;

            OrderLine line = new OrderLine();
            line.OrderId = order.Id;
            line.OrderLineLabel = "inscription de " + child.LastName + " " + child.FirstName;
            line.ProductId = tuitionId;
            line.ProductFee = child.RegFee;
            line.ChildId = user.Id;
            line.ClassLevelId = nextClassLevel.Id;
            line.Qty = 1;
            line.UnitPrice = child.TuitionFee;
            line.TotalHT = line.Qty * line.UnitPrice + line.ProductFee;
            line.AmountHT = line.TotalHT - line.Discount;
            line.AmountTTC = line.AmountHT + line.TVAAmount;
            line.Active = true;
            _repo.Add(line);
            _context.SaveChanges();

            //add data for orderline history
            OrderLineHistory orderlineHistory = new OrderLineHistory();
            orderlineHistory.OrderLineId = line.Id;
            orderlineHistory.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            orderlineHistory.OpDate = order.OrderDate;
            orderlineHistory.Action = "ADD";
            orderlineHistory.OldAmount = 0;
            orderlineHistory.NewAmount = line.AmountTTC;
            orderlineHistory.Delta = orderlineHistory.NewAmount - orderlineHistory.OldAmount;
            orderlineHistory.Cashed = true;
            _repo.Add(orderlineHistory);

            var lineDto = _mapper.Map<OrderLineDto>(line);
            lineDto.DueAmount = child.RegFee + child.DownPayment;
            linesDto.Add(lineDto);

            byte seq = 1;
            foreach (var deadline in deadlines)
            {
              decimal Pct = deadline.Percentage;
              decimal amount = Pct * tuitionFee;
              OrderLineDeadline orderDeadline = new OrderLineDeadline();
              orderDeadline.OrderLineId = line.Id;
              orderDeadline.Percent = Pct;
              orderDeadline.Amount = amount;
              if(seq == 1)
                orderDeadline.ProductFee = line.ProductFee;
              orderDeadline.DueDate = deadline.DueDate;
              orderDeadline.Seq = seq;
              _repo.Add(orderDeadline);
              seq++;
            }

            //an invoice for each child
            Invoice invoice = new Invoice();
            invoice.InvoiceDate = DateTime.Now;
            invoice.Amount = line.AmountTTC;
            invoice.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            invoice.OrderId = order.Id;
            invoice.OrderLineId = line.Id;
            _repo.Add(invoice);
            _context.SaveChanges();
            invoice.InvoiceNum = _repo.GetInvoiceNumber(invoice.Id);
            _context.Update(invoice);

            ChildRegistrationDto crd = new ChildRegistrationDto();
            crd.LastName = child.LastName;
            crd.FirstName = child.FirstName;
            crd.NextClass = nextClassLevel.Name;
            crd.RegistrationFee = child.RegFee.ToString("N0");
            crd.TuitionAmount = classProduct.Price.ToString("N0");
            crd.DueAmountPct = (DPPct * 100).ToString("N0") + "%";
            crd.DueAmount = DownPayment.ToString("N0");
            crd.TotalDueForChild = (child.RegFee + DownPayment).ToString("N0");
            children.Add(crd);
          }

          //father
          User father = new User();
          string fathercode = "";
          RegistrationEmailDto fatherEmail = new RegistrationEmailDto();
          if(newTuition.FActive)
          {
            var GUID = Guid.NewGuid();
            father.UserName = GUID.ToString();
            father.LastName = newTuition.FLastName;
            father.FirstName = newTuition.FFirstName;
            father.Gender = 1;
            father.PhoneNumber = newTuition.FCell;
            father.Email = newTuition.FEmail;
            father.UserTypeId = parentTypeId;
            father.RegCreated = true;
            var result = await _userManager.CreateAsync(father, password);
            if(result.Errors.Count() > 0)
            {
              identityContextTransaction.Rollback();
              return BadRequest("erreur lors de l'ajout de l'inscription.");
            }
            if(result.Succeeded)
            {
              // add user role
              var role = await _context.Roles.FirstOrDefaultAsync(a => a.Id == parentRoleId);
              _userManager.AddToRoleAsync(father, role.Name).Wait();

              father.IdNum = father.Id.ToString().To5Digits();
              fathercode = await _userManager.GenerateEmailConfirmationTokenAsync(father);
              _repo.Update(father);

              foreach (var child in ChildList)
              {
                UserLink ul = new UserLink();
                ul.UserId = child.Id;
                ul.UserPId = father.Id;
                _repo.Add(ul);
              }
            }

            order.FatherId = father.Id;

            emails.Add(fatherEmail);
            fatherEmail.ParentId = father.Id;
            fatherEmail.ParentLastName = father.LastName;
            fatherEmail.ParentFirstName = father.FirstName;
            fatherEmail.ParentEmail = father.Email;
            fatherEmail.ParentCellPhone = father.PhoneNumber;
            fatherEmail.ParentGender = father.Gender;
            fatherEmail.EmailSubject = schoolName + " - inscription pour l'année scolaire prochaine";
            fatherEmail.OrderId = order.Id;
            fatherEmail.OrderNum = order.OrderNum;
            fatherEmail.Token = fathercode;
            fatherEmail.DueDate = firstDeadline.DueDate.ToString("dd/MM/yyyy", frC);
            fatherEmail.TotalAmount = newTuition.DueAmount.ToString("N0");
            fatherEmail.Children = children;
          }

          //mother
          User mother = new User();
          string mothercode = "";
          RegistrationEmailDto motherEmail = new RegistrationEmailDto();
          if(newTuition.MActive)
          {
            var GUID = Guid.NewGuid();
            mother.UserName = GUID.ToString();
            mother.LastName = newTuition.MLastName;
            mother.FirstName = newTuition.MFirstName;
            mother.Gender = 0;
            mother.PhoneNumber = newTuition.MCell;
            mother.Email = newTuition.MEmail;
            mother.UserTypeId = parentTypeId;
            mother.RegCreated = true;

            var result = await _userManager.CreateAsync(mother, password);            
            if(result.Errors.Count() > 0)
            {
              identityContextTransaction.Rollback();
              return BadRequest("erreur lors de l'ajout de l'inscription.");
            }
            if(result.Succeeded)
            {
              // add user role
              var role = await _context.Roles.FirstOrDefaultAsync(a => a.Id == parentRoleId);
              _userManager.AddToRoleAsync(mother, role.Name).Wait();

              mother.IdNum = mother.Id.ToString().To5Digits();
              mothercode = await _userManager.GenerateEmailConfirmationTokenAsync(mother);
              _repo.Update(mother);

              foreach (var child in ChildList)
              {
                UserLink ul = new UserLink();
                ul.UserId = child.Id;
                ul.UserPId = mother.Id;
                _repo.Add(ul);
              }
            }

            order.MotherId = mother.Id;

            emails.Add(motherEmail);
            motherEmail.ParentId = mother.Id;
            motherEmail.ParentLastName = mother.LastName;
            motherEmail.ParentFirstName = mother.FirstName;
            motherEmail.ParentEmail = mother.Email;
            motherEmail.ParentCellPhone = mother.PhoneNumber;
            motherEmail.ParentGender = mother.Gender;
            motherEmail.EmailSubject = schoolName + " - inscription pour l'année scolaire prochaine";
            motherEmail.OrderId = order.Id;
            motherEmail.OrderNum = order.OrderNum;
            motherEmail.Token = mothercode;
            motherEmail.DueDate = firstDeadline.DueDate.ToString("dd/MM/yyyy", frC);
            motherEmail.TotalAmount = newTuition.DueAmount.ToString("N0");
            motherEmail.Children = children;
          }

          _repo.Update(order);

          var template = await _context.EmailTemplates.FirstAsync(t => t.Id == newRegToBePaidEmailId);
          var RegEmails = await _repo.SetEmailDataForRegistration(emails, template.Body, RegDeadLine);
          _context.AddRange(RegEmails);

          // var linesDto = _mapper.Map<IEnumerable<OrderLineDto>>(order.Lines);
          if(await _repo.SaveAll())
          {
            identityContextTransaction.Commit();
            return Ok(new {
              orderId = order.Id,
              orderlines = linesDto
            });
          }
        }
        catch (System.Exception)
        {
          identityContextTransaction.Rollback();
          return BadRequest("erreur lors de l'ajout de l'inscription.");
        }
      }

      return BadRequest("problème pour ajouter l'inscription");
    }

    [HttpGet("TuitionFigures")]
    public async Task<IActionResult> GetTuitionFigures()
    {
      var tuitionIds = await _context.Orders.Where(t => t.OrderTypeId == tuitionTypeId).Select(s => s.Id).ToListAsync();
      var tuitions = await _context.OrderLines.Where(t => tuitionIds.Contains(t.OrderId)).ToListAsync();
      var totalTuitions = tuitions.Where(t => tuitionIds.Contains(t.OrderId)).Count();
      var tuitionsNotValidated = tuitions.Where(t => t.Validated == false).Count();
      var tuitionsValidated = tuitions.Where(t => t.Validated == true).Count();
      var classSpaces = await _context.Classes.SumAsync(c => c.MaxStudent);

      return Ok(new {
        totalTuitions,
        tuitionsNotValidated,
        tuitionsValidated,
        classSpaces
      });
    }

    [HttpGet("TuitionList")]
    public async Task<IActionResult> GetTuitionList()
    {
      var classlevels = await _context.Classes
                                .Include(i => i.ClassLevel)
                                .OrderBy(o => o.ClassLevel.DsplSeq)
                                .Select(c => c.ClassLevel).Distinct().ToListAsync();
      List<TuitionListDto> tuitionList = new List<TuitionListDto>();
      var orderlines = await _context.OrderLines
                              .Include(i => i.Order)
                              .Where(o => o.Order.isReg == true)
                              .ToListAsync();
      var finOpLines = await _context.FinOpOrderLines.Where(f => f.FinOp.Cashed).ToListAsync();
      decimal totalInvoiced = orderlines.Sum(s => s.AmountTTC);
      decimal totalPaid = finOpLines.Sum(s => s.Amount);
      decimal totalTuitions = orderlines.Count();
      decimal totalTuitionsOK = orderlines.Where(s => s.Validated).Count();
      foreach (var level in classlevels)
      {
        TuitionListDto tld = new TuitionListDto();
        tld.ClassLevelId = level.Id;
        tld.ClassLevelName = level.Name;
        tld.NbTuitions = orderlines.Where(o => o.ClassLevelId == level.Id).Count();
        tld.NbTuitionsOK = orderlines.Where(o => o.Validated == true && o.ClassLevelId == level.Id).Count();
        tld.LevelAmount = orderlines.Where(f => f.ClassLevelId == level.Id).Sum(o => o.AmountTTC);
        tld.strLevelAmount = tld.LevelAmount.ToString("N0") + " F";
        tld.LevelAmountOK = finOpLines.Where(o => o.OrderLine.ClassLevelId == level.Id && o.OrderLine.Validated).Sum(o => o.Amount);
        tld.strLevelAmountOK = tld.LevelAmountOK.ToString("N0") + " F";
        tuitionList.Add(tld);
      }

      return Ok(new {
        tuitionList,
        totalInvoiced,
        totalPaid,
        totalTuitions,
        totalTuitionsOK
      });
    }

    [HttpGet("AmountByDeadline")]
    public async Task<IActionResult> GetOrderAmountWithDeadlines()
    {
      var today = DateTime.Now.Date;
      var duedates = _context.OrderLineDeadlines.OrderBy(o => o.DueDate).Select(s => s.DueDate).Distinct();
      List<AmountWithDeadlinesDto> amountDeadlines = new List<AmountWithDeadlinesDto>();
      int i = 0;
      var olddate = new DateTime();
      foreach (var duedate in duedates)
      {
        var linedeadlines = new List<OrderLineDeadline>();
        if(i == 0)
          linedeadlines =  await _context.OrderLineDeadlines.Where(o => o.DueDate <= duedate).ToListAsync();
        else
          linedeadlines =  await _context.OrderLineDeadlines.Where(o => o.DueDate > olddate && o.DueDate <= duedate).ToListAsync();
        decimal invoiced = linedeadlines.Sum(s => s.Amount + s.ProductFee);
        decimal paid = linedeadlines.Where(o => o.Paid).Sum(s => s.Amount + s.ProductFee);
        decimal balance = invoiced - paid;
        decimal lateAmount = 0;
        if(duedate.Date < today)
          lateAmount = linedeadlines.Where(o => !o.Paid).Sum(s => s.Amount + s.ProductFee);

        AmountWithDeadlinesDto awd = new AmountWithDeadlinesDto();
        awd.DueDate = duedate;
        awd.strDueDate = duedate.ToString("dd/MM/yyyy", frC);
        awd.Invoiced = invoiced;
        awd.Paid = paid;
        awd.Balance = balance;
        awd.LateAmount = lateAmount;
        amountDeadlines.Add(awd);

        olddate = duedate;
        i++;
      }

      return Ok(amountDeadlines);
    }

    [HttpGet("LevelLatePayments")]
    public async Task<IActionResult> GetRecoveryData()
    {
      var today = DateTime.Now.Date;
      var activelevels = await _repo.GetActiveClassLevels();
      List<RecoveryForLevelDto> levelRecovery = new List<RecoveryForLevelDto>();
      foreach (var level in activelevels)
      {
        RecoveryForLevelDto rfld = new RecoveryForLevelDto();
        rfld.LevelId = level.Id;
        rfld.LevelName = level.Name;
        rfld.ProductRecovery = new List<ProductRecoveryDto>();

        decimal levelDueAmount = 0;
        List<ProductRecoveryDto> productRecovery = new List<ProductRecoveryDto>();
        rfld.ProductRecovery = productRecovery;
        var products = await _repo.GetActiveProducts();
        foreach (var product in products)
        {
          decimal productDueAmount = 0;
          decimal lateAmount7Days = 0;
          decimal lateAmount15Days = 0;
          decimal lateAmount30Days = 0;
          decimal lateAmount60Days = 0;
          decimal lateAmount60DaysPlus = 0;

          var lines = await _context.OrderLines
                            .Where(o => o.ClassLevelId == level.Id && o.ProductId == product.Id)
                            .ToListAsync();
          
          ProductRecoveryDto prd = new ProductRecoveryDto();
          prd.ProductName = product.Name;
          foreach(var line in lines)
          {
            var lineDeadlines = await _context.OrderLineDeadlines
                                        .Where(o => o.OrderLineId == line.Id)
                                        .OrderBy(o => o.DueDate)
                                        .ToListAsync();
            var amountPaid = await _context.FinOpOrderLines
                                    .Where(f => f.OrderLineId == line.Id && f.FinOp.Cashed)
                                    .SumAsync(s => s.Amount);
            decimal lineDueAmount = 0;
            if(lineDeadlines.Count() > 0)
            {
              foreach(var lineD in lineDeadlines)
              {
                if(lineD.DueDate.Date < today && !lineD.Paid)
                {
                  lineDueAmount = lineD.Amount + lineD.ProductFee - amountPaid;
                  productDueAmount += lineDueAmount;
                  levelDueAmount += lineDueAmount;

                  // split late amount in amounts of days late
                  var nbDaysLate = Math.Abs((today - lineD.DueDate.Date).TotalDays);
                  if(nbDaysLate <= 7)
                    lateAmount7Days += lineDueAmount;
                  else if(nbDaysLate > 7 && nbDaysLate <= 15)
                    lateAmount15Days += lineDueAmount;
                  else if(nbDaysLate > 15 && nbDaysLate <= 30)
                    lateAmount30Days += lineDueAmount;
                  else if(nbDaysLate > 30 && nbDaysLate <= 60)
                    lateAmount60Days += lineDueAmount;
                  else if(nbDaysLate > 60)
                    lateAmount60DaysPlus += lineDueAmount;
                }
              }
            }
            else
            {
              if(line.Deadline.Date < today)
              {
                lineDueAmount = line.AmountTTC - amountPaid;
                productDueAmount += lineDueAmount;
                levelDueAmount += lineDueAmount;

                // split late amount in amounts of days late
                var nbDaysLate = Math.Abs((today - line.Deadline.Date).TotalDays);
                if(nbDaysLate <= 7)
                  lateAmount7Days += line.AmountTTC;
                else if(nbDaysLate > 7 && nbDaysLate <= 15)
                  lateAmount15Days += line.AmountTTC;
                else if(nbDaysLate > 15 && nbDaysLate <= 30)
                  lateAmount30Days += line.AmountTTC;
                else if(nbDaysLate > 30 && nbDaysLate <= 60)
                  lateAmount60Days += line.AmountTTC;
                else if(nbDaysLate > 60)
                  lateAmount60DaysPlus += line.AmountTTC;
              }
            }
          }

          prd.LateAmount = productDueAmount;
          prd.LateAmount7Days = lateAmount7Days;
          prd.LateAmount15Days = lateAmount15Days;
          prd.LateAmount30Days = lateAmount30Days;
          prd.LateAmount60Days = lateAmount60Days;
          prd.LateAmount60DaysPlus = lateAmount60DaysPlus;
          
          rfld.ProductRecovery.Add(prd);
        }

        rfld.LateAmount = levelDueAmount;
        levelRecovery.Add(rfld);
      }

      return Ok(levelRecovery);
    }

    [HttpGet("ChildLatePayments")]
    public async Task<IActionResult> GetChildrenRecoveryData()
    {
      var today = DateTime.Now.Date;
      var childrenFromDB = await _context.Users
                            .Include(i => i.ClassLevel)
                            .Include(i => i.Photos)
                            .Include(i => i.Class)
                            .Where(o => o.UserTypeId == studentTypeId)
                            .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                            .ToListAsync();
      var children = _mapper.Map<IEnumerable<UserForDetailedDto>>(childrenFromDB);
      List<RecoveryForChildDto> childRecovery = new List<RecoveryForChildDto>();
      foreach (var child in children)
      {
        RecoveryForChildDto rfcd = new RecoveryForChildDto();
        rfcd.ProductRecovery = new List<ProductRecoveryDto>();
        rfcd.LastName = child.LastName;
        rfcd.FirstName = child.FirstName;
        rfcd.LevelName = child.ClassLevelName;
        rfcd.ClassName = child.ClassName;
        rfcd.PhotoUrl = child.PhotoUrl;

        decimal childDueAmount = 0;
        List<ProductRecoveryDto> productRecovery = new List<ProductRecoveryDto>();
        var products = await _repo.GetActiveProducts();
        foreach (var product in products)
        {
          ProductRecoveryDto prd = new ProductRecoveryDto();
          prd.ProductName = product.Name;
          decimal productDueAmount = 0;

          decimal lateAmount7Days = 0;
          decimal lateAmount15Days = 0;
          decimal lateAmount30Days = 0;
          decimal lateAmount60Days = 0;
          decimal lateAmount60DaysPlus = 0;

          var lines = await _context.OrderLines
                            .Where(o => o.ChildId == child.Id && o.ProductId == product.Id)
                            .ToListAsync();
          foreach(var line in lines)
          {
            var lineDeadlines = await _context.OrderLineDeadlines
                                        .Where(o => o.OrderLineId == line.Id)
                                        .OrderBy(o => o.DueDate)
                                        .ToListAsync();
            var amountPaid = await _context.FinOpOrderLines
                                    .Where(f => f.OrderLineId == line.Id && f.FinOp.Cashed)
                                    .SumAsync(s => s.Amount);
            decimal lineDueAmount = 0;
            if(lineDeadlines.Count() > 0)
            {
              foreach(var lineD in lineDeadlines)
              {
                if(lineD.DueDate.Date < today && !lineD.Paid)
                {
                  lineDueAmount = lineD.Amount + lineD.ProductFee - amountPaid;
                  productDueAmount += lineDueAmount;
                  childDueAmount += lineDueAmount;

                  // split late amount in amounts of days late
                  var nbDaysLate = Math.Abs((today - lineD.DueDate.Date).TotalDays);
                  if(nbDaysLate <= 7)
                    lateAmount7Days += lineDueAmount;
                  else if(nbDaysLate > 7 && nbDaysLate <= 15)
                    lateAmount15Days += lineDueAmount;
                  else if(nbDaysLate > 15 && nbDaysLate <= 30)
                    lateAmount30Days += lineDueAmount;
                  else if(nbDaysLate > 30 && nbDaysLate <= 60)
                    lateAmount60Days += lineDueAmount;
                  else if(nbDaysLate > 60)
                    lateAmount60DaysPlus += lineDueAmount;
                }
              }
            }
            else
            {
              if(line.Deadline.Date < today)
              {
                lineDueAmount = line.AmountTTC - amountPaid;
                productDueAmount += lineDueAmount;
                childDueAmount += lineDueAmount;

                // split late amount in amounts of days late
                var nbDaysLate = Math.Abs((today - line.Deadline.Date).TotalDays);
                if(nbDaysLate <= 7)
                  lateAmount7Days += line.AmountTTC;
                else if(nbDaysLate > 7 && nbDaysLate <= 15)
                  lateAmount15Days += line.AmountTTC;
                else if(nbDaysLate > 15 && nbDaysLate <= 30)
                  lateAmount30Days += line.AmountTTC;
                else if(nbDaysLate > 30 && nbDaysLate <= 60)
                  lateAmount60Days += line.AmountTTC;
                else if(nbDaysLate > 60)
                  lateAmount60DaysPlus += line.AmountTTC;
              }
            }
          }

          prd.LateAmount = productDueAmount;
          prd.LateAmount7Days = lateAmount7Days;
          prd.LateAmount15Days = lateAmount15Days;
          prd.LateAmount30Days = lateAmount30Days;
          prd.LateAmount60Days = lateAmount60Days;
          prd.LateAmount60DaysPlus = lateAmount60DaysPlus;
          
          rfcd.ProductRecovery.Add(prd);
        }

        rfcd.LateAmount = childDueAmount;
        childRecovery.Add(rfcd);
      }

      return Ok(childRecovery);
    }

    [HttpGet("ChildLatePaymentByLevel/{levelid}")]
    public async Task<IActionResult> GetChildLatePaymentByLevel(int levelid)
    {
      var today = DateTime.Now.Date;
      var childrenFromDB = await _context.Users
                            .Include(i => i.ClassLevel)
                            .Include(i => i.Photos)
                            .Include(i => i.Class)
                            .Where(o => o.ClassLevelId == levelid && o.UserTypeId == studentTypeId)
                            .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                            .ToListAsync();
      var children = _mapper.Map<IEnumerable<UserForDetailedDto>>(childrenFromDB);
      List<RecoveryForChildDto> childRecovery = new List<RecoveryForChildDto>();
      foreach (var child in children)
      {
        RecoveryForChildDto rfcd = new RecoveryForChildDto();
        rfcd.ProductRecovery = new List<ProductRecoveryDto>();
        rfcd.Id = child.Id;
        rfcd.LastName = child.LastName;
        rfcd.FirstName = child.FirstName;
        rfcd.LevelName = child.ClassLevelName;
        rfcd.ClassName = child.ClassName;
        rfcd.PhotoUrl = child.PhotoUrl;

        decimal childDueAmount = 0;
        List<ProductRecoveryDto> productRecovery = new List<ProductRecoveryDto>();
        var products = await _repo.GetActiveProducts();
        foreach (var product in products)
        {
          ProductRecoveryDto prd = new ProductRecoveryDto();
          prd.ProductName = product.Name;
          decimal productDueAmount = 0;

          // decimal lateAmount7Days = 0;
          // decimal lateAmount15Days = 0;
          // decimal lateAmount30Days = 0;
          // decimal lateAmount60Days = 0;
          // decimal lateAmount60DaysPlus = 0;

          var lines = await _context.OrderLines
                            .Where(o => o.ChildId == child.Id && o.ProductId == product.Id)
                            .ToListAsync();
          foreach(var line in lines)
          {
            var lineDeadlines = await _context.OrderLineDeadlines
                                        .Where(o => o.OrderLineId == line.Id)
                                        .OrderBy(o => o.DueDate)
                                        .ToListAsync();
            var amountPaid = await _context.FinOpOrderLines
                                    .Where(f => f.OrderLineId == line.Id && f.FinOp.Cashed)
                                    .SumAsync(s => s.Amount);
            decimal lineDueAmount = 0;
            if(lineDeadlines.Count() > 0)
            {
              foreach(var lineD in lineDeadlines)
              {
                if(lineD.DueDate.Date < today && !lineD.Paid)
                {
                  lineDueAmount = lineD.Amount + lineD.ProductFee - amountPaid;
                  productDueAmount += lineDueAmount;
                  childDueAmount += lineDueAmount;

                  // split late amount in amounts of days late
                  // var nbDaysLate = Math.Abs((today - lineD.DueDate.Date).TotalDays);
                  // if(nbDaysLate <= 7)
                  //   lateAmount7Days += lineDueAmount;
                  // else if(nbDaysLate > 7 && nbDaysLate <= 15)
                  //   lateAmount15Days += lineDueAmount;
                  // else if(nbDaysLate > 15 && nbDaysLate <= 30)
                  //   lateAmount30Days += lineDueAmount;
                  // else if(nbDaysLate > 30 && nbDaysLate <= 60)
                  //   lateAmount60Days += lineDueAmount;
                  // else if(nbDaysLate > 60)
                  //   lateAmount60DaysPlus += lineDueAmount;
                }
              }
            }
            else
            {
              if(line.Deadline.Date < today)
              {
                lineDueAmount = line.AmountTTC + amountPaid;
                productDueAmount += lineDueAmount;
                childDueAmount += lineDueAmount;

                // split late amount in amounts of days late
                // var nbDaysLate = Math.Abs((today - line.Deadline.Date).TotalDays);
                // if(nbDaysLate <= 7)
                //   lateAmount7Days += line.AmountTTC;
                // else if(nbDaysLate > 7 && nbDaysLate <= 15)
                //   lateAmount15Days += line.AmountTTC;
                // else if(nbDaysLate > 15 && nbDaysLate <= 30)
                //   lateAmount30Days += line.AmountTTC;
                // else if(nbDaysLate > 30 && nbDaysLate <= 60)
                //   lateAmount60Days += line.AmountTTC;
                // else if(nbDaysLate > 60)
                //   lateAmount60DaysPlus += line.AmountTTC;
              }
            }
          }

          prd.LateAmount = productDueAmount;
          // prd.LateAmount7Days = lateAmount7Days;
          // prd.LateAmount15Days = lateAmount15Days;
          // prd.LateAmount30Days = lateAmount30Days;
          // prd.LateAmount60Days = lateAmount60Days;
          // prd.LateAmount60DaysPlus = lateAmount60DaysPlus;
          if(productDueAmount > 0)
            rfcd.ProductRecovery.Add(prd);
        }

        rfcd.LateAmount = childDueAmount;
        if(childDueAmount > 0)
          childRecovery.Add(rfcd);
      }

      return Ok(childRecovery);
    }

  }
}