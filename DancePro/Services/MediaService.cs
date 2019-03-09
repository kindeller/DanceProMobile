using System;
using System.Collections.Generic;
using System.IO;
using DancePro.Models;
using Foundation;

namespace DancePro.Services
{
    public class MediaService
    {
        private const string MediaPath = "./Library/Media/";
        private const int FileWarningThreshhold = 25;

        public static IDictionary<string, MediaTypes> FileToMediaTypes = new Dictionary<string, MediaTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            {".jpg",MediaTypes.Image},
            {".jpeg",MediaTypes.Image},
            {".png",MediaTypes.Image},
            {".mp4",MediaTypes.Video},
            {".mp3",MediaTypes.Audio},
            {".wav",MediaTypes.Audio}

        };

        public static MediaTypes GetMediaType(string filePath)
        {
            MediaTypes type;
            FileToMediaTypes.TryGetValue(Path.GetExtension(filePath), out type);
            return type;
        }

        public string GetMediaPath() {
            return Path.GetFullPath(MediaPath);
        }

        public DirectoryInfo GetMediaPathDirectory() {
            return new DirectoryInfo(MediaPath);
        }


        public static NSString GetSegueString(MediaObject obj) {

            switch (obj.MediaType) {
                case MediaTypes.Image:
                    return new NSString("ImageSegue");
                case MediaTypes.Video:
                    return new NSString("VideoSegue");
                case MediaTypes.Audio:
                    return new NSString("AudioSegue");
                default:
                    return null;
            }
        }

        public List<MediaObject> GetMediaFromFolder() {

            return GetMediaFromFolder(MediaPath);
        }

        public List<MediaObject> GetMediaFromFolder(DirectoryInfo dir) {
            return GetMediaFromFolder(dir.FullName);
        }

        public List<MediaObject> GetMediaFromFolder(string folderPath)
        {
            List<MediaObject> mediaObjects = new List<MediaObject>();
            var thing = Path.GetFullPath(folderPath);
            Directory.CreateDirectory(Path.GetFullPath(folderPath));
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(Path.GetFullPath(folderPath)));
            foreach (var file in files)
            {
                mediaObjects.Add(GetMediaObject(file));
            }

           var directories = Directory.GetDirectories(folderPath);

            foreach(var directory in directories) {
                mediaObjects.Add(new MediaObject(directory));
            }

            return mediaObjects;
        }

        public MediaObject GetMediaObject(string path)
        {
            MediaTypes type = GetMediaType(path);

            switch (type)
            {
                case MediaTypes.Image:
                    ImageObject imageObject = new ImageObject(path);
                    return imageObject as MediaObject;

                case MediaTypes.Audio:
                    AudioObject audioObject = new AudioObject(path);
                    return audioObject as MediaObject;

                case MediaTypes.Video:
                    VideoObject videoObject = new VideoObject(path);
                    return videoObject as MediaObject;
                default:
                    return new MediaObject(path);
            }
        }

        public bool MoveMediaObject(MediaObject obj, string destPath) 
        {
            try
            {
                if (File.Exists(obj.FilePath))
                {
                    var directory = Path.GetDirectoryName(obj.FilePath);
                    var dest = Path.Combine(directory, obj.FileName);
                    File.Move(obj.FilePath, dest);
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public bool RenameMediaObject(MediaObject obj, string newName)
        {
            try 
            {

                if (File.Exists(obj.FilePath)) {
                    var directory = Path.GetDirectoryName(obj.FilePath);
                    var destinationPath = Path.Combine(directory, newName);
                    File.Move(obj.FilePath, destinationPath);
                }
                else {
                    return false;
                }

            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public bool DuplicateMediaObject(MediaObject obj) 
        {
            try 
            {
                if (File.Exists(obj.FilePath) && obj.MediaType != MediaTypes.Other)
                {
                    var filename = Path.GetFileNameWithoutExtension(obj.FilePath);
                    filename = string.Concat(filename, "Copy", Path.GetExtension(obj.FilePath));
                    var dest = Path.GetDirectoryName(obj.FilePath);
                    dest = Path.Combine(dest, filename);
                    File.Copy(obj.FilePath, dest);
                    return true;
                }
                else
                {
                    if(obj.MediaType == MediaTypes.Other) {

                        //if (isUnderMaxFileThreadshold(obj.FilePath)) {
                        throw new NotImplementedException("The Duplication of Folders is not yet Supported.");
                        //}

                    }
                    else {
                        return false;
                    }
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }



        public bool DeleteMediaObject(MediaObject obj) 
        {
            try
            {
                File.Delete(obj.FilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public bool CreateFolder(string folderName, string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) {
                    path = Path.GetFullPath(MediaPath);
                }
                string newFolder = Path.Combine(path, folderName);
                if (!Directory.Exists(newFolder)) {
                    Directory.CreateDirectory(newFolder);
                }
                else {
                    throw new IOException("Cannot create Directory as it already exists.");
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public bool isValidFolderName(string folder) {

            var invalid = Path.GetInvalidPathChars();
            foreach(var c in invalid) {

                if (folder.Contains(c)) {
                    return false;
                }
            }

            return true;
        }

        public bool isValidFileName(string filename) {

            var invalid = Path.GetInvalidFileNameChars();
            foreach (var c in invalid)
            {

                if (filename.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        public bool isUnderMaxFileThreadshold(string _directory) {
            DirectoryInfo directory = new DirectoryInfo(_directory);
            var fileTotal = directory.GetFiles().Length;

            foreach (var dir in directory.GetDirectories())
            {
                fileTotal += dir.GetFiles().Length;
            }

            return (fileTotal < FileWarningThreshhold) ? true : false;
        }

    }
}
