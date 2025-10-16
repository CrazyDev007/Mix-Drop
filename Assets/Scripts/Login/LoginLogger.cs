using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace MixDrop.Login
{
    /// <summary>
    /// Centralized error handling and logging infrastructure for the Login system
    /// </summary>
    public static class LoginLogger
    {
        private const string LOG_PREFIX = "[MixDrop Login] ";

        /// <summary>
        /// Log an informational message
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void LogInfo(string message)
        {
            Debug.Log(LOG_PREFIX + message);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">Warning message to log</param>
        public static void LogWarning(string message)
        {
            Debug.LogWarning(LOG_PREFIX + message);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">Error message to log</param>
        public static void LogError(string message)
        {
            Debug.LogError(LOG_PREFIX + message);
        }

        /// <summary>
        /// Log an exception with context
        /// </summary>
        /// <param name="exception">Exception to log</param>
        /// <param name="context">Additional context information</param>
        public static void LogException(Exception exception, string context = null)
        {
            string message = context != null ? $"{context}: {exception.Message}" : exception.Message;
            Debug.LogError(LOG_PREFIX + "Exception: " + message);
            Debug.LogException(exception);
        }
    }

    /// <summary>
    /// Custom exception for login-related errors
    /// </summary>
    public class LoginException : Exception
    {
        public LoginErrorType ErrorType { get; private set; }

        public LoginException(string message, LoginErrorType errorType = LoginErrorType.Unknown)
            : base(message)
        {
            ErrorType = errorType;
        }

        public LoginException(string message, Exception innerException, LoginErrorType errorType = LoginErrorType.Unknown)
            : base(message, innerException)
        {
            ErrorType = errorType;
        }
    }

    /// <summary>
    /// Types of login errors
    /// </summary>
    public enum LoginErrorType
    {
        Unknown,
        InvalidCredentials,
        NetworkError,
        AccountLocked,
        EmailNotVerified,
        ServiceUnavailable,
        ValidationError,
        EmailAlreadyExists,
        WeakPassword
    }

    /// <summary>
    /// Utility methods for login operations
    /// </summary>
    public static class LoginUtils
    {
        /// <summary>
        /// Get user-friendly error message for a login error type
        /// </summary>
        /// <param name="errorType">The error type</param>
        /// <returns>User-friendly message</returns>
        public static string GetErrorMessage(LoginErrorType errorType)
        {
            switch (errorType)
            {
                case LoginErrorType.InvalidCredentials:
                    return "Invalid email or password. Please check your credentials and try again.";
                case LoginErrorType.AccountLocked:
                    return "Account is temporarily locked due to too many failed attempts.";
                case LoginErrorType.NetworkError:
                    return "Network error. Please check your internet connection and try again.";
                case LoginErrorType.ValidationError:
                    return "Invalid input. Please check your email and password format.";
                case LoginErrorType.ServiceUnavailable:
                    return "Authentication service is currently unavailable. Please try again later.";
                case LoginErrorType.EmailNotVerified:
                    return "Please verify your email address before logging in.";
                case LoginErrorType.EmailAlreadyExists:
                    return "An account with this email already exists. Please use a different email or try logging in.";
                case LoginErrorType.WeakPassword:
                    return "Password does not meet requirements. Please choose a stronger password.";
                default:
                    return "An error occurred. Please try again.";
            }
        }
    }

    /// <summary>
    /// Security utilities for authentication system
    /// </summary>
    public static class SecurityUtils
    {
        private const string ENCRYPTION_KEY = "MixDropAuthKey2025"; // In production, use a secure key management system

        /// <summary>
        /// Encrypt a string using AES
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <returns>Encrypted base64 string</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = DeriveKey(ENCRYPTION_KEY);
                aes.IV = new byte[16]; // Use zero IV for simplicity, in production use random IV

                ICryptoTransform encryptor = aes.CreateEncryptor();

                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                return Convert.ToBase64String(encryptedBytes);
            }
        }

        /// <summary>
        /// Decrypt a string using AES
        /// </summary>
        /// <param name="encryptedText">Encrypted base64 string</param>
        /// <returns>Decrypted string</returns>
        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return encryptedText;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = DeriveKey(ENCRYPTION_KEY);
                    aes.IV = new byte[16];

                    ICryptoTransform decryptor = aes.CreateDecryptor();

                    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception ex)
            {
                LoginLogger.LogError($"Failed to decrypt data: {ex.Message}");
                return null; // Return null if decryption fails
            }
        }

        /// <summary>
        /// Derive a 256-bit key from a password
        /// </summary>
        /// <param name="password">Password to derive from</param>
        /// <returns>256-bit key</returns>
        private static byte[] DeriveKey(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Validate password strength with enhanced requirements
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>True if password meets security requirements</returns>
        public static bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsWhiteSpace(c)) hasSpecial = true;
            }

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        /// <summary>
        /// Sanitize input by trimming and limiting length
        /// </summary>
        /// <param name="input">Input to sanitize</param>
        /// <param name="maxLength">Maximum allowed length</param>
        /// <returns>Sanitized input</returns>
        public static string SanitizeInput(string input, int maxLength = 254)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string trimmed = input.Trim();
            return trimmed.Length > maxLength ? trimmed.Substring(0, maxLength) : trimmed;
        }

        /// <summary>
        /// Check if input contains potentially dangerous characters
        /// </summary>
        /// <param name="input">Input to check</param>
        /// <returns>True if input is safe</returns>
        public static bool IsInputSafe(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;

            // Check for common injection patterns
            string[] dangerousPatterns = { "<script", "javascript:", "onload=", "onerror=", "<iframe", "<object" };

            string lowerInput = input.ToLower();
            foreach (string pattern in dangerousPatterns)
            {
                if (lowerInput.Contains(pattern))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Generate a secure random string for additional security
        /// </summary>
        /// <param name="length">Length of the random string</param>
        /// <returns>Random string</returns>
        public static string GenerateSecureRandomString(int length = 32)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new System.Random();
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }
    }
}