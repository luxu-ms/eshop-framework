<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="eShopLegacy.Account.LoginPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row justify-content-center">
        <div class="col-md-5">
            <div class="card shadow">
                <div class="card-header text-center">
                    <h4 class="my-2 fw-bold">Sign In</h4>
                </div>
                <div class="card-body p-4">

                    <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                        <asp:Literal ID="litError" runat="server" />
                    </asp:Panel>

                    <div class="mb-3">
                        <label class="form-label">Email address</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="you@example.com" />
                        <asp:RequiredFieldValidator ControlToValidate="txtEmail" runat="server"
                            ErrorMessage="Email is required." CssClass="text-danger small" Display="Dynamic" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label">Password</label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Password" />
                        <asp:RequiredFieldValidator ControlToValidate="txtPassword" runat="server"
                            ErrorMessage="Password is required." CssClass="text-danger small" Display="Dynamic" />
                    </div>

                    <div class="mb-3 form-check">
                        <asp:CheckBox ID="chkRemember" runat="server" CssClass="form-check-input" />
                        <label class="form-check-label" for="<%: chkRemember.ClientID %>">Remember me</label>
                    </div>

                    <asp:Button ID="btnLogin" runat="server" Text="Sign In"
                        CssClass="btn btn-primary w-100" OnClick="btnLogin_Click" />

                    <hr />
                    <p class="text-center mb-0">
                        Don't have an account?
                        <a href="~/Account/Register.aspx" runat="server">Register</a>
                    </p>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
