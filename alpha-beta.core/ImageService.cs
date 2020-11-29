using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            var requestUri = Endpoint
                + $"?q={Uri.EscapeDataString(tag)}"
                + $"&count={Uri.EscapeDataString(count.ToString())}"
                + $"&mkt={Uri.EscapeDataString(_configuration.Locale)}"
                + $"&setLang={Uri.EscapeDataString(_configuration.Locale)}"
                + "&safeSearch=strict";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration.SearchKey);

            using var response = await httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            return json["value"].Select(t => 
                new Image(
                    new Uri(t["thumbnailUrl"].ToString()), 
                    new Uri(t["contentUrl"].ToString())));
            
        }
    }
}
