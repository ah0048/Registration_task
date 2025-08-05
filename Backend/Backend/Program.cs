
using Backend.Helpers;
using Backend.Mapper;
using Backend.Models;
using Backend.Repositories.Implementations;
using Backend.Repositories.Interfaces;
using Backend.Services.Implementation;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            builder.Services.AddControllers();

            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddIdentity<CompanyUser, IdentityRole>(opts => {
                opts.Password.RequireDigit = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireNonAlphanumeric = true;
                opts.Password.RequiredLength = 7;
                opts.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               var key = builder.Configuration["Jwt:Key"];
               var secrectKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

               options.TokenValidationParameters = new TokenValidationParameters()
               {
                   IssuerSigningKey = secrectKey,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
                   ValidAudience = builder.Configuration["Jwt:Audience"],
               };
           });

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingConfig>());

            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            builder.Services.AddTransient<IEmailSender, EmailSenderService>();
            builder.Services.AddScoped<IPhotoService, PhotoService>();

            builder.Services.AddOpenApi();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }
            app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "v1"));

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();


            app.Run();
        }
    }
}
