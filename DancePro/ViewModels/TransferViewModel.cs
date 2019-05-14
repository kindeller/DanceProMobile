using System;
using System.Collections.Generic;
using DancePro.Models;
using DancePro.Services;

namespace DancePro.ViewModels
{
    public class TransferViewModel : BaseViewModel
    {
        NetworkService NetworkService;
        List<NewDownloadModel> DownloadingMedia;

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
            App.MediaService.startedEventHandler += MediaService_StartedEventHandler;
            App.MediaService.DownloadUpdate += MediaService_DownloadUpdate;
            DownloadingMedia = new List<NewDownloadModel>();
        }

        void MediaService_DownloadUpdate(object sender, NewDownloadModel model)
        {
            foreach(var m in DownloadingMedia)
            {
                if(m.ID == model.ID)
                {
                    m.CopyModel(model);
                    OnDownloadsUpdated();
                }
            }
        }

        void MediaService_StartedEventHandler(object sender, NewDownloadModel model)
        {
            DownloadingMedia.Add(model);
            OnDownloadsUpdated();
        }

        public int GetDownloadCount()
        {
            return DownloadingMedia.Count;
        }

        public int GetCompletedCount()
        {
            int total = 0;
            foreach(var item in DownloadingMedia)
            {
                if (item.Status == NewDownloadModel.DownloadStatus.Completed) total++;
            }

            return total;
        }

        public void ClearDownloads()
        {
            DownloadingMedia.Clear();
            OnDownloadsUpdated();
        }

    }
}
