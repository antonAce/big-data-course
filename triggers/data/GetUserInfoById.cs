using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
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
    public class GetUserInfoById
    {
        private readonly CSession _session;
        private readonly ILogger<GetUserInfoById> _logger;

        public GetUserInfoById(ILogger<GetUserInfoById> logger, CSession session)
        {
            _logger = logger;
            _session = session;
        }
        
        [FunctionName("GetUserInfoById")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req, ILogger log)
        {
            _logger.LogInformation($"Triggered get user info by id function with query params {{ {string.Join(", ", QueryConverter.StringifyQueryCollection(req.Query))} }}");
            string nickname = req.Query["nickname"];
            
            if (string.IsNullOrEmpty(nickname))
                return new BadRequestErrorMessageResult("Field nickname is missing");

            const string query = @"SELECT nickname, steam_profile_link, wishlist, owned_games, friends
                                   FROM gamestore.users WHERE nickname = ? ALLOW FILTERING";
            var mapper = new Mapper(_session);
            var user = await mapper.FirstOrDefaultAsync<User>(query, nickname);
            
            return new OkObjectResult(user);
        }
    }
}