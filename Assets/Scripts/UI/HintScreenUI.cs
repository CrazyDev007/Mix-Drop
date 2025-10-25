using ScreenFlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class HintScreenUI : ScreenUI
    {

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
            GameManager.Instance.IsUIOpen = false;
            ScreenManager.Instance.ShowScreen("gameplay-screen");
        }

        protected override void SetupScreen(VisualElement screen)
        {
            hintTextLabel = screen.Q<Label>("hint-text");
            closeButton = screen.Q<Button>("close-button");

            closeButton.clicked += OnClickBtnClose;
        }
    }
}