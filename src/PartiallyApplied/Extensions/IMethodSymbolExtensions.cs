﻿using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Extensions
{
	internal static class IMethodSymbolExtensions
	{
		private const int MaximumParameterCount = 16;

		internal static bool AreEqual(this IMethodSymbol self, IMethodSymbol other)
		{
			if ((self.ReturnsVoid && !other.ReturnsVoid) ||
				(!self.ReturnsVoid && other.ReturnsVoid) ||
				(!self.ReturnsVoid && !other.ReturnsVoid && !self.ReturnType.Equals(other.ReturnType, SymbolEqualityComparer.Default)))
			{
				return false;
			}

			if (self.Parameters.Length != other.Parameters.Length)
			{
				return false;
			}

			for (var i = 0; i < self.Parameters.Length; i++)
			{
				if (!self.Parameters[i].Type.Equals(other.Parameters[i].Type, SymbolEqualityComparer.Default))
				{
					return false;
				}
			}

			return true;
		}

		// Can we build Apply() using Func or Action.
		internal static bool IsStandard(this IMethodSymbol self)
		{
			if (self.Parameters.Length > IMethodSymbolExtensions.MaximumParameterCount)
			{
				return false;
			}

			if (self.Parameters.Any(_ => _.Type.IsRefLikeType || _.HasExplicitDefaultValue))
			{
				return false;
			}

			if (!self.ReturnsVoid && (self.ReturnsByRef || self.ReturnsByRefReadonly))
			{
				return false;
			}

			return true;
		}
	}
}