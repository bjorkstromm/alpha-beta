using System;

namespace alpha_beta.core
{
    public class Image
    {
        public Uri ThumbnailUri { get; }
        public Uri ContentUri { get; }

        public Image(Uri thumbnailUri, Uri contentUri)
        {
            ThumbnailUri = thumbnailUri;
            ContentUri = contentUri;
        }
    }
}
