using System.Collections;
using NUnit.Framework;
using MixDrop.Login.Services;
using MixDrop.Login;

namespace MixDrop.Login.Tests.Authentication
{
    public class AuthenticationEdgeCaseTests
    {
        [Test]
        public void SignInWithEmailPassword_NullEmail_ThrowsException()
        {
            // Arrange
            string nullEmail = null;
            string password = "ValidPass123";

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignInWithEmailPassword(nullEmail, password)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignInWithEmailPassword_EmptyEmail_ThrowsException()
        {
            // Arrange
            string emptyEmail = "";
            string password = "ValidPass123";

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignInWithEmailPassword(emptyEmail, password)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignInWithEmailPassword_WhitespaceEmail_ThrowsException()
        {
            // Arrange
            string whitespaceEmail = "   ";
            string password = "ValidPass123";

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignInWithEmailPassword(whitespaceEmail, password)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignInWithEmailPassword_NullPassword_ThrowsException()
        {
            // Arrange
            string email = "test@example.com";
            string nullPassword = null;

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignInWithEmailPassword(email, nullPassword)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignInWithEmailPassword_EmptyPassword_ThrowsException()
        {
            // Arrange
            string email = "test@example.com";
            string emptyPassword = "";

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignInWithEmailPassword(email, emptyPassword)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignInWithEmailPassword_InvalidEmailFormat_ThrowsException()
        {
            // Arrange
            string invalidEmail = "invalid-email";
            string password = "ValidPass123";

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignInWithEmailPassword(invalidEmail, password)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignInWithEmailPassword_EmailWithoutAt_ThrowsException()
        {
            // Arrange
            string invalidEmail = "testexample.com";
            string password = "ValidPass123";

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignInWithEmailPassword(invalidEmail, password)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignInWithEmailPassword_EmailWithoutDomain_ThrowsException()
        {
            // Arrange
            string invalidEmail = "test@";
            string password = "ValidPass123";

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignInWithEmailPassword(invalidEmail, password)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignUpWithEmailPassword_NullEmail_ThrowsException()
        {
            // Arrange
            string nullEmail = null;
            string password = "ValidPass123";

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignUpWithEmailPassword(nullEmail, password)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignUpWithEmailPassword_EmptyEmail_ThrowsException()
        {
            // Arrange
            string emptyEmail = "";
            string password = "ValidPass123";

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignUpWithEmailPassword(emptyEmail, password)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignUpWithEmailPassword_NullPassword_ThrowsException()
        {
            // Arrange
            string email = "test@example.com";
            string nullPassword = null;

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignUpWithEmailPassword(email, nullPassword)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignUpWithEmailPassword_EmptyPassword_ThrowsException()
        {
            // Arrange
            string email = "test@example.com";
            string emptyPassword = "";

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignUpWithEmailPassword(email, emptyPassword)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void SignUpWithEmailPassword_InvalidEmailFormat_ThrowsException()
        {
            // Arrange
            string invalidEmail = "invalid-email";
            string password = "ValidPass123";

            // Act & Assert
            var exception = Assert.ThrowsAsync<LoginException>(
                async () => await AuthenticationService.Instance.SignUpWithEmailPassword(invalidEmail, password)
            );
            Assert.AreEqual(LoginErrorType.ValidationError, exception.ErrorType);
        }

        [Test]
        public void LoadUserAccount_NonExistentEmail_ReturnsNull()
        {
            // Arrange
            string nonExistentEmail = "nonexistent@example.com";

            // Act
            var account = AuthenticationService.Instance.LoadUserAccount(nonExistentEmail);

            // Assert
            Assert.IsNull(account);
        }

        [Test]
        public void LoadUserAccount_EmptyEmail_ReturnsNull()
        {
            // Arrange
            string emptyEmail = "";

            // Act
            var account = AuthenticationService.Instance.LoadUserAccount(emptyEmail);

            // Assert
            Assert.IsNull(account);
        }

        [Test]
        public void LoadUserAccount_NullEmail_ReturnsNull()
        {
            // Arrange
            string nullEmail = null;

            // Act
            var account = AuthenticationService.Instance.LoadUserAccount(nullEmail);

            // Assert
            Assert.IsNull(account);
        }
    }
}