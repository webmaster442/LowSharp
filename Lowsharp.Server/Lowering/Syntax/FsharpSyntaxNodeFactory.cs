using FSharp.Compiler.Syntax;

namespace Lowsharp.Server.Lowering.Syntax;

internal static class FsharpSyntaxNodeFactory
{
    public static NodeOrTokenDto Create(ParsedInput parseTree, string code)
    {
        return parseTree switch
        {
            ParsedInput.ImplFile implFile => ConvertImplFile(implFile, code),
            ParsedInput.SigFile sigFile => ConvertSigFile(sigFile, code),
            _ => new NodeOrTokenDto
            {
                Kind = "UnknownParsedInput",
                Children = []
            }
        };
    }

    private static NodeOrTokenDto ConvertImplFile(ParsedInput.ImplFile implFile, string sourceCode)
    {
        List<NodeOrTokenDto> children = [];

        foreach (SynModuleOrNamespace module in implFile.Item.Contents)
        {
            children.Add(ConvertModuleOrNamespace(module, sourceCode));
        }

        return new NodeOrTokenDto
        {
            Kind = "ImplFile",
            Text = implFile.QualifiedName.Text,
            Children = children
        };
    }

    private static NodeOrTokenDto ConvertSigFile(ParsedInput.SigFile sigFile, string sourceCode)
    {
        return new NodeOrTokenDto
        {
            Kind = "SigFile",
            Text = sigFile.QualifiedName.Text,
            Children = []
        };
    }

    private static NodeOrTokenDto ConvertModuleOrNamespace(SynModuleOrNamespace module, string sourceCode)
    {
        List<NodeOrTokenDto> children = [];

        foreach (SynModuleDecl decl in module.decls)
        {
            children.Add(ConvertModuleDecl(decl, sourceCode));
        }

        return new NodeOrTokenDto
        {
            Kind = "ModuleOrNamespace",
            Text = string.Join(".", module.longId.Select(id => id.idText)),
            Children = children
        };
    }

    private static NodeOrTokenDto ConvertModuleDecl(SynModuleDecl decl, string sourceCode)
    {
        return decl switch
        {
            SynModuleDecl.Let letDecl => new NodeOrTokenDto
            {
                Kind = "LetDeclaration",
                Text = letDecl.isRecursive ? "rec" : null,
                Children = letDecl.bindings.Select(b => ConvertBinding(b, sourceCode)).ToList()
            },
            SynModuleDecl.Types typesDecl => new NodeOrTokenDto
            {
                Kind = "TypeDeclarations",
                Children = typesDecl.typeDefns.Select(t => ConvertTypeDefn(t, sourceCode)).ToList()
            },
            SynModuleDecl.Open openDecl => new NodeOrTokenDto
            {
                Kind = "OpenDeclaration",
                Text = GetRangeText(openDecl.range, sourceCode),
                Children = []
            },
            SynModuleDecl.NestedModule nestedModule => new NodeOrTokenDto
            {
                Kind = "NestedModule",
                Text = string.Join(".", nestedModule.moduleInfo.longId.Select(id => id.idText)),
                Children = nestedModule.decls.Select(d => ConvertModuleDecl(d, sourceCode)).ToList()
            },
            _ => new NodeOrTokenDto
            {
                Kind = $"Unhandled_{decl.GetType().Name}",
                Text = $"Not yet implemented: {decl.GetType().Name}",
                Children = []
            }
        };
    }

    private static NodeOrTokenDto ConvertBinding(SynBinding binding, string sourceCode)
    {
        return new NodeOrTokenDto
        {
            Kind = "Binding",
            Text = GetRangeText(binding.RangeOfBindingWithRhs, sourceCode),
            Children = []
        };
    }

    private static NodeOrTokenDto ConvertTypeDefn(SynTypeDefn typeDefn, string sourceCode)
    {
        return new NodeOrTokenDto
        {
            Kind = "TypeDefinition",
            Text = typeDefn.typeInfo.ToString(),
            Children = []
        };
    }


    private static string? GetRangeText(FSharp.Compiler.Text.Range range, string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
            return null;

        try
        {
            string[] lines = sourceCode.Split('\n');
            if (range.StartLine > lines.Length || range.EndLine > lines.Length)
                return null;

            if (range.StartLine == range.EndLine)
            {
                string line = lines[range.StartLine - 1];
                int start = Math.Min(range.StartColumn, line.Length);
                int end = Math.Min(range.EndColumn, line.Length);
                return line.Substring(start, end - start);
            }

            return $"[{range.StartLine}:{range.StartColumn}-{range.EndLine}:{range.EndColumn}]";
        }
        catch
        {
            return null;
        }
    }
}
