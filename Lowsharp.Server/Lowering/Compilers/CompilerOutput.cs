namespace Lowsharp.Server.Lowering.Compilers;

public sealed record class CompilerOutput(bool Success, LoweringDiagnostic[] Diagnostics);
