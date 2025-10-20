using System;
using System.Globalization;
using UnityEngine;

namespace UI.Lobby
{
    /// <summary>
    /// Encapsulates lobby preference persistence and provides an in-memory fallback if PlayerPrefs is unavailable.
    /// Defaults align with specification requirements and emit change events for controller bindings.
    /// </summary>
    public sealed class LobbyPreferencesModel
    {
        public const string PreferenceNamespace = "LobbyPreferences.";
        public const string DefaultThemeId = "default";

        private const string SfxKey = PreferenceNamespace + "SfxEnabled";
        private const string VfxKey = PreferenceNamespace + "VfxEnabled";
        private const string ThemeKey = PreferenceNamespace + "ThemeId";
        private const string LastUpdatedKey = PreferenceNamespace + "LastUpdatedUtc";

        private readonly IPreferenceStorage primaryStorage;
        private readonly IPreferenceStorage fallbackStorage;

        public event Action<LobbyPreferencesSnapshot> PreferencesChanged;

        public LobbyPreferencesModel()
            : this(new PlayerPrefsStorage(), new InMemoryPreferenceStorage())
        {
        }

        public LobbyPreferencesModel(IPreferenceStorage primary, IPreferenceStorage fallback)
        {
            primaryStorage = primary ?? throw new ArgumentNullException(nameof(primary));
            fallbackStorage = fallback ?? throw new ArgumentNullException(nameof(fallback));
        }

        public bool IsSfxEnabled
        {
            get => GetBool(SfxKey, true);
            set => SetBool(SfxKey, value);
        }

        public bool IsVfxEnabled
        {
            get => GetBool(VfxKey, true);
            set => SetBool(VfxKey, value);
        }

        public string ThemeId
        {
            get => GetString(ThemeKey, DefaultThemeId);
            set => SetString(ThemeKey, string.IsNullOrWhiteSpace(value) ? DefaultThemeId : value);
        }

        public DateTime LastUpdatedUtc => GetDateTime(LastUpdatedKey, DateTime.MinValue);

        public LobbyPreferencesSnapshot Snapshot => new LobbyPreferencesSnapshot(
            IsSfxEnabled,
            IsVfxEnabled,
            ThemeId,
            LastUpdatedUtc);

        public void ResetToDefaults()
        {
            SetBoolInternal(SfxKey, true, false);
            SetBoolInternal(VfxKey, true, false);
            SetStringInternal(ThemeKey, DefaultThemeId, false);
            SetDateTimeInternal(LastUpdatedKey, DateTime.UtcNow, false);
            NotifyChanged();
        }

        private bool GetBool(string key, bool defaultValue)
        {
            if (TryReadBool(primaryStorage, key, out var value)) return value;
            if (TryReadBool(fallbackStorage, key, out value)) return value;
            return defaultValue;
        }

        private string GetString(string key, string defaultValue)
        {
            if (TryReadString(primaryStorage, key, out var value)) return value;
            if (TryReadString(fallbackStorage, key, out value)) return value;
            return defaultValue;
        }

        private DateTime GetDateTime(string key, DateTime defaultValue)
        {
            if (TryReadString(primaryStorage, key, out var value) && TryParseDate(value, out var dt)) return dt;
            if (TryReadString(fallbackStorage, key, out value) && TryParseDate(value, out dt)) return dt;
            return defaultValue;
        }

        private static bool TryParseDate(string raw, out DateTime result)
        {
            return DateTime.TryParseExact(raw, "O", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out result);
        }

        private void SetBool(string key, bool value)
        {
            if (GetBool(key, !value) == value) return;
            SetBoolInternal(key, value, true);
            NotifyChanged();
        }

        private void SetString(string key, string value)
        {
            if (GetString(key, null) == value) return;
            SetStringInternal(key, value, true);
            NotifyChanged();
        }

        private void SetBoolInternal(string key, bool value, bool stampTime)
        {
            WriteBool(primaryStorage, key, value);
            WriteBool(fallbackStorage, key, value);

            if (stampTime)
            {
                SetDateTimeInternal(LastUpdatedKey, DateTime.UtcNow, false);
            }
        }

