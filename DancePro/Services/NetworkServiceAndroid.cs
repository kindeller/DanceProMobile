#if __ANDROID__
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Android.App;
using Android.Content;
using Android.Net.Wifi;

namespace DancePro.Services
{

    public class NetworkServiceAndroid : NetworkService
    {


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
            var con = Application.Context.GetSystemService(Context.WifiService) as WifiManager;

            if(con != null)
            {
                //add && con.ConnectionInfo.SSID == "DPPV" below to exit testing
                return con.IsWifiEnabled && (con.ConnectionInfo.NetworkId != -1);
            }

            return false;
        }

        public override void ConnectToWifi()
        {

            //TODO:
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
