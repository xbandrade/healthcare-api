using Xunit;
using Microsoft.AspNetCore.Mvc;
using HealthcareAPI.Controllers;
using HealthcareAPI.Data;
using Moq;

namespace HealthcareAPI.Tests;

[Trait("T", "Auth")]
public class AuthControllerTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<TestHealthcareDBContext> _mockContext;

    public AuthControllerTests()
    {    
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json")
            .Build();
        _mockConfig = new();
        _mockConfig.Setup(x => x["Jwt:Issuer"]).Returns(config["Jwt:Issuer"]);
        _mockConfig.Setup(x => x["Jwt:Audience"]).Returns(config["Jwt:Audience"]);
        _mockConfig.Setup(x => x["Jwt:SecretKey"]).Returns(config["Jwt:SecretKey"]);
        _mockContext = new();
        var masterStaff = new StaffMember { Username = "master" };
        masterStaff.SetPassword("a12");
        _mockContext.Setup(x => x.StaffMembers).Returns(new List<StaffMember> { masterStaff }.AsQueryable());
    }

    [Fact]
    public void Login_ValidCredentials_ReturnsToken()
    {
        var authController = new AuthController(_mockConfig.Object, _mockContext.Object);
        var loginDto = new LoginDTO { Username = "master", Password = "a12" };
        var result = authController.Login(loginDto) as ObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var authController = new AuthController(_mockConfig.Object, _mockContext.Object);
        var loginDto = new LoginDTO { Username = "master", Password = "invalid" };
        var result = authController.Login(loginDto) as UnauthorizedResult;
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }
}
