using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Extensions
{
	public static class INamespaceSymbolExtensions
	{
		internal static string GetName(this INamespaceSymbol? self) =>
			self?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat) ?? string.Empty;
	}
}