using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace AlphaBeta.Core
{
    public class AudioService
    {
        private readonly SpeechConfig _configuration;

        public bool IsEnabled { get; }

        public AudioService(Configuration configuration)
        {
            IsEnabled = false;

            if (!string.IsNullOrWhiteSpace(configuration.SpeechKey))
            {
                _configuration = SpeechConfig.FromSubscription(configuration.SpeechKey, configuration.SpeechRegion);
                _configuration.SpeechSynthesisLanguage = configuration.Locale;

                IsEnabled = true;
            }
        }

        public async Task<Audio> GetAudioAsync(string text)
        {
            if (IsEnabled)
            {
                using var synthesizer = new SpeechSynthesizer(_configuration, null);
                var result = await synthesizer.SpeakTextAsync(text);
                return new Audio
                {
                    Text = text,
                    Data = result.AudioData,
                };
            }

            return null;
        }
    }
}
