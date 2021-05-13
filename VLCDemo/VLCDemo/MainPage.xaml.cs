using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using Xamarin.Forms;

namespace VLCDemo
{
    public partial class MainPage : ContentPage
    {
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;

        #region MoviePosition
        public static readonly BindableProperty MoviePositionProperty = BindableProperty.Create(nameof(MoviePosition), typeof(float), typeof(MainPage), propertyChanged: (obj, old, newV) =>
        {
            var me = obj as MainPage;
            if (newV != null && !(newV is float)) return;
            var oldMoviePosition = (float)old;
            var newMoviePosition = (float)newV;
            me?.MoviePositionChanged(oldMoviePosition, newMoviePosition);
        });

        private void MoviePositionChanged(float oldMoviePosition, float newMoviePosition)
        {

        }

        /// <summary>
        /// A bindable property
        /// </summary>
        public float MoviePosition
        {
            get => (float)GetValue(MoviePositionProperty);
            set => SetValue(MoviePositionProperty, value);
        }
        #endregion

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //VideoView.MediaPlayerChanged += MediaPlayerChanged;
            Core.Initialize();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC)
            {
                Media = new Media(_libVLC, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4"))
            };
            
            VideoView.MediaPlayer = _mediaPlayer;
            _mediaPlayer.PositionChanged += MediaPlayerPositionChanged;
            _mediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
            _mediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
            _mediaPlayer.EndReached += MediaPlayer_EndReached;
            _mediaPlayer.Playing += MediaPlayer_Playing;
            _mediaPlayer.Play();
        }

        public async Task PlayStreamWithHeaders()
        {
            var stream = await GetStreamFromUrl("http://mediathatrequireauth", new Dictionary<string, string>
            {
                { "Authentication", "Bearer {Token goes here}"}
            });
            var mediaInput = new StreamMediaInput(stream);
            var media = new Media(_libVLC, mediaInput);
            _mediaPlayer.Play(media);
        }

        private async Task<Stream> GetStreamFromUrl(string url, Dictionary<string, string> headers)
        {
            byte[] data;

            using (var client = new System.Net.Http.HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                data = await client.GetByteArrayAsync(url);
            }

            return new MemoryStream(data);
        }


        private void MediaPlayer_Playing(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                DurationLabel.Text = string.Format("{0:mm\\:ss}", TimeSpan.FromMilliseconds(_mediaPlayer.Length));
            });
        }

        private void MediaPlayer_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ElapsedTimeLabel.Text = string.Format("{0:mm\\:ss}", TimeSpan.FromMilliseconds(_mediaPlayer.Time));
            });
        }

        private void MediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            //Called when an error occures while playing the media item.
        }

        private void MediaPlayer_EndReached(object sender, EventArgs e)
        {
            //Called when the media player terminates the current media
            //Note: When this is called, if you have a queue to manage, call the next item on the queue.
        }

        private void MediaPlayerPositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            MoviePosition = e.Position * 100;
        }

        private void StopButton_Clicked(object sender, EventArgs e)
        {
            _mediaPlayer.Stop();
        }

        void SeekTo(TimeSpan seconds)
        {
            _mediaPlayer.Time = (long)seconds.TotalMilliseconds;
        }

        private void PlayPauseButton_Clicked(object sender, EventArgs e)
        {
            //Note: Use the set pause option to resume the player from pause state
            //Since using the play option will play as if it was a new media we added
            _mediaPlayer.SetPause(_mediaPlayer.IsPlaying);
            PlayPauseButton.Text =
                _mediaPlayer.IsPlaying ? "Pause" : "Play";
        }

        private void Back10SecsButton_Clicked(object sender, EventArgs e)
        {
            SeekTo(TimeSpan.FromMilliseconds(_mediaPlayer.Time) - TimeSpan.FromSeconds(10));
        }

        private void Forward10SecsButton_Clicked(object sender, EventArgs e)
        {
            SeekTo(TimeSpan.FromMilliseconds(_mediaPlayer.Time) + TimeSpan.FromSeconds(10));
        }

        private void DurationSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.NewValue != Math.Round(_mediaPlayer.Position * 100, 2))
            {
                var val = e.NewValue;
                _mediaPlayer.Position = (float) (val / 100);
            }
        }

        private void MuteUnMuteButton_Clicked(object sender, EventArgs e)
        {
            _mediaPlayer.Mute = !_mediaPlayer.Mute;
            MuteUnMuteButton.Text = _mediaPlayer.Mute ? "Unmute" : "Mute";
        }

        private void VolumeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            _mediaPlayer.Volume = (int)e.NewValue;
        }
    }
}
