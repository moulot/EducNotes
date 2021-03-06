using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EducNotes.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace EducNotes.API.Helpers
{
  public class LogUserActivity : IAsyncActionFilter
  {
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      var resultContext = await next();

      var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
      var repo = resultContext.HttpContext.RequestServices.GetService<IEducNotesRepository>();
      var user = await repo.GetUser(userId, true);
      user.LastActive = DateTime.Now;
      await repo.SaveAll();
    }
  }
}