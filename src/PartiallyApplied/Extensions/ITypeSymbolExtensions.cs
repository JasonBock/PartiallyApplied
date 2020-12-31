using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Extensions
{
	public static class ITypeSymbolExtensions
	{
		public static string GetName(this ITypeSymbol self) =>
			self.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
	}
}