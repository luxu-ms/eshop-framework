<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="eShopLegacy.Account.LoginPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row justify-content-center mt-5">
        <div class="col-md-4">
            <div class="card shadow p-4">
                <h4 class="fw-bold mb-3 text-center">Sign In</h4>

                <asp:Panel ID="ErrorPanel" runat="server" Visible="false" CssClass="alert alert-danger">
                    <asp:Literal ID="ErrorMessage" runat="server" />
                </asp:Panel>

                <div class="mb-3">
                    <label for="Email" class="form-label">Email</label>
                    <asp:TextBox ID="Email" runat="server" CssClass="form-control" placeholder="admin@eshop.com" />
                </div>
                <div class="mb-3">
                    <label for="Password" class="form-label">Password</label>
                    <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="form-control" />
                </div>
                <asp:Button ID="LoginButton" runat="server" Text="Sign In" CssClass="btn btn-primary w-100" OnClick="LoginButton_Click" />

                <p class="small text-muted mt-3 text-center">Default: admin@eshop.com / Admin@123!</p>
            </div>
        </div>
    </div>

</asp:Content>
