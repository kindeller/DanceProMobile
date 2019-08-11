using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;


namespace DancePro.Services
{
    public class HttpListenerCallbackState
    {
        public HttpListenerCallbackState(HttpListener listener)
        {
            Listener = listener ?? throw new ArgumentNullException(nameof(listener));
            ListenForNextRequest = new AutoResetEvent(false);
        }

        public HttpListener Listener { get; private set; }
        public AutoResetEvent ListenForNextRequest { get; private set; }
    }

    public class HttpRequestHandler
        {

        private static readonly string[] indexFiles =
            {
                "index.html",
                "index.htm",
                "default.html",
                "default.htm"
            };

        private static readonly Dictionary<string, string> mimeTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {".asf", "video/x-ms-asf"},
            {".asx", "video/x-ms-asf"},
            {".avi", "video/x-msvideo"},
            {".bin", "application/octet-stream"},
            {".cco", "application/x-cocoa"},
            {".crt", "application/x-x509-ca-cert"},
            {".css", "text/css"},
            {".deb", "application/octet-stream"},
            {".der", "application/x-x509-ca-cert"},
            {".dll", "application/octet-stream"},
            {".dmg", "application/octet-stream"},
            {".ear", "application/java-archive"},
            {".eot", "application/octet-stream"},
            {".exe", "application/octet-stream"},
            {".flv", "video/x-flv"},
            {".gif", "image/gif"},
            {".hqx", "application/mac-binhex40"},
            {".htc", "text/x-component"},
            {".htm", "text/html"},
            {".html", "text/html"},
            {".ico", "image/x-icon"},
            {".img", "application/octet-stream"},
            {".iso", "application/octet-stream"},
            {".jar", "application/java-archive"},
            {".jardiff", "application/x-java-archive-diff"},
            {".jng", "image/x-jng"},
            {".jnlp", "application/x-java-jnlp-file"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".js", "application/x-javascript"},
            {".mml", "text/mathml"},
            {".mng", "video/x-mng"},
            {".mov", "video/quicktime"},
            {".mp3", "audio/mpeg"},
            {".mpeg", "video/mpeg"},
            {".mpg", "video/mpeg"},
            {".msi", "application/oc" +
                "tet-stream"},
            {".msm", "application/octet-stream"},
            {".msp", "application/octet-stream"},
            {".pdb", "application/x-pilot"},
            {".pdf", "application/pdf"},
            {".pem", "application/x-x509-ca-cert"},
            {".pl", "application/x-perl"},
            {".pm", "application/x-perl"},
            {".png", "image/png"},
            {".prc", "application/x-pilot"},
            {".ra", "audio/x-realaudio"},
            {".rar", "application/x-rar-compressed"},
            {".rpm", "application/x-redhat-package-manager"},
            {".rss", "text/xml"},
            {".run", "application/x-makeself"},
            {".sea", "application/x-sea"},
            {".shtml", "text/html"},
            {".sit", "application/x-stuffit"},
            {".swf", "application/x-shockwave-flash"},
            {".tcl", "application/x-tcl"},
            {".tk", "application/x-tcl"},
            {".txt", "text/plain"},
            {".war", "application/java-archive"},
            {".wbmp", "image/vnd.wap.wbmp"},
            {".wmv", "video/x-ms-wmv"},
            {".xml", "text/xml"},
            {".xpi", "application/x-xpinstall"},
            {".zip", "application/zip"},
            {".map", "application/json"}
        };
        string path;
        private int requestCounter = 0;
            private ManualResetEvent stopEvent = new ManualResetEvent(false);

        public event EventHandler ListenerStoppedEvent;

        public HttpRequestHandler(string _path)
        {
            path = _path;
        }


        public void ListenAsynchronously(IEnumerable<string> prefixes)
            {
                HttpListener listener = new HttpListener();

                foreach (string s in prefixes)
                {
                    listener.Prefixes.Add(s);
                }

                listener.Start();
                HttpListenerCallbackState state = new HttpListenerCallbackState(listener);
                ThreadPool.QueueUserWorkItem(Listen, state);
            }

            public void StopListening()
            {
                stopEvent.Set();
            }


