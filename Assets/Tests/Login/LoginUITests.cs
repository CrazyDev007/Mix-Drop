using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using MixDrop.Login.UI;
using MixDrop.Login.Services;

namespace MixDrop.Login.Tests
{
    public class LoginUITests
    {
        private GameObject uiGameObject;
        private LoginUI loginUI;
        private UIDocument uiDocument;
        private TextField emailField;
        private TextField passwordField;
        private Button loginButton;
        private Label errorLabel;

        [SetUp]
        public void Setup()
        {
            // Create test GameObject with UIDocument
            uiGameObject = new GameObject("TestLoginUI");
            uiDocument = uiGameObject.AddComponent<UIDocument>();
            loginUI = uiGameObject.AddComponent<LoginUI>();

            // Create PanelSettings for UIDocument
            uiDocument.panelSettings = ScriptableObject.CreateInstance<PanelSettings>();

            // Create test UI elements
            var root = new VisualElement();
            emailField = new TextField("email-field") { value = "test@example.com" };
            passwordField = new TextField("password-field") { value = "password123" };
            loginButton = new Button { name = "login-button" };
            errorLabel = new Label { name = "error-text" };

            root.Add(emailField);
            root.Add(passwordField);
            root.Add(loginButton);
            root.Add(errorLabel);

            // Manually set up the LoginUI with test elements (bypassing UIDocument for testing)
            loginUI.uiDocument = uiDocument;
            // Note: In a real scenario, UIDocument would populate these, but for testing we simulate
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(uiGameObject);
        }

        [Test]
        public void LoginUI_ComponentExists()
        {
            // Arrange & Act
            var component = uiGameObject.GetComponent<LoginUI>();

            // Assert
            Assert.IsNotNull(component);
            Assert.AreEqual(loginUI, component);
        }

        [Test]
        [Ignore("UI setup requires UXML/USS implementation - test will be enabled once T013/T014 are complete")]
        public void LoginUI_GetEmail_ReturnsCorrectValue()
        {
            // This test requires proper UI setup with UXML/USS
            Assert.Fail("Test pending UI implementation");
        }

        [Test]
        [Ignore("UI setup requires UXML/USS implementation - test will be enabled once T013/T014 are complete")]
        public void LoginUI_GetPassword_ReturnsCorrectValue()
        {
            // This test requires proper UI setup with UXML/USS
            Assert.Fail("Test pending UI implementation");
        }

        [Test]
        public void LoginUI_SetErrorMessage_DisplaysError()
        {
            // Arrange
            string testErrorMessage = "Invalid credentials";

            // Act
            loginUI.SetErrorMessage(testErrorMessage);

            // Assert
            Assert.AreEqual(testErrorMessage, errorLabel.text);
            Assert.AreEqual(DisplayStyle.Flex, errorLabel.style.display);
        }

        [Test]
        public void LoginUI_SetErrorMessage_EmptyString_HidesError()
        {
            // Arrange
            loginUI.SetErrorMessage("Some error");

            // Act
            loginUI.SetErrorMessage(string.Empty);

            // Assert
            Assert.AreEqual(string.Empty, errorLabel.text);
            Assert.AreEqual(DisplayStyle.None, errorLabel.style.display);
        }

        [Test]
        [Ignore("UI setup requires UXML/USS implementation - test will be enabled once T013/T014 are complete")]
        public void LoginUI_ClearInputs_ResetsFields()
        {
            // This test requires proper UI setup with UXML/USS
            Assert.Fail("Test pending UI implementation");
        }

        [Test]
        public void LoginUI_SetLoadingState_DisablesButton()
        {
            // Arrange
            loginButton.SetEnabled(true);
            loginButton.text = "Login";

            // Act
            loginUI.SetLoadingState(true);

            // Assert
            Assert.IsFalse(loginButton.enabledSelf);
            Assert.AreEqual("Logging in...", loginButton.text);
        }

        [Test]
        public void LoginUI_SetLoadingState_EnablesButton()
        {
            // Arrange
            loginUI.SetLoadingState(true);

            // Act
            loginUI.SetLoadingState(false);

            // Assert
            Assert.IsTrue(loginButton.enabledSelf);
            Assert.AreEqual("Login", loginButton.text);
        }

        // Integration test for account lockout UI feedback
        [Test]
        public void LoginUI_AccountLockout_DisplaysLockoutMessage()
        {
            // Arrange
            string lockoutMessage = "Your account has been temporarily locked due to too many failed login attempts. Please try again later.";

            // Act
            loginUI.SetErrorMessage(lockoutMessage);

            // Assert
            Assert.AreEqual(lockoutMessage, errorLabel.text);
            Assert.AreEqual(DisplayStyle.Flex, errorLabel.style.display);
            Assert.IsTrue(errorLabel.text.Contains("locked"));
            Assert.IsTrue(errorLabel.text.Contains("temporarily"));
        }
    }
}