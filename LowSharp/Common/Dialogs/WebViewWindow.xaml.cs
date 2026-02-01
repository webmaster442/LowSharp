using System.Diagnostics;
using System.Windows;

using Microsoft.Web.WebView2.Core;

namespace LowSharp.Common.Dialogs;
/// <summary>
/// Interaction logic for WebViewWindow.xaml
/// </summary>
public partial class WebViewWindow : Window
{
    private string _html;
    private Uri? _url;

    public WebViewWindow()
    {
        _html = string.Empty;
        InitializeComponent();
    }

    private void SetPosition(Window owner)
    {
        Width = owner.ActualWidth * 0.8;
        Height = owner.ActualHeight * 0.8;
        Top = (owner.Top + (owner.ActualHeight - Height) / 2);
        Left = (owner.Left + (owner.ActualWidth - Width) / 2);
    }

    public void ShowFromHtml(Window owner, string html)
    {
        _html = html;
        SetPosition(owner);
        Show();
    }

    public void ShowAndNavigateTo(Window owner, Uri url)
    {
        _url = url;
        SetPosition(owner);
        Show();
    }

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        try
        {
            var webView2Environment = await CoreWebView2Environment.CreateAsync();
            await webView.EnsureCoreWebView2Async(webView2Environment);

            if (_url != null)
            {
                webView.Source = _url;
                return;
            }

            if (!string.IsNullOrEmpty(_html))
            {
                webView.NavigateToString(_html);
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }

}
