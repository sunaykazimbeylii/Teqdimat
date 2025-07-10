namespace TravelFinalProject.Models
{
    public class ExchangeRateResponse
    {
        public string base_code { get; set; }
        public Dictionary<string, decimal> rates { get; set; }
    }
}
