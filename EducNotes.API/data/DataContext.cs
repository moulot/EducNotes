using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EducNotes.API.Data {
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>> {
            public DataContext (DbContextOptions<DataContext> options) : base (options) { }

            public DbSet<Value> Values { get; set; }
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
            // public DbSet<Fee> Fees { get; set; }
            // public DbSet<FeeType> FeeTypes { get; set; }
            public DbSet<FinOp> FinOps { get; set; }
            public DbSet<PaymentType> PaymentTypes { get; set; }
            public DbSet<UserEvaluation> UserEvaluations { get; set; }
            public DbSet<UserLink> UserLinks { get; set; }
            public DbSet<UserType> UserTypes { get; set; }
            public DbSet<Schedule> Schedules { get; set; }
            public DbSet<Absence> Absences { get; set; }
            public DbSet<AbsenceType> AbsenceTypes { get; set; }
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
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            /////////////////////////////// DATA FROM MOHAMED KABORE ////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////
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
            public DbSet<OrderLine> OrderLines { get; set; }
            public DbSet<ProductDeadLine> ProductDeadLine { get; set; }
            public DbSet<PayableAt> PayableAt { get; set; }
            public DbSet<Periodicity> Periodicity { get; set; }

            protected override void OnModelCreating (ModelBuilder builder) {
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