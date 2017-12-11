namespace HotSpotManager
{
    /// <summary>
    /// Client Information based on AP Scan
    /// </summary>
    public class ClientScanResult
    {
        /// <summary>
        /// Get or set the IP Address
        /// </summary>
        public string IPAddr { get; set; }
        /// <summary>
        /// Get or set the Hardware Address
        /// </summary>
        public string HWAddr { get; set; }
        /// <summary>
        /// Get or set the Device
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// Get or set if client is reachable
        /// </summary>
        public bool IsReachable { get; set; }

        /// <summary>
        /// Default Ctor
        /// </summary>
        /// <param name="ipAddr">The IP Address</param>
        /// <param name="hwAddr">The Hardware Address</param>
        /// <param name="device">The Device</param>
        /// <param name="isReachable">If Reachable</param>
        public ClientScanResult(string ipAddr, string hwAddr, string device, bool isReachable)
        {
            IPAddr = ipAddr;
            HWAddr = hwAddr;
            Device = device;
            IsReachable = isReachable;
        }
    }
}