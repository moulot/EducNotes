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

        public static int CalculateAge(this DateTime? theDateTime)
        {
            var age = 0;
            if(theDateTime!=null)
            {
                age = DateTime.Today.Year - Convert.ToInt32(theDateTime?.Year);
                if(theDateTime?.AddYears(age) > DateTime.Today)
                    age--;
            }
          

            return age;
        }

        public static string CalculateTop(this DateTime startHourMin)
        {
            // to be retrieved from appSettings
            var scheduleHourSize = Convert.ToDouble(Startup.StaticConfig.GetSection("AppSettings:DimHourSchedule").Value);
            var startCourseHour = Convert.ToDouble(Startup.StaticConfig.GetSection("AppSettings:CoursesHourStart").Value);

            var netHours = startHourMin.Hour - startCourseHour;
            var mins = startHourMin.Minute;

            var top = scheduleHourSize * (netHours + (double)mins/60) + 2 * netHours;
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
        
        public static string FormatPhoneNumber(string phone)
        {
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
    }
}