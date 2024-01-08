using System.Net.Http.Headers;
using System.Text;
using AccountMicroservice.AsyncDataServices.Interfaces;
using AccountMicroservice.MessageBusEvents;
using AccountMicroservice.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;

public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private Mock<IMessageBusClient> _mockMessageBusClient;


    public AuthControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            _mockMessageBusClient = new Mock<IMessageBusClient>();
            // Setup the mock to do nothing when PublishMessage is called
            _mockMessageBusClient.Setup(m => m.PublishMessage(It.IsAny<UserRegisteredEvent>(), It.IsAny<string>()));

            // Replace the actual IMessageBusClient registration with the mock
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IMessageBusClient>(provider => _mockMessageBusClient.Object);
            });
        }).CreateClient();
    }
    [Fact]
    public async Task Login_ReturnsToken_ForValidCredentials()
    {
        // Arrange
        var loginModel = new Login { Username = "testuser", Password = "Test123!" }; // Use existing user in the seeded database
        var content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/login", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(responseString);
        Assert.NotNull(result.token);
    }

    [Fact]
    public async Task Register_CreatesNewUser_WhenDataIsValid()
    {
        // Arrange
        var registerModel = new Register
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "NewUser123!"
        };
        var content = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/register", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<Response>(responseString);
        Assert.Equal("Success", result.Status);
    }

    [Fact]
    public async Task GetUserDetails_ReturnsUserDetails_WhenUserIsAuthenticated()
    {
        // Arrange
        var loginModel = new Login { Username = "testuser", Password = "Test123!" }; // Replace with valid credentials
        var loginContent = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
        var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
        loginResponse.EnsureSuccessStatusCode();
        var loginResponseString = await loginResponse.Content.ReadAsStringAsync();
        var loginResult = JsonConvert.DeserializeObject<dynamic>(loginResponseString);
        var token = (string)loginResult.token; // Adjust this line if the token is nested differently in your response

        // Act
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.GetAsync("/api/auth/userdetails");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var userDetails = JsonConvert.DeserializeObject<dynamic>(responseString);
        Assert.NotNull(userDetails);
        Assert.NotNull(userDetails.user); // Adjust if the structure of the response is different
    }







}
