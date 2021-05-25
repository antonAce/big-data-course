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
using CSession = Cassandra.ISession;

namespace GameStore.Games.FetchGames
{
    public class GetGameById
    {
        private readonly CSession _session;
        private readonly ILogger<GetGameById> _logger;

        public GetGameById(ILogger<GetGameById> logger, CSession session)
        {
            _logger = logger;
            _session = session;
            session.UserDefinedTypes.Define(
                UdtMap.For<PriceHistory>("price_record")
                    .Map(p => p.DatePrice, "date_price")
                    .Map(p => p.InitialPrice, "initial_price")
                    .Map(p => p.FinalPrice, "final_price")
                    .Map(p => p.DiscountOnPrice, "discount_on_price"));
        }
        
        [FunctionName("GetGameById")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"Triggered game details function with query params {{ {string.Join(", ", QueryConverter.StringifyQueryCollection(req.Query))} }}");
            string gameIdQuery =  req.Query["id"];
            int gameId = 0;
            
            if (!int.TryParse(gameIdQuery, out gameId))
                return new BadRequestErrorMessageResult("Game ID is not specified in query parameters");
            
            if (gameId < 0)
                return new BadRequestErrorMessageResult("Game ID value is out of range");

            const string query = @"
                SELECT app_id, name, description,
                       origin, genres, developers,
                       release_date, price_history
                FROM gamestore.games WHERE app_id = ?
                ALLOW FILTERING
            ";
            var mapper = new Mapper(_session);
            var priceHistory = await mapper.FirstOrDefaultAsync<Game>(query, gameId);
            
            return new OkObjectResult(priceHistory);
        }
    }
}
