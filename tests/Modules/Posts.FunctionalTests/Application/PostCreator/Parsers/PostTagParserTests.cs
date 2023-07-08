using AutoFixture.Xunit2;

using DevMikroblog.Modules.Posts.Application.PostCreator.Parsers;
using DevMikroblog.Modules.Posts.Domain.Model;

using FluentAssertions;

using LanguageExt.UnsafeValueAccess;

using Xunit;

namespace Posts.FunctionalTests.Application.PostCreator.Parsers;

public class PostTagParserTests
{
    [Theory]
    [AutoData]
    internal void TestWhenPostContentNotContainsAnyTags(PostTagParser parser)
    {
        // Arrange
        const string content = "Hello from fsharp";
        
        // Act

        var tags = parser.ParseTagsFromPostContent(content);
        
        // Test

        tags.Should().BeEmpty();
    }
    
    [Theory]
    [AutoData]
    internal void TestWhenPostContentContainsTags(PostTagParser parser)
    {
        // Arrange
        const string content = "Hello from #fsharp and #csharp";
        
        // Act

        var subject = parser.ParseTagsFromPostContent(content);
        
        // Test
        subject.Should().NotBeEmpty();
        subject.Should().HaveCount(2);
         subject.Should().Contain("CSHARP").And.Contain("FSHARP");
    }

}