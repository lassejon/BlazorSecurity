using BlazorSecurity.Components.Pages;
using Bunit;
using Bunit.TestDoubles;

namespace BlazorSecurity.Tests;

public class HomePageCodeTests
{
    [Fact]
    public void IsNotAuthenticatedTest()
    {
        // Arrange
        var context = new TestContext();
        _ = context.AddTestAuthorization();
        
        // Act
        var cut = context.RenderComponent<Home>();
        
        // Assert
        Assert.False(cut.Instance.IsAuthenticated);
    }
    
    [Fact]
    public void IsAuthenticatedTest()
    {
        // Arrange
        var context = new TestContext();

        var authContext = context.AddTestAuthorization();
        authContext.SetAuthorized("test@test.com", AuthorizationState.Authorized);
        
        // Act
        var cut = context.RenderComponent<Home>();
        
        // Assert
        Assert.True(cut.Instance.IsAuthenticated);
    }
    
    [Fact]
    public void IsAuthenticatedAndAdminTest()
    {
        // Arrange
        var context = new TestContext();
        var authContext = context.AddTestAuthorization();
        authContext.SetAuthorized("test@test.com", AuthorizationState.Authorized);
        authContext.SetRoles("Administrator");
        
        // Act
        var cut = context.RenderComponent<Home>();
        
        // Assert
        Assert.True(cut.Instance.IsAuthenticated);
        Assert.True(cut.Instance.IsAdmin);
    }
}