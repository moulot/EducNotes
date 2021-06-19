using System.Linq;
using EducNotes.API.Data;

namespace EducNotes.API.Helpers
{
  public static class CacheKeys
  {
    public static string Users { get { return "_Users"; } }
    public static string TeacherCourses { get { return "_TeacherCourses"; } }
    public static string ClassCourses { get { return "_ClassCourses"; } }
    public static string ClassLevels { get { return "_ClassLevels"; } }
    public static string ClassLevelProducts { get { return "_ClassLevelProducts"; } }
    public static string Classes { get  { return "_Classes"; }}
    public static string EducLevels { get { return "_EducLevels"; } }
    public static string Schools { get { return "_Schools"; } }
    public static string Cycles { get { return "_Cycles"; } }
    public static string Courses { get { return "_Courses"; } }
    public static string ClassTypes { get { return "_ClassTypes"; } }
    public static string CLClassTypes { get { return "_CLClassTypes"; } }
    public static string EmailTemplates { get { return "_EmailTemplates"; } }
    public static string SmsTemplates { get { return "_SmsTemplates"; } }
    public static string Settings { get { return "_Settings"; } }
    public static string Tokens { get { return "_Tokens"; } }
    public static string ProductDeadLines { get { return "_ProductDeadLines"; } }
    public static string Roles { get { return "_Roles"; } }
    public static string Capabilities { get { return "_Capabilities"; } }
    public static string RoleCapabilities { get { return "_RoleCapabilities"; } }
    public static string Menus { get { return "_Menus"; } }
    public static string MenuItems { get { return "_MenuItems"; } }
    public static string Orders { get { return "_"; } }
    public static string OrderLines { get { return "_OrderLines"; } }
    public static string OrderLineDeadLines { get { return "_OrderLineDeadLines"; } }
    public static string UserLinks { get { return "_UserLinks"; } }
    public static string FinOps { get { return "_FinOps"; } }
    public static string FinOpOrderLines { get { return "_FinOpOrderLines"; } }
    public static string Cheques { get { return "_Cheques"; } }
    public static string Banks { get { return "_Banks"; } }
     public static string PaymentTypes  { get { return "_PaymentTypes"; } }
    public static string Products { get { return "_Products"; } }
    public static string ProductTypes { get { return "_ProductTypes"; } }
    public static string UserTypes { get { return "_UserTypes"; } }
    public static string Schedules { get { return "_Schedules"; } }
    public static string ScheduleCourses { get { return "_ScheduleCourses"; } }
    public static string Agendas { get { return "_Agendas"; } }
    public static string Sessions { get { return "_Sessions"; } }
    public static string Events { get { return "_Events"; } }
    public static string CourseTypes { get { return "_CourseTypes"; } }
    public static string Conflicts { get { return "_Conflicts"; } }
    public static string CourseConflicts { get { return "_CourseConflicts"; } }
    public static string UserRoles { get { return "_UserRoles"; } }
    public static string Countries { get { return "_Countries"; } }
    public static string Cities { get { return "_Cities"; } }
    public static string Districts { get { return "_Districts"; } }
    public static string MaritalStatus { get { return "_MaritalStatus"; } }
    public static string Photos { get { return "_Photos"; } }
  }
}