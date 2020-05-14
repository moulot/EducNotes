using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
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
    [Route ("api/[controller]")]
    [ApiController]
    public class CommController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IEducNotesRepository _repo;
        private readonly IConfiguration _config;

        public CommController (DataContext context, IMapper mapper, IEducNotesRepository repo, IConfiguration config) {
            _config = config;
            _context = context;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpPost("Twilio_SendSMS")]
        public IActionResult SendSMS([FromBody] SmsDto smsData)
        {
            var from = smsData.From;
            var To = "+225" + smsData.To;
            var body = smsData.Body;

            string smsPhone = _config.GetValue<string>("AppSettings:SMS_PHONE");
            string accountSid = _config.GetValue<string>("AppSettings:ACCOUNT_SID");
            string authToken = _config.GetValue<string>("AppSettings:AUTHTOKEN");;
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

            using(var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
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
          if(sms != null)
          {
            if(Convert.ToInt64(timeStamp) > Convert.ToInt64(sms.cb_TimeStamp))
            {
              sms.cb_IntegrationName = integrationName;
              sms.cb_Status = status;
              sms.cb_StatusDesc = statusDesc;
              sms.cb_StatusCode = Convert.ToInt32(statusCode);
              sms.cb_RequestId = requestId;
              sms.cb_TimeStamp = timeStamp;
              _context.Update(sms);
              if(await _repo.SaveAll())
                return NoContent();
            }
          }

          throw new Exception($"saisie des data du callBack a échoué");
        }

        [HttpGet("EmailCategories")]
        public async Task<IActionResult> GetEmailCtegories()
        {
          var emailCats =  await _context.EmailCategories.OrderBy(c => c.Name).ToListAsync();
          return Ok(emailCats);
    
        }

        [HttpGet("SmsCategories")]
        public async Task<IActionResult> GetSmsCtegories()
        {
          var smsCats =  await _context.SmsCategories.OrderBy(c => c.Name).ToListAsync();
          return Ok(smsCats);
    
        }

        [HttpGet("EmailTemplates")]
        public async Task<IActionResult> GetEmailTemplates()
        {
          var templates = await _context.EmailTemplates
                                  .Include(i => i.EmailCategory)
                                  .OrderBy(s => s.Name).ToListAsync();
          var templatesToReturn = _mapper.Map<IEnumerable<EmailTemplateForListDto>>(templates);
          return Ok(templatesToReturn);
        }

        [HttpGet("EmailTemplates/{id}")]
        public async Task<IActionResult> GetEmailTemplate(int id)
        {
          var template = await _context.EmailTemplates.FirstOrDefaultAsync(t => t.Id == id);
          return Ok(template);
        }

        [HttpPut("SaveEmailTemplate")]
        public async Task<IActionResult> AddEmailTemplate ([FromBody] EmailTemplateForSaveDto emailTemplateDto)
        {
          var id = emailTemplateDto.Id;
          if(id == 0) {
              EmailTemplate newTemplate = new EmailTemplate();
              _mapper.Map(emailTemplateDto, newTemplate);
              _repo.Add(newTemplate);
          } else {
              var templateFromRepo = await _repo.GetEmailTemplate(id);
              _mapper.Map(emailTemplateDto, templateFromRepo);
              _repo.Update(templateFromRepo);
          }

          if (await _repo.SaveAll()) { return Ok(); }

          // throw new Exception($"Updating/Saving agendaItem failed");

          return BadRequest("ajout du email modèle a échoué");
        }

        [HttpGet("SmsTemplates")]
        public async Task<IActionResult> GetSmsTemplates()
        {
          var templates = await _context.SmsTemplates
                                  .Include(i => i.SmsCategory)
                                  .OrderBy(s => s.Name).ToListAsync();
          var templatesToReturn = _mapper.Map<IEnumerable<SmsTemplateForListDto>>(templates);
          return Ok(templatesToReturn);
        }

        [HttpGet("SmsTemplates/{id}")]
        public async Task<IActionResult> GetSmsTemplate(int id)
        {
          var template = await _context.SmsTemplates.FirstOrDefaultAsync(t => t.Id == id);
          return Ok(template);
        }

        [HttpPut("SaveSmsTemplate")]
        public async Task<IActionResult> AddSmsTemplate ([FromBody] SmsTemplateForSaveDto smsTemplateDto)
        {
          var id = smsTemplateDto.Id;
          if(id == 0) {
              SmsTemplate newTemplate = new SmsTemplate();
              _mapper.Map(smsTemplateDto, newTemplate);
              _repo.Add(newTemplate);
          } else {
              var templateFromRepo = await _repo.GetSmsTemplate(id);
              _mapper.Map(smsTemplateDto, templateFromRepo);
              _repo.Update(templateFromRepo);
          }

          if (await _repo.SaveAll()) { return Ok(); }

          // throw new Exception($"Updating/Saving agendaItem failed");

          return BadRequest("ajout du modèle sms a échoué");
        }

        [HttpGet("Tokens")]
        public async Task<IActionResult> GetTokens()
        {
          var tokens = await _context.Tokens.OrderBy(t => t.Name).ToListAsync();
          return Ok(tokens);
        }
    }
}