using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Extensions;

internal static class ITypeSymbolExtensions
{
	internal static string GetName(this ITypeSymbol self) =>
		self.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
}