using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PartiallyApplied.Diagnostics;
using PartiallyApplied.Extensions;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PartiallyApplied
{
	public sealed class PartiallyAppliedInformation
	{
		private readonly List<InvocationExpressionSyntax> candidates;
		private readonly Compilation compilation;

		public PartiallyAppliedInformation(List<InvocationExpressionSyntax> candidates, Compilation compilation)
		{
			(this.candidates, this.compilation) = (candidates, compilation);
			this.Validate();
		}

		private void Validate()
		{
			var diagnostics = new List<Diagnostic>();
			var results = new List<PartiallyAppliedInformationResult>(this.candidates.Count);

			foreach(var candidate in this.candidates)
			{
				var model = this.compilation.GetSemanticModel(candidate.SyntaxTree);

				if(candidate.ArgumentList.Arguments.Count < 2)
				{
					diagnostics.Add(IncorrectApplyArgumentCountDiagnostics.Create(candidate));
				}
				else
				{
					var arguments = candidate.ArgumentList.Arguments;
					var delegateArgument = arguments[0];
					var (delegateSymbol, wasFound) = delegateArgument.Expression.TryGetMethodSymbol(model);

					if(delegateSymbol is null)
					{
						diagnostics.Add(NoTargetMethodFoundDiagnostics.Create(delegateArgument));
					}
					else if(!wasFound)
					{
						if(delegateSymbol.Parameters.Length < 2)
						{
							diagnostics.Add(MinimalParameterCountNotMetDiagnostics.Create(delegateArgument));
						}
						else
						{
							if (arguments.Count > delegateSymbol.Parameters.Length)
							{
								diagnostics.Add(TooManyArgumentsDiagnostics.Create(candidate));
							}
							else
							{
								var partialArgumentCount = arguments.Count - 1;
								var addedDiagnostic = false;

								for (var i = 0; i < delegateSymbol.Parameters.Length; i++)
								{
									var delegateParameter = delegateSymbol.Parameters[i];

									if(delegateParameter.RefKind == RefKind.Ref || delegateParameter.RefKind == RefKind.Out ||
										delegateParameter.RefKind == RefKind.In)
									{
										diagnostics.Add(UnsupportedParameterModifiersDiagnostics.Create(candidate));
										addedDiagnostic = true;
									}
									else if(i < partialArgumentCount && delegateParameter.Type.IsRefLikeType)
									{
										diagnostics.Add(CannotPartiallyApplyRefStructDiagnostics.Create(candidate));
										addedDiagnostic = true;
									}
								}

								if(!addedDiagnostic)
								{
									var applyName = ((candidate.Expression as MemberAccessExpressionSyntax)!.Name as IdentifierNameSyntax)!.Identifier.Text;
									results.Add(new(delegateSymbol, partialArgumentCount, applyName));
								}
							}
						}
					}
				}
			}

			this.Diagnostics = diagnostics.ToImmutableArray();
			this.Results = results.ToImmutableArray();
		}

		public ImmutableArray<Diagnostic> Diagnostics { get; private set; }
		public ImmutableArray<PartiallyAppliedInformationResult> Results { get; private set; }
	}
}