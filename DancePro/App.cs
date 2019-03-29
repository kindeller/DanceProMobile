using System;
using DancePro.Services;

namespace DancePro
{
    public class App
    {
        public static bool UseMockDataStore = true;
        public static string BackendUrl = "http://localhost:5000";
        public static NetworkService NetworkService;
        public static MediaService MediaService;

        public static void Initialize()
        {
            if (UseMockDataStore)
                ServiceLocator.Instance.Register<IDataStore<Item>, MockDataStore>();
            else
                ServiceLocator.Instance.Register<IDataStore<Item>, CloudDataStore>();

            NetworkService = new NetworkService();
            MediaService = new MediaService();
        }


    }
}
