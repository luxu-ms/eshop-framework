<%@ Page Title="Admin - Products" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="eShopLegacy.Admin.ProductsAdminPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="d-flex justify-content-between align-items-center mb-4">
        <h1 class="display-6 fw-bold mb-0">Product Management</h1>
        <asp:Button ID="btnShowAdd" runat="server" Text="+ New Product" CssClass="btn btn-success" OnClick="btnShowAdd_Click" />
    </div>

    <!-- Add / Edit form -->
    <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="card mb-4">
        <div class="card-header fw-semibold">
            <asp:Label ID="lblFormTitle" runat="server" Text="Add Product" />
        </div>
        <div class="card-body">
            <asp:HiddenField ID="hfEditId" runat="server" Value="0" />
            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label">Name</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ControlToValidate="txtName" runat="server"
                        ErrorMessage="Name required." CssClass="text-danger small" Display="Dynamic"
                        ValidationGroup="ProductForm" />
                </div>
                <div class="col-md-6">
                    <label class="form-label">Price</label>
                    <asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" placeholder="0.00" />
                    <asp:RequiredFieldValidator ControlToValidate="txtPrice" runat="server"
                        ErrorMessage="Price required." CssClass="text-danger small" Display="Dynamic"
                        ValidationGroup="ProductForm" />
                </div>
                <div class="col-12">
                    <label class="form-label">Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                </div>
                <div class="col-md-4">
                    <label class="form-label">Brand</label>
                    <asp:DropDownList ID="ddlBrand" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-4">
                    <label class="form-label">Type</label>
                    <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-4">
                    <label class="form-label">Available Stock</label>
                    <asp:TextBox ID="txtStock" runat="server" CssClass="form-control" Text="0" />
                </div>
            </div>
            <div class="mt-3 d-flex gap-2">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" ValidationGroup="ProductForm" OnClick="btnSave_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline-secondary" OnClick="btnCancel_Click" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>

    <!-- Products table -->
    <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success alert-dismissible">
        <asp:Literal ID="litSuccessMsg" runat="server" />
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </asp:Panel>

    <div class="table-responsive">
        <asp:GridView ID="gvProducts" runat="server"
            CssClass="table table-hover table-bordered align-middle"
            AutoGenerateColumns="false"
            DataKeyNames="Id"
            OnRowCommand="gvProducts_RowCommand">
            <HeaderStyle CssClass="table-dark" />
            <Columns>
                <asp:BoundField DataField="Id"          HeaderText="ID"          HeaderStyle-Width="50px" />
                <asp:BoundField DataField="Name"        HeaderText="Name" />
                <asp:BoundField DataField="Price"       HeaderText="Price"       DataFormatString="{0:$0.00}" />
                <asp:TemplateField HeaderText="Brand">
                    <ItemTemplate><%# Eval("CatalogBrand.Brand") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Type">
                    <ItemTemplate><%# Eval("CatalogType.Type") %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="AvailableStock" HeaderText="Stock" />
                <asp:TemplateField HeaderText="Actions" HeaderStyle-Width="130px">
                    <ItemTemplate>
                        <asp:LinkButton runat="server" CommandName="EditItem" CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-outline-primary me-1">Edit</asp:LinkButton>
                        <asp:LinkButton runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-outline-danger"
                            OnClientClick="return confirm('Delete this product?')">Delete</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</asp:Content>
