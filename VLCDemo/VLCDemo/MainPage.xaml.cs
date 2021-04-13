using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            _mediaPlayer.
            _mediaPlayer.Play();
        }

        private void MediaPlayerPositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            DurationSlider.Value = e.Position * 100;
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
            if (Math.Round(e.NewValue, 2) != Math.Round(_mediaPlayer.Position * 100, 2))
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
