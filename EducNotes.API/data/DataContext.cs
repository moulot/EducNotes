using EducNotes.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EducNotes.API.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, 
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}

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
        public DbSet<Fee> Fees { get; set; }
        public DbSet<FeeType> FeeTypes { get; set; }
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
  /////////////////////////////////////////////////////////////////////////////////////////////////////
  /////////////////////////////// DATA FROM MOHAMED KABORE ////////////////////////////////////////////
  /////////////////////////////////////////////////////////////////////////////////////////////////////
        public DbSet<ClassType> ClassTypes { get; set; }
        public DbSet<InscriptionType> InscriptionTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<Like>()
                .HasKey(k => new {k.LikerId, k.LikeeId});

            builder.Entity<Like>()
                .HasOne(u => u.Likee)
                .WithMany(u => u.Likers)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(u => u.Liker)
                .WithMany(u => u.Likees)
                .HasForeignKey(u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(u => u.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(u => u.MessagesRecceived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);
        }
    }
}
