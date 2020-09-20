using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Essentials;
#if __ANDROID__
using Android.App;
#endif

namespace DancePro.Services
{
    public abstract class NetworkService
    {
        //public event EventHandler isListeningChanged;
        public event EventHandler OnStoppedListening;
        public event EventHandler OnWifiConnectSuccess;
        public event EventHandler OnWifiConnectFail;
        public event EventHandler OnServerConnected;
        public event EventHandler OnWifiDisconnected;

        public HttpRequestHandler handler;
        List<string> prefixes = new List<string>();
        public int Port { get; private set; }
        public string Address { get; private set; }
        public bool isListening;
        public bool Enabled { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:DancePro.Services.NetworkService"/> class.
        /// Adds Listener for change of network Address.
        /// Sets up Port and handler along with init for the first time.
        /// </summary>
        public NetworkService()
        {

            handler = new HttpRequestHandler("./Root");
#if __ANDROID__
                string path = Path.Combine(Application.Context.FilesDir.Path, "Root").ToString();
            handler = new HttpRequestHandler(path);
#endif
            if (handler != null) handler.ListenerStoppedEvent += Handler_ListenerStoppedEvent;
            Initialise();

        }

        void Handler_ListenerStoppedEvent(object sender, EventArgs e)
        {
            Console.WriteLine("Listener Stopped Event.");
            SetIsListening(false);
            OnStoppedListening?.Invoke(this, null);
        }

        private void SetIsListening(bool b)
        {
            isListening = b;
            //isListeningChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Initialise this instance with IP Address Details.
        /// </summary>
        private void Initialise()
        {
            //Request new Port number
            //Port = new Random().Next(8081, 10000);
            Port = 3045;

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

        /// <summary>
        /// Connect this instance to the handler and begins listening.
        /// </summary>
        public void Connect()
        {
            
            if (isListening) return;
            Initialise();
            if (ValidateNetwork())
            {
                try
                {
                    SetIsListening(true);
                    handler.ListenAsynchronously(prefixes);
                    OnServerConnected.Invoke(this,null);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    SetIsListening(false);
                }

            }

        }

        /// <summary>
        /// Disconnect this instance from the handler and stops listening.
        /// </summary>
        public void Disconnect()
        {
            Console.WriteLine("Disconnecting...");
            handler.StopListening();
            SetIsListening(false);
            DisconnectFromWifi();
        }

        public bool ValidateNetwork()
        {
            if (isOnWifi())
            {
                return true;
            }
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

        public abstract void DisconnectFromWifi();

        public abstract IPAddress GetIP();

        public abstract void ConnectToWifi();

        public string GetDeviceID()
        {
            var id = GetIP()?.ToString()?.Split('.')?[3].Split(':')?[0];
            return id ?? "-";
        }

        protected void WifiConnectSuccess()
        {
            OnWifiConnectSuccess?.Invoke(this,null);
        }

        protected void WifiConnectFail()
        {
            OnWifiConnectFail?.Invoke(this, null);
        }

        protected void WifiDisconnected()
        {
            OnWifiDisconnected?.Invoke(this, null);
        }
        
    }
}
