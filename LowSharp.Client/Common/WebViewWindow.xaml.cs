

using MahApps.Metro.SimpleChildWindow;

using Microsoft.Web.WebView2.Core;

namespace LowSharp.Client.Common;
/// <summary>
/// Interaction logic for DiagramPreviewWindow.xaml
/// </summary>
public partial class WebViewWindow : ChildWindow
{
    public WebViewWindow()
    {
        InitializeComponent();
        InitializeAsync();
        WebView.NavigationStarting += OnNavigationStart;
    }

    private void OnNavigationStart(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        WebView.CoreWebView2.ExecuteScriptAsync($"alert('Navigation is disabled')");
        // Prevent navigation to other pages
        e.Cancel = true;
    }

    private async void InitializeAsync()
        => await WebView.EnsureCoreWebView2Async(null);

    private void BtnClose_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Close();
    }
}
