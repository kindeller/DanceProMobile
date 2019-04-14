#if __IOS__
using System;
using Xamarin.Essentials;
using NetworkExtension;
using UIKit;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace DancePro.Services
{

    public class NetworkServiceIOS : NetworkService
    {
        NEHotspotConfigurationManager WifiManager = new NEHotspotConfigurationManager();
        NEHotspotConfiguration config = new NEHotspotConfiguration("DPPV", "dppv3778", false);

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

            ConnectToWifi();


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

            WifiManager.ApplyConfiguration(config, (obj) => { });
        }

        public override void DisconnectFromWifi()
        {
            if (isOnWifi())
            {
                //WifiManager.RemoveConfiguration(config.Ssid);
            }
        }

    }
}


#endif
