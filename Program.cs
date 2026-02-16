using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using pigeon_api.Contexts;
using pigeon_api.Middlewares;
using pigeon_api.Services;
using pigeon_api.Messaging.Nats;
using pigeon_api.Messaging.Consumers;
using pigeon_api.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using pigeon_api.SignalR;

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
            builder.Services.AddHostedService<FriendshipEventsConsumer>();
            builder.Services.AddHostedService<MessageEventsConsumer>();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

            // APPLICATION SERVICES
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<FriendshipService>();
            builder.Services.AddScoped<MessageService>();
            builder.Services.AddScoped<NotificationService>();

            // FRAMEWORK
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("SignalRCors", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("SignalRCors");

            app.UseMiddleware<UserAuthMiddleware>();
            // app.UseMiddleware<ApiKeyAuthMiddleware>();
            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.MapControllers();

            app.MapHub<NotificationsHub>("/hubs/notifications");

            app.Run();
        }
    }
}
