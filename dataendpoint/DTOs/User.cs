using System.Collections.Generic;

namespace GameStore.Games.DTOs
{
    public class User
    {
        public string Nickname { get; set; }
        public string SteamProfileLink { get; set; }
        public IEnumerable<string> Wishlist { get; set; }
        public IEnumerable<string> OwnedGames { get; set; }
        public IEnumerable<string> Friends { get; set; }
    }
}