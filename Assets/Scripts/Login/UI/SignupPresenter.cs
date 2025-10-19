using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using MixDrop.Login;
using MixDrop.Login.Services;
using MixDrop.Login.UI;

namespace MixDrop.Login.UI
{
    /// <summary>
    /// Presenter for handling signup UI events and coordinating authentication flow
    /// </summary>
    public class SignupPresenter
    {
        private SignupScreen signupScreen;
        private string mainGameSceneName = "Main";

        public SignupPresenter(SignupScreen signupScreen)
        {
            this.signupScreen = signupScreen;
            // Subscribe to signup button click event
            signupScreen.OnSignupAttempted += OnSignupButtonClicked;
        }

        public void Dispose()
        {
            // Unsubscribe from events to prevent memory leaks
            if (signupScreen != null)
            {
                signupScreen.OnSignupAttempted -= OnSignupButtonClicked;
            }
        }

        /// <summary>
        /// Handle signup button click event
        /// </summary>
        private async void OnSignupButtonClicked()
        {
            string email = signupScreen.GetEmail();
            string password = signupScreen.GetPassword();
            string confirmPassword = signupScreen.GetConfirmPassword();

            // Basic client-side validation
            if (!IsValidInput(email, password, confirmPassword))
            {
                return; // Error message already set by validation method
            }

            // Start signup process
            await PerformSignup(email, password);
        }

        /// <summary>
        /// Validate user input before attempting signup
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        /// <param name="confirmPassword">User's confirm password</param>
        /// <returns>True if input is valid</returns>
        private bool IsValidInput(string email, string password, string confirmPassword)
        {
            // Sanitize inputs
            email = SecurityUtils.SanitizeInput(email);
            password = SecurityUtils.SanitizeInput(password);
            confirmPassword = SecurityUtils.SanitizeInput(confirmPassword);

            // Check for safe input
            if (!SecurityUtils.IsInputSafe(email) || !SecurityUtils.IsInputSafe(password) || !SecurityUtils.IsInputSafe(confirmPassword))
            {
                signupScreen.SetErrorMessage("Invalid characters detected in input.");
                return false;
            }

            if (!MixDrop.Login.Models.UserAccount.IsValidEmail(email))
            {
                signupScreen.SetErrorMessage("Please enter a valid email address.");
                return false;
            }

            if (!SecurityUtils.IsPasswordStrong(password))
            {
                signupScreen.SetErrorMessage("Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character.");
                return false;
            }

            // Check if passwords match
            if (password != confirmPassword)
            {
                signupScreen.SetErrorMessage("Passwords do not match.");
                return false;
            }

            // Clear any previous error messages
            signupScreen.SetErrorMessage(string.Empty);
            return true;
        }

        /// <summary>
        /// Perform the signup operation
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        private async Task PerformSignup(string email, string password)
        {
            // Ensure Unity Services are initialized
            if (Unity.Services.Core.UnityServices.State != Unity.Services.Core.ServicesInitializationState.Initialized)
            {
                await Unity.Services.Core.UnityServices.InitializeAsync();
            }

            try
            {
                // Set loading state
                signupScreen.SetLoadingState(true);
                signupScreen.SetErrorMessage(string.Empty);

                // Attempt account creation
                await AuthenticationService.Instance.SignUpWithEmailPassword(email, password);

                // Signup successful - user is automatically logged in
                LoginLogger.LogInfo($"Signup successful for user: {AuthenticationService.Instance.UserId}");

                // Transition to main game scene
                SceneManager.LoadScene(mainGameSceneName);

            }
            catch (LoginException signupEx)
            {
                // Handle signup-specific errors
                HandleSignupError(signupEx);
            }
            catch (System.Exception ex)
            {
                // Handle unexpected errors
                LoginLogger.LogException(ex, "Unexpected error during signup process");
                signupScreen.SetErrorMessage("An unexpected error occurred. Please try again.");
            }
            finally
            {
                // Reset loading state
                signupScreen.SetLoadingState(false);
            }
        }

        /// <summary>
        /// Handle signup errors and display appropriate messages
        /// </summary>
        /// <param name="signupEx">The signup exception</param>
        private void HandleSignupError(LoginException signupEx)
        {
            string errorMessage = LoginUtils.GetErrorMessage(signupEx.ErrorType);

            signupScreen.SetErrorMessage(errorMessage);
            LoginLogger.LogError($"Signup error: {errorMessage}");
        }
    }
}