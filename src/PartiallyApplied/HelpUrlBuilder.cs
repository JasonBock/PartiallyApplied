namespace PartiallyApplied;

internal static class HelpUrlBuilder
{
	internal static string Build(string identifier, string title) =>
		$"https://github.com/JasonBock/PartiallyApplied/tree/master/docs/{identifier}-{title}.md";
}