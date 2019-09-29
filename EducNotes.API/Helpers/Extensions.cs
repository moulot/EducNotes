using System;
using System.Globalization;
using EducNotes.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
            var age = DateTime.Today.Year - theDateTime.Year;
            if(theDateTime.AddYears(age) > DateTime.Today)
                age--;

            return age;
        }

        public static string CalculateTop(this DateTime startHourMin)
        {
            // to be retrieved from appSettings
            var scheduleHourSize = 100;
            var startCourseHour = 7;

            var netHours = startHourMin.Hour - startCourseHour;
            var mins = startHourMin.Minute;

            var top = scheduleHourSize * (netHours + (double)mins/60) + 2 * netHours;
            top = Math.Round(top, 2);
            return (top + "px").Replace(",", ".");            
        }

        public static string CalculateHeight(this DateTime startHourMin, DateTime endHourMin)
        {
            // to be retrieved from appSettings
            var scheduleHourSize = 100;

            TimeSpan span = endHourMin.Subtract(startHourMin);

            var height = span.TotalMinutes * scheduleHourSize/60;
            height = Convert.ToDouble(Math.Round(height, 2));
            return (height + "px").Replace(",", ".");
        }
    }
}