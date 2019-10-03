using System;
using System.Collections.Generic;
using System.IO;
using Android.Content;
using Android.Graphics;
using DancePro.Models;
using DancePro.Services;

namespace DancePro.Droid
{

    public class MediaFolder
    {
        private DirectoryInfo Directory;
        private List<MediaObject> MediaObjects;

        public MediaFolder()
        {
            Directory = App.MediaService.GetMediaPathDirectory();
            RefreshMedia();
        }

        public MediaFolder(DirectoryInfo directory)
        {
            Directory = directory;
            RefreshMedia();
            
        }

        public MediaFolder(string filePath)
        {
            ChangeDirectory(filePath);
            RefreshMedia();
        }

        public int ItemCount
        {
            get
            {
                return MediaObjects.Count;
            }
        }

        public List<MediaObject> GetAll()
        {
            return MediaObjects;
        }

        public MediaObject GetItem(int i)
        {
            return MediaObjects[i];
        }

        public MediaObject this[int i]
        {
            get { return MediaObjects[i]; }
        }

        public Bitmap GetThumb(Context c, int i)
        {
            MediaObject media = this[i];
            BitmapFactory.Options op = new BitmapFactory.Options();
            op.InSampleSize = 3;
            switch (media.MediaType)
            {
                //TODO: Add Alt Icon Types
                case MediaTypes.Image:
                    using (var imageBitmap = BitmapFactory.DecodeFile(media.FilePath, op))
                    {
                        return Bitmap.CreateScaledBitmap(imageBitmap, 150, 150, false);
                    }
                        
                case MediaTypes.Audio:
                    using (var audioBitmap = BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.speaker))
                    {
                        return Bitmap.CreateScaledBitmap(audioBitmap, 150, 150, false);
                    }
                case MediaTypes.Folder:
                    using (var folderBitmap = BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.FolderBlueIcon30))
                    {
                        return Bitmap.CreateScaledBitmap(folderBitmap, 150, 150, false);
                    }
                case MediaTypes.Other:
                    using (var backBitmap = BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.BackIconBlue500))
                    {
                        return Bitmap.CreateScaledBitmap(backBitmap, 150, 150, false);
                    }
                default:
                    using (var defaultLogo = BitmapFactory.DecodeResource(c.Resources, Resource.Drawable.logo))
                    {
                        return Bitmap.CreateScaledBitmap(defaultLogo, 150, 150, false);
                    }
                    
            }
            
        }

        public Bitmap GetFullImage(int i, int sampleSize = 1)
        {
            var obj = MediaObjects[i];
            if (obj.MediaType != MediaTypes.Image) return null;

            BitmapFactory.Options op = new BitmapFactory.Options();
            op.InSampleSize = sampleSize;
            try
            {
                using (var image = BitmapFactory.DecodeFile(obj.FilePath, op))
                {
                    var width = image.GetScaledWidth(image.Density / 2);
                    var height = image.GetScaledHeight(image.Density / 2);
                    //TODO: Workout ratiofor scale and scale to appropriate dimentions
                    return Bitmap.CreateScaledBitmap(image, width, height, false);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + "[Bitmap Loading] Failed to load bitmap Image.");
                if (sampleSize > 5) throw new Exception("[Bitmap Loading] Failed to load bitmap Image from maximum sample size threshhold (5)");
                //reduce sample size and try again
                return GetFullImage(i, sampleSize + 1);
            }

        }

        public void RefreshMedia()
        {
            MediaObjects = new List<MediaObject>();
            MediaObjects = App.MediaService.GetMediaFromFolder(Directory.ToString());
        }

        public void ChangeDirectory(string path)
        {
            DirectoryInfo TargetDir;

            // -- Path is Directory
            TargetDir = new DirectoryInfo(path);
            if (TargetDir.Exists)
            {
                Directory = TargetDir;
                RefreshMedia();
                return;
            }

            //If failed and is a file with an extention try opening its parent directory
            if (System.IO.Path.HasExtension(path))
            {
                ChangeDirectory(new DirectoryInfo(System.IO.Path.GetDirectoryName(path)).FullName);
            }
            
            
        }

        public MediaFolder ImagesOnly()
        {
            MediaObjects.RemoveAll((MediaObject a) => a.MediaType != MediaTypes.Image);
            return this;
        }

        public void AddFolder(string folderName)
        {
            App.MediaService.CreateFolder(folderName, Directory.FullName);
        }
    }
}