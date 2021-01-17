using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace PartiallyApplied
{
	public sealed class PartiallyAppliedReceiver
		: ISyntaxReceiver
	{
		public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
		{
			if (syntaxNode is InvocationExpressionSyntax invocation &&
				invocation.Expression is MemberAccessExpressionSyntax access &&
				access.Expression is IdentifierNameSyntax accessIdentifier &&
				accessIdentifier.Identifier.Text == Naming.PartiallyClassName &&
				access.Name.Identifier.Text.StartsWith(Naming.ApplyMethodName))
			{
				this.Candidates.Add(invocation);
			}
		}

		public List<InvocationExpressionSyntax> Candidates { get; } = new List<InvocationExpressionSyntax>();
	}
}