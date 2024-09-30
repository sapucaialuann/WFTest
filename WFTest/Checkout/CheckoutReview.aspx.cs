using System;
using System.Web;
using WFTest.Logic;
using WFTest.Models;
using WFTest.Services;

namespace WFTest.Checkout
{
    public partial class CheckoutReview : System.Web.UI.Page
    {
        private readonly PayPalService _payPalService;
        public CheckoutReview()
        {
            _payPalService = new PayPalService();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ProcessCheckout();
            }
        }
        private async void ProcessCheckout()
        {
            try
            {
                var orderDetails = await _payPalService.GetOrderDetailsAsync();

                if (orderDetails == null)
                {
                    RedirectToErrorPage("Failed to retrieve PayPal order details.");
                    return;
                }

                var cartActions = new ShoppingCartActions();
                var total = cartActions.GetTotal();

                // Validate the amount
                if (!ValidateOrderAmount(total))
                {
                    RedirectToErrorPage("Amount total mismatch.");
                    return;
                }
                var order = MapPayPalOrderResponseIntoDBObject(orderDetails);
                SaveOrderToDatabase(order);
                BindOrderDetailsToPage(order);
            }
            catch (Exception ex)
            {
                RedirectToErrorPage($"Error during checkout: {ex.Message}");
            }
        }
        private void RedirectToErrorPage(string errorMessage)
        {
            Response.Redirect($"CheckoutError.aspx?Desc={HttpUtility.UrlEncode(errorMessage)}");
        }
        private bool ValidateOrderAmount(decimal orderTotal)
        {
            decimal sessionPaymentAmount = Convert.ToDecimal(Session["payment_amt"]?.ToString() ?? "0");
            return sessionPaymentAmount == orderTotal;
        }
        private void SaveOrderToDatabase(DBOrder order)
        {
            using (var db = new ProductContext())
            {
                db.Order.Add(order);
                db.SaveChanges();
            }
        }
        private void BindOrderDetailsToPage(DBOrder order)
        {
            using (var cartActions = new ShoppingCartActions())
            {
                var orderItems = cartActions.GetCartItems();
                OrderItemList.DataSource = orderItems;
                OrderItemList.DataBind();
            }
        }
        private DBOrder MapPayPalOrderResponseIntoDBObject(dynamic payPalOrder)
        {
            var order = new DBOrder
            {
                OrderId = payPalOrder.id,
                OrderDate = payPalOrder.create_time,
                FirstName = payPalOrder.payer.name.given_name,
                LastName = payPalOrder.payer.name.surname,
                Email = payPalOrder.payer.email_address,
                Total = payPalOrder.purchase_units[0].amount.value,
            };
            return order;
        }
        protected void CheckoutConfirm_Click(object sender, EventArgs e)
        {
            Session["userCheckoutCompleted"] = "true";
            Response.Redirect("~/Checkout/CheckoutComplete.aspx");
        }
    }
}
