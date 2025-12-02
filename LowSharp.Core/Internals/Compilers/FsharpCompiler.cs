using FSharp.Compiler.CodeAnalysis;
using FSharp.Compiler.Diagnostics;

using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using Microsoft.IO;

namespace LowSharp.Core.Internals.Compilers;

internal sealed class FsharpCompiler
{
    private readonly FSharpChecker _compiler;

    public FsharpCompiler()
    {
        _compiler = FSharpChecker.Create(projectCacheSize: null,
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

    public async Task<(bool result, IEnumerable<LoweringDiagnostic> diagnostics)> Compile(string code,
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

        return (errors.Length == 0, errors.Where(e => !e.Severity.IsHidden).Select(Mappers.ToLoweringDiagnostic));

    }
}
