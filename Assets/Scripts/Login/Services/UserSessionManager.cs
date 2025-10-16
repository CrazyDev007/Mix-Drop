using System;
using Unity.Services.Authentication;
using UnityEngine;

namespace MixDrop.Login.Services
{
    /// <summary>
    /// Manages user authentication sessions securely
    /// </summary>
    public class UserSessionManager : MonoBehaviour
    {
        private static UserSessionManager _instance;
        public static UserSessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("UserSessionManager");
                    _instance = go.AddComponent<UserSessionManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private const string SESSION_TOKEN_KEY = "MixDrop_SessionToken";
        private const string EXPIRY_KEY = "MixDrop_TokenExpiry";

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Store the current session securely
        /// </summary>
        public void StoreSession()
        {
            if (!Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogWarning("Cannot store session: User not signed in");
                return;
            }

            // Unity Authentication SDK handles secure session persistence automatically
            // Additional secure storage can be implemented here if needed
            LoginLogger.LogInfo("Session stored securely by Unity SDK");
        }

        /// <summary>
        /// Restore session from secure storage
        /// </summary>
        /// <returns>True if session was restored successfully</returns>
        public bool RestoreSession()
        {
            // Unity SDK handles session restoration automatically on app start
            return Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn;
        }

        /// <summary>
        /// Clear the stored session
        /// </summary>
        public void ClearSession()
        {
            Unity.Services.Authentication.AuthenticationService.Instance.SignOut();
            LoginLogger.LogInfo("Session cleared");
        }

        /// <summary>
        /// Check if a valid session exists
        /// </summary>
        public bool HasValidSession()
        {
            return Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn;
        }
    }
}