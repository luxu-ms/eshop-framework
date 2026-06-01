<%@ Page Title="Catalog" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="eShopLegacy.Catalog.CatalogPage" EnableEventValidation="false" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row mb-4 align-items-end">
        <div class="col">
            <h1 class="display-6 fw-bold">Catalog</h1>
        </div>
    </div>

    <!-- Filters -->
    <div class="row g-3 mb-4">
        <div class="col-md-3">
            <label class="form-label fw-semibold">Brand</label>
            <asp:DropDownList ID="ddlBrand" runat="server" CssClass="form-select">
                <asp:ListItem Value="0" Text="All Brands" />
            </asp:DropDownList>
        </div>
        <div class="col-md-3">
            <label class="form-label fw-semibold">Type</label>
            <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select">
                <asp:ListItem Value="0" Text="All Types" />
            </asp:DropDownList>
        </div>
        <div class="col-md-4">
            <label class="form-label fw-semibold">Search</label>
            <div class="input-group">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search products..." />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-outline-secondary" OnClick="btnSearch_Click" />
            </div>
        </div>
    </div>

    <!-- Product Grid -->
    <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 g-4" id="productGrid">
                <asp:Repeater ID="rptProducts" runat="server">
                    <ItemTemplate>
                        <div class="col">
                            <div class="card h-100 shadow-sm product-card">
                                <a href='<%# "~/Catalog/ProductDetail.aspx?id=" + Eval("Id") %>' runat="server">
                                    <img src='<%# GetProductImage(Eval("PictureUri")) %>'
                                         class="card-img-top product-img"
                                         alt='<%# Eval("Name") %>'
                                         onerror="this.onerror=null;this.src='<%= eShopLegacy.CdnHelper.GetUrl("/Content/placeholder.png") %>?v=2';" />
                                </a>
                                <div class="card-body d-flex flex-column">
                                    <h6 class="card-title"><%# Eval("Name") %></h6>
                                    <p class="card-text text-muted small flex-grow-1"><%# Eval("CatalogBrand.Brand") %> &mdash; <%# Eval("CatalogType.Type") %></p>
                                    <div class="d-flex justify-content-between align-items-center mt-2">
                                        <span class="fs-5 fw-bold text-success">$<%# string.Format("{0:0.00}", Eval("Price")) %></span>
                                        <asp:Button ID="btnAddToCart" runat="server"
                                            Text="Add to Cart"
                                            CssClass="btn btn-primary btn-sm"
                                            CommandName="AddToCart"
                                            CommandArgument='<%# Eval("Id") + "|" + Eval("Price") %>'
                                            OnCommand="btnAddToCart_Command" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <!-- Empty state -->
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="text-center py-5">
                <p class="text-muted fs-5">No products found matching your criteria.</p>
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" CssClass="btn btn-outline-primary" OnClick="btnClearFilters_Click" />
            </asp:Panel>

            <!-- Pager -->
            <asp:Panel ID="pnlPager" runat="server" CssClass="d-flex justify-content-center mt-4">
                <nav>
                    <ul class="pagination">
                        <li class="page-item">
                            <asp:HyperLink ID="btnPrev" runat="server" CssClass="page-link">&laquo; Previous</asp:HyperLink>
                        </li>
                        <li class="page-item disabled">
                            <span class="page-link">Page <asp:Label ID="lblPage" runat="server" /> of <asp:Label ID="lblTotalPages" runat="server" /></span>
                        </li>
                        <li class="page-item">
                            <asp:HyperLink ID="btnNext" runat="server" CssClass="page-link">Next &raquo;</asp:HyperLink>
                        </li>
                    </ul>
                </nav>
            </asp:Panel>

    <!-- Toast notification -->
    <div id="cartToast" class="toast align-items-center text-white bg-success border-0 position-fixed bottom-0 end-0 m-3" role="alert" style="z-index:9999">
        <div class="d-flex">
            <div class="toast-body">Item added to cart!</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    </div>
</asp:Content>

<asp:Content ID="ScriptsContent" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        (function () {
            // Filter dropdowns navigate via GET — no postback
            function applyFilters() {
                var brand = document.getElementById('<%= ddlBrand.ClientID %>').value;
                var type  = document.getElementById('<%= ddlType.ClientID %>').value;
                var q     = document.getElementById('<%= txtSearch.ClientID %>').value.trim();
                var url   = 'Default.aspx';
                var sep   = '?';
                if (brand !== '0') { url += sep + 'brand=' + brand; sep = '&'; }
                if (type  !== '0') { url += sep + 'type='  + type;  sep = '&'; }
                if (q)             { url += sep + 'q='     + encodeURIComponent(q); }
                window.location.href = url;
            }
            document.getElementById('<%= ddlBrand.ClientID %>').addEventListener('change', applyFilters);
            document.getElementById('<%= ddlType.ClientID %>').addEventListener('change', applyFilters);

            // Show add-to-cart toast if redirected back with ?added=1
            var qs = new URLSearchParams(window.location.search);
            if (qs.get('added') === '1') {
                var toastEl = document.getElementById('cartToast');
                if (toastEl) { new bootstrap.Toast(toastEl).show(); }
                qs.delete('added');
                var clean = window.location.pathname + (qs.toString() ? '?' + qs.toString() : '');
                history.replaceState(null, '', clean);
            }
        })();
    </script>
</asp:Content>
