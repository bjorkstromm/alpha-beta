using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using alpha_beta.core;
using GalaSoft.MvvmLight;
using System.Windows;
using System.IO;
using GalaSoft.MvvmLight.Threading;
using System.Net.Http;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using System.Media;
using System.Threading;

namespace alpha_beta.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly ImageService _imageService;
        private readonly WordService _wordService;
        private readonly AudioService _audioService;
        private readonly RelayCommand<KeyEventArgs> _keyUpCommand;
        private readonly RelayCommand<KeyEventArgs> _keyDownCommand;
        private int _currentIndex;
        private volatile bool _keyDown;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ImageService imageService, WordService wordService, AudioService audioService)
        {
            _imageService = imageService;
            _wordService = wordService;
            _audioService = audioService;
            _keyUpCommand = new RelayCommand<KeyEventArgs>(OnKeyUp, args => _keyDown);
            _keyDownCommand = new RelayCommand<KeyEventArgs>(OnKeyDown, args => !_keyDown);
        }

        private void OnKeyDown(KeyEventArgs obj)
        {
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

        private ImageSource _image1;
        private ImageSource _image2;
        private ImageSource _image3;
        private ImageSource _image4;
        private ImageSource _image5;

        public ImageSource Image1
        {
            get { return _image1; }
            set
            {
                _image1 = value;
                RaisePropertyChanged();
            }
        }

        public ImageSource Image2
        {
            get { return _image2; }
            set
            {
                _image2 = value;
                RaisePropertyChanged();
            }
        }

        public ImageSource Image3
        {
            get { return _image3; }
            set
            {
                _image3 = value;
                RaisePropertyChanged();
            }
        }

        public ImageSource Image4
        {
            get { return _image4; }
            set
            {
                _image4 = value;
                RaisePropertyChanged();
            }
        }

        public ImageSource Image5
        {
            get { return _image5; }
            set
            {
                _image5 = value;
                RaisePropertyChanged();
            }
        }

        public ICommand KeyUpCommand => _keyUpCommand;
        public ICommand KeyDownCommand => _keyDownCommand;

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

        public async Task NextWord()
        {
            _currentIndex = 0;
            Word = _wordService.GetRandomWord();
            TypedWord = new String(' ', Word.Length);
            var images = (await _imageService.GetImagesByTagAsync(Word, 5)).ToList();
            Image1 = new BitmapImage(images[0].ThumbnailUri);
            Image2 = new BitmapImage(images[1].ThumbnailUri);
            Image3 = new BitmapImage(images[2].ThumbnailUri);
            Image4 = new BitmapImage(images[3].ThumbnailUri);
            Image5 = new BitmapImage(images[4].ThumbnailUri);

            var audio = await _audioService.GetAudioAsync(Word);

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(audio, 0, audio.Length);
                memoryStream.Position = 0;
                var player = new SoundPlayer(memoryStream);
                player.PlaySync();
            }
            _keyDown = false;
        }
    }
}