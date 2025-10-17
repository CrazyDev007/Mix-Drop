using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using MixDrop.Login;
using MixDrop.Login.Services;
using MixDrop.Login.UI;

namespace MixDrop.Login.UI
{
    /// <summary>
    /// Controller for handling signup UI events and coordinating authentication flow
    /// </summary>
    public class SignupController : MonoBehaviour
    {
        [Header("UI Components")]
        public SignupUI signupUI;

        [Header("Scene Management")]
        public string mainGameSceneName = "GamePlay";

        private void Start()
        {
            if (signupUI == null)
            {
                signupUI = GetComponent<SignupUI>();
                if (signupUI == null)
                {
                    Debug.LogError("SignupUI component not found. Please assign it in the inspector or attach it to the same GameObject.");
                    return;
                }
            }

            // Subscribe to signup button click event
            signupUI.OnSignupAttempted += OnSignupButtonClicked;
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks
            if (signupUI != null)
            {
                signupUI.OnSignupAttempted -= OnSignupButtonClicked;
            }
        }

        /// <summary>
        /// Handle signup button click event
        /// </summary>
        private async void OnSignupButtonClicked()
        {
            string email = signupUI.GetEmail();
            string password = signupUI.GetPassword();
            string confirmPassword = signupUI.GetConfirmPassword();

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
                signupUI.SetErrorMessage("Invalid characters detected in input.");
                return false;
            }

            if (!MixDrop.Login.Models.UserAccount.IsValidEmail(email))
            {
                signupUI.SetErrorMessage("Please enter a valid email address.");
                return false;
            }

            if (!SecurityUtils.IsPasswordStrong(password))
            {
                signupUI.SetErrorMessage("Password must be at least 8 characters long and contain uppercase, lowercase, number, and special character.");
                return false;
            }

            // Check if passwords match
            if (password != confirmPassword)
            {
                signupUI.SetErrorMessage("Passwords do not match.");
                return false;
            }

            // Clear any previous error messages
            signupUI.SetErrorMessage(string.Empty);
            return true;
        }

        /// <summary>
        /// Perform the signup operation
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password</param>
        private async Task PerformSignup(string email, string password)
        {
            try
            {
                // Set loading state
                signupUI.SetLoadingState(true);
                signupUI.SetErrorMessage(string.Empty);

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
                signupUI.SetErrorMessage("An unexpected error occurred. Please try again.");
            }
            finally
            {
                // Reset loading state
                signupUI.SetLoadingState(false);
            }
        }

        /// <summary>
        /// Handle signup errors and display appropriate messages
        /// </summary>
        /// <param name="signupEx">The signup exception</param>
        private void HandleSignupError(LoginException signupEx)
        {
            string errorMessage = LoginUtils.GetErrorMessage(signupEx.ErrorType);

            signupUI.SetErrorMessage(errorMessage);
            LoginLogger.LogError($"Signup error: {errorMessage}");
        }
    }
}