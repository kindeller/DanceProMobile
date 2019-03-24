using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using Xamarin.Essentials;
using System.Net.NetworkInformation;

namespace DancePro.Services
{
    public class FileTransferService
    {
        HttpRequestHandler handler;
        List<string> prefixes = new List<string>();
        public int Port { get; }
        public FileTransferService()
        {
            //GetRandomPort
            //Port = new Random().Next(8081, 65535);
            Port = 3045; //Temp testing value

            handler = new HttpRequestHandler("./Root");

            //Request Local Wifi IP Address
            var local = GetIP();

            if (local == null)
            {
                //If fails to get Wifi - Assign Loopback Address of Localhost
                prefixes.Add($"http://localhost:{Port}/");
            }
            else
            {
                prefixes.Add($"http://{local}:{Port}/");
            }

        }

        public void Connect()
        {
            handler.ListenAsynchronously(prefixes);
        }

        public void Disconnect()
        {

            handler.StopListening();

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

        public string GetMainAddress()
        {
            return prefixes[0];
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
    }
}
