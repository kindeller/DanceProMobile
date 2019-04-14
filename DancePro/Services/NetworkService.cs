using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Net.NetworkInformation;


namespace DancePro.Services
{
    public abstract class NetworkService
    {

        public HttpRequestHandler handler;
        List<string> prefixes = new List<string>();
        public int Port { get; private set; }
        public string Address { get; private set; }
        public bool isListening;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DancePro.Services.NetworkService"/> class.
        /// Adds Listener for change of network Address.
        /// Sets up Port and handler along with init for the first time.
        /// </summary>
        public NetworkService()
        {
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            handler = new HttpRequestHandler("./Root");
            Initialise();

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
        /// Handles the change to network address.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            if (handler == null) return;
            Initialise();
        }

        /// <summary>
        /// Connect this instance to the handler and begins listening.
        /// </summary>
        public void Connect()
        {
            isListening = true;
            handler.ListenAsynchronously(prefixes);
        }

        /// <summary>
        /// Disconnect this instance from the handler and stops listening.
        /// </summary>
        public void Disconnect()
        {
            isListening = false;
            handler.StopListening();
            DisconnectFromWifi();
        }

        public abstract void DisconnectFromWifi();

        public abstract IPAddress GetIP();

        public abstract bool ValidateNetwork();

        public abstract bool isOnWifi();

        public abstract void ConnectToWifi();
    }
}
