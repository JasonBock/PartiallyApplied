﻿using PartiallyApplied.Extensions;
using System.CodeDom.Compiler;
using System.Linq;
using System.Threading;

namespace PartiallyApplied.Builders
{
	internal static class CustomDelegateBuilder
	{
		private static int id;

		internal static void Build(PartiallyAppliedInformationResult result,
			IndentedTextWriter writer, NamespaceGatherer gatherer)
		{
			var target = result.Target;
			gatherer.AddRange(target.Parameters.Select(_ => _.Type.ContainingNamespace));

			if (!target.ReturnsVoid)
			{
				gatherer.Add(target.ReturnType.ContainingNamespace);
			}

			var id = Interlocked.Increment(ref CustomDelegateBuilder.id);
			var targetDelegateName = $"Target_{id}_Delegate";
			var targetDelegateParameters = string.Join(", ", target.Parameters.Select(_ => $"{_.Type.GetName()} {_.Name}"));
			var applyDelegateName = $"Apply_{id}_Delegate";
			var applyDelegateParameters = string.Join(", ", target.Parameters.Skip(result.PartialArgumentCount).Select(_ => $"{_.Type.GetName()} {_.Name}"));
			var applyParameters = string.Join(", ", target.Parameters.Take(result.PartialArgumentCount).Select(_ => $"{_.Type.GetName()} {_.Name}"));

			writer.WriteLine($"public delegate {target.ReturnType.GetName()} {targetDelegateName}({targetDelegateParameters});");
			writer.WriteLine($"public delegate {target.ReturnType.GetName()} {applyDelegateName}({applyDelegateParameters});");

			writer.WriteLine($"public static {applyDelegateName} {Naming.PartiallyMethodName}({targetDelegateName} method, {applyParameters}) =>");
			writer.Indent++;
			var remainingParameters = string.Join(", ", target.Parameters.Skip(result.PartialArgumentCount).Select(_ => _.Name));
			var allParameters = string.Join(", ", target.Parameters.Select(_ => _.Name));
			var refReturn = target.ReturnsByRef ? "ref " :
				target.ReturnsByRefReadonly ? "ref readonly " : string.Empty;
			writer.WriteLine($"new(({remainingParameters}) => {refReturn}method({allParameters}));");
			writer.Indent--;
		}
	}
}