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
            prefixes.Add($"http://localhost:{Port}/");



        }

        public void Connect() {
            handler.ListenAsynchronously(prefixes);
        }

        public void Disconnect() {

            handler.StopListening();

        }

    }
}
