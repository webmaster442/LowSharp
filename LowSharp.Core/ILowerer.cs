
namespace LowSharp.Core;

public interface ILowerer
{
    Task<string?> CreateExport(LowerRequest request, CancellationToken cancellationToken);
    IOrderedEnumerable<ComponentVersion> GetComponentVersions();
    Task<LowerResponse> ToLowerCodeAsync(LowerRequest request, CancellationToken cancellationToken);
}