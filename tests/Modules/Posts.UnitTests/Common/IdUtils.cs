using System;

using DevMikroblog.Modules.Posts.Common;
using FluentAssertions;

using Xunit;

using PostId = DevMikroblog.Modules.Posts.Domain.Model.PostId;

namespace Posts.UnitTests.Common;

public class IdUtilsTests
{
    [Fact]
    public void TestSlugGeneration_ShouldReturnBase64Slug()
    {
        var uuid = new Guid("914d2958-7910-4b6d-85e1-19a46010a4b8");

        var postId = new PostId(uuid);

        var slug = postId.ToSlug();

        slug.Should().NotBeNullOrEmpty();

        slug.Should().Be("WClNkRB5bUuF4RmkYBCkuA  ");
    }
    
    [Fact]
    public void TestFromSlugGeneration_ShouldReturnValidPostId()
    {
        var postId = new PostId(new Guid("914d2958-7910-4b6d-85e1-19a46010a4b8"));
        const string slug = "WClNkRB5bUuF4RmkYBCkuA  ";

        var id = Id.FromSlug(slug);

        id.Should().Be(postId);
    }
    
    [Fact]
    public void TestToAndFromGenerationBeetweenPostIdAndSlug_SholdBeCorrect()
    {
        var postId = PostId.New();

        var slug = postId.ToSlug();
        
        var newpostid = Id.FromSlug(slug);

        newpostid.Should().Be(postId);
    }
}