namespace PartiallyApplied.Analysis.Models;

internal sealed record InvocationModel(string ApplyName, int PartialArgumentCount, MethodModel Target);