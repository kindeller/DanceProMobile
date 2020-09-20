#if __ANDROID__
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;

namespace DancePro.Services
{

    public class NetworkServiceAndroid : NetworkService
    {
        //LegacyVariables
        WifiManager wifiManager;
        [Obsolete]
        WifiConfiguration wifiConfiguration;
        int wifiID = 0;

        //AndroidX 
        ConnectivityManager Manager;
        NetworkCallback _networkCallback;

        private const string _ssid = "DPPV";
        private const string _passphrase = "DPPV3778";

        private string _legacySsid = $"\"{_ssid}\"";
        private string _legacyPassphrase = $"\"{_passphrase}\"";

        //private const string _ssid = "BudiiLite-primary64C38C-5G";
        //private const string _passphrase = "826f9cb1";


        public NetworkServiceAndroid()
        {
        }

        public override IPAddress GetIP()
        {
            if (isOnWifi())
            {
                //Active WiFi
                foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return addrInfo.Address;
                            }
                        }
                    }
                }
            }

            //backup 
            return null;
        }

        /// <summary>
        /// This method handles connection to Wifi using the depreciated wifi API
        /// </summary>
        /// <param name="callback"></param>
        [Obsolete]
        private void LegacyConnectToWifi()
        {
            Console.WriteLine("Legacy Wifi Connect...");
            wifiManager = (WifiManager)Application.Context.GetSystemService(Context.WifiService);
            
            wifiConfiguration = new WifiConfiguration()
            {
                Ssid = _legacySsid,
                PreSharedKey = _legacyPassphrase
            };
            wifiID = wifiManager.AddNetwork(wifiConfiguration);
            wifiManager.EnableNetwork(wifiID, true);
            WifiConfiguration _wifiConfiguration = new WifiConfiguration();
            foreach (var item in wifiManager.ConfiguredNetworks)
            {
                 if(item.Ssid == _legacySsid)
                {
                    _wifiConfiguration = item;
                }
            }

            if (_wifiConfiguration.Ssid == null)
            {
                Console.WriteLine($"Cannot connect to network: {_ssid}");
                WifiConnectFail();
                return;
            }
            //wifiManager.Disconnect();
            //var enableNetwork = wifiManager.EnableNetwork(_wifiConfiguration.NetworkId, true);

        }

        /// <summary>
        /// This method validates the API level and connects to wifi using the AndroidX API.
        /// </summary>
        /// <param name="callback"></param>
        public override void ConnectToWifi()
        {


            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
            {
                Manager = Application.Context.GetSystemService(Context.ConnectivityService) as ConnectivityManager;
                _networkCallback = new LegacyNetworkCallback(Manager)
                {
                    NetworkAvailable = network =>
                    {
                        // we are connected!
                        Console.WriteLine("Connected to Wifi");
                        WifiConnectSuccess();

                    },
                    NetworkUnavailable = () =>
                    {
                        Console.WriteLine("Failed to Connect to WIFI");
                        WifiConnectFail();
                    }
                };
                Manager.RegisterDefaultNetworkCallback(_networkCallback);
                LegacyConnectToWifi();
                return;
            }
            else
            {
                Manager = Application.Context.GetSystemService(Context.ConnectivityService) as ConnectivityManager;
                _networkCallback = new NetworkCallback(Manager)
                {
                    NetworkAvailable = network =>
                    {
                        // we are connected!
                        Console.WriteLine("Connected to Wifi");
                        WifiConnectSuccess();

                    },
                    NetworkUnavailable = () =>
                    {
                        Console.WriteLine("Failed to Connect to WIFI");
                        WifiConnectFail();
                    }
                };
                var wifiSpecifier = new WifiNetworkSpecifier.Builder()
                   .SetSsid(_ssid)
                   .SetWpa2Passphrase(_passphrase)
                   .Build();

                var request = new NetworkRequest.Builder()
                    .AddTransportType(Android.Net.TransportType.Wifi) // we want WiFi
                    .RemoveCapability(NetCapability.Internet) // Internet not required
                    .SetNetworkSpecifier(wifiSpecifier) // we want _our_ network
                    .Build();

                Manager.RequestNetwork(request, _networkCallback);
            }

        }

        
        public override void DisconnectFromWifi()
        {

            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
            {
                LegacyDisconnectFromWifi();
                return;
            }
            
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
            {
                // need this line to release the binding.  
                Manager.BindProcessToNetwork(null);
                //the UnregisterNetworkCallback releases the wifi connectivity but does not release the binding
                // so your default wifi and/or mobile data is active but your app has no network connectivity.
                Manager.UnregisterNetworkCallback(_networkCallback);
                WifiDisconnected();
            }
            
        }

        [Obsolete]
        private void LegacyDisconnectFromWifi()
        {
            Console.WriteLine("Legacy Wifi Disconnect...");
            wifiManager = (WifiManager)Application.Context.GetSystemService(Context.WifiService);
            wifiManager.Disconnect();
            foreach (WifiConfiguration config in wifiManager.ConfiguredNetworks)
            {
                if (config.Ssid == _legacySsid)
                {
                    wifiManager.RemoveNetwork(config.NetworkId);
                    WifiDisconnected();
                }
            }

        }


    }

    class NetworkCallback : ConnectivityManager.NetworkCallback
    {
        private ConnectivityManager _conn;
        public Action<Network> NetworkAvailable { get; set; }
        public Action NetworkUnavailable { get; set; }

        public NetworkCallback(ConnectivityManager connectivityManager)
        {
            _conn = connectivityManager;
        }

        public override void OnAvailable(Network network)
        {
            base.OnAvailable(network);
            // Need this to bind to network otherwise it is connected to wifi 
            // but traffic is not routed to the wifi specified
            _conn.BindProcessToNetwork(network);
            NetworkAvailable?.Invoke(network);
            
        }

        public override void OnUnavailable()
        {
            base.OnUnavailable();
            NetworkUnavailable?.Invoke();
        }

    }

    class LegacyNetworkCallback : NetworkCallback
    {
        public LegacyNetworkCallback(ConnectivityManager connectivityManager) : base(connectivityManager)
        {
        }

        public override void OnAvailable(Network network)
        {
            base.OnAvailable(network);
            NetworkAvailable?.Invoke(network);

        }

        public override void OnUnavailable()
        {
            base.OnUnavailable();
            NetworkUnavailable?.Invoke();
        }

    }
}



#endif
