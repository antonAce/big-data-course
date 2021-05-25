using Cassandra;
using Cassandra.Mapping;
using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using GameStore.Games.DTOs;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(GameStore.Games.StartUp.StartUp))]
namespace GameStore.Games.StartUp
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var hostName = Environment.GetEnvironmentVariable("ENV_HOST_NAME");
            var hostUsername = Environment.GetEnvironmentVariable("ENV_HOST_USERNAME");
            var hostPassword = Environment.GetEnvironmentVariable("ENV_HOST_PASSWORD");
            var hostContactPoint = Environment.GetEnvironmentVariable("ENV_CONTACT_POINT");
            var hostPort = Convert.ToInt32(Environment.GetEnvironmentVariable("ENV_HOST_PORT") ?? "0");
            
            var options = new SSLOptions(SslProtocols.Tls12, true, ValidateServerCertificate);
            options.SetHostNameResolver((ipAddress) => hostName);
            Cluster cluster = Cluster.Builder()
                .WithCredentials(hostUsername, hostPassword).WithPort(hostPort)
                .AddContactPoint(hostContactPoint).WithSSL(options).Build();
            
            builder.Services.AddScoped<ISession>(s => cluster.Connect("gamestore"));
            
            MappingConfiguration.Global.Define(
                new Map<Game>()
                    .TableName("games")
                    .PartitionKey(g => g.Name)
                    .Column(g => g.AppId, cm => cm.WithName("app_id"))
                    .Column(g => g.Name, cm => cm.WithName("name"))
                    .Column(g => g.Description, cm => cm.WithName("description"))
                    .Column(g => g.Origin, cm => cm.WithName("origin"))
                    .Column(g => g.Genres, cm => cm.WithName("genres"))
                    .Column(g => g.Developers, cm => cm.WithName("developers"))
                    .Column(g => g.ReleaseDate, cm => cm.WithName("release_date"))
                    .Column(g => g.PriceHistory, cm => cm.WithName("price_history")),
                new Map<GamePreview>()
                    .TableName("games")
                    .PartitionKey(g => g.Name)
                    .Column(g => g.AppId, cm => cm.WithName("app_id"))
                    .Column(g => g.Name, cm => cm.WithName("name"))
                    .Column(g => g.Description, cm => cm.WithName("description"))
                    .Column(g => g.Origin, cm => cm.WithName("origin"))
                    .Column(g => g.Genres, cm => cm.WithName("genres"))
                    .Column(g => g.Developers, cm => cm.WithName("developers"))
                    .Column(g => g.ReleaseDate, cm => cm.WithName("release_date")),
                new Map<User>()
                    .TableName("users")
                    .PartitionKey(g => g.Nickname)
                    .Column(g => g.Nickname, cm => cm.WithName("nickname"))
                    .Column(g => g.SteamProfileLink, cm => cm.WithName("steam_profile_link"))
                    .Column(g => g.Wishlist, cm => cm.WithName("wishlist"))
                    .Column(g => g.OwnedGames, cm => cm.WithName("owned_games"))
                    .Column(g => g.Friends, cm => cm.WithName("friends"))
            );
        }

        private static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }
    }
}