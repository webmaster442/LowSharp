using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LowSharp.ApiV1.SchemaCodeGen;
using LowSharp.ClientLib;
using LowSharp.Common;
using LowSharp.Common.Controls;
using LowSharp.Common.ViewModels;

namespace LowSharp.Schema;

internal sealed partial class JsonSchemaToCsharpViewModel : ViewModelWithMenus
{
    private readonly IClient _client;
    private readonly IDialogs _dialogs;

    public MenuCheckableViewModel ShowLineNumbers { get; }
    public MenuCheckableViewModel WordWrap { get; }

    public EnumViewModel<DateType> DateTypes { get; }

    public EnumViewModel<TimeType> TimeTypes { get; }

    public EnumViewModel<DateTimeType> DateTimeTypes { get; }

    public EnumViewModel<AccessModifier> Accesmodifers { get; }

    public EnumViewModel<ClassStyle> ClassStyles { get; }

    public EnumViewModel<JsonLibary> JsonLibaries { get; }

    [ObservableProperty]
    public partial bool UseRequired { get; set; }

    [ObservableProperty]
    public partial bool UseNullableReferenceTypes { get; set; }

    [ObservableProperty]
    public partial string Namespace { get; set; }

    [ObservableProperty]
    public partial string Json { get; set; }

    [ObservableProperty]
    public partial string CsharpCode { get; set; }

    public JsonSchemaToCsharpViewModel(IClient client, IDialogs dialogs)
    {
        _client = client;
        _dialogs = dialogs;
        DateTypes = new EnumViewModel<DateType>(DateType.Dateonly);
        TimeTypes = new EnumViewModel<TimeType>(TimeType.Timeonly);
        DateTimeTypes = new EnumViewModel<DateTimeType>(DateTimeType.Datetimeoffset);
        Accesmodifers = new EnumViewModel<AccessModifier>(AccessModifier.Public);
        ClassStyles = new EnumViewModel<ClassStyle>(ClassStyle.Record);
        JsonLibaries = new EnumViewModel<JsonLibary>(JsonLibary.Systemtext);
        Json = string.Empty;
        CsharpCode = string.Empty;
        Namespace = "MyNamespace";

        ShowLineNumbers = new MenuCheckableViewModel
        {
            Header = "Show Line Numbers",
            IsChecked = true
        };

        WordWrap = new MenuCheckableViewModel
        {
            Header = "Word Wrap",
            IsChecked = true
        };

        Menus.Add(new MenuViewModel
        {
            Header = "View",
            Children =
            {
                ShowLineNumbers,
                WordWrap
            }
        });

        Menus.Add(new MenuViewModel
        {
            Header = "Import/Export",
            Children =
            {
                new MenuCommandViewModel
                {
                    Header = "Open JSON...",
                    Command = OpenCodeCommand
                },
                new MenuCommandViewModel
                {
                    Header = "Save Generated...",
                    Command = SaveCodeCommand
                }
            }
        });
    }


    [RelayCommand]
    public void OpenCode()
    {
        if (_dialogs.TryOpen("Open Json Schema", "JSON|*.json", out var filename))
        {
            Json = File.ReadAllText(filename);
        }
    }

    [RelayCommand]
    public void SaveCode()
    {
        if (_dialogs.TrySave("Save generated code", "C# source|*.cs", out string fileName))
        {
            File.WriteAllText(fileName, CsharpCode);
        }
    }

    [RelayCommand]
    public void Reset()
    {
        Namespace = "MyNamespace";
        UseRequired = true;
        UseNullableReferenceTypes = true;
        DateTypes.SelectValue(DateType.Dateonly);
        TimeTypes.SelectValue(TimeType.Timeonly);
        DateTimeTypes.SelectValue(DateTimeType.Datetimeoffset);
        Accesmodifers.SelectValue(AccessModifier.Public);
        ClassStyles.SelectValue(ClassStyle.Record);
        JsonLibaries.SelectValue(JsonLibary.Systemtext);
    }

    [RelayCommand]
    public async Task Generate()
    {
        var options = new JsonSchemaToCsharpOptions
        {
            Namespace = Namespace,
            UseRequired = UseRequired,
            Nullable = UseNullableReferenceTypes,
            DateType = DateTypes.SelectedValue,
            TimeType = TimeTypes.SelectedValue,
            DateTimeType = DateTimeTypes.SelectedValue,
            AccessModifier = Accesmodifers.SelectedValue,
            ClassStyle = ClassStyles.SelectedValue,
            JsonLibary = JsonLibaries.SelectedValue,
        };

        var generated = await _client.SchemaToCode.JsonSchemaToCsharpAsync(Json, options);

        generated.Map(success =>
        {
            CsharpCode = success;
        },
        failure =>
        {
            _dialogs.ClientError(failure);
        });
    }
}
