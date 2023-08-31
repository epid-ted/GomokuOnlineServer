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
            RedisConfig.Redis = ConnectionMultiplexer.Connect(
                builder.Configuration.GetConnectionString("SessionConnectionString")
            );

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

            // Run Socket Server
            Task.Run(() => { StartSocketServer(); });

            // Run Web Server
            app.Run();
        }
    }
}