using System;
using NUnit.Framework;
using UI.Lobby;

namespace MixDrop.Tests.EditMode.Lobby
{
    public sealed class LobbyPreferencesModelTests
    {
        private FakeStorage primary;
        private FakeStorage fallback;
        private LobbyPreferencesModel model;

        [SetUp]
        public void SetUp()
        {
            primary = new FakeStorage();
            fallback = new FakeStorage();
            model = new LobbyPreferencesModel(primary, fallback);
        }

        [Test]
        public void Snapshot_ReturnsDefaultsWhenNoValuesPersisted()
        {
            var snapshot = model.Snapshot;

            Assert.IsTrue(snapshot.SfxEnabled);
            Assert.IsTrue(snapshot.VfxEnabled);
            Assert.AreEqual(LobbyPreferencesModel.DefaultThemeId, snapshot.ThemeId);
            Assert.AreEqual(DateTime.MinValue, snapshot.LastUpdatedUtc);
        }

        [Test]
        public void SettingBooleanPreference_UpdatesBothStoresAndRaisesEvent()
        {
            LobbyPreferencesSnapshot captured = default;
            var eventCount = 0;
            model.PreferencesChanged += snapshot =>
            {
                captured = snapshot;
                eventCount++;
            };

            model.IsSfxEnabled = false;

            Assert.AreEqual(1, eventCount, "Expected PreferencesChanged to fire once.");
            Assert.IsFalse(captured.SfxEnabled);
            Assert.AreEqual(false, primary.LastBool);
            Assert.AreEqual(false, fallback.LastBool);
            Assert.That(primary.LastWriteKey, Does.Contain("SfxEnabled"));
            Assert.That(fallback.LastWriteKey, Does.Contain("SfxEnabled"));
            Assert.That(captured.LastUpdatedUtc, Is.Not.EqualTo(DateTime.MinValue));
        }

        [Test]
        public void ReadingFallsBackWhenPrimaryMissing()
        {
            fallback.SetString("LobbyPreferences.ThemeId", "retro");
            var theme = model.ThemeId;

            Assert.AreEqual("retro", theme);
        }

        [Test]
        public void ResetToDefaults_ClearsPersistedValues()
        {
            model.IsSfxEnabled = false;
            model.ThemeId = "neon";
            primary.ResetTracking();
            fallback.ResetTracking();

            model.ResetToDefaults();

            Assert.IsTrue(model.IsSfxEnabled);
            Assert.AreEqual(LobbyPreferencesModel.DefaultThemeId, model.ThemeId);
            Assert.That(primary.LastWriteKey, Does.Contain("ThemeId"));
            Assert.That(fallback.LastWriteKey, Does.Contain("ThemeId"));
        }

        private sealed class FakeStorage : LobbyPreferencesModel.IPreferenceStorage
        {
            private readonly System.Collections.Generic.Dictionary<string, object> values =
                new System.Collections.Generic.Dictionary<string, object>();

            public string LastWriteKey { get; private set; }
            public bool? LastBool { get; private set; }

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
                LastWriteKey = key;
                LastBool = value;
            }

            public void SetString(string key, string value)
            {
                values[key] = value;
                LastWriteKey = key;
            }

            public void Save()
            {
                // No-op for fake storage.
            }

            public void SetString(string key, object value)
            {
                values[key] = value;
            }

            public void ResetTracking()
            {
                LastWriteKey = null;
                LastBool = null;
            }
        }
    }
}