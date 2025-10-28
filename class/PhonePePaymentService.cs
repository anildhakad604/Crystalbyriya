using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class PhonePePaymentService
{
    private readonly HttpClient _httpClient;
    private readonly string _merchantId;
    private readonly string _merchantKey;
    private readonly string _baseUrl;

    public PhonePePaymentService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _merchantId = configuration["PhonePe:MerchantId"];
        _merchantKey = configuration["PhonePe:MerchantKey"];
        _baseUrl = configuration["PhonePe:BaseUrl"];
    }

    public async Task<string> InitiatePaymentAsync(string orderId, decimal amount, string redirectUrl)
    {
        var payload = new
        {
            merchantId = _merchantId,
            orderId = orderId,
            amount = amount * 100, // convert to paise
            redirectUrl = redirectUrl
            // other required fields
        };

        var jsonPayload = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/pg/v1/pay", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }

    public async Task<string> VerifyPaymentAsync(string orderId)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/v3/order/{orderId}/status");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        return responseString;
    }
}
