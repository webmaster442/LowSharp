using System.Windows;

using ICSharpCode.AvalonEdit;

using Microsoft.Xaml.Behaviors;

namespace LowSharp.Client.Common;

internal sealed class AvalonEditBehaviour : Behavior<TextEditor>
{
    public static readonly DependencyProperty BindedTextProperty =
        DependencyProperty.Register(nameof(BindedText), typeof(string), typeof(AvalonEditBehaviour),
        new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

    public string BindedText
    {
        get { return (string)GetValue(BindedTextProperty); }
        set { SetValue(BindedTextProperty, value); }
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject != null)
            AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject != null)
            AssociatedObject.TextChanged -= AssociatedObjectOnTextChanged;
    }

    private void AssociatedObjectOnTextChanged(object? sender, EventArgs eventArgs)
    {
        var textEditor = sender as TextEditor;
        if (textEditor != null)
        {
            if (textEditor.Document != null)
                BindedText = textEditor.Document.Text;
        }
    }

    private static void PropertyChangedCallback(DependencyObject dependencyObject,
                                                DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        if (dependencyObject is AvalonEditBehaviour behaviour
            && behaviour.AssociatedObject is TextEditor editor
            && editor.Document != null)
        {
            var caretOffset = editor.CaretOffset;
            editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
            editor.CaretOffset = caretOffset;
        }
    }
}
