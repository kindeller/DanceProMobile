using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Sockets;
using System.Collections.Generic;
using System;

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
            var local = GetIP();
            prefixes.Add($"http://{local}:{Port}/");
        }

        public void Connect() {
            handler.ListenAsynchronously(prefixes);
        }

        public void Disconnect() {

            handler.StopListening();

        }

        private IPAddress GetIP()
        {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            if( addresses.Length > 0 && addresses[1] != null)
            {
                return addresses[1];
            }

            return null;
        }

        public string GetMainAddress()
        {
            return prefixes[0];
        }

    }
}
