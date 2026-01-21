using System.Diagnostics;

using MahApps.Metro.Controls;

using Microsoft.Web.WebView2.Core;

namespace LowSharp.Client.Common;
/// <summary>
/// Interaction logic for WebViewWindow.xaml
/// </summary>
public partial class WebViewWindow : MetroWindow
{
    private string _html;

    public WebViewWindow()
    {
        _html = string.Empty;
        InitializeComponent();
    }

    public bool? ShowFromHtml(string html)
    {
        _html = html;
        return ShowDialog();
    }

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        try
        {
            var webView2Environment = await CoreWebView2Environment.CreateAsync();
            await webView.EnsureCoreWebView2Async(webView2Environment);

            if (!string.IsNullOrEmpty(_html))
                webView.NavigateToString(_html);
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }

}
