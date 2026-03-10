<%@ Page Title="My Orders" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderHistory.aspx.cs" Inherits="eShopLegacy.Checkout.OrderHistoryPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <h1 class="display-6 fw-bold mb-4">My Orders</h1>

    <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="text-center py-5">
        <p class="text-muted fs-5">You haven't placed any orders yet.</p>
        <a href="~/Catalog/Default.aspx" runat="server" class="btn btn-primary">Browse Catalog</a>
    </asp:Panel>

    <asp:Repeater ID="rptOrders" runat="server">
        <ItemTemplate>
            <div class="card mb-4 shadow-sm">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <div>
                        <strong>Order #<%# Eval("Id") %></strong>
                        <span class="text-muted ms-3 small"><%# ((DateTime)Eval("OrderDate")).ToString("MMM d, yyyy h:mm tt") %></span>
                    </div>
                    <span class='<%# "badge " + GetStatusBadge(Eval("Status")) %>'><%# Eval("Status") %></span>
                </div>
                <div class="card-body p-0">
                    <table class="table table-borderless mb-0">
                        <thead class="table-light">
                            <tr>
                                <th class="ps-3">Product</th>
                                <th class="text-center">Qty</th>
                                <th class="text-end">Unit Price</th>
                                <th class="text-end pe-3">Subtotal</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptItems" runat="server" DataSource='<%# Eval("OrderItems") %>'>
                                <ItemTemplate>
                                    <tr>
                                        <td class="ps-3"><%# Eval("ProductName") %></td>
                                        <td class="text-center"><%# Eval("Units") %></td>
                                        <td class="text-end">$<%# string.Format("{0:0.00}", Eval("UnitPrice")) %></td>
                                        <td class="text-end pe-3">$<%# string.Format("{0:0.00}", (decimal)Eval("UnitPrice") * (int)Eval("Units") - (decimal)Eval("Discount")) %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                        <tfoot class="table-light">
                            <tr>
                                <td colspan="3" class="text-end fw-bold ps-3">Total</td>
                                <td class="text-end fw-bold text-success pe-3">$<%# string.Format("{0:0.00}", Eval("Total")) %></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
                <div class="card-footer text-muted small">
                    Ship to: <%# Eval("Street") %>, <%# Eval("City") %>, <%# Eval("State") %> <%# Eval("ZipCode") %>, <%# Eval("Country") %>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

</asp:Content>
