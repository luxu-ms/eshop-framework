<%@ Page Title="Order Confirmed" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderComplete.aspx.cs" Inherits="eShopLegacy.Checkout.OrderCompletePage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="text-center py-5">
        <div class="display-1 text-success mb-3">&#10003;</div>
        <h1 class="display-5 fw-bold">Thank you for your order!</h1>
        <p class="lead text-muted">Your order #<asp:Label ID="lblOrderId" runat="server" CssClass="fw-bold" /> has been placed successfully.</p>
        <p class="text-muted">Placed on <asp:Label ID="lblOrderDate" runat="server" /></p>
    </div>

    <div class="row justify-content-center">
        <div class="col-md-8">
            <div class="card">
                <div class="card-header fw-semibold">Order Items</div>
                <div class="card-body p-0">
                    <table class="table mb-0">
                        <thead class="table-light">
                            <tr>
                                <th>Product</th>
                                <th class="text-center">Qty</th>
                                <th class="text-end">Unit Price</th>
                                <th class="text-end">Total</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptItems" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("ProductName") %></td>
                                        <td class="text-center"><%# Eval("Units") %></td>
                                        <td class="text-end">$<%# string.Format("{0:0.00}", Eval("UnitPrice")) %></td>
                                        <td class="text-end">$<%# string.Format("{0:0.00}", (decimal)Eval("UnitPrice") * (int)Eval("Units")) %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                        <tfoot>
                            <tr class="fw-bold">
                                <td colspan="3" class="text-end">Order Total</td>
                                <td class="text-end text-success">$<asp:Label ID="lblTotal" runat="server" /></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </div>

            <div class="d-flex gap-3 justify-content-center mt-4">
                <a href="~/Catalog/Default.aspx" runat="server" class="btn btn-primary">Continue Shopping</a>
            </div>
        </div>
    </div>

</asp:Content>
