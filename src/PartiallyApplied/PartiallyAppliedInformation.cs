using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PartiallyApplied.Diagnostics;
using PartiallyApplied.Extensions;
using System.Collections.Immutable;

namespace PartiallyApplied;

public sealed class PartiallyAppliedInformation
{
	private readonly Compilation compilation;
	private readonly InvocationExpressionSyntax target;

	public PartiallyAppliedInformation(InvocationExpressionSyntax target, Compilation compilation)
	{
		(this.target, this.compilation) = (target, compilation);
		this.Validate();
	}

	private void Validate()
	{
		var diagnostics = new List<Diagnostic>();

		if (this.target.ArgumentList.Arguments.Count < 2)
		{
			diagnostics.Add(IncorrectApplyArgumentCountDiagnostic.Create(this.target));
		}
		else
		{
			var model = this.compilation.GetSemanticModel(this.target.SyntaxTree);
			var arguments = this.target.ArgumentList.Arguments;
			var delegateArgument = arguments[0];
			var (delegateSymbol, wasFound) = delegateArgument.Expression.TryGetMethodSymbol(model);

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
						diagnostics.Add(TooManyArgumentsDiagnostic.Create(this.target));
					}
					else
					{
						var partialArgumentCount = arguments.Count - 1;

						if (delegateSymbol.Parameters.Any(_ => _.RefKind == RefKind.Ref ||
							_.RefKind == RefKind.Out || _.RefKind == RefKind.In))
						{
							diagnostics.Add(UnsupportedParameterModifiersDiagnostic.Create(this.target));
						}

						if (delegateSymbol.Parameters.Take(partialArgumentCount).Any(_ => _.Type.IsRefLikeType))
						{
							diagnostics.Add(CannotPartiallyApplyRefStructDiagnostic.Create(this.target));
						}

						if (diagnostics.Count == 0)
						{
							var applyName = (this.target.Expression as MemberAccessExpressionSyntax)!.Name.Identifier.Text;
							this.Result = new(delegateSymbol, partialArgumentCount, applyName);
						}
					}
				}
			}
		}

		this.Diagnostics = diagnostics.ToImmutableArray();
	}

	public ImmutableArray<Diagnostic> Diagnostics { get; private set; }
	public PartiallyAppliedInformationResult? Result { get; private set; }
}

//public sealed class PartiallyAppliedInformation
//{
//	private readonly ImmutableArray<InvocationExpressionSyntax> candidates;
//	private readonly Compilation compilation;

//	public PartiallyAppliedInformation(ImmutableArray<InvocationExpressionSyntax> candidates, Compilation compilation)
//	{
//		(this.candidates, this.compilation) = (candidates, compilation);
//		this.Validate();
//	}

//	private void Validate()
//	{
//		var diagnostics = new List<Diagnostic>();
//		var results = new List<PartiallyAppliedInformationResult>(this.candidates.Length);

//		foreach (var candidate in this.candidates)
//		{
//			var model = this.compilation.GetSemanticModel(candidate.SyntaxTree);

//			if (candidate.ArgumentList.Arguments.Count < 2)
//			{
//				diagnostics.Add(IncorrectApplyArgumentCountDiagnostic.Create(candidate));
//			}
//			else
//			{
//				var arguments = candidate.ArgumentList.Arguments;
//				var delegateArgument = arguments[0];
//				var (delegateSymbol, wasFound) = delegateArgument.Expression.TryGetMethodSymbol(model);

//				if (delegateSymbol is null)
//				{
//					diagnostics.Add(NoTargetMethodFoundDiagnostic.Create(delegateArgument));
//				}
//				else if (!wasFound)
//				{
//					if (delegateSymbol.Parameters.Length < 2)
//					{
//						diagnostics.Add(MinimalParameterCountNotMetDiagnostic.Create(delegateArgument));
//					}
//					else
//					{
//						if (arguments.Count > delegateSymbol.Parameters.Length)
//						{
//							diagnostics.Add(TooManyArgumentsDiagnostic.Create(candidate));
//						}
//						else
//						{
//							var partialArgumentCount = arguments.Count - 1;

//							if (delegateSymbol.Parameters.Any(_ => _.RefKind == RefKind.Ref ||
//								_.RefKind == RefKind.Out || _.RefKind == RefKind.In))
//							{
//								diagnostics.Add(UnsupportedParameterModifiersDiagnostic.Create(candidate));
//							}

//							if (delegateSymbol.Parameters.Take(partialArgumentCount).Any(_ => _.Type.IsRefLikeType))
//							{
//								diagnostics.Add(CannotPartiallyApplyRefStructDiagnostic.Create(candidate));
//							}

//							if (diagnostics.Count == 0)
//							{
//								var applyName = (candidate.Expression as MemberAccessExpressionSyntax)!.Name.Identifier.Text;
//								results.Add(new(delegateSymbol, partialArgumentCount, applyName));
//							}
//						}
//					}
//				}
//			}
//		}

//		this.Diagnostics = diagnostics.ToImmutableArray();
//		this.Results = results.ToImmutableArray();
//	}

//	public ImmutableArray<Diagnostic> Diagnostics { get; private set; }
//	public ImmutableArray<PartiallyAppliedInformationResult> Results { get; private set; }
//}