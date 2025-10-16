using NUnit.Framework;
using MixDrop.Login.Models;

namespace MixDrop.Login.Tests.Authentication
{
    public class UserAccountValidationTests
    {
        [Test]
        public void IsValidEmail_NullEmail_ReturnsFalse()
        {
            // Arrange
            string nullEmail = null;

            // Act
            bool isValid = UserAccount.IsValidEmail(nullEmail);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidEmail_EmptyEmail_ReturnsFalse()
        {
            // Arrange
            string emptyEmail = "";

            // Act
            bool isValid = UserAccount.IsValidEmail(emptyEmail);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidEmail_WhitespaceEmail_ReturnsFalse()
        {
            // Arrange
            string whitespaceEmail = "   ";

            // Act
            bool isValid = UserAccount.IsValidEmail(whitespaceEmail);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidEmail_ValidEmail_ReturnsTrue()
        {
            // Arrange
            string validEmail = "test@example.com";

            // Act
            bool isValid = UserAccount.IsValidEmail(validEmail);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void IsValidEmail_EmailWithSubdomain_ReturnsTrue()
        {
            // Arrange
            string validEmail = "test@sub.example.com";

            // Act
            bool isValid = UserAccount.IsValidEmail(validEmail);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void IsValidEmail_EmailWithNumbers_ReturnsTrue()
        {
            // Arrange
            string validEmail = "test123@example.com";

            // Act
            bool isValid = UserAccount.IsValidEmail(validEmail);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void IsValidEmail_EmailWithUnderscore_ReturnsTrue()
        {
            // Arrange
            string validEmail = "test_user@example.com";

            // Act
            bool isValid = UserAccount.IsValidEmail(validEmail);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void IsValidEmail_EmailWithoutAt_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "testexample.com";

            // Act
            bool isValid = UserAccount.IsValidEmail(invalidEmail);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidEmail_EmailWithoutDomain_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "test@";

            // Act
            bool isValid = UserAccount.IsValidEmail(invalidEmail);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidEmail_EmailWithMultipleAt_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "test@@example.com";

            // Act
            bool isValid = UserAccount.IsValidEmail(invalidEmail);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidEmail_EmailWithSpaces_ReturnsFalse()
        {
            // Arrange
            string invalidEmail = "test @ example.com";

            // Act
            bool isValid = UserAccount.IsValidEmail(invalidEmail);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidPassword_NullPassword_ReturnsFalse()
        {
            // Arrange
            string nullPassword = null;

            // Act
            bool isValid = UserAccount.IsValidPassword(nullPassword);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidPassword_EmptyPassword_ReturnsFalse()
        {
            // Arrange
            string emptyPassword = "";

            // Act
            bool isValid = UserAccount.IsValidPassword(emptyPassword);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidPassword_WhitespacePassword_ReturnsFalse()
        {
            // Arrange
            string whitespacePassword = "   ";

            // Act
            bool isValid = UserAccount.IsValidPassword(whitespacePassword);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidPassword_PasswordTooShort_ReturnsFalse()
        {
            // Arrange
            string shortPassword = "12345"; // 5 characters

            // Act
            bool isValid = UserAccount.IsValidPassword(shortPassword);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValidPassword_PasswordExactly8Characters_ReturnsTrue()
        {
            // Arrange
            string validPassword = "12345678"; // 8 characters

            // Act
            bool isValid = UserAccount.IsValidPassword(validPassword);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void IsValidPassword_PasswordLongerThan8Characters_ReturnsTrue()
        {
            // Arrange
            string validPassword = "123456789"; // 9 characters

            // Act
            bool isValid = UserAccount.IsValidPassword(validPassword);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void UserAccount_Constructor_NullEmail_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<System.ArgumentException>(() => new UserAccount(null));
        }

        [Test]
        public void UserAccount_Constructor_EmptyEmail_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<System.ArgumentException>(() => new UserAccount(""));
        }

        [Test]
        public void UserAccount_Constructor_WhitespaceEmail_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<System.ArgumentException>(() => new UserAccount("   "));
        }

        [Test]
        public void UserAccount_Email_Property_NullValue_ThrowsException()
        {
            // Arrange
            var account = new UserAccount("initial@example.com");

            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => account.Email = null);
        }

        [Test]
        public void UserAccount_Email_Property_EmptyValue_ThrowsException()
        {
            // Arrange
            var account = new UserAccount("initial@example.com");

            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => account.Email = "");
        }

        [Test]
        public void UserAccount_Email_Property_WhitespaceValue_ThrowsException()
        {
            // Arrange
            var account = new UserAccount("initial@example.com");

            // Act & Assert
            Assert.Throws<System.ArgumentException>(() => account.Email = "   ");
        }

        [Test]
        public void UserAccount_Email_Property_TrimsAndLowercasesValue()
        {
            // Arrange
            var account = new UserAccount("initial@example.com");

            // Act
            account.Email = "  TEST@EXAMPLE.COM  ";

            // Assert
            Assert.AreEqual("test@example.com", account.Email);
        }

        [Test]
        public void UserAccount_LoginAttempts_Property_NegativeValue_ClampedToZero()
        {
            // Arrange
            var account = new UserAccount("test@example.com");

            // Act
            account.LoginAttempts = -5;

            // Assert
            Assert.AreEqual(0, account.LoginAttempts);
        }

        [Test]
        public void UserAccount_LoginAttempts_Property_PositiveValue_Accepted()
        {
            // Arrange
            var account = new UserAccount("test@example.com");

            // Act
            account.LoginAttempts = 3;

            // Assert
            Assert.AreEqual(3, account.LoginAttempts);
        }

        [Test]
        public void UserAccount_IsLockedOut_AccountNotLocked_ReturnsFalse()
        {
            // Arrange
            var account = new UserAccount("test@example.com");

            // Act
            bool isLockedOut = account.IsLockedOut();

            // Assert
            Assert.IsFalse(isLockedOut);
        }

        [Test]
        public void UserAccount_IsLockedOut_AccountLockedButTimeExpired_ReturnsFalse()
        {
            // Arrange
            var account = new UserAccount("test@example.com");
            account.LockAccount();
            // Simulate time passing by setting lockout time to past
            account.LockoutUntil = System.DateTime.UtcNow.AddMinutes(-1);

            // Act
            bool isLockedOut = account.IsLockedOut();

            // Assert
            Assert.IsFalse(isLockedOut);
        }

        [Test]
        public void UserAccount_IsLockedOut_AccountLockedAndTimeNotExpired_ReturnsTrue()
        {
            // Arrange
            var account = new UserAccount("test@example.com");
            account.LockAccount();
            // Lockout time is set to future in LockAccount()

            // Act
            bool isLockedOut = account.IsLockedOut();

            // Assert
            Assert.IsTrue(isLockedOut);
        }

        [Test]
        public void UserAccount_ToString_ReturnsExpectedFormat()
        {
            // Arrange
            var account = new UserAccount("test@example.com");
            account.RecordFailedLogin();
            account.RecordFailedLogin();

            // Act
            string result = account.ToString();

            // Assert
            StringAssert.Contains("UserAccount{Email='test@example.com'", result);
            StringAssert.Contains("Status=Active", result);
            StringAssert.Contains("Attempts=2", result);
        }
    }
}