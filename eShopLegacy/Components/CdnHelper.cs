using System.Configuration;

namespace eShopLegacy.Components
{
    /// <summary>
    /// Provides helper methods for resolving static asset URLs through Azure CDN.
    /// When CdnBaseUrl is configured, all static assets (CSS, JS, images) are served
    /// from the Azure CDN endpoint that fronts the Azure Blob Storage container.
    /// When CdnBaseUrl is not configured, falls back to local relative paths.
    /// </summary>
    public static class CdnHelper
    {
        private static string _cdnBaseUrl;

        /// <summary>
        /// Gets the Azure CDN base URL from configuration (CdnBaseUrl app setting).
        /// Example: https://&lt;your-cdn-endpoint&gt;.azureedge.net
        /// </summary>
        public static string BaseUrl
        {
            get
            {
                if (_cdnBaseUrl == null)
                    _cdnBaseUrl = ConfigurationManager.AppSettings["CdnBaseUrl"] ?? string.Empty;
                return _cdnBaseUrl;
            }
        }

        /// <summary>
        /// Resolves a relative static-asset path to an absolute CDN URL.
        /// If no CDN is configured, returns a root-relative URL for local serving.
        /// </summary>
        /// <param name="relativePath">
        /// The path relative to the blob container root, e.g. "Content/bootstrap.min.css"
        /// </param>
        /// <returns>Absolute CDN URL or root-relative path.</returns>
        public static string Url(string relativePath)
        {
            var baseUrl = BaseUrl;
            if (string.IsNullOrEmpty(baseUrl))
                return "/" + relativePath.TrimStart('/');

            return baseUrl.TrimEnd('/') + "/" + relativePath.TrimStart('/');
        }
    }
}
