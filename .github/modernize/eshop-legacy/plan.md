# Modernization Plan: eShopLegacy Azure Migration

**Project**: eShopLegacy

---

## Technical Framework

- **Language**: C# (.NET Framework 4.8)
- **Framework**: ASP.NET Web Forms with OWIN/Katana
- **Build Tool**: MSBuild
- **Database**: SQL Server (LocalDB with MDF, production via Integrated Security)
- **Key Dependencies**: Entity Framework 6, ASP.NET Identity 2.x (OWIN), System.Data.SqlClient

---

## Overview

> This migration prepares the eShopLegacy ASP.NET Web Forms application for Azure App Service deployment. The application currently uses in-process session state, Windows-integrated SQL authentication, direct connection strings in config files, and serves static assets from the application directory.
>
> The new architecture will:
>
> - Replace Windows Authentication with Azure App Service built-in auth / Microsoft Entra ID for cloud-compatible identity
> - Migrate database connectivity to Azure SQL with Microsoft.Data.SqlClient for Managed Identity support
> - Externalize session state to Azure Cache for Redis to enable horizontal scale-out
> - Move static content to Azure Blob Storage with Azure CDN for performance and cost optimization
> - Secure all secrets and connection strings via Azure Key Vault and configuration builders
>
> The migration follows a phased approach: identity first (mandatory), then database, scalability, and security hardening.

---

## Migration Impact Summary

| Application  | Original Service            | New Azure Service              | Authentication   | Comments                              |
|-------------|----------------------------|-------------------------------|-----------------|---------------------------------------|
| eShopLegacy | Windows Auth (SQL)         | Microsoft Entra ID            | Managed Identity | Mandatory for App Service deployment  |
| eShopLegacy | SQL Server (LocalDB)       | Azure SQL Database            | Managed Identity | Upgrade SqlClient package             |
| eShopLegacy | In-process session state   | Azure Cache for Redis         | Managed Identity | Required for scale-out                |
| eShopLegacy | Local static files         | Azure Blob Storage + CDN     | Managed Identity | CSS, JS, images                       |
| eShopLegacy | Web.config connection strings | Azure Key Vault           | Managed Identity | Config builders for secrets           |

---

## Clarifications

The following items were not explicitly requested but may be needed for a complete implementation:

1. **Azure SQL Database SKU**: The plan assumes Azure SQL Database (serverless or provisioned). Azure SQL Managed Instance is an alternative if full SQL Server compatibility is required.
   - **Recommendation**: Use Azure SQL Database unless Managed Instance features are needed.

2. **Redis Cache tier**: Standard C0 is assumed for session state. Premium tier may be needed for VNet injection.
   - **Recommendation**: Start with Standard C0 and scale as needed.

3. **CDN profile**: Azure CDN Standard (Microsoft) is assumed for static content delivery.
   - **Recommendation**: Use Microsoft CDN unless a specific provider is required.
