# Xamarin.HotSpotManager
[![Build status](https://ci.appveyor.com/api/projects/status/9xnqpdq5grglvpq1?svg=true)](https://ci.appveyor.com/project/nwestfall/xamarin-hotspotmanager)
[![NuGet version](https://badge.fury.io/nu/Xamarin.HotSpotManager.svg)](https://badge.fury.io/nu/Xamarin.HotSpotManager)

Xamarin C# port of https://github.com/nickrussler/Android-Wifi-Hotspot-Manager-Class to allow creating and managing Hot Spots on Android devices

Table of contents
 * [HotSpotManager](#hot-spot-manager)
    * [What it does](#what-it-does)
 * [Usage](#usage)
    * [Install](#install)
        * [Nuget](#nuget)
    * [Basic Example](#basic-example)
    * [Advanced](#advanced)

Hot Spot Manager
================
An Xamarin Android library (and demo app) that provides a wrapper for the private android hotspot API.

What it does
------------
The manager let's you access and do the following with the private android hotspot API.
 * Check current status of hotspot
 * Enable/disable hotspot
 * Read or save the current hotspot configuration
 * Get list of clients connected to the hotspot

 Usage
 =====

 Install
 -------
 ### Nuget
 ```
 Install-Package Xamarin.HotSpotManager
 ```

Basic Example
-------------
The HotSpotManager.App is a quick app to see how the API works.  Setting it up is very simple.
```C#
//Other android usings

using HotSpotManager;

namespace MyApp
{
    [Activity(Label = "MyApp", MainLauncher = true)]
    public class MainActivity : Activity
    {
        WifiApManager wifiApManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Android view setup

            wifiApManager = new WifiApManager(this);
        }

        private void SetupHotspot()
        {
            WifiConfiguration config = new WifiConfiguration();
            config.Ssid = "MyAccessPoint";

            bool result = wifiApManager.SetWifiApEnabled(config, true);
        }

        private void SetupHotspotWithPassword()
        {
            //WPA Example
            WifiConfiguration config = new WifiConfiguration();
            config.Ssid = "MyAccessPoint";
            config.AllowKeyManagement.Set((int)KeyManagementType.WpaPsk);
            config.PreSharedKey = "Password";

            bool result = wifiApManager.SetWifiApEnabled(config, true);
        }
    }
}
```

Advanced
--------
```C#
/// <summary>
/// Show write permission settings page to user if necessary or force
/// </summary>
/// <param name="force">Show settings page even when rights are already granted</param>
public void ShowWritePermissionSettings(bool force);

/// <summary>
/// Start AccessPoint mode with the specified configuration.  If the radio is already running in AP mode, update
/// the new configuration
/// Note that starting in access point mode disables station mode operation
/// </summary>
/// <param name="wifiConfig">SSID, security and channel details as part of <see cref="WifiConfiguration"/></param>
/// <param name="enabled">If enabled</param>
/// <returns><code>true</code> if the operation succeeds, <code>false</code> otherwise</returns>
public bool SetWifiApEnabled(WifiConfiguration wifiConfig, bool enabled);

/// <summary>
/// Gets the Wi-Fi enabled state.
/// <see cref="IsWifiApEnabled()"/>
/// </summary>
/// <returns><see cref="Enums.WIFI_AP_STATE"/></returns>
public Enums.WIFI_AP_STATE GetWifiApState();

/// <summary>
/// Returns whether Wi-Fi AP is enabled or disabled
/// 
/// <see cref="GetWifiApState()"/>
/// </summary>
/// <returns><code>true</code> if Wi-Fi AP is enabled</returns>
public bool IsWifiApEnabled();

/// <summary>
/// Gets the Wi-Fi AP Configuration
/// </summary>
/// <returns>AP details in <see cref="WifiConfiguration"/></returns>
public WifiConfiguration GetWifiApConfiguration();

/// <summary>
/// Sets the Wi-Fi AP Configuration
/// </summary>
/// <param name="wifiConfig">The <see cref="WifiConfiguration"/></param>
/// <returns><code>true</code> if the operation succeeded, <code>false</code> otherwise</returns>
public bool SetWifiApConfiguration(WifiConfiguration wifiConfig);

/// <summary>
/// Gets a list of the clients connected to the Hotspot
/// </summary>
/// <param name="onlyReachables"><code>false</code> if the list should contain unreachable (probably disconnected) clients, <code>true </code> otherwise</param>
/// <param name="reachableTimeout">Reachable timeout in milliseconds</param>
/// <returns>List of <see cref="ClientScanResult"/></returns>
public Task<IList<ClientScanResult>> GetClientListAsync(bool onlyReachables, int reachableTimeout = 300);
```