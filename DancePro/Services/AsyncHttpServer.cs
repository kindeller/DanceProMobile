using System;
using System.Net.Mime;
using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DancePro.Services
{

    [Obsolete("This Class is Depreciated! DO NOT USE!")]
    public class AsyncHttpServer
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

        private HttpListener listener;
        private readonly string path;
        private readonly string ip;
        private readonly int port;

        public AsyncHttpServer(string path, string ip, int port)
        {
            this.path = Path.GetFullPath(path);
            this.ip = ip;
            this.port = port;

        }

        public void Start()
        {
            if (!HttpListener.IsSupported)
            {
                throw new Exception("Not supported!");
            }

            listener = new HttpListener();
            listener.Prefixes.Add(string.Format("http://{0}:{1}/", ip, port));
            listener.Start();
            IAsyncResult result = listener.BeginGetContext(new AsyncCallback(HandleAsyncCallback), listener);
        }

        protected void HandleAsyncCallback(IAsyncResult ar)
        {

            var context = listener.EndGetContext(ar);
            listener.BeginGetContext(new AsyncCallback(HandleAsyncCallback), listener);
            try
            {
                ProcessContextAsync(context);
            }
            catch(Exception e)
            {
                throw e;
            }

        }


        private void ProcessContextAsync(HttpListenerContext context)
        {
                // get filename path
                string filename = context.Request.Url.AbsolutePath;
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

                Console.WriteLine("Loading file: " + filename);
                filename = Path.Combine(path, filename);

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
                context.Response.OutputStream.Close();

        }

        public void Stop()
        {
            listener.Stop();
            listener = null;
        }
    }
}