            private void Listen(object state)
            {
                HttpListenerCallbackState callbackState = (HttpListenerCallbackState)state;

                while (callbackState.Listener.IsListening)
                {
                    callbackState.Listener.BeginGetContext(new AsyncCallback(ListenerCallback), callbackState);
                    int n = WaitHandle.WaitAny(new WaitHandle[] { callbackState.ListenForNextRequest, stopEvent });

                    if (n == 1)
                    {
                        // stopEvent was signalled 
                        callbackState.Listener.Stop();
                        ListenerStoppedEvent?.Invoke(this,null);
                        break;
                    }
                }
            }

            private void ListenerCallback(IAsyncResult ar)
            {
                HttpListenerCallbackState callbackState = (HttpListenerCallbackState)ar.AsyncState;
                HttpListenerContext context = null;

                int requestNumber = Interlocked.Increment(ref requestCounter);

                try
                {
                    context = callbackState.Listener.EndGetContext(ar);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    callbackState.ListenForNextRequest.Set();
                }

                if (context == null) return;

            switch (context.Request.HttpMethod) {

                case "GET":
                    GET(context);
                    break;
                case "POST":
                    POST(context);
                    break;
            }

            }

        private void GET(HttpListenerContext context) {
            HttpListenerRequest request = context.Request;
            string filename = "";
            using (StreamReader sr = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                string requestData = sr.ReadToEnd();

                // get filename path

                filename = context.Request.Url.AbsolutePath;
                if (filename != null) filename = filename.Substring(1);

                // get default index file if needed
                if (string.IsNullOrEmpty(filename))
                {
                    foreach (string indexFile in indexFiles)
                    {
                        string fullPath = Path.Combine(path, indexFile);
                        
                        if (File.Exists(fullPath))
                        {
                            filename = indexFile;
                            break;
                        }
                    }

                }
                filename = Path.Combine(path, filename);
            }

            try
            {
                using (HttpListenerResponse response = context.Response)
                {
                    // send file
                    HttpStatusCode statusCode;

                    if (File.Exists(filename))
                    {
                        try
                        {
                            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                // get mime type TODO: Change to inbuilt mime type
                                context.Response.ContentType = mimeTypes[Path.GetExtension(filename)];
                                context.Response.ContentLength64 = stream.Length;

                                // copy file stream to response
                                stream.CopyTo(context.Response.OutputStream);
                                stream.Flush();
                                context.Response.OutputStream.Flush();
                                context.Response.OutputStream.Close();
                            }

                            statusCode = HttpStatusCode.OK;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("ERROR: " + e.Message);
                            statusCode = HttpStatusCode.InternalServerError;
                        }
                    }
                    else
                    {
                        Console.WriteLine("File not found: " + filename);
                        statusCode = HttpStatusCode.NotFound;
                    }
                    // finish
                    context.Response.StatusCode = (int)statusCode;
                    if (statusCode == HttpStatusCode.OK)
                    {
                        context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                        context.Response.AddHeader("Last-Modified", File.GetLastWriteTime(filename).ToString("r"));
                    }

                    response.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void POST(HttpListenerContext context) {


            int id = new Random().Next(1, 1000);
            MediaService MediaService = App.MediaService;
            MediaService.NewDownload(id);

            MultipartParser parser = new MultipartParser(context.Request.InputStream, context.Request.ContentEncoding);

            if (parser.Success)
            {
                Console.WriteLine("[HTTP] Incoming Download: " + parser.Filename);
                if (MediaService.SaveDownload(id,parser.Filename,parser.FilePath,parser.FileContents))
                {
                    context.Response.StatusCode = 204;
                    context.Response.Close();
                }
                else
                {
                    context.Response.StatusCode = 500;
                    context.Response.Close();
                }
            }
        }

    }



    //TODO: Update File info handling and response to request
    public class UploadFileInfo
    {
        public string name { get; set; }
        public long size { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string type { get; set; }
        public string dir { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string error { get; set; }
        public string delete_type { get; set; }
        public string delete_url { get; set; }
        public Dictionary<string, UploadFileInfo> image_versions { get; set; }
    }


    //Prep Serialization for JSON response to each file request.

    //public static class Json
    //{
    //    public static T Deserialise<T>(string json)
    //    {
    //        T obj = Activator.CreateInstance<T>();
    //        using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
    //        {
    //            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
    //            obj = (T)serializer.ReadObject(ms);
    //            return obj;
    //        }
    //    }
    //    public static string Serialize<T>(T obj)
    //    {
    //        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
    //        using (MemoryStream ms = new MemoryStream())
    //        {
    //            serializer.WriteObject(ms, obj);
    //            return Encoding.UTF8.GetString(ms.ToArray());
    //        }
    //    }
    //}
}
