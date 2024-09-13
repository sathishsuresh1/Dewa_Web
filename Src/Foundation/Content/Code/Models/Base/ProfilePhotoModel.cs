using System;

namespace DEWAXP.Foundation.Content.Models.Base
{
    [Serializable]
    public class ProfilePhotoModel
    {
        public bool HasProfilePhoto { get; set; }
        public byte[] ProfilePhoto { get; set; }
    }
}