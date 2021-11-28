﻿using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Extensions;

internal static class INamespaceSymbolExtensions
{
	internal static string GetName(this INamespaceSymbol? self) =>
		self?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat) ?? string.Empty;
}