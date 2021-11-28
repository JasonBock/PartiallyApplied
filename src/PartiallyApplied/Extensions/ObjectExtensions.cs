﻿namespace PartiallyApplied.Extensions;

internal static class ObjectExtensions
{
	/// <summary>
	/// This should only be used to get a stringified version of a default value
	/// that will be put into the call site of an emitted method.
	/// </summary>
	internal static string GetDefaultValue(this object? self) =>
		self switch
		{
			string s => $"\"{s}\"",
			bool b => $"{(b ? "true" : "false")}",
			null => "null",
			_ => self.ToString() ?? string.Empty
		};
}