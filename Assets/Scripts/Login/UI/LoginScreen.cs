using UnityEngine;
using UnityEngine.UIElements;
using UI;
using ScreenFlow;
using UnityEngine.SceneManagement;

namespace MixDrop.Login.UI
{
    /// <summary>
    /// Base UI script for handling Login screen interactions with Unity UI Toolkit
    /// </summary>
    public class LoginScreen : ScreenUI
    {
        [Header("UI Elements")]
        [SerializeField] private string emailFieldName = "email-field";
        [SerializeField] private string passwordFieldName = "password-field";
        [SerializeField] private string loginButtonName = "login-button";
        [SerializeField] private string errorTextName = "error-text";
        [SerializeField] private string lockoutTextName = "lockout-text";
        [SerializeField] private string signupLinkName = "signup-link";

        private TextField emailField;
        private TextField passwordField;
        private Button loginButton;
        private Label errorText;
        private Label lockoutText;
        private Button signupLink;

        private LoginPresenter presenter;

        private void InitializeUIElements(VisualElement root)
        {
            emailField = root.Q<TextField>(emailFieldName);
            passwordField = root.Q<TextField>(passwordFieldName);
            loginButton = root.Q<Button>(loginButtonName);
            errorText = root.Q<Label>(errorTextName);
            lockoutText = root.Q<Label>(lockoutTextName);
            signupLink = root.Q<Button>(signupLinkName);

            if (emailField == null) Debug.LogWarning($"Email field '{emailFieldName}' not found");
            if (passwordField == null) Debug.LogWarning($"Password field '{passwordFieldName}' not found");
            if (loginButton == null) Debug.LogWarning($"Login button '{loginButtonName}' not found");
            if (errorText == null) Debug.LogWarning($"Error text '{errorTextName}' not found");
            if (lockoutText == null) Debug.LogWarning($"Lockout text '{lockoutTextName}' not found");
            if (signupLink == null) Debug.LogWarning($"Signup link '{signupLinkName}' not found");
        }

        private void SetupEventHandlers()
        {
            if (loginButton != null)
            {
                loginButton.clicked += OnLoginButtonClicked;
            }
            if (signupLink != null)
            {
                signupLink.clicked += OnSignupLinkClicked;
            }
        }

        private void OnLoginButtonClicked()
        {
            // Raise the login attempt event
            OnLoginAttempted?.Invoke();
        }

        private void OnSignupLinkClicked()
        {
            ScreenManager.Instance.ShowScreen("SignupScreen");
        }

        /// <summary>
        /// Event raised when user attempts to login
        /// </summary>
        public event System.Action OnLoginAttempted;

        /// <summary>
        /// Get the current email input value
        /// </summary>
        public string GetEmail()
        {
            return emailField?.value ?? string.Empty;
        }

        /// <summary>
        /// Get the current password input value
        /// </summary>
        public string GetPassword()
        {
            return passwordField?.value ?? string.Empty;
        }

        /// <summary>
        /// Set the error message to display
        /// </summary>
        /// <param name="message">Error message to show</param>
        public void SetErrorMessage(string message)
        {
            if (errorText != null)
            {
                errorText.text = message;
                errorText.style.display = string.IsNullOrEmpty(message) ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        /// <summary>
        /// Set the lockout message to display
        /// </summary>
        /// <param name="message">Lockout message to show</param>
        public void SetLockoutMessage(string message)
        {
            if (lockoutText != null)
            {
                lockoutText.text = message;
                lockoutText.style.display = string.IsNullOrEmpty(message) ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        /// <summary>
        /// Clear all input fields
        /// </summary>
        public void ClearInputs()
        {
            if (emailField != null) emailField.value = string.Empty;
            if (passwordField != null) passwordField.value = string.Empty;
            SetErrorMessage(string.Empty);
            SetLockoutMessage(string.Empty);
        }

        /// <summary>
        /// Set the loading state of the UI
        /// </summary>
        /// <param name="isLoading">Whether login is in progress</param>
        public void SetLoadingState(bool isLoading)
        {
            if (loginButton != null)
            {
                loginButton.SetEnabled(!isLoading);
                loginButton.text = isLoading ? "Logging in..." : "Login";
            }
        }
        protected override void SetupScreen(VisualElement screen)
        {
            InitializeUIElements(screen);
            SetupEventHandlers();
            presenter = new LoginPresenter(this);
            presenter.OnLoginSuccessful += () => SceneManager.LoadScene("Main");
        }
    }
}