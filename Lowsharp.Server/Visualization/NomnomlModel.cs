namespace Lowsharp.Server.Visualization;

internal sealed class NomnomlModel
{
    public string GraphereUrl { get; }

    public string NomnomlUrl { get; }

    public string Code { get; }

    public NomnomlModel(string serverUrl, string code)
    {
        GraphereUrl = $"{serverUrl}/script/graphere.js";
        NomnomlUrl = $"{serverUrl}/script/nomnoml.js";
        Code = code;
    }
}
