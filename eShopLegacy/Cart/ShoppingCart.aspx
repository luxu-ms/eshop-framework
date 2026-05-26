<%@ Page Title="Shopping Cart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ShoppingCart.aspx.cs" Inherits="eShopLegacy.Cart.ShoppingCartPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <h1 class="display-6 fw-bold mb-4">Shopping Cart</h1>

    <!-- Empty cart -->
    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="text-center py-5">
        <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" fill="currentColor" class="text-muted mb-3" viewBox="0 0 16 16">
            <path d="M0 1.5A.5.5 0 0 1 .5 1H2a.5.5 0 0 1 .485.379L2.89 3H14.5a.5.5 0 0 1 .491.592l-1.5 8A.5.5 0 0 1 13 12H4a.5.5 0 0 1-.491-.408L2.01 3.607 1.61 2H.5a.5.5 0 0 1-.5-.5z"/>
        </svg>
        <p class="text-muted fs-5">Your cart is empty.</p>
        <a href="~/Catalog/Default.aspx" runat="server" class="btn btn-primary">Continue Shopping</a>
    </asp:Panel>

    <!-- Cart table -->
    <asp:Panel ID="pnlCart" runat="server">
        <div class="table-responsive">
            <table class="table align-middle">
                <thead class="table-light">
                    <tr>
                        <th>Product</th>
                        <th class="text-center">Price</th>
                        <th class="text-center" style="width:140px">Quantity</th>
                        <th class="text-end">Total</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptCart" runat="server" OnItemCommand="rptCart_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <div class="d-flex align-items-center gap-3">
                                        <img src="<%# eShopLegacy.Components.CdnHelper.Url("Content/placeholder.png") %>"
                                             width="56" height="56" class="rounded" alt='<%# Eval("ProductName") %>' />
                                        <span class="fw-semibold"><%# Eval("ProductName") %></span>
                                    </div>
                                </td>
                                <td class="text-center">$<%# string.Format("{0:0.00}", Eval("UnitPrice")) %></td>
                                <td class="text-center">
                                    <div class="input-group input-group-sm justify-content-center" style="max-width:110px;margin:auto">
                                        <asp:LinkButton runat="server" CssClass="btn btn-outline-secondary"
                                            CommandName="Decrement" CommandArgument='<%# Eval("Id") %>'>-</asp:LinkButton>
                                        <span class="input-group-text"><%# Eval("Quantity") %></span>
                                        <asp:LinkButton runat="server" CssClass="btn btn-outline-secondary"
                                            CommandName="Increment" CommandArgument='<%# Eval("Id") %>'>+</asp:LinkButton>
                                    </div>
                                </td>
                                <td class="text-end fw-bold">$<%# string.Format("{0:0.00}", (decimal)Eval("UnitPrice") * (int)Eval("Quantity")) %></td>
                                <td class="text-end">
                                    <asp:LinkButton runat="server" CssClass="btn btn-sm btn-outline-danger"
                                        CommandName="Remove" CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick="return confirm('Remove this item?')">
                                        &times;
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>

        <!-- Summary -->
        <div class="row justify-content-end mt-3">
            <div class="col-md-4">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Order Summary</h5>
                        <div class="d-flex justify-content-between mb-2">
                            <span>Subtotal (<asp:Label ID="lblItemCount" runat="server" /> items)</span>
                            <strong>$<asp:Label ID="lblSubtotal" runat="server" /></strong>
                        </div>
                        <div class="d-flex justify-content-between mb-2 text-muted">
                            <span>Shipping</span>
                            <span>Free</span>
                        </div>
                        <hr />
                        <div class="d-flex justify-content-between fw-bold fs-5">
                            <span>Total</span>
                            <span class="text-success">$<asp:Label ID="lblTotal" runat="server" /></span>
                        </div>
                    </div>
                    <div class="card-footer">
                        <asp:Button ID="btnCheckout" runat="server" Text="Proceed to Checkout"
                            CssClass="btn btn-success w-100" OnClick="btnCheckout_Click" />
                        <a href="~/Catalog/Default.aspx" runat="server" class="btn btn-link w-100 text-center mt-1">Continue Shopping</a>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

</asp:Content>
