# Migration Progress: eShopLegacy Azure Migration

**Session ID**: 7f85ded3-b17e-41d9-8152-2d2cf06ea440
**Branch**: modernize/dotnet-20260601135014
**Started**: 2026-06-01
**Language**: dotnet (.NET Framework 4.8)

## General

| Property | Value |
|----------|-------|
| Previous Branch | main |
| Current Branch | modernize/dotnet-20260601135014 |
| Version Control | Git |

## Task Status

| Task ID | Description | Status | Commit |
|---------|-------------|--------|--------|
| 001-transform-identity-forms-to-entra-id | Migrate Forms/OWIN auth → Microsoft Entra ID | 🔄 In Progress | - |
| 002-transform-database-to-azure-sql | Migrate LocalDB → Azure SQL Database | ⏳ Pending | - |
| 003-transform-sqlclient-upgrade | Upgrade System.Data.SqlClient → Microsoft.Data.SqlClient | ⏳ Pending | - |
| 004-transform-session-to-redis | Migrate InProc session → Azure Cache for Redis | ⏳ Pending | - |
| 005-transform-config-to-keyvault | Secure connection strings via Azure Key Vault | ⏳ Pending | - |
| 006-transform-externalize-secrets | Externalize hardcoded secrets to Key Vault | ⏳ Pending | - |
| 007-transform-static-content-to-blob-cdn | Static content → Azure Blob Storage + CDN | ⏳ Pending | - |
