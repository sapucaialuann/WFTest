using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WFTest.Configuration
{
    public class PayPalConfig
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _payPalApiBaseUrl;

        public PayPalConfig()
        {
            _clientId = Environment.GetEnvironmentVariable("PAYPAL_CLIENT_ID");
            _clientSecret = Environment.GetEnvironmentVariable("PAYPAL_CLIENT_SECRET");
            var useSandbox = System.Configuration.ConfigurationManager.AppSettings["UseSandbox"];
            _payPalApiBaseUrl = bool.Parse(useSandbox) ? "https://api.sandbox.paypal.com" : "https://api.paypal.com";

            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_clientSecret))
            {
                throw new Exception("PayPal credentials are not set in the environment variables.");
            }
        }
        public async Task<string> GetAccessTokenAsync(HttpClient _httpClient)
        {
            var basicAuthValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", basicAuthValue);

            var content = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            HttpResponseMessage response = await _httpClient.PostAsync($"{_payPalApiBaseUrl}/v1/oauth2/token", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to authenticate with PayPal");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<PayPalTokenResponse>(jsonResponse);

            return tokenResponse.access_token;
        }

        // Adapted GetAPIContext method using GetAccessTokenAsync
        public async Task<PayPalApiContext> GetAPIContext(HttpClient _httpClient)
        {
            try
            {
                // Get the access token by calling GetAccessTokenAsync
                string accessToken = await GetAccessTokenAsync(_httpClient);

                // Return a new PayPalApiContext object initialized with the token
                return new PayPalApiContext(accessToken);
            }
            catch (Exception ex)
            {
                // Handle and log the exception accordingly
                throw new Exception("Unable to authenticate with PayPal: " + ex.Message);
            }
        }
    }

    // The PayPalApiContext class to hold the token
    public class PayPalApiContext
    {
        public string AccessToken { get; }

        public PayPalApiContext(string accessToken)
        {
            AccessToken = accessToken;
        }
    }

    // Token response class for deserialization
    public class PayPalTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}