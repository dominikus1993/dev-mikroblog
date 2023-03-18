using System;
using System.Collections.Generic;

using AutoFixture.Xunit2;

using DevMikroblog.Modules.Posts.Core.Dto;
using DevMikroblog.Modules.Posts.Domain.Model;

using FluentAssertions;

using Xunit;

using AuthorId = DevMikroblog.Modules.Posts.Domain.Model.AuthorId;
using PostId = DevMikroblog.Modules.Posts.Domain.Model.PostId;
using static LanguageExt.Prelude;

namespace Posts.UnitTests.Core.Dto;

public class PostDtoTests
{
    [Theory]
    [AutoData]
    public void TestMapPostToDto_ShouldBeCorrectDto(Author author, Post post)
    {
        var subject = PostDto.FromPost(post);

        subject.Should().NotBeNull();
        subject.Content.Should().Be(post.Content);
        subject.PostId.Should().Be(post.Id.Value);
        subject.Likes.Should().Be(post.Likes);
        subject.RepliesQuantity.Should().Be(post.RepliesQuantity);
        subject.Tags.Should().BeNull();
        subject.Author.AuthorId.Should().Be(author.Id.Value);
        subject.Author.AuthorName.Should().Be(author.Name);
    }
}