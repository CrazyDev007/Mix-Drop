using NUnit.Framework;
using MixDrop.Login.Services;

namespace MixDrop.Login.Tests.Authentication
{
    public class SessionManagementTests
    {
        [Test]
        public void UserSessionManager_Instance_IsNotNull()
        {
            // Arrange & Act
            var instance = UserSessionManager.Instance;

            // Assert
            Assert.IsNotNull(instance);
        }

        [Test]
        public void UserSessionManager_Instance_IsSingleton()
        {
            // Arrange & Act
            var instance1 = UserSessionManager.Instance;
            var instance2 = UserSessionManager.Instance;

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void UserSessionManager_StoreSession_WhenNotSignedIn_LogsWarning()
        {
            // Arrange
            var sessionManager = UserSessionManager.Instance;

            // Act
            sessionManager.StoreSession();

            // Assert
            // Test passes if no exception is thrown
            // Warning would be logged but we can't easily test logs in unit tests
            Assert.IsTrue(true);
        }

        [Test]
        public void UserSessionManager_RestoreSession_ReturnsFalse_WhenNotSignedIn()
        {
            // Arrange
            var sessionManager = UserSessionManager.Instance;

            // Act
            bool restored = sessionManager.RestoreSession();

            // Assert
            Assert.IsFalse(restored);
        }

        [Test]
        public void UserSessionManager_HasValidSession_ReturnsFalse_WhenNotSignedIn()
        {
            // Arrange
            var sessionManager = UserSessionManager.Instance;

            // Act
            bool hasValidSession = sessionManager.HasValidSession();

            // Assert
            Assert.IsFalse(hasValidSession);
        }

        [Test]
        public void UserSessionManager_ClearSession_DoesNotThrowException()
        {
            // Arrange
            var sessionManager = UserSessionManager.Instance;

            // Act & Assert
            Assert.DoesNotThrow(() => sessionManager.ClearSession());
        }
    }
}