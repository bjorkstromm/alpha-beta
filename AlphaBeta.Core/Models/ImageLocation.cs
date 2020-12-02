using System;

namespace AlphaBeta.Core
{
    public class ImageLocation
    {
        public Uri ThumbnailUri { get; }
        public Uri ContentUri { get; }

        public ImageLocation(Uri thumbnailUri, Uri contentUri)
        {
            ThumbnailUri = thumbnailUri;
            ContentUri = contentUri;
        }
    }
}
