using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace GameStore.Games.Helpers
{
    public class QueryConverter
    {
        public static string[] StringifyQueryCollection(IQueryCollection query) =>
            new List<string>(query.Select(q => $"{q.Key}: {q.Value.ToString()}")).ToArray();
    }
}