using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using WFTest.Logic;
using WFTest.Services;

namespace WFTest.Checkout
{
    public partial class CheckoutStart : System.Web.UI.Page
    {
        public CheckoutStart() => AsyncMode = true;

        protected async void Page_Load(object sender, EventArgs e)
        {
            string retMsg = string.Empty;

            if (Session["payment_amt"] != null)
            {
                string amount = Session["payment_amt"].ToString();

                try
                {
                    PayPalService payPalService = new PayPalService();

                    string approvalUrl = await payPalService.StartCheckoutAsync(amount);

                    Response.Redirect(approvalUrl);
                }
                catch (Exception ex)
                {
                    retMsg = HttpUtility.UrlEncode(ex.Message);
                    Response.Redirect("CheckoutError.aspx?ErrorMsg=" + retMsg);
                }
            }
            else
            {
                Response.Redirect("CheckoutError.aspx?ErrorCode=AmtMissing");
            }
        }
    }
}