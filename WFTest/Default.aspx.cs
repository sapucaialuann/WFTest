using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WFTest.Configuration;

namespace WFTest
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    SetAPIToken();
            //}
        }

        //private async Task SetAPIToken()
        //{
        //    try
        //    {
        //        await PayPalConfig.GetAPIContext();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("falha no engano: ", ex);
        //    }
        //}
    }
}