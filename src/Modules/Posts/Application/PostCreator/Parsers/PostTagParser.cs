using System.Text.RegularExpressions;

using DevMikroblog.Modules.Posts.Domain.Model;

using LanguageExt;
using static LanguageExt.Prelude;

using Array = System.Array;

namespace DevMikroblog.Modules.Posts.Application.PostCreator.Parsers;

public interface IPostTagParser
{
    IReadOnlyList<Tag> ParseTagsFromPostContent(string content);
}

internal class PostTagParser : IPostTagParser
{
    private static readonly Regex TagRegex = new(@"(?<=#)\w+", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    public IReadOnlyList<Tag> ParseTagsFromPostContent(string content)
    {
        ArgumentException.ThrowIfNullOrEmpty(content);
        var result = TagRegex.Matches(content);
        if (result.Count == 0)
        {
            return Array.Empty<Tag>();
        }
        return result.Select(tag => new Tag(tag.Value.ToUpperInvariant())).ToArray();
    }
}