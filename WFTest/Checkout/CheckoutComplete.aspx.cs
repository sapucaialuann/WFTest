using System;
using WFTest.Logic;
using System.Web;
using WFTest.Services;

namespace WFTest.Checkout
{
    public partial class CheckoutComplete : System.Web.UI.Page
    {
        private readonly PayPalService _payPalService;
        public CheckoutComplete()
        {
            _payPalService = new PayPalService();
        }
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userCheckoutCompleted"]?.ToString() != "true")
                {
                    Session["userCheckoutCompleted"] = string.Empty;
                    Response.Redirect("CheckoutError.aspx?Desc=Unvalidated%20Checkout.");
                    return;
                }
                string orderId = Session["orderId"]?.ToString();

                if (string.IsNullOrEmpty(orderId))
                {
                    Response.Redirect("CheckoutError.aspx?Desc=Missing%20Payment%20Details.");
                    return;
                }
                try
                {
                    var captureResponse = await _payPalService.CapturePaymentAsync(orderId);
                    TransactionId.Text = captureResponse;
                    EmptyShoppingCart();
                    Session["orderId"] = string.Empty;
                    Session["currentOrderId"] = string.Empty;
                }
                catch (Exception ex)
                {
                    Response.Redirect("CheckoutError.aspx?Desc=" + HttpUtility.UrlEncode(ex.Message));
                }
            }
        }
        private void EmptyShoppingCart()
        {
            using (var usersShoppingCart = new ShoppingCartActions())
            {
                usersShoppingCart.EmptyCart();
            }
        }
        protected void Continue_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default.aspx");
        }
    }
}
