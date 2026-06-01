using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Configuration;

namespace eShopLegacy.Services
{
    /// <summary>
    /// Service for reading secrets from Azure Key Vault using Managed Identity.
    /// Use this for secrets that cannot be injected via the Key Vault Configuration Builder
    /// (e.g., secrets needed before configuration is fully loaded, or runtime-fetched secrets).
    ///
    /// The recommended approach for most secrets is to store them in Key Vault and let the
    /// AzureKeyVaultConfigBuilder in Web.config populate ConfigurationManager at startup.
    /// </summary>
    public class KeyVaultService
    {
        private readonly SecretClient _client;

        /// <summary>
        /// Initialises the Key Vault client using Managed Identity (DefaultAzureCredential).
        /// The Key Vault URI is read from the 'KeyVaultUri' appSetting in Web.config.
        /// </summary>
        public KeyVaultService()
        {
            var vaultUri = ConfigurationManager.AppSettings["KeyVaultUri"]
                ?? throw new InvalidOperationException(
                    "KeyVaultUri appSetting is not configured. " +
                    "Add <add key=\"KeyVaultUri\" value=\"https://your-vault.vault.azure.net/\" /> to Web.config.");

            _client = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
        }

        /// <summary>
        /// Retrieves a secret value from Azure Key Vault by name.
        /// </summary>
        /// <param name="secretName">Name of the secret in Key Vault.</param>
        /// <returns>The secret value string.</returns>
        public string GetSecret(string secretName)
        {
            KeyVaultSecret secret = _client.GetSecret(secretName);
            return secret.Value;
        }

        /// <summary>
        /// Tries to retrieve a secret from Key Vault. Returns null if the secret is not found
        /// or if Key Vault is unreachable.
        /// </summary>
        /// <param name="secretName">Name of the secret in Key Vault.</param>
        /// <returns>The secret value, or null if not found.</returns>
        public string TryGetSecret(string secretName)
        {
            try
            {
                KeyVaultSecret secret = _client.GetSecret(secretName);
                return secret.Value;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                return null;
            }
            catch (Exception)
            {
                // Log or swallow — caller should decide fallback behaviour
                return null;
            }
        }
    }
}
