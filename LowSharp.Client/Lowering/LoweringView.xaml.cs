using System.Windows.Controls;

using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Client.Common;

namespace LowSharp.Client.Lowering;

/// <summary>
/// Interaction logic for LoweringView.xaml
/// </summary>
public partial class LoweringView : UserControl
{
    public LoweringView()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<RequestMessages.GetInputCodeRequest>(this, OnGetInputCode); // (recipient, message) => m
        WeakReferenceMessenger.Default.Register<Messages.SetInputCode>(this, OnSetInputCode);
        WeakReferenceMessenger.Default.Register<Messages.SetOutputCodeRequest>(this, OnSetOutputCode);
    }

    private void OnSetOutputCode(object recipient, Messages.SetOutputCodeRequest message)
        => Output.Document.Text = message.Code;

    private void OnSetInputCode(object recipient, Messages.SetInputCode message)
        => Input.Document.Text = message.Code;

    private void OnGetInputCode(object recipient, RequestMessages.GetInputCodeRequest message)
        => message.Reply(Input.Document.Text);
}
