using BlazorSecurity.Components.Pages;
using Bunit;
using Bunit.TestDoubles;

namespace BlazorSecurity.Tests;

public class AuthorizationTest
{
    [Fact]
    public void IsAuthenticatedTest()
    {
        // Arrange
        var context = new TestContext();

        var authContext = context.AddTestAuthorization();
        authContext.SetAuthorized("test@test.com");
        authContext.SetRoles("Administrator");
        
        // Act
        var cut = context.RenderComponent<Home>();
        
        var cutInstance = cut.Instance;
        
        // Assert
        cut.MarkupMatches(@"
            <div class=""nav-item px-3"">Authenticated</div>
            <div class=""nav-item px-3"">You are sgu Authenticated</div>
            <div class=""nav-item px-3"">ADMIN</div>
        ");
        
        Assert.True(cutInstance.IsAdmin);
    }
}