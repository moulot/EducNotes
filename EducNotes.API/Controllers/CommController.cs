using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using EducNotes.API.data;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace EducNotes.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class CommController : ControllerBase
  {
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly IEducNotesRepository _repo;
    private readonly IConfiguration _config;
    public readonly ICacheRepository _cache;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;

    public CommController(DataContext context, IMapper mapper, IEducNotesRepository repo,
      IConfiguration config, ICacheRepository cache)
    {
      _cache = cache;
      _config = config;
      _context = context;
      _mapper = mapper;
      _repo = repo;
      teacherTypeId = _config.GetValue<int> ("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int> ("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int> ("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int> ("AppSettings:studentTypeId");
    }

    [HttpPost("Twilio_SendSMS")]
    public IActionResult SendSMS([FromBody] SmsDto smsData)
    {
      var from = smsData.From;
      var To = "+225" + smsData.To;
      var body = smsData.Body;

      string smsPhone = _config.GetValue<string>("AppSettings:SMS_PHONE");
      string accountSid = _config.GetValue<string>("AppSettings:ACCOUNT_SID");
      string authToken = _config.GetValue<string>("AppSettings:AUTHTOKEN"); ;
      TwilioClient.Init(accountSid, authToken);

      var message = MessageResource.Create(
          body: body,
          from: new Twilio.Types.PhoneNumber(smsPhone),
          to: new Twilio.Types.PhoneNumber(To)
      );

      return Ok();
    }

    [HttpPost("SendSMS")]
    public IActionResult Clickatell_SendSMS([FromBody] clickatellParamsDto smsData)
    {
      Dictionary<string, string> Params = new Dictionary<string, string>();
      Params.Add("content", smsData.Content);
      Params.Add("to", smsData.To);
      Params.Add("validityPeriod", "1");
      Params.Add("callback", "3");
      Params.Add("deliv_ack", "1");

      Params["to"] = CreateRecipientList(Params["to"]);
      string JsonArray = JsonConvert.SerializeObject(Params, Formatting.None);
      JsonArray = JsonArray.Replace("\\\"", "\"").Replace("\"[", "[").Replace("]\"", "]");

      string Token = _config.GetValue<string>("AppSettings:CLICKATELL_TOKEN");

      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
      var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://platform.clickatell.com/messages");
      httpWebRequest.ContentType = "application/json";
      httpWebRequest.Method = "POST";
      httpWebRequest.Accept = "application/json";
      httpWebRequest.PreAuthenticate = true;
      httpWebRequest.Headers.Add("Authorization", Token);

      using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
        streamWriter.Write(JsonArray);
        streamWriter.Flush();
        streamWriter.Close();
      }

      var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
      using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
      {
        var result = streamReader.ReadToEnd();
        return Ok(result);
      }
    }

    //This function converts the recipients list into an array string so it can be parsed correctly by the json array.
    public static string CreateRecipientList(string to)
    {
      string[] tmp = to.Split(',');
      to = "[\"";
      to = to + string.Join("\",\"", tmp);
      to = to + "\"]";
      return to;
    }

    [AllowAnonymous]
    [HttpGet("CallBack")]
    public async Task<IActionResult> GetCallBackData()
    {
      string query = Request.QueryString.ToString();

      String messageId = HttpUtility.ParseQueryString(query).Get("messageId");
      String integrationName = HttpUtility.ParseQueryString(query).Get("integrationName");
      String clientMsgId = HttpUtility.ParseQueryString(query).Get("clientMessageId");
      String status = HttpUtility.ParseQueryString(query).Get("status");
      String statusDesc = HttpUtility.ParseQueryString(query).Get("statusDesc");
      string timeStamp = HttpUtility.ParseQueryString(query).Get("timeStamp");
      string statusCode = HttpUtility.ParseQueryString(query).Get("statusCode");
      string requestId = HttpUtility.ParseQueryString(query).Get("requestId");

      var sms = await _context.Sms.FirstOrDefaultAsync(s => s.res_ApiMsgId == messageId);
      if (sms != null)
      {
        if (Convert.ToInt64(timeStamp) > Convert.ToInt64(sms.cb_TimeStamp))
        {
          sms.cb_IntegrationName = integrationName;
          sms.cb_Status = status;
          sms.cb_StatusDesc = statusDesc;
          sms.cb_StatusCode = Convert.ToInt32(statusCode);
          sms.cb_RequestId = requestId;
          sms.cb_TimeStamp = timeStamp;
          _context.Update(sms);
          if (await _repo.SaveAll())
            return NoContent();
        }
      }

      throw new Exception($"saisie des data du callBack a échoué");
    }

    [HttpGet("EmailCategories")]
    public async Task<IActionResult> GetEmailCtegories()
    {
      var emailCats = await _context.EmailCategories.OrderBy(c => c.Name).ToListAsync();
      return Ok(emailCats);

    }

    [HttpGet("SmsCategories")]
    public async Task<IActionResult> GetSmsCtegories()
    {
      var smsCats = await _context.SmsCategories.OrderBy(c => c.Name).ToListAsync();
      return Ok(smsCats);

    }

    [HttpGet("EmailBroadCastData")]
    public async Task<IActionResult> GetEmailBroadCastData()
    {
      var schools = await _repo.GetSchools();
      var cycles = await _repo.GetCycles();
      var educLevels = await _repo.GetEducationLevelsWithClasses();
      return Ok (new {
        schools,
        cycles,
        educLevels
      });
    }

    [HttpGet ("EmailTemplates")]
    public async Task<IActionResult> GetEmailTemplates()
    {
      var emailTemplates = await _context.EmailTemplates.Include(i => i.EmailCategory)
                                                        .OrderBy(o => o.Name).ToListAsync();

      List<BroadcastMessageDto> templates = new List<BroadcastMessageDto>();
      foreach(var tpl in emailTemplates)
      {
        BroadcastMessageDto ebdd = new BroadcastMessageDto();
        ebdd.Id = tpl.Id;
        ebdd.Name = tpl.Name;
        ebdd.Subject = tpl.Subject;
        ebdd.BodyContent = tpl.Body;
        ebdd.EmailCategoryName = tpl.EmailCategory.Name;
        templates.Add(ebdd);
      }
      return Ok(templates);
    }

    // [HttpGet("EmailTemplates")]
    // public async Task<IActionResult> GetEmailTemplates()
    // {
    //   var templates = await _context.EmailTemplates
    //                           .Include(i => i.EmailCategory)
    //                           .OrderBy(s => s.Name).ToListAsync();
    //   var templatesToReturn = _mapper.Map<IEnumerable<EmailTemplateForListDto>>(templates);
    //   return Ok(templatesToReturn);
    // }

    [HttpGet("EmailTemplates/{id}")]
    public async Task<IActionResult> GetEmailTemplate(int id)
    {
      var template = await _context.EmailTemplates.FirstOrDefaultAsync(t => t.Id == id);
      return Ok(template);
    }

    [HttpPut("SaveEmailTemplate")]
    public async Task<IActionResult> AddEmailTemplate([FromBody] EmailTemplateForSaveDto emailTemplateDto)
    {
      var id = emailTemplateDto.Id;
      if (id == 0)
      {
        EmailTemplate newTemplate = new EmailTemplate();
        _mapper.Map(emailTemplateDto, newTemplate);
        _repo.Add(newTemplate);
      }
      else
      {
        var templateFromRepo = await _repo.GetEmailTemplate(id);
        _mapper.Map(emailTemplateDto, templateFromRepo);
        _repo.Update(templateFromRepo);
      }

      if (await _repo.SaveAll())
      {
        // await _cache.LoadEmailTemplates();
        return Ok();
      }

      // throw new Exception($"Updating/Saving agendaItem failed");

      return BadRequest("problème pour ajouter le modèle");
    }

    [HttpGet("SmsTemplates")]
    public async Task<IActionResult> GetSmsTemplates()
    {
      var smsTemplates = await _context.SmsTemplates.Include(i => i.SmsCategory)
                                                    .OrderBy(o => o.Name).ToListAsync();

      List<BroadcastMessageDto> templates = new List<BroadcastMessageDto>();
      foreach(var tpl in smsTemplates)
      {
        BroadcastMessageDto msg = new BroadcastMessageDto();
        msg.Id = tpl.Id;
        msg.Name = tpl.Name;
        msg.BodyContent = tpl.Content;
        msg.SmsCategoryName = tpl.SmsCategory.Name;
        templates.Add(msg);
      }
      return Ok(templates);
    }

    // [HttpGet("SmsTemplates")]
    // public async Task<IActionResult> GetSmsTemplates()
    // {
    //   var templates = await _context.SmsTemplates
    //                           .Include(i => i.SmsCategory)
    //                           .OrderBy(s => s.Name).ToListAsync();
    //   var templatesToReturn = _mapper.Map<IEnumerable<SmsTemplateForListDto>>(templates);
    //   return Ok(templatesToReturn);
    // }

    [HttpGet("SmsTemplates/{id}")]
    public async Task<IActionResult> GetSmsTemplate(int id)
    {
      var template = await _context.SmsTemplates.FirstOrDefaultAsync(t => t.Id == id);
      return Ok(template);
    }

    [HttpPut("SaveSmsTemplate")]
    public async Task<IActionResult> AddSmsTemplate([FromBody] SmsTemplateForSaveDto smsTemplateDto)
    {
      var id = smsTemplateDto.Id;
      if (id == 0)
      {
        SmsTemplate newTemplate = new SmsTemplate();
        _mapper.Map(smsTemplateDto, newTemplate);
        _repo.Add(newTemplate);
      }
      else
      {
        var templateFromRepo = await _repo.GetSmsTemplate(id);
        _mapper.Map(smsTemplateDto, templateFromRepo);
        _repo.Update(templateFromRepo);
      }

      if (await _repo.SaveAll())
      {
        // await _cache.LoadSmsTemplates();
        return Ok();
      }

      // throw new Exception($"Updating/Saving agendaItem failed");

      return BadRequest("problème pour ajouter le modèle sms");
    }

    [HttpGet("Tokens")]
    public async Task<IActionResult> GetTokens()
    {
      var tokens = await _context.Tokens.OrderBy(t => t.Name).ToListAsync();
      return Ok(tokens);
    }

    [HttpPost("UsersBroadCastMessaging")]
    public async Task<IActionResult> UsersBroadcastMessaging()
    {
      var users = await _context.Users.ToListAsync();
      return Ok();
    }

    [HttpPost("BroadcastRecap")]
    public async Task<IActionResult> BroadcastRecap(DataForBroadcastDto dataForMsgDto)
    {
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
      List<int> userTypeIds = dataForMsgDto.UserTypeIds;
      List<int> educLevelIds = dataForMsgDto.EducLevelIds;
      List<int> schoolIds = dataForMsgDto.SchoolIds;
      List<int> classLevelIds = new List<int>(); //dataForEmailDto.ClassLevelIds;
      List<int> classIds = dataForMsgDto.ClassIds;
      int msgType = dataForMsgDto.MsgType;
      int msgToRecipients = dataForMsgDto.MsgToRecipients;
      Boolean sendToNotValidated = dataForMsgDto.SendToUSersNOK;
      int templateId = dataForMsgDto.TemplateId;
      string subject = "";
      string body = "";

      //did we select a template?
      if(templateId != 0)
      {
        if(msgType == 1)
        {
          var template = await _context.EmailTemplates.FirstAsync(t => t.Id == templateId);
          subject = template.Subject;
          body = template.Subject;
        }
        else
        {
          var template = await _context.SmsTemplates.FirstAsync(t => t.Id == templateId);
          body = template.Content;
        }
      }
      else
      {
        subject = dataForMsgDto.Subject;
        body = dataForMsgDto.Body;
      }

      List<UserForDetailedDto> recipients = new List<UserForDetailedDto>();
      List<UserForDetailedDto> usersNOK = new List<UserForDetailedDto>();
      var usersFromDB = new List<User>();
      usersFromDB = await _context.Users.Where(u => userTypeIds.Contains(Convert.ToInt32(u.UserTypeId))).ToListAsync();
      var usersList = _mapper.Map<List<UserForDetailedDto>>(usersFromDB);

      MsgRecipientsDto msgRecipients = new MsgRecipientsDto();
      switch(msgToRecipients)
      {
        case 0:
          msgRecipients = _repo.GetMsgRecipientsForUsers(usersList, msgType, sendToNotValidated);
          recipients = msgRecipients.UsersOK;
          usersNOK =  msgRecipients.UsersNOK;
          break;
        case 1:
          msgRecipients = await _repo.GetMsgRecipientsForClasses(usersList, educLevelIds, schoolIds, 
            classLevelIds, classIds, msgType, sendToNotValidated);
          recipients = msgRecipients.UsersOK;
          usersNOK =  msgRecipients.UsersNOK;
          break;
        case 2:
          break;
        default:
          break;
      }
      
      return Ok(new {
        recipients,
        usersNOK,
        subject,
        body
      });
    }

    [HttpPost("ClassesBroadcastMessaging")]
    public async Task<IActionResult> ClassesBroadcastMessaging(DataForBroadcastDto dataForMsgDto)
    {
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
      List<int> userTypeIds = dataForMsgDto.UserTypeIds;
      List<int> educLevelIds = dataForMsgDto.EducLevelIds;
      List<int> schoolIds = dataForMsgDto.SchoolIds;
      List<int> classLevelIds = new List<int>();
      List<int> classIds = dataForMsgDto.ClassIds;
      int msgType = dataForMsgDto.MsgType;
      int templateId = dataForMsgDto.TemplateId;
      string subject = "";
      string body = "";

      List<BroadcastDataDto> recipients = new List<BroadcastDataDto>();
      List<UserForDetailedDto> usersWithNoEmailSms = new List<UserForDetailedDto>();

      var users = new List<User>();
      foreach(var ut in userTypeIds)
      {
        if(ut == studentTypeId || ut == parentTypeId)
        {
          if(users.Count() == 0)
          {
            users = await _context.Users
              .Where(u => classIds.Contains(Convert.ToInt32(u.ClassId)) && u.UserTypeId == studentTypeId).ToListAsync();
          }

          if(ut == studentTypeId)
          {
            foreach(var user in users)
            {
              BroadcastDataDto userData = new BroadcastDataDto();
              userData.RecipientId = user.Id;
              if(msgType == 1) //email
              {
                if(user.EmailConfirmed)
                {
                  userData.RecipientEmail = user.Email;
                  recipients.Add(userData);
                }
                else
                {
                  UserForDetailedDto userNoEmailSms = _mapper.Map<UserForDetailedDto>(user);
                  usersWithNoEmailSms.Add(userNoEmailSms);
                }
              }
              else
              {
                if(user.PhoneNumberConfirmed)
                {
                  userData.RecipientMobile = user.PhoneNumber;
                  recipients.Add(userData);
                }
                else
                {
                  UserForDetailedDto userNoEmailSms = _mapper.Map<UserForDetailedDto>(user);
                  usersWithNoEmailSms.Add(userNoEmailSms);
                }
              }
            }
          }

          if(ut == parentTypeId)
          {
            var ids = users.Select(u => u.Id);
            var parents = _context.UserLinks.Where(u => ids.Contains(u.UserId)).Select(u => u.UserP).Distinct();

            foreach(var user in parents)
            {
              BroadcastDataDto userData = new BroadcastDataDto();
              userData.RecipientId = user.Id;
              if(msgType == 1) //email
              {
                if(user.EmailConfirmed)
                {
                  userData.RecipientEmail = user.Email;
                  recipients.Add(userData);
                }
                else
                {
                  UserForDetailedDto userNoEmailSms = _mapper.Map<UserForDetailedDto>(user);
                  usersWithNoEmailSms.Add(userNoEmailSms);
                }
              }
              else
              {
                if(user.PhoneNumberConfirmed)
                {
                  userData.RecipientMobile = user.PhoneNumber;
                  recipients.Add(userData);
                }
                else
                {
                  UserForDetailedDto userNoEmailSms = _mapper.Map<UserForDetailedDto>(user);
                  usersWithNoEmailSms.Add(userNoEmailSms);
                }
              }
            }
          }
        }

        if(ut == teacherTypeId)
        {
          List<ClassCourse> classCourses = await _context.ClassCourses.Include(i => i.Teacher).ToListAsync();
          var teachers = classCourses
            .Where(t => classIds.Contains(t.ClassId) && t.Teacher.UserTypeId == teacherTypeId)
            .Select(t => t.Teacher).Distinct().ToList();

          foreach (var user in teachers)
          {
            BroadcastDataDto userData = new BroadcastDataDto();
            userData.RecipientId = user.Id;
            if(msgType == 1) //email
            {
              if(user.EmailConfirmed)
              {
                userData.RecipientEmail = user.Email;
                recipients.Add(userData);
              }
              else
              {
                UserForDetailedDto userNoEmailSms = _mapper.Map<UserForDetailedDto>(user);
                usersWithNoEmailSms.Add(userNoEmailSms);
              }
            }
            else
            {
              if(user.PhoneNumberConfirmed)
              {
                userData.RecipientMobile = user.PhoneNumber;
                recipients.Add(userData);
              }
              else
              {
                UserForDetailedDto userNoEmailSms = _mapper.Map<UserForDetailedDto>(user);
                usersWithNoEmailSms.Add(userNoEmailSms);
              }
            }
          }
        }
      }

      List<Setting> settings = await _context.Settings.ToListAsync();
      var schoolName = settings.First(s => s.Name.ToLower() == "schoolname");

      if(recipients.Count() > 0)
      {
        if(msgType == 1) //email
        {
          //did we select a template?
          if (templateId != 0)
          {
            var template = await _context.EmailTemplates.FirstAsync(t => t.Id == templateId);
            subject = dataForMsgDto.Subject;
            return NoContent();
          }
          else
          {
            body = dataForMsgDto.Body;
            subject = dataForMsgDto.Subject;

            List<Email> emailsToBeSent = new List<Email>();
            //save emails to Emails table
            foreach (var recipient in recipients)
            {
              Email newEmail = new Email ();
              newEmail.EmailTypeId = 1;
              newEmail.ToUserId = recipient.RecipientId;
              newEmail.FromAddress = "no-reply@educnotes.com";
              newEmail.Subject = subject;
              newEmail.Body = body;
              newEmail.ToAddress = recipient.RecipientEmail;
              newEmail.TimeToSend = DateTime.Now;
              newEmail.InsertUserId = currentUserId;
              newEmail.InsertDate = DateTime.Now;
              newEmail.UpdateUserId = currentUserId;
              newEmail.UpdateDate = DateTime.Now;
              emailsToBeSent.Add(newEmail);
            }
            _context.AddRange(emailsToBeSent);
          }
        }
        else //sms
        {
          //did we select a template?
          if (templateId != 0)
          {
            var template = await _context.SmsTemplates.FirstAsync(t => t.Id == templateId);
            return NoContent();
          }
          else
          {
            body = dataForMsgDto.Body;

            List<Sms> smsToBeSent = new List<Sms>();
            //save emails to Emails table
            foreach (var recipient in recipients)
            {
              Sms newSms = new Sms();
              newSms.SmsTypeId = 3;
              newSms.ToUserId = recipient.RecipientId;
              newSms.To = recipient.RecipientMobile;
              newSms.Content = body;
              newSms.InsertUserId = currentUserId;
              newSms.InsertDate = DateTime.Now;
              newSms.UpdateDate = DateTime.Now;
              smsToBeSent.Add(newSms);
            }
            _context.AddRange(smsToBeSent);
          }
        }

        if(!await _repo.SaveAll())
        {
          return BadRequest ("problème pour envoyer les messages");
        }
      }

      return Ok(new {
        nbMgsSent = recipients.Count(),
        usersWithNoEmailSms
      });
    }

  }
}