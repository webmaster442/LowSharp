
using LowSharp.ApiV1.Regex;

namespace LowSharp.ClientLib;

public interface IRegexClient
{
    Task<Either<RegexMatchResponse, Exception>> MatchAsync(string input,
                                                           string pattern,
                                                           RegexOptions options,
                                                           CancellationToken cancellation = default);
    Task<Either<RegexReplaceResponse, Exception>> ReplaceAsync(string input,
                                                               string replacement,
                                                               string pattern,
                                                               RegexOptions options,
                                                               CancellationToken cancellation = default);
    Task<Either<RegexSplitResponse, Exception>> SplitAsync(string input,
                                                           string pattern,
                                                           RegexOptions options,
                                                           CancellationToken cancellation = default);
}
