using System.Reflection;
using Microsoft.OpenApi.Models;

namespace NowAround.WebApi.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwaggerSecurity(this IServiceCollection services)
    {
         services.AddSwaggerGen(options =>
         {
             options.SwaggerDoc("v1", new OpenApiInfo
             {
                 Title = "NowAround.Api", 
                 Version = "v1" 
                 
             });
             
             options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
             {
                 Name = "Authorization",
                 Type = SecuritySchemeType.Http,
                 Scheme = "Bearer",
                 BearerFormat = "JWT",
                 In = ParameterLocation.Header,
                 Description = "Enter 'Bearer' followed by your token",
             });
             
             options.AddSecurityRequirement(new OpenApiSecurityRequirement
             {
                 {
                     new OpenApiSecurityScheme
                     {
                         Reference = new OpenApiReference
                         {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                         }
                     },
                     []
                 }
             });
             var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
             options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
         });
         
    }
}