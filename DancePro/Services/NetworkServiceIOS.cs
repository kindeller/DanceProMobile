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
using CoreFoundation;

namespace DancePro.Services
{

    public class NetworkServiceIOS : NetworkService
    {

        NEHotspotConfigurationManager WifiManager = new NEHotspotConfigurationManager();
        //NEHotspotConfiguration config = new NEHotspotConfiguration("DPPV", "DPPV3778", false);
        NEHotspotConfiguration config = new NEHotspotConfiguration("BudiiLite-primary64C38C-5G", "826f9cb1", false);

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

        public override void ConnectToWifi(Action<string> callback)
        {
            // -- Not implimented yet but added for future.
            config.JoinOnce = true;
            config.LifeTimeInDays = 1;
            // -- End

            WifiManager.ApplyConfiguration(config, (NSError e) => {
                if(e != null)
                {
                    Console.WriteLine("Failed to Connect to WIFI");
                    callback("Wifi Declined");
                }
                else
                {
                    callback("Enabling Network");
                }
                

            });
        }

        public override void DisconnectFromWifi()
        {
            if (isOnWifi())
            {
                WifiManager.RemoveConfiguration(config.Ssid);
            }
        }
    }
}


#endif
