using System.Windows;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace LowSharp.Common.Dialogs;

/// <summary>
/// Interaction logic for MessageDialog.xaml
/// </summary>
public partial class MessageDialog : Window
{
    public MessageDialog()
    {
        InitializeComponent();
    }

    private static MessageDialog Create(Window owner, string title, string message)
    {
        var dialog = new MessageDialog
        {
            Title = title,
        };
        dialog.Width = owner.ActualWidth * 0.8;
        dialog.Height = owner.ActualHeight * 0.8;
        dialog.TbMessage.Text = message;
        dialog.TbTitle.Text = title;
        dialog.Owner = owner;
        return dialog;
    }

    public static bool Show(Window owner, string title, string message)
    {
        var dialog = Create(owner, title, message);
        dialog.BtnCancel.Visibility = Visibility.Collapsed;
        return dialog.ShowDialog() == true;
    }

    public static bool Confirm(Window owner, string title, string message)
    {
        var dialog = Create(owner, title, message);
        return dialog.ShowDialog() == true;
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
        => DialogResult = true;

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
