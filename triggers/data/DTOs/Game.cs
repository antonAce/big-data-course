using System;
using System.Collections.Generic;

namespace GameStore.Games.DTOs
{
    public class Game
    {
        public int AppId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Origin { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public IEnumerable<string> Developers { get; set; }
        public DateTime ReleaseDate { get; set; }
        public PriceHistory[] PriceHistory { get; set; }
    }
}