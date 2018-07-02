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
    ///     <MyNamespace:SongSelectTab/>
    ///
    /// </summary>
    public class SongSelectTab : Control
    {
        static SongSelectTab()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SongSelectTab), new FrameworkPropertyMetadata(typeof(SongSelectTab)));
        }
        #region DependencyProperty
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(SongSelectTab), new PropertyMetadata());

        public ImageSource IconImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        public static readonly DependencyProperty IconImageSourceProperty =
            DependencyProperty.Register("IconImageSource", typeof(ImageSource), typeof(SongSelectTab), new PropertyMetadata());

        public ImageSource RankImageSrouce
        {
            get { return (ImageSource)GetValue(RankImageSrouceProperty); }
            set { SetValue(RankImageSrouceProperty, value); }
        }
        public static readonly DependencyProperty RankImageSrouceProperty =
            DependencyProperty.Register("RankImageSrouce", typeof(ImageSource), typeof(SongSelectTab), new PropertyMetadata());

        public string SongTitle
        {
            get { return (string)GetValue(SongTitleProperty); }
            set { SetValue(SongTitleProperty, value); }
        }
        public static readonly DependencyProperty SongTitleProperty =
            DependencyProperty.Register("SongTitle", typeof(string), typeof(SongSelectTab), new PropertyMetadata("SongTitle"));

        public string SongAuthor
        {
            get { return (string)GetValue(SongAuthorProperty); }
            set { SetValue(SongAuthorProperty, value); }
        }
        public static readonly DependencyProperty SongAuthorProperty =
            DependencyProperty.Register("SongAuthor", typeof(string), typeof(SongSelectTab), new PropertyMetadata("SongAuthor"));

        public string DifficultityName
        {
            get { return (string)GetValue(DifficultityNameProperty); }
            set { SetValue(DifficultityNameProperty, value); }
        }
        public static readonly DependencyProperty DifficultityNameProperty =
            DependencyProperty.Register("DifficultityName", typeof(string), typeof(SongSelectTab), new PropertyMetadata("Diff Name"));

        public double SongTitleFontSize
        {
            get { return (double)GetValue(SongTitleFontSizeProperty); }
            set { SetValue(SongTitleFontSizeProperty, value); }
        }
        public static readonly DependencyProperty SongTitleFontSizeProperty =
            DependencyProperty.Register("SongTitleFontSize", typeof(double), typeof(SongSelectTab), new PropertyMetadata((double)24));

        public string DiffcultityStar
        {
            get { return (string)GetValue(DiffcultityStarProperty); }
            set { SetValue(DiffcultityStarProperty, value); }
        }
        public static readonly DependencyProperty DiffcultityStarProperty =
            DependencyProperty.Register("DiffcultityStar", typeof(string), typeof(SongSelectTab), new PropertyMetadata(5.ToString()));

        public double SongAuthorFontSize
        {
            get { return (double)GetValue(SongAuthorFontSizeProperty); }
            set { SetValue(SongAuthorFontSizeProperty, value); }
        }
        public static readonly DependencyProperty SongAuthorFontSizeProperty =
            DependencyProperty.Register("SongAuthorFontSize", typeof(double), typeof(SongSelectTab), new PropertyMetadata((double)18));

        public double IconOpactity
        {
            get { return (double)GetValue(IconOpactityProperty); }
            set { SetValue(IconOpactityProperty, value); }
        }
        public static readonly DependencyProperty IconOpactityProperty =
            DependencyProperty.Register("IconOpactity", typeof(double), typeof(SongSelectTab), new PropertyMetadata((double)1));

        public double CS
        {
            get { return (double)GetValue(CSProperty); }
            set { SetValue(CSProperty, value); }
        }
        public static readonly DependencyProperty CSProperty =
            DependencyProperty.Register("CS", typeof(double), typeof(SongSelectTab), new PropertyMetadata((double)1));

        public double OD
        {
            get { return (double)GetValue(ODProperty); }
            set { SetValue(ODProperty, value); }
        }
        public static readonly DependencyProperty ODProperty =
            DependencyProperty.Register("OD", typeof(double), typeof(SongSelectTab), new PropertyMetadata((double)1));

        public double AR
        {
            get { return (double)GetValue(ARProperty); }
            set { SetValue(ARProperty, value); }
        }
        public static readonly DependencyProperty ARProperty =
            DependencyProperty.Register("AR", typeof(double), typeof(SongSelectTab), new PropertyMetadata((double)1));

        public double HP
        {
            get { return (double)GetValue(HPProperty); }
            set { SetValue(HPProperty, value); }
        }
        public static readonly DependencyProperty HPProperty =
            DependencyProperty.Register("HP", typeof(double), typeof(SongSelectTab), new PropertyMetadata((double)1));

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(SongSelectTab), new PropertyMetadata(new Thickness(0)));

        public Thickness RankImageMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }
        public static readonly DependencyProperty RankImageMarginProperty =
            DependencyProperty.Register("RankImageMargin", typeof(Thickness), typeof(SongSelectTab), new PropertyMetadata(new Thickness(0)));

        public Brush OverlayColor
        {
            get { return (Brush)GetValue(OverlayColorProperty); }
            set { SetValue(OverlayColorProperty, value); }
        }
        public static readonly DependencyProperty OverlayColorProperty =
            DependencyProperty.Register("OverlayColor", typeof(Brush), typeof(SongSelectTab), new PropertyMetadata(Brushes.Transparent));
        #endregion

        public override void OnApplyTemplate()
        {

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
