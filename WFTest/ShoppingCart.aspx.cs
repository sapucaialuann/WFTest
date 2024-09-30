using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WFTest.Logic;
using WFTest.Models;

namespace WFTest
{
    public partial class ShoppingCart : System.Web.UI.Page
    {
        private ShoppingCartActions _shoppingCartActions;

        protected void Page_Load(object sender, EventArgs e)
        {
            _shoppingCartActions = new ShoppingCartActions();

            if (!IsPostBack)
            {
                BindCartData();
            }
        }
        private void BindCartData()
        {
            decimal cartTotal = _shoppingCartActions.GetTotal();
            if (cartTotal > 0)
            {
                lblTotal.Text = $"{cartTotal:c}";
                UpdateBtn.Visible = true;
                CheckoutImageBtn.Visible = true;
                ShoppingCartTitle.InnerText = "Your Shopping Cart";
            }
            else
            {
                ClearCartDisplay();
            }
        }
        private void ClearCartDisplay()
        {
            lblTotal.Text = string.Empty;
            LabelTotalText.Text = string.Empty;
            ShoppingCartTitle.InnerText = "Shopping Cart is Empty";
            UpdateBtn.Visible = false;
            CheckoutImageBtn.Visible = false;
        }
        public List<CartItem> GetShoppingCartItems()
        {
            return _shoppingCartActions.GetCartItems();
        }
        public List<CartItem> UpdateCartItems()
        {
            string cartId = _shoppingCartActions.GetCartId();
            var cartUpdates = GetCartUpdates();

            _shoppingCartActions.UpdateShoppingCartDatabase(cartId, cartUpdates);
            CartList.DataBind();

            lblTotal.Text = $"{_shoppingCartActions.GetTotal():c}";
            return _shoppingCartActions.GetCartItems();
        }
        private ShoppingCartActions.ShoppingCartUpdates[] GetCartUpdates()
        {
            var cartUpdates = new ShoppingCartActions.ShoppingCartUpdates[CartList.Rows.Count];

            for (int i = 0; i < CartList.Rows.Count; i++)
            {
                GridViewRow row = CartList.Rows[i];
                cartUpdates[i] = new ShoppingCartActions.ShoppingCartUpdates
                {
                    ProductId = GetProductId(row),
                    RemoveItem = IsItemToRemove(row),
                    PurchaseQuantity = GetPurchaseQuantity(row)
                };
            }

            return cartUpdates;
        }
        private int GetProductId(GridViewRow row)
        {
            IOrderedDictionary rowValues = GetValues(row);
            return Convert.ToInt32(rowValues["ProductId"]);
        }

        private bool IsItemToRemove(GridViewRow row)
        {
            var removeCheckBox = (CheckBox)row.FindControl("Remove");
            return removeCheckBox != null && removeCheckBox.Checked;
        }

        private short GetPurchaseQuantity(GridViewRow row)
        {
            var quantityTextBox = (TextBox)row.FindControl("PurchaseQuantity");
            return quantityTextBox != null ? Convert.ToInt16(quantityTextBox.Text) : (short)0;
        }

        public static IOrderedDictionary GetValues(GridViewRow row)
        {
            IOrderedDictionary values = new OrderedDictionary();
            foreach (DataControlFieldCell cell in row.Cells)
            {
                if (cell.Visible)
                {
                    cell.ContainingField.ExtractValuesFromCell(values, cell, row.RowState, true);
                }
            }

            return values;
        }
        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            UpdateCartItems();
        }
        protected void CheckoutBtn_Click(object sender, ImageClickEventArgs e)
        {
            Session["payment_amt"] = _shoppingCartActions.GetTotal();
            Response.Redirect("Checkout/CheckoutStart.aspx");
        }
    }

}