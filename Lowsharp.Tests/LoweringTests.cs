using Lowsharp.Server.CodeExamples;
using Lowsharp.Server.Lowering;

namespace Lowsharp.Tests;

[TestFixture]
public class LoweringTests
{
    private LoweringEngine _sut;

    public static IEnumerable<TestCaseData> LoweringExamples
    {
        get
        {
            ExampleProvider exampleProvider = new ExampleProvider();
            foreach (var example in exampleProvider)
            {
                yield return new TestCaseData(example.Value, example.Lang);
            }
        }
    }

    private static EngineInput CreateInput(string code, string language, OutputLanguage outputLanguage)
    {
        return new EngineInput
        {
            Code = code,
            InputLanguage = language switch
            {
                "csharp" => InputLanguage.Csharp,
                "vb" => InputLanguage.VisualBasic,
                "fsharp" => InputLanguage.FSharp,
                _ => throw new NotSupportedException($"Language '{language}' is not supported.")
            },
            OutputLanguage = outputLanguage,
            OutputOptimizationLevel = OutputOptimizationLevel.Debug
        };
    }

    [SetUp]
    public void Setup()
    {
        _sut = new LoweringEngine();
    }

    [TestCaseSource(nameof(LoweringExamples))]
    public async Task TestLowering_ToIL(string code, string language)
    {
        var results = await _sut.ToLowerCodeAsync(CreateInput(code, language, OutputLanguage.IL), default);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(results.LoweredCode, Is.Not.Null.And.Not.Empty);
            Assert.That(results.Diagnostics, Is.Empty);
        }
    }

    [TestCaseSource(nameof(LoweringExamples))]
    public async Task TestLowering_ToCsharp(string code, string language)
    {
        var results = await _sut.ToLowerCodeAsync(CreateInput(code, language, OutputLanguage.Csharp), default);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(results.LoweredCode, Is.Not.Null.And.Not.Empty);
            Assert.That(results.Diagnostics, Is.Empty);
        }
    }

    [TestCaseSource(nameof(LoweringExamples))]
    public async Task TestLowering_Nomnoml(string code, string language)
    {
        var results = await _sut.ToLowerCodeAsync(CreateInput(code, language, OutputLanguage.NomnommlClassTree), default);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(results.LoweredCode, Is.Not.Null.And.Not.Empty);
            Assert.That(results.Diagnostics, Is.Empty);
        }
    }


    [TestCaseSource(nameof(LoweringExamples))]
    public async Task TestLowering_SyntaxTree(string code, string language)
    {
        var results = await _sut.ToLowerCodeAsync(CreateInput(code, language, OutputLanguage.SyntaxTreeJson), default);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(results.LoweredCode, Is.Not.Null.And.Not.Empty);
            Assert.That(results.Diagnostics, Is.Empty);
        }
    }
}
