<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="eShopLegacy.Account.RegisterPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row justify-content-center">
        <div class="col-md-6">
            <div class="card shadow">
                <div class="card-header text-center">
                    <h4 class="my-2 fw-bold">Create Account</h4>
                </div>
                <div class="card-body p-4">

                    <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-danger">
                        <asp:Literal ID="litError" runat="server" />
                    </asp:Panel>

                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label">First name</label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ControlToValidate="txtFirstName" runat="server"
                                ErrorMessage="Required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Last name</label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ControlToValidate="txtLastName" runat="server"
                                ErrorMessage="Required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-12">
                            <label class="form-label">Email address</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="you@example.com" />
                            <asp:RequiredFieldValidator ControlToValidate="txtEmail" runat="server"
                                ErrorMessage="Email is required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
                            <asp:RequiredFieldValidator ControlToValidate="txtPassword" runat="server"
                                ErrorMessage="Password is required." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Confirm password</label>
                            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" />
                            <asp:CompareValidator ControlToValidate="txtConfirmPassword"
                                ControlToCompare="txtPassword" runat="server"
                                ErrorMessage="Passwords do not match." CssClass="text-danger small" Display="Dynamic" />
                        </div>
                    </div>

                    <asp:Button ID="btnRegister" runat="server" Text="Create Account"
                        CssClass="btn btn-primary w-100 mt-4" OnClick="btnRegister_Click" />

                    <hr />
                    <p class="text-center mb-0">
                        Already have an account?
                        <a href="~/Account/Login.aspx" runat="server">Sign In</a>
                    </p>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
