using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Osu_Cancer.CustomControls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Osu_Cancer.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Osu_Cancer.CustomControls;assembly=Osu_Cancer.CustomControls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:GroupSelectTab/>
    ///
    /// </summary>
    public class GroupSelectTab : Control
    {
        static GroupSelectTab()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupSelectTab), new FrameworkPropertyMetadata(typeof(GroupSelectTab)));
        }
        #region DependencyProperty
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set
            {
                SetValue(ImageSourceProperty, value);
                AutoFillName();
            }
        }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(GroupSelectTab), new PropertyMetadata());

        public ImageSource IconImageSource
        {
            get { return (ImageSource)GetValue(IconImageSourceProperty); }
            set
            {
                SetValue(IconImageSourceProperty, value);
                AutoFillName();
            }
        }
        public static readonly DependencyProperty IconImageSourceProperty =
            DependencyProperty.Register("IconImageSource", typeof(ImageSource), typeof(GroupSelectTab), new PropertyMetadata());


        public ImageSource RankImageSource
        {
            get { return (ImageSource)GetValue(RankImageSourceProperty); }
            set { SetValue(RankImageSourceProperty, value); }
        }

        public static readonly DependencyProperty RankImageSourceProperty =
            DependencyProperty.Register("RankImageSource", typeof(ImageSource), typeof(GroupSelectTab), new PropertyMetadata());

        public string SongTitle
        {
            get { return (string)GetValue(SongTitleProperty); }
            set { SetValue(SongTitleProperty, value); }
        }
        public static readonly DependencyProperty SongTitleProperty =
            DependencyProperty.Register("SongTitle", typeof(string), typeof(GroupSelectTab), new PropertyMetadata("SongTitle"));

        public string DifficultyName
        {
            get { return (string)GetValue(DifficultyNameProperty); }
            set { SetValue(DifficultyNameProperty, value); }
        }
        public static readonly DependencyProperty DifficultyNameProperty =
            DependencyProperty.Register("DifficultyName", typeof(string), typeof(GroupSelectTab), new PropertyMetadata("Diff name"));

        public string DifficultyStar
        {
            get { return (string)GetValue(DifficultyStarProperty); }
            set { SetValue(DifficultyStarProperty, value); }
        }
        public static readonly DependencyProperty DifficultyStarProperty =
            DependencyProperty.Register("DifficultyStar", typeof(string), typeof(GroupSelectTab), new PropertyMetadata(5.ToString()));

        public string SongAuthor
        {
            get { return (string)GetValue(SongAuthorProperty); }
            set { SetValue(SongAuthorProperty, value); }
        }
        public static readonly DependencyProperty SongAuthorProperty =
            DependencyProperty.Register("SongAuthor", typeof(string), typeof(GroupSelectTab), new PropertyMetadata("SongAuthor"));

        public double SongTitleFontSize
        {
            get { return (double)GetValue(SongTitleFontSizeProperty); }
            set { SetValue(SongTitleFontSizeProperty, value); }
        }
        public static readonly DependencyProperty SongTitleFontSizeProperty =
            DependencyProperty.Register("SongTitleFontSize", typeof(double), typeof(GroupSelectTab), new PropertyMetadata((double)24));

        public double SongAuthorFontSize
        {
            get { return (double)GetValue(SongAuthorFontSizeProperty); }
            set { SetValue(SongAuthorFontSizeProperty, value); }
        }
        public static readonly DependencyProperty SongAuthorFontSizeProperty =
            DependencyProperty.Register("SongAuthorFontSize", typeof(double), typeof(GroupSelectTab), new PropertyMetadata((double)18));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(GroupSelectTab), new PropertyMetadata((double)75));

        public double IconOpactity
        {
            get { return (double)GetValue(IconOpactityProperty); }
            set { SetValue(IconOpactityProperty, value); }
        }
        public static readonly DependencyProperty IconOpactityProperty =
            DependencyProperty.Register("IconOpactity", typeof(double), typeof(GroupSelectTab), new PropertyMetadata((double)1));

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(GroupSelectTab), new PropertyMetadata(new Thickness(0)));

        public Thickness RankImageMargin
        {
            get { return (Thickness)GetValue(RankImageMarginProperty); }
            set { SetValue(RankImageMarginProperty, value); }
        }
        public static readonly DependencyProperty RankImageMarginProperty =
            DependencyProperty.Register("RankImageMargin", typeof(Thickness), typeof(GroupSelectTab), new PropertyMetadata());

        public Brush OverlayColor
        {
            get { return (Brush)GetValue(OverlayColorProperty); }
            set { SetValue(OverlayColorProperty, value); }
        }
        public static readonly DependencyProperty OverlayColorProperty =
            DependencyProperty.Register("OverlayColor", typeof(Brush), typeof(GroupSelectTab), new PropertyMetadata(Brushes.Transparent));

        public object SongSelectTab
        {
            get { return (object)GetValue(SongSelectTabProperty); }
            set { SetValue(SongSelectTabProperty, value); }
        }
        public static readonly DependencyProperty SongSelectTabProperty =
            DependencyProperty.Register("SongSelectTab", typeof(object), typeof(GroupSelectTab), new PropertyMetadata());

        public bool IsExpand
        {
            get { return (bool)GetValue(IsExpandProperty); }
            set
            {
                SetValue(IsExpandProperty, value);
                if (value == true)
                    _DiffInfo.Visibility = Visibility.Visible;
                else
                    _DiffInfo.Visibility = Visibility.Hidden;
            }
        }
        public static readonly DependencyProperty IsExpandProperty =
            DependencyProperty.Register("IsExpand", typeof(bool), typeof(GroupSelectTab), new PropertyMetadata());
        #endregion

        public ToggleButton _Header = new ToggleButton();
        public StackPanel _DiffInfo = new StackPanel();


        public override void OnApplyTemplate()
        {
            _Header = GetTemplateChild<ToggleButton>("headerSite");
            _DiffInfo = GetTemplateChild<StackPanel>("diffInfo");
            _Header.Checked += (s, e) => {
                IsExpand = true;
            };

            AutoFillName();
        }

        private void AutoFillName()
        {
            StackPanel songContainer = SongSelectTab as StackPanel;
            foreach (SongSelectTab item in songContainer.Children)
            {
                item.SongTitle = SongTitle;
                item.SongAuthor = SongAuthor;
            }
        }

        T GetTemplateChild<T>(string name) where T : DependencyObject
        {
            var child = GetTemplateChild(name) as T;
            if (child == null)
                throw new NullReferenceException(name);
            return child;
        }
    }
}
