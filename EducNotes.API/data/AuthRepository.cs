using System;
using System.Threading.Tasks;
using EducNotes.API.Dtos;
using EducNotes.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Net;



namespace EducNotes.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IEducNotesRepository _educNotesRepo;
        private readonly IConfiguration _config;

        public AuthRepository(DataContext context,IEducNotesRepository educNotesRepo, IConfiguration config)
        {
            _context = context;
            _educNotesRepo = educNotesRepo;
            _config = config;
        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.UserName == username);

            if(user == null)
            return null;

            // if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            // return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            // user.PasswordHash = passwordHash;
            // user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.UserName == username))
                return true;
            
            return false;
        }

   

   
    
  

    public async Task<User> GetUserById(int id)
    {
     return await _context.Users.FirstOrDefaultAsync(u=>u.Id==id);
    }

   

    
  

    public async Task<bool> SaveAll()
    {
          return await _context.SaveChangesAsync() > 0;
    }
    public async Task<bool> SendEmail(EmailFormDto emailFormDto)
    {
      var emailMessage = new MimeMessage();

      emailMessage.From.Add(new MailboxAddress("Educ'Notes","no-reply@inscriptions.educNotes.com"));
      emailMessage.To.Add(new MailboxAddress("", emailFormDto.toEmail));
      emailMessage.Subject = emailFormDto.subject;

      emailMessage.Body = new TextPart(TextFormat.Html)
      {
      Text = emailFormDto.content
      };

      try
      {
           
          using (var client = new MailKit.Net.Smtp.SmtpClient())
          {

              var credentials = new NetworkCredential
              {
                  UserName = _config.GetValue<String>("Email:Smtp:Username"), // replace with valid value
                  Password = _config.GetValue<String>("Email:Smtp:Password")// replace with valid value
              };

              // check your smtp server setting and amend accordingly:
              await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.Auto).ConfigureAwait(false);
              await client.AuthenticateAsync(credentials);
              await client.SendAsync(emailMessage).ConfigureAwait(false);
              await client.DisconnectAsync(true).ConfigureAwait(false);

          }
          return true;            
        }
        catch (System.Exception)
        {

            return false;
        }
    }
  }
}