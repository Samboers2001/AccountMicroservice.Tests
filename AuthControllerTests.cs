using AccountMicroservice.AsyncDataServices.Interfaces;
using AccountMicroservice.Controllers;
using AccountMicroservice.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
public class AuthControllerTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IMessageBusClient> _messageBusClientMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c["JWTAuth:SecretKey"]).Returns("YourSecretKeyHere");
        _configurationMock.Setup(c => c["JWTAuth:ValidIssuerURL"]).Returns("YourIssuerURLHere");
        _configurationMock.Setup(c => c["JWTAuth:ValidAudienceURL"]).Returns("YourAudienceURLHere");

        _messageBusClientMock = new Mock<IMessageBusClient>();

        _controller = new AuthController(_userManagerMock.Object, _roleManagerMock.Object, _configurationMock.Object, _messageBusClientMock.Object);
    }

    [Fact]
    public async Task Login_UserExists_ReturnsToken()
    {
        // Arrange
        var user = new IdentityUser { UserName = "testuser", Email = "test@example.com" };
        var loginModel = new Login { Username = "testuser", Password = "testpassword" };
        var userRoles = new List<string> { "User" };

        _userManagerMock.Setup(um => um.FindByNameAsync(loginModel.Username)).ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginModel.Password)).ReturnsAsync(true);
        _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(userRoles);

        // Act
        var result = await _controller.Login(loginModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);

        var tokenString = okResult.Value.ToString();
        Assert.NotNull(tokenString);
        Assert.NotEmpty(tokenString);
    }

    [Fact]
    public async Task Register_ValidModel_ReturnsSuccess()
    {
        // Arrange
        var registerModel = new Register
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "Password123"
        };

        _userManagerMock.Setup(um => um.FindByNameAsync(registerModel.Username)).ReturnsAsync((IdentityUser)null); // User does not exist
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerModel.Password))
                        .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.Register(registerModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Response>(okResult.Value);
        Assert.NotNull(response);
        Assert.Equal("Success", response.Status);
        Assert.Equal("User created successfully!", response.Message);
    }

    [Fact]
    public async Task RegisterAdmin_ValidModel_ReturnsSuccess()
    {
        // Arrange
        var registerModel = new Register
        {
            Username = "newadmin",
            Email = "newadmin@example.com",
            Password = "Password123" 
        };

        _userManagerMock.Setup(um => um.FindByNameAsync(registerModel.Username)).ReturnsAsync((IdentityUser)null); // User does not exist
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerModel.Password))
                        .ReturnsAsync(IdentityResult.Success);


        // Act
        var result = await _controller.RegisterAdmin(registerModel);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Response>(okResult.Value);
        Assert.NotNull(response);
        Assert.Equal("Success", response.Status);
        Assert.Equal("User created successfully!", response.Message);
    }











}
