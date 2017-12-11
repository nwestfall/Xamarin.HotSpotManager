namespace HotSpotManager
{
    public static class Enums
    {
        /// <summary>
        /// Possible Wi-Fi AP States
        /// </summary>
        public enum WIFI_AP_STATE
        {
            /// <summary>
            /// AP is disabling
            /// </summary>
            WIFI_AP_STATE_DISABLING,
            /// <summary>
            /// AP is disabled
            /// </summary>
            WIFI_AP_STATE_DISABLED,
            /// <summary>
            /// AP is enabling
            /// </summary>
            WIFI_AP_STATE_ENABLING,
            /// <summary>
            /// AP is enabled
            /// </summary>
            WIFI_AP_STATE_ENABLED,
            /// <summary>
            /// AP failed
            /// </summary>
            WIFI_AP_STATE_FAILED
        }
    }
}
