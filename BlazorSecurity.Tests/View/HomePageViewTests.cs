using BlazorSecurity.Components.Pages;
using Bunit;
using Bunit.TestDoubles;

namespace BlazorSecurity.Tests.View;

public class HomePageViewTests
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
        cut.MarkupMatches(@"
            <div class=""nav-item px-3"">NON-Authenticated</div>
            <div class=""nav-item px-3"">You are sgu NON-Authenticated</div>
            <div class=""nav-item px-3"">NOT AN ADMIN</div>
        ");
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
        cut.MarkupMatches(@"
            <div class=""nav-item px-3"">Authenticated</div>
            <div class=""nav-item px-3"">You are sgu Authenticated</div>
            <div class=""nav-item px-3"">NOT AN ADMIN</div>
        ");
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
        cut.MarkupMatches(@"
            <div class=""nav-item px-3"">Authenticated</div>
            <div class=""nav-item px-3"">You are sgu Authenticated</div>
            <div class=""nav-item px-3"">ADMIN</div>
        ");
    }
}