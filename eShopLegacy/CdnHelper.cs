using System.Configuration;

namespace eShopLegacy
{
    /// <summary>
    /// Helper for generating CDN-resolved URLs for static assets.
    ///
    /// Static assets (CSS, JS, images) are stored in Azure Blob Storage and served
    /// through Azure CDN for improved performance and global distribution.
    ///
    /// Configuration:
    ///   Web.config appSetting 'CdnBaseUrl' — set to the Azure CDN endpoint URL,
    ///   e.g. https://your-cdn-endpoint.azureedge.net/static
    ///
    ///   When CdnBaseUrl is empty or not configured, local paths are returned as-is
    ///   (useful for local development without a CDN).
    ///
    /// Azure Blob Storage container structure:
    ///   Container: static (public blob access or CDN delivery)
    ///   Path layout mirrors the local wwwroot/file system structure:
    ///     Content/bootstrap.min.css
    ///     Content/site.css
    ///     Scripts/jquery-3.6.0.min.js
    ///     Scripts/bootstrap.bundle.min.js
    ///     images/placeholder.png
    ///     favicon.ico
    /// </summary>
    public static class CdnHelper
    {
        private static string _cdnBaseUrl;

        private static string CdnBaseUrl
        {
            get
            {
                if (_cdnBaseUrl == null)
                {
                    _cdnBaseUrl = ConfigurationManager.AppSettings["CdnBaseUrl"] ?? string.Empty;
                }
                return _cdnBaseUrl;
            }
        }

        /// <summary>
        /// Returns the CDN URL for a static asset, or the original local path if CDN is not configured.
        /// </summary>
        /// <param name="localPath">Local path starting with / e.g. /Content/site.css</param>
        public static string GetUrl(string localPath)
        {
            if (string.IsNullOrEmpty(CdnBaseUrl))
                return localPath;

            // Combine CDN base URL with the local path (strip leading slash to avoid double slash)
            return CdnBaseUrl.TrimEnd('/') + "/" + localPath.TrimStart('/');
        }
    }
}
