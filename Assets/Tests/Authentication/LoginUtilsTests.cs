using NUnit.Framework;
using MixDrop.Login;

namespace MixDrop.Login.Tests.Authentication
{
    public class LoginUtilsTests
    {
        [Test]
        public void GetErrorMessage_InvalidCredentials_ReturnsCorrectMessage()
        {
            // Arrange
            var errorType = LoginErrorType.InvalidCredentials;

            // Act
            string message = LoginUtils.GetErrorMessage(errorType);

            // Assert
            Assert.AreEqual("Invalid email or password. Please check your credentials and try again.", message);
        }

        [Test]
        public void GetErrorMessage_AccountLocked_ReturnsCorrectMessage()
        {
            // Arrange
            var errorType = LoginErrorType.AccountLocked;

            // Act
            string message = LoginUtils.GetErrorMessage(errorType);

            // Assert
            Assert.AreEqual("Account is temporarily locked due to too many failed attempts.", message);
        }

        [Test]
        public void GetErrorMessage_NetworkError_ReturnsCorrectMessage()
        {
            // Arrange
            var errorType = LoginErrorType.NetworkError;

            // Act
            string message = LoginUtils.GetErrorMessage(errorType);

            // Assert
            Assert.AreEqual("Network error. Please check your internet connection and try again.", message);
        }

        [Test]
        public void GetErrorMessage_ValidationError_ReturnsCorrectMessage()
        {
            // Arrange
            var errorType = LoginErrorType.ValidationError;

            // Act
            string message = LoginUtils.GetErrorMessage(errorType);

            // Assert
            Assert.AreEqual("Invalid input. Please check your email and password format.", message);
        }

        [Test]
        public void GetErrorMessage_EmailNotVerified_ReturnsCorrectMessage()
        {
            // Arrange
            var errorType = LoginErrorType.EmailNotVerified;

            // Act
            string message = LoginUtils.GetErrorMessage(errorType);

            // Assert
            Assert.AreEqual("Please verify your email address before logging in.", message);
        }

        [Test]
        public void GetErrorMessage_ServiceUnavailable_ReturnsCorrectMessage()
        {
            // Arrange
            var errorType = LoginErrorType.ServiceUnavailable;

            // Act
            string message = LoginUtils.GetErrorMessage(errorType);

            // Assert
            Assert.AreEqual("Authentication service is currently unavailable. Please try again later.", message);
        }

        [Test]
        public void GetErrorMessage_EmailAlreadyExists_ReturnsCorrectMessage()
        {
            // Arrange
            var errorType = LoginErrorType.EmailAlreadyExists;

            // Act
            string message = LoginUtils.GetErrorMessage(errorType);

            // Assert
            Assert.AreEqual("An account with this email already exists. Please use a different email or try logging in.", message);
        }

        [Test]
        public void GetErrorMessage_WeakPassword_ReturnsCorrectMessage()
        {
            // Arrange
            var errorType = LoginErrorType.WeakPassword;

            // Act
            string message = LoginUtils.GetErrorMessage(errorType);

            // Assert
            Assert.AreEqual("Password does not meet requirements. Please choose a stronger password.", message);
        }

        [Test]
        public void GetErrorMessage_Unknown_ReturnsDefaultMessage()
        {
            // Arrange
            var errorType = LoginErrorType.Unknown;

            // Act
            string message = LoginUtils.GetErrorMessage(errorType);

            // Assert
            Assert.AreEqual("An error occurred. Please try again.", message);
        }

        [Test]
        public void GetErrorMessage_AllErrorTypes_HaveMessages()
        {
            // Arrange
            var allErrorTypes = System.Enum.GetValues(typeof(LoginErrorType));

            // Act & Assert
            foreach (LoginErrorType errorType in allErrorTypes)
            {
                string message = LoginUtils.GetErrorMessage(errorType);
                Assert.IsNotNull(message, $"Message for {errorType} should not be null");
                Assert.IsNotEmpty(message, $"Message for {errorType} should not be empty");
            }
        }
    }
}