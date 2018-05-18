using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Osu_Cancer
{
    /// <summary>
    /// Class Contain some commonly use. Resource input as a parent folder of the resource folder
    /// </summary>
    class Cons: INotifyPropertyChanged
    {
        /// <summary>
        /// Use the default application path to access resource
        /// </summary>
        public Cons()
        {
            BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            SetProperty();
        }
        /// <summary>
        /// Use a custom path to access resources
        /// </summary>
        /// <param name="defaultPath">A reources path</param>
        public Cons(string defaultPath)
        {
            BaseDir = defaultPath;
            SetProperty();
        }

        private void SetProperty()
        {
            Logo = BaseDir + @"Resources\Logo.png";
            NewUpdateandBuildDir = BaseDir + @"Resources\BuildNotes.txt";
            NewUpdateContent = FileOperation.FileToString(NewUpdateandBuildDir, EncodingType.UTF8);
            SongPath = BaseDir + @"Resources\Songs\";
            SkinsParentpath = BaseDir + @"Resources\Skins\";
            settingIcon = new List<BitmapImage>();
            IsSettingPanelOpen = false;
            GetAccountInformation();
            ScreenX = int.Parse(System.Windows.SystemParameters.PrimaryScreenWidth.ToString());
            ScreenY = int.Parse(System.Windows.SystemParameters.PrimaryScreenHeight.ToString());
            IsBGMPause = false;
            NowplayingLength = 366;
            isShowPernamentlyInfo = true;
            MasterVolumeValue = 99;
            SongProgress = 50;
            PlayTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\PlayTab.png";
            ExitTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\ExitTab.png";
            EditTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\EditTab.png";
            OptionTab = @"F:\All Project\Osu!Cancer\Osu!Cancer\bin\Debug\Resources\Icon\OptionTab.png";
        }

        private void GetAccountInformation()
        {
            Username = FileOperation.GetSettingValueFromFile(BaseDir + @"\Resources\UserInfo.cfg", "Username");
            try { ProgLv = int.Parse(FileOperation.GetSettingValueFromFile(BaseDir + @"\Resources\UserInfo.cfg", "EXP")); }
            catch { ProgLv = 0; }

            Accuracy = FileOperation.GetSettingValueFromFile(BaseDir + @"\Resources\UserInfo.cfg", "Accuracy");
            Lv = FileOperation.GetSettingValueFromFile(BaseDir + @"\Resources\UserInfo.cfg", "LV");
            Pp = FileOperation.GetSettingValueFromFile(BaseDir + @"\Resources\UserInfo.cfg", "PP");
            Rank = FileOperation.GetSettingValueFromFile(BaseDir + @"\Resources\UserInfo.cfg", "Rank");
            Nowplaying = "Nekodex - Circle";
        }

        public string BaseDir { get; set; }
        private string logo;
        public string Logo {
            get { return logo; }
            set
            {
                if(logo != value)
                {
                    logo = value;
                    NotifyPropertyChanged("Logo");
                }
            }
        }
        private string playTab;
        public string PlayTab {
            get { return playTab; }
            set
            {

                if(playTab != value)
                {
                    playTab = value;
                    NotifyPropertyChanged("PlayTab");
                }
            }
        }
        private string exitTab;
        public string ExitTab
        {
            get { return exitTab; }
            set
            {
                if (exitTab != value)
                {
                    exitTab = value;
                    NotifyPropertyChanged("ExitTab");
                }
            }
        }
        private string editTab;
        public string EditTab
        {
            get { return editTab; }
            set
            {
                if(editTab != value)
                {
                    editTab = value;
                    NotifyPropertyChanged("EditTab");
                }
            }
        }
        private string optionTab;
        public string OptionTab
        {
            get { return optionTab;}
            set
            {
                if(optionTab != value)
                {
                    optionTab = value;
                    NotifyPropertyChanged("OptionTab");
                }
            }
        }
        public string NewUpdateandBuildDir { get; set; }
        public string NewUpdateContent { get; set; }
        public string SongPath { get; set; }
        public string SkinsParentpath { get; set; }
        public SkinsLib CurrentSkin { get; set; }
        public List<BitmapImage> settingIcon { get; set; }
        public int ScreenX { get; set; }
        public int ScreenY { get; set; }
        public bool IsSettingPanelOpen { get; set; }
        public bool IsBGMPause { get; set; }
        public string Username { get; set;}
        public string Accuracy { get; set; }
        public string Lv { get; set; }
        public string Pp { get; set; }
        public int ProgLv { get; set; }
        public string Rank { get; set; }
        public string Nowplaying
        {
            get { return nowplaying; }
            set
            {
                if (value != nowplaying)
                {
                    nowplaying = value;
                    NotifyPropertyChanged("Nowplaying");
                }
            }
        }
        private string nowplaying;
        public int NowplayingLength
        {
            get { return nowplayingLength; }
            set
            {
                if(nowplayingLength != value)
                {
                    if(value > 365)
                    {
                        nowplayingLength = value;                       
                    }
                    else
                    {
                        nowplayingLength = 366;
                    }
                    NotifyPropertyChanged("NowplayingLength");
                }
            }
        }
        private int nowplayingLength;
        public bool isShowPernamentlyInfo { get; set; }
        private double songProgress;
        public double SongProgress
        {
            get { return songProgress; }
            set
            {
                if(songProgress != value)
                {
                    songProgress = value * 3;
                    NotifyPropertyChanged("SongProgress");
                }
            }
        }
        #region Volume
        private double _BGMVolume;
        public double BGMVolume
        {
            get { return _BGMVolume; }
            set
            {
                if(_BGMVolume != value)
                {
                    _BGMVolume = value;
                    NotifyPropertyChanged("BGMVolume");
                }
            }
        }
        public double MasterVolumeValue
        {
            get { return masterVolumeValue; }
            set
            {
                if(masterVolumeValue != value && value > 0 && value < 100)
                {
                    masterVolumeValue = value;
                    MasterVolumePathVal = FileOperation.CreateArc(61.5, masterVolumeValue * 3.6);
                    MasterVolumeStringIndicator = masterVolumeValue + "%";
                    BGMVolume = masterVolumeValue / 100;
                }
            }
        }
        private double masterVolumeValue;
        public Geometry MasterVolumePathVal
        {
            get { return masterVolumePathVal; }
            set
            {
                if (masterVolumePathVal != value)
                {
                    masterVolumePathVal = value;
                    NotifyPropertyChanged("MasterVolumePathVal");
                }
            }
        }
        private Geometry masterVolumePathVal;
        public string MasterVolumeStringIndicator
        {
            get { return masterVolumeStringIndicator; }
            set
            {
                if (masterVolumeStringIndicator != value)
                {
                    masterVolumeStringIndicator = value;
                    NotifyPropertyChanged("MasterVolumeStringIndicator");
                }
            }
        }



        private string masterVolumeStringIndicator;
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
    public class GameIcon
    {
        public GameIcon(string baseDir)
        {
            BaseDir = baseDir + @"Resources\Icon\";
            Information = BaseDir + "Information.png";
            MusicNote = BaseDir + "MusicNote.png";
            Next = BaseDir + "Next.png";
            Stop = BaseDir + "Stop.png";
            Play = BaseDir + "Play.png";
            Pause = BaseDir + "Pause.png";
            Window = BaseDir + "Window.png";
            Previous = BaseDir + "Previous.png";
            SettingBrushLit = BaseDir + "SettingBrushLit.png";
            SettingBrushUnlit = BaseDir + "SettingBrushUnlit.png";
            SettingCircleLit = BaseDir + "SettingCircleLit.png";
            SettingCircleUnlit = BaseDir + "SettingCircleUnlit.png";
            SettingConsoleLit = BaseDir + "SettingConsoleLit.png";
            SettingConsoleUnlit = BaseDir + "SettingConsoleUnlit.png";
            SettingDisplayLit = BaseDir + "SettingDisplayLit.png";
            SettingDisplayUnlit = BaseDir + "SettingDisplayUnlit.png";
            SettingGearLit = BaseDir + "SettingGearLit.png";
            SettingGearUnlit = BaseDir + "SettingGearUnlit.png";
            SettingPencilLit = BaseDir + "SettingPencilLit.png";
            SettingPencilUnlit = BaseDir + "SettingPencilUnlit.png";
            SettingSoundLit = BaseDir + "SettingSoundLit.png";
            SettingSoundUnlit = BaseDir + "SettingSoundUnlit.png";
            SettingWorldLit = BaseDir + "SettingWorldLit.png";
            SettingWorldUnlit = BaseDir + "SettingWorldUnlit.png";
            SettingWrenchLit = BaseDir + "SettingWrenchLit.png";
            SettingWrenchUnlit = BaseDir + "SettingWrenchUnlit.png";
        }
        public string BaseDir { get; set; }
        public string Previous { get; set; }
        public string Information { get; set; }
        public string MusicNote { get; set; }
        public string Next { get; set; }
        public string Pause { get; set; }
        public string Play { get; set; }
        public string Stop { get; set; }
        public string Window { get; set; }
        public string SettingBrushLit { get; set; }
        public string SettingBrushUnlit { get; set; }
        public string SettingCircleLit { get; set; }
        public string SettingCircleUnlit { get; set; }
        public string SettingConsoleLit { get; set; }
        public string SettingConsoleUnlit { get; set; }
        public string SettingDisplayLit { get; set; }
        public string SettingDisplayUnlit { get; set; }
        public string SettingGearLit { get; set; }
        public string SettingGearUnlit { get; set; }
        public string SettingPencilLit { get; set; }
        public string SettingPencilUnlit { get; set; }
        public string SettingSoundLit { get; set; }
        public string SettingSoundUnlit { get; set; }
        public string SettingWorldLit { get; set; }
        public string SettingWorldUnlit { get; set; }
        public string SettingWrenchLit { get; set; }
        public string SettingWrenchUnlit { get; set; }
    }

    public enum OsuSection
    {
        PreLoad,
        FadeInPreLoad,
        MainScreen,
    }
    public class EndAnimationPos
    {
        public Thickness Activated;
        public Thickness NonActivated;
        public EndAnimationPos(Thickness activated, Thickness nonActivated)
        {
            Activated = activated;
            NonActivated = nonActivated;
        }
    }
}
