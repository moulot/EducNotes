using System.Linq;
using EducNotes.API.Data;

namespace EducNotes.API.Helpers {
  public class CacheKeys {

    public CacheKeys(DataContext context)
    {
      string subDomain = context.Settings.FirstOrDefault(s => s.Name.ToLower () == "subdomain").Value;
      Users = subDomain + "Users";
      TeacherCourses = subDomain + "TeacherCourses";
      ClassCourses = subDomain + "ClassCourses";
      ClassLevels = subDomain + "ClassLevels";
      ClassLevelProducts = subDomain + "ClassLevelProducts";
      Classes = subDomain + "Classes";
      EducLevels = subDomain + "EducLevels";
      Schools = subDomain + "Schools";
      Cycles = subDomain + "Cycles";
      Courses = subDomain + "Courses";
      ClassTypes = subDomain + "ClassTypes";
      CLClassTypes = subDomain + "CLClassTypes";
      EmailTemplates = subDomain + "EmailTemplates";
      SmsTemplates = subDomain + "SmsTemplates";
      Settings = subDomain + "Settings";
      Tokens = subDomain + "Tokens";
      ProductDeadLines = subDomain + "ProductDeadLines";
      Roles = subDomain + "Roles";
      Orders = subDomain + "Orders";
      OrderLines = subDomain + "OrderLines";
      OrderLineDeadLines = subDomain + "OrderLineDeadLines";
      UserLinks = subDomain + "UserLinks";
      FinOps = subDomain + "FinOps";
      FinOpOrderLines = subDomain + "FinOpOrderLines";
      Cheques = subDomain + "Cheques";
      Banks = subDomain + "Banks";
      PaymentTypes = subDomain + "PaymentTypes";
      Products = subDomain + "Products";
      UserTypes = subDomain + "UserTypes";
    }


    public  string Users { get; set; }
    public  string TeacherCourses { get; set; }
    public  string ClassCourses { get; set; }
    public  string ClassLevels { get; set; }
    public  string ClassLevelProducts { get; set; }
    public  string Classes { get; set; }
    public  string EducLevels { get; set; }
    public  string Schools { get; set; }
    public  string Cycles { get; set; }
    public  string Courses { get; set; }
    public  string ClassTypes { get; set; }
    public  string CLClassTypes { get; set; }
    public  string EmailTemplates { get; set; }
    public  string SmsTemplates { get; set; }
    public  string Settings { get; set; }
    public  string Tokens { get; set; }
    public  string ProductDeadLines { get; set; }
    public  string Roles { get; set; }
    public  string Orders { get; set; }
    public  string OrderLines { get; set; }
    public  string OrderLineDeadLines { get; set; }
    public  string UserLinks { get; set; }
    public  string FinOps { get; set; }
    public  string FinOpOrderLines { get; set; }
    public  string Cheques  { get; set; }
    public  string Banks  { get; set; }
     public  string PaymentTypes  { get; set; }
    public  string Products { get; set; }
    public  string UserTypes { get; set; }
  }
}