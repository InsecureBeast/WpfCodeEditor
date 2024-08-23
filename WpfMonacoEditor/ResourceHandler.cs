using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace WpfMonacoEditor
{
    internal class ResourceHandler
    {
        private readonly WebView2 _webView;

        public ResourceHandler(WebView2 webView)
        {
            _webView = webView;
        }

        public void Initialize() 
        {
            _webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            _webView.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
        }

        private void OnWebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            var resourceName = GetResourcePath(e.Request.Uri);
            var stream = typeof(CodeEditor).Assembly.GetManifestResourceStream($"WpfMonacoEditor.{resourceName}");
            string mimeType = GetMimeMapping(e.Request.Uri);
            e.Response = _webView.CoreWebView2.Environment.CreateWebResourceResponse(stream, 200, "OK", $"content-type: {mimeType}");
        }

        private string GetResourcePath(string uri)
        {
            var replaceToIndex = uri.ToLower().LastIndexOf("monaco");
            if (replaceToIndex == -1)
                return uri;

            var resourcePath = uri.Substring(replaceToIndex);
            return resourcePath.Replace("/", ".");
        }

        private string GetMimeMapping(string filename)
        {
            var provider = new FileExtensionContentTypeProvider();
            return provider.TryGetContentType(filename, out string mimeType)
                ? mimeType
                : "application/octet-stream";
        }
    }
}
