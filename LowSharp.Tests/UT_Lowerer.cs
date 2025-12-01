using LowSharp.Core;

namespace LowSharp.Tests;

[TestFixture]
public class UT_Lowerer
{
    private Lowerer _sut;

    [SetUp]
    public void Setup()
    {
        _sut = new Lowerer();
    }

    [Test]
    public async Task EnsureThat_Lowerer_ToLowerCodeAsync_WorksCsharp()
    {
        var code = """
            using System;
            
            public class C {
                public void M() {
                }
            }
            """;

        var result = await _sut.ToLowerCodeAsync(new LowerRequest
        {
            InputLanguage = InputLanguage.Csharp,
            Code = code,
        }, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasErrors, Is.False);
            Assert.That(result.Diagnostics, Is.Empty);
            Assert.That(result.LoweredCode, Is.Not.Null.And.Not.Empty);
        }
    }

    [Test]
    public async Task EnsureThat_Lowerer_ToLowerCodeAsync_WorksVisualBasic()
    {
        var code = """
            Imports System
            Public Class C
                Public Sub M()
                End Sub
            End Class
            """;

        var result = await _sut.ToLowerCodeAsync(new LowerRequest
        {
            InputLanguage = InputLanguage.VisualBasic,
            Code = code,
        }, CancellationToken.None);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.HasErrors, Is.False);
            Assert.That(result.Diagnostics, Is.Empty);
            Assert.That(result.LoweredCode, Is.Not.Null.And.Not.Empty);
        }
    }
}
