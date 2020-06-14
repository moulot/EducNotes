using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
    string password;
    int registrationEmailId, tuitionId, nextYearTuitionId, newRegToBePaidEmailId;

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
      registrationEmailId = _config.GetValue<int>("AppSettings:registrationEmailId");
      newRegToBePaidEmailId = _config.GetValue<int>("AppSettings:newRegToBePaidEmailId");
      tuitionId = _config.GetValue<int>("AppSettings:tuitionId");
      nextYearTuitionId = _config.GetValue<int>("AppSettings:nextYearTuitionId");
      password = _config.GetValue<String>("AppSettings:defaultPassword");
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

    [HttpGet("{id}", Name = "GetOrder")]
    public async Task<IActionResult> GetOrder(int id)
    {
      var orderFromRepo = await _repo.GetOrder(id);
      var order = _mapper.Map<OrderDto>(orderFromRepo);
      return Ok(order);
    }

    [HttpGet("balanceData")]
    public async Task<IActionResult> GetBalanceData()
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

    [HttpPost("NewTuition")]
    public async Task<IActionResult> AddNewTuition(TuitionDataDto newTuition)
    {
      var baseUrl = "localhost:4200"; // _config.GetValue<String>("AppSettings:DefaultLink");
      List<Setting> settings = await _context.Settings.ToListAsync();
      var schoolName = settings.First(s => s.Name.ToLower() == "schoolname").Value;
      string RegDeadLine = settings.First(s => s.Name == "RegistrationDeadLine").Value;

      var deadlines = await _context.ProductDeadLines
                      .OrderBy(o => o.DueDate)
                      .Where(p => p.ProductId == tuitionId).ToListAsync();
      var firstDeadline = deadlines.First(d => d.Seq == 1);

      List<RegistrationEmailDto> emails = new List<RegistrationEmailDto>();

      using (TransactionScope scope = new TransactionScope())
      {
        //father
        User father = new User();
        RegistrationEmailDto fatherEmail = new RegistrationEmailDto();
        if(newTuition.FActive)
        {
          father.UserName = newTuition.FEmail;
          father.LastName = newTuition.FLastName;
          father.FirstName = newTuition.FFirstName;
          father.Gender = 1;
          father.PhoneNumber = newTuition.FCell;
          father.Email = newTuition.FEmail;
          father.UserTypeId = parentTypeId;
          father.RegCreated = true;

          var result = await _userManager.CreateAsync(father, password);
          if(result.Errors.Count() > 0)
            return BadRequest("erreur lors de l'ajout de l'inscription.");

          string fCallbackUrl = "";
          if(result.Succeeded)
          {
            var fathercode = await _userManager.GenerateEmailConfirmationTokenAsync(father);
            fCallbackUrl = string.Format("{0}/confirmEmail?id={1}&token={2}", baseUrl, father.Id, HttpUtility.UrlEncode(fathercode));
          }
          else
          {
            return BadRequest("erreur lors de l'ajout de l'inscription.");
          }

          emails.Add(fatherEmail);
          fatherEmail.ParentLastName = father.LastName;
          fatherEmail.ParentFirstName = father.FirstName;
          fatherEmail.ParentEmail = father.Email;
          fatherEmail.ParentCellPhone = father.PhoneNumber;
          fatherEmail.ParentGender = father.Gender;
          fatherEmail.EmailSubject = schoolName + " - inscription pour l'année scolaire prochaine";
          fatherEmail.DueDate = firstDeadline.DueDate.ToString("dd/MM/yyyy", frC);
          fatherEmail.TotalAmount = newTuition.DueAmount.ToString("N0");
        }

        //mother
        User mother = new User();
        RegistrationEmailDto motherEmail = new RegistrationEmailDto();
        if(newTuition.MActive)
        {
          mother.LastName = newTuition.MLastName;
          mother.FirstName = newTuition.MFirstName;
          mother.Gender = 0;
          mother.PhoneNumber = newTuition.MCell;
          mother.Email = newTuition.MEmail;
          mother.UserTypeId = parentTypeId;
          mother.RegCreated = true;

          var result = await _userManager.CreateAsync(mother, password);
          if(result.Errors.Count() > 0)
            return BadRequest("erreur lors de l'ajout de l'inscription.");
          
          string mCallbackUrl = "";
          if(result.Succeeded)
          {
            var mothercode = await _userManager.GenerateEmailConfirmationTokenAsync(mother);
            mCallbackUrl = string.Format("{0}/confirmEmail?id={1}&token={2}", baseUrl, mother.Id, HttpUtility.UrlEncode(mothercode));
          }

          emails.Add(motherEmail);
          motherEmail.ParentLastName = mother.LastName;
          motherEmail.ParentFirstName = mother.FirstName;
          motherEmail.ParentEmail = mother.Email;
          motherEmail.ParentCellPhone = mother.PhoneNumber;
          motherEmail.ParentGender = mother.Gender;
          motherEmail.EmailSubject = schoolName + " - inscription pour l'année scolaire prochaine";
          motherEmail.DueDate = firstDeadline.DueDate.ToString("dd/MM/yyyy", frC);
          motherEmail.TotalAmount = newTuition.DueAmount.ToString("N0");
        }

        //tuition order
        Order order = new Order();
        order.OrderDate = DateTime.Now;
        order.OrderLabel = "inscription";
        if(newTuition.FActive && newTuition.FSendEmail)
          order.FatherId = father.Id;
        if(newTuition.MActive && newTuition.MSendEmail)
          order.MotherId = mother.Id;
        order.Deadline = newTuition.Deadline;
        order.Validity = newTuition.Validity;
        order.TotalHT = newTuition.OrderAmount;
        order.AmountHT = order.TotalHT - order.Discount;
        order.AmountTTC = order.TotalHT;
        order.Status = Convert.ToByte(Order.StatusEnum.ValidatedByClient);
        order.isReg = true;
        _repo.Add(order);
        if(! await _repo.SaveAll())
          return BadRequest("problème pour ajouter l'inscription");
        order.OrderNum = Helpers.Utils.GetOrderNumber(order.Id);
        fatherEmail.OrderId = order.Id;
        motherEmail.OrderId = order.Id;
        fatherEmail.OrderNum = order.OrderNum;
        motherEmail.OrderNum = order.OrderNum;
        _repo.Update(order);
        
        // //children
        fatherEmail.Children = new List<ChildRegistrationDto>();
        motherEmail.Children = new List<ChildRegistrationDto>();
        List<TuitionChildDataDto> children = newTuition.Children;
        List<OrderLine> lines = new List<OrderLine>();
        foreach (var child in children)
        {
          User user = new User();
          user.UserTypeId = studentTypeId;
          user.LastName = child.LastName;
          user.FirstName = child.FirstName;
          user.Gender = child.Sex;
          user.DateOfBirth = child.DateOfBirth;
          _repo.Add(user);

          var nextClassLevel = await _context.ClassLevels.FirstAsync(c => c.Id == child.ClassLevelId);
          ChildRegistrationDto crd = new ChildRegistrationDto();
          crd.LastName = child.LastName;
          crd.FirstName = child.FirstName;
          crd.NextClass = nextClassLevel.Name;
          crd.RegistrationFee = child.RegFee.ToString("N0") + "FCFA";

          var classProduct = await _context.ClassLevelProducts
                              .FirstAsync(c => c.ClassLevelId == nextClassLevel.Id && c.ProductId == tuitionId);
          decimal tuitionFee = Convert.ToDecimal(classProduct.Price);
          decimal DPPct = firstDeadline.Percentage;
          decimal DownPayment = DPPct * tuitionFee;

          crd.TuitionAmount = classProduct.Price.ToString("N0");
          crd.DueAmountPct = (DPPct * 100).ToString("N0") + "%";
          crd.DueAmount = DownPayment.ToString("N0");
          crd.TotalDueForChild = (child.RegFee + DownPayment).ToString("N0");
          fatherEmail.Children.Add(crd);
          motherEmail.Children.Add(crd);

          OrderLine line = new OrderLine();
          line.OrderId = order.Id;
          line.OrderLineLabel = "inscription de " + child.LastName + " " + child.FirstName;
          line.ProductId = tuitionId;
          line.ProductFee = child.RegFee;
          line.ChildId = user.Id;
          line.Qty = 1;
          line.UnitPrice = child.TuitionFee;
          line.TotalHT = line.Qty * line.UnitPrice + line.ProductFee;
          line.AmountHT = line.TotalHT - line.Discount;
          line.AmountTTC = line.AmountHT + line.TVAAmount;
          line.Status = Convert.ToByte(OrderLine.StatusEnum.Created);
          _repo.Add(line);

          byte seq = 1;
          foreach (var deadline in deadlines)
          {
            decimal Pct = deadline.Percentage;
            decimal Payment = Pct * tuitionFee;
            OrderLineDeadline orderDeadline = new OrderLineDeadline();
            orderDeadline.OrderLineId = line.Id;
            orderDeadline.Percent = Pct * 100;
            orderDeadline.Amount = Payment;
            orderDeadline.DueDate = deadline.DueDate;
            orderDeadline.Seq = seq;
            _repo.Add(orderDeadline);
            seq++;
          }
        }

        var template = await _context.EmailTemplates.FirstAsync(t => t.Id == newRegToBePaidEmailId);
        List<Email> RegEmails = _repo.SetEmailDataForRegistration(emails, template.Body, RegDeadLine);
        _context.AddRange(RegEmails);
        if(await _repo.SaveAll())
        {
          scope.Complete();
          return Ok();
        }
      }

      return BadRequest("problème pour ajouter l'inscription");
    }

  }
}