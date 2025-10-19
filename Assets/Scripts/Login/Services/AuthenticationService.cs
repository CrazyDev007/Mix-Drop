using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using MixDrop.Login;

namespace MixDrop.Login.Services
{
    /// <summary>
    /// Service for handling Unity Authentication operations
    /// </summary>
    public class AuthenticationService : MonoBehaviour
    {
        private static AuthenticationService _instance;
        public static AuthenticationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("AuthenticationService");
                    _instance = go.AddComponent<AuthenticationService>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        /// <summary>
        /// Check if user is currently signed in
        /// </summary>
        public bool IsSignedIn => Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn;

        /// <summary>
        /// Get the current user's ID
        /// </summary>
        public string UserId => Unity.Services.Authentication.AuthenticationService.Instance.PlayerId;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Save user account data locally with encryption
        /// </summary>
        /// <param name="account">User account to save</param>
        private void SaveUserAccount(MixDrop.Login.Models.UserAccount account)
        {
            string json = JsonUtility.ToJson(account);
            string encryptedJson = SecurityUtils.Encrypt(json);
            PlayerPrefs.SetString($"UserAccount_{account.Email}", encryptedJson);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load user account data locally with decryption
        /// </summary>
        /// <param name="email">User's email</param>
        /// <returns>User account or null if not found</returns>
        public MixDrop.Login.Models.UserAccount LoadUserAccount(string email)
        {
            string key = $"UserAccount_{email}";
            if (PlayerPrefs.HasKey(key))
            {
                string encryptedJson = PlayerPrefs.GetString(key);
                string json = SecurityUtils.Decrypt(encryptedJson);
                if (json != null)
                {
                    return JsonUtility.FromJson<MixDrop.Login.Models.UserAccount>(json);
                }
                else
                {
                    LoginLogger.LogError($"Failed to decrypt user account data for {email}");
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Sign in with email and password
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="password">User's password</param>
        /// <returns>Task representing the async operation</returns>
        public async Task SignInWithEmailPassword(string email, string password)
        {
            // Ensure Unity Services are initialized
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
            }

            // Load or create user account for local state management
            var userAccount = LoadUserAccount(email) ?? new MixDrop.Login.Models.UserAccount(email);

            // Check if account is currently locked out
            if (userAccount.IsLockedOut())
            {
                throw new LoginException("Account is temporarily locked due to too many failed attempts", LoginErrorType.AccountLocked);
            }

            try
            {
                LoginLogger.LogInfo($"Attempting login for email: {email}");

                // Validate input
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    throw new LoginException("Email and password are required", LoginErrorType.ValidationError);
                }

                // Call Unity Authentication service
                await Unity.Services.Authentication.AuthenticationService.Instance.SignInWithUsernamePasswordAsync(email, password);

                LoginLogger.LogInfo($"Login successful for user: {UserId}");

                // Record successful login
                userAccount.RecordSuccessfulLogin();
                SaveUserAccount(userAccount);

                // Store session securely
                UserSessionManager.Instance.StoreSession();

            }
            catch (AuthenticationException authEx)
            {
                var (errorType, errorMessage) = MapAuthenticationError(authEx, "login");

                // Record failed login attempt for invalid credentials
                if (errorType == LoginErrorType.InvalidCredentials)
                {
                    userAccount.RecordFailedLogin();
                    if (userAccount.ShouldLockAccount())
                    {
                        userAccount.LockAccount();
                    }
                    SaveUserAccount(userAccount);
                }

                LoginLogger.LogError($"Login failed for {email}: {errorMessage}");
                throw new LoginException(errorMessage, errorType);
            }
            catch (RequestFailedException requestEx)
            {
                LoginLogger.LogError($"Network error during login for {email}: {requestEx.Message}");
                throw new LoginException("Network error - please check your connection", LoginErrorType.NetworkError);
            }
            catch (Exception ex)
            {
                LoginLogger.LogException(ex, $"Unexpected error during login for {email}");
                throw new LoginException("An unexpected error occurred", LoginErrorType.Unknown);
            }
        }

        /// <summary>
        /// Sign up with email and password
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="password">User's password</param>
        /// <returns>Task representing the async operation</returns>
        public async Task SignUpWithEmailPassword(string email, string password)
        {
            // Ensure Unity Services are initialized
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await UnityServices.InitializeAsync();
            }

            try
            {
                LoginLogger.LogInfo($"Attempting signup for email: {email}");

                // Validate input
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    throw new LoginException("Email and password are required", LoginErrorType.ValidationError);
                }

                // Call Unity Authentication service
                // Note: Unity Authentication SDK handles secure communication over HTTPS
                await Unity.Services.Authentication.AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(email, password);

                LoginLogger.LogInfo($"Signup successful for user: {UserId}");

                // Create user account record and record successful login
                var userAccount = new MixDrop.Login.Models.UserAccount(email);
                userAccount.RecordSuccessfulLogin();
                SaveUserAccount(userAccount);

                // Store session securely
                UserSessionManager.Instance.StoreSession();

            }
            catch (AuthenticationException authEx)
            {
                var (errorType, errorMessage) = MapAuthenticationError(authEx, "signup");

                LoginLogger.LogError($"Signup failed for {email}: {errorMessage}");
                throw new LoginException(errorMessage, errorType);
            }
            catch (RequestFailedException requestEx)
            {
                LoginLogger.LogError($"Network error during signup for {email}: {requestEx.Message}");
                throw new LoginException("Network error - please check your connection", LoginErrorType.NetworkError);
            }
            catch (Exception ex)
            {
                LoginLogger.LogException(ex, $"Unexpected error during signup for {email}");
                throw new LoginException("An unexpected error occurred", LoginErrorType.Unknown);
            }
        }

        /// <summary>
        /// Sign out the current user</summary>
        public void SignOut()
        {
            Unity.Services.Authentication.AuthenticationService.Instance.SignOut();
        }

        /// <summary>
        /// Delete the current user's account
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        public async Task DeleteAccount()
        {
            await Unity.Services.Authentication.AuthenticationService.Instance.DeleteAccountAsync();
        }

        /// <summary>
        /// Map AuthenticationException to LoginErrorType and user-friendly message
        /// </summary>
        /// <param name="authEx">The authentication exception</param>
        /// <param name="operation">The operation being performed (login/signup)</param>
        /// <returns>Tuple of error type and message</returns>
        private (LoginErrorType errorType, string message) MapAuthenticationError(AuthenticationException authEx, string operation)
        {
            string errorMsg = authEx.Message.ToLower();

            if (errorMsg.Contains("invalid") || errorMsg.Contains("parameter"))
            {
                return (LoginErrorType.ValidationError, $"Invalid email or password format for {operation}");
            }
            else if (errorMsg.Contains("credential") || errorMsg.Contains("not found"))
            {
                return (LoginErrorType.InvalidCredentials, "Invalid email or password");
            }
            else if (errorMsg.Contains("locked") || errorMsg.Contains("temporarily"))
            {
                return (LoginErrorType.AccountLocked, "Account is temporarily locked due to too many failed attempts");
            }
            else if (operation == "signup" && (errorMsg.Contains("already") || errorMsg.Contains("exists")))
            {
                return (LoginErrorType.EmailAlreadyExists, "An account with this email already exists");
            }
            else if (operation == "signup" && errorMsg.Contains("password") && errorMsg.Contains("requirements"))
            {
                return (LoginErrorType.WeakPassword, "Password does not meet strength requirements");
            }
            else
            {
                return (LoginErrorType.Unknown, $"{operation} failed");
            }
        }
    }
}