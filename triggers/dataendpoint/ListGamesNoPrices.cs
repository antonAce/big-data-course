using System;
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
using CSession = Cassandra.ISession;

namespace GameStore.Games.FetchGames
{
    public class ListGamesNoPrices
    {
        private readonly CSession _session;
        private readonly ILogger<ListGamesNoPrices> _logger;

        public ListGamesNoPrices(ILogger<ListGamesNoPrices> logger, CSession session)
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
        
        [FunctionName("ListGamesNoPrices")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req, ILogger log)
        {
            _logger.LogInformation($"Triggered game listing function with query params {{ {string.Join(", ", QueryConverter.StringifyQueryCollection(req.Query))} }}");

            var itemsLimit = Convert.ToInt32(Environment.GetEnvironmentVariable("ENV_ITEMS_LIMIT") ?? "50");
            string pagingState =  req.Query["pagingState"];
            
            var statement = Cql.New(@"SELECT app_id, name, description, origin, genres,
                                        developers, release_date
                                        FROM gamestore.games").WithOptions(opt =>
                opt.SetPageSize(itemsLimit)
                    .SetPagingState(!string.IsNullOrEmpty(pagingState) ?
                        Convert.FromBase64String(pagingState) : null));

            var mapper = new Mapper(_session);
            var page = await mapper.FetchPageAsync<GamePreview>(statement);
            
            return new OkObjectResult(new { Page = page.AsEnumerable(), PagingState = page.PagingState });
        }
    }
}