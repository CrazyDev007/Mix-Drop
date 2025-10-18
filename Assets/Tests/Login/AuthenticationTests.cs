using System.Collections;
using NUnit.Framework;
using MixDrop.Login.Services;

namespace MixDrop.Login.Tests
{
    public class AuthenticationTests
    {
        [Test]
        public async System.Threading.Tasks.Task SignInWithEmailPassword_WithValidCredentials_Succeeds()
        {
            // Arrange
            string testEmail = "test@example.com";
            string testPassword = "ValidPass123";

            // Act
            await AuthenticationService.Instance.SignInWithEmailPassword(testEmail, testPassword);

            // Assert
            Assert.IsTrue(AuthenticationService.Instance.IsSignedIn, "User should be signed in after successful login");
            Assert.IsNotNull(AuthenticationService.Instance.UserId, "UserId should not be null after login");
            Assert.IsNotEmpty(AuthenticationService.Instance.UserId, "UserId should not be empty after login");
        }

        [Test]
        public void AuthenticationService_Instance_IsNotNull()
        {
            // Arrange & Act
            var instance = AuthenticationService.Instance;

            // Assert
            Assert.IsNotNull(instance);
        }

        [Test]
        public void AuthenticationService_IsSignedIn_ReturnsBool()
        {
            // Arrange
            var authService = AuthenticationService.Instance;

            // Act
            bool isSignedIn = authService.IsSignedIn;

            // Assert
            // Should not throw exception - just test that property is accessible
            Assert.IsInstanceOf<bool>(isSignedIn);
        }

        [Test]
        public void AuthenticationService_UserId_ReturnsString()
        {
            // Arrange
            var authService = AuthenticationService.Instance;

            // Act
            string userId = authService.UserId;

            // Assert
            Assert.IsInstanceOf<string>(userId);
        }

        [Test]
        public void SignInWithEmailPassword_WithInvalidCredentials_ThrowsException()
        {
            // Arrange
            string invalidEmail = "invalid@example.com";
            string invalidPassword = "WrongPass123";

            // Act & Assert
            var exception = Assert.ThrowsAsync<Unity.Services.Authentication.AuthenticationException>(
                async () => await AuthenticationService.Instance.SignInWithEmailPassword(invalidEmail, invalidPassword)
            );
            
            // Assert exception details if needed
            Assert.IsNotNull(exception.Message);
        }
    }
}