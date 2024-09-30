using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.UI;
using System.Web.UI.WebControls;
using WFTest.Models;

namespace WFTest
{
    public partial class ProductDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public IQueryable<Product> GetProduct([QueryString("productId")] int? productId)
        {
            var db = new WFTest.Models.ProductContext();
            IQueryable<Product> query = db.Products;
            if (productId.HasValue && productId > 0)
            {
                query = query.Where(p => p.ProductId == productId);
            }
            else
            {
                query = null;
            }
            return query;
        }
    }
}