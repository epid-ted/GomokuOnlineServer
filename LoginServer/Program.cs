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

            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
                builder.Configuration.GetConnectionString("SessionConnectionString")
            );
            builder.Services.AddScoped(s => redis.GetDatabase());

            IDatabase database = redis.GetDatabase();
            string serverSessionId = Guid.NewGuid().ToString();
            database.StringSet("serversession:LoginServer", serverSessionId);
            ServerConfig.ServerSessionId = serverSessionId;

            builder.Services.AddScoped<IAccountRepository, AccountRepositoryEFCore>();
            builder.Services.AddScoped<ISessionRepository, SessionRepositoryRedis>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<SessionService>();

            ServerConfig.LoginServerPrivateAddress = builder.Configuration.GetValue<string>("ServerInfo:LoginServerPrivateAddress");
            ServerConfig.MatchServerPrivateAddress = builder.Configuration.GetValue<string>("ServerInfo:MatchServerPrivateAddress");
            ServerConfig.GameServerPrivateAddress = builder.Configuration.GetValue<string>("ServerInfo:GameServerPrivateAddress");
            ServerConfig.LoginServerPublicAddress = builder.Configuration.GetValue<string>("ServerInfo:LoginServerPublicAddress");
            ServerConfig.MatchServerPublicAddress = builder.Configuration.GetValue<string>("ServerInfo:MatchServerPublicAddress");
            ServerConfig.GameServerPublicAddress = builder.Configuration.GetValue<string>("ServerInfo:GameServerPublicAddress");
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