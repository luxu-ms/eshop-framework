# Modernization Plan: eShopLegacy Azure Migration

**Project**: eShopLegacy

---

## Technical Framework

- **Language**: C# (.NET Framework 4.8)
- **Framework**: ASP.NET Web Forms with OWIN/ASP.NET Identity
- **Build Tool**: MSBuild
- **Database**: SQL Server (LocalDB) via Entity Framework 6 / System.Data.SqlClient
- **Key Dependencies**: Entity Framework 6, Microsoft.AspNet.Identity, OWIN, jQuery, Bootstrap

---

## Overview

> This migration modernizes the eShopLegacy ASP.NET Web Forms application for Azure cloud readiness. The application currently runs on .NET Framework 4.8 with in-process session state, Forms authentication via ASP.NET Identity/OWIN, SQL Server accessed through System.Data.SqlClient, and static assets served from the local filesystem. The new architecture will:
>
> - Migrate identity management from Forms/OWIN authentication to Microsoft Entra ID (Azure AD) for cloud-native, centralized identity
> - Move the SQL database connection to Azure SQL Database for managed, scalable data storage
> - Externalize session state from in-process to Azure Cache for Redis for horizontal scalability
> - Upgrade the data access layer from System.Data.SqlClient to Microsoft.Data.SqlClient for modern features and Azure compatibility
> - Secure all connection strings and sensitive configuration using Azure Key Vault
> - Externalize hardcoded sensitive data (credentials, secrets) to Azure Key Vault
> - Move static content (CSS, JS, images) to Azure Blob Storage with CDN for improved performance and scalability
>
> The migration follows a phased approach addressing mandatory issues first (identity), then database and security concerns, and finally scalability optimizations.

---

## Migration Impact Summary

| Application  | Original Service         | New Azure Service              | Authentication     | Comments                          |
|--------------|--------------------------|--------------------------------|--------------------|-----------------------------------|
| eShopLegacy  | Forms/OWIN Auth          | Microsoft Entra ID             | Managed Identity   | Mandatory for cloud deployment    |
| eShopLegacy  | SQL Server (LocalDB)     | Azure SQL Database             | Managed Identity   | Migrate connection configuration  |
| eShopLegacy  | In-Process Session       | Azure Cache for Redis          | Managed Identity   | Required for horizontal scaling   |
| eShopLegacy  | System.Data.SqlClient    | Microsoft.Data.SqlClient       | N/A                | NuGet package upgrade             |
| eShopLegacy  | Web.config appSettings   | Azure Key Vault                | Managed Identity   | Secure connection strings         |
| eShopLegacy  | Hardcoded secrets        | Azure Key Vault                | Managed Identity   | Externalize sensitive data        |
| eShopLegacy  | Local static files       | Azure Blob Storage + CDN       | Managed Identity   | CSS, JS, images to CDN            |

---

## Clarifications

The following items were not explicitly requested but may be needed for a complete implementation:

1. **Azure App Service Target**: The assessment targets Azure App Service. The plan assumes deployment to Azure App Service (Windows) without containerization unless otherwise specified.
   - **Recommendation**: Proceed with Azure App Service (Windows) as the deployment target.

2. **Entity Framework 6 Compatibility**: Upgrading System.Data.SqlClient to Microsoft.Data.SqlClient may require Entity Framework configuration changes.
   - **Recommendation**: Update the EF6 provider configuration alongside the SqlClient migration.
