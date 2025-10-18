using System;
using System.Threading.Tasks;
using MixDrop.Login;
using MixDrop.Login.Services;
using MixDrop.Login.UI;

namespace MixDrop.Login.UI
{
    /// <summary>
    /// Presenter for handling login UI events and coordinating authentication flow
    /// </summary>
    public class LoginPresenter
    {
        private readonly LoginScreen loginUI;

        public event Action OnLoginSuccessful;

        public LoginPresenter(LoginScreen view)
        {
            loginUI = view ?? throw new ArgumentNullException(nameof(view));

            // Subscribe to login button click event
            loginUI.OnLoginAttempted += OnLoginButtonClicked;
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

                // Raise event for successful login
                OnLoginSuccessful?.Invoke();

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
                    int minutes = (int)Math.Ceiling(remaining.TotalMinutes);
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