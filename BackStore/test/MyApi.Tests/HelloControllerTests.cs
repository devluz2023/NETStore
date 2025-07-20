using Xunit;
using HelloWorldApi.Controllers; // ensure namespace matches where your HelloController is

public class HelloControllerTests
{
    [Fact]
    public void Get_Returns_HelloWorld2()
    {
        // Arrange
        var controller = new HelloController();

        // Act
        var result = controller.GetPublic();

        // Assert
        Assert.Equal("Hello World 2 - This is a PUBLIC endpoint!", result);
    }
}