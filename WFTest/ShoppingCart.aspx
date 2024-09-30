<%@ Page Title="Shopping Cart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ShoppingCart.aspx.cs" Inherits="WFTest.ShoppingCart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="ShoppingCartTitle" runat="server" class=""><h1>Shopping Cart</h1></div>
    <asp:GridView ID="CartList" runat="server" AutoGenerateColumns="False" ShowFooter="true" GridLines="Vertical" CellPadding="4" ItemType="WFTest.Models.CartItem" SelectMethod="GetShoppingCartItems" CssClass="table table-striped table-bordered">
        <Columns>
            <asp:BoundField DataField="ProductId" HeaderText="ÏD" />
            <asp:BoundField DataField="Product.ProductName" HeaderText="Name" SortExpression="Product.ProductName"/>
            <asp:BoundField DataField="Product.ProductPrice" HeaderText="Price" />
            <asp:TemplateField HeaderText="Quantity">
                <ItemTemplate>
                    <asp:TextBox ID="PurchaseQuantity" runat="server" Text="<%#: Item.Quantity %>"></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Item Total">
                <ItemTemplate>
                    <%#: String.Format("{0:c}", ((Convert.ToDouble(Item.Quantity)) * Convert.ToDouble(Item.Product.ProductPrice))) %>
                </ItemTemplate>
            </asp:TemplateField> 
            <asp:TemplateField HeaderText="Remove Item">            
                <ItemTemplate>
                    <asp:CheckBox id="Remove" runat="server"></asp:CheckBox>
                </ItemTemplate>        
            </asp:TemplateField>   
        </Columns>
    </asp:GridView>
    <div>
        <p></p>
        <strong>
            <asp:Label ID="LabelTotalText" runat="server" Text="Order Total: "></asp:Label>
            <asp:Label ID="lblTotal" runat="server" EnableViewState="false"></asp:Label>
        </strong> 
    </div>
    <br />
    <table>
        <tr>
            <td>
                <asp:Button ID="UpdateBtn" runat="server" Text="Update" OnClick="UpdateBtn_Click"/>
            </td>
            <td>
                <asp:ImageButton ID="CheckoutImageBtn" runat="server"
                                 ImageUrl="https://www.paypalobjects.com/webstatic/en_US/i/buttons/checkout-logo-medium.png"
                                 Width="145" AlternateText="Check out with PayPal"
                                 OnClick="CheckoutBtn_Click" 
                                 BackColor="Transparent" BorderWidth="0" />
                <asp:Label ID="ErrorLabel" runat="server" ForeColor="Red" />
            </td>
        </tr>
    </table>
</asp:Content>
