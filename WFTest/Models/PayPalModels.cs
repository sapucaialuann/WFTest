using System;
using System.Collections.Generic;

namespace WFTest
{
    public class PayPalOrderRequest
    {
        public string intent { get; set; }
        public PayPalPurchaseUnit[] purchase_units { get; set; }
        public PayPalApplicationContext application_context { get; set; }
    }

    public class PayPalApplicationContext
    {
        public string return_url { get; set; }
        public string cancel_url { get; set; }
    }

    public class PayPalPurchaseUnit
    {
        public string reference_id { get; set; }
        public PayPalAmount amount { get; set; }
    }

    public class PayPalAmount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class PayPalCreateOrderResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public List<PayPalLink> links { get; set; }
    }

    public class PayPalLink
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }

    public class PayPalApprovalUrl
    {
        public string ApprovalUrl { get; set; }
        public string OrderId { get; set; }
    }

    public class PayPalTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    public class PayPalCaptureResponse
    {
        public string id { get; set; }
        public string status { get; set; }
    }

    public class PurchaseUnit
    {
        public string ReferenceId { get; set; }
        public AmountWithCurrency Amount { get; set; }
    }

    public class AmountWithCurrency
    {
        public string CurrencyCode { get; set; }
        public decimal Value { get; set; }
    }

    public class Payer
    {
        public string EmailAddress { get; set; }
        public string PayerId { get; set; }
        public Name Name { get; set; }
    }

    public class Name
    {
        public string GivenName { get; set; }
        public string Surname { get; set; }
    }

    public class PaymentSource
    {
        public PayPalPaymentSource PayPal { get; set; }
    }

    public class PayPalPaymentSource
    {
        public Payer Payer { get; set; }
        public string AccountId { get; set; }
    }
    public class PayPalDetailedOrder
    {
        public PayPalCreateOrderResponse OrderCreationData { get; set; }
        public string Intent { get; set; }
        public PaymentSource PaymentSource { get; set; }
        public List<PurchaseUnit> PurchaseUnits { get; set; }
        public Payer Payer { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}