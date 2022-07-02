using DevMikroblog.Modules.Posts.Application.PostCreator.Parsers;
using DevMikroblog.Modules.Posts.Domain.Model;

using FluentAssertions;

using LanguageExt.UnsafeValueAccess;

using Xunit;

namespace Posts.FunctionalTests.Application.PostCreator.Parsers;

public class PostTagParserTests
{
    [Fact]
    public void TestWhenPostContentNotContainsAnyTags()
    {
        // Arrange
        var parser = new PostTagParser();
        string content = "Hello from fsharp";
        
        // Act

        var tags = parser.ParseTagsFromPostContent(content);
        
        // Test

        tags.IsNone.Should().BeTrue();
    }
    
    [Fact]
    public void TestWhenPostContentContainsTags()
    {
        // Arrange
        var parser = new PostTagParser();
        string content = "Hello from #fsharp and #csharp";
        
        // Act

        var subject = parser.ParseTagsFromPostContent(content);
        
        // Test
        subject.IsSome.Should().BeTrue();
        var tags = subject.ValueUnsafe();
        tags.Should().HaveCount(2);
        tags.Should().Contain(new Tag("csharp")).And.Contain(new Tag("fsharp"));
    }

}