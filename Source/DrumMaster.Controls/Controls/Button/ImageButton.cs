using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Restless.App.DrumMaster.Controls
{
    public class ImageButton : ButtonBase
    {


        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register
            (
                nameof(ImageSource), typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null)
            );

        public int ImageSize
        {
            get { return (int)GetValue(ImageSizeProperty); }
            set { SetValue(ImageSizeProperty, value); }
        }

        public static readonly DependencyProperty ImageSizeProperty = DependencyProperty.Register
            (
                nameof(ImageSize), typeof(int), typeof(ImageButton), new PropertyMetadata(32)
            );

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
            (
                nameof(Text), typeof(string), typeof(ImageButton), new PropertyMetadata(null)
            );




        public ImageButton()
        {
        }


        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

    }
}
