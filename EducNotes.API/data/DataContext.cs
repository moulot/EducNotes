using EducNotes.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EducNotes.API.Data {
  public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>> {
      private readonly IConfiguration _config;
      private readonly IHttpContextAccessor _httpContext;

      public DataContext (DbContextOptions<DataContext> options, IConfiguration config,
        IHttpContextAccessor httpContext) : base (options) 
      {
        _httpContext = httpContext;
        _config = config;
      }

      public DbSet<Photo> Photos { get; set; }
      public DbSet<Like> Likes { get; set; }
      public DbSet<Message> Messages { get; set; }
      public DbSet<Establishment> Establishments { get; set; }
      public DbSet<Class> Classes { get; set; }
      public DbSet<ClassCourse> ClassCourses { get; set; }
      public DbSet<Course> Courses { get; set; }
      public DbSet<ClassLevel> ClassLevels { get; set; }
      public DbSet<EvalType> EvalTypes { get; set; }
      public DbSet<Evaluation> Evaluations { get; set; }
      public DbSet<Agenda> Agendas { get; set; }
      public DbSet<Bank> Banks { get; set; }
      public DbSet<BankAccount> BankAccounts { get; set; }
      public DbSet<CashDesk> CashDesks { get; set; }
      public DbSet<FinOpType> FinOpTypes { get; set; }
      public DbSet<FinOp> FinOps { get; set; }
      public DbSet<FinOpOrderLine> FinOpOrderLines { get; set; }
      public DbSet<PaymentType> PaymentTypes { get; set; }
      public DbSet<UserEvaluation> UserEvaluations { get; set; }
      public DbSet<UserLink> UserLinks { get; set; }
      public DbSet<UserType> UserTypes { get; set; }
      public DbSet<Schedule> Schedules { get; set; }
      public DbSet<ScheduleCourse> ScheduleCourses { get; set; }
      public DbSet<Absence> Absences { get; set; }
      public DbSet<AbsenceType> AbsenceTypes { get; set; }
      public DbSet<ClassEvent> ClassEvents { get; set; }
      public DbSet<UserClassEvent> UserClassEvents { get; set; }
      public DbSet<Sanction> Sanctions { get; set; }
      public DbSet<Reward> Rewards { get; set; }
      public DbSet<UserSanction> UserSanctions { get; set; }
      public DbSet<UserReward> UserRewards { get; set; }
      public DbSet<Skill> Skills { get; set; }
      public DbSet<ProgramElement> ProgramElements { get; set; }
      public DbSet<CourseSkill> CourseSkills { get; set; }
      public DbSet<Period> Periods { get; set; }
      public DbSet<EvalProgElt> EvalProgElts { get; set; }
      public DbSet<Holiday> Holidays { get; set; }
      public DbSet<District> Districts { get; set; }
      public DbSet<City> Cities { get; set; }
      public DbSet<Inscription> Inscriptions { get; set; }
      public DbSet<Session> Sessions { get; set; }
      public DbSet<Email> Emails { get; set; }
      public DbSet<EmailType> EmailTypes { get; set; }
      public DbSet<ClassLevelSchedule> ClassLevelSchedules { get; set; }
      public DbSet<Event> Events { get; set; }
      public DbSet<EventType> EventTypes { get; set; }
      public DbSet<Sms> Sms { get; set; }
      public DbSet<SmsCategory> SmsCategories { get; set; }
      public DbSet<SmsTemplate> SmsTemplates { get; set; }
      public DbSet<UserSmsTemplate> UserSmsTemplates { get; set; }
      public DbSet<Token> Tokens { get; set; }
      public DbSet<TokenType> TokenTypes { get; set; }
      public DbSet<TeacherCourse> TeacherCourses { get; set; }
      public DbSet<ClassType> ClassTypes { get; set; }
      public DbSet<InscriptionType> InscriptionTypes { get; set; }
      public DbSet<ProductType> ProductTypes { get; set; }
      public DbSet<Product> Products { get; set; }
      public DbSet<ClassLevelProduct> ClassLevelProducts { get; set; }
      public DbSet<DeadLine> DeadLines { get; set; }
      public DbSet<Invoice> Invoices { get; set; }
      public DbSet<CourseCoefficient> CourseCoefficients { get; set; }
      public DbSet<Country> Countries { get; set; }
      public DbSet<Address> Addresses { get; set; }
      public DbSet<Order> Orders { get; set; }
      public DbSet<OrderType> OrderTypes { get; set; }
      public DbSet<OrderHistory> OrderHistories { get; set; }
      public DbSet<OrderLine> OrderLines { get; set; }
      public DbSet<OrderLineHistory> OrderLineHistories { get; set; }
      public DbSet<OrderLineDeadline> OrderLineDeadlines { get; set; }
      public DbSet<ProductDeadLine> ProductDeadLines { get; set; }
      public DbSet<PayableAt> PayableAts { get; set; }
      public DbSet<Periodicity> Periodicities { get; set; }
      public DbSet<Fichier> Fichiers { get; set; }
      public DbSet<Theme> Themes { get; set; }
      public DbSet<Lesson> Lessons { get; set; }
      public DbSet<LessonContent> LessonContents { get; set; }
      public DbSet<LessonSession> LessonSessions { get; set; }
      public DbSet<Document> Documents { get; set; }
      public DbSet<ClassCourseProgress> ClassCourseProgresses { get; set; }
      public DbSet<SmsType> SmsTypes { get; set; }
      public DbSet<EmailTemplate> EmailTemplates { get; set; }
      public DbSet<EmailCategory> EmailCategories { get; set; }
      public DbSet<Setting> Settings { get; set; }
      public DbSet<LessonDoc> LessonDocs { get; set; }
      public DbSet<LessonDocDocument> LessonDocDocuments { get; set; }
      public DbSet<LessonContentDoc> LessonContentDocs { get; set; }
      public DbSet<DocType> DocTypes { get; set; }
      public DbSet<FileType> FileTypes { get; set; }
      public DbSet<EducationLevel> EducationLevels { get; set; }
      public DbSet<Cycle> Cycles { get; set; }
      public DbSet<School> Schools { get; set; }
      public DbSet<Cheque> Cheques { get; set; }
      public DbSet<LoginPageInfo> LoginPageInfos { get; set; }
      public DbSet<ClassLevelCourse> ClassLevelCourses { get; set; }
      public DbSet<ClassLevelClassType> ClassLevelClassTypes { get; set; }
      public DbSet<MenuItem> MenuItems { get; set; }
      public DbSet<Capability> Capabilities { get; set; }
      public DbSet<RoleCapability> RoleCapabilities { get; set; }
      public DbSet<CourseType> CourseTypes { get; set; }
      public DbSet<Conflict> Conflicts { get; set; }
      public DbSet<CourseConflict> CourseConflicts { get; set; }
      public DbSet<MaritalStatus> MaritalStatuses { get; set; }

      protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
      {
        string subdomain = "EducNotes";
        //To get subdomain
        string[] fullAddress = _httpContext.HttpContext?.Request?.Headers?["Host"].ToString ()?.Split ('.');
        if (fullAddress != null)
        {
          subdomain = fullAddress[0].ToLower();
          if(subdomain == "localhost:5000" || subdomain == "test2")
          {
            subdomain = "educnotes";
          }
          else if (subdomain == "test1" || subdomain == "www" || subdomain == "educnotes") {
            subdomain = "demo";
          }
        }
        string tenantConnString = string.Format(_config.GetConnectionString("DefaultConnection"), $"{subdomain}");
        optionsBuilder.UseSqlServer(tenantConnString);
        base.OnConfiguring(optionsBuilder);
      }

      protected override void OnModelCreating (ModelBuilder builder)
      {
        base.OnModelCreating (builder);

        builder.Entity<UserRole> (userRole => {
          userRole.HasKey (ur => new { ur.UserId, ur.RoleId });

          userRole.HasOne (ur => ur.Role)
            .WithMany (r => r.UserRoles)
            .HasForeignKey (ur => ur.RoleId)
            .IsRequired ();

          userRole.HasOne (ur => ur.User)
            .WithMany (r => r.UserRoles)
            .HasForeignKey (ur => ur.UserId)
            .IsRequired ();
        });

        builder.Entity<Like> ()
          .HasKey (k => new { k.LikerId, k.LikeeId });

        builder.Entity<Like> ()
          .HasOne (u => u.Likee)
          .WithMany (u => u.Likers)
          .HasForeignKey (u => u.LikeeId)
          .OnDelete (DeleteBehavior.Restrict);

        builder.Entity<Like> ()
          .HasOne (u => u.Liker)
          .WithMany (u => u.Likees)
          .HasForeignKey (u => u.LikerId)
          .OnDelete (DeleteBehavior.Restrict);

        builder.Entity<Message> ()
          .HasOne (u => u.Sender)
          .WithMany (u => u.MessagesSent)
          .OnDelete (DeleteBehavior.Restrict);

        builder.Entity<Message> ()
          .HasOne (u => u.Recipient)
          .WithMany (u => u.MessagesRecceived)
          .OnDelete (DeleteBehavior.Restrict);

        builder.Entity<Photo> ().HasQueryFilter (p => p.IsApproved);
      }

    }
}