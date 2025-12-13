using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using pigeon_api.Contexts;
using pigeon_api.Middlewares;
using pigeon_api.Services;
using pigeon_api.Messaging.Nats;
using pigeon_api.Messaging.Consumers;

namespace pigeon_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>();
            }

            // DATABASE
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // NATS (INFRASTRUCTURE)
            builder.Services.AddSingleton<NatsConnection>();
            builder.Services.AddSingleton<NatsPublisher>();

            // NATS CONSUMERS (BACKGROUND WORKERS)
            builder.Services.AddHostedService<FriendshipCreatedConsumer>();

            // APPLICATION SERVICES
            builder.Services.AddScoped<FriendshipService>();

            // FRAMEWORK
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<AuthenticationMiddleware>();
            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.MapControllers();
            app.Run();
        }
    }
}
