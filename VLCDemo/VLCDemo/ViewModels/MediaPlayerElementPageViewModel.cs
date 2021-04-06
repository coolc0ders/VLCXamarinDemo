using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using LibVLCSharp.Shared;

namespace VLCDemo.ViewModels
{
    /// <summary>
    /// NOTE: Code sample gotten from : https://code.videolan.org/mfkl/libvlcsharp-samples/-/blob/master/MediaElement/MediaElement/MainViewModel.cs
    /// </summary>
    public class MediaPlayerElementPageViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="MainViewModel"/> class.
        /// </summary>
        public MediaPlayerElementPageViewModel()
        {
        }

        private LibVLC _libVLC;
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.LibVLC"/> instance.
        /// </summary>
        public LibVLC LibVLC
        {
            get => _libVLC;
            private set => Set(nameof(LibVLC), ref _libVLC, value);
        }

        private MediaPlayer _mediaPlayer;
        /// <summary>
        /// Gets the <see cref="LibVLCSharp.Shared.MediaPlayer"/> instance.
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set => Set(nameof(MediaPlayer), ref _mediaPlayer, value);
        }

        /// <summary>
        /// Initialize LibVLC and playback when page appears
        /// </summary>
        public void OnAppearing()
        {
            Core.Initialize();

            LibVLC = new LibVLC();

            var media = new Media(LibVLC,
                new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4"));

            MediaPlayer = new MediaPlayer(media) { EnableHardwareDecoding = true };
            MediaPlayer.Play();
        }

        private void Set<T>(string propertyName, ref T field, T value)
        {
            if (field == null && value != null || field != null && !field.Equals(value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
