using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Extensions;

internal static class ITypeSymbolExtensions
{
	extension(ITypeSymbol self)
	{
		internal string GetName() =>
			self.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

		internal string GetFullyQualifiedName(Compilation compilation, bool includeNullableAnnotation = true)
		{
			const string GlobalPrefix = "global::";

			var symbolFormatter = SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(
					SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

			if (includeNullableAnnotation)
			{
				symbolFormatter = symbolFormatter.AddMiscellaneousOptions(
					SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
			}
			else
			{
				symbolFormatter = symbolFormatter.RemoveMiscellaneousOptions(
					SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
			}

			var symbolName = self.ToDisplayString(symbolFormatter);

			if (!includeNullableAnnotation && symbolName.EndsWith("?"))
			{
				symbolName = symbolName.Substring(0, symbolName.Length - 1);
			}

			// If the symbol name has "global::" at the start,
			// then see if the type's assembly has at least one alias.
			// If there is one, then replace "global::" with "{alias}::",
			// but only the FIRST "global::"

			// TODO: self could be a closed generic where the
			// type arguments need aliases. I should add a test for that to see
			// what ToDisplayString() would do in that case.

			if (symbolName.StartsWith(GlobalPrefix))
			{
				var aliases = compilation.GetMetadataReference(self.ContainingAssembly)?.Properties.Aliases ?? [];

				if (aliases.Length > 0)
				{
					symbolName = $"{aliases[0]}::{symbolName.Remove(0, GlobalPrefix.Length)}";
				}
			}

			return symbolName;
		}
	}
}