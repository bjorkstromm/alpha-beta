using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AlphaBeta.Core;
using GalaSoft.MvvmLight;
using System.IO;
using System.Windows.Input;
using System.Media;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using AlphaBeta.Utilities;

namespace AlphaBeta.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ImageService _imageService;
        private readonly WordService _wordService;
        private readonly AudioService _audioService;
        private readonly RelayCommand<KeyEventArgs> _keyUpCommand;
        private readonly RelayCommand<KeyEventArgs> _keyDownCommand;
        private int _currentIndex;
        private volatile bool _keyDown;

        public MainViewModel(ImageService imageService, WordService wordService, AudioService audioService)
        {
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _wordService = wordService ?? throw new ArgumentNullException(nameof(wordService));
            _audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
            _keyUpCommand = new RelayCommand<KeyEventArgs>(OnKeyUp, args => _keyDown);
            _keyDownCommand = new RelayCommand<KeyEventArgs>(OnKeyDown, args => !_keyDown);
        }

        public ICommand KeyUpCommand => _keyUpCommand;
        public ICommand KeyDownCommand => _keyDownCommand;

        public bool HasImages => Images?.Count > 0;

        private string _word;
        public string Word
        {
            get { return _word; }
            set
            {
                _word = value.ToUpper();
                RaisePropertyChanged();
            }
        }

        private string _typedWord;
        public string TypedWord
        {
            get { return _typedWord; }
            set
            {
                _typedWord = value.ToUpper();
                RaisePropertyChanged();
            }
        }

        private bool _waiting;
        public bool IsWaiting
        {
            get => _waiting;
            set
            {
                _waiting = value;
                RaisePropertyChanged();
            }
        }

        private List<ImageSource> _images;
        public List<ImageSource> Images
        {
            get => _images;
            set
            {
                _images = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(HasImages));
            }
        }

        public async Task NextWord()
        {
            IsWaiting = true;
            TypedWord = new String(' ', Word?.Length ?? 0);

            _currentIndex = 0;
            var word = _wordService.GetRandomWord();
            var images = (await _imageService.GetImagesByTagAsync(word.Query)).ToList();

            Images = new List<ImageSource>(images.Select(x => new BitmapImage(x.ThumbnailUri)));
            Word = word.Text;

            SayWord(Word);

            _keyDown = false;

            IsWaiting = false;
        }

        private void SayWord(string word)
        {
            if (!_audioService.IsEnabled)
            {
                return;
            }

            // Queue it on the thread pool to not lock up the UI.
            // We're not interested in the result, and we won't exhaust it.
            ThreadPool.QueueUserWorkItem(async state =>
            {
                try
                {
                    var audio = await _audioService.GetAudioAsync(word);
                    using (var memoryStream = new MemoryStream())
                    {
                        memoryStream.Write(audio.Data, 0, audio.Length);
                        memoryStream.Position = 0;
                        var player = new SoundPlayer(memoryStream);

                        player.PlaySync();
                    }
                }
                catch
                {
                    // Seen some exceptions thrown from here due to
                    // wave header being corrupt, so let's just catch 
                    // it for now and ignore it.
                }
            });
        }

        private void OnKeyDown(KeyEventArgs obj)
        {
            if (IsWaiting)
            {
                obj.Handled = true;
                return;
            }

            var character = KeyHelper.GetCharFromKey(obj.Key);
            if (!char.IsLetter(character) && character != ' ')
            {
                return;
            }

            if (!_keyDown)
            {
                _keyDown = true;
                var typedWord =
                    (TypedWord.Substring(0, _currentIndex) + KeyHelper.GetCharFromKey(obj.Key))
                    .PadRight(Word.Length);
                TypedWord = typedWord;
            }
        }

        private async void OnKeyUp(KeyEventArgs obj)
        {
            if (IsWaiting)
            {
                obj.Handled = true;
                return;
            }

            if (_keyDown)
            {
                if (!string.Equals(TypedWord.Substring(0, _currentIndex + 1), Word.Substring(0, _currentIndex + 1), StringComparison.OrdinalIgnoreCase))
                {
                    TypedWord = TypedWord.Remove(_currentIndex).PadRight(Word.Length);
                }
                else if (string.Equals(TypedWord, Word, StringComparison.OrdinalIgnoreCase))
                {
                    await NextWord();
                }
                else
                {
                    Interlocked.Increment(ref _currentIndex);
                }

                _keyDown = false;
            }
        }
    }
}