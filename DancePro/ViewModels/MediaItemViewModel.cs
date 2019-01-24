using System;
namespace DancePro.ViewModels
{

    public enum FileTypes
    {
        Image = 0,
        Video = 1
    }

    public class MediaItemViewModel
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public FileTypes FileType { get; set; }

        public MediaItemViewModel()
        {
        }

        public MediaItemViewModel(string title, string description, string filePath, FileTypes fileType)
        {

            Title = title;
            Description = description;
            FilePath = filePath;
            FileType = fileType;
        }
    }
}
