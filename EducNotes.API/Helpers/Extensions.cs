using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EducNotes.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddPagination(this HttpResponse response,
            int currentPage, int itemsPerPage, int totalItems, int totalPages)
            {
                var paginationHeader = new PaginationHeader(currentPage, itemsPerPage,
                    totalItems, totalPages);
                var camelCaseFormatter = new JsonSerializerSettings();
                camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
                response.Headers.Add("Pagination",
                    JsonConvert.SerializeObject(paginationHeader,camelCaseFormatter));
                response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
            }

        public static int CalculateAge(this DateTime theDateTime)
        {
          var age = 0;
          if(theDateTime!=null)
          {
            age = DateTime.Today.Year - Convert.ToInt32(theDateTime.Year);
            if(theDateTime.AddYears(age) > DateTime.Today)
              age--;
          }
          return age;
        }

        public static string DayIntToName(this int dayInt)
        {
          string dayName = "";
          switch (dayInt)
          {
            case 1:
              dayName = "lundi";
              break;
            case 2:
              dayName = "mardi";
              break;
            case 3:
              dayName = "mercredi";
              break;
            case 4:
              dayName = "jeudi";
              break;
            case 5:
              dayName = "vendredi";
              break;
            case 6:
              dayName = "samedi";
              break;
            case 7:
              dayName = "dimanche";
              break;
            default:
              dayName = "";
              break;
          }

          return dayName;
        }

        public static string CalculateTop(this DateTime startHourMin)
        {
            // to be retrieved from appSettings
            var scheduleHourSize = Convert.ToDouble(Startup.StaticConfig.GetSection("AppSettings:DimHourSchedule").Value);
            var startCourseHour = Convert.ToDouble(Startup.StaticConfig.GetSection("AppSettings:CoursesHourStart").Value);

            var netHours = startHourMin.Hour - startCourseHour;
            var mins = startHourMin.Minute;

            var top = scheduleHourSize * (netHours + (double)mins/60) + 1 * netHours;
            top = Math.Round(top, 2);
            return (top + "px").Replace(",", ".");            
        }

        public static string CalculateHeight(this DateTime startHourMin, DateTime endHourMin)
        {
            // to be retrieved from appSettings
            var scheduleHourSize =  Convert.ToDouble(Startup.StaticConfig.GetSection("AppSettings:DimHourSchedule").Value);

            TimeSpan span = endHourMin.Subtract(startHourMin);

            var height = span.TotalMinutes * scheduleHourSize/60;
            height = Convert.ToDouble(Math.Round(height, 2));
            return (height + "px").Replace(",", ".");
        }

        public static bool IsNumeric(this string str)
        {
          if (str == null)
            return false;

          try
          {
            double result;
            if (double.TryParse(str, out result))
              return true;
            else
              return false;
          }
          catch
          {
            return false;
          }
        }

        // Checks whether there are duplicates in a list.
        public static bool AreAnyDuplicates<T>(this IEnumerable<T> list)
        {
          var hashset = new HashSet<T>();
          return list.Any(e => !hashset.Add(e));
        }

        // Checks whether or not a date is a valid date.
        public static bool IsDate(this string str)
        {
          try
          {
            DateTime dt = DateTime.Parse(str);

            if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
              return true;
            return false;
          }
          catch
          {
            return false;
          }
        }

        public static string FirstLetterToUpper(this string s)
        {
          if (string.IsNullOrEmpty(s))
          {
            return string.Empty;
          }

          char[] a = s.ToCharArray();
          a[0] = char.ToUpper(a[0]);
          return new string(a);
        }

        // public static string FirstLetterToUpper(this string str)
        // {
        //   if (str == null)
        //     return null;

        //   if (str.Length > 1)
        //     return char.ToUpper(str[0]) + str.Substring(1).ToLower();

        //   return str.ToUpper();
        // }

        public static string UppercaseWords(this string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }

        public static string To5Digits(this string data)
        {
          switch (data.Length)
          {
            case 1:
              data = "0000" + data;
              break;
            case 2:
              data = "000" + data;
              break;
            case 3:
              data = "00" + data;
              break;
            case 4:
              data = "0" + data;
              break;
          }
          return data;
        }
        
        public static string FormatPhoneNumber(this string phone)
        {
          if(phone == null)
            return phone;
          if (phone.Length == 8)
          {
            return String.Format("{0}.{1}.{2}.{3}", phone.Substring(0, 2), phone.Substring(2, 2),
              phone.Substring(4, 2), phone.Substring(6));
          }
          return phone;
        }

        public static string RegNum5digits(string refNum)
        {
          string idnum = "";

          if (refNum.Length == 1)
            idnum += "0000" + refNum;
          else if (refNum.Length == 2)
            idnum += "000" + refNum;
          else if (refNum.Length == 3)
            idnum += "00" + refNum;
          else if (refNum.Length == 4)
            idnum += "0" + refNum;
          else
            idnum += refNum;

          return idnum;
        }

        //email validation
        public static bool EmailValid(this string email)
        {
          string pattern = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z][a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
          pattern = @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$";
          System.Text.RegularExpressions.Match match = Regex.Match(email.Trim(), pattern, RegexOptions.IgnoreCase);
          if (match.Success)
            return true;
          else
            return false;
        }

        public static int GetOrderNumber(this int orderId)
        {
          var today = DateTime.Now;
          string year = today.Year.ToString().Substring(2);
          string month = today.Month.ToString().Length == 1 ? "0" + today.Month.ToString() : today.Month.ToString();
          string day = today.Day.ToString().Length == 1 ? "0" +  today.Day.ToString() : today.Day.ToString();
          var data = year + month + day + orderId.ToString();
          return Convert.ToInt32(data);
        }

        public static string GetSubDomain(string url)
        {
          if (url.Split('.').Length > 2)
          {
            int lastIndex = url.LastIndexOf(".");
            int index = url.LastIndexOf(".", lastIndex - 1);
            return url.Substring(0, index).ToLower();
          }
          return null;
        }
    }
}