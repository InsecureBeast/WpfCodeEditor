using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System.IO;
using System.Threading.Tasks;

namespace WpfMonacoEditor
{
    internal class WebViewInitializer
    {
        private readonly WebView2 _webView;
        private CoreWebView2Environment _env;

        public WebViewInitializer(WebView2 webView)
        {
            _webView = webView;
        }

        public async Task InitializeAsync()
        {
            await InitEnvironmentAsync();
            await _webView.EnsureCoreWebView2Async(_env);
            _webView.CoreWebView2InitializationCompleted += OnCoreWebView2InitializationCompleted;
        }

        private async Task InitEnvironmentAsync()
        {
            if (_env == null)
            {
                var temp = Path.Combine(Path.GetTempPath(), "WpfMonacoEditor");
                if (!Directory.Exists(temp))
                    Directory.CreateDirectory(temp);

                _env = await CoreWebView2Environment.CreateAsync(userDataFolder: temp);
            }
        }

        private void OnCoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            // Обработка завершения инициализации WebView2
        }
    }
}
