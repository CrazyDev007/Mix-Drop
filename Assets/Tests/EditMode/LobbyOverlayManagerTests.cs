using System.Collections.Generic;
using NUnit.Framework;
using UI.Lobby;
using UnityEngine;

namespace MixDrop.Tests.EditMode.Lobby
{
    public sealed class LobbyOverlayManagerTests
    {
        private LobbyOverlayManager manager;
        private GameObject managerObject;

        [SetUp]
        public void SetUp()
        {
            managerObject = new GameObject(nameof(LobbyOverlayManager));
            manager = managerObject.AddComponent<LobbyOverlayManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(managerObject);
        }

        [Test]
        public void TryShowOverlay_ActivatesOverlayAndRaisesEvents()
        {
            var overlay = new FakeOverlay("how-to-play", manager);
            manager.RegisterOverlay(overlay);

            var opened = new List<string>();
            manager.OverlayOpened += opened.Add;

            var result = manager.TryShowOverlay(overlay.OverlayId);

            Assert.IsTrue(result, "Expected overlay show call to succeed.");
            Assert.AreEqual(overlay.OverlayId, manager.ActiveOverlayId);
            Assert.IsTrue(overlay.WasShown, "Expected overlay Show() to be invoked.");
            Assert.AreEqual(1, opened.Count);
            Assert.AreEqual(overlay.OverlayId, opened[0]);

            // Ensure instrumentation captured at least one sample (opened stage)
            Assert.IsNotEmpty(manager.PerformanceSamples);
            Assert.AreEqual(LobbyOverlayManager.OverlayLifecycleStage.Opened, manager.PerformanceSamples[0].Stage);
        }

        [Test]
        public void TryShowOverlay_WhenOtherActive_QueuesAndSwitchesOverlays()
        {
            var helpOverlay = new FakeOverlay("how-to-play", manager);
            var settingsOverlay = new FakeOverlay("settings", manager);

            manager.RegisterOverlay(helpOverlay);
            manager.RegisterOverlay(settingsOverlay);

            manager.TryShowOverlay(helpOverlay.OverlayId);
            manager.TryShowOverlay(settingsOverlay.OverlayId);

            Assert.AreEqual(settingsOverlay.OverlayId, manager.ActiveOverlayId);
            Assert.IsTrue(helpOverlay.WasHidden, "Active overlay should be hidden before switching.");
            Assert.IsTrue(settingsOverlay.WasShown, "Pending overlay should be shown after previous hides.");

            // Expect at least three samples: help opened, help closed, settings opened
            Assert.GreaterOrEqual(manager.PerformanceSamples.Count, 3);
        }

        private sealed class FakeOverlay : ILobbyOverlay
        {
            private readonly LobbyOverlayManager manager;

            public FakeOverlay(string overlayId, LobbyOverlayManager manager)
            {
                OverlayId = overlayId;
                this.manager = manager;
                RootElement = new UnityEngine.UIElements.VisualElement();
            }

            public string OverlayId { get; }
            public UnityEngine.UIElements.VisualElement RootElement { get; }
            public bool IsVisible { get; private set; }
            public bool WasShown { get; private set; }
            public bool WasHidden { get; private set; }

            public void Show()
            {
                WasShown = true;
                IsVisible = true;
                manager.NotifyOverlayReady(OverlayId);
            }

            public void Hide()
            {
                WasHidden = true;
                IsVisible = false;
                manager.NotifyOverlayHidden(OverlayId);
            }
        }
    }
}