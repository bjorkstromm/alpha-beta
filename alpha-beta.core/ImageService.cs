using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace alpha_beta.core
{
    public class ImageService
    {
        private static string Endpoint = "https://api.bing.microsoft.com/v7.0/images/search";
        private readonly Configuration _configuration;

        public ImageService(Configuration configuration)
        {
            _configuration = configuration;
        }

        public Task<IEnumerable<Image>> GetImagesByTagAsync(string tag)
        {
            return GetImagesByTagAsync(tag, 5);
        }

        public async Task<IEnumerable<Image>> GetImagesByTagAsync(string tag, int count)
        {
            var uriQuery = Endpoint
                + $"?q={Uri.EscapeDataString(tag)}"
                + $"&count={Uri.EscapeDataString(count.ToString())}"
                + $"&mkt={Uri.EscapeDataString(_configuration.Locale)}"
                + $"&setLang={Uri.EscapeDataString(_configuration.Locale)}"
                + "&safeSearch=strict";

            var request = WebRequest.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = _configuration.SearchKey;

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var json = JObject.Parse(reader.ReadToEnd());

                return json["value"].Select(t => 
                    new Image(
                        new Uri(t["thumbnailUrl"].ToString()), 
                        new Uri(t["contentUrl"].ToString())));
            }
        }
    }
}
