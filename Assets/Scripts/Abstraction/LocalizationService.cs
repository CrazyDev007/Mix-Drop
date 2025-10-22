using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CrazyDev007.LevelEditor.Abstraction
{
    /// <summary>
    /// Implementation of the localization service for Unity
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private const string LOCALIZATION_FOLDER = "Localization";
        private const string DEFAULT_CULTURE = "en-US";

        private Dictionary<string, Dictionary<string, string>> _localizationData;
        private string _currentCulture;
        private readonly List<string> _availableCultures;

        public string CurrentCulture => _currentCulture;
        public IEnumerable<string> AvailableCultures => _availableCultures.AsReadOnly();

        public LocalizationService()
        {
            _localizationData = new Dictionary<string, Dictionary<string, string>>();
            _availableCultures = new List<string>();
            _currentCulture = DEFAULT_CULTURE;

            LoadLocalizationData();
        }

        /// <summary>
        /// Loads localization data from JSON files in Resources/Localization/
        /// </summary>
        private void LoadLocalizationData()
        {
            _localizationData.Clear();
            _availableCultures.Clear();

            // Load all localization JSON files
            var localizationFiles = Resources.LoadAll<TextAsset>(LOCALIZATION_FOLDER);

            foreach (var file in localizationFiles)
            {
                try
                {
                    var cultureData = JsonUtility.FromJson<LocalizationData>(file.text);
                    if (cultureData != null && !string.IsNullOrEmpty(cultureData.cultureCode))
                    {
                        _localizationData[cultureData.cultureCode] = cultureData.translations;
                        _availableCultures.Add(cultureData.cultureCode);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to load localization file {file.name}: {ex.Message}");
                }
            }

            // Sort cultures alphabetically
            _availableCultures.Sort();

            // Ensure default culture exists
            if (!_availableCultures.Contains(DEFAULT_CULTURE))
            {
                _availableCultures.Insert(0, DEFAULT_CULTURE);
                _localizationData[DEFAULT_CULTURE] = new Dictionary<string, string>();
            }

            // Set initial culture
            if (!string.IsNullOrEmpty(Application.systemLanguage.ToString()))
            {
                var systemCulture = GetCultureCodeFromSystemLanguage(Application.systemLanguage);
                if (_availableCultures.Contains(systemCulture))
                {
                    _currentCulture = systemCulture;
                }
            }
        }

        public string GetText(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (_localizationData.TryGetValue(_currentCulture, out var cultureDict) &&
                cultureDict.TryGetValue(key, out var text))
            {
                return text;
            }

            // Fallback to default culture
            if (_currentCulture != DEFAULT_CULTURE &&
                _localizationData.TryGetValue(DEFAULT_CULTURE, out var defaultDict) &&
                defaultDict.TryGetValue(key, out var defaultText))
            {
                return defaultText;
            }

            // Return key if not found
            Debug.LogWarning($"Localization key not found: {key}");
            return key;
        }

        public string GetText(string key, params object[] args)
        {
            var text = GetText(key);
            if (args.Length > 0)
            {
                try
                {
                    return string.Format(text, args);
                }
                catch (FormatException ex)
                {
                    Debug.LogError($"Localization format error for key '{key}': {ex.Message}");
                    return text;
                }
            }
            return text;
        }

        public bool HasKey(string key)
        {
            if (_localizationData.TryGetValue(_currentCulture, out var cultureDict) &&
                cultureDict.ContainsKey(key))
            {
                return true;
            }

            if (_currentCulture != DEFAULT_CULTURE &&
                _localizationData.TryGetValue(DEFAULT_CULTURE, out var defaultDict) &&
                defaultDict.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        public bool SetCulture(string cultureCode)
        {
            if (string.IsNullOrEmpty(cultureCode))
                return false;

            if (_availableCultures.Contains(cultureCode))
            {
                _currentCulture = cultureCode;

                // Save preference
                PlayerPrefs.SetString("Localization_Culture", cultureCode);
                PlayerPrefs.Save();

                // Notify listeners (could be implemented with events)
                OnCultureChanged?.Invoke(cultureCode);

                return true;
            }

            return false;
        }

        public string GetCultureDisplayName(string cultureCode)
        {
            try
            {
                var cultureInfo = new CultureInfo(cultureCode);
                return cultureInfo.DisplayName;
            }
            catch
            {
                return cultureCode;
            }
        }

        public void Reload()
        {
            Resources.UnloadUnusedAssets();
            LoadLocalizationData();
        }

        /// <summary>
        /// Event triggered when culture changes
        /// </summary>
        public event Action<string> OnCultureChanged;

        /// <summary>
        /// Converts SystemLanguage to culture code
        /// </summary>
        private string GetCultureCodeFromSystemLanguage(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.English: return "en-US";
                case SystemLanguage.Spanish: return "es-ES";
                case SystemLanguage.French: return "fr-FR";
                case SystemLanguage.German: return "de-DE";
                case SystemLanguage.Italian: return "it-IT";
                case SystemLanguage.Portuguese: return "pt-BR";
                case SystemLanguage.Russian: return "ru-RU";
                case SystemLanguage.Japanese: return "ja-JP";
                case SystemLanguage.ChineseSimplified: return "zh-CN";
                case SystemLanguage.ChineseTraditional: return "zh-TW";
                case SystemLanguage.Korean: return "ko-KR";
                default: return DEFAULT_CULTURE;
            }
        }

        /// <summary>
        /// Serializable class for localization JSON data
        /// </summary>
        [Serializable]
        private class LocalizationData
        {
            public string cultureCode;
            public Dictionary<string, string> translations;
        }
    }
}