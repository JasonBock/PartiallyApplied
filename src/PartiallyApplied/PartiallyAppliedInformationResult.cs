using Microsoft.CodeAnalysis;
using PartiallyApplied.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartiallyApplied
{
	public sealed class PartiallyAppliedInformationResult 
		: IEquatable<PartiallyAppliedInformationResult?>
	{
		public PartiallyAppliedInformationResult(IMethodSymbol target, int partialArgumentCount) =>
			(this.Target, this.PartialArgumentCount) = (target, partialArgumentCount);

		public static bool operator ==(PartiallyAppliedInformationResult left, PartiallyAppliedInformationResult right) => 
			EqualityComparer<PartiallyAppliedInformationResult>.Default.Equals(left, right);
		
		public static bool operator !=(PartiallyAppliedInformationResult left, PartiallyAppliedInformationResult right) => 
			!(left == right);

		public override bool Equals(object? obj) => 
			this.Equals(obj as PartiallyAppliedInformationResult);

		public bool Equals(PartiallyAppliedInformationResult? other) =>
			other is not null &&
				this.PartialArgumentCount == other.PartialArgumentCount &&
				this.Target.AreEqual(other.Target);

		public override int GetHashCode() =>
			(this.GetTargetHashCode(), this.PartialArgumentCount).GetHashCode();

		private int GetTargetHashCode() =>
			$"{this.Target.ReturnType.Name}{this.Target.Parameters.Select(_ => _.Type.GetName())}".GetHashCode();

		public int PartialArgumentCount { get; }
		public IMethodSymbol Target { get; }
	}
}