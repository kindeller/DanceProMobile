using System;
namespace DancePro.Models
{
    public class NewDownloadModel
    {


        public enum DownloadStatus
        {
            Failed,
            Downloading,
            Copying,
            Completed
        }

        public int ID { get; set; }
        public string FileName { get; set; }
        public MediaTypes FileType { get; set; }
        public string Message { get; set; }
        public DownloadStatus Status {get;set;}
        public string FilePath { get; set;}

        public NewDownloadModel(int id, string filename = "Unknown", MediaTypes type = MediaTypes.Other,DownloadStatus status = DownloadStatus.Downloading, string message = "Downloading...")
        {
            ID = id;
            FileName = filename;
            FileType = type;
            Message = message;
            Status = status;

        }

        public void CopyModel(NewDownloadModel model)
        {
            FileName = model.FileName;
            FileType = model.FileType;
            Status = model.Status;
            Message = model.Message;
            FilePath = model.FilePath;
        }
    }
}
