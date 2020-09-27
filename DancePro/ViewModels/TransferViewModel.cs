using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DancePro.Models;
using DancePro.Services;

namespace DancePro.ViewModels
{
    public class TransferViewModel : BaseViewModel
    {
        NetworkService _NetworkService;
        List<NewDownloadModel> DownloadingMedia;
        public int CompletedCount { get; private set; }

        public event EventHandler OnStoppedListening;
        public event EventHandler OnServerConnected;
        public event EventHandler OnWifiConnectSuccess;
        public event EventHandler OnWifiConnectFail;
        public event EventHandler OnWifiDisconnected;
        

        public delegate void DownloadMediaChanged(List<NewDownloadModel> mediaList);

        public event DownloadMediaChanged DownloadsUpdated;

        protected virtual void OnDownloadsUpdated()
        {
            DownloadMediaChanged handler = DownloadsUpdated;
            handler?.Invoke(DownloadingMedia);
        }



        public TransferViewModel(NetworkService networkService)
        {
            _NetworkService = networkService;

            SetUpEventListeners();
            App.MediaService.startedEventHandler += MediaService_StartedEventHandler;
            App.MediaService.DownloadUpdate += MediaService_DownloadUpdate;
            DownloadingMedia = new List<NewDownloadModel>();
        }

        private void _NetworkService_OnServerConnected(object sender, EventArgs e)
        {
            OnServerConnected?.Invoke(sender, e);
        }

        private void _NetworkService_OnWifiDisconnected(object sender, EventArgs e)
        {
            OnWifiDisconnected?.Invoke(sender, e);
        }

        private void _NetworkService_OnWifiConnectFail(object sender, EventArgs e)
        {
            OnWifiConnectFail?.Invoke(sender, e);
        }

        private void _NetworkService_OnWifiConnectSuccess(object sender, EventArgs e)
        {
            OnWifiConnectSuccess?.Invoke(sender, e);
        }

        void NetworkService_OnStoppedListening(object sender, EventArgs e)
        {
            OnStoppedListening?.Invoke(sender, e);
            SetUpEventListeners();
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

        public string GetCompletedText()
        {
            return "(" + GetCompletedCount() + "/" + GetDownloadCount() + ")";
        }

        public void ClearDownloads()
        {
            DownloadingMedia.Clear();
            OnDownloadsUpdated();
        }

        public void Connect()
        {
            _NetworkService.Connect();
        }

        public void Disconnect()
        {
            _NetworkService.Disconnect();
        }

        public bool isNetworkListening()
        {
           return _NetworkService.isListening;
        }

        public string GetDeviceID()
        {
            return _NetworkService.GetDeviceID();
        }

        public void UpdateNetworkService(NetworkService service)
        {
            _NetworkService = service;
        }

        public string GetDeviceText()
        {
            string text = "";

            if (isNetworkListening())
            {
                var id = GetDeviceID();
                if (!string.IsNullOrEmpty(id)) text = "Device ID: " + id;
                //text = NetworkService.GetIP().ToString(); //To Debug a show the full
            }

            return text;
            
        }

        public string GetButtonText()
        {
            return isNetworkListening() ? "Disable" : "Enable";
        }

        public bool isOnWifi()
        {
            return _NetworkService.isOnWifi();
        }

        private void SetUpEventListeners()
        {
            _NetworkService.OnStoppedListening += NetworkService_OnStoppedListening;
            _NetworkService.OnServerConnected += _NetworkService_OnServerConnected;
            _NetworkService.OnWifiConnectSuccess += _NetworkService_OnWifiConnectSuccess;
            _NetworkService.OnWifiConnectFail += _NetworkService_OnWifiConnectFail;
            _NetworkService.OnWifiDisconnected += _NetworkService_OnWifiDisconnected;
        }

        public void ConnectToWifi()
        {
            _NetworkService.ConnectToWifi();
        }
    }
}
