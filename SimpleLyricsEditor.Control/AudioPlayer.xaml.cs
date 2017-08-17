﻿using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using SimpleLyricsEditor.DAL;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleLyricsEditor.Control
{
    public sealed partial class AudioPlayer : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(Music), typeof(AudioPlayer), new PropertyMetadata(Music.Empty));

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
            nameof(Time), typeof(TimeSpan), typeof(AudioPlayer), new PropertyMetadata(TimeSpan.Zero));

        public static readonly DependencyProperty IsPlayProperty = DependencyProperty.Register(
            nameof(IsPlay), typeof(bool), typeof(AudioPlayer), new PropertyMetadata(false));

        private Music musicTemp;
        private ThreadPoolTimer refreshTime_Timer;

        public AudioPlayer()
        {
            InitializeComponent();
            Rewind_Button.Opacity = 0;
            FastForward_Button.Opacity = 0;
            RewindButton_Transform.TranslateX = 44;
            FastForwardButton_Transform.TranslateX = -44;

            Position_Slider.AddHandler(PointerPressedEvent, new PointerEventHandler((s, e) => refreshTime_Timer?.Cancel()), true);
            Position_Slider.AddHandler(PointerReleasedEvent, new PointerEventHandler(Position_Slider_PointerReleased), true);

        }

        public Music Source
        {
            get => (Music) GetValue(SourceProperty);
            private set => SetValue(SourceProperty, value);
        }

        public bool IsPlay
        {
            get => (bool) GetValue(IsPlayProperty);
            private set => SetValue(IsPlayProperty, value);
        }

        private TimeSpan Time
        {
            get => (TimeSpan) GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public TimeSpan Position => Player.Position;

        public async void SetSource(Music source)
        {
            if (source.Equals(Music.Empty))
                return;

            musicTemp = source;

            Player.SetSource(await musicTemp.File.OpenAsync(FileAccessMode.Read), musicTemp.File.ContentType);
        }

        public void SetPosition(TimeSpan newPosition)
        {
            Time = newPosition;
            Player.Position = Time;
        }

        public async Task PickMusicFile()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".wav");
            picker.FileTypeFilter.Add(".flac");
            picker.FileTypeFilter.Add(".alac");
            picker.FileTypeFilter.Add(".aac");
            picker.FileTypeFilter.Add(".m4a");
            picker.FileTypeFilter.Add(".mp3");
            var file = await picker.PickSingleFileAsync();

            if (file != null)
                SetSource(await Music.Parse(file));
        }

        private void RefreshTime()
        {
            Time = Player.Position;
        }

        private void StartRefreshTimeTimer()
        {
            async void refreshTime(ThreadPoolTimer timer)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, RefreshTime);
            }

            refreshTime_Timer = ThreadPoolTimer.CreatePeriodicTimer(refreshTime, TimeSpan.FromMilliseconds(50));
        }

        public void DisplayPositionControlButtons()
        {
            if (RewindButton_Transform.TranslateX.Equals(44))
                Expand.Begin();
        }

        public void HidePositionControlButtons()
        {
            if (RewindButton_Transform.TranslateX.Equals(0))
                Folding.Begin();
        }

        public void Play()
        {
            Player.Play();
            StartRefreshTimeTimer();
        }

        public void Pause()
        {
            Player.Pause();
            refreshTime_Timer.Cancel();
            RefreshTime();
        }

        public void Rewind()
        {
            var ms = 500;
            var newPosition = Position - TimeSpan.FromMilliseconds(ms) >= TimeSpan.Zero ? Position - TimeSpan.FromMilliseconds(ms) : TimeSpan.Zero;

            SetPosition(newPosition);
        }

        public void FastForward()
        {
            var ms = 500;
            var newPosition = Position + TimeSpan.FromMilliseconds(ms) <= Player.NaturalDuration.TimeSpan ? Position + TimeSpan.FromMilliseconds(ms) : Player.NaturalDuration.TimeSpan;

            SetPosition(newPosition);
        }

        private async void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Source.Equals(Music.Empty))
                await PickMusicFile();
            else
                Play();
        }

        private void Player_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (Player.CurrentState)
            {
                case MediaElementState.Closed:
                    break;
                case MediaElementState.Opening:
                    break;
                case MediaElementState.Buffering:
                    break;
                case MediaElementState.Playing:
                    IsPlay = true;
                    break;
                case MediaElementState.Paused:
                case MediaElementState.Stopped:
                    IsPlay = false;
                    break;
            }
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            Source = musicTemp;
            DisplayPositionControlButtons();
            Play();
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            Pause();
            SetPosition(TimeSpan.Zero);
        }

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            throw new Exception(e.ErrorMessage);
        }

        private void Position_Slider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Slider slider)
                SetPosition(TimeSpan.FromMinutes(slider.Value));
            if (IsPlay)
                StartRefreshTimeTimer();
        }
    }
}