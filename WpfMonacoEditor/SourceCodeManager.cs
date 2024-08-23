using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Text;
using System.Threading.Tasks;

namespace WpfMonacoEditor
{
    internal class SourceCodeManager
    {
        private readonly WebView2 _webView;
        private bool _reactOnSourceChanged = true;

        public SourceCodeManager(WebView2 webView)
        {
            _webView = webView;
            _webView.WebMessageReceived += OnMessageReceived;
        }

        public async Task InitializeAsync(string initialSourceCode)
        {
            _webView.Source = new Uri("file://monaco/index.html");

            var scriptBuilder = new StringBuilder();
            scriptBuilder.Append($"editor.setValue(`{initialSourceCode}`);");
            await _webView.ExecuteScriptAsync(scriptBuilder.ToString());
        }

        public async Task UpdateSourceCodeAsync(string sourceCode)
        {
            if (_reactOnSourceChanged == false)
                return;

            var scriptBuilder = new StringBuilder();
            scriptBuilder.Append($"editor.setValue(`{sourceCode}`);");
            await _webView.ExecuteScriptAsync(scriptBuilder.ToString());
        }

        private async void OnMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            string content = args.TryGetWebMessageAsString();
            if (content.StartsWith("onContentChanged"))
            {
                var script = @"editor.getValue({lineEnding: '\r\n', preserveBOM: false})";
                var value = await _webView.ExecuteScriptAsync(script);
                value = value.Replace(@"\r\n", Environment.NewLine).Replace(@"\", "").TrimStart('\"').TrimEnd('\"');
                SetSourceCodeWithoutNotification(value);
            }
        }

        private void SetSourceCodeWithoutNotification(string value)
        {
            _reactOnSourceChanged = false;
            _reactOnSourceChanged = true;
        }
    }
}
