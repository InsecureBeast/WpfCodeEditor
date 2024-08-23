using Microsoft.Web.WebView2.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace WpfMonacoEditor
{
    public class CodeEditor : Control
    {
        public static readonly DependencyProperty SourceCodeProperty = DependencyProperty.Register(
                "SourceCode",
                typeof(string),
                typeof(CodeEditor),
                new PropertyMetadata(null, OnSourceCodeChanged));

        private Grid _layoutRoot;
        private WebViewInitializer _webViewInitializer;
        private SourceCodeManager _sourceCodeManager;
        private ResourceHandler _resourceHandler;

        static CodeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CodeEditor), new FrameworkPropertyMetadata(typeof(CodeEditor)));
        }

        public CodeEditor()
        {
            Loaded += OnLoaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _layoutRoot = (Grid)Template.FindName("PART_LayoutRoot", this);
            var monacoWebView = GetTemplateChild("MonacoWebView") as WebView2;

            _resourceHandler = new ResourceHandler(monacoWebView);
            _webViewInitializer = new WebViewInitializer(monacoWebView);
            _sourceCodeManager = new SourceCodeManager(monacoWebView);
        }

        public string SourceCode
        {
            get { return (string)GetValue(SourceCodeProperty); }
            set { SetValue(SourceCodeProperty, value); }
        }

        private static async void OnSourceCodeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is CodeEditor codeEditor && e.NewValue is string sourceCode)
            {
                await codeEditor._sourceCodeManager.UpdateSourceCodeAsync(sourceCode);
            }
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await _webViewInitializer.InitializeAsync();
            _resourceHandler.Initialize();
            await _sourceCodeManager.InitializeAsync(SourceCode);
            _layoutRoot.Opacity = 100;
            Loaded -= OnLoaded;
        }
    }
}
