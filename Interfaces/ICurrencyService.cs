namespace TravelFinalProject.Interfaces
{
    public interface ICurrencyService
    {
        Task<decimal> ConvertAsync(decimal priceInUSD, string targetCurrency);
        string GetSymbol(string currencyCode);
    }
}
