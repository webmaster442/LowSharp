using System.Windows.Controls;

using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Client.Common;

namespace LowSharp.Client.Repl;
/// <summary>
/// Interaction logic for ReplView.xaml
/// </summary>
public partial class ReplView : UserControl
{
    public ReplView()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<Messages.AppendReplOutput>(this, OnOutput);
        WeakReferenceMessenger.Default.Register<Messages.ClearReplOutput>(this, OnClearOutput);
        Output.Document = new System.Windows.Documents.FlowDocument();
    }

    private void OnClearOutput(object recipient, Messages.ClearReplOutput message)
        => Output.Document.Blocks.Clear();

    private void OnOutput(object recipient, Messages.AppendReplOutput message)
    {
        FlowDocumentFormatter.AppendFormattedText(Output.Document, message.Output);
        Output.CaretPosition = Output.Document.ContentEnd;
    }
}
