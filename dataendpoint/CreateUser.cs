using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Cassandra;
using Cassandra.Mapping;
using GameStore.Games.DTOs;
using GameStore.Games.Helpers;
using Newtonsoft.Json;
using CSession = Cassandra.ISession;

namespace GameStore.Games.FetchGames
{
    public class CreateUser
    {
        private readonly CSession _session;
        private readonly ILogger<CreateUser> _logger;

        public CreateUser(ILogger<CreateUser> logger, CSession session)
        {
            _logger = logger;
            _session = session;
        }
        
        [FunctionName("CreateUser")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
            HttpRequest req, ILogger log)
        {
            _logger.LogInformation($"Triggered create user function with query params {{ {string.Join(", ", QueryConverter.StringifyQueryCollection(req.Query))} }}");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var user = JsonConvert.DeserializeObject<UserHeader>(requestBody);

            var statement = await _session.PrepareAsync(@"
                INSERT INTO gamestore.users (nickname, steam_profile_link, wishlist, owned_games, friends)
                VALUES (:nickname, :steam_profile_link, :wishlist, :owned_games, :friends)
            ");

            await _session.ExecuteAsync(statement.Bind(new
            {
                nickname = user.Nickname,
                steam_profile_link = user.SteamProfileLink,
                wishlist = new string[0],
                owned_games = new string[0],
                friends = new string[0]
            }));

            return new OkObjectResult(new User
            {
                Nickname = user.Nickname,
                SteamProfileLink = user.SteamProfileLink,
                Wishlist = new string[0],
                OwnedGames = new string[0],
                Friends = new string[0]
            });
        }
    }
}