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
using EducNotes.API.data;

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
    public ICacheRepository _cache { get; }

    public OrdersController(IEducNotesRepository repo, IMapper mapper, DataContext context,
     IConfiguration config, UserManager<User> userManager, ICacheRepository cache)
    {
      _cache = cache;
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
                                .Where(c => c.ProductId == tuitionId || c.ProductId == nextYearTuitionId)
                                .ToListAsync();

      //order dates
      var todayDate = DateTime.Now;
      var today = todayDate.ToString("dd/MM/yyyy", frC);
      var date = todayDate.AddDays(daysToValidate);
      var tuitionValidity = date.ToString("dd/MM/yyyy", frC);

      return Ok(new
      {
        deadlines,
        classProducts,
        today,
        tuitionValidity
      });
    }

    [HttpGet("tuitionFromChild/{id}")]
    public async Task<IActionResult> GetTuitionFromChild(int id)
    {
      List<Order> orders = await _cache.GetOrders();
      List<OrderLineDeadline> lineDeadlines = await _cache.GetOrderLineDeadLines();
      List<User> students = await _cache.GetStudents();

      var parents = await _repo.GetParents(id);
      int fatherId = 0;
      int motherId = 0;
      var father = parents.FirstOrDefault(p => p.Gender == 1);
      if (father != null)
        fatherId = father.Id;
      var mother = parents.FirstOrDefault(p => p.Gender == 0);
      if (mother != null)
        motherId = mother.Id;
      var order = orders.FirstOrDefault(t => t.isReg && (t.MotherId == motherId || t.FatherId == fatherId));
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
      if (!tuition.Paid)
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
      var lineDeadline = lineDeadlines
                              .Where(l => l.OrderLine.ChildId == id)
                              .OrderBy(o => o.DueDate)
                              .First();
      tuition.DownPayment = lineDeadline.Amount + lineDeadline.ProductFee;
      tuition.Validated = (students.Where(u => u.Id == id).First()).Validated;

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
      List<Order> orders = await _cache.GetOrders();
      List<FinOp> finOps = await _cache.GetFinOps();
      var today = DateTime.Now.Date;

      // invoiced amount
      var invoiced = orders.Sum(s => s.AmountTTC);
      // cashed amount
      var cashed = finOps
                    .Where(o => o.Cashed == true && o.FinOpTypeId == finOpTypePayment)
                    .Sum(s => s.Amount);
      // in validation amount
      var toBeValidated = finOps
                          .Where(o => o.Cashed == false && o.Rejected == false && o.FinOpTypeId == finOpTypePayment)
                          .Sum(s => s.Amount);
      // outstanding balance
      var openBalance = await _context.OrderLineHistories.Where(f => f.Cashed == true).SumAsync(b => b.Delta);

      LateAmountsDto lateAmounts = await _repo.GetLateAmountsDue();

      return Ok(new
      {
        invoiced,
        cashed,
        openBalance,
        toBeValidated,
        lateAmounts
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
          List<ProductDeadLine> ProdDeadLines = await _cache.GetProductDeadLines();
          List<Setting> settings = await _cache.GetSettings();
          List<ClassLevel> classlevels = await _cache.GetClassLevels();
          List<ClassLevelProduct> levelProducts = await _cache.GetClassLevelProducts();
          List<Role> roles = await _cache.GetRoles();
          List<EmailTemplate> emailTemplates = await _cache.GetEmailTemplates();

          var schoolName = settings.First(s => s.Name.ToLower() == "schoolname").Value;
          string RegDeadLine = settings.First(s => s.Name == "RegistrationDeadLine").Value;

          var deadlines = ProdDeadLines
                                .OrderBy(o => o.DueDate)
                                .Where(p => p.ProductId == tuitionId).ToList();
          var firstDeadline = deadlines.First();
          var firstDeadlineDate = firstDeadline.DueDate.Date;

          //tuition order
          Order order = new Order();
          order.OrderTypeId = tuitionTypeId;
          order.OrderDate = DateTime.Now;
          order.OrderLabel = "inscription";
          order.Deadline = newTuition.Deadline;
          order.Validity = newTuition.Validity;
          if (firstDeadlineDate < order.Validity.Date)
            order.Validity = firstDeadlineDate;
          order.TotalHT = newTuition.OrderAmount;
          order.AmountHT = order.TotalHT - order.Discount;
          order.AmountTTC = order.TotalHT;
          // order.Created = true;
          order.isReg = true;
          order.InsertUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
          order.UpdateUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
          _repo.Add(order);

          if (!await _repo.SaveAll())
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
            if (result.Errors.Count() > 0)
            {
              identityContextTransaction.Rollback();
              return BadRequest("erreur lors de l'ajout de l'inscription.");
            }
            // add user role
            var role = roles.FirstOrDefault(a => a.Id == memberRoleId);
            _userManager.AddToRoleAsync(user, role.Name).Wait();

            user.IdNum = _repo.GetUserIDNumber(user.Id, user.LastName, user.FirstName);
            _repo.Update(user);
            ChildList.Add(user);

            var nextClassLevel = classlevels.First(c => c.Id == child.ClassLevelId);
            var classProduct = levelProducts.First(c => c.ClassLevelId == nextClassLevel.Id && c.ProductId == tuitionId);
            decimal tuitionFee = Convert.ToDecimal(classProduct.Price);
            decimal DPPct = firstDeadline.Percentage;
            decimal DownPayment = DPPct * tuitionFee;

            OrderLine line = new OrderLine();
            line.OrderId = order.Id;
            line.OrderLineLabel = "inscription de " + child.LastName + " " + child.FirstName;
            line.ProductId = tuitionId;
            line.ProductFee = child.RegFee;
            line.Deadline = order.Deadline;
            line.Validity = order.Validity;
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
            foreach(var deadline in deadlines)
            {
              decimal Pct = deadline.Percentage;
              decimal amount = Pct * tuitionFee;
              OrderLineDeadline orderDeadline = new OrderLineDeadline();
              orderDeadline.OrderLineId = line.Id;
              orderDeadline.Percent = Pct;
              orderDeadline.Amount = amount;
              if (seq == 1)
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

          var template = emailTemplates.First(t => t.Id == newRegToBePaidEmailId);

          //father
          if (newTuition.FEmail != "")
          {
            User father = new User();
            string fathercode = "";
            RegistrationEmailDto fatherEmail = new RegistrationEmailDto();
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
            if (result.Errors.Count() > 0)
            {
              identityContextTransaction.Rollback();
              return BadRequest("erreur lors de l'ajout de l'inscription.");
            }
            if (result.Succeeded)
            {
              // add user role
              var role = roles.FirstOrDefault(a => a.Id == parentRoleId);
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
            fatherEmail.EmailSubject = template.Subject.Replace("<NOM_ECOLE>", schoolName);
            fatherEmail.OrderId = order.Id;
            fatherEmail.OrderNum = order.OrderNum;
            fatherEmail.Token = fathercode;
            fatherEmail.DueDate = firstDeadline.DueDate.ToString("dd/MM/yyyy", frC);
            fatherEmail.TotalAmount = newTuition.DueAmount.ToString("N0");
            fatherEmail.Children = children;
          }

          //mother
          if (newTuition.MEmail != "")
          {
            User mother = new User();
            string mothercode = "";
            RegistrationEmailDto motherEmail = new RegistrationEmailDto();
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
            if (result.Errors.Count() > 0)
            {
              identityContextTransaction.Rollback();
              return BadRequest("erreur lors de l'ajout de l'inscription.");
            }
            if (result.Succeeded)
            {
              // add user role
              var role = roles.FirstOrDefault(a => a.Id == parentRoleId);
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
            motherEmail.EmailSubject = template.Subject.Replace("<NOM_ECOLE>", schoolName);
            motherEmail.OrderId = order.Id;
            motherEmail.OrderNum = order.OrderNum;
            motherEmail.Token = mothercode;
            motherEmail.DueDate = firstDeadline.DueDate.ToString("dd/MM/yyyy", frC);
            motherEmail.TotalAmount = newTuition.DueAmount.ToString("N0");
            motherEmail.Children = children;
          }

          _repo.Update(order);

          var RegEmails = await _repo.SetEmailDataForRegistration(emails, template.Body, RegDeadLine);
          _context.AddRange(RegEmails);

          if (await _repo.SaveAll())
          {
            await _cache.LoadOrders();
            await _cache.LoadOrderLines();
            await _cache.LoadOrderLineDeadLines();
            await _cache.LoadUsers();
            await _cache.LoadUserLinks();
            identityContextTransaction.Commit();
            return Ok(new {
              orderId = order.Id,
              orderlines = linesDto
            });
          }
        }
        catch
        {
          identityContextTransaction.Rollback();
          return BadRequest("problème pour ajouter l'inscription");
        }
      }

      return BadRequest("problème pour ajouter l'inscription");
    }

    [HttpGet("TuitionFigures")]
    public async Task<IActionResult> GetTuitionFigures()
    {
      List<Order> orders = await _cache.GetOrders();
      List<OrderLine> lines = await _cache.GetOrderLines();
      List<Class> classes = await _cache.GetClasses();

      var tuitionIds = orders.Where(t => t.OrderTypeId == tuitionTypeId).Select(s => s.Id).ToList();
      var tuitions = lines.Where(t => tuitionIds.Contains(t.OrderId)).ToList();
      var totalTuitions = tuitions.Count();
      var tuitionsNotValidated = tuitions.Where(t => t.Validated == false).Count();
      var tuitionsValidated = tuitions.Where(t => t.Validated == true).Count();
      var classSpaces = classes.Sum(c => c.MaxStudent);

      return Ok(new
      {
        totalTuitions,
        tuitionsNotValidated,
        tuitionsValidated,
        classSpaces
      });
    }

    [HttpGet("TuitionList")]
    public async Task<IActionResult> GetTuitionList()
    {
      List<Class> classes = await _cache.GetClasses();
      List<OrderLine> lines = await _cache.GetOrderLines();
      List<FinOpOrderLine> finOpLinesCached = await _cache.GetFinOpOrderLines();

      var classlevels = classes.OrderBy(o => o.ClassLevel.DsplSeq)
                               .Select(c => c.ClassLevel).Distinct().ToList();
      List<TuitionListDto> tuitionList = new List<TuitionListDto>();
      var orderlines = lines.Where(o => o.Order.isReg == true).ToList();
      var finOpLines = finOpLinesCached.Where(f => f.FinOp.Cashed).ToList();
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
      List<OrderLineDeadline> lineDeadlinesCached = await _cache.GetOrderLineDeadLines();

      var balanceLinesPaid = await _repo.GetOrderLinesPaid();
      var today = DateTime.Now.Date;
      var duedates = lineDeadlinesCached.OrderBy(o => o.DueDate).Select(s => s.DueDate).Distinct();
      List<AmountWithDeadlinesDto> amountDeadlines = new List<AmountWithDeadlinesDto>();
      int i = 0;
      var olddate = new DateTime();
      foreach (var duedate in duedates)
      {
        var linedeadlines = new List<OrderLineDeadline>();
        if (i == 0)
        {
          linedeadlines = lineDeadlinesCached.Where(o => o.DueDate <= duedate).ToList();
        }
        else
        {
          linedeadlines = lineDeadlinesCached.OrderBy(o => o.DueDate)
                                             .Where(o => o.DueDate > olddate && o.DueDate <= duedate).ToList();
        }

        decimal invoiced = linedeadlines.Sum(s => s.Amount + s.ProductFee);

        var lineids = linedeadlines.Select(s => s.OrderLineId);
        // decimal linePaid = 0;
        decimal paid = 0;
        foreach (var lineid in lineids)
        {
          var linePaid = balanceLinesPaid.First(f => f.OrderLineId == lineid).Amount;
          var lined = linedeadlines.First(d => d.OrderLineId == lineid);
          var lineDueAmount = lined.Amount + lined.ProductFee;
          decimal amountpaid = 0;
          if (linePaid >= lineDueAmount)
          {
            paid += lineDueAmount;
            amountpaid = lineDueAmount;
          }
          else
          {
            paid += linePaid;
            amountpaid = linePaid;
          }
          balanceLinesPaid.First(f => f.OrderLineId == lineid).Amount -= amountpaid;
        }

        decimal balance = invoiced - paid;

        AmountWithDeadlinesDto awd = new AmountWithDeadlinesDto();
        awd.DueDate = duedate;
        awd.strDueDate = duedate.ToString("dd/MM/yyyy", frC);
        awd.Invoiced = invoiced;
        awd.Paid = paid;
        awd.Balance = balance;
        awd.IsLate = duedate.Date < today ? true : false;
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

        List<ProductRecoveryDto> productRecovery = new List<ProductRecoveryDto>();
        rfld.ProductRecovery = productRecovery;
        decimal levelDueAmount = 0;
        var products = await _repo.GetActiveProducts();
        foreach (var product in products)
        {
          ProductRecoveryDto prd = new ProductRecoveryDto();
          prd.ProductName = product.Name;
          prd.LateAmounts = await _repo.GetProductLateAmountsDue(product.Id, level.Id);
          levelDueAmount += prd.LateAmounts.TotalLateAmount;
          rfld.ProductRecovery.Add(prd);
        }

        rfld.LateAmountDue = levelDueAmount;
        levelRecovery.Add(rfld);
      }

      return Ok(levelRecovery);
    }

    [HttpGet("ChildLatePayments")]
    public async Task<IActionResult> GetChildrenRecoveryData()
    {
      List<User> students = await _cache.GetStudents();

      var today = DateTime.Now.Date;
      var childrenFromDB = students.Where(o => o.UserTypeId == studentTypeId)
                                   .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                                   .ToList();
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
        products = products.OrderBy(o => o.DsplSeq).ToList();
        foreach (var product in products)
        {
          ProductRecoveryDto prd = new ProductRecoveryDto();
          prd.ProductName = product.Name;
          prd.LateAmounts = await _repo.GetChildLateAmountsDue(product.Id, child.Id);
          childDueAmount += prd.LateAmounts.TotalLateAmount;
          rfcd.ProductRecovery.Add(prd);
        }

        rfcd.LateDueAmount = childDueAmount;
        childRecovery.Add(rfcd);
      }

      return Ok(childRecovery);
    }

    [HttpGet("ChildLatePaymentByLevel/{levelid}")]
    public async Task<IActionResult> GetChildLatePaymentByLevel(int levelid)
    {
      List<User> students = await _cache.GetStudents();
      var today = DateTime.Now.Date;

      var childrenFromDB = students.Where(o => o.ClassLevelId == levelid && o.UserTypeId == studentTypeId)
                                   .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                                   .ToList();
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
          prd.LateAmounts = await _repo.GetChildLateAmountsDue(product.Id, child.Id);
          childDueAmount += prd.LateAmounts.TotalLateAmount;
          productDueAmount += prd.LateAmounts.TotalLateAmount;

          if (productDueAmount > 0)
            rfcd.ProductRecovery.Add(prd);
        }

        rfcd.LateDueAmount = childDueAmount;
        if (childDueAmount > 0)
          childRecovery.Add(rfcd);
      }

      return Ok(childRecovery);
    }

  }
}