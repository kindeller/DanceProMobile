using System;
using System.IO;
using Android.App;

namespace DancePro.Droid
{
    public class CopyAssets
    {
        string DefaultPath = Path.Combine(Application.Context.FilesDir.Path, "Root");
        string AssetsFolder = "Root/";

        public CopyAssets()
        {

        }

        public CopyAssets(string defaultPath)
        {
            DefaultPath = defaultPath;
        }

        public void CopyAllAssets()
        {
            Directory.CreateDirectory(Path.Combine(DefaultPath, "js/vendor"));
            Directory.CreateDirectory(Path.Combine(DefaultPath, "css"));
            CopyToFolder("index.html");
            CopyToFolder("logo.jpg");

            CopyToFolder("js/app.js");
            CopyToFolder("js/jquery.fileupload.js");
            CopyToFolder("js/jquery.iframe-transport.js");
            CopyToFolder("js/jquery-3.2.1.min.js");
            CopyToFolder("js/main.js");
            CopyToFolder("js/popper.min.js");
            CopyToFolder("js/bootstrap.js");
            CopyToFolder("js/bootstrap.min.js");
            CopyToFolder("js/bootstrap.min.js.map");
            CopyToFolder("js/bootstrap.bundle.js");
            CopyToFolder("js/bootstrap.bundle.min.js");
            CopyToFolder("js/bootstrap-4.2.1.min.js");
            CopyToFolder("js/vendor/jquery.ui.widget.js");

            CopyToFolder("css/bootstrap.css");
            CopyToFolder("css/bootstrap.min.map");
            CopyToFolder("css/bootstrap.min.css");
            CopyToFolder("css/bootstrap.min.css.map");
            CopyToFolder("css/bootstrap-grid.css");
            CopyToFolder("css/jquery.fileupload.css");
            CopyToFolder("css/jquery.fileupload-noscript.css");
            CopyToFolder("css/jquery.fileupload-ui.css");
            CopyToFolder("css/jquery.fileupload-ui-noscript.css");
            CopyToFolder("css/style.css");


        }

        public void CopyToFolder(string path)
        {

            try
            {
                using (var stream = Application.Context.Assets.Open(Path.Combine(AssetsFolder, path)))
                {
                    
                    using (var dest = File.Create(Path.Combine(DefaultPath, path)))
                    {
                        stream.CopyTo(dest);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }


    }
}
