<%@ Page Title="Product Detail" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductDetail.aspx.cs" Inherits="eShopLegacy.Catalog.ProductDetailPage" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlNotFound" runat="server" Visible="false" CssClass="alert alert-warning">
        Product not found. <a href="~/Catalog/Default.aspx" runat="server">Back to Catalog</a>
    </asp:Panel>

    <asp:Panel ID="pnlProduct" runat="server">
        <nav aria-label="breadcrumb" class="mb-3">
            <ol class="breadcrumb">
                <li class="breadcrumb-item"><a href="~/Catalog/Default.aspx" runat="server">Catalog</a></li>
                <li class="breadcrumb-item active"><asp:Label ID="lblBreadcrumb" runat="server" /></li>
            </ol>
        </nav>

        <div class="row g-5">
            <!-- Image -->
            <div class="col-md-5 text-center">
                <img id="imgProduct" runat="server" class="img-fluid rounded shadow product-detail-img" alt="Product image" />
            </div>

            <!-- Details -->
            <div class="col-md-7">
                <h2 class="fw-bold"><asp:Label ID="lblName" runat="server" /></h2>
                <p class="text-muted mb-1">
                    <asp:Label ID="lblBrand" runat="server" CssClass="badge bg-secondary me-1" />
                    <asp:Label ID="lblType"  runat="server" CssClass="badge bg-info text-dark" />
                </p>
                <h3 class="text-success my-3">$<asp:Label ID="lblPrice" runat="server" /></h3>
                <p class="lead"><asp:Label ID="lblDescription" runat="server" /></p>

                <div class="mb-3">
                    <asp:Label ID="lblStock" runat="server" CssClass="badge bg-success" />
                </div>

                <div class="d-flex align-items-center gap-3 mt-4">
                    <div class="input-group" style="max-width:130px">
                        <span class="input-group-text">Qty</span>
                        <asp:TextBox ID="txtQty" runat="server" CssClass="form-control text-center" Text="1"
                            type="number" min="1" max="99" />
                    </div>
                    <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart"
                        CssClass="btn btn-primary btn-lg" OnClick="btnAddToCart_Click" />
                </div>

                <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success mt-3">
                    Item added to your cart. <a href="~/Cart/ShoppingCart.aspx" runat="server">View Cart</a>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>

</asp:Content>
