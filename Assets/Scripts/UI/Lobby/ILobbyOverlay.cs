using UnityEngine.UIElements;

namespace UI.Lobby
{
    /// <summary>
    /// Describes the contract for lobby overlays rendered via Unity UI Toolkit.
    /// </summary>
    public interface ILobbyOverlay
    {
        /// <summary>
        /// Gets the unique identifier used by <see cref="LobbyOverlayManager"/> to manage the overlay instance.
        /// </summary>
        string OverlayId { get; }

        /// <summary>
        /// Gets the instantiated root <see cref="VisualElement"/> for this overlay.
        /// </summary>
        VisualElement RootElement { get; }

        /// <summary>
        /// Indicates whether the overlay is currently visible to the player.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Requests the overlay to present itself. Implementations should show the UI, apply bindings,
        /// and then invoke <see cref="LobbyOverlayManager.NotifyOverlayReady(string)"/> once visible.
        /// </summary>
        void Show();

        /// <summary>
        /// Requests the overlay to hide itself. Implementations should initiate dismissal and then
        /// invoke <see cref="LobbyOverlayManager.NotifyOverlayHidden(string)"/> when complete.
        /// </summary>
        void Hide();
    }
}