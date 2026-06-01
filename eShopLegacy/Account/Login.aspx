<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="eShopLegacy.Account.LoginPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <%-- Authentication is now handled by Microsoft Entra ID (Azure AD) via OpenID Connect.
         Page_Load immediately triggers the OIDC challenge for unauthenticated users.
         This markup is shown only briefly before the redirect fires. --%>
    <div class="row justify-content-center mt-5">
        <div class="col-md-5 text-center">
            <div class="card shadow p-4">
                <h4 class="fw-bold mb-3">Sign In</h4>
                <p class="text-muted">
                    Authentication is managed through <strong>Microsoft Entra ID</strong> (Azure AD).
                    You will be redirected to the Microsoft sign-in page.
                </p>
                <div class="spinner-border text-primary mt-3" role="status">
                    <span class="visually-hidden">Redirecting...</span>
                </div>
                <p class="small text-muted mt-3">Redirecting to Microsoft sign-in&hellip;</p>
            </div>
        </div>
    </div>

</asp:Content>
