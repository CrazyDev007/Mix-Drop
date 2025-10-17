using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

namespace MixDrop.Login
{
    /// <summary>
    /// Manages login scene initialization and Unity Services setup
    /// </summary>
    public class LoginManager : MonoBehaviour
    {
        private async void Awake()
        {
            // Initialize Unity Services
            await UnityServices.InitializeAsync();

            // Optionally, you can sign in anonymously or handle authentication here
            // For now, just initialize
        }
    }
}