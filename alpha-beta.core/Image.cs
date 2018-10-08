using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
