using BlazorSecurity.Components.Pages;
using System.Security.Claims;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorSecurity.Tests;

public class AuthenticationTest
{
    [Fact]
    public void IsNotAuthenticatedTest()
    {
        // Arrange
        var context = new TestContext();
        _ = context.AddTestAuthorization();
        
        // Act
        var cut = context.RenderComponent<Home>();
        
        var cutInstance = cut.Instance;
        
        // Assert
        cut.MarkupMatches(@"
            <div class=""nav-item px-3"">NON-Authenticated</div>
            <div class=""nav-item px-3"">You are sgu NON-Authenticated</div>
            <div class=""nav-item px-3"">NOT AN ADMIN</div>
        ");
        
        Assert.False(cutInstance.IsAuthenticated);
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
        
        var cutInstance = cut.Instance;
        
        // Assert
        cut.MarkupMatches(@"
            <div class=""nav-item px-3"">Authenticated</div>
            <div class=""nav-item px-3"">You are sgu Authenticated</div>
            <div class=""nav-item px-3"">NOT AN ADMIN</div>
        ");
        
        Assert.True(cutInstance.IsAuthenticated);
    }
}