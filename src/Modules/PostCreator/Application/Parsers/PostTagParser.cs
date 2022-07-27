using System.Text.RegularExpressions;

using LanguageExt;

using PostCreator.Domain.Model;

using static LanguageExt.Prelude;

namespace PostCreator.Application.Parsers;

public interface IPostTagParser
{
    Option<IReadOnlyList<Tag>> ParseTagsFromPostContent(string content);
}

internal class PostTagParser : IPostTagParser
{
    private static Regex TagRegex = new(@"(?<=#)\w+", RegexOptions.Compiled);
    public Option<IReadOnlyList<Tag>> ParseTagsFromPostContent(string content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));
        var result = TagRegex.Matches(content);
        if (result.Count == 0)
        {
            return None;
        }
        return result.Select(tag => new Tag(tag.Value.ToLowerInvariant())).ToList();
    }
}