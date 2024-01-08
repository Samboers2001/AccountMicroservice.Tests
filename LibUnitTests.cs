using AccountMicroservice.Helpers;

public class LibTests
{
    private readonly Lib _lib;

    public LibTests()
    {
        _lib = new Lib();
    }

    [Fact]
    public void IsValidPassword_ValidPassword_ReturnsTrue()
    {
        // Arrange
        string validPassword = "Password1";

        // Act
        bool result = _lib.IsValidPassword(validPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidPassword_InvalidPassword_ReturnsFalse()
    {
        // Arrange
        string invalidPassword = "invalid";

        // Act
        bool result = _lib.IsValidPassword(invalidPassword);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("valid@example.com")]
    [InlineData("another.valid.email@example.co.uk")]
    public void IsValidEmail_ValidEmail_ReturnsTrue(string email)
    {
        // Act
        bool result = _lib.IsValidEmail(email);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("invalid.email@.com")]
    [InlineData("invalid.email")]
    public void IsValidEmail_InvalidEmail_ReturnsFalse(string email)
    {
        // Act
        bool result = _lib.IsValidEmail(email);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void EmailIsEmpty_EmptyString_ReturnsFalse()
    {
        // Arrange
        string emptyEmail = "";

        // Act
        bool result = _lib.EmailIsEmpty(emptyEmail);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void EmailIsEmpty_NonEmptyString_ReturnsTrue()
    {
        // Arrange
        string nonEmptyEmail = "example@example.com";

        // Act
        bool result = _lib.EmailIsEmpty(nonEmptyEmail);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void PasswordIsEmpty_EmptyString_ReturnsFalse()
    {
        // Arrange
        string emptyPassword = "";

        // Act
        bool result = _lib.PasswordIsEmpty(emptyPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void PasswordIsEmpty_NonEmptyString_ReturnsTrue()
    {
        // Arrange
        string nonEmptyPassword = "Password123";

        // Act
        bool result = _lib.PasswordIsEmpty(nonEmptyPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void UsernameIsEmpty_EmptyString_ReturnsFalse()
    {
        // Arrange
        string emptyUsername = "";

        // Act
        bool result = _lib.UsernameIsEmpty(emptyUsername);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UsernameIsEmpty_NonEmptyString_ReturnsTrue()
    {
        // Arrange
        string nonEmptyUsername = "exampleuser";

        // Act
        bool result = _lib.UsernameIsEmpty(nonEmptyUsername);

        // Assert
        Assert.True(result);
    }
}
