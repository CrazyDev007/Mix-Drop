using NUnit.Framework;
using MixDrop.Login.Models;

namespace MixDrop.Login.Tests
{
    public class SecurityTests
    {
        [Test]
        public void UserAccount_RecordFailedLogin_IncrementsLoginAttempts()
        {
            // Arrange
            var account = new UserAccount("test@example.com");

            // Act
            account.RecordFailedLogin();

            // Assert
            Assert.AreEqual(1, account.LoginAttempts);
        }

        [Test]
        public void UserAccount_RecordFailedLogin_MultipleAttempts_IncrementsCorrectly()
        {
            // Arrange
            var account = new UserAccount("test@example.com");

            // Act
            account.RecordFailedLogin();
            account.RecordFailedLogin();
            account.RecordFailedLogin();

            // Assert
            Assert.AreEqual(3, account.LoginAttempts);
        }

        [Test]
        public void UserAccount_ShouldLockAccount_ReturnsFalse_WhenAttemptsBelowThreshold()
        {
            // Arrange
            var account = new UserAccount("test@example.com");
            account.RecordFailedLogin();
            account.RecordFailedLogin();
            account.RecordFailedLogin();
            account.RecordFailedLogin(); // 4 attempts

            // Act
            bool shouldLock = account.ShouldLockAccount(5);

            // Assert
            Assert.IsFalse(shouldLock);
        }

        [Test]
        public void UserAccount_ShouldLockAccount_ReturnsTrue_WhenAttemptsAtThreshold()
        {
            // Arrange
            var account = new UserAccount("test@example.com");
            for (int i = 0; i < 5; i++)
            {
                account.RecordFailedLogin();
            } // 5 attempts

            // Act
            bool shouldLock = account.ShouldLockAccount(5);

            // Assert
            Assert.IsTrue(shouldLock);
        }

        [Test]
        public void UserAccount_ShouldLockAccount_ReturnsTrue_WhenAttemptsAboveThreshold()
        {
            // Arrange
            var account = new UserAccount("test@example.com");
            for (int i = 0; i < 6; i++)
            {
                account.RecordFailedLogin();
            } // 6 attempts

            // Act
            bool shouldLock = account.ShouldLockAccount(5);

            // Assert
            Assert.IsTrue(shouldLock);
        }

        [Test]
        public void UserAccount_ShouldLockAccount_ReturnsFalse_WhenAccountAlreadyLocked()
        {
            // Arrange
            var account = new UserAccount("test@example.com");
            account.LockAccount(); // Account is already locked

            // Act
            bool shouldLock = account.ShouldLockAccount(5);

            // Assert
            Assert.IsFalse(shouldLock);
        }

        [Test]
        public void UserAccount_LockAccount_SetsStatusToLocked()
        {
            // Arrange
            var account = new UserAccount("test@example.com");

            // Act
            account.LockAccount();

            // Assert
            Assert.IsTrue(account.IsLocked);
            Assert.AreEqual(AccountStatus.Locked, account.AccountStatus);
        }

        [Test]
        public void UserAccount_UnlockAccount_SetsStatusToActive()
        {
            // Arrange
            var account = new UserAccount("test@example.com");
            account.LockAccount();

            // Act
            account.UnlockAccount();

            // Assert
            Assert.IsTrue(account.IsActive);
            Assert.AreEqual(AccountStatus.Active, account.AccountStatus);
        }

        [Test]
        public void UserAccount_RecordSuccessfulLogin_ResetsLoginAttempts()
        {
            // Arrange
            var account = new UserAccount("test@example.com");
            account.RecordFailedLogin();
            account.RecordFailedLogin();
            account.RecordFailedLogin();

            // Act
            account.RecordSuccessfulLogin();

            // Assert
            Assert.AreEqual(0, account.LoginAttempts);
            Assert.IsTrue(account.IsActive);
        }

        [Test]
        public void UserAccount_RecordSuccessfulLogin_SetsLastLoginTimestamp()
        {
            // Arrange
            var account = new UserAccount("test@example.com");

            // Act
            account.RecordSuccessfulLogin();

            // Assert
            Assert.IsNotNull(account.LastLoginTimestamp);
            Assert.IsTrue(account.LastLoginTimestamp <= System.DateTime.UtcNow);
        }

        [Test]
        public void UserAccount_IsLocked_ReturnsTrue_WhenStatusIsLocked()
        {
            // Arrange
            var account = new UserAccount("test@example.com");
            account.LockAccount();

            // Act & Assert
            Assert.IsTrue(account.IsLocked);
        }

        [Test]
        public void UserAccount_IsActive_ReturnsTrue_WhenStatusIsActive()
        {
            // Arrange
            var account = new UserAccount("test@example.com");

            // Act & Assert
            Assert.IsTrue(account.IsActive);
        }
    }
}