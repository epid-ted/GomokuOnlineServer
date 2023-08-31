using LoginServer.Configuration;
using LoginServer.Data;
using LoginServer.Repositories;
using LoginServer.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace LoginServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ============================ Added ============================
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySQL(builder.Configuration.GetConnectionString("AccountConnectionString"))
            //options.UseSqlServer(builder.Configuration.GetConnectionString("AccountConnectionString"))
            );
            //builder.Services.AddDbContextPool<AppDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("AccountConnectionString"))
            //);

            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
                builder.Configuration.GetConnectionString("SessionConnectionString")
            );
            builder.Services.AddScoped(s => redis.GetDatabase());

            builder.Services.AddScoped<IAccountRepository, AccountRepositoryEFCore>();
            builder.Services.AddScoped<ISessionRepository, SessionRepositoryRedis>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<SessionService>();

            ServerConfig.LoginServer = builder.Configuration.GetValue<string>("ServerInfo:LoginServer");
            ServerConfig.MatchServer = builder.Configuration.GetValue<string>("ServerInfo:MatchServer");
            ServerConfig.GameServer = builder.Configuration.GetValue<string>("ServerInfo:GameServer");
            // ============================ Added ============================

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}