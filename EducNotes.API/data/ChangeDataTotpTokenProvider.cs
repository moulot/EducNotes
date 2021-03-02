using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace EducNotes.API.data
{
  public class ChangeDataTotpTokenProvider<TUser> : TotpSecurityStampBasedTokenProvider<TUser> where TUser : class
  {  
    public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
    {
      return Task.FromResult(false);
    }

    public override async Task<string> GetUserModifierAsync(string purpose, UserManager<TUser> manager, TUser user)
    {
      var userid = await manager.GetUserIdAsync(user);
      return "ChangeEmail:" + purpose + ":" + userid;
    }
  }

  public class ChangeDataTotpTokenProviderOptions : DataProtectionTokenProviderOptions
  {
  }
}