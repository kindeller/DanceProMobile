using System;
using System.Collections.Generic;
using System.IO;
using DancePro.Models;

#if __IOS__
using Foundation;
using UIKit;

#endif

namespace DancePro.Services
{
    public class MediaService
    {
        public bool AutoAppendDuplicates { get; set; } = true;
        private string MediaPath = string.Empty;
        private const int FileWarningThreshhold = 25;

        public delegate void DownloadUpdateHandler(object sender, NewDownloadModel model);
        public delegate void DownloadStartedEventHandler(object sender, NewDownloadModel model);

        public event DownloadUpdateHandler DownloadUpdate;
        public event DownloadStartedEventHandler startedEventHandler;

        protected virtual void OnDownloadUpdate(object sender, NewDownloadModel e)
        {
            DownloadUpdateHandler handler = DownloadUpdate;
            handler?.Invoke(this, e);
        }
        protected virtual void OnDownloadStarted(object sender, NewDownloadModel e)
        {
            DownloadStartedEventHandler handler = startedEventHandler;
            handler?.Invoke(this, e);
        }


        public static IDictionary<string, MediaTypes> FileToMediaTypes = new Dictionary<string, MediaTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            {".jpg",MediaTypes.Image},
            {".jpeg",MediaTypes.Image},
            {".png",MediaTypes.Image},
            {".mp4",MediaTypes.Video},
            {".mp3",MediaTypes.Audio},
            {".wav",MediaTypes.Audio},
            {".m4a",MediaTypes.Audio}

        };

        private static readonly string[] AcceptedFileTypes =
        {

            ".jpeg",
            ".jpg",
            ".mov",
            ".mp3",
            ".mpeg",
            ".m4a",
            ".mpg",
            ".png",
            ".mp4"
            //TODO: Enable Zip extension support once Zip can be unzipped in app.
            //".zip"

        };

        public MediaService()
        {
            MediaPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var LibraryPath = Path.Combine(MediaPath + "/Library/");
            if (!Directory.Exists(LibraryPath)) {
                Directory.CreateDirectory(LibraryPath);
            }
            MediaPath = LibraryPath;

        }

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

#if __IOS__
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
#endif

        public void NewDownload(int id)
        {
            OnDownloadStarted(this, new NewDownloadModel(id));
        }

        public bool SaveDownload(int id, string Filename, string FilePath, byte[] FileContents)
        {
            byte[] FileData = new byte[FileContents.Length];
            FileContents.CopyTo(FileData, 0);
            NewDownloadModel model = new NewDownloadModel(id, Filename,status: NewDownloadModel.DownloadStatus.Copying);
            OnDownloadUpdate(this, model);
            Console.WriteLine("[Media] Copying " + "(" + id + ") " + model.FileName + "...");
            if (IsValidFileType(Filename))
            {
                try
                {
                    string fullPath = GetMediaPath() + FilePath;
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }

                    string fileName = fullPath + Filename;
                    //if (File.Exists(fileName))
                    //{
                    //    //Append Copy
                    //    var name = Path.GetFileNameWithoutExtension(parser.Filename) + "Copy" + Path.GetExtension(parser.Filename);
                    //    fileName = fullPath + name;
                    //}
                    File.WriteAllBytes(fileName, FileData);
                    model.Status = NewDownloadModel.DownloadStatus.Completed;
                    model.Message = "Complete";
                    OnDownloadUpdate(this, model);
                    Console.WriteLine("[Media] Download Complete: " + model.FileName);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error writting to file..." + Filename + " : " + e.Message);
                    model.Status = NewDownloadModel.DownloadStatus.Failed;
                    model.Message = "Copy Error";
                    OnDownloadUpdate(this, model);
                    return false;
                }
            }
            else
            {
                model.Status = NewDownloadModel.DownloadStatus.Failed;
                model.Message = "Invalid Type";
                OnDownloadUpdate(this, model);
                Console.WriteLine("Invalid Incoming file type..." + Path.GetExtension(Filename));
                return false;
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
                    var dest = Path.Combine(destPath, obj.FileName);
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
                    destinationPath = destinationPath += Path.GetExtension(obj.FilePath);
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


        /// <summary>
        /// Recursive function for copying all media from one location to another. Can specify a new folder or if it will append to each file/folder.
        /// </summary>
        /// <returns><c>true</c>, if media object was duplicated, <c>false</c> otherwise.</returns>
        /// <param name="obj">Media Object to be copied.</param>
        /// <param name="dir">Specified Directory to copy too.</param>
        /// <param name="appendCopy">If set to <c>true</c> append "copy" onto each object.</param>
        public bool DuplicateMediaObject(MediaObject obj, DirectoryInfo dir = null, bool appendCopy = true) 
        {
            try 
            {
                //Set Append String
                string append = appendCopy ? "Copy" : "";



                // -- Non Folder Object --

                if (obj.MediaType != MediaTypes.Folder)
                {
                    if (!File.Exists(obj.FilePath))
                    {
                        throw new IOException("File Does not Exist");
                    }

                    var filename = Path.GetFileNameWithoutExtension(obj.FilePath);
                    filename = string.Concat(filename, append, Path.GetExtension(obj.FilePath));

                    var dest = (dir == null) ? Path.GetDirectoryName(obj.FilePath) : dir.FullName;
                    dest = Path.Combine(dest, filename);
                    File.Copy(obj.FilePath, dest);
                    return true;
                }
                else
                {
                    // -- Folder Object -- 

                    if(obj.MediaType == MediaTypes.Folder) {

                        //Check for Maximum Files allowed to be copied.
                        if (isUnderMaxFileThreadshold(obj.FilePath)) 
                        {

                            //Copy folder with new name
                            DirectoryInfo newDir = null;
                            if(dir != null)
                            {
                                newDir = Directory.CreateDirectory(Path.Combine(dir.FullName,obj.FileName + append));
                            }
                            else
                            {
                                newDir = Directory.CreateDirectory(obj.FilePath + append);
                            }
                            //get media from folder to list
                            var media = GetMediaFromFolder(obj.FilePath);
                            bool failed = true;
                            foreach (var item in media)
                            {
                                //Copy each item in the folder.
                                DuplicateMediaObject(item, newDir);
                            }
                            return failed ? false : true;



                        }
                        return false;
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

        public bool RenameFolder(MediaObject obj, string newName) {
            try {

                var newPath = Path.Combine(Path.GetDirectoryName(obj.FilePath), newName);
                Directory.Move(obj.FilePath, newPath);
                return true;

            }catch(IOException e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool MoveFolder(MediaObject folderToMove, MediaObject destFolder) {

            try {
                var newDir = Path.Combine(destFolder.FilePath + "/" + folderToMove.FileName);
                Directory.Move(folderToMove.FilePath, newDir);
                return true;
            }catch(IOException e) {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool DeleteFolder(MediaObject obj)
        {
            try
            {
                Directory.Delete(obj.FilePath, true);
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
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

        private bool IsValidFileType(string filename)
        {
            string fileExt = Path.GetExtension(filename).ToLower();

            if (string.IsNullOrEmpty(fileExt)) return true;
            foreach (string ext in AcceptedFileTypes)
            {
                if (string.Compare(fileExt, ext) == 0)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
