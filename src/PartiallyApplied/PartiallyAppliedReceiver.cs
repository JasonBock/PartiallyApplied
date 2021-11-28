//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.CodeAnalysis;

//namespace PartiallyApplied;

//public sealed class PartiallyAppliedReceiver
//	 : ISyntaxReceiver
//{
//	public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
//	{
//		if (syntaxNode is InvocationExpressionSyntax invocation &&
//			invocation.Expression is MemberAccessExpressionSyntax access &&
//			access.Expression is IdentifierNameSyntax accessIdentifier &&
//			accessIdentifier.Identifier.Text == Naming.PartiallyClassName &&
//			access.Name.Identifier.Text.StartsWith(Naming.ApplyMethodName, StringComparison.InvariantCulture))
//		{
//			this.Candidates.Add(invocation);
//		}
//	}

//#pragma warning disable CA1002 // Do not expose generic lists
//   public List<InvocationExpressionSyntax> Candidates { get; } = new List<InvocationExpressionSyntax>();
//#pragma warning restore CA1002 // Do not expose generic lists
//}