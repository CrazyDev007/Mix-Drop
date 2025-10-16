using UnityEngine;
using UnityEngine.UIElements;

namespace MixDrop.Login.UI
{
    /// <summary>
    /// Base UI script for handling Signup screen interactions with Unity UI Toolkit
    /// </summary>
    public class SignupUI : MonoBehaviour
    {
        [Header("UI Document")]
        [SerializeField] public UIDocument uiDocument;

        [Header("UI Elements")]
        [SerializeField] private string emailFieldName = "email-field";
        [SerializeField] private string passwordFieldName = "password-field";
        [SerializeField] private string confirmPasswordFieldName = "confirm-password-field";
        [SerializeField] private string signupButtonName = "signup-button";
        [SerializeField] private string errorTextName = "error-text";
        [SerializeField] private string loginLinkName = "login-link";

        private TextField emailField;
        private TextField passwordField;
        private TextField confirmPasswordField;
        private Button signupButton;
        private Label errorText;
        private Button loginLink;

        private bool isInitialized = false;

        private void OnEnable()
        {
            if (uiDocument == null)
            {
                Debug.LogError("UIDocument not assigned to SignupUI");
                return;
            }

            if (!isInitialized)
            {
                InitializeUIElements();
                isInitialized = true;
            }
            SetupEventHandlers();
        }

        private void OnDisable()
        {
            if (signupButton != null)
            {
                signupButton.clicked -= OnSignupButtonClicked;
            }
            if (loginLink != null)
            {
                loginLink.clicked -= OnLoginLinkClicked;
            }
        }

        private void InitializeUIElements()
        {
            var root = uiDocument.rootVisualElement;

            emailField = root.Q<TextField>(emailFieldName);
            passwordField = root.Q<TextField>(passwordFieldName);
            confirmPasswordField = root.Q<TextField>(confirmPasswordFieldName);
            signupButton = root.Q<Button>(signupButtonName);
            errorText = root.Q<Label>(errorTextName);
            loginLink = root.Q<Button>(loginLinkName);

            if (emailField == null) Debug.LogWarning($"Email field '{emailFieldName}' not found");
            if (passwordField == null) Debug.LogWarning($"Password field '{passwordFieldName}' not found");
            if (confirmPasswordField == null) Debug.LogWarning($"Confirm password field '{confirmPasswordFieldName}' not found");
            if (signupButton == null) Debug.LogWarning($"Signup button '{signupButtonName}' not found");
            if (errorText == null) Debug.LogWarning($"Error text '{errorTextName}' not found");
            if (loginLink == null) Debug.LogWarning($"Login link '{loginLinkName}' not found");
        }

        private void SetupEventHandlers()
        {
            if (signupButton != null)
            {
                signupButton.clicked += OnSignupButtonClicked;
            }
            if (loginLink != null)
            {
                loginLink.clicked += OnLoginLinkClicked;
            }
        }

        private void OnSignupButtonClicked()
        {
            // Raise the signup attempt event
            OnSignupAttempted?.Invoke();
        }

        private void OnLoginLinkClicked()
        {
            // Raise the login link clicked event
            OnLoginLinkClickedEvent?.Invoke();
        }

        /// <summary>
        /// Event raised when user attempts to signup
        /// </summary>
        public event System.Action OnSignupAttempted;

        /// <summary>
        /// Event raised when user clicks the login link
        /// </summary>
        public event System.Action OnLoginLinkClickedEvent;

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
        /// Get the current confirm password input value
        /// </summary>
        public string GetConfirmPassword()
        {
            return confirmPasswordField?.value ?? string.Empty;
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
        /// Clear all input fields
        /// </summary>
        public void ClearInputs()
        {
            if (emailField != null) emailField.value = string.Empty;
            if (passwordField != null) passwordField.value = string.Empty;
            if (confirmPasswordField != null) confirmPasswordField.value = string.Empty;
            SetErrorMessage(string.Empty);
        }

        /// <summary>
        /// Set the loading state of the UI
        /// </summary>
        /// <param name="isLoading">Whether signup is in progress</param>
        public void SetLoadingState(bool isLoading)
        {
            if (signupButton != null)
            {
                signupButton.SetEnabled(!isLoading);
                signupButton.text = isLoading ? "Creating Account..." : "Sign Up";
            }
        }
    }
}