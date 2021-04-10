using System.Linq;
using EducNotes.API.Data;

namespace EducNotes.API.Helpers {
  public class CacheKeys {

    // public CacheKeys(DataContext context)
    // {
    //   string subDomain = context.Settings.FirstOrDefault(s => s.Name.ToLower () == "subdomain").Value;
    //   Users = subDomain + "Users";
    //   TeacherCourses = subDomain + "TeacherCourses";
    //   ClassCourses = subDomain + "ClassCourses";
    //   ClassLevels = subDomain + "ClassLevels";
    //   ClassLevelProducts = subDomain + "ClassLevelProducts";
    //   Classes = subDomain + "Classes";
    //   EducLevels = subDomain + "EducLevels";
    //   Schools = subDomain + "Schools";
    //   Cycles = subDomain + "Cycles";
    //   Courses = subDomain + "Courses";
    //   ClassTypes = subDomain + "ClassTypes";
    //   CLClassTypes = subDomain + "CLClassTypes";
    //   EmailTemplates = subDomain + "EmailTemplates";
    //   SmsTemplates = subDomain + "SmsTemplates";
    //   Settings = subDomain + "Settings";
    //   Tokens = subDomain + "Tokens";
    //   ProductDeadLines = subDomain + "ProductDeadLines";
    //   Roles = subDomain + "Roles";
    //   Orders = subDomain + "Orders";
    //   OrderLines = subDomain + "OrderLines";
    //   OrderLineDeadLines = subDomain + "OrderLineDeadLines";
    //   UserLinks = subDomain + "UserLinks";
    //   FinOps = subDomain + "FinOps";
    //   FinOpOrderLines = subDomain + "FinOpOrderLines";
    //   Cheques = subDomain + "Cheques";
    //   Banks = subDomain + "Banks";
    //   PaymentTypes = subDomain + "PaymentTypes";
    //   Products = subDomain + "Products";
    //   UserTypes = subDomain + "UserTypes";
    // }

    public static string Users { get; set; }
    public static string TeacherCourses { get; set; }
    public static string ClassCourses { get; set; }
    public static string ClassLevels { get; set; }
    public static string ClassLevelProducts { get; set; }
    public static string Classes { get; set; }
    public static string EducLevels { get; set; }
    public static string Schools { get; set; }
    public static string Cycles { get; set; }
    public static string Courses { get; set; }
    public static string ClassTypes { get; set; }
    public static string CLClassTypes { get; set; }
    public static string EmailTemplates { get; set; }
    public static string SmsTemplates { get; set; }
    public static string Settings { get; set; }
    public static string Tokens { get; set; }
    public static string ProductDeadLines { get; set; }
    public static string Roles { get; set; }
    public static string Orders { get; set; }
    public static string OrderLines { get; set; }
    public static string OrderLineDeadLines { get; set; }
    public static string UserLinks { get; set; }
    public static string FinOps { get; set; }
    public static string FinOpOrderLines { get; set; }
    public static string Cheques { get; set; }
    public static string Banks { get; set; }
     public static string PaymentTypes  { get; set; }
    public static string Products { get; set; }
    public static string UserTypes { get; set; }
  }
}