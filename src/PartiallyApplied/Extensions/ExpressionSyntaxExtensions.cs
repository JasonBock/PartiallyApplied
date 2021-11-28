using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Extensions;

internal static class ExpressionSyntaxExtensions
{
	internal static (IMethodSymbol?, bool) TryGetMethodSymbol(this ExpressionSyntax self, SemanticModel model)
	{
		var symbol = model.GetSymbolInfo(self);

		if (symbol.Symbol is IMethodSymbol methodSymbol)
		{
			return (methodSymbol, true);
		}
		else if (symbol.CandidateSymbols.Length > 0 && symbol.CandidateSymbols[0] is IMethodSymbol methodFromCandidateSymbol)
		{
			return (methodFromCandidateSymbol, false);
		}
		else
		{
			return (null, false);
		}
	}
}