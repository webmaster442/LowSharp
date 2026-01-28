using System.Windows;
using System.Windows.Controls;

namespace LowSharp.Common.Controls;

internal class EnumComboBox : Control
{
    public EnumViewModel Enum
    {
        get { return (EnumViewModel)GetValue(EnumProperty); }
        set { SetValue(EnumProperty, value); }
    }

    public static readonly DependencyProperty EnumProperty =
        DependencyProperty.Register(nameof(Enum), typeof(EnumViewModel), typeof(EnumComboBox), new PropertyMetadata(null));
}
