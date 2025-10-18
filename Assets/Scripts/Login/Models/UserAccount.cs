using System;
using UnityEngine;

namespace MixDrop.Login.Models
{
    /// <summary>
    /// Represents a player's authentication account in the Mix-Drop game system
    /// </summary>
    [Serializable]
    public class UserAccount
    {
        /// <summary>
        /// User's email address used for login (unique identifier)
        /// </summary>
        [SerializeField]
        private string email;

        /// <summary>
        /// Securely hashed password (handled by Unity Authentication service)
        /// </summary>
        [SerializeField]
        private string passwordHash;

        /// <summary>
        /// Counter for failed login attempts
        /// </summary>
        [SerializeField]
        private int loginAttempts;

        /// <summary>
        /// Timestamp of last successful login
        /// </summary>
        [SerializeField]
        private DateTime? lastLoginTimestamp;

        /// <summary>
        /// Timestamp when account was locked (ticks, 0 if not locked)
        /// </summary>
        [SerializeField]
        private long lockoutUntilTicks;

        /// <summary>
        /// Current account status
        /// </summary>
        [SerializeField]
        private AccountStatus accountStatus;

        /// <summary>
        /// Timestamp when account was locked
        /// </summary>
        public DateTime? LockoutUntil
        {
            get => lockoutUntilTicks == 0 ? null : new DateTime(lockoutUntilTicks);
            set => lockoutUntilTicks = value?.Ticks ?? 0;
        }

        /// <summary>
        /// User's email address
        /// </summary>
        public string Email
        {
            get => email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email cannot be null or empty", nameof(Email));
                email = value.Trim().ToLower();
            }
        }

        /// <summary>
        /// Securely hashed password
        /// </summary>
        public string PasswordHash
        {
            get => passwordHash;
            set => passwordHash = value;
        }

        /// <summary>
        /// Number of failed login attempts
        /// </summary>
        public int LoginAttempts
        {
            get => loginAttempts;
            set => loginAttempts = Mathf.Max(0, value);
        }

        /// <summary>
        /// Timestamp of last successful login
        /// </summary>
        public DateTime? LastLoginTimestamp
        {
            get => lastLoginTimestamp;
            set => lastLoginTimestamp = value;
        }

        /// <summary>
        /// Current account status
        /// </summary>
        public AccountStatus AccountStatus
        {
            get => accountStatus;
            set => accountStatus = value;
        }

        /// <summary>
        /// Check if account is currently active
        /// </summary>
        public bool IsActive => accountStatus == AccountStatus.Active;

        /// <summary>
        /// Check if account is locked
        /// </summary>
        public bool IsLocked => accountStatus == AccountStatus.Locked;

        /// <summary>
        /// Check if account is currently locked out (considering time)
        /// </summary>
        public bool IsLockedOut()
        {
            return LockoutUntil.HasValue && DateTime.UtcNow < LockoutUntil.Value;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public UserAccount()
        {
            loginAttempts = 0;
            accountStatus = AccountStatus.Active;
        }

        /// <summary>
        /// Constructor with email
        /// </summary>
        /// <param name="email">User's email address</param>
        public UserAccount(string email) : this()
        {
            Email = email;
        }

        /// <summary>
        /// Record a successful login
        /// </summary>
        public void RecordSuccessfulLogin()
        {
            LoginAttempts = 0;
            LastLoginTimestamp = DateTime.UtcNow;
            AccountStatus = AccountStatus.Active;
            LockoutUntil = null;
        }

        /// <summary>
        /// Record a failed login attempt
        /// </summary>
        public void RecordFailedLogin()
        {
            LoginAttempts++;
        }

        /// <summary>
        /// Lock the account
        /// </summary>
        public void LockAccount()
        {
            AccountStatus = AccountStatus.Locked;
            LockoutUntil = DateTime.UtcNow.AddMinutes(15);
        }

        /// <summary>
        /// Unlock the account
        /// </summary>
        public void UnlockAccount()
        {
            AccountStatus = AccountStatus.Active;
            LockoutUntil = null;
        }

        /// <summary>
        /// Check if account should be locked based on failed attempts
        /// </summary>
        /// <param name="maxAttempts">Maximum allowed failed attempts</param>
        /// <returns>True if account should be locked</returns>
        public bool ShouldLockAccount(int maxAttempts = 5)
        {
            return LoginAttempts >= maxAttempts && IsActive;
        }

        /// <summary>
        /// Validate email format
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <returns>True if email format is valid</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate password strength
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>True if password meets minimum requirements</returns>
        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 8;
        }

        public override string ToString()
        {
            return $"UserAccount{{Email='{Email}', Status={AccountStatus}, Attempts={LoginAttempts}}}";
        }
    }

    /// <summary>
    /// Account status enumeration
    /// </summary>
    public enum AccountStatus
    {
        /// <summary>
        /// Account is active and can be used for login
        /// </summary>
        Active,

        /// <summary>
        /// Account is temporarily locked due to security reasons
        /// </summary>
        Locked
    }
}