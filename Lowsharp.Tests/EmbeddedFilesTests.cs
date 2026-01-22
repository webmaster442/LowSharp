using Lowsharp.Server.Infrastructure;

namespace Lowsharp.Tests;

[TestFixture]
internal class EmbeddedFilesTests
{
    [TestCase("", "")]
    [TestCase("Lowsharp.Server.Infrastructure.Files.Foo.js", "Foo.js")]
    public void Test_GetEmbeddedFileName(string input, string expected)
    {
        var result = EmbeddedFiles.GetEmbeddedFileName(input);
        Assert.That(result, Is.EqualTo(expected));
    }
}
