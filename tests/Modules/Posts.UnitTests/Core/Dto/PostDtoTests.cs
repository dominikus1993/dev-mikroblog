using System;
using System.Collections.Generic;

using DevMikroblog.Modules.Posts.Core.Dto;
using DevMikroblog.Modules.Posts.Core.Model;

using FluentAssertions;

using Xunit;

namespace Posts.UnitTests.Core.Dto;

public class PostDtoTests
{
    [Fact]
    public void TestMapPostToDto_ShouldBeCorrectDto()
    {
        var author = new Author(AuthorId.New(), "xD");
        var post = new Post(PostId.New(), "test", new List<Post>(), DateTime.UtcNow, author, 2);

        var subject = PostDto.FromPost(post);

        subject.Should().NotBeNull();
        subject.Content.Should().Be(post.Content);
        subject.PostId.Should().Be(post.Id.Value);
    }
}