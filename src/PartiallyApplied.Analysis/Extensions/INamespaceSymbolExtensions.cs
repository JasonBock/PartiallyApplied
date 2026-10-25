using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Extensions;

internal static class INamespaceSymbolExtensions
{
	extension(INamespaceSymbol? self)
	{
		internal string GetName() =>
			self?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat) ?? string.Empty;
	}
}