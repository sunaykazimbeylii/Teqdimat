namespace TravelFinalProject.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Symbol { get; set; }
        public decimal RateToUSD { get; set; }
    }
}
