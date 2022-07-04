using System.Text;

using DevMikroblog.BuildingBlocks.Infrastructure.Messaging.OpenTelemetry;

using FluentAssertions;

using Moq;

using RabbitMQ.Client;

namespace Infrastructure.Tests.Messaging.OpenTelemetry;

public class RabbitMqOpenTelemetryTests
{
    [Fact]
    public void TestExtractorWhenPropsHeadersIsNotNull()
    {
        // Arrange
        var mock = new Mock<IBasicProperties>();
        mock.Setup(x => x.Headers).Returns(new Dictionary<string, object>
        {
            {
                "traceparent",
                Encoding.UTF8.GetBytes("30302D39303039666462373133376630353366343833613162666661363837373862382D613438633166363834383338323238372D3031")
            }
        });
        
        // Act
        var subject = RabbitMqOpenTelemetry.ExtractContextFromHeader(mock.Object, "traceparent").ToList();
        
        // Test

        subject.Should().NotBeNull();
        subject.Should().NotBeEmpty();
        subject.Should().HaveCount(1);
    }
    
    [Fact]
    public void TestExtractorWhenPropsHeadersIsNull()
    {
        // Arrange
        var mock = new Mock<IBasicProperties>();
        mock.Setup(x => x.Headers).Returns((Dictionary<string, object>?)null);
        
        // Act
        var subject = RabbitMqOpenTelemetry.ExtractContextFromHeader(mock.Object, "traceparent").ToList();
        
        // Test

        subject.Should().NotBeNull();
        subject.Should().BeEmpty();
    }
    
    [Fact]
    public void TestExtractorWhenPropsHeadersWhenKeyIsNotExists()
    {
        // Arrange
        var mock = new Mock<IBasicProperties>();
        mock.Setup(x => x.Headers).Returns(new Dictionary<string, object>
        {
            {
                "traceparent",
                Encoding.UTF8.GetBytes("30302D39303039666462373133376630353366343833613162666661363837373862382D613438633166363834383338323238372D3031")
            }
        });
        
        // Act
        var subject = RabbitMqOpenTelemetry.ExtractContextFromHeader(mock.Object, "xDDD").ToList();
        
        // Test

        subject.Should().NotBeNull();
        subject.Should().BeEmpty();
    }
}