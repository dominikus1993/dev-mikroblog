using System;

using AutoFixture.Xunit2;

using DevMikroblog.Modules.Posts.Common;
using FluentAssertions;

using Xunit;

using PostId = DevMikroblog.Modules.Posts.Domain.Model.PostId;

namespace Posts.UnitTests.Common;

public class IdUtilsTests
{
    [Theory]
    [InlineData("914d2958-7910-4b6d-85e1-19a46010a4b8", "WClNkRB5bUuF4RmkYBCkuA==")]
    [InlineData("914d2958-7910-4b6d-85e1-19a46010a4b9", "WClNkRB5bUuF4RmkYBCkuQ==")]
    public void TestSlugGeneration_ShouldReturnBase64Slug(string uuid, string expectedSlug)
    {
        var guid = new Guid(uuid);

        var postId = new PostId(guid);

        var slug = postId.ToSlug();

        slug.Should().NotBeNullOrEmpty();
    
        Assert.Equal(expectedSlug, slug);
    }
    
    [Theory]
    [InlineData("914d2958-7910-4b6d-85e1-19a46010a4b8", "WClNkRB5bUuF4RmkYBCkuA==")]
    public void TestFromSlugGeneration_ShouldReturnValidPostId(string uuid, string expectedSlug)
    {
        var postId = new PostId(new Guid(uuid));
        var id = Id.FromSlug(expectedSlug);
        
        Assert.Equal(id, postId);
    }
    
    [Theory]
    [AutoData]
    public void TestToAndFromGenerationBeetweenPostIdAndSlug_SholdBeCorrect(PostId postId)
    {
        var slug = postId.ToSlug();
        
        var newpostid = Id.FromSlug(slug);

        newpostid.Should().Be(postId);
    }
}