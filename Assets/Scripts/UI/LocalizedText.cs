using UnityEngine;
using UnityEngine.UIElements;
using CrazyDev007.LevelEditor.Abstraction;

namespace CrazyDev007.LevelEditor.UI
{
    /// <summary>
    /// UI Toolkit element that displays localized text
    /// </summary>
    [UxmlElement]
    public partial class LocalizedText : Label
    {
        private ILocalizationService _localizationService;
        private string _localizationKey;
        private object[] _formatArgs;

        /// <summary>
        /// The localization key for this text element
        /// </summary>
        [UxmlAttribute]
        public string LocalizationKey
        {
            get => _localizationKey;
            set
            {
                _localizationKey = value;
                UpdateText();
            }
        }

        /// <summary>
        /// Format arguments for the localized text
        /// </summary>
        public object[] FormatArgs
        {
            get => _formatArgs;
            set
            {
                _formatArgs = value;
                UpdateText();
            }
        }

        public LocalizedText()
        {
            // Get localization service from ServiceLocator
            _localizationService = ServiceLocator.Container.Resolve<ILocalizationService>();

            // Subscribe to culture changes
            if (_localizationService is LocalizationService localizationService)
            {
                localizationService.OnCultureChanged += OnCultureChanged;
            }
        }

        private void OnCultureChanged(string newCulture)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (_localizationService != null && !string.IsNullOrEmpty(_localizationKey))
            {
                if (_formatArgs != null && _formatArgs.Length > 0)
                {
                    text = _localizationService.GetText(_localizationKey, _formatArgs);
                }
                else
                {
                    text = _localizationService.GetText(_localizationKey);
                }
            }
        }

        /// <summary>
        /// Sets the localization key and format arguments
        /// </summary>
        public void SetLocalizedText(string key, params object[] args)
        {
            _localizationKey = key;
            _formatArgs = args;
            UpdateText();
        }

        ~LocalizedText()
        {
            // Unsubscribe from culture changes
            if (_localizationService is LocalizationService localizationService)
            {
                localizationService.OnCultureChanged -= OnCultureChanged;
            }
        }
    }
}