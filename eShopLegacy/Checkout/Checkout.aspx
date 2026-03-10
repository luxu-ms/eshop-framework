<%@ Page Title="Checkout" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="eShopLegacy.Checkout.CheckoutPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <h1 class="display-6 fw-bold mb-4">Checkout</h1>

    <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
        <asp:Literal ID="litError" runat="server" />
    </asp:Panel>

    <div class="row g-4">
        <!-- Left: Forms -->
        <div class="col-lg-8">

            <!-- Shipping address -->
            <div class="card mb-4">
                <div class="card-header fw-semibold">Shipping Address</div>
                <div class="card-body">
                    <div class="row g-3">
                        <div class="col-12">
                            <label class="form-label">Street address</label>
                            <asp:TextBox ID="txtStreet" runat="server" CssClass="form-control" placeholder="123 Main St" />
                            <asp:RequiredFieldValidator ControlToValidate="txtStreet" runat="server"
                                ErrorMessage="Street is required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-md-5">
                            <label class="form-label">City</label>
                            <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ControlToValidate="txtCity" runat="server"
                                ErrorMessage="City is required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-md-4">
                            <label class="form-label">State / Province</label>
                            <asp:TextBox ID="txtState" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">ZIP code</label>
                            <asp:TextBox ID="txtZip" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ControlToValidate="txtZip" runat="server"
                                ErrorMessage="ZIP required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-md-5">
                            <label class="form-label">Country</label>
                            <asp:TextBox ID="txtCountry" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ControlToValidate="txtCountry" runat="server"
                                ErrorMessage="Country is required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Payment -->
            <div class="card mb-4">
                <div class="card-header fw-semibold">Payment Information</div>
                <div class="card-body">
                    <div class="row g-3">
                        <div class="col-12">
                            <label class="form-label">Card number</label>
                            <asp:TextBox ID="txtCardNumber" runat="server" CssClass="form-control" placeholder="xxxx xxxx xxxx xxxx" MaxLength="19" />
                            <asp:RequiredFieldValidator ControlToValidate="txtCardNumber" runat="server"
                                ErrorMessage="Card number is required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Card holder name</label>
                            <asp:TextBox ID="txtCardHolder" runat="server" CssClass="form-control" placeholder="Full name on card" />
                            <asp:RequiredFieldValidator ControlToValidate="txtCardHolder" runat="server"
                                ErrorMessage="Card holder name is required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Expiry (MM/YY)</label>
                            <asp:TextBox ID="txtExpiry" runat="server" CssClass="form-control" placeholder="MM/YY" MaxLength="5" />
                            <asp:RequiredFieldValidator ControlToValidate="txtExpiry" runat="server"
                                ErrorMessage="Expiry required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">CVV</label>
                            <asp:TextBox ID="txtCVV" runat="server" CssClass="form-control" MaxLength="4" placeholder="123" TextMode="Password" />
                            <asp:RequiredFieldValidator ControlToValidate="txtCVV" runat="server"
                                ErrorMessage="CVV required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Right: Order summary -->
        <div class="col-lg-4">
            <div class="card sticky-top" style="top:80px">
                <div class="card-header fw-semibold">Order Summary</div>
                <div class="card-body p-0">
                    <ul class="list-group list-group-flush">
                        <asp:Repeater ID="rptSummary" runat="server">
                            <ItemTemplate>
                                <li class="list-group-item d-flex justify-content-between">
                                    <span><%# Eval("ProductName") %> &times; <%# Eval("Quantity") %></span>
                                    <strong>$<%# string.Format("{0:0.00}", (decimal)Eval("UnitPrice") * (int)Eval("Quantity")) %></strong>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                        <li class="list-group-item d-flex justify-content-between fw-bold">
                            <span>Total</span>
                            <span class="text-success">$<asp:Label ID="lblTotal" runat="server" /></span>
                        </li>
                    </ul>
                </div>
                <div class="card-footer">
                    <asp:Button ID="btnPlaceOrder" runat="server" Text="Place Order"
                        CssClass="btn btn-success w-100 btn-lg" OnClick="btnPlaceOrder_Click" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>
