using System.Runtime.CompilerServices;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;

using Microsoft.IO;

namespace LowSharp.Core.Internals;

internal sealed class CsharpDecompiler : IDecompiler
{
    private readonly CSharpFormattingOptions _formattingOptions;
    private readonly DecompilerSettings _decompilerSettings;
    private readonly IAssemblyResolver _assemblyResolver;

    public CsharpDecompiler()
    {
        _formattingOptions = FormattingOptionsFactory.CreateAllman();
        _formattingOptions.IndentationString = new string(' ', 4);
        _formattingOptions.MinimumBlankLinesAfterUsings = 1;
        _formattingOptions.MinimumBlankLinesBetweenTypes = 1;

        _decompilerSettings = new DecompilerSettings(LanguageVersion.CSharp1)
        {
            ArrayInitializers = false,
            AutomaticEvents = false,
            DecimalConstants = false,
            FixedBuffers = false,
            UsingStatement = false,
            SwitchStatementOnString = false,
            LockStatement = false,
            ForStatement = false,
            ForEachStatement = false,
            SparseIntegerSwitch = false,
            DoWhileStatement = false,
            StringConcat = false,
            UseRefLocalsForAccurateOrderOfEvaluation = true,
            InitAccessors = true,
            FunctionPointers = true,
            NativeIntegers = true,
            SwitchExpressions = false,
            RecordClasses = false,
            RecordStructs = false,
            DecompileMemberBodies = true,
            Deconstruction = false,
            DictionaryInitializers = false,
            ExpandMemberDefinitions = true,
            UseDebugSymbols = true,
        };

        _assemblyResolver = new UniversalAssemblyResolver("", false, null);
    }

    private class ExtendedCSharpOutputVisitor(TextWriter textWriter, CSharpFormattingOptions formattingPolicy)
        : CSharpOutputVisitor(textWriter, formattingPolicy)
    {
        public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            base.VisitTypeDeclaration(typeDeclaration);
            if (typeDeclaration.NextSibling is NamespaceDeclaration or TypeDeclaration)
            {
                NewLine();
            }
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
        {
            base.VisitNamespaceDeclaration(namespaceDeclaration);
            if (namespaceDeclaration.NextSibling is NamespaceDeclaration or TypeDeclaration)
            {
                NewLine();
            }
        }

        public override void VisitAttributeSection(AttributeSection attributeSection)
        {
            base.VisitAttributeSection(attributeSection);
            if (attributeSection is { AttributeTarget: "assembly" or "module", NextSibling: not AttributeSection { AttributeTarget: "assembly" or "module" } })
            {
                NewLine();
            }
        }
    }

    private static void SortTree(SyntaxTree root)
    {
        // Note: the sorting logic cannot be reused, but should match IL and Jit ASM ordering
        var firstMovedNode = (AstNode?)null;
        foreach (var node in root.Children)
        {
            if (node == firstMovedNode)
                break;

            if (node is NamespaceDeclaration @namespace && IsNonUserCode(@namespace))
            {
                node.Remove();
                root.AddChildWithExistingRole(node);
                firstMovedNode ??= node;
            }
        }
    }

    private static bool IsNonUserCode(NamespaceDeclaration @namespace)
    {
        // Note: the logic cannot be reused, but should match IL and Jit ASM
        foreach (var member in @namespace.Members)
        {
            if (member is not TypeDeclaration type)
                return false;

            if (!IsCompilerGenerated(type))
                return false;
        }

        return true;
    }

    private static bool IsCompilerGenerated(TypeDeclaration type)
    {
        foreach (var section in type.Attributes)
        {
            foreach (var attribute in section.Attributes)
            {
                if (attribute.Type is SimpleType { Identifier: nameof(CompilerGeneratedAttribute) or "CompilerGenerated" })
                    return true;
            }
        }
        return false;
    }

    public bool TryDecompile(RecyclableMemoryStream assemblyStream, RecyclableMemoryStream pdbStream, out string result)
    {
        try
        {
            using var assemblyFile = new PEFile("", assemblyStream);
            using var pdbFile = new PdbDebugInfoProvider(pdbStream);

            var decompiler = new CSharpDecompiler(assemblyFile, _assemblyResolver, _decompilerSettings)
            {
                DebugInfoProvider = pdbFile
            };

            var syntaxTree = decompiler.DecompileWholeModuleAsSingleFile();
            SortTree(syntaxTree);

            using var codeWriter = new StringWriter();

            new ExtendedCSharpOutputVisitor(codeWriter, _formattingOptions)
                .VisitSyntaxTree(syntaxTree);

            result = codeWriter.ToString();
            return true;
        }
        catch (Exception ex)
        {
            result = ex.Message;
            return false;
        }
    }
}