using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PartiallyApplied.Diagnostics;
using PartiallyApplied.Extensions;

namespace PartiallyApplied.Analysis.Models;

internal sealed class AppliedInvocationModel
{
	private AppliedInvocationModel(MethodModel? methodModel, EquatableArray<Diagnostic> diagnostics) =>
		(this.Invocation, this.Diagnostics) = (methodModel, diagnostics);

	internal static AppliedInvocationModel Create(InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel)
	{
		var diagnostics = new List<Diagnostic>();
		var results = new List<PartiallyAppliedInformationResult>();

		if (invocationExpression.ArgumentList.Arguments.Count < 2)
		{
			diagnostics.Add(IncorrectApplyArgumentCountDiagnostic.Create(invocationExpression));
		}
		else
		{
			var arguments = invocationExpression.ArgumentList.Arguments;
			var delegateArgument = arguments[0];

			foreach (var (delegateSymbol, wasFound) in delegateArgument.Expression.TryGetMethodSymbols(semanticModel))
			{
				if (delegateSymbol is null)
				{
					diagnostics.Add(NoTargetMethodFoundDiagnostic.Create(delegateArgument));
				}
				else if (!wasFound)
				{
					if (delegateSymbol.Parameters.Length < 2)
					{
						diagnostics.Add(MinimalParameterCountNotMetDiagnostic.Create(delegateArgument));
					}
					else
					{
						if (arguments.Count > delegateSymbol.Parameters.Length)
						{
							diagnostics.Add(TooManyArgumentsDiagnostic.Create(invocationExpression));
						}
						else
						{
							var partialArgumentCount = arguments.Count - 1;

							if (delegateSymbol.Parameters.Any(_ => _.RefKind == RefKind.Ref ||
								_.RefKind == RefKind.Out || _.RefKind == RefKind.In))
							{
								diagnostics.Add(UnsupportedParameterModifiersDiagnostic.Create(invocationExpression));
							}

							if (delegateSymbol.Parameters.Take(partialArgumentCount).Any(_ => _.Type.IsRefLikeType))
							{
								diagnostics.Add(CannotPartiallyApplyRefStructDiagnostic.Create(invocationExpression));
							}

							if (diagnostics.Count == 0)
							{
								var applyName = (invocationExpression.Expression as MemberAccessExpressionSyntax)!.Name.Identifier.Text;
								results.Add(new(delegateSymbol, partialArgumentCount, applyName));
							}
						}
					}
				}
			}
		}
	}

	internal MethodModel? Invocation { get; }

	internal EquatableArray<Diagnostic> Diagnostics { get; }
}