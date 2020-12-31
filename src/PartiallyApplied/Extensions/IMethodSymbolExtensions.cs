using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Extensions
{
	public static class IMethodSymbolExtensions
	{
		private const int MaximumParameterCount = 16;

		public static bool AreEqual(this IMethodSymbol self, IMethodSymbol other)
		{
			if (!self.ReturnType.Equals(other.ReturnType))
			{
				return false;
			}

			if (self.Parameters.Length != other.Parameters.Length)
			{
				return false;
			}

			for (var i = 0; i < self.Parameters.Length; i++)
			{
				if (!self.Parameters[i].Type.Equals(self.Parameters[i].Type))
				{
					return false;
				}
			}

			return true;
		}

		public static bool IsStandard(this IMethodSymbol self)
		{
			if (self.Parameters.Length > IMethodSymbolExtensions.MaximumParameterCount)
			{
				return false;
			}

			foreach (var parameter in self.Parameters)
			{
				if (parameter.RefKind == RefKind.Ref || parameter.RefKind == RefKind.Out ||
					parameter.RefKind == RefKind.In)
				{
					return false;
				}
			}

			if (!self.ReturnsVoid && (self.ReturnsByRef || self.ReturnsByRefReadonly))
			{
				return false;
			}

			return true;
		}
	}
}