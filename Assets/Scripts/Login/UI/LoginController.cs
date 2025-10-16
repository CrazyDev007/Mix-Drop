using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using MixDrop.Login;
using MixDrop.Login.Services;
using MixDrop.Login.UI;

namespace MixDrop.Login.UI
{
    /// <summary>
    /// Controller for handling login UI events and coordinating authentication flow
    /// </summary>
    public class LoginController : MonoBehaviour
    {
    [Header("UI Components")]
    public LoginUI loginUI;

    [Header("Scene Management")]
    public string mainGameSceneName = "GamePlay";

        private void Start()
        {
            if (loginUI == null)
            {
                loginUI = GetComponent<LoginUI>();
                if (loginUI == null)
                {
                    Debug.LogError("LoginUI component not found. Please assign it in the inspector or attach it to the same GameObject.");
                    return;
                }
            }

            // Subscribe to login button click event
            loginUI.OnLoginAttempted += OnLoginButtonClicked;
            loginUI.OnSignupLinkClickedEvent += OnSignupLinkClicked;
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (loginUI != null)
            {
                loginUI.OnLoginAttempted -= OnLoginButtonClicked;
                loginUI.OnSignupLinkClickedEvent -= OnSignupLinkClicked;
            }
        }

        /// <summary>
        /// Handle login button click event
        /// </summary>
        private async void OnLoginButtonClicked()
        {
            string email = loginUI.GetEmail();
            string password = loginUI.GetPassword();

            // Basic client-side validation
            if (!IsValidInput(email, password))
            {
                return; // Error message already set by validation method
            }

            // Start login process
            await PerformLogin(email, password);
        }

        /// <summary>
        /// Handle signup link click event
        /// </summary>
        private void OnSignupLinkClicked()
        {
            // Navigate to signup screen by activating signup controller and deactivating self
            GameObject signupController = GameObject.Find("SignupController");
            if (signupController != null)
            {
                signupController.SetActive(true);
            }
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Validate user input before attempting login
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <returns>True if input is valid</returns>
        private bool IsValidInput(string email, string password)
        {
            // Sanitize inputs
            email = SecurityUtils.SanitizeInput(email);
            password = SecurityUtils.SanitizeInput(password);

            // Check for safe input
            if (!SecurityUtils.IsInputSafe(email) || !SecurityUtils.IsInputSafe(password))
            {
                loginUI.SetErrorMessage("Invalid characters detected in input.");
                return false;
            }

            if (!MixDrop.Login.Models.UserAccount.IsValidEmail(email))
            {
                loginUI.SetErrorMessage("Please enter a valid email address.");
                return false;
            }

            if (!SecurityUtils.IsPasswordStrong(password))
            {
                loginUI.SetErrorMessage("Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character.");
                return false;
            }

            // Clear any previous error messages
            loginUI.SetErrorMessage(string.Empty);
            return true;
        }

        /// <summary>
        /// Perform the login operation
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        private async Task PerformLogin(string email, string password)
        {
            try
            {
                // Set loading state
                loginUI.SetLoadingState(true);
                loginUI.SetErrorMessage(string.Empty);

                // Attempt authentication
                await AuthenticationService.Instance.SignInWithEmailPassword(email, password);

                // Login successful
                LoginLogger.LogInfo($"Login successful for user: {AuthenticationService.Instance.UserId}");

                // Transition to main game scene
                SceneManager.LoadScene(mainGameSceneName);

            }
            catch (LoginException loginEx)
            {
                // Handle login-specific errors
                HandleLoginError(loginEx);
            }
            catch (System.Exception ex)
            {
                // Handle unexpected errors
                LoginLogger.LogException(ex, "Unexpected error during login process");
                loginUI.SetErrorMessage("An unexpected error occurred. Please try again.");
            }
            finally
            {
                // Reset loading state
                loginUI.SetLoadingState(false);
            }
        }

        /// <summary>
        /// Handle login errors and display appropriate messages
        /// </summary>
        /// <param name="loginEx">The login exception</param>
        private void HandleLoginError(LoginException loginEx)
        {
            string errorMessage = LoginUtils.GetErrorMessage(loginEx.ErrorType);

            if (loginEx.ErrorType == LoginErrorType.AccountLocked)
            {
                // For lockout, show in lockout text with countdown if possible
                string email = loginUI.GetEmail();
                var userAccount = AuthenticationService.Instance.LoadUserAccount(email);
                if (userAccount != null && userAccount.IsLockedOut())
                {
                    TimeSpan remaining = userAccount.LockoutUntil.Value - DateTime.UtcNow;
                    int minutes = Mathf.CeilToInt((float)remaining.TotalMinutes);
                    loginUI.SetLockoutMessage($"Account locked. Try again in {minutes} minute(s).");
                }
                else
                {
                    loginUI.SetLockoutMessage(errorMessage);
                }
                return; // Don't set error message
            }

            loginUI.SetErrorMessage(errorMessage);
            LoginLogger.LogError($"Login error: {errorMessage}");
        }
    }
}