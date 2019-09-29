using System;
using System.Net;
using System.Text;
using AutoMapper;
using EducNotes.API.Data;
using EducNotes.API.Helpers;
using EducNotes.API.Models;
using EducNotes.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EducNotes.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(Options => Options.UseSqlServer(Configuration
                .GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            });
            
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;

                // opt.TokenValidationParameters = new TokenValidationParameters
                // {
                //     ValidateIssuerSigningKey = true,
                //     IssuerSigningKey = key,
                //     ValidateAudience = false,
                //     ValidateIssuer = false,
                //     ValidateLifetime = true,
                //     ClockSkew = TimeSpan.Zero
                // };
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(Options => {
                    Options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
                options.AddPolicy("VipOnly", policy => policy.RequireRole("VIP"));
            });

            // requires
            // using Microsoft.AspNetCore.Identity.UI.Services;
            // using WebPWrecover.Services;
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddMvc(options => 
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                }
            )
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(opt => {
                    opt.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            //services.BuildServiceProvider().GetService<DataContext>().Database.Migrate();
            services.AddCors();
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            //Mapper.Reset();
            services.AddAutoMapper(typeof(EducNotesRepository).Assembly);
            services.AddTransient<Seed>();
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddScoped<IEducNotesRepository, EducNotesRepository>();
            services.AddScoped<LogUserActivity>();
        }

        //  This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseDeveloperExceptionPage();
                // app.UseExceptionHandler(builder => {
                //     builder.Run(async context => {
                //         context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                //         var error = context.Features.Get<IExceptionHandlerFeature>();
                //         if(error != null)
                //         {
                //             context.Response.AddApplicationError(error.Error.Message);
                //             await context.Response.WriteAsync(error.Error.Message);
                //         }
                //     });
                // });
                app.UseHsts();
            }
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
           // seeder.SeedUsers();
        //    app.UseCors(x => x.WithOrigins("http://localhost:4200")
        //         .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
           app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
            app.UseMvc(routes => {
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new {Controller = "Fallback", Action = "Index"}
                );
            });

        }
    }
}
