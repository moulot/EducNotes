using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Dtos;
using Microsoft.AspNetCore.Mvc;
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
    }
}