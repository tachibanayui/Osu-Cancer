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
using System.Net.Sockets;
using System.Net;
using System.Windows.Controls.Primitives;

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
        OsuCookieBehaviour osuCookieBehaviour;
        bool isPlayClickFirstTime = true;
        bool isMainMenuClickFirstTime = true;
        Thread bouncingOsu;
        ManualResetEvent wait = new ManualResetEvent(false);
        bool isLogoBusy;
        bool IsLogoBusy
        {
            get { return isLogoBusy; }
            set
            {
                if (value == true)
                {
                    wait.Reset();
                }
                else
                {
                    wait.Set();
                }
            }
        }
        bool isOsuLogoMoving;
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
            osuCookieBehaviour = OsuCookieBehaviour.ClickToOpenTab;
            ChangeAllSettingIconUnlit(SettingIcon1);
            SettingIcon1.Opacity = 1;
            OsuBouncingController(240);
            grdBackLightContainer.Visibility = Visibility.Visible;

            //Add default margin to SelectionTab
            imgPlayTab.Tag = new EndAnimationPos(new Thickness(450, 155, 210, 515), new Thickness(490, 155, 170, 515));
            imgExitTab.Tag = new EndAnimationPos(new Thickness(450, 505, 210, 165), new Thickness(490, 505, 170, 165));
            imgEditTab.Tag = new EndAnimationPos(new Thickness(430, 275, 130, 395), new Thickness(470, 275, 130, 395));
            imgOptionTab.Tag = new EndAnimationPos(new Thickness(440, 385, 110, 285), new Thickness(470, 385, 130, 285));
            imgSoloTab.Tag = new EndAnimationPos(new Thickness(460, 225, 210, 447), new Thickness(500, 225, 170, 447));
            imgMultiTab.Tag = new EndAnimationPos(new Thickness(460, 335, 200, 335), new Thickness(500, 335, 160, 335));
            imgBackTab.Tag = new EndAnimationPos(new Thickness(460, 440, 200, 230), new Thickness(500, 440, 160, 230));
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
            if (isMessageOverlayHideIntervalActive)
                messageOverlayHideInterval.Abort();

            HideOverlayMessage();
            OverlayMessage.Visibility = Visibility.Visible;
            txblOverlayMessage.Text = text;
            OverlayMessage.Opacity = 1;
            DoubleAnimation expandAnimation = new DoubleAnimation(0, 50, TimeSpan.FromSeconds(0.1), FillBehavior.HoldEnd);
            OverlayMessage.BeginAnimation(HeightProperty, expandAnimation);
            messageOverlayHideInterval = new Thread(() =>
            {
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
        private void OpenSettingPanel()
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
            if (currentSection != OsuSection.MainScreen || isOsuLogoMoving)
                return;
            isLogoBusy = true;
            ThicknessAnimation animation = new ThicknessAnimation(new Thickness(defaultLogoMargin.Left - 10, defaultLogoMargin.Top - 10, defaultLogoMargin.Right - 10, defaultLogoMargin.Bottom - 10), TimeSpan.FromSeconds(0.1), FillBehavior.Stop);
            animation.Completed += (s, a) => {
                imgOsuLogo.Margin = new Thickness(defaultLogoMargin.Left - 10, defaultLogoMargin.Top - 10, defaultLogoMargin.Right - 10, defaultLogoMargin.Bottom - 10);
                IsLogoBusy = false;
            };
            imgOsuLogo.BeginAnimation(MarginProperty, animation);
        }
        private void imgOsuLogo_MouseLeave(object sender, MouseEventArgs e)
        {
            if (currentSection != OsuSection.MainScreen || isOsuLogoMoving)
                return;
            IsLogoBusy = true;
            ThicknessAnimation animation = new ThicknessAnimation(defaultLogoMargin, TimeSpan.FromSeconds(0.1), FillBehavior.Stop);
            animation.Completed += (s, a) =>
            {
                imgOsuLogo.Margin = defaultLogoMargin;
                IsLogoBusy = false;
            };
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
        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (currentSection == OsuSection.PreLoad || workingResources.IsSettingPanelOpen)
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
            {
                if(EffectVolumeGauge.IsMouseOver)
                    workingResources.EffectVolumeValue += 5;
                else if(MusicVolumeGauge.IsMouseOver)
                    workingResources.MusicVolumeValue += 5;
                else
                    workingResources.MasterVolumeValue += 5;
            }
            else
            {
                if (EffectVolumeGauge.IsMouseOver)
                    workingResources.EffectVolumeValue -= 5;
                else if (MusicVolumeGauge.IsMouseOver)
                    workingResources.MusicVolumeValue -= 5;
                else
                    workingResources.MasterVolumeValue -= 5;
            }

            CountDownFadeOutVolume();
            _volumeGauge.Start();
            isAwaitVolumeGaugeRunning = true;
        }

        private bool IsEffectMusicVolumeMouseWheel()
        {
            if (EffectVolumeGauge.IsMouseOver || MusicVolumeGauge.IsMouseOver)
                return true;
            else
                return false;
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

        private void imgOsuLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (osuCookieBehaviour == OsuCookieBehaviour.ClickToOpenTab && currentSection == OsuSection.MainScreen)
            {
                IsLogoBusy = true;
                isOsuLogoMoving = true;
                    
                Thickness slideVal = new Thickness(-105, 130, 400, 130);

                ThicknessAnimation slide = new ThicknessAnimation(slideVal, TimeSpan.FromSeconds(0.35));
                slide.Completed += (s, ee) => 
                {
                    isOsuLogoMoving = false;
                    IsLogoBusy = false;
                    imgOsuLogo_MouseEnter(this, e);
                };
                TabSlider();

                imgOsuLogo.BeginAnimation(MarginProperty, slide);
                defaultLogoMargin = slideVal;
                osuCookieBehaviour = OsuCookieBehaviour.ClickToAutoSelectPlay;
            }
        }
        private void TabSlider()
        {
            imgPlayTab.Visibility = Visibility.Visible;
            imgExitTab.Visibility = Visibility.Visible;
            imgEditTab.Visibility = Visibility.Visible;
            imgOptionTab.Visibility = Visibility.Visible;

            ThicknessAnimation tabPlaySlide = new ThicknessAnimation(new Thickness(450, 155, 210, 515), TimeSpan.FromSeconds(0.5));
            ThicknessAnimation tabExitSlide = new ThicknessAnimation(new Thickness(450, 505, 210, 165), TimeSpan.FromSeconds(0.5));
            ThicknessAnimation tabEditSlide = new ThicknessAnimation(new Thickness(430, 275, 130, 395), TimeSpan.FromSeconds(0.5));
            ThicknessAnimation tabOptionSlide = new ThicknessAnimation(new Thickness(440, 385, 110, 285), TimeSpan.FromSeconds(0.5));
            DoubleAnimation tabFadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5));

            imgPlayTab.BeginAnimation(MarginProperty, null);
            imgEditTab.BeginAnimation(MarginProperty, null);
            imgOptionTab.BeginAnimation(MarginProperty, null);
            imgExitTab.BeginAnimation(MarginProperty, null);

            imgPlayTab.BeginAnimation(MarginProperty, tabPlaySlide);
            imgPlayTab.BeginAnimation(OpacityProperty, tabFadeIn);
            imgExitTab.BeginAnimation(MarginProperty, tabExitSlide);
            imgExitTab.BeginAnimation(OpacityProperty, tabFadeIn);
            imgEditTab.BeginAnimation(MarginProperty, tabEditSlide);
            imgEditTab.BeginAnimation(OpacityProperty, tabFadeIn);
            imgOptionTab.BeginAnimation(MarginProperty, tabOptionSlide);
            imgOptionTab.BeginAnimation(OpacityProperty, tabFadeIn);

            if (isMainMenuClickFirstTime)
            {
                imgPlayTab.MouseEnter += ImgPlayTab_MouseEnter;
                imgPlayTab.MouseLeave += ImgPlayTab_MouseLeave;
                imgExitTab.MouseEnter += ImgPlayTab_MouseEnter;
                imgExitTab.MouseLeave += ImgPlayTab_MouseLeave;
                imgEditTab.MouseEnter += ImgPlayTab_MouseEnter;
                imgEditTab.MouseLeave += ImgPlayTab_MouseLeave;
                imgOptionTab.MouseEnter += ImgPlayTab_MouseEnter;
                imgOptionTab.MouseLeave += ImgPlayTab_MouseLeave;

                imgPlayTab.MouseDown += ImgPlayTab_MouseDown;
                imgOptionTab.MouseDown += (object sender, MouseButtonEventArgs e) => { OpenSettingPanel(); };
                imgExitTab.MouseDown += (object sender, MouseButtonEventArgs e) =>
                {
                    media.Source = new Uri(workingResources.BaseDir + @"Resources\Default Audio\Goodbye.wav");
                    windowCurtain.Visibility = Visibility.Visible;
                    DoubleAnimation doubleAnimation = new DoubleAnimation(1, TimeSpan.FromSeconds(1.25));
                    doubleAnimation.Completed += (object sendeer, EventArgs ee) => { Environment.Exit(0); };
                    media.Play();
                    windowCurtain.BeginAnimation(OpacityProperty, doubleAnimation);
                };
                imgEditTab.MouseDown += (object sender, MouseButtonEventArgs e) => { ShowOverlayMessage("Not avaiable! Please try later version"); };
                isMainMenuClickFirstTime = false;
            }
        }
        private void ImgPlayTab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            ThicknessAnimation slideOut = new ThicknessAnimation(((EndAnimationPos)img.Tag).Activated, TimeSpan.FromSeconds(0.15));
            DoubleAnimation fadeOut = new DoubleAnimation(0, TimeSpan.FromSeconds(0.15));
            fadeOut.Completed += (object senderr, EventArgs ee) =>
            {

                imgPlayTab.Margin = new Thickness(250, 155, 410, 515);
                imgEditTab.Margin = new Thickness(230, 275, 330, 395);
                imgOptionTab.Margin = new Thickness(240, 385, 310, 285);
                imgExitTab.Margin = new Thickness(250, 505, 410, 165);
            };

            img.BeginAnimation(MarginProperty, slideOut);
            img.BeginAnimation(OpacityProperty, fadeOut);

            if (img.Name == "imgPlayTab")
            {
                fadeOut.Completed += (object sendder, EventArgs ee) =>
                {
                    imgEditTab.Visibility = Visibility.Collapsed;
                    imgPlayTab.Visibility = Visibility.Collapsed;
                    imgOptionTab.Visibility = Visibility.Collapsed;
                    imgExitTab.Visibility = Visibility.Collapsed;
                };
                imgEditTab.BeginAnimation(OpacityProperty, fadeOut);
                imgOptionTab.BeginAnimation(OpacityProperty, fadeOut);
                imgExitTab.BeginAnimation(OpacityProperty, fadeOut);
                PlayModeOpen();
            }
            else
            {
                fadeOut.Completed += (object sendder, EventArgs ee) =>
                {
                    imgSoloTab.Visibility = Visibility.Collapsed;
                    imgMultiTab.Visibility = Visibility.Collapsed;
                    imgBackTab.Visibility = Visibility.Collapsed;
                };
                imgSoloTab.BeginAnimation(OpacityProperty, fadeOut);
                imgMultiTab.BeginAnimation(OpacityProperty, fadeOut);
                TabSlider();
            }

        }
        private void PlayModeOpen()
        {
            imgSoloTab.Visibility = Visibility.Visible;
            imgMultiTab.Visibility = Visibility.Visible;
            imgBackTab.Visibility = Visibility.Visible;

            ThicknessAnimation sildeInSoloTab = new ThicknessAnimation(new Thickness(460, 225, 210, 447), TimeSpan.FromSeconds(0.5));
            ThicknessAnimation sildeInMultiTab = new ThicknessAnimation(new Thickness(460, 335, 200, 335), TimeSpan.FromSeconds(0.5));
            ThicknessAnimation sildeInBackTab = new ThicknessAnimation(new Thickness(460, 440, 200, 230), TimeSpan.FromSeconds(0.5));
            DoubleAnimation fadeInTab = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));

            imgSoloTab.BeginAnimation(MarginProperty, sildeInSoloTab);
            imgSoloTab.BeginAnimation(OpacityProperty, fadeInTab);
            imgMultiTab.BeginAnimation(MarginProperty, sildeInMultiTab);
            imgMultiTab.BeginAnimation(OpacityProperty, fadeInTab);
            imgBackTab.BeginAnimation(MarginProperty, sildeInBackTab);
            imgBackTab.BeginAnimation(OpacityProperty, fadeInTab);

            if (isPlayClickFirstTime)
            {
                imgSoloTab.MouseEnter += ImgPlayTab_MouseEnter;
                imgSoloTab.MouseLeave += ImgPlayTab_MouseLeave;
                imgMultiTab.MouseEnter += ImgPlayTab_MouseEnter;
                imgMultiTab.MouseLeave += ImgPlayTab_MouseLeave;
                imgBackTab.MouseEnter += ImgPlayTab_MouseEnter;
                imgBackTab.MouseLeave += ImgPlayTab_MouseLeave;

                imgMultiTab.MouseDown += (object senders, MouseButtonEventArgs ee) => { ShowOverlayMessage("Multiplayer is not avaible in this version! Try again in the next future"); };
                imgBackTab.MouseDown += ImgPlayTab_MouseDown;
                isPlayClickFirstTime = false;
            }
        }
        private void ImgPlayTab_MouseLeave(object sender, MouseEventArgs e)
        {
            if (currentSection != OsuSection.MainScreen)
                return;

            Image img = sender as Image;

            img.BeginAnimation(MarginProperty, null);
            img.Margin = ((EndAnimationPos)img.Tag).NonActivated;
            ThicknessAnimation animation = new ThicknessAnimation(((EndAnimationPos)img.Tag).Activated, TimeSpan.FromSeconds(0.075));
            animation.Completed += (object senderr, EventArgs ee) =>
            {
                ChangeTabImage(img, false);
            };
            img.BeginAnimation(MarginProperty, animation);
        }
        private void ChangeTabImage(Image img, bool isActivate)
        {
            switch (img.Name)
            {
                case "imgPlayTab":
                    if (isActivate)
                        workingResources.PlayTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\SelectedPlayTab.png";
                    else
                        workingResources.PlayTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\PlayTab.png";
                    break;
                case "imgExitTab":
                    if (isActivate)
                        workingResources.ExitTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\SelectedExitTab.png";
                    else
                        workingResources.ExitTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\ExitTab.png";
                    break;
                case "imgEditTab":
                    if (isActivate)
                        workingResources.EditTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\SelectedEditTab.png";
                    else
                        workingResources.EditTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\EditTab.png";
                    break;
                case "imgOptionTab":
                    if (isActivate)
                        workingResources.OptionTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\SelectedOptionTab.png";
                    else
                        workingResources.OptionTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\OptionTab.png";
                    break;
                case "imgSoloTab":
                    if (isActivate)
                        workingResources.SoloTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\SelectedSoloTab.png";
                    else
                        workingResources.SoloTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\SoloTab.png";
                    break;
                case "imgMultiTab":
                    if (isActivate)
                        workingResources.MultiTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\SelectedMultiTab.png";
                    else
                        workingResources.MultiTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\MultiTab.png";
                    break;
                case "imgBackTab":
                    if (isActivate)
                        workingResources.BackTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\SelectedBackTab.png";
                    else
                        workingResources.BackTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\BackTab.png";
                    break;
                default:
                    break;
            }
        }
        private void ImgPlayTab_MouseEnter(object sender, MouseEventArgs e)
        {
            if (currentSection != OsuSection.MainScreen)
                return;

            Image img = (Image)sender;
            img.BeginAnimation(MarginProperty, null);

            img.Margin = ((EndAnimationPos)img.Tag).Activated;

            ChangeTabImage(img, true);
            ThicknessAnimation animation = new ThicknessAnimation(((EndAnimationPos)img.Tag).NonActivated, TimeSpan.FromSeconds(0.075));
            animation.Completed += (object a, EventArgs w) => { ChangeTabImage(img, true); };
            img.BeginAnimation(MarginProperty, animation);
        }
        private void scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UIElement choosenElement = new UIElement();
            UIElement choosenRectangle = new UIElement();
            switch (scroller.VerticalOffset)
            {
                case Object a when (scroller.VerticalOffset < 384):
                    LitSettingIcon(SettingIcon1);
                    choosenElement = SettingIcon1;
                    break;
                case Object a when (scroller.VerticalOffset > 383 && scroller.VerticalOffset < 1248):
                    LitSettingIcon(SettingIcon2);
                    choosenElement = SettingIcon2;
                    break;
                case Object a when (scroller.VerticalOffset > 1247 && scroller.VerticalOffset < 1584):
                    LitSettingIcon(SettingIcon3);
                    choosenElement = SettingIcon3;
                    break;
                case Object a when (scroller.VerticalOffset > 1583 && scroller.VerticalOffset < 1920):
                    LitSettingIcon(SettingIcon4);
                    choosenElement = SettingIcon4;
                    break;
                case Object a when (scroller.VerticalOffset > 1919 && scroller.VerticalOffset < 2261):
                    LitSettingIcon(SettingIcon5);
                    choosenElement = SettingIcon5;
                    break;
                case Object a when (scroller.VerticalOffset > 2140):
                    LitSettingIcon(SettingIcon6);
                    choosenElement = SettingIcon6;
                    break;
            }
            ChangeAllSettingIconUnlit(choosenElement);
        }
        private void LitSettingIcon(Grid settingIcon1)
        {
            settingIcon1.Opacity = 1;
            Rectangle rec = (Rectangle)settingIcon1.Children[0];
            rec.Opacity = 1;
        }
        private void ChangeAllSettingIconUnlit(UIElement oddOut)
        {
            AddFadeOutAnim(SettingIcon1, oddOut, 0.25, 0.8);
            AddFadeOutAnim(SettingIcon2, oddOut, 0.25, 0.8);
            AddFadeOutAnim(SettingIcon3, oddOut, 0.25, 0.8);
            AddFadeOutAnim(SettingIcon4, oddOut, 0.25, 0.8);
            AddFadeOutAnim(SettingIcon5, oddOut, 0.25, 0.8);
            AddFadeOutAnim(SettingIcon6, oddOut, 0.25, 0.8);
            AddFadeOutAnim(SettingIcon7, oddOut, 0.25, 0.8);
            AddFadeOutAnim(SettingIcon8, oddOut, 0.25, 0.8);
            AddFadeOutAnim(SettingIcon9, oddOut, 0.25, 0.8);
        }
        private void AddFadeOutAnim(UIElement target, UIElement oddOut, double interval, double fadeVal)
        {
            if (target == oddOut || target.Opacity == fadeVal)
                return;

            target.BeginAnimation(OpacityProperty, null);
            DoubleAnimation fadeouAnimation = new DoubleAnimation(fadeVal, TimeSpan.FromSeconds(interval), FillBehavior.Stop);
            fadeouAnimation.Completed += (s, e) =>
            {
                target.Opacity = fadeVal;
                Grid targetGrid = (Grid)target;
                Rectangle rec = (Rectangle)targetGrid.Children[0];
                rec.Opacity = 0;
            };
            target.BeginAnimation(OpacityProperty, fadeouAnimation);
        }
        private void Icon_ClickJumpToSetting(object sender, MouseButtonEventArgs e)
        {
            Grid target = (Grid)sender;

            int nameID = int.Parse(target.Name.Substring(11));
            switch (nameID)
            {
                case 1:
                    ScrollAnimation(0);
                    break;
                case 2:
                    ScrollAnimation(490);
                    break;
                case 3:
                    ScrollAnimation(1290);
                    break;
                case 4:
                    ScrollAnimation(1630);
                    break;
                case 5:
                    ScrollAnimation(1980);
                    break;
                case 6:
                    ScrollAnimation(2370);
                    break;
                default:
                    break;
            }
        }
        private void ScrollAnimation(int scrollVal)
        {
            Thread worker;

            int value = 50;
            int scrollval = 0;
            Dispatcher.Invoke(() => { scrollval = (scrollVal - (int)scroller.VerticalOffset) / 50; });

            if (scrollval < 0)
            {
                value = -50;
                scrollval = Math.Abs(scrollval);
            }


            worker = new Thread(() =>
            {
                for (int i = 0; i < scrollval; i++)
                {
                    Dispatcher.Invoke(() => { scroller.ScrollToVerticalOffset(scroller.VerticalOffset + value); });
                    Thread.Sleep(10);
                }
                return;
            });

            worker.Start();

        }
        private void OsuBouncingController(int bpm)
        {
            bouncingOsu = new Thread(BouncingLogo);
            bouncingOsu.Start(bpm);
        }

        private void ChangeSongBPMRelatedEffect(int bpm)
        {
            bouncingOsu.Abort();
            OsuBouncingController(bpm);
        }

        private void BouncingLogo(object objBpm)
        {
            int bpm = (int)objBpm;
            while (true)
            {
                Thickness baseElementMargin = new Thickness();
                Dispatcher.Invoke(() => { baseElementMargin = imgOsuLogo.Margin; });
                int step = 60000 / (bpm * 2);
                bool isIncrease = true;

                for (int i = 0; i <= 10; i++)
                {
                    Dispatcher.Invoke(() => { imgOsuLogo.Margin = new Thickness(baseElementMargin.Left + i, baseElementMargin.Top + i, baseElementMargin.Right + i, baseElementMargin.Bottom + i); });
                    Thread.Sleep(step / 10);
                    if (isIncrease)
                    {
                        if (i == 3)
                            Dispatcher.Invoke(() => { BackLight.Opacity = 0.3; });
                        else if (i == 6)
                            Dispatcher.Invoke(() => { BackLight.Opacity = 0.4; });
                        else if (i == 10)
                            Dispatcher.Invoke(() => { BackLight.Opacity = 0.5; });
                    }
                    else
                    {
                        if (i == 3)
                            Dispatcher.Invoke(() => { BackLight.Opacity = 0.4; });
                        else if (i == 6)
                            Dispatcher.Invoke(() => { BackLight.Opacity = 0.3; });
                        else if (i == 10)
                            Dispatcher.Invoke(() => { BackLight.Opacity = 0; });
                    }

                    if (IsLogoBusy)
                        wait.WaitOne();
                }

                Dispatcher.Invoke(() => { imgOsuLogo.Margin = baseElementMargin; });
                Thread.Sleep(step);
                isIncrease = !isIncrease;

            }
        }
        #endregion

        #region Setting Related
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenSettingPanel();
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
            workingResources.Nowplaying = beatmaps[LastSongIndex].Artist + " - " + beatmaps[LastSongIndex].SongName;
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
            TempShowSongInfo = new Thread(() =>
            {
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
            ChnageBPM();

        }
        private void ChnageBPM()
        {
            ChangeSongBPMRelatedEffect(beatmaps[lastSongIndex].BPM);
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
            DoubleAnimation FadeOut = new DoubleAnimation(0.15, 0, TimeSpan.FromSeconds(0.2), FillBehavior.HoldEnd);
            AccountPanelOverlay.BeginAnimation(OpacityProperty, FadeOut);
        }
        #endregion

        #region Server Comunication Related
        private void btnConnectEP(object sender, RoutedEventArgs e)
        {
            string btnContent = (string)((Button)sender).Content;
            if (btnContent == "Connected!" || btnContent == "Connecting...")
            {
                return;
            }

            IPEndPoint serverEP;
            IPAddress serverIP;
            try
            {
                string strIPAdress = txbEndPoint.Text.Split(':')[0];
                int port = int.Parse(txbEndPoint.Text.Split(':')[1]);
                serverIP = IPAddress.Parse(strIPAdress);
                serverEP = new IPEndPoint(serverIP, port);
            }
            catch
            {
                ShowOverlayMessage("Can't connect to server, Please check your information!");
                return;
            }

            workingResources.CurrentSocket = new Socket(serverIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            SocketOpreation socketOpration = new SocketOpreation(1024);
            socketOpration.Socket = workingResources.CurrentSocket;
            socketOpration.State = sender;
            workingResources.CurrentSocket.BeginConnect(serverEP, ConnectCallBack, socketOpration);
            ((Button)sender).Content = "Connecting...";
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            SocketOpreation socketOpreation = (SocketOpreation)ar.AsyncState;
            Socket socket = socketOpreation.Socket;
            Button btn = (Button)socketOpreation.State;

            Dispatcher.Invoke(() => { btn.Content = "Connected!"; });
            try { socket.EndConnect(ar); }
            catch { return; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (workingResources.CurrentSocket == null)
            {
                ShowOverlayMessage("You not connect to any server! Online Feature not work without server!");
                return;
            }
            byte[] data = Encoding.Default.GetBytes($"{txbUsername.Text}:{txbPassword.Text}");
            workingResources.CurrentSocket.BeginSend(data, 0, data.Length, 0, LoginCallBack, workingResources.CurrentSocket);

        }

        private void LoginCallBack(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;
            socket.EndSend(ar);
            byte[] buffer = new byte[1024];
            int receiveSize = 0;
            try { receiveSize = socket.Receive(buffer); }
            catch (Exception e)
            {
                FileOperation.ExceptionHandle(e, "Server Disconnected!");
            }
            string receivedString = Encoding.Default.GetString(buffer, 0, receiveSize);
            if (receivedString == "WrongPassword" || receivedString == "WrongUsername")
            {
                Dispatcher.Invoke(() => { ShowOverlayMessage("Failed to Login:" + receivedString); });
                return;
            }

            FileOperation.ByteArraytoFile(workingResources.BaseDir + @"Resources\UserInfo.cfg", buffer, receiveSize);
            workingResources.ReloadInformation();
        }



        #endregion

       
    }
}