        private void SetStringInternal(string key, string value, bool stampTime)
        {
            WriteString(primaryStorage, key, value);
            WriteString(fallbackStorage, key, value);

            if (stampTime)
            {
                SetDateTimeInternal(LastUpdatedKey, DateTime.UtcNow, false);
            }
        }

        private void SetDateTimeInternal(string key, DateTime value, bool stampTime)
        {
            var iso = value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
            WriteString(primaryStorage, key, iso);
            WriteString(fallbackStorage, key, iso);

            if (stampTime && key != LastUpdatedKey)
            {
                SetDateTimeInternal(LastUpdatedKey, DateTime.UtcNow, false);
            }
        }

        private static bool TryReadBool(IPreferenceStorage storage, string key, out bool value)
        {
            try
            {
                return storage.TryGetBool(key, out value);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"LobbyPreferencesModel: Failed to read bool '{key}' from storage. {ex.Message}");
                value = default;
                return false;
            }
        }

        private static bool TryReadString(IPreferenceStorage storage, string key, out string value)
        {
            try
            {
                return storage.TryGetString(key, out value);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"LobbyPreferencesModel: Failed to read string '{key}' from storage. {ex.Message}");
                value = default;
                return false;
            }
        }

        private static void WriteBool(IPreferenceStorage storage, string key, bool value)
        {
            try
            {
                storage.SetBool(key, value);
                storage.Save();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"LobbyPreferencesModel: Failed to write bool '{key}' to storage. {ex.Message}");
            }
        }

        private static void WriteString(IPreferenceStorage storage, string key, string value)
        {
            try
            {
                storage.SetString(key, value);
                storage.Save();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"LobbyPreferencesModel: Failed to write string '{key}' to storage. {ex.Message}");
            }
        }

        private void NotifyChanged()
        {
            PreferencesChanged?.Invoke(Snapshot);
        }

        public interface IPreferenceStorage
        {
            bool TryGetBool(string key, out bool value);
            bool TryGetString(string key, out string value);
            void SetBool(string key, bool value);
            void SetString(string key, string value);
            void Save();
        }

        private sealed class PlayerPrefsStorage : IPreferenceStorage
        {
            public bool TryGetBool(string key, out bool value)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    value = PlayerPrefs.GetInt(key) == 1;
                    return true;
                }

                value = default;
                return false;
            }

            public bool TryGetString(string key, out string value)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    value = PlayerPrefs.GetString(key);
                    return true;
                }

                value = default;
                return false;
            }

            public void SetBool(string key, bool value)
            {
                PlayerPrefs.SetInt(key, value ? 1 : 0);
            }

            public void SetString(string key, string value)
            {
                PlayerPrefs.SetString(key, value);
            }

            public void Save()
            {
                PlayerPrefs.Save();
            }
        }

        private sealed class InMemoryPreferenceStorage : IPreferenceStorage
        {
            private readonly System.Collections.Generic.Dictionary<string, object> values =
                new System.Collections.Generic.Dictionary<string, object>();

            public bool TryGetBool(string key, out bool value)
            {
                if (values.TryGetValue(key, out var boxed) && boxed is bool b)
                {
                    value = b;
                    return true;
                }

                value = default;
                return false;
            }

            public bool TryGetString(string key, out string value)
            {
                if (values.TryGetValue(key, out var boxed) && boxed is string s)
                {
                    value = s;
                    return true;
                }

                value = default;
                return false;
            }

            public void SetBool(string key, bool value)
            {
                values[key] = value;
            }

            public void SetString(string key, string value)
            {
                values[key] = value;
            }

            public void Save()
            {
                // In-memory store persists for lifetime of the model; no action required.
            }
        }
    }

    public readonly struct LobbyPreferencesSnapshot
    {
        public LobbyPreferencesSnapshot(bool sfxEnabled, bool vfxEnabled, string themeId, DateTime lastUpdatedUtc)
        {
            SfxEnabled = sfxEnabled;
            VfxEnabled = vfxEnabled;
            ThemeId = themeId;
            LastUpdatedUtc = lastUpdatedUtc;
        }

        public bool SfxEnabled { get; }
        public bool VfxEnabled { get; }
        public string ThemeId { get; }
        public DateTime LastUpdatedUtc { get; }
    }
}