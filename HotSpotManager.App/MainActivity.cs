using System.Text;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Net.Wifi;

namespace HotSpotManager.App
{
    [Activity(Label = "HotSpotManager.App", MainLauncher = true)]
    public class MainActivity : Activity
    {
        WifiApManager wifiApManager;

        EditText SSID, Password;
        TextView Clients;
        CheckBox HideSSID;
        Button StartAPBtn, StopAPBtn, ShowClientsBtn, GetSSIDBtn, GetStateBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            SSID = FindViewById<EditText>(Resource.Id.SSID);
            Password = FindViewById<EditText>(Resource.Id.Password);
            HideSSID = FindViewById<CheckBox>(Resource.Id.Hidden);
            Clients = FindViewById<TextView>(Resource.Id.Clients);
            StartAPBtn = FindViewById<Button>(Resource.Id.StartAP);
            StopAPBtn = FindViewById<Button>(Resource.Id.StopAP);
            ShowClientsBtn = FindViewById<Button>(Resource.Id.ShowClients);
            GetSSIDBtn = FindViewById<Button>(Resource.Id.GetSSID);
            GetStateBtn = FindViewById<Button>(Resource.Id.GetState);

            StartAPBtn.Click += StartAPBtn_Click;
            StopAPBtn.Click += StopAPBtn_Click;
            ShowClientsBtn.Click += ShowClientsBtn_Click;
            GetSSIDBtn.Click += GetSSIDBtn_Click;
            GetStateBtn.Click += GetStateBtn_Click;

            wifiApManager = new WifiApManager(this);
            wifiApManager.ShowWritePermissionSettings(true);
        }

        private void GetStateBtn_Click(object sender, System.EventArgs e)
        {
            var state = wifiApManager.GetWifiApState();
            Toast.MakeText(this, $"State: {state}", ToastLength.Short).Show();
        }

        private void GetSSIDBtn_Click(object sender, System.EventArgs e)
        {
            var config = wifiApManager.GetWifiApConfiguration();
            if (config == null)
                Toast.MakeText(this, "Unable to get config", ToastLength.Short).Show();
            else
                Toast.MakeText(this, $"SSID: {config.Ssid}", ToastLength.Short).Show();
        }

        private async void ShowClientsBtn_Click(object sender, System.EventArgs e)
        {
            var clients = await wifiApManager.GetClientListAsync(true);
            StringBuilder clientDetails = new StringBuilder();
            clientDetails.AppendLine($"{clients.Count} clients");
            clientDetails.AppendLine("-----------------------------------------");
            for(var i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                clientDetails.AppendLine($"Client #{i + 1}: {client.Device}|{client.HWAddr}|{client.IPAddr}|{client.IsReachable}");
            }
            Clients.Text = clientDetails.ToString();
        }

        private void StopAPBtn_Click(object sender, System.EventArgs e)
        {
            bool result = wifiApManager.SetWifiApEnabled(null, false);

            if (result)
                Toast.MakeText(this, "AP Turned Off", ToastLength.Short).Show();
            else
                Toast.MakeText(this, "Unable to turn AP off", ToastLength.Short).Show();
        }

        private void StartAPBtn_Click(object sender, System.EventArgs e)
        {
            WifiConfiguration config = new WifiConfiguration();
            config.HiddenSSID = HideSSID.Checked;
            config.Ssid = SSID.Text;
            config.AllowedKeyManagement.Set((int)KeyManagementType.WpaPsk);
            config.PreSharedKey = Password.Text;

            bool result = false;
            if (wifiApManager.IsWifiApEnabled())
                result = wifiApManager.SetWifiApConfiguration(config);
            else
                result = wifiApManager.SetWifiApEnabled(config, true);

            if (result)
                Toast.MakeText(this, "AP Turned On", ToastLength.Short).Show();
            else
                Toast.MakeText(this, "Unable to turn AP on", ToastLength.Short).Show();
        }
    }
}

