using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DancePro.Services
{
    public class FileTransferService
    {
        HttpListener listener = new HttpListener();

        public FileTransferService()
        {
            listener.Prefixes.Add("http://localhost:3045/");
        }


        public void CloseServer() {

            listener.Stop();
        }

        //TODO: Convert to Async function
        public void StartServer() {

            listener.Start();
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            string responseString = "<HTML><BODY><h1>DancePro</h1></BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }

    }
}
