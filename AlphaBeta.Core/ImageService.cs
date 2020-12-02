using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AlphaBeta.Core
{
    public class ImageService
    {
        private static string Endpoint = "https://api.bing.microsoft.com/v7.0/images/search";
        private readonly Configuration _configuration;

        public bool IsEnabled { get; }

        public ImageService(Configuration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            IsEnabled = !string.IsNullOrEmpty(_configuration.SearchKey);
        }

        public Task<IEnumerable<ImageLocation>> GetImagesByTagAsync(string tag)
        {
            if (!IsEnabled)
            {
                return Task.FromResult(Enumerable.Empty<ImageLocation>());
            }

            return GetImagesByTagAsync(tag, 5);
        }

        private async Task<IEnumerable<ImageLocation>> GetImagesByTagAsync(string tag, int count)
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
                new ImageLocation(
                    new Uri(t["thumbnailUrl"].ToString()), 
                    new Uri(t["contentUrl"].ToString())));
            
        }
    }
}
