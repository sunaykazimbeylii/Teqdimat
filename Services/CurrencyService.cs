using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TravelFinalProject.Interfaces;
using TravelFinalProject.Models;

public class CurrencyService : ICurrencyService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CurrencyService> _logger;
    private readonly string _apiUrl;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

    public CurrencyService(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        ILogger<CurrencyService> logger,
        IOptions<CurrencySettings> options)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _logger = logger;
        _apiUrl = options.Value.ApiUrl;
    }

    public async Task<decimal> ConvertAsync(decimal priceInUSD, string targetCurrency)
    {
        if (string.IsNullOrEmpty(targetCurrency) || targetCurrency.ToUpper() == "USD")
            return priceInUSD;

        var rates = await GetRatesAsync();

        if (rates != null && rates.TryGetValue(targetCurrency.ToUpper(), out var rate))
        {
            return Math.Round(priceInUSD * rate, 2);
        }

        _logger.LogWarning($"Currency rate not found for '{targetCurrency}'. Returning original price.");
        return priceInUSD;
    }

    private async Task<Dictionary<string, decimal>?> GetRatesAsync()
    {
        if (_cache.TryGetValue("CurrencyRates", out Dictionary<string, decimal> rates))
        {
            return rates;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync(_apiUrl);
            var data = JsonSerializer.Deserialize<ExchangeRateResponse>(response);

            if (data != null && data.rates != null)
            {
                rates = data.rates;
                _cache.Set("CurrencyRates", rates, CacheDuration);
                return rates;
            }
            else
            {
                _logger.LogWarning("Failed to deserialize currency rates or data is null.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching currency rates from API.");
        }

        return null;
    }
    public string GetSymbol(string currencyCode)
    {
        return currencyCode.ToUpper() switch
        {
            "USD" => "$",
            "AZN" => "₼",
            "EUR" => "€",
            "TRY" => "₺",
            "GBP" => "£",
            _ => ""
        };
    }

}
