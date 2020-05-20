using System;
using System.Text.RegularExpressions;

namespace EducNotes.API.Helpers
{
  public class Utils
  {
    //email validation
    public static bool EmailValid(string email)
    {
      string pattern = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z][a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
      pattern = @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$";
      System.Text.RegularExpressions.Match match = Regex.Match(email.Trim(), pattern, RegexOptions.IgnoreCase);
      if (match.Success)
        return true;
      else
        return false;
    }

    public static int GetOrderNumber(int orderId)
    {
      var today = DateTime.Now;
      string year = today.Year.ToString().Substring(2);
      string month = today.Month.ToString().Length == 1 ? "0" + today.Month.ToString() : today.Month.ToString();
      string day = today.Day.ToString().Length == 1 ? "0" +  today.Day.ToString() : today.Day.ToString();
      var data = year + month + day + orderId.ToString();
      return Convert.ToInt32(data);
    }
  }
}