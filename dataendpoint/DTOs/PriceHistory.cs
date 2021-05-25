using System;

namespace GameStore.Games.DTOs
{
    public class PriceHistory
    {
        public DateTime DatePrice { get; set; }
        public double InitialPrice { get; set; }
        public double FinalPrice { get; set; }
        public int DiscountOnPrice { get; set; }
    }
}