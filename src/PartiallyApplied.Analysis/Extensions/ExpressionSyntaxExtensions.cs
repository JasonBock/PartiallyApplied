using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace PartiallyApplied.Extensions;

internal static class ExpressionSyntaxExtensions
{
	extension(ExpressionSyntax self)
	{
		internal ImmutableArray<(IMethodSymbol?, bool)> TryGetMethodSymbols(SemanticModel model)
		{
			var symbols = new List<(IMethodSymbol?, bool)>();

			var symbol = model.GetSymbolInfo(self);

			if (symbol.Symbol is IMethodSymbol methodSymbol)
			{
				symbols.Add((methodSymbol, true));
			}
			else if (model.GetTypeInfo(self).Type is INamedTypeSymbol { TypeKind: TypeKind.Delegate, DelegateInvokeMethod: { } delegateMethodSymbol })
			{
				symbols.Add((delegateMethodSymbol, false));
			}
			else if (symbol.CandidateSymbols.Length > 0)
			{
				// TODO: Do I ever actually run into more than 1
				// candidate symbol?
				foreach (var candidate in symbol.CandidateSymbols)
				{
					if (candidate is IMethodSymbol methodFromCandidateSymbol)
					{
						symbols.Add((methodFromCandidateSymbol, false));
					}
				}
			}
			else
			{
				symbols.Add((null, false));
			}

			return [.. symbols];
		}
	}
}