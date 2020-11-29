using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace alpha_beta.core
{
    public class AudioService
    {
        private readonly SpeechConfig _configuration;

        public AudioService(Configuration configuration)
        {
            _configuration = SpeechConfig.FromSubscription(configuration.SpeechKey, configuration.SpeechRegion);
            _configuration.SpeechSynthesisLanguage = configuration.Locale;
            _configuration.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff16Khz16BitMonoPcm);
        }

        public async Task<byte[]> GetAudioAsync(string text)
        {
            using (var synthesizer = new SpeechSynthesizer(_configuration, null))
            {
                var result = await synthesizer.SpeakTextAsync(text);

                return result.AudioData;
            }
        }
    }
}
