# alpha-beta
A simple learning game, which uses Azure cognitive services, that I've made for my children. It picks a random word from a list of words for the user to type. It uses `Bing Search v7` for searching related images and `Text to Speech` for reading the word out loud.

![demo](./media/demo.gif)

# Prerequisites
- Visual Studio 2019 or later
- Active Azure Subscription with [Bing Search v7](https://docs.microsoft.com/en-us/bing/search-apis/bing-web-search/create-bing-search-service-resource) and [Speech](https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/overview#try-the-speech-service-for-free) resources created.

# Build
- Open `alpha-beta.sln` in Visual Studio.
- Modify `alpha-beta\appsettings.json` and add your cognitive services API keys.
  - Add `Bing Search v7` key to `searchKey`
  ![search-key](./media/search-keys.PNG)
  - Add `Speech` key to `speechKey` and location to `speechRegion`
  ![speech-key](./media/speech-keys.PNG)
  - Alternatively modify language by modifying `locale`. Supported locales can be found [here](https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#standard-voices)
- Build

# Add/remove words used
- Open up `words.txt` and add/remove words. Each word is separated by newline.
