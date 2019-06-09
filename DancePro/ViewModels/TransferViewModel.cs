﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DancePro.Models;
using DancePro.Services;

namespace DancePro.ViewModels
{
    public class TransferViewModel : BaseViewModel
    {
        NetworkService NetworkService;
        List<NewDownloadModel> DownloadingMedia;
        public int CompletedCount { get; private set; }

        public delegate void DownloadMediaChanged(List<NewDownloadModel> mediaList);

        public event DownloadMediaChanged DownloadsUpdated;

        protected virtual void OnDownloadsUpdated()
        {
            DownloadMediaChanged handler = DownloadsUpdated;
            handler?.Invoke(DownloadingMedia);
        }



        public TransferViewModel(NetworkService networkService)
        {
            NetworkService = networkService;
            NetworkService.Connect();
            NetworkService.OnStoppedListening += NetworkService_OnStoppedListening;
            App.MediaService.startedEventHandler += MediaService_StartedEventHandler;
            App.MediaService.DownloadUpdate += MediaService_DownloadUpdate;
            DownloadingMedia = new List<NewDownloadModel>();
        }

        void NetworkService_OnStoppedListening(object sender, EventArgs e)
        {
        }

        void MediaService_DownloadUpdate(object sender, NewDownloadModel model)
        {
            Console.WriteLine("Update Incoming: " + model.FileName);
            int completedCount = 0;
            lock (DownloadingMedia)
            {
                foreach (var m in DownloadingMedia)
                {
                    if (m.ID == model.ID)
                    {
                        Console.WriteLine("Updating Media: " + model.FileName + ". Status: " + model.Status);
                        m.CopyModel(model);
                        if (model.Status == NewDownloadModel.DownloadStatus.Completed) completedCount++;
                        OnDownloadsUpdated();
                    }
                }
            }

            CompletedCount = completedCount;
           

        }

        void MediaService_StartedEventHandler(object sender, NewDownloadModel model)
        {
            Console.WriteLine("New Download: " + model.FileName + ". Status: " + model.Status);
            lock (DownloadingMedia)
            {
                DownloadingMedia.Add(model);
            }
            OnDownloadsUpdated();
        }

        public int GetDownloadCount()
        {
            return DownloadingMedia.Count;
        }

        public int GetCompletedCount()
        {
            try
            {
                int total = 0;
                foreach (var item in DownloadingMedia)
                {
                    if (item.Status == NewDownloadModel.DownloadStatus.Completed) total++;
                }
                return total;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }

        }

        public void ClearDownloads()
        {
            DownloadingMedia.Clear();
            OnDownloadsUpdated();
        }

        public void Connect()
        {
           NetworkService.Connect();
        }

        public void Disconnect()
        {
            NetworkService.Disconnect();
        }

        public void ToggleConnection()
        {
            if (NetworkService.isListening)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        public bool isNetworkListening()
        {
           return NetworkService.isListening;
        }

        public string GetDeviceID()
        {
            return NetworkService.GetDeviceID();
        }

        public void UpdateNetworkService(NetworkService service)
        {
            NetworkService = service;
        }

        public void ConnectToWifi()
        {
            if (!NetworkService.isOnWifi())
            {
                NetworkService.ConnectToWifi();
            }

        }
    }
}
