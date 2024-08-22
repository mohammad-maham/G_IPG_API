
using G_IPG_API.BusinessLogic;
using G_IPG_API.BusinessLogic.Interfaces;
using G_IPG_API.Interfaces;
using G_IPG_API.Models;
using G_IPG_API.Models.Wallet;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;
using System.Text.Json.Serialization;

namespace Accounting;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Gold Marketing API's", Version = "v1", Description = ".NET Core 8 Web API" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
           {
               {
                   new OpenApiSecurityScheme
                   {
                       Reference = new OpenApiReference
                       {
                           Type=ReferenceType.SecurityScheme,
                           Id="Bearer"
                       }
                   },
                   new string[]{}}
           });
        });

        builder.Services.AddDbContext<GIpgDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("GIPGDbContext"),
options => options.UseNodaTime()));

        builder.Services.AddDbContext<GWalletDbContext>(options =>
       options.UseNpgsql(builder.Configuration.GetConnectionString("GWalletDbContext"),
options => options.UseNodaTime()));

        builder.Services.AddMvc();
        builder.Services.AddProblemDetails();
        builder.Services.AddScoped<IUnitOfWork, GIpgDbContext>();
        builder.Services.AddScoped<ISaman, Saman>();
        builder.Services.AddScoped<IIranKish, IranKish>();
        builder.Services.AddScoped<IMellat, Mellat>();
        builder.Services.AddScoped<IZarrinpal, Zarrinpal>();
        builder.Services.AddScoped<IIPG, IPG>();

        builder.Services.AddProblemDetails();

        WebApplication app = builder.Build();

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        //app.UseMiddleware<ExceptionMiddleware>();
        app.MapControllers();
        app.Run();
    }
}
