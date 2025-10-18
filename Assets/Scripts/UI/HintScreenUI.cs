using ScreenFlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class HintScreenUI : ScreenUI
    {
        [SerializeField] private GameObject hintScreen;

        private Label hintTextLabel;
        private Button closeButton;

        public void SetHintText(string hintText)
        {
            if (hintTextLabel != null)
            {
                hintTextLabel.text = hintText;
            }
        }

        public void OnClickBtnClose()
        {
            // Hide the hint screen
            // This would typically be handled by the UI manager
            // For now, we'll just deactivate the GameObject
            if (hintScreen != null)
            {
                hintScreen.SetActive(false);
            }
        }

        protected override void SetupScreen(VisualElement screen)
        {
            hintTextLabel = screen.Q<Label>("hint-text");
            closeButton = screen.Q<Button>("close-button");

            closeButton.clicked += OnClickBtnClose;
        }
    }
}