using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.IO;

namespace Osu_Cancer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Field/Property
        bool isDebugPanelHold = false;
        Point holdDebugPannlPosition;
        Point lastDebugPanelPosition;
        int lastImage;
        int LastImage
        {
            get
            {
                return lastImage;
            }
            set
            {
                if (value < workingResources.CurrentSkin.Background.Count)
                    lastImage = value;
                else
                    lastImage = 0;
            }
        }
        Cons workingResources = new Cons();
        List<BeatmapGroup> beatmaps = new List<BeatmapGroup>();
        List<SkinsLib> skins = new List<SkinsLib>();
        OsuSection currentSection = OsuSection.PreLoad;
        public static RoutedCommand openSetting = new RoutedCommand();
        public static RoutedCommand escPressed = new RoutedCommand();
        GameIcon IconLib;
        int lastSongIndex;
        int LastSongIndex
        {
            get { return lastSongIndex; }
            set
            {
                if (value < beatmaps.Count && value > -1)
                    lastSongIndex = value;
                else
                    lastSongIndex = 0;
            }
        }
        Thread messageOverlayHideInterval;
        bool isMessageOverlayHideIntervalActive = false;
        Thread TempShowSongInfo;
        Thread _volumeGauge;
        Thread songProgress;
        bool isTempThreadRunning = false;
        bool isAwaitVolumeGaugeRunning = false;
        int songLength;
        Thickness defaultLogoMargin;
        #endregion

        public MainWindow()
        {           
            InitializeComponent();
            scrollUpdateLog.ScrollToEnd();
            DataContext = workingResources;      
            imgOsuLogo.Opacity = 0;
            IconLib = new GameIcon(workingResources.BaseDir);
            pgbSong.DataContext = workingResources;
            MusicIcon.DataContext = IconLib;
            GetSongName();
            GetSkinInfo();
            Task task = new Task(() =>
            {

                Thread.Sleep(3000);
                Dispatcher.Invoke(FadeInBlackBackGround);
            });
            Task task2 = new Task(() =>
            {
                Thread.Sleep(6000);
                try { Dispatcher.Invoke(MainOsu); }
                catch { }
            });
            task.Start();
            SongControlPanel.DataContext = IconLib;
            task2.Start();
        }

        #region Method
        private void GetSkinInfo()
        {
            string[] Skins = Directory.GetDirectories(workingResources.SkinsParentpath);
            foreach (var item in Skins)
            {
                skins.Add(new SkinsLib(item));
            }
            workingResources.CurrentSkin = skins[0];
        }
        private void GetSongName()
        {
            string[] beatmapGroup = Directory.GetDirectories(workingResources.SongPath);
            foreach (var item in beatmapGroup)
            {
                beatmaps.Add(new BeatmapGroup(item));
            }
        }
        private void MainOsu()
        {
            BlackCurtain.Visibility = Visibility.Collapsed;
            imgOsuLogo.BeginAnimation(OpacityProperty, null);
            imgOsuLogo.Opacity = 1;
            currentSection = OsuSection.MainScreen;
            HotkeyImplementing();
            BackgroundSlide();
            BGMPlayer.Source = new Uri(workingResources.BaseDir + @"\Resources\Default Audio\Circle.wav");
            BGMPlayer.Play();
            MainOsuHeader.Visibility = Visibility.Visible;
            BGMPlayer.MediaEnded += BGMPlayer_MediaEnded;
            AccountInfo.Visibility = Visibility.Visible;
            imgOsuLogo.Margin = new Thickness(130);
            defaultLogoMargin = imgOsuLogo.Margin;
        }
        private void HotkeyImplementing()
        {
            openSetting.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            escPressed.InputGestures.Add(new KeyGesture(Key.Escape));
        }
        private void BackgroundSlide()
        {
            workingResources.CurrentSkin = skins.Find((SkinsLib obj) =>
            {
                return obj.SkinName == workingResources.CurrentSkin.SkinName;
            });

            BackgroundImage.Source = new BitmapImage(new Uri(workingResources.CurrentSkin.Background[LastImage]));
            LastImage++;
        }
        private void FadeInBlackBackGround()
        {
            BGMPlayer.Source = new Uri(workingResources.BaseDir + @"\Resources\Default Audio\Welcome.wav");
            BGMPlayer.Play();
            DoubleAnimation animation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            BlackCurtain.BeginAnimation(OpacityProperty, animation);
            DoubleAnimation animationLogo = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            imgOsuLogo.BeginAnimation(OpacityProperty, animationLogo);
        }
        private void ShowOverlayMessage(string text)
        {
            if(isMessageOverlayHideIntervalActive)
                messageOverlayHideInterval.Abort();

            HideOverlayMessage();
            OverlayMessage.Visibility = Visibility.Visible;
            txblOverlayMessage.Text = text;
            OverlayMessage.Opacity = 1;
            DoubleAnimation expandAnimation = new DoubleAnimation(0, 50, TimeSpan.FromSeconds(0.1), FillBehavior.HoldEnd);
            OverlayMessage.BeginAnimation(HeightProperty, expandAnimation);
            messageOverlayHideInterval = new Thread(() => {
                Thread.Sleep(1500);
                Dispatcher.Invoke(HideOverlayMessage);
            });
            messageOverlayHideInterval.Start();
            isMessageOverlayHideIntervalActive = true;
        }
        private void HideOverlayMessage()
        { 
            OverlayMessage.Opacity = 0;
            OverlayMessage.BeginAnimation(OpacityProperty, null);
            OverlayMessage.BeginAnimation(HeightProperty, null);
            OverlayMessage.Visibility = Visibility.Collapsed;
            isMessageOverlayHideIntervalActive = false;
        }
        
        #endregion

        #region EventHandler related to the debug tab
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Grid gridItemContainer = border.Child as Grid;
            Grid closeIcon = gridItemContainer.Children[1] as Grid;

            closeIcon.Opacity = 0.5;
        }
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            Grid gridItemContainer = border.Child as Grid;
            Grid closeIcon = gridItemContainer.Children[1] as Grid;

            closeIcon.Opacity = 0;
        }
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.Opacity = 1;
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DebugPanelBorder.Visibility = Visibility.Collapsed;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDebugPanelHold = true;
            holdDebugPannlPosition = Mouse.GetPosition(RectDebugPanel);
        }
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDebugPanelHold)
            {
                lastDebugPanelPosition = Mouse.GetPosition(this);
                Canvas.SetLeft(DebugPanelBorder, lastDebugPanelPosition.X - holdDebugPannlPosition.X);
                Canvas.SetTop(DebugPanelBorder, lastDebugPanelPosition.Y - holdDebugPannlPosition.Y);
            }
        }
        private void DebugPanelBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDebugPanelHold = false;
        }
        #endregion

        #region EventHandler related to Main Osu!
        private void imgOsuLogo_MouseEnter(object sender, MouseEventArgs e)
        {
            if (currentSection != OsuSection.MainScreen)
                return;
            ThicknessAnimation animation = new ThicknessAnimation(new Thickness(defaultLogoMargin.Left - 10, defaultLogoMargin.Top - 10, defaultLogoMargin.Right - 10, defaultLogoMargin.Bottom - 10), TimeSpan.FromSeconds(0.1), FillBehavior.HoldEnd);
            imgOsuLogo.BeginAnimation( MarginProperty, animation);
        }
        private void imgOsuLogo_MouseLeave(object sender, MouseEventArgs e)
        {
            if (currentSection != OsuSection.MainScreen)
                return;
            ThicknessAnimation animation = new ThicknessAnimation(defaultLogoMargin, TimeSpan.FromSeconds(0.1), FillBehavior.HoldEnd);
            imgOsuLogo.BeginAnimation(MarginProperty, animation);
        }
        private void DynamicBGAction(object sender, MouseEventArgs e)
        {

            double ScreenXRatio = workingResources.ScreenX / 100;
            double ScreenYRatio = workingResources.ScreenY / 100;
            Point CurrentMousePos = Mouse.GetPosition(this);
            Point ScaledMousePos = new Point(CurrentMousePos.X / ScreenXRatio, CurrentMousePos.Y / ScreenYRatio);
            double leftMargin = (ScaledMousePos.X - 50) / 10 - 10;
            double topMargin = (ScaledMousePos.Y - 50) / 10 - 10;
            BackgroundImage.Margin = new Thickness(leftMargin, topMargin, -10 - leftMargin, -10 - topMargin);
        }
        #endregion

        #region Setting Related
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (workingResources.IsSettingPanelOpen)
                return;
            SettingPanelBackground.Visibility = Visibility.Visible;
            SettingPanel.Visibility = Visibility.Visible;
            SettingPanelBackground.BeginAnimation(WidthProperty, null);
            SettingPanel.BeginAnimation(OpacityProperty, null);
            SettingPanel.Opacity = 1;
            DoubleAnimation slideInAnimation = new DoubleAnimation(0, 500, TimeSpan.FromSeconds(0.25), FillBehavior.HoldEnd);
            SettingPanelBackground.BeginAnimation(WidthProperty, slideInAnimation);
            workingResources.IsSettingPanelOpen = true;
        }
        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            if (!workingResources.IsSettingPanelOpen)
                return;

            SettingPanelBackground.BeginAnimation(WidthProperty, null);
            SettingPanel.BeginAnimation(OpacityProperty, null);
            DoubleAnimation slideInAnimation = new DoubleAnimation(500, 0, TimeSpan.FromSeconds(0.25), FillBehavior.HoldEnd);
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.25), FillBehavior.HoldEnd);
            fadeOut.Completed += FadeOut_Completed;
            SettingPanelBackground.BeginAnimation(WidthProperty, slideInAnimation);
            SettingPanel.BeginAnimation(OpacityProperty, fadeOut);
            workingResources.IsSettingPanelOpen = false;
        }
        private void FadeOut_Completed(object sender, EventArgs e)
        {
            SettingPanelBackground.Visibility = Visibility.Collapsed;
            SettingPanel.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region EventHandler Related to MusicControlPanel
        private void ControlsButtonZoomIn(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            ThicknessAnimation ZoomIn = new ThicknessAnimation(new Thickness(10), new Thickness(5), TimeSpan.FromSeconds(0.08), FillBehavior.HoldEnd);
            image.BeginAnimation(MarginProperty, ZoomIn);
        }
        private void ControlsButtonZoomOut(object sender, MouseEventArgs e)
        {
            Image image = sender as Image;
            ThicknessAnimation ZoomOut = new ThicknessAnimation(new Thickness(5), new Thickness(10), TimeSpan.FromSeconds(0.08), FillBehavior.HoldEnd);
            image.BeginAnimation(MarginProperty, ZoomOut);
        }
        private void PauseClick(object sender, MouseButtonEventArgs e)
        {
            BGMPlayer.Pause();
            workingResources.IsBGMPause = true;
            ShowOverlayMessage("Pause");
        }
        private void PlayClick(object sender, MouseButtonEventArgs e)
        {
            if (workingResources.IsBGMPause)
                BGMPlayer.Play();
            else
            {
                BGMPlayer.Position = TimeSpan.FromSeconds(0);
                BGMPlayer.Play();
            }
            ShowOverlayMessage("Play");
            workingResources.IsBGMPause = false;
        }
        private void StopClick(object sender, MouseButtonEventArgs e)
        {
            BGMPlayer.Stop();
            workingResources.IsBGMPause = true;
            ShowOverlayMessage("Stop Playing");
        }
        private void NextClick(object sender, MouseButtonEventArgs e)
        {
            NextSong();
            ShowOverlayMessage(">> Next");
            BGMPlayer.Play();
        }
        private void PreviousClick(object sender, MouseButtonEventArgs e)
        {
            LastSongIndex--;
            BGMPlayer.Source = new Uri(beatmaps[LastSongIndex].GroupPath + @"\Background.mp3");
            workingResources.Nowplaying =  beatmaps[LastSongIndex].Artist + " - " + beatmaps[LastSongIndex].SongName;
            SongTrans();
            ShowOverlayMessage("<< Prev");
            BGMPlayer.Play();
        }
        private void ShowHideInfo(object sender, MouseButtonEventArgs e)
        {
            if (workingResources.isShowPernamentlyInfo)
                ShowOverlayMessage("Song info will be tempotary displayed");
            else
                ShowOverlayMessage("Song info will be permanently displayed");

            workingResources.isShowPernamentlyInfo = !workingResources.isShowPernamentlyInfo;
            SongTrans();
            SongChangeAction();
        }
        private void HideInTenSecs()
        {
            if (workingResources.isShowPernamentlyInfo)
                return;
            ThicknessAnimation slideOut = new ThicknessAnimation(new Thickness(0, 0, 0, 75), new Thickness(0, 0, -NowplayingTextPanel.Width, 75), TimeSpan.FromSeconds(1), FillBehavior.Stop);
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1), FillBehavior.Stop);
            fadeOut.Completed += FadeOut_Completed1;

            NowplayingTextPanel.BeginAnimation(MarginProperty, slideOut);
            NowplayingTextPanel.BeginAnimation(OpacityProperty, fadeOut);
        }
        private void FadeOut_Completed1(object sender, EventArgs e)
        {
            NowplayingTextPanel.Visibility = Visibility.Collapsed;
            ThicknessAnimation animation = new ThicknessAnimation(new Thickness(1000, 55, 0, 25), new Thickness(1000, 0, 0, 80), TimeSpan.FromSeconds(0.5), FillBehavior.HoldEnd);
            NowplayingControlPanel.BeginAnimation(MarginProperty, animation);
        }
        private void SongChange(object sender, RoutedEventArgs e)
        {
            SongChangeAction();
            songLength = (int)BGMPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            songProgress = new Thread(SongProgressUpdate) { IsBackground = true };
            songProgress.Start();
        }
        private void SongChangeAction()
        {
            if (isTempThreadRunning)
                TempShowSongInfo.Abort();
            TempShowSongInfo = new Thread(() => {
                Thread.Sleep(5000);
                try
                {
                    Dispatcher.Invoke(HideInTenSecs);
                }
                catch { }
                isTempThreadRunning = false;

            });
            try
            {
                TempShowSongInfo.Start();
            }
            catch
            {
                return;
            }
            isTempThreadRunning = true;
        }
        private void BGMPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            NextSong();

        }

        private void SongProgressUpdate()
        {
            while (true)
            {

                try { Dispatcher.Invoke(() => { workingResources.SongProgress = BGMPlayer.Position.TotalSeconds / songLength * 100; }); }
                catch { }
                Thread.Sleep(1000);
            }
        }


        private void NextSong()
        {
            LastSongIndex++;
            BGMPlayer.Source = new Uri(beatmaps[LastSongIndex].GroupPath + @"\Background.mp3");
            workingResources.Nowplaying = beatmaps[LastSongIndex].Artist + " - " + beatmaps[LastSongIndex].SongName;
            SongTrans();
            BackgroundSlide();

        }
        private void SongTrans()
        {
            if (NowplayingTextPanel.Visibility == Visibility.Collapsed)
            {
                NowplayingTextPanel.Visibility = Visibility.Visible;
                ThicknessAnimation animation = new ThicknessAnimation(new Thickness(1000, 0, 0, 80), new Thickness(1000, 55, 0, 25), TimeSpan.FromSeconds(0.2), FillBehavior.HoldEnd);
                NowplayingControlPanel.BeginAnimation(MarginProperty, animation);
            }
            workingResources.NowplayingLength = (txblNowPlaying.Text.Length) * 10 + 100;
            ThicknessAnimation SlideInAnimation = new ThicknessAnimation(new Thickness(0, 0, -(workingResources.NowplayingLength) / 4, 75), new Thickness(0, 0, 0, 75), TimeSpan.FromSeconds(0.35), FillBehavior.HoldEnd);
            DoubleAnimation FadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.45), FillBehavior.HoldEnd);
            NowplayingTextPanel.BeginAnimation(MarginProperty, SlideInAnimation);
            NowplayingTextPanel.BeginAnimation(OpacityProperty, FadeInAnimation);
        }
        #endregion

        #region Account Related
        private void AccPnlGlowingOverlay(object sender, MouseEventArgs e)
        {
            DoubleAnimation FadeIn = new DoubleAnimation(0, 0.15, TimeSpan.FromSeconds(0.2), FillBehavior.HoldEnd);
            AccountPanelOverlay.BeginAnimation(OpacityProperty, FadeIn);
        }
        private void AccPnlUngrowing(object sender, MouseEventArgs e)
        {
            DoubleAnimation FadeOut = new DoubleAnimation(0.15,0, TimeSpan.FromSeconds(0.2), FillBehavior.HoldEnd);
            AccountPanelOverlay.BeginAnimation(OpacityProperty, FadeOut);
        }
        #endregion

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (currentSection == OsuSection.PreLoad)
                return;

            if (isAwaitVolumeGaugeRunning)
            {
                _volumeGauge.Abort();
                isAwaitVolumeGaugeRunning = false;
            }

            if (VolumeMixer.Visibility == Visibility.Collapsed)
            {
                VolumeMixer.Visibility = Visibility.Visible;
                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.1), FillBehavior.Stop);
                fadeIn.Completed += (object senderr, EventArgs ee) => { VolumeMixer.Opacity = 1; };
                VolumeMixer.BeginAnimation(OpacityProperty, fadeIn);
                CountDownFadeOutVolume();
                _volumeGauge.Start();
                isAwaitVolumeGaugeRunning = true;
                return;
            }

            if (e.Delta > 0)            
                workingResources.MasterVolumeValue += 5;
            else
                workingResources.MasterVolumeValue -= 5;

            CountDownFadeOutVolume();
            _volumeGauge.Start();
            isAwaitVolumeGaugeRunning = true;
        }
        private void CountDownFadeOutVolume()
        {
            try
            {
                _volumeGauge = new Thread(() =>
                {
                    Thread.Sleep(1500);

                    Dispatcher.Invoke(() =>
                    {
                        DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.1), FillBehavior.Stop);
                        fadeOut.Completed += (object senderr, EventArgs ee) =>
                        {
                            VolumeMixer.Opacity = 0;
                            VolumeMixer.Visibility = Visibility.Collapsed;
                        };
                        VolumeMixer.BeginAnimation(OpacityProperty, fadeOut);
                        isAwaitVolumeGaugeRunning = false;
                    });
                });
            }
            catch { }
        }
        private void VolmumeAnimmation()
        {
            for (int i = 0; i < workingResources.MasterVolumeValue; i++)
            {
                VolumeString.Text = i + "%";
                Thread.Sleep(20);
            }
        }

        private void imgOsuLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            imgPlayTab.Visibility = Visibility.Visible;
            Thickness slideVal = new Thickness(-105, 130, 400, 130);

            ThicknessAnimation tabPlaySlide = new ThicknessAnimation(new Thickness(450, 155, 210, 515), TimeSpan.FromSeconds(0.5));
            DoubleAnimation tabPlayFadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));

            ThicknessAnimation tabExitSlide = new ThicknessAnimation(new Thickness(450, 505, 210, 165), TimeSpan.FromSeconds(0.35));
            ThicknessAnimation slide = new ThicknessAnimation(slideVal, TimeSpan.FromSeconds(0.35));
            imgPlayTab.BeginAnimation(MarginProperty, tabPlaySlide);
            imgPlayTab.BeginAnimation(OpacityProperty, tabPlayFadeIn);
            imgOsuLogo.BeginAnimation(MarginProperty, slide);
            defaultLogoMargin = slideVal;

            imgPlayTab.MouseEnter += ImgPlayTab_MouseEnter;
            imgPlayTab.MouseLeave += ImgPlayTab_MouseLeave;
        }

        private void ImgPlayTab_MouseLeave(object sender, MouseEventArgs e)
        {
            imgPlayTab.Margin = new Thickness(490, 155, 170, 515);
            ThicknessAnimation animation = new ThicknessAnimation(new Thickness(450, 155, 210, 515), TimeSpan.FromSeconds(0.075));
            animation.Completed += (object senderr, EventArgs ee) => { workingResources.PlayTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\PlayTab.png"; };
            imgPlayTab.BeginAnimation(MarginProperty, animation);
        }

        private void ImgPlayTab_MouseEnter(object sender, MouseEventArgs e)
        {
            imgPlayTab.Margin = new Thickness(450, 155, 210, 515);
            workingResources.PlayTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\SelectedPlayTab.png";
            ThicknessAnimation animation = new ThicknessAnimation(new Thickness(490, 155, 170, 515), TimeSpan.FromSeconds(0.075));
            imgPlayTab.BeginAnimation(MarginProperty, animation);
        }
    }
}
