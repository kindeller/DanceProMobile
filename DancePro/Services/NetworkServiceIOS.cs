 #if __IOS__
using System;
using Xamarin.Essentials;
using NetworkExtension;
using UIKit;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using SystemConfiguration;
using Foundation;

namespace DancePro.Services
{

    public class NetworkServiceIOS : NetworkService
    {
        NEHotspotConfigurationManager WifiManager = new NEHotspotConfigurationManager();
        NEHotspotConfiguration config = new NEHotspotConfiguration("DPPV", "DPPV3778", false);
      

        public NetworkServiceIOS()
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

        public override bool ValidateNetwork()
        {
            if (isOnWifi())
            {
                return true;
            }
            return false;
        }

        public override bool isOnWifi()
        {

            var profiles = Connectivity.ConnectionProfiles;

            foreach (var profile in profiles)
            {
                if (profile == ConnectionProfile.WiFi)
                {

                    return true;
                }

            }
            return false;
        }

        public override void ConnectToWifi()
        {
            // -- Not implimented yet but added for future.
            config.JoinOnce = true;
            config.LifeTimeInDays = 1;
            // -- End
            WifiManager.ApplyConfiguration(config, (obj) => {
                Console.WriteLine(obj?.Description);
                WifiManager.RemoveConfiguration(config.Ssid);
            });
        }

        public override void DisconnectFromWifi()
        {
            if (isOnWifi())
            {
                //WifiManager.RemoveConfiguration(config.Ssid);
            }
        }

        private async void AsyncWaitForConnection()
        {
            await Task.Run(() =>
            {

                while (true)
                {
                    if (isOnWifi())
                    {
                        Console.WriteLine("Connected to Wifi!");
                        //TODO: Add "failed to/connected to wifi network" prompt.
                        //TODO: This works and removes the config but still auto-joins after taking away the wifi network.
                        WifiManager.RemoveConfiguration(config.Ssid);
                        return;
                    }
                }

            });

        }

    }
}


#endif
