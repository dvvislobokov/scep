using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using DNMDM.Domain.Abstractions;
using DNMDM.Domain.Services;
using Microsoft.AspNetCore.Diagnostics;

namespace DNMDM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options => 
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSingleton<ICertificatesService, CertificateService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (app.Environment.IsProduction())
            {
                app.UseExceptionHandler(x =>
                {
                    x.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;


                        context.Response.ContentType = "application/json";

                        var exceptionHandlerPathFeature =
                            context.Features.Get<IExceptionHandlerPathFeature>();

                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            error = exceptionHandlerPathFeature.Error.Message
                        }));
                    });
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}