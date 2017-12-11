using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

using Android.Content;
using Android.OS;
using Android.Util;
using Android.Net.Wifi;
using Android.Provider;

using Java.Lang;
using Java.Lang.Reflect;
using Java.Net;

namespace HotSpotManager
{
    /// <summary>
    /// Wi-Fi AP Manager
    /// </summary>
    public class WifiApManager
    {
        private readonly WifiManager wifiManager;
        private Context context;

        /// <summary>
        /// Default Ctor
        /// </summary>
        /// <param name="context">The context</param>
        public WifiApManager(Context context)
        {
            this.context = context;
            wifiManager = (WifiManager)this.context.GetSystemService(Context.WifiService);
        }

        /// <summary>
        /// Show write permission settings page to user if necessary or force
        /// </summary>
        /// <param name="force">Show settings page even when rights are already granted</param>
        public void ShowWritePermissionSettings(bool force)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if(force || !Settings.System.CanWrite(this.context))
                {
                    var intent = new Intent(Settings.ActionManageWriteSettings);
                    intent.SetData(Android.Net.Uri.Parse($"package:{this.context.PackageName}"));
                    intent.AddFlags(ActivityFlags.NewTask);
                    this.context.StartActivity(intent);
                }
            }
        }

        /// <summary>
        /// Start AccessPoint mode with the specified configuration.  If the radio is already running in AP mode, update
        /// the new configuration
        /// Note that starting in access point mode disables station mode operation
        /// </summary>
        /// <param name="wifiConfig">SSID, security and channel details as part of <see cref="WifiConfiguration"/></param>
        /// <param name="enabled">If enabled</param>
        /// <returns><code>true</code> if the operation succeeds, <code>false</code> otherwise</returns>
        public bool SetWifiApEnabled(WifiConfiguration wifiConfig, bool enabled)
        {
            try
            {
                if (enabled) //disable wifi in any case
                    wifiManager.SetWifiEnabled(false);
                
                Method method = wifiManager.Class.GetMethods().FirstOrDefault(m => m.Name == "setWifiApEnabled");
                return (bool)method.Invoke(wifiManager, wifiConfig, enabled);
            }
            catch(System.Exception ex)
            {
                Log.Error(nameof(WifiApManager), "", ex);
                return false;
            }
        }

        /// <summary>
        /// Gets the Wi-Fi enabled state.
        /// <see cref="IsWifiApEnabled()"/>
        /// </summary>
        /// <returns><see cref="Enums.WIFI_AP_STATE"/></returns>
        public Enums.WIFI_AP_STATE GetWifiApState()
        {
            try
            {
                Method method = wifiManager.Class.GetMethod("getWifiApState");

                int tmp = (int)method.Invoke(wifiManager, null);

                // Fix for Android 4
                if (tmp >= 10)
                    tmp = tmp - 10;

                return (Enums.WIFI_AP_STATE)tmp;
            }
            catch(System.Exception ex)
            {
                Log.Error(nameof(WifiApManager), "", ex);
                return Enums.WIFI_AP_STATE.WIFI_AP_STATE_FAILED;
            }
        }

        /// <summary>
        /// Returns whether Wi-Fi AP is enabled or disabled
        /// 
        /// <see cref="GetWifiApState()"/>
        /// </summary>
        /// <returns><code>true</code> if Wi-Fi AP is enabled</returns>
        public bool IsWifiApEnabled() => GetWifiApState() == Enums.WIFI_AP_STATE.WIFI_AP_STATE_ENABLED;

        /// <summary>
        /// Gets the Wi-Fi AP Configuration
        /// </summary>
        /// <returns>AP details in <see cref="WifiConfiguration"/></returns>
        public WifiConfiguration GetWifiApConfiguration()
        {
            try
            {
                Method method = wifiManager.Class.GetMethod("getWifiApConfiguration");
                return (WifiConfiguration)method.Invoke(wifiManager, null);
            }
            catch(System.Exception ex)
            {
                Log.Error(nameof(WifiApManager), "", ex);
                return null;
            }
        }

        /// <summary>
        /// Sets the Wi-Fi AP Configuration
        /// </summary>
        /// <param name="wifiConfig">The <see cref="WifiConfiguration"/></param>
        /// <returns><code>true</code> if the operation succeeded, <code>false</code> otherwise</returns>
        public bool SetWifiApConfiguration(WifiConfiguration wifiConfig)
        {
            try
            {
                Method method = wifiManager.Class.GetMethod("setWifiApConfiguration", Class.FromType(typeof(WifiConfiguration)));
                return (bool)method.Invoke(wifiManager, wifiConfig);
            }
            catch(System.Exception ex)
            {
                Log.Error(nameof(WifiApManager), "", ex);
                return false;
            }
        }

        /// <summary>
        /// Gets a list of the clients connected to the Hotspot, reachable timeout is 300
        /// </summary>
        /// <param name="onlyReachables"><code>false</code> if the list should contain unreachable (probably disconnected) clients, <code>true </code> otherwise</param>
        /// <returns>List of <see cref="ClientScanResult"/></returns>
        public Task<IList<ClientScanResult>> GetClientListAsync(bool onlyReachables) => GetClientListAsync(onlyReachables, 300);

        /// <summary>
        /// Gets a list of the clients connected to the Hotspot
        /// </summary>
        /// <param name="onlyReachables"><code>false</code> if the list should contain unreachable (probably disconnected) clients, <code>true </code> otherwise</param>
        /// <param name="reachableTimeout">Reachable timeout in milliseconds</param>
        /// <returns>List of <see cref="ClientScanResult"/></returns>
        public Task<IList<ClientScanResult>> GetClientListAsync(bool onlyReachables, int reachableTimeout)
        {
            return Task.Run(() =>
            {
                IList<ClientScanResult> result = new List<ClientScanResult>();

                var macRegex = new Regex("..:..:..:..:..:..", RegexOptions.Compiled);

                try
                {
                    var lines = File.ReadAllLines("/proc/net/arp");
                    foreach (var line in lines)
                    {
                        string[] splitted = line.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

                        if (splitted != null && splitted.Length >= 4)
                        {
                            //Basic sanity check
                            string mac = splitted[3];

                            if (macRegex.IsMatch(mac))
                            {
                                bool isReachable = InetAddress.GetByName(splitted[0]).IsReachable(reachableTimeout);

                                if (!onlyReachables || isReachable)
                                    result.Add(new ClientScanResult(splitted[0], splitted[3], splitted[5], isReachable));
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error(nameof(WifiApManager), "", ex);
                }

                return result;
            });
        }
    }
}