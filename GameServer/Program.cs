using Common.Authorization;
using GameServer.Configuration;
using GameServer.Room;
using NetworkLibrary;
using Server.Session;
using StackExchange.Redis;
using System.Net;

namespace GameServer
{
    public class Program
    {
        static void StartSocketServer()
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = IPAddress.Parse("0.0.0.0");
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 5678);

            Listener listener = new Listener();
            listener.Start(endPoint, 100, () => { return SessionManager.Instance.Create(); });

            while (true)
            {
                RoomManager.Instance.Update();
                Thread.Sleep(100);
            }
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ============================ Added ============================
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
                builder.Configuration.GetConnectionString("SessionConnectionString")
            );
            builder.Services.AddScoped(s => redis.GetDatabase());

            RedisConfig.Redis = ConnectionMultiplexer.Connect(
                builder.Configuration.GetConnectionString("SessionConnectionString")
            );
            IDatabase database = RedisConfig.Redis.GetDatabase();
            string serverSessionId = Guid.NewGuid().ToString();
            database.StringSet("serversession:GameServer", serverSessionId);
            ServerConfig.ServerSessionId = serverSessionId;

            builder.Services.AddScoped<IAuthorizationRepository, AuthorizationRepositoryRedis>();
            builder.Services.AddScoped<AuthorizationService>();

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

            // Run Socket Server
            Task.Run(() => { StartSocketServer(); });

            // Run Web Server
            app.Run();
        }
    }
}