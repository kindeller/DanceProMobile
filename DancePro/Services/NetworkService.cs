using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using Xamarin.Essentials;
using System.Net.NetworkInformation;
using NetworkExtension;
using System.Threading.Tasks;
using UIKit;

namespace DancePro.Services
{
    public class NetworkService
    {
        NEHotspotConfigurationManager WifiManager = new NEHotspotConfigurationManager();
        NEHotspotConfiguration config = new NEHotspotConfiguration("DPPV", "dppv3778", false);

        HttpRequestHandler handler;
        List<string> prefixes = new List<string>();
        public int Port { get; private set; }
        public string Address { get; private set; }
        public bool isListening;


        public NetworkService()
        {
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            //GetRandomPort
            //Port = new Random().Next(8081, 65535);
            Port = 3045; //Temp testing value

            handler = new HttpRequestHandler("./Root");
            Initialise();

        }

        private void Initialise()
        {
            //Request Local Wifi IP Address
            var local = GetIP();
            prefixes.Clear();

            if (local == null)
            {
                //If fails to get Wifi - Assign Loopback Address of Localhost
                prefixes.Add($"http://localhost:{Port}/");
                Address = "localhost";
            }
            else
            {
                prefixes.Add($"http://{local}:{Port}/");
                Address = local.ToString();
            }
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            Initialise();
        }


        public void Connect()
        {
            isListening = true;
            handler.ListenAsynchronously(prefixes);
        }

        public void Disconnect()
        {
            isListening = false;
            handler.StopListening();
            //TODO: attempt to disconnect from wifi here?
            if (isOnWifi())
            {
                WifiManager.RemoveConfiguration(config.Ssid);
            }
        }

        private IPAddress GetIP()
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
        
        public bool ValidateNetwork()
        {
            if (isOnWifi())
            {
                return true;
            }

            ConnectToWifi();


            return false;
        }

        public bool isOnWifi()
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

        public void ConnectToWifi() {

            WifiManager.ApplyConfiguration(config, (obj) => { });
        }
    }
}
