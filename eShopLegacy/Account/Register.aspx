<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="eShopLegacy.Account.RegisterPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <%-- Registration is now handled by Microsoft Entra ID (Azure AD).
         Page_Load redirects unauthenticated users to the Entra ID sign-in/sign-up page.
         Authenticated users are redirected to the home page.
         This markup is shown only briefly before the redirect fires. --%>
    <div class="row justify-content-center mt-5">
        <div class="col-md-6 text-center">
            <div class="card shadow p-4">
                <h4 class="fw-bold mb-3">Create Account</h4>
                <p class="text-muted">
                    Account registration is managed through <strong>Microsoft Entra ID</strong>.
                    You will be redirected to the Microsoft sign-in page to create or sign in to your account.
                </p>
                <div class="spinner-border text-primary mt-3" role="status">
                    <span class="visually-hidden">Redirecting...</span>
                </div>
                <p class="small text-muted mt-3">Redirecting to Microsoft sign-in&hellip;</p>
            </div>
        </div>
    </div>

</asp:Content>
