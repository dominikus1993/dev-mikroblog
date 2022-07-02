using System.Text.RegularExpressions;

using DevMikroblog.Modules.Posts.Domain.Model;

using LanguageExt;
using static LanguageExt.Prelude;

namespace DevMikroblog.Modules.Posts.Application.PostCreator.Parsers;

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