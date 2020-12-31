using System.CodeDom.Compiler;

namespace PartiallyApplied.Builders
{
	internal static class CustomDelegateBuilder
	{
		internal static void Build(PartiallyAppliedInformation information,
			IndentedTextWriter writer, NamespaceGatherer gatherer)
		{
			// TODO: Eventually, we're going to need to do this work:
			// * We only generate target delegates for distinct methods.
			// * We generate return delegates for distinct "method + count".
			// * We generate Apply() methods for distinct "method + count".
			// However, for now, we're only going to work with results where the 
			// target method can be represented as a Func<> or Action<>.

			/*
			var targetDelegates = this.information.Results.Select(_ => _.Target)
				.Distinct(IMethodSymbolEqualityComparer.Default);
			var applyDelegates = this.information.Results.Distinct();
			*/

			/*
			foreach(var targetDelegate in targetDelegates)
			{
				var targetDelegateText =
					$"public delegate {targetDelegate.ReturnType.Name} {Naming.TargetDelegateName} ({string.Join(", ", targetDelegate.Parameters.Select(_ => _.Type.Name + " " + _.Name))});";
				var targetDelegateIdentifier = targetDelegateText.GetHashCode();
			}
			*/
		}
	}
}