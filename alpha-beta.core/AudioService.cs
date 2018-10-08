using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace alpha_beta.core
{
    public class AudioService
    {
        private static readonly string TokenEndpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
        private static readonly string Enpdpoint = "https://speech.platform.bing.com/synthesize";
        private readonly Configuration _configuration;

        private static readonly IReadOnlyDictionary<string, string> ServiceName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ar-EG", "Microsoft Server Speech Text to Speech Voice (ar-EG, Hoda)" },
            { "ar-SA", "Microsoft Server Speech Text to Speech Voice (ar-SA, Naayf)" },
            { "bg-BG", "Microsoft Server Speech Text to Speech Voice (bg-BG, Ivan)" },
            { "ca-ES", "Microsoft Server Speech Text to Speech Voice (ca-ES, HerenaRUS)" },
            { "cs-CZ", "Microsoft Server Speech Text to Speech Voice (cs-CZ, Jakub)" },
            { "da-DK", "Microsoft Server Speech Text to Speech Voice (da-DK, HelleRUS)" },
            { "de-AT", "Microsoft Server Speech Text to Speech Voice (de-AT, Michael)" },
            { "de-CH", "Microsoft Server Speech Text to Speech Voice (de-CH, Karsten)" },
            { "de-DE", "Microsoft Server Speech Text to Speech Voice (de-DE, HeddaRUS)" },
            { "el-GR", "Microsoft Server Speech Text to Speech Voice (el-GR, Stefanos)" },
            { "en-AU", "Microsoft Server Speech Text to Speech Voice (en-AU, HayleyRUS)" },
            { "en-CA", "Microsoft Server Speech Text to Speech Voice (en-CA, HeatherRUS)" },
            { "en-GB", "Microsoft Server Speech Text to Speech Voice (en-GB, HazelRUS)" },
            { "en-IE", "Microsoft Server Speech Text to Speech Voice (en-IE, Sean)" },
            { "en-IN", "Microsoft Server Speech Text to Speech Voice (en-IN, PriyaRUS)" },
            { "en-US", "Microsoft Server Speech Text to Speech Voice (en-US, JessaRUS)" },
            { "es-ES", "Microsoft Server Speech Text to Speech Voice (es-ES, HelenaRUS)" },
            { "es-MX", "Microsoft Server Speech Text to Speech Voice (es-MX, HildaRUS)" },
            { "fi-FI", "Microsoft Server Speech Text to Speech Voice (fi-FI, HeidiRUS)" },
            { "fr-CA", "Microsoft Server Speech Text to Speech Voice (fr-CA, HarmonieRUS)" },
            { "fr-CH", "Microsoft Server Speech Text to Speech Voice (fr-CH, Guillaume)" },
            { "fr-FR", "Microsoft Server Speech Text to Speech Voice (fr-FR, HortenseRUS)" },
            { "he-IL", "Microsoft Server Speech Text to Speech Voice (he-IL, Asaf)" },
            { "hi-IN", "Microsoft Server Speech Text to Speech Voice (hi-IN, Kalpana, Apollo)" },
            { "hr-HR", "Microsoft Server Speech Text to Speech Voice (hr-HR, Matej)" },
            { "hu-HU", "Microsoft Server Speech Text to Speech Voice (hu-HU, Szabolcs)" },
            { "id-ID", "Microsoft Server Speech Text to Speech Voice (id-ID, Andika)" },
            { "it-IT", "Microsoft Server Speech Text to Speech Voice (it-IT, LuciaRUS)" },
            { "ja-JP", "Microsoft Server Speech Text to Speech Voice (ja-JP, HarukaRUS)" },
            { "ko-KR", "Microsoft Server Speech Text to Speech Voice (ko-KR, HeamiRUS)" },
            { "ms-MY", "Microsoft Server Speech Text to Speech Voice (ms-MY, Rizwan)" },
            { "nb-NO", "Microsoft Server Speech Text to Speech Voice (nb-NO, HuldaRUS)" },
            { "nl-NL", "Microsoft Server Speech Text to Speech Voice (nl-NL, HannaRUS)" },
            { "pl-PL", "Microsoft Server Speech Text to Speech Voice (pl-PL, PaulinaRUS)" },
            { "pt-BR", "Microsoft Server Speech Text to Speech Voice (pt-BR, HeloisaRUS)" },
            { "pt-PT", "Microsoft Server Speech Text to Speech Voice (pt-PT, HeliaRUS)" },
            { "ro-RO", "Microsoft Server Speech Text to Speech Voice (ro-RO, Andrei)" },
            { "ru-RU", "Microsoft Server Speech Text to Speech Voice (ru-RU, EkaterinaRUS)" },
            { "sk-SK", "Microsoft Server Speech Text to Speech Voice (sk-SK, Filip)" },
            { "sl-SI", "Microsoft Server Speech Text to Speech Voice (sl-SI, Lado)" },
            { "sv-SE", "Microsoft Server Speech Text to Speech Voice (sv-SE, HedvigRUS)" },
            { "ta-IN", "Microsoft Server Speech Text to Speech Voice (ta-IN, Valluvar)" },
            { "te-IN", "Microsoft Server Speech Text to Speech Voice (te-IN, Chitra)" },
            { "th-TH", "Microsoft Server Speech Text to Speech Voice (th-TH, Pattara)" },
            { "tr-TR", "Microsoft Server Speech Text to Speech Voice (tr-TR, SedaRUS)" },
            { "vi-VN", "Microsoft Server Speech Text to Speech Voice (vi-VN, An)" },
            { "zh-CN", "Microsoft Server Speech Text to Speech Voice (zh-CN, Yaoyao, Apollo)" },
            { "zh-HK", "Microsoft Server Speech Text to Speech Voice (zh-HK, TracyRUS)" },
            { "zh-TW", "Microsoft Server Speech Text to Speech Voice (zh-TW, HanHanRUS)" }
        };

        public AudioService(Configuration configuration)
        {
            _configuration = configuration;
        }

        public async Task<byte[]> GetAudioAsync(string text)
        {
            var token = await GetTokenAsync();
            var content = Encoding.UTF8.GetBytes(GenerateSsml(_configuration.Locale, "female", text));

            var request = (HttpWebRequest)HttpWebRequest.Create(Enpdpoint);
            request.Method = "POST";
            request.Headers["Authorization"] = $"Bearer {token}";
            request.Headers["X-Microsoft-OutputFormat"] = "riff-16khz-16bit-mono-pcm";
            request.UserAgent = "alpha-beta-client";
            request.ContentType = "application/ssml+xml; charset=utf-8";
            request.ContentLength = content.Length;
            using (var contentStream = request.GetRequestStream())
            {
                contentStream.Write(content, 0, content.Length);
            }

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private async Task<string> GetTokenAsync()
        {
            var request = WebRequest.Create(TokenEndpoint);
            request.Method = "POST";
            request.ContentLength = 0;
            request.Headers["Ocp-Apim-Subscription-Key"] = _configuration.SpeechKey;

            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private string GenerateSsml(string locale, string gender, string text)
        {
            var ssmlDoc = new XDocument(
                new XElement("speak",
                    new XAttribute("version", "1.0"),
                    new XAttribute(XNamespace.Xml + "lang", locale),
                    new XElement("voice",
                        new XAttribute(XNamespace.Xml + "lang", locale),
                        new XAttribute(XNamespace.Xml + "gender", gender),
                        new XAttribute("name", ServiceName[locale]),
                        text)));
            return ssmlDoc.ToString();
        }
    }
}
