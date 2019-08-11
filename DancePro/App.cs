﻿using System;
using DancePro.Services;

namespace DancePro
{
    public class App
    {
        public static bool UseMockDataStore = true;
        public static string BackendUrl = "http://localhost:5000";
        public static MediaService MediaService;
        public static NetworkService NetworkService;
        //public static NetworkServiceAndroid NetworkService;

        public static void Initialize()
        {
            if (UseMockDataStore)
                ServiceLocator.Instance.Register<IDataStore<Item>, MockDataStore>();
            else
                ServiceLocator.Instance.Register<IDataStore<Item>, CloudDataStore>();
            MediaService = new MediaService();

#if __IOS__
            NetworkService = new NetworkServiceIOS();
#endif
#if __ANDROID__
            NetworkService = new NetworkServiceAndroid();
#endif
        }


    }
}
