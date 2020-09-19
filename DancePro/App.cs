using System;
using System.Collections.Generic;
using DancePro.Services;


namespace DancePro
{
    public class App
    {
        public static string BackendUrl = "http://localhost:5000";
        public static MediaService MediaService;
        public static NetworkService NetworkService;
        public static ErrorService ErrorService;
        //public static NetworkServiceAndroid NetworkService;

        public static void Initialize()
        {
            MediaService = new MediaService();
            ErrorService = new ErrorService(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
#if __IOS__
            NetworkService = new NetworkServiceIOS();
#endif
#if __ANDROID__
            NetworkService = new NetworkServiceAndroid();
#endif
        }


    }
}
