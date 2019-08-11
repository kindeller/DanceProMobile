using System.Collections.Generic;
using DancePro.Models;
namespace DancePro.Droid
{

    public class MediaFolder
    {
        private List<MediaObject> MediaObjects;

        public MediaFolder()
        {
            MediaObjects = new List<MediaObject>();
            MediaObjects = App.MediaService.GetMediaFromFolder();
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
    }
}