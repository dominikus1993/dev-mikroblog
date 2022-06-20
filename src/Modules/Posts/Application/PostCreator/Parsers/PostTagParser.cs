using System.Text.RegularExpressions;

using DevMikroblog.Modules.Posts.Domain.Model;

namespace DevMikroblog.Modules.Posts.Application.PostCreator.Parsers;

public interface IPostTagParser
{
    IReadOnlyList<Tag> ParseTagsFromPostContent(string content);
}

internal class PostTagParser : IPostTagParser
{
    private static Regex TagRegex = new(@"(?<=#)\w+", RegexOptions.Compiled);
    public IReadOnlyList<Tag> ParseTagsFromPostContent(string content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));
        var result = TagRegex.Matches(content);
        if (result.Count == 0)
        {
            return new List<Tag>(0);
        }
        return result.Select(tag => new Tag(tag.Value.ToLowerInvariant())).ToList();
    }
}