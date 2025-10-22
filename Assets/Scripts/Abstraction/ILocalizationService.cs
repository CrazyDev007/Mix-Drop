using System.Collections.Generic;

namespace CrazyDev007.LevelEditor.Abstraction
{
    /// <summary>
    /// Interface for localization services providing text translation and culture management
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets the current culture/language code (e.g., "en-US", "es-ES")
        /// </summary>
        string CurrentCulture { get; }

        /// <summary>
        /// Gets all available cultures
        /// </summary>
        IEnumerable<string> AvailableCultures { get; }

        /// <summary>
        /// Translates a key to the localized text for the current culture
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <returns>The localized text, or the key if not found</returns>
        string GetText(string key);

        /// <summary>
        /// Translates a key with format arguments
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="args">Format arguments</param>
        /// <returns>The formatted localized text</returns>
        string GetText(string key, params object[] args);

        /// <summary>
        /// Checks if a localization key exists
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <returns>True if the key exists</returns>
        bool HasKey(string key);

        /// <summary>
        /// Sets the current culture/language
        /// </summary>
        /// <param name="cultureCode">The culture code (e.g., "en-US")</param>
        /// <returns>True if the culture was set successfully</returns>
        bool SetCulture(string cultureCode);

        /// <summary>
        /// Gets the display name for a culture code
        /// </summary>
        /// <param name="cultureCode">The culture code</param>
        /// <returns>The display name (e.g., "English (US)")</returns>
        string GetCultureDisplayName(string cultureCode);

        /// <summary>
        /// Reloads localization data (useful for runtime updates)
        /// </summary>
        void Reload();
    }
}