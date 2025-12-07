namespace LowSharp.Cli.Highlighter;

internal sealed class VisualBasicHighlighter : KeywordHighlighter
{
    protected override IEqualityComparer<string> Comparision => throw new NotImplementedException();

    protected override IEnumerable<string> GetKeywords()
    {
        return
        [
            "#Const", "#Else", "#ElseIf", "#End", "#If", "&", "&=", "*", "*=", "+", "+=", "-", "-=",
            "/", "/=", "<<", "<<=", "=", ">>", ">>=", "AddHandler", "AddressOf", "Alias", "And", "AndAlso",
            "As", "Boolean", "ByRef", "ByVal", "Byte", "CBool", "CByte", "CChar", "CDate", "CDbl", "CDec", 
            "CInt", "CLng", "CObj", "CSByte", "CShort", "CSng", "CStr", "CType", "CUInt", "CULng", "CUShort",
            "Call", "Case", "Catch", "Char", "Class", "Const", "Continue", "Date", "Decimal", "Declare", "Default",
            "Delegate", "Dim", "DirectCast", "Do", "Double", "Each", "Each", "Else", "ElseIf", "End", "EndIf", "Enum", 
            "Erase", "Error", "Event", "Exit", "False", "Finally", "For", "Friend", "Function", "Get", "GetType", 
            "GetXMLNamespace", "Global", "GoSub", "GoTo", "Handles", "If", "Implements", "Imports", "In", "Inherits",
            "Integer", "Interface", "Is", "Is", "IsNot", "Let", "Lib", "Like", "Long", "Loop", "Me", "Mod", "Module", 
            "MustInherit", "MustOverride", "MyBase", "MyClass", "NameOf", "Namespace", "Narrowing", "New", "Next",
            "Not", "NotInheritable", "NotOverridable", "Nothing", "Object", "Of", "On",  "Operator", "Option", "Optional",
            "Or", "OrElse", "Out", "Overloads", "Overridable", "Overrides", "ParamArray", "Partial", "Private", "Property",
            "Protected", "Public", "REM", "RaiseEvent", "ReDim", "ReadOnly", "RemoveHandler", "Resume", "Return", "SByte",
            "Select", "Set", "Shadows", "Shared", "Short", "Single", "Static", "Step", "Stop", "String", "Structure", "Sub",
            "SyncLock", "Then", "Throw", "To", "True", "Try", "TryCast", "TypeOf", "UInteger", "ULong", "UShort", "Using",
            "Variant", "Wend", "When", "While", "Widening", "With", "WithEvents", "WriteOnly", "Xor", @"\", @"\=", "^", "^="
        ];
    }
}