using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using WFTest.Configuration;

namespace WFTest.Services
{
    public class PayPalService
    {
        private readonly string _payPalApiBaseUrl;
        private readonly HttpClient _httpClient;
        private readonly PayPalConfig _payPalConfig;
        public PayPalService()
        {
            var useSandbox = System.Configuration.ConfigurationManager.AppSettings["UseSandbox"];
            _payPalApiBaseUrl = bool.Parse(useSandbox) ? "https://api.sandbox.paypal.com" : "https://api.paypal.com";
            _httpClient = new HttpClient();
            _payPalConfig = new PayPalConfig();
        }
        public async Task<string> StartCheckoutAsync(string amount)
        {
            var orderResponse = await CreatePayPalOrder(amount);
            var approvalLink = orderResponse.links[1].href;
            if (approvalLink == null)
            {
                throw new Exception("Failed to get approval link from PayPal.");
            }
            HttpContext.Current.Session["orderId"] = orderResponse.id;
            return approvalLink;
        }
        private async Task<PayPalCreateOrderResponse> CreatePayPalOrder(string amount)
        {
            var accessToken = await _payPalConfig.GetAccessTokenAsync(_httpClient);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var baseUrl = $"{_payPalApiBaseUrl}/v2/checkout/orders";
            var orderRequestBody = new PayPalOrderRequest
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new PayPalPurchaseUnit
                    {
                        amount = new PayPalAmount
                        {
                            currency_code = "USD", 
                            value = amount
                        },
                    }
                },
                application_context = new PayPalApplicationContext()
                {
                    return_url = "https://localhost:44326/Checkout/CheckoutReview.aspx",
                    cancel_url = "https://localhost:44326/Checkout/CheckoutError.aspx"
                }
            };
            var request = new StringContent(JsonConvert.SerializeObject(orderRequestBody), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(baseUrl, request);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to create PayPal order.");
            }
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var orderResponse = JsonConvert.DeserializeObject<PayPalCreateOrderResponse>(jsonResponse);
            HttpContext.Current.Session["PayPalCreateOrderResponse"] = orderResponse;
            return orderResponse;
        }
        public async Task<dynamic> GetOrderDetailsAsync()
        {
            PayPalCreateOrderResponse orderResponse = (PayPalCreateOrderResponse)HttpContext.Current.Session["PayPalCreateOrderResponse"];
            var accessToken = await _payPalConfig.GetAccessTokenAsync(_httpClient);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _httpClient.GetAsync($"{orderResponse.links[0].href}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Something went wrong when trying to fetch the order details.");
            }
            var orderDetailsJson = await response.Content.ReadAsStringAsync();
            orderDetailsJson.Trim().TrimStart('{').TrimEnd('}');
            return JsonConvert.DeserializeObject<dynamic>(orderDetailsJson);
        }
        private async Task<dynamic> CaptureOrderAsync(string orderId)
        {
            var accessToken = await _payPalConfig.GetAccessTokenAsync(_httpClient);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"{_payPalApiBaseUrl}/v2/checkout/orders/{orderId}/capture", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to capture PayPal order.");
            }
            var orderDetailsJson = await response.Content.ReadAsStringAsync();
            orderDetailsJson.Trim().TrimStart('{').TrimEnd('}');
            return JsonConvert.DeserializeObject<dynamic>(orderDetailsJson);
        }
        public async Task<string> CapturePaymentAsync(string orderId)
        {
            var captureResponse = await CaptureOrderAsync(orderId);
            if (captureResponse.status == "COMPLETED")
            {
                return captureResponse.id;
            }
            throw new Exception("Payment capture failed.");
        }
    }
}
