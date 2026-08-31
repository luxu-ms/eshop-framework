[CmdletBinding()]
param(
    [string] $BaseUrl = 'http://localhost:5195',
    [switch] $SkipBuild
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot

if (-not $SkipBuild) {
    & (Join-Path $PSScriptRoot 'build.ps1') -Configuration Debug
    if ($LASTEXITCODE -ne 0) { throw 'Legacy build failed.' }
    dotnet build (Join-Path $repositoryRoot 'eShop.Web\eShop.Web.csproj') --configuration Debug --verbosity minimal
    if ($LASTEXITCODE -ne 0) { throw 'Core build failed.' }
}

function Assert-Response([string] $Path, [int] $StatusCode, [string] $Contains) {
    $response = Invoke-WebRequest ($BaseUrl.TrimEnd('/') + $Path) -SkipHttpErrorCheck
    if ($response.StatusCode -ne $StatusCode) {
        throw "$Path returned $($response.StatusCode); expected $StatusCode."
    }
    if ($Contains -and -not $response.Content.Contains($Contains)) {
        throw "$Path did not contain '$Contains'."
    }
}

Assert-Response '/health' 200 'Healthy'
Assert-Response '/health/data' 200 '"catalogItems":12'
Assert-Response '/Catalog/Default.aspx' 200 'Catalog'
Assert-Response '/Catalog/ProductDetail.aspx?id=1' 200 'Add to Cart'
Assert-Response '/Cart/ShoppingCart.aspx' 200 'Shopping Cart'
Assert-Response '/Account/Login.aspx' 200 'Sign In'
Assert-Response '/Account/Register.aspx' 200 'Create Account'
Assert-Response '/Checkout/Checkout.aspx' 200 'Sign In'
Assert-Response '/Checkout/OrderHistory.aspx' 200 'Sign In'
Assert-Response '/Admin/Products.aspx' 200 'Sign In'
Assert-Response '/Content/site.css' 200 'product-card'

Write-Output 'All Core HTTP smoke tests passed.'