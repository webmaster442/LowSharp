using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

using FSharp.Compiler.CodeAnalysis;
using FSharp.Compiler.Diagnostics;
using FSharp.Compiler.Syntax;
using FSharp.Compiler.Text;

using Lowsharp.Server.Lowering;
using Lowsharp.Server.Lowering.Syntax;

using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using Microsoft.IO;

namespace Lowsharp.Server.Lowering.Compilers;

internal sealed class FsharpCompiler : ICompiler
{
    private readonly FSharpChecker _compiler;

    public FsharpCompiler()
    {
        _compiler = FSharpChecker.Create(
            projectCacheSize: null,
            keepAssemblyContents: null,
            keepAllBackgroundResolutions: null,
            legacyReferenceResolver: null,
            tryGetMetadataSnapshot: null,
            suggestNamesForErrors: null,
            keepAllBackgroundSymbolUses: null,
            enableBackgroundItemKeyStoreAndSemanticClassification: null,
            enablePartialTypeChecking: null,
            parallelReferenceResolution: null,
            captureIdentifiersWhenParsing: null,
            documentSource: null,
            useTransparentCompiler: true,
            transparentCompilerCacheSizes: null);
    }

    public async Task<CompilerOutput> CompileAsync(string code,
                                                   OutputOptimizationLevel outputOptimizationLevel,
                                                   RecyclableMemoryStream assemblyStream,
                                                   RecyclableMemoryStream pdbStream,
                                                   CancellationToken cancellationToken)
    {
        using var souceTemp = new TempFile(".fs");
        using var targetTemp = new TempFile(".dll");
        using var pdbTemp = new TempFile(".pdb");

        await souceTemp.WriteAsync(code, cancellationToken);

        var aguments = new List<string>
        {
            "fsc.exe",
            "-o",
            $"\"{targetTemp.FullPath}\"",
            "-a",
            $"\"{souceTemp.FullPath}\"",
            "--debug+",
            $"--pdb:{pdbTemp.FullPath}"
        };

        switch (outputOptimizationLevel)
        {
            case OutputOptimizationLevel.Debug:
                aguments.Add("--optimize-");
                break;
            case OutputOptimizationLevel.Release:
                aguments.Add("--optimize+");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(outputOptimizationLevel), outputOptimizationLevel, null);
        }

        FSharpAsync<Tuple<FSharpDiagnostic[], FSharpOption<Exception>?>> compile = _compiler.Compile(aguments.ToArray(), null);

        var (errors, exceptionOpt) = await FSharpAsync.StartAsTask(compile, taskCreationOptions: null, cancellationToken: cancellationToken);

        if (pdbTemp.IsExisting)
            await pdbTemp.CopyTo(pdbStream, cancellationToken);

        if (targetTemp.IsExisting)
            await targetTemp.CopyTo(assemblyStream, cancellationToken);

        assemblyStream.Seek(0, SeekOrigin.Begin);
        pdbStream.Seek(0, SeekOrigin.Begin);

        var messages = errors
            .Where(e => !e.Severity.IsHidden)
            .Select(Mappers.ToLoweringDiagnostic)
            .ToArray();

        return new CompilerOutput(messages.Length == 0, messages);
    }

    public async Task<string> CompileToSyntaxTreeJsonAsync(string code, CancellationToken cancellationToken)
    {
        const string inputFileName = "InMemory.fs";

        try
        {
            FSharpParsingOptions parsingOptions = new FSharpParsingOptions(
                sourceFiles: [inputFileName],
                applyLineDirectives: FSharpParsingOptions.Default.ApplyLineDirectives,
                conditionalDefines: FSharpParsingOptions.Default.ConditionalDefines,
                diagnosticOptions: FSharpParsingOptions.Default.DiagnosticOptions,
                langVersionText: FSharpParsingOptions.Default.LangVersionText,
                isInteractive: FSharpParsingOptions.Default.IsInteractive,
                indentationAwareSyntax: FSharpParsingOptions.Default.IndentationAwareSyntax,
                strictIndentation: FSharpParsingOptions.Default.StrictIndentation,
                compilingFSharpCore: FSharpParsingOptions.Default.CompilingFSharpCore,
                isExe: FSharpParsingOptions.Default.IsExe);

            FSharpAsync<FSharpParseFileResults> parseResults = _compiler.ParseFile(inputFileName,
                                                                                   SourceText.ofString(code),
                                                                                   parsingOptions,
                                                                                   null,
                                                                                   null);

            FSharpParseFileResults results = await FSharpAsync.StartAsTask(parseResults,
                                                                           taskCreationOptions: null,
                                                                           cancellationToken: cancellationToken);

            NodeOrTokenDto rootNode = FsharpSyntaxNodeFactory.Create(results.ParseTree, code);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(rootNode, options);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
